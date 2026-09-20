using System.Text.Json;
using ATM_PROJECT.Helpers;
using ATM_PROJECT.Models;

namespace ATM_PROJECT.Repositories;

public class AccountRepository : IAccountRepository
{
  private readonly string _dataFolder;
  private readonly string _filePath;

  public AccountRepository()
  {
    _dataFolder = PathHelper.GetDataDirectory();

    _filePath = Path.Combine(_dataFolder, "accounts.json");
  }

  public List<Account> GetAll()
  {
    try
    {
      EnsureFileExists();

      using (StreamReader reader = new StreamReader(_filePath))
      {
        string json = reader.ReadToEnd();

        if (string.IsNullOrWhiteSpace(json))
        {
          return new List<Account>();
        }

        return JsonSerializer.Deserialize<List<Account>>(json) ?? new List<Account>();
      }
    }
    catch (Exception ex)
    {
      Console.WriteLine($"Error reading accounts: {ex.Message}");

      return new List<Account>();
    }
  }

  public Account? GetById(Guid id)
  {
    List<Account> accounts = GetAll();

    return accounts.FirstOrDefault(account => account.Id == id);
  }

  public Account? GetByUserId(Guid userId)
  {
    List<Account> accounts = GetAll();

    return accounts.FirstOrDefault(account => account.UserId == userId);
  }

  public void Add(Account account)
  {
    List<Account> accounts = GetAll();

    accounts.Add(account);

    SaveAll(accounts);
  }

  public void Update(Account account)
  {
    List<Account> accounts = GetAll();

    Account? existingAccount = accounts.FirstOrDefault(a => a.Id == account.Id);

    if (existingAccount is null) return;

    accounts[accounts.IndexOf(existingAccount)] = account;

    SaveAll(accounts);
  }

  private void SaveAll(List<Account> accounts)
  {
    try
    {
      EnsureFileExists();

      string json = JsonSerializer.Serialize(
        accounts,
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
      Console.WriteLine($"Error saving accounts: {ex.Message}");
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
