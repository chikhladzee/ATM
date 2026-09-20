using ATM_PROJECT.Models.Enums;

namespace ATM_PROJECT.Models;

public class LoanRequest
{
  public Guid Id { get; set; }
  public Guid ClientId { get; set; }
  public decimal Amount { get; set; }
  public LoanStatus Status { get; set; } = LoanStatus.Pending;
}
