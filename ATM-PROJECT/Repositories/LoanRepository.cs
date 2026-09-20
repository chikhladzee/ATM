using System.Text.Json;
using ATM_PROJECT.Helpers;
using ATM_PROJECT.Models;

namespace ATM_PROJECT.Repositories;

public class LoanRepository : ILoanRepository
{
  private readonly string _dataFolder;
  private readonly string _filePath;

  public LoanRepository()
  {
    _dataFolder = PathHelper.GetDataDirectory();

    _filePath = Path.Combine(
      _dataFolder,
      "loan-requests.json");
  }

  public List<LoanRequest> GetAll()
  {
    try
    {
      EnsureFileExists();

      using (StreamReader reader = new StreamReader(_filePath))
      {
        string json = reader.ReadToEnd();

        if (string.IsNullOrWhiteSpace(json))
        {
          return new List<LoanRequest>();
        }

        return JsonSerializer.Deserialize<List<LoanRequest>>(json) ?? new List<LoanRequest>();
      }
    }
    catch (Exception ex)
    {
      Console.WriteLine($"Error reading loan requests: {ex.Message}");
      return new List<LoanRequest>();
    }
  }

  public LoanRequest? GetById(Guid id)
  {
    List<LoanRequest> loanRequests = GetAll();
    return loanRequests.FirstOrDefault(loan =>
      loan.Id == id);
  }

  public List<LoanRequest> GetByClientId(Guid clientId)
  {
    List<LoanRequest> loanRequests = GetAll();
    return loanRequests
      .Where(loan => loan.ClientId == clientId)
      .ToList();
  }

  public void Add(LoanRequest loanRequest)
  {
    List<LoanRequest> loanRequests = GetAll();
    loanRequests.Add(loanRequest);
    SaveAll(loanRequests);
  }

  public void Update(LoanRequest loanRequest)
  {
    List<LoanRequest> loanRequests = GetAll();
    LoanRequest? existingLoan =
      loanRequests.FirstOrDefault(loan =>
        loan.Id == loanRequest.Id);

    if (existingLoan is null)
    {
            return;
    }

    loanRequests[loanRequests.IndexOf(existingLoan)] = loanRequest;

    SaveAll(loanRequests);
  }

  private void SaveAll(List<LoanRequest> loanRequests)
  {
    try
    {
      EnsureFileExists();

      string json = JsonSerializer.Serialize(
        loanRequests,
        new JsonSerializerOptions()
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
      Console.WriteLine($"Error saving loan requests: {ex.Message}");
    }
  }

  private void EnsureFileExists()
  {
    if (!Directory.Exists(_dataFolder)) {
      Directory.CreateDirectory(_dataFolder);
    }

    if (!File.Exists(_filePath))
    {
      using (StreamWriter writer = new StreamWriter(_filePath))
      {
        writer.Write("[]");
      }
    }
  }
}
