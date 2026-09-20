namespace ATM_PROJECT.Models;

public class Admin : User
{
  public Admin()
  {
    Role = Enums.UserRole.Admin;
  }

  public override void DisplayMenu()
  {
    Console.WriteLine("Admin Menu");
  }
}
