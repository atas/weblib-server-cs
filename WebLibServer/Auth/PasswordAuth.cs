/*using System;
using System.Linq;
using System.Threading.Tasks;
using JetBrains.Annotations;
using UnlockFeedApp.Areas.Api.Models;
using UnlockFeedApp.Libs.Emailing;
using UnlockFeedApp.Libs.Users;
using UnlockFeedApp.Models.DbEntities;
using UnlockFeedApp.Models.EmailViewModels;
using UnlockFeedApp.Models.Enums;
using UnlockFeedApp.Models.Extensions;
using UnlockFeedApp.Services.Contexts;
using WebLibServer.Exceptions;
using WebLibServer.Metrics;
using WebLibServer.SharedLogic.Fx;
using WebLibServer.Utils;
using WebLibServer.WebSys.DI;

namespace UnlockFeedApp.Libs.Auth;

[Service(ServiceAttribute.Scopes.Scoped)]
[UsedImplicitly]
public class PasswordAuth(
    IDbProvider dbProvider,
    AuthCommon authCommon,
    EmailerFactory emailerFactory,
    IMetricsSvc metricsSvc,
    FirstLoginFlow firstLoginFlow,
    DefaultLocationCurrencyCx defaultLocationCurrencyCx,
    TenantCx tenantCx,
    UsernameCheckerSvc usernameCheckerSvc)
{
    public async Task<User> TryLoginByEmailPwd(string email, string password)
    {
        await using var db = dbProvider.Create();
        var user = db.Users.FirstOrDefault(u => u.Email == email);

        if (user == null)
        {
            // TODO Collect these metrics at the top level exception catcher
            metricsSvc.Collect("NonExistingUserLoginTry", 1);
            throw new HttpJsonError("User isn't registered or activated.");
        }

        if (user.Password == null)
        {
            metricsSvc.Collect("LoginTryByPwdForOauthUser", 1);
            throw new HttpJsonError("Did you login with Google before?");
        }

        if (user.Password == PasswordUtils.GetHash(password, user.Salt))
        {
            await authCommon.LoginWith(user);
        }
        else
        {
            metricsSvc.Collect("UserLoginPwdFail", 1);
            return null;
        }

        metricsSvc.CollectNamed(AppEvent.UserPwdLoginSuccess, 1);

        return user;
    }

    /// <summary>
    ///     Activates a user from DomainUserRegs table
    /// </summary>
    /// <param name="userRegId"></param>
    /// <returns></returns>
    /// <exception cref="HttpJsonError"></exception>
    public User Activate(int userRegId)
    {
        using var db = dbProvider.Create();

        var userReg = db.UserRegs.SingleOrDefault(ur => ur.Id == userRegId);

        if (userReg == null) throw new HttpJsonError("This user activation is not available.");

        if (userReg.IsUsed) throw new HttpJsonError("This user activation is already used.");

        var user = new User
        {
            Username = userReg.Username,
            Password = userReg.Password,
            Email = userReg.Email,
            Salt = userReg.Salt,
            Secret = StringUtils.RandomString(16),
            Role = !db.Users.Any() ? UserRole.TenantAdmin : UserRole.Default, // First user is admin
            LastOnlineAt = DateTime.UtcNow,
            Currency = defaultLocationCurrencyCx.Get()
        };

        db.Users.Add(user);
        userReg.IsUsed = true;
        db.SaveChanges();

        firstLoginFlow.Run(user.Id);

        metricsSvc.Collect("ActivateUser", 1);

        return user;
    }

    /// <summary>
    ///     When a user registers first time, send an email
    /// </summary>
    private async Task SendActivationEmail(string email, string username, int userRegId, string userRegSecret)
    {
        var emailModel = new ActivationEmailVM
        {
            Username = username,
            ActivationUrl =
                $"https://{tenantCx.Value.GetHostname()}/api/auth/activate?userRegId={userRegId}&secret={userRegSecret}"
        };

        using (var emailer = emailerFactory.GetActivationEmailer())
        {
            await emailer.Send(email, $"Welcome to {tenantCx.Value.Name}", emailModel);
        }

        metricsSvc.Collect("UserActivationEmailSent", 1);
    }

    public async Task<UserReg> Register(UserRegFormRequestModel model)
    {
        Validate(model);

        var pwdSalt = PasswordUtils.GetRandomSalt();

        usernameCheckerSvc.CheckAndThrowIfInvalid(model.Username);

        var userReg = new UserReg
        {
            Username = model.Username.Trim().StripHtmlTags(),
            Email = model.Email.Trim().StripHtmlTags(),
            Password = PasswordUtils.GetHash(model.Password, pwdSalt),
            Salt = pwdSalt,
            Secret = StringUtils.RandomString(16)
        };

        await using var db = dbProvider.Create();
        await db.UserRegs.Where(ur => ur.Email == userReg.Email).DeleteFromQueryAsync();
        db.UserRegs.Add(userReg);
        await db.SaveChangesAsync();

        await SendActivationEmail(userReg.Email, userReg.Username, userReg.Id, userReg.Secret);

        metricsSvc.Collect("UserRegisteredByPwd", 1);

        return userReg;
    }

    private void Validate(UserRegFormRequestModel model)
    {
        using var db = dbProvider.Create();

        if (db.Users.Any(u => u.Email == model.Email))
            throw new HttpJsonError(
                "This email address is in use. If it is yours, try Forgot Password page to reset your password.");

        if (db.Users.Any(u => u.Username == model.Username))
            throw new HttpJsonError(
                "This username address is in use. If it is yours, try Forgot Password page to reset your password.");
    }
}*/