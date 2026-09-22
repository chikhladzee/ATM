using ATM_PROJECT.Models;
using ATM_PROJECT.Models.Enums;

namespace ATM_PROJECT.Services.Interfaces;

public interface ITransactionService
{
  void CreateTransaction(Guid userId, TransactionType type, decimal amount);
  List<Transaction> GetByUserId(Guid userId);
}
