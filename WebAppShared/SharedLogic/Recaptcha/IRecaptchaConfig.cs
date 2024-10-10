namespace WebAppShared.SharedLogic.Recaptcha;

public interface IRecaptchaConfig
{
    public string Key { get; }
    public string Secret { get; }
}