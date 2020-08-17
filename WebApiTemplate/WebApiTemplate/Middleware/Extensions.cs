using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace Npgg.Middleware
{
    public static class Extensions
    {
        public static void SetItem<T>(this HttpContext httpContext, T Item)
        {
            var key = typeof(T).FullName;
            httpContext.Items[key] = Item;
        }
        public static void SetItem(this HttpContext httpContext, object Item)
        {
            var key = Item.GetType().FullName;
            httpContext.Items[key] = Item;
        }

        public static T GetItem<T>(this HttpContext httpContext)
        {
            var key = typeof(T).FullName;

            if (httpContext.Items.TryGetValue(key, out var obj) == false)
            {
                return default(T);
            }

            return (T)obj;
        }

        public static bool TryGetValue<T>(this IHeaderDictionary header, string key, out T result)
        {
            result = default(T);

            if (header.TryGetValue(key, out var values) == false)
            {
                return false;
            }

            if (values.Count == 0)
            {
                return false;
            }

            var value = values.First();

            var converter = TypeDescriptor.GetConverter(typeof(T));

            var converted = converter.ConvertFromString(value);

            if(converted is T == false)
            {
                return false;
            }

            result = (T)converted;
            return true;
        }
    }
}
