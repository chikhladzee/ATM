using ATM_PROJECT.Models;

namespace ATM_PROJECT.UI;

public class MainMenu
{
  private readonly ClientMenu _clientMenu;
  private readonly AdminMenu _adminMenu;

  public MainMenu(
    ClientMenu clientMenu,
    AdminMenu adminMenu)
  {
    _clientMenu = clientMenu;
    _adminMenu = adminMenu;
  }

  public void Show(User user)
  {
    if (user is Client client)
    {
      _clientMenu.Show(client);
      return;
    }

    if (user is Admin admin)
    {
      _adminMenu.Show(admin);
    }
  }
}
