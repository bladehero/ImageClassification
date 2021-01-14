using System;
using System.Collections.Generic;
using System.Linq;

namespace ImageClassification.Preparation.Extensions
{
    public static class EnumerableExtensions
    {
        /// <summary>
        /// Helper method for chunking list into list of smaller lists.
        /// </summary>
        /// <typeparam name="T">Type of source element.</typeparam>
        /// <param name="source">Source collection.</param>
        /// <param name="chunkSize">Chunk size</param>
        /// <returns>Collection of smaller lists.</returns>
        public static IEnumerable<IEnumerable<T>> ChunkBy<T>(this IEnumerable<T> source, int chunkSize)
        {
            if (source is null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            return source.Select((x, i) => new { Index = i, Value = x })
                         .GroupBy(x => x.Index / chunkSize)
                         .Select(x => x.Select(v => v.Value).ToList())
                         .ToList();
        }

        /// <summary>
        /// Helper method for enumerating collection using usual foreach loop.
        /// </summary>
        /// <typeparam name="T">Type of source element.</typeparam>
        /// <param name="source">Source collection.</param>
        /// <param name="action">An action to do for each element.</param>
        public static void ForEach<T>(this IEnumerable<T> source, Action<T> action)
        {
            if (action is null)
            {
                throw new ArgumentNullException(nameof(action));
            }

            foreach (T element in source)
                action(element);
        }
    }
}
