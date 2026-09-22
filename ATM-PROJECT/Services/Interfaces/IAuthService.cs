using ATM_PROJECT.Models;

namespace ATM_PROJECT.Services.Interfaces;

public interface IAuthService
{
  Client RegisterClient(string username, string password);
  User? Login(string username, string password);
}
