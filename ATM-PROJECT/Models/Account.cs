using System.Text.Json.Serialization;

namespace ATM_PROJECT.Models;

public class Account
{
  public Guid Id { get; set; }
  public Guid UserId { get; set; }
  [JsonInclude]
  public decimal Balance { get; private set; }

  public void Deposit(decimal amount)
  {
    if (amount <= 0) throw new ArgumentException("Amount must be greater than zero.");

    Balance += amount;
  }

  public void Withdraw(decimal amount)
  {
    if (amount <= 0) throw new ArgumentException("Amount must be greater than zero.");

    if (amount > Balance) throw new InvalidOperationException("Insufficient balance.");

    Balance -= amount;
  }
}
