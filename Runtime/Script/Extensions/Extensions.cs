using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AllArt.SUI.Extensions
{
    public static class Extensions {

        internal static T[] Slice<T>(this T[] source, int start)
        {
            return Slice(source, start, -1);
        }

        internal static T[] Slice<T>(this T[] source, int start, int end)
        {
            if (end < 0)
                end = source.Length;

            var len = end - start;

            var res = new T[len];
            for (var i = 0; i < len; i++) res[i] = source[i + start];
            return res;
        }

    }

}
