using ATM_PROJECT.Models.Enums;

namespace ATM_PROJECT.Models;

public class Transaction
{
  public Guid Id { get; set; }
  public Guid UserId { get; set; }
  public TransactionType Type { get; set; }
  public decimal Amount { get; set; }
  public DateTime CreatedAt { get; set; }
}
