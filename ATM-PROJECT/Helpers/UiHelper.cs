using Spectre.Console;

namespace ATM_PROJECT.Helpers;

public static class UiHelper
{
  public static void ShowSuccess(
    string title,
    string message)
  {
    AnsiConsole.WriteLine();

    AnsiConsole.Write(
      new Panel(
          $"[bold green]✔ {Markup.Escape(message)}[/]")
        .Header(
          $"[bold white on green] {Markup.Escape(title)} [/]")
        .Border(BoxBorder.Rounded)
        .BorderColor(Color.Green)
        .Padding(1, 1));

    Pause();
  }

  public static void ShowError(string message)
  {
    AnsiConsole.WriteLine();

    AnsiConsole.Write(
      new Panel(
          $"[bold red]✖ {Markup.Escape(message)}[/]")
        .Header("[bold white on red] ERROR [/]")
        .Border(BoxBorder.Rounded)
        .BorderColor(Color.Red)
        .Padding(1, 1));

    Pause();
  }

  public static void Pause()
  {
    AnsiConsole.WriteLine();

    AnsiConsole.MarkupLine("[grey]Press any key to continue...[/]");

    Console.ReadKey(true);
  }
}
