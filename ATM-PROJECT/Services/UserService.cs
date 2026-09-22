using ATM_PROJECT.Models;
using ATM_PROJECT.Repositories.Interfaces;
using ATM_PROJECT.Services.Interfaces;

namespace ATM_PROJECT.Services;

public class UserService : IUserService
{
  private readonly IUserRepository _userRepository;

  public UserService(IUserRepository userRepository)
  {
    _userRepository = userRepository;
  }

  public UserData? GetById(Guid id)
  {
    return _userRepository.GetById(id);
  }

  public UserData? GetByUsername(string username)
  {
    return _userRepository.GetByUsername(username);
  }

  public bool UsernameExists(string username)
  {
    UserData? user = _userRepository.GetByUsername(username);
    return user is not null;
  }

  public Client CreateClient(string username, string passwordHash)
  {
    if (string.IsNullOrWhiteSpace(username))
    {
      throw new ArgumentException("Username is required.");
    }

    if (string.IsNullOrWhiteSpace(passwordHash))
    {
      throw new ArgumentException("Password hash is required.");
    }

    if (UsernameExists(username))
    {
      throw new InvalidOperationException("Username already exists.");
    }

    Client client = new Client
    {
      Id = Guid.NewGuid(),
      Username = username,
      PasswordHash = passwordHash
    };

    UserData userData = new UserData
    {
      Id = client.Id,
      Username = client.Username,
      PasswordHash = client.PasswordHash,
      Role = client.Role
    };

    _userRepository.Add(userData);

    return client;
  }
}
