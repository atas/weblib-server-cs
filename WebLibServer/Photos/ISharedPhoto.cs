namespace WebLibServer.Photos;

public interface ISharedPhoto
{
    Guid Id { get; set; }
    string Ext { get; set; }
    int Height { get; set; }
    int Width { get; set; }
    List<int> Sizes { get; set; }
}
