using WebAppShared.EF;

namespace WebAppShared.Types;

public interface ITenancyQueryFilter
{
    public int TenantId { get; set; }
}