using ATM_PROJECT.Models;

namespace ATM_PROJECT.Repositories.Interfaces;

public interface IUserRepository
{
  List<UserData> GetAll();
  UserData? GetById(Guid id);
  UserData? GetByUsername(string username);
  void Add(UserData user);
  void Update(UserData user);
}
