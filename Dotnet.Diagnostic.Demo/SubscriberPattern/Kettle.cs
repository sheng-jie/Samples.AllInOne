using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Dotnet.Diagnostic.Demo.SubscriberPattern
{
    /// <summary>
    /// 热水壶
    /// </summary>
    public class Kettle : IObservable<Temperature>
    {
        private List<IObserver<Temperature>> observers;
        private decimal temperature = 0;

        public decimal Temperature
        {
            get => temperature;
            private set
            {
                temperature = value;
                observers.ForEach(observer => observer.OnNext(new Temperature(temperature, DateTime.Now)));
                
                if (temperature == 100)
                    observers.ForEach(observer => observer.OnCompleted());
            }
        }

        public Kettle()
        {
            observers = new List<IObserver<Temperature>>();
        }

        public IDisposable Subscribe(IObserver<Temperature> observer)
        {
            if (!observers.Contains(observer))
            {
                Console.WriteLine("Subscribed!");
                observers.Add(observer);
            }

            //使用UnSubscriber包装，返回IDisposable对象，用于观察者取消订阅
            return new UnSubscriber<Temperature>(observers, observer);
        }

        /// <summary>
        /// 烧水方法
        /// </summary>
        public async Task StartBoilWaterAsync()
        {
            var random = new Random(DateTime.Now.Millisecond);

            while (Temperature < 100)
            {
                Temperature += 10;
                await Task.Delay(random.Next(5000));
            }

        }
    }
}