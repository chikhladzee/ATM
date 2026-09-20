using ATM_PROJECT.Models.Enums;

namespace ATM_PROJECT.Models;

public abstract class User
{
  public Guid Id { get; set; }
  public string Username { get; set; } = string.Empty;
  public string PasswordHash { get; set; } = string.Empty;
  public UserRole Role { get; protected set; }
  public abstract void DisplayMenu();
}
