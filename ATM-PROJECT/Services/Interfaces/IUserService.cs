using ATM_PROJECT.Models;

namespace ATM_PROJECT.Services.Interfaces;

public interface IUserService
{
  UserData? GetById(Guid id);
  UserData? GetByUsername(string username);
  bool UsernameExists(string username);
  Client CreateClient(
    string username,
    string passwordHash);
}
