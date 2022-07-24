namespace AsyncLocal.Demo.Tenant;

public interface ICurrentTenant
{
    public string TenantCode { get; }
    public string TenantName { get; }

    IDisposable Use(string tenantCode, string name = null);
}

public class MulitTenantMiddleware : IMiddleware
{
    private readonly ICurrentTenant _currentTenant;

    public MulitTenantMiddleware(ICurrentTenant currentTenant)
    {
        _currentTenant = currentTenant;
    }
    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        if (context.Request.Query.TryGetValue("tenant", out var tenantCode))
        {
           using(_currentTenant.Use(tenantCode))
           {
               await next(context);
           }
        }
        else
        {
            await next(context);
        }
    }
}