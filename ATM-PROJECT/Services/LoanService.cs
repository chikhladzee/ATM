using ATM_PROJECT.Models;
using ATM_PROJECT.Models.Enums;
using ATM_PROJECT.Repositories;

namespace ATM_PROJECT.Services;

public class LoanService
{
  private readonly ILoanRepository _loanRepository;
  private readonly AccountService _accountService;

  public LoanService(ILoanRepository loanRepository,
    AccountService accountService)
  {
    _loanRepository = loanRepository;
    _accountService = accountService;
  }

  public LoanRequest CreateLoanRequest(Guid clientId, decimal amount)
  {
    if (amount <= 0)
    {
      throw new ArgumentException("Loan amount must be greater than zero.");
    }

    LoanRequest loanRequest = new LoanRequest
    {
      Id = Guid.NewGuid(),
      ClientId = clientId,
      Amount = amount,
      Status = LoanStatus.Pending
    };

    _loanRepository.Add(loanRequest);

    return loanRequest;
  }

  public List<LoanRequest> GetByClientId(Guid clientId)
  {
    return _loanRepository.GetByClientId(clientId);
  }

  public LoanRequest? GetById(Guid id)
  {
    return _loanRepository.GetById(id);
  }

  public List<LoanRequest> GetPendingLoans()
  {
    List<LoanRequest> loanRequests = _loanRepository.GetAll();

    return loanRequests.Where(loan => loan.Status == LoanStatus.Pending).ToList();
  }

  public List<LoanRequest> GetAllLoans()
  {
    return _loanRepository.GetAll();
  }

  public void ApproveLoan(Guid loanId)
  {
    LoanRequest? loanRequest = _loanRepository.GetById(loanId);

    if (loanRequest is null)
    {
      throw new InvalidOperationException("Loan request not found.");
    }

    if (loanRequest.Status != LoanStatus.Pending)
    {
      throw new InvalidOperationException("Only pending loans can be approved.");
    }

    _accountService.DepositLoan(loanRequest.ClientId, loanRequest.Amount);

    loanRequest.Status = LoanStatus.Approved;

    _loanRepository.Update(loanRequest);
  }

  public void RejectLoan(Guid loanId)
  {
    LoanRequest? loanRequest = _loanRepository.GetById(loanId);

    if (loanRequest is null)
    {
      throw new InvalidOperationException("Loan request not found.");
    }

    if (loanRequest.Status != LoanStatus.Pending)
    {
      throw new InvalidOperationException("Only pending loans can be rejected.");
    }

    loanRequest.Status = LoanStatus.Rejected;

    _loanRepository.Update(loanRequest);
  }
}
