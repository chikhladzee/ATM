using ATM_PROJECT.Models.Enums;

namespace ATM_PROJECT.Models;

public class Client : User
{
  public Client()
  {
    Role = UserRole.Client;
  }

  public override void DisplayMenu()
  {
    Console.WriteLine("Client menu");
  }
}
