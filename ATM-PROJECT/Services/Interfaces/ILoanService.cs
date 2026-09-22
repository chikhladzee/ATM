using ATM_PROJECT.Models;

namespace ATM_PROJECT.Services.Interfaces;

public interface ILoanService
{
  LoanRequest CreateLoanRequest(Guid clientId, decimal amount);
  List<LoanRequest> GetByClientId(Guid clientId);
  List<LoanRequest> GetPendingLoans();
  List<LoanRequest> GetAllLoans();
  LoanRequest? GetById(Guid id);
  void ApproveLoan(Guid loanId);
  void RejectLoan(Guid loanId);
}
