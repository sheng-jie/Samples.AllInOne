namespace AsyncLocal.Demo.Tenant;

public class CurrentTenantAccessor
{
    private readonly AsyncLocal<BaseTenantInfo> _currentScope;

    public CurrentTenantAccessor()
    {
        _currentScope = new AsyncLocal<BaseTenantInfo>();
    }

    public BaseTenantInfo Current
    {
        get => _currentScope.Value;
        set => _currentScope.Value = value;
    }
}