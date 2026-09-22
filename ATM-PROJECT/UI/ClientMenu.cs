using ATM_PROJECT.Helpers;
using ATM_PROJECT.Models;
using ATM_PROJECT.Models.Enums;
using ATM_PROJECT.Services.Interfaces;
using Spectre.Console;

namespace ATM_PROJECT.UI;

public class ClientMenu
{
  private readonly IAccountService _accountService;
  private readonly ILoanService _loanService;
  private readonly ITransactionService _transactionService;

  public ClientMenu(IAccountService accountService, ILoanService loanService, ITransactionService transactionService)
  {
    _accountService = accountService;
    _loanService = loanService;
    _transactionService = transactionService;
  }

  public void Show(Client client)
  {
    while (true)
    {
      AnsiConsole.Clear();

      ShowHeader(client);

      string choice = AnsiConsole.Prompt(
        new SelectionPrompt<string>()
          .Title("[bold cyan]Choose an action:[/]")
          .HighlightStyle(new Style(Color.Aqua))
          .AddChoices(
            "View Balance",
            "Deposit",
            "Withdraw",
            "Request Loan",
            "My Loans",
            "Transaction History",
            "[red]Logout[/]"));

      switch (choice)
      {
        case "View Balance":
          ShowBalance(client);
          break;

        case "Deposit":
          Deposit(client);
          break;

        case "Withdraw":
          Withdraw(client);
          break;

        case "Request Loan":
          RequestLoan(client);
          break;

        case "My Loans":
          ShowMyLoans(client);
          break;

        case "Transaction History":
          ShowTransactionHistory(client);
          break;

        case "[red]Logout[/]":
          return;
      }
    }
  }

  private void ShowHeader(Client client)
  {
    decimal balance = _accountService.GetBalance(client.Id);

    AnsiConsole.Write(new FigletText("ATM").Centered().Color(Color.Aqua));

    AnsiConsole.WriteLine();

    AnsiConsole.Write(
      new Panel(
          $"[bold cyan]👤Client:[/] {Markup.Escape(client.Username)}\n" +
          $"[bold green]💰Balance:[/] ${balance:N2}")
        .Header("[bold aqua]Account[/]")
        .Border(BoxBorder.Ascii)
        .BorderColor(Color.Aqua)
        .Padding(1, 1));

    AnsiConsole.WriteLine();
  }

  private void ShowBalance(Client client)
  {
    try
    {
      decimal balance = _accountService.GetBalance(client.Id);

      AnsiConsole.WriteLine();

      AnsiConsole.Write(
        new Panel(
            $"[bold green]${balance:N2}[/]")
          .Header("[cyan]Current Balance[/]")
          .Border(BoxBorder.Rounded)
          .BorderColor(Color.Green)
          .Padding(2, 1));

      UiHelper.Pause();
    }
    catch (Exception ex)
    {
      UiHelper.ShowError(ex.Message);
    }
  }

  private void Deposit(Client client)
  {
    AnsiConsole.Clear();

    AnsiConsole.Write(new Rule("[green]Deposit Money[/]").RuleStyle("green"));

    AnsiConsole.WriteLine();

    decimal amount = AnsiConsole.Prompt(
      new TextPrompt<decimal>("[cyan]Amount:[/]")
        .Validate(value =>
        {
          if (value <= 0)
            return ValidationResult.Error("[red]✖ Amount must be greater than zero.[/]");

          return ValidationResult.Success();
        }));

    try
    {
      _accountService.Deposit(client.Id, amount);

      AnsiConsole.WriteLine();

      UiHelper.ShowSuccess("Success", "deposited successfully.");
    }
    catch (Exception ex)
    {
      UiHelper.ShowError(ex.Message);
    }
  }

  private void Withdraw(Client client)
  {
    AnsiConsole.Clear();

    AnsiConsole.Write(new Rule("[yellow]Withdraw Money[/]").RuleStyle("yellow"));

    AnsiConsole.WriteLine();

    decimal amount = AnsiConsole.Prompt(
      new TextPrompt<decimal>("[cyan]Amount:[/]")
        .Validate(value =>
        {
          if (value <= 0)
            return ValidationResult.Error("[red]Amount must be greater than zero.[/]");

          return ValidationResult.Success();
        }));

    try
    {
      _accountService.Withdraw(client.Id, amount);

      AnsiConsole.WriteLine();

      AnsiConsole.Write(
        new Panel(
            $"[bold green]${amount:N2} withdrawn successfully.[/]")
          .Header("[green]Success[/]")
          .Border(BoxBorder.Rounded)
          .BorderColor(Color.Green)
          .Padding(1, 1));

      UiHelper.Pause();
    }
    catch (Exception ex)
    {
      UiHelper.ShowError(ex.Message);
    }
  }

  private void RequestLoan(Client client)
  {
    AnsiConsole.Clear();

    AnsiConsole.Write(
      new Rule("[cyan]Request Loan[/]")
        .RuleStyle("cyan"));

    AnsiConsole.WriteLine();

    decimal amount = AnsiConsole.Prompt(
      new TextPrompt<decimal>("[cyan]Loan amount:[/]")
        .Validate(value =>
        {
          if (value <= 0)
          {
            return ValidationResult.Error(
              "[red]Loan amount must be greater than zero.[/]");
          }

          return ValidationResult.Success();
        }));

    try
    {
      LoanRequest loanRequest =
        _loanService.CreateLoanRequest(
          client.Id,
          amount);

      AnsiConsole.WriteLine();

      AnsiConsole.Write(
        new Panel(
            $"[bold green]Loan request created successfully![/]\n\n" +
            $"Amount: [cyan]${loanRequest.Amount:N2}[/]\n" +
            $"Status: [yellow]{loanRequest.Status}[/]")
          .Header("[green]Success[/]")
          .Border(BoxBorder.Rounded)
          .BorderColor(Color.Green)
          .Padding(1, 1));

      UiHelper.Pause();
    }
    catch (Exception ex)
    {
      UiHelper.ShowError(ex.Message);
    }
  }

  private void ShowMyLoans(Client client)
  {
    AnsiConsole.Clear();

    AnsiConsole.Write(
      new Rule("[cyan]My Loans[/]")
        .RuleStyle("cyan"));

    AnsiConsole.WriteLine();

    List<LoanRequest> loans =
      _loanService.GetByClientId(client.Id);

    if (loans.Count == 0)
    {
      AnsiConsole.Write(
        new Panel(
            "[grey]You have no loan requests.[/]")
          .Header("[cyan]My Loans[/]")
          .Border(BoxBorder.Rounded)
          .BorderColor(Color.Grey)
          .Padding(1, 1));

      UiHelper.Pause();

      return;
    }

    Table table = new Table();

    table.Border(TableBorder.Rounded);

    table.AddColumn("[cyan]ID[/]");
    table.AddColumn("[cyan]Amount[/]");
    table.AddColumn("[cyan]Status[/]");

    foreach (LoanRequest loan in loans)
    {
      string status = loan.Status switch
      {
        LoanStatus.Pending => "[yellow]Pending[/]",
        LoanStatus.Approved => "[green]Approved[/]",
        LoanStatus.Rejected => "[red]Rejected[/]",
        _ => "[grey]Unknown[/]"
      };

      table.AddRow(
        loan.Id.ToString(),
        $"${loan.Amount:N2}",
        status);
    }

    AnsiConsole.Write(table);

    UiHelper.Pause();
  }

  private void ShowTransactionHistory(Client client)
  {
    AnsiConsole.Clear();

    AnsiConsole.Write(
      new Rule("[cyan]Transaction History[/]")
        .RuleStyle("cyan"));

    AnsiConsole.WriteLine();

    List<Transaction> transactions =
      _transactionService.GetByUserId(client.Id);

    if (transactions.Count == 0)
    {
      AnsiConsole.Write(
        new Panel(
            "[grey]You don't have any transactions yet.[/]")
          .Header("[cyan]Transaction History[/]")
          .Border(BoxBorder.Rounded)
          .BorderColor(Color.Grey)
          .Padding(1, 1));

      UiHelper.Pause();
      return;
    }

    Table table = new Table();

    table.Border(TableBorder.Rounded);

    table.AddColumn("[cyan]Type[/]");
    table.AddColumn("[cyan]Amount[/]");
    table.AddColumn("[cyan]Date[/]");
    table.AddColumn("[cyan]Time[/]");

    foreach (Transaction transaction in transactions)
    {
      string amountText = transaction.Type switch
      {
        TransactionType.Deposit =>
          $"[green]+${transaction.Amount:N2}[/]",
        TransactionType.Loan =>
          $"[green]+${transaction.Amount:N2}[/]",
        TransactionType.Withdraw =>
          $"[red]-${transaction.Amount:N2}[/]",
        _ =>
          $"${transaction.Amount:N2}"
      };

      table.AddRow(
        transaction.Type.ToString(),
        amountText,
        transaction.CreatedAt.ToString("dd.MM.yyyy"),
        transaction.CreatedAt.ToString("HH:mm"));
    }

    AnsiConsole.Write(table);

    UiHelper.Pause();
  }
}
