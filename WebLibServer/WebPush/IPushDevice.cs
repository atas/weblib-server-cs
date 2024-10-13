namespace WebLibServer.WebPush;

public interface IPushDevice
{
    public int Id { get; set; }
    public string Token { get; set; }
}