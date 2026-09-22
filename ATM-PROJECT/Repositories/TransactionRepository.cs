using System.Text.Json;
using ATM_PROJECT.Helpers;
using ATM_PROJECT.Models;
using ATM_PROJECT.Repositories.Interfaces;

namespace ATM_PROJECT.Repositories;

public class TransactionRepository : ITransactionRepository
{
  private readonly string _dataFolder;
  private readonly string _filePath;

  public TransactionRepository()
  {
    _dataFolder = PathHelper.GetDataDirectory();

    _filePath = Path.Combine(_dataFolder, "transactions.json");
  }

  public List<Transaction> GetAll()
  {
    try
    {
      EnsureFileExists();

      using (StreamReader reader = new StreamReader(_filePath))
      {
        string json = reader.ReadToEnd();

        if (string.IsNullOrWhiteSpace(json)) return new List<Transaction>();

        return JsonSerializer.Deserialize<List<Transaction>>(json) ?? new List<Transaction>();
      }
    }
    catch (Exception ex)
    {
      Console.WriteLine($"Error reading transactions: {ex.Message}");

      return new List<Transaction>();
    }
  }

  public Transaction? GetById(Guid id)
  {
    List<Transaction> transactions = GetAll();

    return transactions.FirstOrDefault(transaction => transaction.Id == id);
  }

  public List<Transaction> GetByUserId(Guid userId)
  {
    List<Transaction> transactions = GetAll();

    return transactions
      .Where(transaction => transaction.UserId == userId)
      .ToList();
  }

  public void Add(Transaction transaction)
  {
    List<Transaction> transactions = GetAll();

    transactions.Add(transaction);

    SaveAll(transactions);
  }

  private void SaveAll(List<Transaction> transactions)
  {
    try
    {
      EnsureFileExists();

      string json = JsonSerializer.Serialize(
        transactions,
        new JsonSerializerOptions
        {
          WriteIndented = true
        });

      using (StreamWriter writer = new StreamWriter(_filePath))
      {
        writer.Write(json);
      }
    }
    catch (Exception ex)
    {
      Console.WriteLine($"Error saving transactions: {ex.Message}");
    }
  }

  private void EnsureFileExists()
  {
    if (!Directory.Exists(_dataFolder))
    {
      Directory.CreateDirectory(_dataFolder);
    }

    if (!File.Exists(_filePath))
    {
      using (StreamWriter writer =
             new StreamWriter(_filePath))
      {
        writer.Write("[]");
      }
    }
  }
}
