using System;

namespace Dotnet.Diagnostic.Demo.SubscriberPattern
{
    public class Temperature
    {
        public Temperature(decimal temperature, DateTime date)
        {
            Degree = temperature;
            Date = date;
        }

        public decimal Degree { get; }
        public DateTime Date { get; }
    }
}