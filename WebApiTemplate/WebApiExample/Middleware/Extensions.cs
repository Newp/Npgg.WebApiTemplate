using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;

namespace Npgg.Middleware
{
    public static class Extensions
    {
        public static void SetItem<T>(this HttpContext httpContext, T Item)
        {
            var key = typeof(T);
            httpContext.Items[key] = Item;
        }

        public static T? GetItem<T>(this HttpContext httpContext)
        {
            var key = typeof(T);

            if (httpContext.Items.TryGetValue(key, out var obj) == false)
            {
                return default;
                //throw new Exception($"context item not found=>{key.FullName}");
            }

            return (T)obj;
        }
        public static T GetRequiredItem<T>(this HttpContext httpContext) where T : notnull
        {
            var key = typeof(T);

            if (httpContext.Items.TryGetValue(key, out var obj) == false || obj == null)
            {
                throw new Exception($"context item not found=>{key.FullName}");
            }

            return (T)obj;
        }

        public static bool TryGetHeader(this HttpContext context, string key, [NotNullWhen(returnValue: true)] out string? result)
        {
            if (context.Request.Headers.TryGetValue(key, out var list) == false || list.Count == 0)
            {
                result = default;
                return false;
            }

            result = list.First();
            return true;
        }


        public static bool TryGetValue<T>(this IHeaderDictionary header, string key, out T? result)
        {
            result = default;

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
