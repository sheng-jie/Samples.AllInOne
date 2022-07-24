namespace AsyncLocal.Demo.Tenant;

public class DefaultTenant : ICurrentTenant
{
    private readonly CurrentTenantAccessor _tenantAccessor;

    public DefaultTenant(CurrentTenantAccessor tenantAccessor)
    {
        _tenantAccessor = tenantAccessor;
    }

    public string TenantCode => _tenantAccessor.Current?.TenantCode;
    public string TenantName => _tenantAccessor.Current?.Name;

    public IDisposable Use(string tenantCode, string name = null)
    {
        return SetCurrent(tenantCode, name);
    }

    private IDisposable SetCurrent(string tenantCode, string name)
    {
        var parentScope = _tenantAccessor.Current;
        _tenantAccessor.Current = new BaseTenantInfo(tenantCode, name);
        return new DisposeAction(() => _tenantAccessor.Current = parentScope);
    }
}