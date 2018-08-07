﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace InvisibleCollectorLib.Utils
{
    internal static class DictionaryExtensions
    {
        internal static bool EqualsDict<K, V>(this IDictionary<K, V> dict1, IDictionary<K, V> dict2)
        {
            return IcUtils.ReferenceNullableEquals(dict1, dict2) ??
                   dict1.Count == dict2.Count && !dict1.Except(dict2).Any();
        }

        internal static string StringifyDictionary<TKey, TValue>(this IDictionary<TKey, TValue> dictionary)
        {
            return
                $"{{ {string.Join(", ", dictionary.Select(pair => Convert.ToString(pair.Key) + "=" + Convert.ToString(pair.Value)).ToArray())} }}";
        }
    }
}
