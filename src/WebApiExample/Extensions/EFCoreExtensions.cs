using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebApiExample
{
    public static class EFCoreExtensions
    {
        public static void SetJsonConverter<Context>(this Context context, ModelBuilder modelBuilder) where Context : DbContext
        {
            var dbsetType = typeof(DbSet<>);

            var list = context.GetType().GetProperties()
                .Select(property => property.PropertyType)
                .Where(property => property.IsGenericType && dbsetType.IsAssignableFrom(property.GetGenericTypeDefinition()))
                .Select(property => property.GetGenericArguments().First())
                .ToArray();

            var valueConverterType = typeof(EFCoreExtensions)
                .GetMethod(nameof(CreateJsonConverter), System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static)
                ?? throw new Exception($"not found {nameof(CreateJsonConverter)}");

            foreach (var type in list)
            {
                var entity = modelBuilder.Entity(type);

                var notPrimitiveMembers = type.GetProperties()
                    .Where(property => property.PropertyType.IsPrimitive == false
                    && property.PropertyType != typeof(DateTime)
                    && property.PropertyType != typeof(string))
                    .ToArray();


                foreach (var property in notPrimitiveMembers)
                {
                    var converterGenerator = valueConverterType.MakeGenericMethod(property.PropertyType);

                    var converter = (ValueConverter)converterGenerator.Invoke(context, null)!;

                    entity.Property(property.Name)?.HasConversion(converter);
                }
            }
        }
        static ValueConverter CreateJsonConverter<T>() => new ValueConverter<T, string>(
                instance => JsonConvert.SerializeObject(instance)
                , json => JsonConvert.DeserializeObject<T>(json)
            );
    }
}