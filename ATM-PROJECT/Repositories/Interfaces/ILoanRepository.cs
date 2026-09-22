using ATM_PROJECT.Models;

namespace ATM_PROJECT.Repositories.Interfaces;

public interface ILoanRepository
{
  List<LoanRequest> GetAll();
  LoanRequest? GetById(Guid id);
  List<LoanRequest> GetByClientId(Guid clientId);
  void Add(LoanRequest loanRequest);
  void Update(LoanRequest loanRequest);
}
