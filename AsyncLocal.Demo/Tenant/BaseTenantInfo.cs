namespace AsyncLocal.Demo.Tenant;

public class BaseTenantInfo
{
    public string TenantCode { get; set; }
    public string Name { get; set; }

    public BaseTenantInfo(string tenantCode, string name)
    {
        TenantCode = tenantCode;
        Name = name;
    }
}