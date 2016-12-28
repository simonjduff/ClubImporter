using System;
using System.Collections.Generic;
using System.Linq;

namespace DdhpCore.ClubImporter.Runner.Extensions
{
    public static class EnumerableExtensions
    {
        public static IEnumerable<IEnumerable<T>> Partition<T>(this IEnumerable<T> input, int partitionSize)
        {
            var inputArray = input.ToArray();
            var partitionCount = (int)Math.Ceiling(inputArray.Length / (double)partitionSize);
            var remainder = inputArray.Length % partitionSize;
            var arrays = new T[partitionCount][];

            for (var i = 0; i < partitionCount - 1; i++)
            {
                var segment = new ArraySegment<T>(inputArray, i * partitionSize, partitionSize);
                yield return segment;
            }

            var lastArrayIndex = arrays.Length - 1;
            yield return new ArraySegment<T>(inputArray, lastArrayIndex * partitionSize, remainder);
        }
    }
}