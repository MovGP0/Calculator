using System.Collections;
using System.Collections.Generic;

namespace Calculator.Pages
{
    public sealed class PathSampleCollection : ICollection<PathSample>
    {
        public PathSampleCollection()
        {
            Items = new List<PathSample>();
        }

        public PathSampleCollection(int capacity)
        {
            Items = new List<PathSample>(capacity);
        }

        public PathSampleCollection(IEnumerable<PathSample> pathSamples)
        {
            Items = new List<PathSample>(pathSamples);
        }

        private List<PathSample> Items { get; }

        public IEnumerator<PathSample> GetEnumerator()
        {
            return Items.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Add(PathSample item)
        {
            Items.Add(item);
        }

        public void Clear()
        {
            Items.Clear();
        }

        public bool Contains(PathSample item)
        {
            return Items.Contains(item);
        }

        public void CopyTo(PathSample[] array, int arrayIndex)
        {
            Items.CopyTo(array, arrayIndex);
        }

        public bool Remove(PathSample item)
        {
            return Items.Remove(item);
        }

        public int Count => Items.Count;
        public bool IsReadOnly => false;
    }
}