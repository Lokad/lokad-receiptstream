#region (c)2012 Lokad - New BSD license
// Company: http://www.lokad.com
// This code is released under the terms of the new BSD license
#endregion
using System;
using System.Collections.Generic;

namespace Lokad.ReceiptStream
{
    public static class BinaryHelper
    {
        public static DateTime FromInt32ToDateTime(this int seconds)
        {
            return (new DateTime(2001, 1, 1)).AddSeconds(seconds);
        }

        public static int FromDateTimeToInt32(this DateTime dateTime)
        {
            return (int)dateTime.Subtract(new DateTime(2001, 1, 1)).TotalSeconds;
        }

        public static void Swap<T>(this List<T> list, int index1, int index2)
        {
            var v = list[index1];
            list[index1] = list[index2];
            list[index2] = v;
        }
    }
}
