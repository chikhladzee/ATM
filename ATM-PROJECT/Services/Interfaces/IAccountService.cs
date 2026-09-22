using ATM_PROJECT.Models;

namespace ATM_PROJECT.Services.Interfaces;

public interface IAccountService
{
  Account CreateAccount(Guid userId);
  Account? GetByUserId(Guid userId);
  decimal GetBalance(Guid userId);
  void Deposit(Guid userId, decimal amount);
  void Withdraw(Guid userId, decimal amount);
  void DepositLoan(Guid userId, decimal amount);
}
