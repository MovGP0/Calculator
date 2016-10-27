using System;
using System.Threading.Tasks;
using MemBus;
using MemBus.Subscribing;

namespace Calculator.Messages
{
    public sealed class EventBus : IEventBus
    {
        private IBus Bus { get; }

        public EventBus(IBus bus)
        {
            Bus = bus;
        }

        public void Dispose()
        {
            Bus.Dispose();
        }

        public void Publish(object message)
        {
            Bus.Publish(message);
        }

        public IDisposable Publish<T>(IObservable<T> observable)
        {
            return Bus.Publish(observable);
        }

        public Task PublishAsync(object message)
        {
            return Bus.PublishAsync(message);
        }

        public IDisposable Subscribe<T>(Action<T> subscription)
        {
            return Bus.Subscribe(subscription);
        }

        public IDisposable Subscribe(object subscriber)
        {
            return Bus.Subscribe(subscriber);
        }

        public IDisposable Subscribe<T>(Action<T> subscription, ISubscriptionShaper customization)
        {
            return Bus.Subscribe(subscription, customization);
        }

        public IObservable<T> Observe<T>()
        {
            return Bus.Observe<T>();
        }
    }
}