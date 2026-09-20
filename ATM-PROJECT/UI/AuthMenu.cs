using ATM_PROJECT.Helpers;
using ATM_PROJECT.Models;
using ATM_PROJECT.Services;
using Spectre.Console;

namespace ATM_PROJECT.UI;

public class AuthMenu
{
  private readonly AuthService _authService;

  public AuthMenu(AuthService authService)
  {
    _authService = authService;
  }

  public User? Show()
  {
    while (true)
    {
      AnsiConsole.Clear();

      ShowHeader();

      string choice = AnsiConsole.Prompt(
        new SelectionPrompt<string>()
          .Title("[cyan]What would you like to do?[/]")
          .HighlightStyle(new Style(Color.Aqua))
          .AddChoices(
            "Login",
            "Register",
            "Exit"));

      switch (choice)
      {
        case "Login":
          User? user = Login();

          if (user is not null)
          {
            return user;
          }
          break;

        case "Register":
          Register();
          break;

        case "Exit":
          return null;
      }
    }
  }

  private void ShowHeader()
  {
    AnsiConsole.Write(
      new FigletText("ATM")
        .Centered()
        .Color(Color.Aqua));

    AnsiConsole.Write(
      new Rule("[grey]Secure Banking System[/]")
        .RuleStyle("grey"));

    AnsiConsole.WriteLine();
  }

  private void Register()
  {
    AnsiConsole.Clear();

    AnsiConsole.Write(
      new Rule("[green]Create Account[/]")
        .RuleStyle("green"));

    AnsiConsole.WriteLine();

    string username = AnsiConsole.Ask<string>("[cyan]Username:[/]");

    string password = AnsiConsole.Prompt(new TextPrompt<string>("[cyan]Password:[/]").Secret());

    try
    {
      Client client = _authService.RegisterClient(username, password);

      AnsiConsole.WriteLine();

      AnsiConsole.Write(
        new Panel(
            $"[bold green]Registration successful![/]\n\n" +
            $"Welcome, [cyan]{Markup.Escape(client.Username)}[/]!\n" +
            $"Your account has been created.")
          .Header("[green] Success [/]")
          .Border(BoxBorder.Rounded)
          .BorderColor(Color.Green)
          .Padding(1, 1));

      AnsiConsole.WriteLine();

      AnsiConsole.MarkupLine("[grey]Press any key to continue...[/]");

      Console.ReadKey(true);
    }
    catch (Exception ex)
    {
      AnsiConsole.WriteLine();

      AnsiConsole.Write(
        new Panel(
            $"[bold red]{Markup.Escape(ex.Message)}[/]")
          .Header("[red] Error [/]")
          .Border(BoxBorder.Rounded)
          .BorderColor(Color.Red)
          .Padding(1, 1));

      AnsiConsole.WriteLine();

      AnsiConsole.MarkupLine("[grey]Press any key to continue...[/]");

      Console.ReadKey(true);
    }
  }

  private User? Login()
  {
    AnsiConsole.Clear();

    AnsiConsole.Write(new Rule("[cyan]Login[/]").RuleStyle("cyan"));

    AnsiConsole.WriteLine();

    string username = AnsiConsole.Ask<string>("[cyan]Username:[/]");

    string password = AnsiConsole.Prompt(new TextPrompt<string>("[cyan]Password:[/]").Secret());

    User? user = _authService.Login(username, password);

    if (user is null)
    {
      AnsiConsole.WriteLine();

      UiHelper.ShowError("Invalid username or password.");

      return null;
    }

    AnsiConsole.WriteLine();

    UiHelper.ShowSuccess("Login Successful", $"Welcome, {user.Username}!");

    return user;
  }
}
