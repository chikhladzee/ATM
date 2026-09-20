using ATM_PROJECT.Models;

namespace ATM_PROJECT.Repositories;

public interface IAccountRepository
{
  List<Account> GetAll();
  Account? GetById(Guid id);
  Account? GetByUserId(Guid userId);
  void Add(Account account);
  void Update(Account account);
}
