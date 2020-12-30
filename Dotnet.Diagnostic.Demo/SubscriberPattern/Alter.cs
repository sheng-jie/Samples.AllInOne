using System;

namespace Dotnet.Diagnostic.Demo.SubscriberPattern
{
    public class Alter : IObserver<Temperature>
    {
        public void OnCompleted()
        {
            Console.WriteLine("du du du !!!");
        }

        public void OnError(Exception error)
        {
            //Nothing to do
        }

        public void OnNext(Temperature value)
        {
            Console.WriteLine($"{value.Date.ToString()}: Current temperature is {value.Degree}.");
        }
    }
}