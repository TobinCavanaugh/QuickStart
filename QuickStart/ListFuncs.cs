using System.Collections.Generic;

namespace QuickStart
{
    public static class ListFuncs
    {
        public static void AddUnique(this IList<string> list, string item)
        {
            if (list != null && !list.Contains(item))
            {
                list.Add(item);
            }
        }

        public static void AddUniqueRange(this IList<string> list, string[] items)
        {
            foreach (var item in items)
            {
                list.AddUnique(item);
            }
        }
    }
}