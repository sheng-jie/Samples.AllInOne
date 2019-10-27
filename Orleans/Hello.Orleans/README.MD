![Orleans 3.0 Released](https://upload-images.jianshu.io/upload_images/2799767-cd6a51eba5fc0ac9.png?imageMogr2/auto-orient/strip%7CimageView2/2/w/1240)

# 1. 引言
是的，**Orleans v3.0.0** 已经发布了，并已经完全支持 **.NET Core 3.0**。
所以，Orleans 系列是时候继续了，抱歉，让大家久等了。
万丈高楼平地起，这一节我们就先来了解下Orleans的基本使用。

# 2. 模板项目讲解
![Orleans 核心概念](https://upload-images.jianshu.io/upload_images/2799767-f8c59d53a7c60a8a.png?imageMogr2/auto-orient/strip%7CimageView2/2/w/1240)

在上一篇文章中，我们了解到Orleans 作为.NET 分布式框架，其主要包括三个部分：Client、Grains、Silo Host（Server）。因此，为了方便讲解，创建如下的项目结构进行演示：
![Hello.Orleans 项目结构](https://upload-images.jianshu.io/upload_images/2799767-8721ed10c45fc942.png?imageMogr2/auto-orient/strip%7CimageView2/2/w/1240)

这里有几点需要说明：
1. Orleans.Grains： 类库项目，用于定义Grain的接口以及实现，需要引用`Microsoft.Orleans.CodeGenerator.MSBuild`和`Microsoft.Orleans.Core.Abstractions` NuGet包。
2. Orleans.Server：控制台项目，为 Silo 宿主提供宿主环境，需要引用`Microsoft.Orleans.Server` 和`Microsoft.Extensions.Hosting` NuGet包，以及`Orleans.Grains` 项目。
3. Orleans.Client：控制台项目，用于演示如何借助Orleans Client建立与Orleans Server的连接，需要引用`Microsoft.Orleans.Client` 和`Microsoft.Extensions.Hosting` NuGet包，同时添加`Orleans.Grains`项目引用。

# 3. 第一个Grain
Grain作为Orleans的第一公民，以及Virtual Actor的实际代言人，想吃透Orleans，那Grain就是第一道坎。
先看一个简单的Demo，我们来模拟统计网站的实时在线用户。
在`Orlean s.Grains`添加`ISessionControl`接口，主要用户登录状态的管理。
```
public interface ISessionControlGrain : IGrainWithStringKey
{
    Task Login(string userId);
    Task Logout(string userId);
    Task<int> GetActiveUserCount();
}
```
可以看见Grain的定义很简单，只需要指定继承自IGrain的接口就好。这里面继承自`IGrainWithStringKey`，说明该Grain 的Identity Key(身份标识)为`string`类型。同时需要注意的是
**Grain 的方法申明，返回值必须是： Task、Task<T>、ValueTask<T>**。
紧接着定义`SessionControlGrain`来实现`ISessionControlGrain`接口。
```
public class SessionControlGrain : Grain, ISessionControlGrain
{
    private List<string> LoginUsers { get; set; } = new List<string>();

    public Task Login(string userId)
    {
        //获取当前Grain的身份标识(因为ISessionControlGrain身份标识为string类型，GetPrimaryKeyString()); 
        var appName = this.GetPrimaryKeyString();

        LoginUsers.Add(userId);

        Console.WriteLine($"Current active users count of {appName} is {LoginUsers.Count}");
        return Task.CompletedTask;
    }

    public Task Logout(string userId)
    {
        //获取当前Grain的身份标识
        var appName = this.GetPrimaryKey();
        LoginUsers.Remove(userId);

        Console.WriteLine($"Current active users count of {appName} is {LoginUsers.Count}");
        return Task.CompletedTask;
    }

    public Task<int> GetActiveUserCount()
    {
        return Task.FromResult(LoginUsers.Count);
    }
}
```
实现也很简单，Grain的实现要继承自`Grain`基类。代码中我们定义了一个`List<string>`集合用于保存登录用户。

# 4. 第一个Silo Host（Server）
定义一个Silo用于暴露Grain提供的服务，在`Orleans.Server.Program`中添加以下代码用于启动Silo Host。
```
static Task Main(string[] args)
{
    Console.Title = typeof(Program).Namespace;

    // define the cluster configuration
    return Host.CreateDefaultBuilder()
        .UseOrleans((builder) =>
            {
                builder.UseLocalhostClustering()
                    .AddMemoryGrainStorageAsDefault()
                    .Configure<ClusterOptions>(options =>
                    {
                        options.ClusterId = "Hello.Orleans";
                        options.ServiceId = "Hello.Orleans";
                    })
                    .Configure<EndpointOptions>(options => options.AdvertisedIPAddress = IPAddress.Loopback)
                    .ConfigureApplicationParts(parts =>
                        parts.AddApplicationPart(typeof(ISessionControlGrain).Assembly).WithReferences());
            }
        )
        .ConfigureServices(services =>
        {
            services.Configure<ConsoleLifetimeOptions>(options =>
            {
                options.SuppressStatusMessages = true;
            });
        })
        .ConfigureLogging(builder => { builder.AddConsole(); })
        .RunConsoleAsync();
}
```
1. `Host.CreateDefaultBuilder()`：创建泛型主机提供宿主环境。
2. `UseOrleans`：用来配置Oleans。
3. `UseLocalhostClustering()` ：用于在开发环境下指定连接到本地集群。
4. `Configure<ClusterOptions>`：用于指定连接到那个集群。
5. `Configure<EndpointOptions>`：用于配置silo与silo、silo与client之间的通信端点。开发环境下可仅指定回环地址作为集群间通信的IP地址。
6. `ConfigureApplicationParts()`：用于指定暴露哪些Grain服务。

以上就是开发环境下，Orleans Server的基本配置。对于详细的配置也可以先参考[Orleans Server Configuration](http://dotnet.github.io/orleans/Documentation/clusters_and_clients/configuration_guide/server_configuration.html)。后续也会有专门的一篇文章来详解。

# 5. 第一个Client
客户端的定义也很简单，主要是创建`IClusterClient`对象建立于Orleans Server的连接。因为`IClusterClient`最好能在程序启动之时就建立连接，所以可以通过继承`IHostedService`来实现。
在`Orleans.Client`中定义`ClusterClientHostedService`继承自`IHostedService`。
```
public class ClusterClientHostedService : IHostedService
{
    public IClusterClient Client { get; }

    private readonly ILogger<ClusterClientHostedService> _logger;

    public ClusterClientHostedService(ILogger<ClusterClientHostedService> logger, ILoggerProvider loggerProvider)
    {
        _logger = logger;
        Client = new ClientBuilder()
            .UseLocalhostClustering()
            .Configure<ClusterOptions>(options =>
            {
                options.ClusterId = "Hello.Orleans";
                options.ServiceId = "Hello.Orleans";
            })
            .ConfigureLogging(builder => builder.AddProvider(loggerProvider))
            .Build();
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        var attempt = 0;
        var maxAttempts = 100;
        var delay = TimeSpan.FromSeconds(1);
        return Client.Connect(async error =>
        {
            if (cancellationToken.IsCancellationRequested)
            {
                return false;
            }

            if (++attempt < maxAttempts)
            {
                _logger.LogWarning(error,
                    "Failed to connect to Orleans cluster on attempt {@Attempt} of {@MaxAttempts}.",
                    attempt, maxAttempts);

                try
                {
                    await Task.Delay(delay, cancellationToken);
                }
                catch (OperationCanceledException)
                {
                    return false;
                }

                return true;
            }
            else
            {
                _logger.LogError(error,
                    "Failed to connect to Orleans cluster on attempt {@Attempt} of {@MaxAttempts}.",
                    attempt, maxAttempts);

                return false;
            }
        });
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        try
        {
            await Client.Close();
        }
        catch (OrleansException error)
        {
            _logger.LogWarning(error, "Error while gracefully disconnecting from Orleans cluster. Will ignore and continue to shutdown.");
        }
    }
}
```
代码讲解：
1. 构造函数中通过借助`ClientBuilder()` 来初始化`IClusterClient`。其中`UseLocalhostClustering()`用于连接到开发环境中的localhost 集群。并通过`Configure<ClusterOptions>`指定连接到哪个集群。（需要注意的是，这里的ClusterId必须与Orleans.Server中配置的保持一致。
```
Client = new ClientBuilder()
    .UseLocalhostClustering()
    .Configure<ClusterOptions>(options =>
    {
        options.ClusterId = "Hello.Orleans";
        options.ServiceId = "Hello.Orleans";
    })
    .ConfigureLogging(builder => builder.AddProvider(loggerProvider))
    .Build();
```
2. 在`StartAsync`方法中通过调用`Client.Connect`建立与Orleans Server的连接。同时定义了一个重试机制。

紧接着我们需要将`ClusterClientHostedService`添加到Ioc容器，添加以下代码到`Orleans.Client.Program`中：
```
static Task Main(string[] args)
{
    Console.Title = typeof(Program).Namespace;

    return Host.CreateDefaultBuilder()
        .ConfigureServices(services =>
        {
            services.AddSingleton<ClusterClientHostedService>();
            services.AddSingleton<IHostedService>(_ => _.GetService<ClusterClientHostedService>());
            services.AddSingleton(_ => _.GetService<ClusterClientHostedService>().Client);

            services.AddHostedService<HelloOrleansClientHostedService>();
            services.Configure<ConsoleLifetimeOptions>(options =>
            {
                options.SuppressStatusMessages = true;
            });
        })
        .ConfigureLogging(builder =>
        {
            builder.AddConsole();
        })
        .RunConsoleAsync();
}
```
对于`ClusterClientHostedService`，并没有选择直接通过`services.AddHostedService<T>`的方式注入，是因为我们需要注入该服务中提供的`IClusterClient`（单例），以供其他类去消费。

紧接着，定义一个`HelloOrleansClientHostedService`用来消费定义的`ISessionControlGrain`。
```
public class HelloOrleansClientHostedService : IHostedService
{
    private readonly IClusterClient _client;
    private readonly ILogger<HelloOrleansClientHostedService> _logger;

    public HelloOrleansClientHostedService(IClusterClient client, ILogger<HelloOrleansClientHostedService> logger)
    {
        _client = client;
        _logger = logger;
    }
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        // 模拟控制台终端用户登录
       await MockLogin("Hello.Orleans.Console");
       // 模拟网页终端用户登录
       await MockLogin("Hello.Orleans.Web");
    }

    /// <summary>
    /// 模拟指定应用的登录
    /// </summary>
    /// <param name="appName"></param>
    /// <returns></returns>
    public async Task MockLogin(string appName)
    {
        //假设我们需要支持不同端登录用户，则只需要将项目名称作为身份标识。
        //即可获取一个代表用来维护当前项目登录状态的的单例Grain。
        var sessionControl = _client.GetGrain<ISessionControlGrain>(appName);
        ParallelLoopResult result = Parallel.For(0, 10000, (index) =>
        {
            var userId = $"User-{index}";
            sessionControl.Login(userId);
        });

        if (result.IsCompleted)
        {
            //ParallelLoopResult.IsCompleted 只是返回所有循环创建完毕，并不保证循环的内部任务创建并执行完毕
            //所以，此处手动延迟5秒后再去读取活动用户数。
            await Task.Delay(TimeSpan.FromSeconds(5));
            var activeUserCount = await sessionControl.GetActiveUserCount();

            _logger.LogInformation($"The Active Users Count of {appName} is {activeUserCount}");
        }
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Closed!");

        return Task.CompletedTask; ;
    }
}
```
代码讲解：
这里定义了一个`MockLogin`用于模拟不同终端10000个用户的并发登录。
1. 通过构造函数注入需要的`IClusterClient`。
2. 通过指定Grain接口以及身份标识，就可以通过Client 获取对应的Grain，进而消费Grain中暴露的方法。`var sessionControl = _client.GetGrain<ISessionControlGrain>(appName);` 这里需要注意的是，指定的身份标识为终端应用的名称，那么在整个应用生命周期内，将有且仅有一个代表这个终端应用的Grain。
3. 使用`Parallel.For` 模拟并发
4. `ParallelLoopResult.IsCompleted` 只是返回所有循环任务创建完毕，并不代表循环的内部任务执行完毕。

# 启动第一个 Orleans 应用

先启动`Orleans.Server`：
![Orleans Server Stared](https://upload-images.jianshu.io/upload_images/2799767-73850e6f877a59bf.png?imageMogr2/auto-orient/strip%7CimageView2/2/w/1240)
再启动`Orleans.Client`：
![Orleans Client](https://upload-images.jianshu.io/upload_images/2799767-2d70db96a1e29fd8.png?imageMogr2/auto-orient/strip%7CimageView2/2/w/1240)

![Orleans Server log](https://upload-images.jianshu.io/upload_images/2799767-8f7598f5643d37a8.png?imageMogr2/auto-orient/strip%7CimageView2/2/w/1240)

从上面的运行结果来看，模拟两个终端10000个用户的并发登录，最终输出的活动用户数量均为10000个。
回顾整个实现，并没有用到诸如锁、并发集合等避免并发导致的线程安全问题，但却输出正确的期望结果，这就正好说明了Orleans强大的并发控制特性。

```
public class SessionControlGrain : Grain, ISessionControlGrain
{
    // 未使用并发集合
    private List<string> LoginUsers { get; set; } = new List<string>();

    public Task Login(string userId)
    {
        //获取当前Grain的身份标识(因为ISessionControlGrain身份标识为string类型，GetPrimaryKeyString());
        var appName = this.GetPrimaryKeyString();
        
        LoginUsers.Add(userId);//未加锁

        Console.WriteLine($"Current active users count of {appName} is {LoginUsers.Count}");
        return Task.CompletedTask;
    }
    ....
}
```

# 小结
通过简单的演示，想必你对Orleans的编程实现有了基本的认知，并体会到其并发控制的强大之处。
这只是简单的入门演练，Orleans很多强大的特性，后续再结合具体场景进行详细阐述。
源码已上传至GitHub：[Hello.Orleans](https://github.com/sheng-jie/Samples.AllInOne/tree/dev/Orleans/Hello.Orleans)