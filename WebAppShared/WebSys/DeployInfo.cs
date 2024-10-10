namespace WebAppShared.WebSys;

public static class DeployInfo
{
    public static string GetUiVersion()
    {
        return AppEnv.IsDevelopment ? null : File.ReadAllText("wwwroot/public/build/version.txt").Trim();
    }

    public static string GetAppVersion()
    {
        return AppEnv.IsDevelopment ? null : File.ReadAllText("App_Data/app-version.txt").Trim();
    }
}