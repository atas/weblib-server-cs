namespace WebLibServer.Emailing;

public interface IEmailingSMTPConfig
{
    int Port { get; }
    bool Encryption { get; }
}