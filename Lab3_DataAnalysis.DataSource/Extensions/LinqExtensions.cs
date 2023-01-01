using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lab3_DataAnalysis.DataSource.Extensions
{
    public static class LinqExtensions
    {
        public static IEnumerable<T> ExcludeOrdinally<T>(this IList<T> source, IList<T> exceptSource)
        {
            return source.Where(e => {
                for (int i = 0; i < exceptSource.Count(); i++)
                {
                    if(source.IndexOf(e) == i)
                    {
                        return false;
                    }
                }

                return true;
            });
        }
    }
}
