using ATM_PROJECT.Models;
using ATM_PROJECT.Models.Enums;
using ATM_PROJECT.Repositories;

namespace ATM_PROJECT.Services;

public class TransactionService
{
  private readonly ITransactionRepository _transactionRepository;

  public TransactionService(
    ITransactionRepository transactionRepository)
  {
    _transactionRepository = transactionRepository;
  }

  public void CreateTransaction(
    Guid userId,
    TransactionType type,
    decimal amount)
  {
    Transaction transaction = new Transaction
    {
      Id = Guid.NewGuid(),
      UserId = userId,
      Type = type,
      Amount = amount,
      CreatedAt = DateTime.Now
    };

    _transactionRepository.Add(transaction);
  }

  public List<Transaction> GetByUserId(Guid userId)
  {
    List<Transaction> transactions =
      _transactionRepository.GetByUserId(userId);

    return transactions
      .OrderByDescending(
        transaction => transaction.CreatedAt)
      .ToList();
  }
}
