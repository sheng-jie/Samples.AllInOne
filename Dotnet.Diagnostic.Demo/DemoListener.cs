using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Dotnet.Diagnostic.Demo
{

    public class DiagnosticListenerSubscribe : IObserver<DiagnosticListener>
    {
        public void OnCompleted()
        {
            throw new NotImplementedException();
        }

        public void OnError(Exception error)
        {
            throw new NotImplementedException();
        }

        public void OnNext(DiagnosticListener listener)
        {
            if (listener.Name.Equals(typeof(LogHttpContextMiddleware).FullName))
            {
                listener.Subscribe(new DemoListener());
            }
        }
    }
    public class DemoListener:IObserver<KeyValuePair<string,object>>
    {
        public void OnCompleted()
        {
            throw new NotImplementedException();
        }

        public void OnError(Exception error)
        {
            throw new NotImplementedException();
        }

        public void OnNext(KeyValuePair<string, object> value)
        {
            switch (value.Key)
            {
                case "Dotnet.Diagnostic.Demo.LogHttpContextMiddleware.Start":
                    Console.WriteLine("Start");
                    break;
                case "Dotnet.Diagnostic.Demo.LogHttpContextMiddleware.End":
                    Console.WriteLine("End");
                    break;
                case "Microsoft.AspNetCore.Hosting.HttpRequestIn.Start":
                    Console.WriteLine("Http request entered!");
                    break;
                default:
                    break;
            }
        }
    }
}