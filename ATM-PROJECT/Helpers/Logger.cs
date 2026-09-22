namespace ATM_PROJECT.Helpers;

public static class Logger
{
  private static readonly string _logDirectory = Path.Combine(PathHelper.GetProjectDirectory(), "Logs");
  private static readonly string _logFilePath = Path.Combine(_logDirectory, "atm-log.txt");

  public static void Info(string message)
  {
    Write("INFO", message);
  }

  public static void Warning(string message)
  {
    Write("WARNING", message);
  }

  public static void Error(string message)
  {
    Write("ERROR", message);
  }

  private static void Write(string level, string message)
  {
    try
    {
      if (!Directory.Exists(_logDirectory)) Directory.CreateDirectory(_logDirectory);

      string logMessage = $"[{DateTime.Now:dd.MM.yyyy HH:mm:ss}] [{level}] {message}";

      using (StreamWriter writer = new StreamWriter(_logFilePath, append: true))
      {
        writer.WriteLine(logMessage);
      }
    }
    catch { }
  }
}
