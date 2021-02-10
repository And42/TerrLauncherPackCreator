using System;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace CommonLibrary.CommonUtils
{
    public static class CollectionUtils
    {
        public static void ForEach<T>(this IEnumerable<T> items, Action<T> action)
        {
            foreach (T item in items)
                action(item);
        }

        public static int IndexOf<T>(this IEnumerable<T> items, T item)
        {
            int i = 0;
            var comparator = EqualityComparer<T>.Default;
            foreach (T it in items)
            {
                if (comparator.Equals(it, item))
                    return i;
                i++;
            }

            return -1;
        }

        [NotNull]
        public static TOutput[] ConvertAll<TInput, TOutput>([NotNull] this TInput[] array, [NotNull] Converter<TInput, TOutput> converter)
        {
            return Array.ConvertAll(array, converter);
        }
    }
}
