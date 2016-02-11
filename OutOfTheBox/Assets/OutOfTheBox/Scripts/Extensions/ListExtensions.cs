using System.Collections.Generic;
using System.Linq;

namespace Assets.OutOfTheBox.Scripts.Extensions
{
    public static class ListExtensions
    {
        public static void Resize<T>(this List<T> list, int size, T element = default(T))
        {
            var count = list.Count;
            if (size < count)
            {
                list.RemoveRange(size, count - size);
            }
            else if (size > list.Capacity)
            {
                // Optimization
                list.Capacity = size;
                list.AddRange(Enumerable.Repeat(element, size - count));
            }
        }
    }
}