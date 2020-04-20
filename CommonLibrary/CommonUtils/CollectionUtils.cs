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

        [NotNull]
        public static TOutput[] ConvertAll<TInput, TOutput>([NotNull] this TInput[] array, [NotNull] Converter<TInput, TOutput> converter)
        {
            return Array.ConvertAll(array, converter);
        }
    }
}
