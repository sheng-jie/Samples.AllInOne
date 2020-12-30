![](https://upload-images.jianshu.io/upload_images/2799767-3916ea3cfb7e680f.png?imageMogr2/auto-orient/strip%7CimageView2/2/w/1240)

>Production-Grade Container Orchestration - Automated container deployment, scaling, and management.
生产级别的容器编排系统——自动化的容器部署、扩展和管理。
# 1. 引言
由于最近在学习微服务，所以就基于之前docker的基础上把玩一下k8s（Kubernetes），以了解基本概念和核心功能。

# 2. What's k8s？
k8s涉及到很多基本概念，可以看[十分钟带你理解Kubernetes核心概念](http://www.dockone.io/article/932)快速了解。
下面这张图包含了k8s了核心组成模块：
![K8S cluster](https://upload-images.jianshu.io/upload_images/2799767-3185ff87eb70a48b.png?imageMogr2/auto-orient/strip%7CimageView2/2/w/1240)

这里就简单罗列以下：
1. k8s Master：k8s主节点，主要包括：
* API Server：提供可以用来和集群交互的REST端点。
* Replication Controller：用来创建和复制Pod。
2. Node：k8s节点，可以是虚拟机或物理机。其又包含以下组件：
* Kubelet：是主节点代理。
* Kube-proxy：Service使用其将链接路由到Pod，如上文所述。
* Docker或Rocket：Kubernetes使用的容器技术来创建容器。
3. Pod：用来托管应用程序实例，包含：
* Container：运行的容器
* Volume：共享存储 (卷)
* IP Address：IP 地址
4. Labels：标签，用于给pod打标签
5. Service：服务，由一组相同Label的Pod组成，其用来控制访问Pods的策略。

# 3. 环境准备

梳理完基本概念，我们来动手玩一玩吧。有三种玩法：一种就是跟随k8s官方的[在线实验室](https://kubernetes.io/zh/docs/tutorials/kubernetes-basics/)进行实操；第二种就是基于Docker For Windows 中集成的k8s进行玩耍；第三种就是安装`MiniKube`捣鼓。这里选择第二种进行讲解。

*PS：很多初学者在环境准备阶段遭遇挫折的后就直接放弃了，笔者为了搭建这个k8s环境也耗费了不少时日，其中包含一次重装系统，汗！希望下面的步骤助你k8s之行有个好的开端。*

## 3.1. 在Docker for Windows中启用Kubernetes
首先确保你已安装Docker for Windows。
因为那道墙，在Docker For Windows Client中启用Kubernetes，并没有想象的那么顺利。最后参照这篇文章成功启用：**[为中国用户在 Docker for Mac/Windows 中开启 Kubernetes](https://github.com/AliyunContainerService/k8s-for-docker-desktop/blob/master/README.md)**。
如果安装了最新版本的docker for windows 客户端（v2.0.0.3)，可参考以下步骤：
1. 为 Docker daemon 配置 Docker Hub 的中国官方镜像加速 `https://registry.docker-cn.com`
2. `git clone https://github.com/AliyunContainerService/k8s-for-docker-desktop.git`
3. `cd k8s-for-docker-desktop`
4. `git checkout v2.0.0.2` **（这一步很重要！！！）**
5. Powell shell执行`./load_images.sh`
6. Enable Kubernetes
7. 执行`kubectl cluster-info`，输出以下，表示正常启动。
```
Kubernetes master is running at https://localhost:6445
KubeDNS is running at https://localhost:6445/api/v1/namespaces/kube-system/services/kube-dns:dns/proxy

To further debug and diagnose cluster problems, use 'kubectl cluster-info dump'.
```
*环境搭建成功，你就成功了一半，请再接再厉动手完成以下实验！*
# 4. 运行第一个Pod

## 4.1. 创建初始镜像
**1**：首先我们执行`dotnet new mvc -n K8s.NET.Demo` 创建一个ASP.NET Core Mvc应用`K8s.NET.Demo`
修改`HomeController`如下所示：
```
public class HomeController : Controller {
    public IActionResult Index () {
        var hostname = Dns.GetHostName ();
        ViewBag.HostName = hostname;
        ViewBag.HostIp = Dns.GetHostAddresses (hostname).FirstOrDefault (ip => ip.AddressFamily == AddressFamily.InterNetwork);
        return View ();
    }
    public IActionResult Privacy () {
        return View ();
    }
    public IActionResult CheckHealth () {
        if (new Random ().Next (100) > 50) {
            return Ok ("OK");
        } else {
            return BadRequest ();
        }
    }
    [ResponseCache (Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error () {
        return View (new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
```
修改`Index.cshtml`如下：
```
@{
    ViewData["Title"] = "Home Page";
}
<div class="text-center">
    <h1 class="display-4">Welcome</h1>
    <h1>Host Name：@ViewBag.HostName</h1>
    <h1>Host IP：@ViewBag.HostIp</h1>
    <p>Learn about <a href="https://docs.microsoft.com/aspnet/core">building Web apps with ASP.NET Core</a>.</p>
</div>
```
**2**：然后添加`Dockerfile`:
```
FROM microsoft/dotnet:sdk AS build-env
WORKDIR /app

# Copy csproj and restore as distinct layers
COPY *.csproj ./
RUN dotnet restore

# Copy everything else and build
COPY . ./
RUN dotnet publish -c Release -o out

# Build runtime image
FROM microsoft/dotnet:aspnetcore-runtime
WORKDIR /app
COPY --from=build-env /app/out .
ENTRYPOINT ["dotnet", "K8s.NET.Demo.dll"]
```
**3**：然后执行`docker build -t k8s.net.demo .`构造镜像，构造成功后执行`docker images`即可查看到名为`k8s.net.demo`的镜像。

## 4.2. 创建 pod 描述文件
添加`k8s-web-pod.yaml`文件如下：
```
apiVersion: v1
kind: Pod # 定义Kubernetes资源的类型为Pod
metadata:
  name: k8s-net-pod # 定义资源的名称
  labels: # 为Pod贴上标签，后面会介绍其用处
    app: k8s-net-pod
spec: # 定义资源的状态，对于Pod来说，最重要属性就是containers
  containers: # containers一个数组类型，如果你希望部署多个容器，可以添加多项
    - name: web # 定义本Pod中该容器的名称
      image: k8s.net.demo # 定义Pod启动的容器镜像地址
      imagePullPolicy: IfNotPresent # k8s默认值为Always，总是从远端拉取镜像，通过设置IfNotPresent或Never来使用本地镜像
      ports:
        - containerPort: 80 # 定义容器监听的端口（与Dockerfile中的EXPOSE类似，只是为了提供文档信息）
      livenessProbe: # 存活探针定义
        httpGet:
          path: /Home/CheckHealth # 存活探针请求路径
          port: 80 #存活探针请求端口
```
## 4.3. 使用kubectl create 创建 pod
执行以下命令完成pod的创建：
```
$ kubectl create -f k8s-web-pod.yaml
pod "k8s-web-pod.yaml" created
$ kubectl get pod
NAME           READY     STATUS    RESTARTS   AGE
k8s-net-pod   1/1       Running   0         1m
```

## 4.4. 访问 pod 中运行的容器
要想与 pod 进行通信，可以通过`kubectl port-forward`配置端口转发，来完成。
```
$ kubectl port-forward k8s-net-pod 8090:80
Forwarding from 127.0.0.1:8090 -> 80
Forwarding from [::1]:8090 -> 80
```
浏览器访问[http://localhost:8090/](http://localhost:8090/)，效果如下图所示：
![](https://upload-images.jianshu.io/upload_images/2799767-608fc9f3e538e8fc.png?imageMogr2/auto-orient/strip%7CimageView2/2/w/1240)

至此我们成功跑起了第一个pod。

这时你可能会问，这和我直接用`docker run -d -p 8091:80 k8s.net.demo` 运行一个容器有什么区别呢？并没有看到k8s强大在哪里啊？！
别急，你现在再执行一次`kubectl get pod`，我来告诉你答案。
```
$ kubectl get po
NAME           READY     STATUS    RESTARTS   AGE
k8s-net-pod   1/1       Running   17         1h
```
看到**RESTARTS**列没有，它是用来说明pod重启了多少次。使用docker运行容器，如果容器挂掉，docker是不会负责给你重启容器的。
而在k8s中，只需要配置**存活探针**，k8s就会自动探测容器的运行状态，进行自动重启。而存活探针仅需要在yaml文件中指定`livenessProbe`节点即可。（*PS：/home/checkhealth 使用随机数来模拟容器应用运行状态，当随机数小于50，就返回BadRequest*。）

而这，只是k8s的冰山一角。

# 5. 运行第一个 Service
Pod运行于集群内部，虽然使用`kubect port-forward`可以映射端口在本机访问，但对于外部依旧无法访问，如果需要暴露供外部直接访问，则需要创建 service。

## 5.1. 使用 kubectl expose 创建 service
我们可以通过`kubectl expose pod`直接将当前运行的pod实例暴露出去。
```
$ kubectl expose pod k8s-net-pod --name k8s-net-service --type=NodePort
service "k8s-net-service" exposed
$ kubectl get service
NAME               TYPE        CLUSTER-IP      EXTERNAL-IP   PORT(S)        AGE
k8s-net-service    NodePort    10.98.62.192    <none>        80:30942/TCP   7m
```
如上，它有一个CLUSTER-IP为`10.98.62.192`，因此我们可以在集群内使用`10.98.62.192:80`来访问该服务，如果是在集群外部，可以使用`NodeIP:30942`(节点所在服务器IP)来访问。

## 5.2. 使用 servive 描述文件创建
另外一种方式就是创建描述文件来创建了，添加`k8s-net-service.yaml`文件：
```
apiVersion: v1
kind: Service # 定义Kubernetes资源的类型为Service
metadata:
  name: k8s-net-service # 定义资源的名称
spec:
  selector: # 指定对应的Pod
    app: k8s-net-pod # 指定Pod的标签为k8s-net-pod
  ports:
  - protocol: TCP # 协议类型
    port: 80 # 指定Service访问的端口
    targetPort: 80 # 指定Service转发请求的端口
    nodePort: 30000
  type: NodePort # 指定Service的类型，在这里使用NodePort来对外访问
```
执行`kubectl create -f k8s-net-service.yaml`来创建service。
```
$ kubectl create -f k8s-net-service.yaml
service "k8s-net-service" created
$ kubectl get service
NAME              TYPE        CLUSTER-IP      EXTERNAL-IP   PORT(S)        AGE
k8s-net-service   NodePort    10.98.62.192    <none>        80:30942/TCP   23m
k8s-net-service   NodePort    10.97.110.150   <none>        80:30000/TCP   34s
kubernetes        ClusterIP   10.96.0.1       <none>        443/TCP        1d
```
# 6. 试试 k8s 的自由伸缩
是时候来体验下k8s强大的自动伸缩功能了。k8s中通过创建`ReplicaSet`或`Deployment`来管理 pod，进而完成自动化扩展和管理。
*PS: 也可以使用ReplicaController，但推荐使用ReplicaSet，因为其标签匹配功能更强大。*

## 6.1. 运行第一个 ReplicaSet
首先定义 ReplicaSet 描述文件`k8s-net-replicaset.yaml`：
```
apiVersion: apps/v1beta2 # rs 的版本号为apps/v1beta2
kind: ReplicaSet # 定义Kubernetes资源的类型为ReplicaSet
metadata:
  name: k8s-net-replicaset # 定义资源的名称
spec:
  replicas: 3 # 指定pod实例的个数
  selector: # pod选择器
    matchLabels: # 指定匹配的标签
      app: k8s-net-pod # 指定Pod的标签为k8s-net-pod
  template: # 创建新的pod模板配置
    metadata:
      labels:
        app: k8s-net-pod # 指定使用哪个pod
    spec:
      containers:
      - name: k8s-net-replicaset
        image: k8s.net.demo # 指定使用的镜像 
        imagePullPolicy: IfNotPresent # k8s默认值为Always，总是从远端拉取镜像，通过设置IfNotPresent或Never来使用本地镜像
```
执行以下命令创建 ReplicaSet，并观察自动创建的pod实例。
```
$ kubectl create -f k8s-net-replicaset.yaml
replicaset.apps "k8s-net-replicaset" created
$ kubectl get rs
NAME                 DESIRED   CURRENT   READY     AGE
k8s-net-replicaset   3         3         3         8s
$ kubectl get pod
NAME                       READY     STATUS    RESTARTS   AGE
k8s-net-pod                1/1       Running   61         12h
k8s-net-replicaset-bxw9c   1/1       Running   0          35s
k8s-net-replicaset-k6kf7   1/1       Running   0          35s
$ kubectl delete po k8s-net-replicaset-bxw9c
pod "k8s-net-replicaset-bxw9c" deleted
$ kubectl get po
NAME                       READY     STATUS        RESTARTS   AGE
k8s-net-pod                1/1       Running       61         12h
k8s-net-replicaset-bxw9c   0/1       Terminating   0          2m
k8s-net-replicaset-k6kf7   1/1       Running       0          2m
k8s-net-replicaset-xvb9l   1/1       Running       0          6s
```
从上面看到，`k8s-net-replicaset`以`k8s-net-pod`为模板创建了额外两个pod副本，当我们尝试删除其中一个副本后，再次查看pod列表，replicaset会自动帮我们重新创建一个pod。
那我们尝试把刚创建的`k8s-net-replicaset`暴露为Service，看看实际运行是什么效果吧。依次执行以下命令：
```
$ kubectl expose replicaset k8s-net-replicaset --type=LoadBalancer --port=8091 --target-port=80 --name k8s-net-rs
-service
service "k8s-net-rs-service" exposed
$ kubectl get service
NAME                 TYPE           CLUSTER-IP      EXTERNAL-IP   PORT(S)          AGE
k8s-net-rs-service   LoadBalancer   10.99.134.237   localhost     8091:32641/TCP   8s
k8s-net-service      NodePort       10.104.21.80    <none>        80:30000/TCP     12h
kubernetes           ClusterIP      10.96.0.1       <none>        443/TCP          12h
```

然后浏览器访问[http://localhost:8091/](http://localhost:8091/)，尝试多次刷新浏览器，显示效果如下，我们发现ReplicaSet已帮我们做好了负载均衡。
![负载均衡效果](https://upload-images.jianshu.io/upload_images/2799767-0bb43b1b8f60140c.gif?imageMogr2/auto-orient/strip)


假如现在网站访问量剧增，3个实例任然无法有效支撑，可以不停止应用的情况下做到水平伸缩吗？Of course, Yes!
仅需执行`kubectl scale`命令进行扩展即可。
```
$ kubectl get pod
NAME                       READY     STATUS    RESTARTS   AGE
k8s-net-replicaset-g4n6g   1/1       Running   0          13m
k8s-net-replicaset-lkrf7   1/1       Running   0          13m
k8s-net-replicaset-tf992   1/1       Running   0          13m
$ kubectl scale replicaset k8s-net-replicaset --replicas=6
replicaset.extensions "k8s-net-replicaset" scaled
$ kubectl get pod
NAME                       READY     STATUS              RESTARTS   AGE
k8s-net-replicaset-cz2bs   0/1       ContainerCreating   0          3s
k8s-net-replicaset-g4n6g   1/1       Running             0          13m
k8s-net-replicaset-lkrf7   1/1       Running             0          13m
k8s-net-replicaset-pjl9m   0/1       ContainerCreating   0          3s
k8s-net-replicaset-qpn2l   0/1       ContainerCreating   0          3s
k8s-net-replicaset-tf992   1/1       Running             0          13m
```
从以上的输出可以看，我们一句命令就扩展pod实例到6个，是不是很简单？！

你可能又问了，我现在访问高峰过了，我怎么快速缩放应用呢？啊，和上面一样的，你把`--replicas`参数改小点就是了，就像这样`kubectl scale replicaset k8s-net-replicaset --replicas=3`。

# 7. 最后
本文从使用docker创建image，到使用k8s创建第一个pod，到暴露第一个Service，再到使用ReplicaSet 进行容器伸缩，基本串通了k8s的核心基础概念，从而对k8s有了基础的认知，希望对你的K8S之路有所帮助。

由于篇幅有限，笔者也是初玩，k8s的很多功能并未一一罗列，那就留着下次分享吧。
如果要问我，k8s有什么好书推荐，首推《Kubernetes In Action》，国内已经有中文版了，翻译的不错！
本文示例代码已上传至GitHub： **[K8S.NET.Demo](https://github.com/sheng-jie/K8S.NET.Demo)**。


>参考资料
 [雨夜朦胧 - Kubernetes初探[1]：部署你的第一个ASP.NET Core应用到k8s集群](https://www.cnblogs.com/RainingNight/p/first-aspnetcore-app-in-k8s.html)

![](https://upload-images.jianshu.io/upload_images/2799767-efe4ebcd991746f2.png?imageMogr2/auto-orient/strip%7CimageView2/2/w/1240)
