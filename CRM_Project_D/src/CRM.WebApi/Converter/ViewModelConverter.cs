using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
namespace CRM.WebApi.Converter
{
    public static class ViewModelConverter
    {
        private static Dictionary<string, U> ConvertToDict<U, T>(T model)
        {
            var props = model.GetType().GetProperties();
            Dictionary<string, U> result = new Dictionary<string, U>();
            foreach (PropertyInfo propertyInfo in props)
            {
                var value = Value<T, U>(model, propertyInfo.Name);
                result.Add(propertyInfo.Name, value);
            }
            return result;
        }
        public static void ConvertTo<U, T>(this U old, T newobj) where T : class, new()
        {
            if (typeof(U) == typeof(T)) return;
            var dict = ConvertToDict<object, U>(old);
            var props = newobj.GetType().GetProperties();
            foreach (var prop in props)
            {
                try
                {
                    newobj.GetType().GetProperty(prop.Name).SetValue(newobj, dict[prop.Name]);
                }
                catch
                {
                    continue;
                }
            }
        }
        private static U Value<T, U>(T value, string propname)
        {
            return (U)value.GetType().GetProperties().Single(p => p.Name == propname).GetValue(value, null);
        }
    }
}