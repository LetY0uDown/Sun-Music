using System;
using System.Collections.Generic;

namespace Desktop_Client.Core.Tools.Extensions;

internal static class ListExtensions
{
    internal static List<T> Shuffle<T> (this List<T> list)
    {
        List<T> copy = new(list);

        for (int i = 0; i < list.Count; i++) {
            int first = Random.Shared.Next(list.Count);
            int second = Random.Shared.Next(list.Count);

            (copy[first], copy[second]) = (copy[second], copy[first]);
        }

        return copy;
    }
}