using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace swapi
{
    public static class Extensions
    {
        /// <summary>
        /// Affiche tous les éléments d'une séquence dans la console
        /// </summary>
        public static void Write<T>(this IEnumerable<T> source)
        {
            foreach (var item in source)
            {
                Console.WriteLine(item);
            }
        }

        /// <summary>
        /// Affiche un seul élément (pratique pour chaîner)
        /// </summary>
        public static void ToConsole<T>(this T item)
        {
            Console.WriteLine(item);
        }
    }
}
