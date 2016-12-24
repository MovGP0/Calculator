using System;
using System.Collections.Generic;

namespace Calculator.Common
{
    public static class CollectionExtensions
    {
        public static void AddRange<T>(this ICollection<T> collection, IEnumerable<T> items)
        {
            if (collection == null) throw new ArgumentNullException(nameof(collection));
            if (items == null) return;
            
            // ReSharper disable once CollectionNeverQueried.Local
            if (collection is List<T> list)
            {
                list.AddRange(items);
                return;
            }

            foreach (var item in items)
            {
                collection.Add(item);
            }
        }
    }
}