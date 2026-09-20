using ATM_PROJECT.Models;
using ATM_PROJECT.Models.Enums;

namespace ATM_PROJECT.Services;

public class AuthService
{
  private readonly UserService _userService;
  private readonly AccountService _accountService;

  public AuthService(UserService userService, AccountService accountService)
  {
    _userService = userService;
    _accountService = accountService;
  }

  public Client RegisterClient(string username, string password)
  {
    string passwordHash = BCrypt.Net.BCrypt.HashPassword(password);

    Client client = _userService.CreateClient(username, passwordHash);

    _accountService.CreateAccount(client.Id);

    return client;
  }

  public User? Login(string username, string password)
  {
    UserData? userData = _userService.GetByUsername(username);

    if (userData is null) return null;

    bool passwordIsValid = BCrypt.Net.BCrypt.Verify(password, userData.PasswordHash);

    if (!passwordIsValid) return null;

    if (userData.Role == UserRole.Client)
    {
      return new Client
      {
        Id = userData.Id,
        Username = userData.Username,
        PasswordHash = userData.PasswordHash
      };
    }

    if (userData.Role == UserRole.Admin)
    {
      return new Admin
      {
        Id = userData.Id,
        Username = userData.Username,
        PasswordHash = userData.PasswordHash
      };
    }

    return null;
  }
}
