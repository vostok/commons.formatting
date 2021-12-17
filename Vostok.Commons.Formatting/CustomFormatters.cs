﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Vostok.Commons.Formatting
{
    internal static class CustomFormatters
    {
        private static readonly Dictionary<Type, Func<object, string>> Formatters
            = new Dictionary<Type, Func<object, string>>
            {
                [typeof(string)] = value => (string)value,
                [typeof(Uri)] = value => value.ToString(),
                [typeof(Enum)] = value => value.ToString(),
                [typeof(Encoding)] = value => ((Encoding)value).WebName,
                [typeof(DateTimeOffset)] = value => ((DateTimeOffset)value).ToString("o")
            };

        public static bool TryFormat(object item, out string s)
        {
            s = null;
            var itemType = item.GetType();

            foreach (var pair in Formatters)
            {
                if (pair.Key.IsAssignableFrom(itemType))
                {
                    s = pair.Value(item);
                    return true;
                }
            }

            return false;
        }
    }
}
