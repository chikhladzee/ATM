using ATM_PROJECT.Models;

namespace ATM_PROJECT.Repositories.Interfaces;

public interface IAccountRepository
{
  List<Account> GetAll();
  Account? GetById(Guid id);
  Account? GetByUserId(Guid userId);
  void Add(Account account);
  void Update(Account account);
}
