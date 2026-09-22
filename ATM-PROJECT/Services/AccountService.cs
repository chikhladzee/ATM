using ATM_PROJECT.Helpers;
using ATM_PROJECT.Models;
using ATM_PROJECT.Models.Enums;
using ATM_PROJECT.Repositories.Interfaces;
using ATM_PROJECT.Services.Interfaces;

namespace ATM_PROJECT.Services;

public class AccountService : IAccountService
{
  private readonly IAccountRepository _accountRepository;
  private readonly ITransactionService _transactionService;
  private readonly IUserRepository _userRepository;

  public AccountService(IAccountRepository accountRepository, ITransactionService transactionService, IUserRepository userRepository)
  {
    _accountRepository = accountRepository;
    _transactionService = transactionService;
    _userRepository = userRepository;
  }

  public Account CreateAccount(Guid userId)
  {
    Account? existingAccount = _accountRepository.GetByUserId(userId);
    UserData? user = _userRepository.GetById(userId);

    string username = user?.Username ?? userId.ToString();

    if (existingAccount is not null)
    {
      throw new InvalidOperationException("User already has an account.");
    }

    Account account = new Account
    {
      Id = Guid.NewGuid(),
      UserId = userId
    };

    _accountRepository.Add(account);

    Logger.Info($"Account '{account.Id}' created for user '{username}'.");

    return account;
  }

  public Account? GetByUserId(Guid userId)
  {
    return _accountRepository.GetByUserId(userId);
  }

  public decimal GetBalance(Guid userId)
  {
    Account? account = _accountRepository.GetByUserId(userId);

    if (account is null)
    {
      throw new InvalidOperationException("Account not found.");
    }

    return account.Balance;
  }

  public void Deposit(Guid userId, decimal amount)
  {
    Account? account = _accountRepository.GetByUserId(userId);
    UserData? user = _userRepository.GetById(userId);

    string username = user?.Username ?? userId.ToString();

    if (account is null)
    {
      throw new InvalidOperationException("Account not found.");
    }

    account.Deposit(amount);

    _accountRepository.Update(account);

    _transactionService.CreateTransaction(userId, TransactionType.Deposit, amount);

    Logger.Info($"User '{username}' deposited ${amount:N2}.");
  }

  public void Withdraw(Guid userId, decimal amount)
  {
    Account? account = _accountRepository.GetByUserId(userId);
    UserData? user = _userRepository.GetById(userId);

    string username = user?.Username ?? userId.ToString();

    if (account is null)
    {
      throw new InvalidOperationException("Account not found.");
    }

    try
    {
      account.Withdraw(amount);
    }
    catch (ArgumentException)
    {
      Logger.Warning($"User '{username}' attempted to withdraw invalid amount ${amount:N2}.");
      throw;
    }
    catch (InvalidOperationException)
    {
      Logger.Warning($"User '{username}' attempted to withdraw ${amount:N2} with insufficient balance.");
      throw;
    }

    _accountRepository.Update(account);

    _transactionService.CreateTransaction(userId, TransactionType.Withdraw, amount);

    Logger.Info($"User '{username}' withdrew ${amount:N2}.");
  }

  public void DepositLoan(Guid userId, decimal amount)
  {
    Account? account = _accountRepository.GetByUserId(userId);
    UserData? user = _userRepository.GetById(userId);

    string username = user?.Username ?? userId.ToString();

    if (account is null)
    {
      throw new InvalidOperationException("Account not found.");
    }

    account.Deposit(amount);

    _accountRepository.Update(account);

    _transactionService.CreateTransaction(userId, TransactionType.Loan, amount);

    Logger.Info($"Loan of ${amount:N2} deposited to user '{username}' account.");
  }
}
