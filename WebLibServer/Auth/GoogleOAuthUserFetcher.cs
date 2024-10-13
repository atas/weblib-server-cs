/*using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Microsoft.AspNetCore.WebUtilities;
using Newtonsoft.Json;
using UnlockFeedApp.Libs.Config.TenantConfigModels;
using UnlockFeedApp.Services.Contexts;
using WebLibServer.Types;

namespace UnlockFeedApp.Libs.Auth.OAuth;

[UsedImplicitly]
public class GoogleOAuthUserFetcher(
    string code,
    Lazy<GoogleConfig> googleConfig,
    IHostnameCx hostname,
    IHttpClientFactory httpClientFactory)
{
    private const string ExchangeTokenEndpoint = "https://oauth2.googleapis.com/token";
    private const string OauthUserInfoEndpoint = "https://www.googleapis.com/oauth2/v2/userinfo";

    private GAccessTokenResponse GAccessToken { get; set; }
    public GUserInfoResponse GUserInfo { get; private set; }

    /// <summary>
    ///     Runs all the OAuth Logic to get the User from Google OAuth
    ///     (1) Exchange the code with access token
    ///     (2) Fetch the user info with access token
    ///     (3) Construct the user object from Google user info
    /// </summary>
    /// <returns></returns>
    public async Task<GUserInfoResponse> GetUser()
    {
        await ExchangeCode();
        await FetchUser();

        return GUserInfo;
    }


    /// <summary>
    ///     Exchange the code with an access token
    /// </summary>
    /// <returns></returns>
    private async Task ExchangeCode()
    {
        var qs = new Dictionary<string, string>
        {
            ["client_id"] = googleConfig.Value.OAuthClientId,
            ["client_secret"] = googleConfig.Value.OAuthClientSecret,
            ["redirect_uri"] = $"https://{hostname.Value}/api/auth/google",
            ["grant_type"] = "authorization_code",
            ["code"] = code
        };

        var url = QueryHelpers.AddQueryString(ExchangeTokenEndpoint, qs);

        var response = await httpClientFactory.CreateClient().PostAsync(url, new StringContent(""));
        response.EnsureSuccessStatusCode();

        GAccessToken =
            JsonConvert.DeserializeObject<GAccessTokenResponse>(await response.Content.ReadAsStringAsync());
    }

    private async Task FetchUser()
    {
        var client = httpClientFactory.CreateClient();
        client.DefaultRequestHeaders.Add("Authorization", "Bearer " + GAccessToken.AccessToken);
        var response = await client.GetStringAsync(OauthUserInfoEndpoint);
        GUserInfo = JsonConvert.DeserializeObject<GUserInfoResponse>(response);
    }

    private class GAccessTokenResponse
    {
        [JsonProperty("access_token")] public string AccessToken { get; set; }

        [JsonProperty("expires_in")] public int ExpiresIn { get; set; }

        [JsonProperty("token_type")] public string TokenType { get; set; }

        [JsonProperty("id_token")] public string IdToken { get; set; }
    }

    public class GUserInfoResponse
    {
        [JsonProperty("id")] public string Id { get; set; }

        [JsonProperty("email")] public string Email { get; set; }

        [JsonProperty("verified_email")] public bool VerifiedEmail { get; set; }

        [JsonProperty("picture")] public string Picture { get; set; }
    }
}*/