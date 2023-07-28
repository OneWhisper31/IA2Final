using System.Collections.Generic;

namespace System.Linq
{
    public static class EnumerablePersonalizated //IA2-LINQ
    {
        public static IEnumerable<Src> ConcatInIndex<Src>(this IEnumerable<Src> collection, IEnumerable<Src> collection2, int index)
        => collection.Take(index).Concat(collection2).Concat(collection.Skip(index));

        public static IEnumerable<Src> PrependAppend<Src>(this IEnumerable<Src> collection, Src start, Src end)
        => collection.Prepend(start).Append(end);
    }
}
