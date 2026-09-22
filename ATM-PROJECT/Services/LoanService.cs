using ATM_PROJECT.Helpers;
using ATM_PROJECT.Models;
using ATM_PROJECT.Models.Enums;
using ATM_PROJECT.Repositories.Interfaces;
using ATM_PROJECT.Services.Interfaces;

namespace ATM_PROJECT.Services;

public class LoanService : ILoanService
{
  private readonly ILoanRepository _loanRepository;
  private readonly IAccountService _accountService;
  private readonly IUserRepository _userRepository;

  public LoanService(ILoanRepository loanRepository, IAccountService accountService, IUserRepository userRepository)
  {
    _loanRepository = loanRepository;
    _accountService = accountService;
    _userRepository = userRepository;
  }

  public LoanRequest CreateLoanRequest(Guid clientId, decimal amount)
  {
    UserData? user = _userRepository.GetById(clientId);

    string username = user?.Username ?? clientId.ToString();

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

    Logger.Info($"User '{username}' requested a loan of ${amount:N2}. Loan ID: '{loanRequest.Id}'.");

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
      Logger.Warning($"Attempt to approve non-existing loan '{loanId}'.");
      throw new InvalidOperationException("Loan request not found.");
    }

    // get username
    UserData? user = _userRepository.GetById(loanRequest.ClientId);
    string username = user?.Username ?? loanRequest.ClientId.ToString();

    if (loanRequest.Status != LoanStatus.Pending)
    {
      Logger.Warning($"Attempt to approve loan '{loanId}' with status '{loanRequest.Status}'.");
      throw new InvalidOperationException("Only pending loans can be approved.");
    }

    _accountService.DepositLoan(loanRequest.ClientId, loanRequest.Amount);

    loanRequest.Status = LoanStatus.Approved;

    _loanRepository.Update(loanRequest);

    Logger.Info($"Loan '{loanRequest.Id}' approved. Client: '{username}', Amount: ${loanRequest.Amount:N2}.");
  }

  public void RejectLoan(Guid loanId)
  {
    LoanRequest? loanRequest = _loanRepository.GetById(loanId);

    if (loanRequest is null)
    {
      Logger.Warning($"Attempt to reject non-existing loan '{loanId}'.");
      throw new InvalidOperationException("Loan request not found.");
    }

    // get username
    UserData? user = _userRepository.GetById(loanRequest.ClientId);
    string username = user?.Username ?? loanRequest.ClientId.ToString();

    if (loanRequest.Status != LoanStatus.Pending)
    {
      Logger.Warning($"Attempt to reject loan '{loanId}' with status '{loanRequest.Status}'.");
      throw new InvalidOperationException("Only pending loans can be rejected.");
    }

    loanRequest.Status = LoanStatus.Rejected;

    _loanRepository.Update(loanRequest);


    Logger.Info($"Loan '{loanRequest.Id}' rejected. Client: '{username}', Amount: ${loanRequest.Amount:N2}.");
  }
}
