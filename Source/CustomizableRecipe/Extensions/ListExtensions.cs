using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomizableRecipe
{
    internal static class ListExtensions
    {
        internal static List<T> MakeListClone<T>(this List<T> list)
        {
            if (list != null)
            {
                return new List<T>(list);
            }

            return null;
        }

    }
}
