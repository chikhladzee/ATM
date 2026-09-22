using ATM_PROJECT.Helpers;
using ATM_PROJECT.Models;
using ATM_PROJECT.Repositories;
using ATM_PROJECT.Repositories.Interfaces;
using ATM_PROJECT.Services;
using ATM_PROJECT.Services.Interfaces;
using ATM_PROJECT.UI;
using Spectre.Console;

IUserRepository userRepository = new UserRepository();
IAccountRepository accountRepository = new AccountRepository();
ILoanRepository loanRepository = new LoanRepository();
ITransactionRepository transactionRepository = new TransactionRepository();

IUserService userService = new UserService(userRepository);
ITransactionService transactionService = new TransactionService(transactionRepository, userRepository);
IAccountService accountService = new AccountService(accountRepository, transactionService, userRepository);
ILoanService loanService = new LoanService(loanRepository, accountService, userRepository);
IAuthService authService = new AuthService(userService, accountService);

AuthMenu authMenu = new AuthMenu(authService);
ClientMenu clientMenu = new ClientMenu(accountService, loanService, transactionService);
AdminMenu adminMenu = new AdminMenu(loanService, userService);
MainMenu mainMenu = new MainMenu(clientMenu, adminMenu);

Logger.Info("ATM application started.");

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

Logger.Info("ATM application stopped.");
