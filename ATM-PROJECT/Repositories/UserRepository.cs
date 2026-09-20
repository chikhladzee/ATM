using System.Text.Json;
using ATM_PROJECT.Helpers;
using ATM_PROJECT.Models;

namespace ATM_PROJECT.Repositories;

public class UserRepository : IUserRepository
{
  private readonly string _dataFolder;
  private readonly string _filePath;

  public UserRepository()
  {
    _dataFolder = PathHelper.GetDataDirectory();

    _filePath =
      Path.Combine(_dataFolder, "users.json");
  }

  public List<UserData> GetAll()
  {
    try
    {
      EnsureFileExists();

      using (StreamReader reader = new StreamReader(_filePath))
      {
        string json = reader.ReadToEnd();

        if (string.IsNullOrWhiteSpace(json))
        {
          return new List<UserData>();
        }

        return JsonSerializer.Deserialize<List<UserData>>(json) ?? new List<UserData>();
      }
    }
    catch (Exception ex)
    {
      Console.WriteLine($"Error reading users: {ex.Message}");
      return new List<UserData>();
    }
  }

  public UserData? GetById(Guid id)
  {
    List<UserData> users = GetAll();
    return users.FirstOrDefault(user => user.Id == id);
  }

  public UserData? GetByUsername(string username)
  {
    List<UserData> users = GetAll();

    return users.FirstOrDefault(user =>
      user.Username.Equals(
        username,
        StringComparison.OrdinalIgnoreCase));
  }

  public void Add(UserData user)
  {
    List<UserData> users = GetAll();

    users.Add(user);

    SaveAll(users);
  }

  public void Update(UserData user)
  {
    List<UserData> users = GetAll();

    UserData? existingUser =
      users.FirstOrDefault(u => u.Id == user.Id);

    if (existingUser is null)
    {
      return;
    }

    existingUser.Username = user.Username;
    existingUser.PasswordHash = user.PasswordHash;
    existingUser.Role = user.Role;

    SaveAll(users);
  }

  private void SaveAll(List<UserData> users)
  {
    try
    {
      EnsureFileExists();

      string json = JsonSerializer.Serialize(
        users,
        new JsonSerializerOptions
        {
          WriteIndented = true
        });

      using (StreamWriter writer = new StreamWriter(_filePath))
      {
        writer.Write(json);
      }
    }
    catch (Exception ex)
    {
      Console.WriteLine($"Error saving users: {ex.Message}");
    }
  }

  private void EnsureFileExists()
  {
    if (!Directory.Exists(_dataFolder))
    {
      Directory.CreateDirectory(_dataFolder);
    }

    if (!File.Exists(_filePath))
    {
      using (StreamWriter writer = new StreamWriter(_filePath))
      {
        writer.Write("[]");
      }
    }
  }
}
