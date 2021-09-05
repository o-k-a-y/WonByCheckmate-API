using System.Collections.Generic;

namespace API.Extensions {
    public static class IEnumerableExtensions {

        // Since LINQ Reverse() is O(N), we can yield the returned objects in reverse
        public static IEnumerable<T> FastReverse<T>(this IList<T> items) {
            for (int i = items.Count - 1; i >= 0; i--) {
                yield return items[i];
            }
        }
    }
}