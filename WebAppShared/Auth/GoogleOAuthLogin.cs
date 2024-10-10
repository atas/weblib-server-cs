/*using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using JetBrains.Annotations;
using Microsoft.AspNetCore.WebUtilities;
using UnlockFeedApp.Libs.Auth.OAuth;
using UnlockFeedApp.Libs.Config.TenantConfigModels;
using UnlockFeedApp.Models.DbEntities;
using UnlockFeedApp.Models.Enums;
using UnlockFeedApp.Services.Contexts;
using WebAppShared.Metrics;
using WebAppShared.Types;
using WebAppShared.Utils;
using WebAppShared.WebSys.DI;

namespace UnlockFeedApp.Libs.Auth;

[Service]
[UsedImplicitly]
public class GoogleOAuthLogin(
    IDbProvider dbProvider,
    AuthCommon authCommon,
    Lazy<GoogleConfig> googleConfig,
    IHostnameCx hostnameCx,
    IMetricsSvc metricsSvc,
    FirstLoginFlow firstLoginFlow,
    IHttpClientFactory httpClientFactory)
{
    private const string OAuthLoginEndpointV2 = "https://accounts.google.com/o/oauth2/v2/auth";

    private const string Scopes = "https://www.googleapis.com/auth/userinfo.email";

    public string GetOAuthLoginPageUrl(string redir = null)
    {
        var qs = new Dictionary<string, string>
        {
            ["client_id"] = googleConfig.Value.OAuthClientId,
            ["redirect_uri"] = $"https://{hostnameCx}/api/auth/google",
            ["scope"] = Scopes,
            ["response_type"] = "code",
            ["state"] = string.IsNullOrEmpty(redir) ? "" : $"redir={HttpUtility.UrlEncode(redir)}"
        };
        return QueryHelpers.AddQueryString(OAuthLoginEndpointV2, qs);
    }

    public async Task<User> Login(string code)
    {
        var userGenerator = new GoogleOAuthUserFetcher(code, googleConfig, hostnameCx, httpClientFactory);
        var userInfo = await userGenerator.GetUser();

        await using var db = dbProvider.Create();
        var domainUser = db.Users.SingleOrDefault(du => du.Email == userInfo.Email);

        if (domainUser == null)
        {
            domainUser = new User
            {
                // GoogleId = GUserInfo.Id,
                Email = userInfo.Email,
                GoogleId = userInfo.Id,
                Secret = StringUtils.RandomString(16),
                LastOnlineAt = DateTime.UtcNow
            };

            db.Users.Add(domainUser);
            await db.SaveChangesAsync();

            firstLoginFlow.Run(domainUser.Id);
        }

        await authCommon.LoginWith(domainUser);

        metricsSvc.CollectNamed(AppEvent.UserGoogleLogin, 1);

        return domainUser;
    }
}*/