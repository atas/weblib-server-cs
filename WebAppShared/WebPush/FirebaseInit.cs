using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using Microsoft.AspNetCore.Hosting;

namespace WebAppShared.WebPush;

public static class FirebaseInit
{
    public static FirebaseApp DefaultFirebaseApp { get; private set; }

    /// <summary>
    ///     Initialize Firebase application in static context
    /// </summary>
    /// <param name="env"></param>
    /// <param name="credentialsPath">Defaults to App_Data/firebase-admin.json</param>
    public static void Init(IWebHostEnvironment env, string credentialsPath = null)
    {
        var credentials = File.ReadAllText(credentialsPath ?? Path.Combine(env.ContentRootPath, "App_Data/firebase-admin.json"));

        DefaultFirebaseApp = FirebaseApp.Create(new AppOptions
        {
            Credential = GoogleCredential.FromJson(credentials)
        });
    }
}