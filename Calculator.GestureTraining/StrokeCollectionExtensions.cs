using System;
using System.Reactive.Linq;
using System.Windows.Ink;

namespace Calculator.GestureTraining
{
    public static class StrokeCollectionExtensions
    {
        public static IObservable<StrokeCollectionChangedEventArgs> ToStrokesChangedObservable(this StrokeCollection collection)
        {
            if (collection == null)
                return Observable.Never<StrokeCollectionChangedEventArgs>();

            return Observable.FromEvent<StrokeCollectionChangedEventHandler, StrokeCollectionChangedEventArgs>(
                handler =>
                {
                    StrokeCollectionChangedEventHandler eh = (sender, args) => handler(args);
                    return eh;
                },
                eh => collection.StrokesChanged += eh,
                eh => collection.StrokesChanged -= eh);
        }
    }
}
