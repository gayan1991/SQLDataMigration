using System.Reflection;

namespace Db.Infrastructure.Extensions
{
    public static class ListExtension
    {
        public static List<TSource> NotExists<TSource>(this List<TSource> source, IEnumerable<TSource> source2)
        {
            if (source2 == null || source2.Count() == 0) 
            { 
                return source;
            }

            var pushData = source.Where(x =>
            {
                var objFound = 0;

                source2.Any(y =>
                {
                    var isMatched = true;
                    foreach (var prop in typeof(TSource).GetProperties(BindingFlags.Instance | BindingFlags.Public))
                    {
                        if (prop.PropertyType.IsValueType)
                        {
                            isMatched = prop.GetValue(x)?.ToString() == prop.GetValue(y)?.ToString();

                            if (!isMatched)
                            {
                                break;
                            }
                        }
                    }

                    if (isMatched)
                    {
                        objFound++;
                    }

                    return isMatched;
                });

                return objFound == 0;
            }).ToList();

            return pushData;
        }
    }
}
