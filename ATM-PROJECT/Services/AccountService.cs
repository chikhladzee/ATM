using ATM_PROJECT.Models;
using ATM_PROJECT.Models.Enums;
using ATM_PROJECT.Repositories.Interfaces;
using ATM_PROJECT.Services.Interfaces;

namespace ATM_PROJECT.Services;

public class AccountService : IAccountService
{
  private readonly IAccountRepository _accountRepository;
  private readonly ITransactionService _transactionService;

  public AccountService(IAccountRepository accountRepository, ITransactionService transactionService)
  {
    _accountRepository = accountRepository;
    _transactionService = transactionService;
  }

  public Account CreateAccount(Guid userId)
  {
    Account? existingAccount = _accountRepository.GetByUserId(userId);

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

    if (account is null)
    {
      throw new InvalidOperationException("Account not found.");
    }

    account.Deposit(amount);

    _accountRepository.Update(account);

    _transactionService.CreateTransaction(userId, TransactionType.Deposit, amount);
  }

  public void Withdraw(Guid userId, decimal amount)
  {
    Account? account = _accountRepository.GetByUserId(userId);

    if (account is null)
    {
      throw new InvalidOperationException("Account not found.");
    }

    account.Withdraw(amount);

    _accountRepository.Update(account);

    _transactionService.CreateTransaction(userId, TransactionType.Withdraw, amount);
  }

  public void DepositLoan(Guid userId, decimal amount)
  {
    Account? account = _accountRepository.GetByUserId(userId);

    if (account is null)
    {
      throw new InvalidOperationException("Account not found.");
    }

    account.Deposit(amount);

    _accountRepository.Update(account);

    _transactionService.CreateTransaction(userId, TransactionType.Loan, amount);
  }
}
