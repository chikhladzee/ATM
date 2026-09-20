using ATM_PROJECT.Helpers;
using ATM_PROJECT.Models;
using ATM_PROJECT.Models.Enums;
using ATM_PROJECT.Services;
using Spectre.Console;

namespace ATM_PROJECT.UI;

public class AdminMenu
{
  private readonly LoanService _loanService;
  private readonly UserService _userService;

  public AdminMenu(LoanService loanService,
    UserService userService)
  {
    _loanService = loanService;
    _userService = userService;
  }

  public void Show(Admin admin)
  {
    while (true)
    {
      AnsiConsole.Clear();

      ShowHeader(admin);

      string choice = AnsiConsole.Prompt(
        new SelectionPrompt<string>()
          .Title("[cyan]Choose an action:[/]")
          .HighlightStyle(new Style(Color.Aqua))
          .AddChoices(
            "Pending Loans",
            "Loan History",
            "Logout"));

      switch (choice)
      {
        case "Pending Loans":
          ShowPendingLoans();
          break;

        case "Loan History":
          ShowLoanHistory();
          break;

        case "Logout":
          return;
      }
    }
  }

  private void ShowHeader(Admin admin)
  {
    AnsiConsole.Write(
      new FigletText("ATM")
        .Centered()
        .Color(Color.Aqua));

    AnsiConsole.Write(
      new Rule(
          $"[red]Admin Panel — {Markup.Escape(admin.Username)}[/]")
        .RuleStyle("red"));

    AnsiConsole.WriteLine();
  }

  private void ShowPendingLoans()
  {
    List<LoanRequest> loans = _loanService.GetPendingLoans();

    AnsiConsole.Clear();

    AnsiConsole.Write(
      new Rule("[yellow]Pending Loans[/]")
        .RuleStyle("yellow"));

    AnsiConsole.WriteLine();

    if (loans.Count == 0)
    {
      AnsiConsole.Write(
        new Panel(
            "[grey]There are no pending loan requests.[/]")
          .Header("[yellow]Pending Loans[/]")
          .Border(BoxBorder.Rounded)
          .BorderColor(Color.Yellow)
          .Padding(1, 1));

      Pause();
      return;
    }

    Table table = new Table();

    table.Border(TableBorder.Rounded);

    table.AddColumn("[cyan]# [/]");
    table.AddColumn("[cyan]Username[/]");
    table.AddColumn("[cyan]Loan ID[/]");
    table.AddColumn("[cyan]Amount[/]");
    table.AddColumn("[cyan]Status[/]");

    for (int i = 0; i < loans.Count; i++)
    {
      LoanRequest loan = loans[i];

      UserData? user = _userService.GetById(loan.ClientId);

      string username = user?.Username ?? "Unknown";

      table.AddRow(
        (i + 1).ToString(),
        Markup.Escape(username),
        loan.Id.ToString()[..8],
        $"${loan.Amount:N2}",
        "[yellow]Pending[/]");
    }

    AnsiConsole.Write(table);

    AnsiConsole.WriteLine();

    string selectedLoanId = AnsiConsole.Prompt(
      new SelectionPrompt<string>()
        .Title("[cyan]Select a loan request:[/]")
        .PageSize(10)
        .AddChoices(
          loans.Select(loan =>
          {
            UserData? user = _userService.GetById(loan.ClientId);

            string username = user?.Username ?? "Unknown";

            return $"{username} | ${loan.Amount:N2} | {loan.Id}";
          })));

    string idPart = selectedLoanId.Split('|').Last().Trim();

    Guid loanId = Guid.Parse(idPart);

    ShowLoanActions(loanId);
  }

  private void ShowLoanHistory()
  {
    List<LoanRequest> loans = _loanService.GetAllLoans();

    AnsiConsole.Clear();

    AnsiConsole.Write(
      new Rule("[cyan]Loan History[/]")
        .RuleStyle("cyan"));

    AnsiConsole.WriteLine();

    if (loans.Count == 0)
    {
      AnsiConsole.Write(
        new Panel(
            "[grey]There are no loan requests yet.[/]")
          .Header("[cyan]Loan History[/]")
          .Border(BoxBorder.Rounded)
          .BorderColor(Color.Grey)
          .Padding(1, 1));

      UiHelper.Pause();

      return;
    }

    Table table = new Table();

    table.Border(TableBorder.Rounded);

    table.AddColumn("[cyan]#[/]");
    table.AddColumn("[cyan]Username[/]");
    table.AddColumn("[cyan]Loan ID[/]");
    table.AddColumn("[cyan]Amount[/]");
    table.AddColumn("[cyan]Status[/]");

    for (int i = 0; i < loans.Count; i++)
    {
      LoanRequest loan = loans[i];

      UserData? user = _userService.GetById(loan.ClientId);

      string username = user?.Username ?? "Unknown";

      string statusText = loan.Status switch
      {
        LoanStatus.Pending => "[yellow]Pending[/]",
        LoanStatus.Approved => "[green]Approved[/]",
        LoanStatus.Rejected => "[red]Rejected[/]",
        _ => loan.Status.ToString()
      };

      table.AddRow(
        (i + 1).ToString(),
        Markup.Escape(username),
        loan.Id.ToString()[..8],
        $"${loan.Amount:N2}",
        statusText);
    }

    AnsiConsole.Write(table);

    AnsiConsole.WriteLine();

    UiHelper.Pause();
  }

  private void ShowLoanActions(Guid loanId)
  {
    LoanRequest? loan = _loanService.GetById(loanId);

    if (loan is null)
    {
      ShowError("Loan request not found.");
      return;
    }

    UserData? user = _userService.GetById(loan.ClientId);

    string username = user?.Username ?? "Unknown";

    AnsiConsole.Clear();

    AnsiConsole.Write(
      new Panel(
          $"[cyan]Client:[/] {Markup.Escape(username)}\n" +
          $"[cyan]Loan ID:[/] {loan.Id}\n" +
          $"[cyan]Amount:[/] ${loan.Amount:N2}\n" +
          $"[cyan]Status:[/] {loan.Status}")
        .Header("[yellow]Loan Details[/]")
        .Border(BoxBorder.Rounded)
        .BorderColor(Color.Yellow)
        .Padding(1, 1));

    AnsiConsole.WriteLine();

    string choice = AnsiConsole.Prompt(
      new SelectionPrompt<string>()
        .Title("[cyan]What do you want to do?[/]")
        .AddChoices(
          "Approve",
          "Reject",
          "Back"));

    switch (choice)
    {
      case "Approve":
        ConfirmApprove(loan, username);
        break;

      case "Reject":
        ConfirmReject(loan, username);
        break;

      case "Back":
        return;
    }
  }

  private void ConfirmApprove(LoanRequest loan, string username)
  {
    bool confirmed = AnsiConsole.Confirm(
    $"Approve [green]${loan.Amount:N2}[/] loan for [cyan]{Markup.Escape(username)}[/]?");

    if (!confirmed) return;

    try
    {
      _loanService.ApproveLoan(loan.Id);

      AnsiConsole.Write(
        new Panel(
            $"[bold green]Loan approved successfully![/]\n\n" +
            $"Client: [cyan]{Markup.Escape(username)}[/]\n" +
            $"Amount: [green]${loan.Amount:N2}[/]")
          .Header("[green]Success[/]")
          .Border(BoxBorder.Rounded)
          .BorderColor(Color.Green)
          .Padding(1, 1));

        Pause();
    }
    catch (Exception ex)
    {
      ShowError(ex.Message);
    }
  }

private void ConfirmReject(LoanRequest loan, string username)
  {
  bool confirmed = AnsiConsole.Confirm(
    $"Reject [red]${loan.Amount:N2}[/] loan for [cyan]{Markup.Escape(username)}[/]?");

    if (!confirmed) return;

    try
    {
      _loanService.RejectLoan(loan.Id);

      AnsiConsole.Write(
        new Panel(
            $"[bold red]Loan rejected successfully![/]\n\n" +
            $"Client: [cyan]{Markup.Escape(username)}[/]\n" +
            $"Amount: [red]${loan.Amount:N2}[/]")
          .Header("[red]Rejected[/]")
          .Border(BoxBorder.Rounded)
          .BorderColor(Color.Red)
          .Padding(1, 1));

      Pause();
    }
    catch (Exception ex)
    {
      ShowError(ex.Message);
    }
  }

  private void ShowError(string message)
  {
    AnsiConsole.Clear();

    AnsiConsole.Write(
      new Panel(
          $"[bold red]{Markup.Escape(message)}[/]")
        .Header("[red]Error[/]")
        .Border(BoxBorder.Rounded)
        .BorderColor(Color.Red)
        .Padding(1, 1));

    Pause();
  }

  private void Pause()
  {
    AnsiConsole.WriteLine();

    AnsiConsole.MarkupLine(
      "[grey]Press any key to continue...[/]");

    Console.ReadKey(true);
  }
}
