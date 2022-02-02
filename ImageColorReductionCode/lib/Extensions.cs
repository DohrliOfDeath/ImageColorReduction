using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Extensions
{
    public static class ExtensionMethods
    {

        /// <summary>
        /// Break a list of items into chunks of a specific size
        /// https://stackoverflow.com/questions/419019/split-list-into-sublists-with-linq
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source">input IEnumerable</param>
        /// <param name="chunksize">how big one chunk is</param>
        /// <returns>IEnuerable of Chunks</returns>
        public static IEnumerable<IEnumerable<T>> Chunk<T>(this IEnumerable<T> source, int chunksize)
        {
            while (source.Any())
            {
                yield return source.Take(chunksize);
                source = source.Skip(chunksize);
            }
        }
        /// <summary>
        /// Shuffling a list
        /// https://stackoverflow.com/questions/19201489/using-linq-to-shuffle-a-deck
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source">input IEnumerable</param>
        /// <returns>IEnumerable of randomized IEnumerable</returns>
        public static IEnumerable<T> Randomize<T>(this IEnumerable<T> source)
        {
            Random rnd = new Random();
            return source.OrderBy((item) => rnd.Next());
        }
    }
}
