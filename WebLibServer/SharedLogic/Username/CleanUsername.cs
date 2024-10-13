namespace WebLibServer.SharedLogic.Username;

public class CleanUsername(string username)
{
    public bool Shorten { get; set; } = true;

    public override string ToString()
    {
        var username1 = username.Trim().Replace(" ", "");

        if (Shorten) username1 = username1.Length > 18 ? username1[..18] : username1;

        return username1;
    }

    public static implicit operator string(CleanUsername a)
    {
        return a.ToString();
    }
}