using System;
using System.Collections.Generic;
using System.Linq;

namespace Emby.WindowsPhone.Model
{
    public class Enum<T>
    {
        public static List<T> GetNames()
        {
            var type = typeof(T);
            if (!type.IsEnum)
            {
                throw new ArgumentException("Type '" + type.Name + "' is not an enum");
            }

            var fields = type.GetFields(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static);

            var items = fields.Select(field => (T) Enum.Parse(typeof (T), field.Name, true)).ToList();

            return items;
        }
    }
}