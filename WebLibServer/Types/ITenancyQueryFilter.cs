using WebLibServer.EF;

namespace WebLibServer.Types;

public interface ITenancyQueryFilter
{
    public int TenantId { get; set; }
}