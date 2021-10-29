using System.Collections.Generic;

namespace StatusApp.Extensions
{
    public static class CollectionExtensions
    {
        public static bool IsEmtpy<T>(this ICollection<T> e) => e.Count <= 0;
    }
}
