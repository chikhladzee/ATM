namespace ATM_PROJECT.Helpers;

public static class PathHelper
{
  public static string GetProjectDirectory()
  {
    return Directory.GetParent(
        AppContext.BaseDirectory)!
      .Parent!
      .Parent!
      .Parent!
      .FullName;
  }

  public static string GetDataDirectory()
  {
    return Path.Combine(
      GetProjectDirectory(),
      "Data");
  }
}
