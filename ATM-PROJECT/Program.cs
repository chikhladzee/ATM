using ATM_PROJECT.Models;
using ATM_PROJECT.Repositories;
using ATM_PROJECT.Services;
using ATM_PROJECT.UI;
using Spectre.Console;

UserRepository userRepository = new UserRepository();
AccountRepository accountRepository = new AccountRepository();
LoanRepository loanRepository = new LoanRepository();
TransactionRepository transactionRepository = new TransactionRepository();

UserService userService = new UserService(userRepository);
TransactionService transactionService = new TransactionService(transactionRepository);
AccountService accountService = new AccountService(accountRepository, transactionService);
LoanService loanService = new LoanService(loanRepository, accountService);
AuthService authService = new AuthService(userService, accountService);

AuthMenu authMenu = new AuthMenu(authService);
ClientMenu clientMenu = new ClientMenu(accountService, loanService, transactionService);
AdminMenu adminMenu = new AdminMenu(loanService, userService);
MainMenu mainMenu = new MainMenu(clientMenu, adminMenu);

while (true)
{
  User? currentUser = authMenu.Show();

  if (currentUser is null) break;

  mainMenu.Show(currentUser);
}

AnsiConsole.Clear();

AnsiConsole.Write(
  new FigletText("Goodbye")
    .Centered()
    .Color(Color.Aqua));
