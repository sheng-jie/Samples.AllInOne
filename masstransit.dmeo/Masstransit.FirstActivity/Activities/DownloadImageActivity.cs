using MassTransit;

namespace Masstransit.FirstActivity.Activities;

public class DownloadImageActivity : IActivity<DownloadImageArguments, DownloadImageLog>
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IHostEnvironment _environment;
    private readonly ILogger<DownloadImageActivity> _logger;

    public DownloadImageActivity(IHttpClientFactory httpClientFactory, IHostEnvironment environment,
        ILogger<DownloadImageActivity> logger)
    {
        _httpClientFactory = httpClientFactory;
        _environment = environment;
        _logger = logger;
    }

    public async Task<ExecutionResult> Execute(ExecuteContext<DownloadImageArguments> execution)
    {
        string saveFolder = _environment.ContentRootPath;
        DownloadImageArguments args = execution.Arguments;
        string imageSavePath = Path.Combine(saveFolder,"Images",
            args.ImageUri.ToString().Split("/").Last());

        var httpClient = _httpClientFactory.CreateClient();
        var fileBytes = await httpClient.GetByteArrayAsync(args.ImageUri.ToString());
        await File.WriteAllBytesAsync(imageSavePath, fileBytes);

        _logger.LogWarning($"Image downloaded to {imageSavePath}");
        return execution.Completed<DownloadImageLog>(new { SavePath = imageSavePath });
    }

    public Task<CompensationResult> Compensate(CompensateContext<DownloadImageLog> compensation)
    {
        File.Delete(compensation.Log.SavePath);
        _logger.LogWarning($"Image deleted");
        return Task.FromResult(compensation.Compensated());
    }
}