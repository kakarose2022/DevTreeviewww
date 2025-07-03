using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows;
using Point = System.Windows.Point;

namespace System.Collections
{
    public static class IEnumerableExtensions
    {
        public static void AddOrInsert(this IList list, int index, object data)
        {
            if (index < list.Count)
            {
                list.Insert(index, data);
            }
            else
            {
                list.Add(data);
            }
        }

        public static int Count(this IEnumerable enumerable)
        {
            ICollection collection = enumerable as ICollection;
            if (collection != null)
            {
                return collection.Count;
            }

            int counter = 0;
            foreach (var item in enumerable)
            {
                counter++;
            }
            return counter;
        }

        public static object ElementAt(this IEnumerable enumerable, int index)
        {
            IList list = enumerable as IList;
            if (list != null)
            {
                return list[index];
            }

            int counter = 0;
            foreach (var item in enumerable)
            {
                if (counter == index) return item;
                counter++;
            }

            throw new ArgumentOutOfRangeException("A item at the specified index was not found.");
        }

        public static Point ToRelativePostion(this Point point, Point adornedElementPosition)
        {
            return new Point(
                 point.X - adornedElementPosition.X,
                 point.Y - adornedElementPosition.Y
                );
        }

        public static void AddRange<T>(this ObservableCollection<T> ob, IEnumerable<T> collection) where T:class
        {
            foreach (var item in collection)
            {
                ob.Add(item);
            }
        }
    }
}
