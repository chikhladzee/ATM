using ATM_PROJECT.Helpers;
using ATM_PROJECT.Models;
using ATM_PROJECT.Models.Enums;
using ATM_PROJECT.Repositories.Interfaces;
using ATM_PROJECT.Services.Interfaces;

namespace ATM_PROJECT.Services;

public class TransactionService : ITransactionService
{
  private readonly ITransactionRepository _transactionRepository;
  private readonly IUserRepository _userRepository;

  public TransactionService(ITransactionRepository transactionRepository, IUserRepository userRepository)
  {
    _transactionRepository = transactionRepository;
    _userRepository = userRepository;
  }

  public void CreateTransaction(Guid userId, TransactionType type, decimal amount)
  {
    UserData? user = _userRepository.GetById(userId);
    string username = user?.Username ?? userId.ToString();

    Transaction transaction = new Transaction
    {
      Id = Guid.NewGuid(),
      UserId = userId,
      Type = type,
      Amount = amount,
      CreatedAt = DateTime.Now
    };

    _transactionRepository.Add(transaction);

    Logger.Info($"Transaction created. User: '{username}', Type: {type}, Amount: ${amount:N2}, Transaction ID: '{transaction.Id}'.");
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
