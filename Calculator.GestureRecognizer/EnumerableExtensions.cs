using System;
using System.Collections.Generic;

namespace Calculator.GestureRecognizer
{
    public static class EnumerableExtensions
    {   
        public static void Dispose(this IEnumerable<IDisposable> list)
        {
            if(list == null) return;

            foreach (var item in list)
            {
                item?.Dispose();
            }
        }
    }
}