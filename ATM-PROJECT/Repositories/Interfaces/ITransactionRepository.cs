using ATM_PROJECT.Models;

namespace ATM_PROJECT.Repositories.Interfaces;

public interface ITransactionRepository
{
  List<Transaction> GetAll();
  Transaction? GetById(Guid id);
  List<Transaction> GetByUserId(Guid userId);
  void Add(Transaction transaction);
}
