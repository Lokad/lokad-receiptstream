#region (c)2012 Lokad - New BSD license
// Company: http://www.lokad.com
// This code is released under the terms of the new BSD license
#endregion
using System.IO;

namespace Lokad.ReceiptStream
{
    // .NET 4.0 does not provide a direct access to 7 bits encoded integers. 

    /// <summary>Wrapper to expose <c>Read7BitEncodedInt</c>.</summary>
    public class MyBinaryReader : BinaryReader
    {
        public MyBinaryReader(Stream stream) : base(stream) { }

        public new int Read7BitEncodedInt()
        {
            return base.Read7BitEncodedInt(); // protected method
        }
    }

    /// <summary>Wrapper to expose <c>Writer7BitEncodedInt</c>.</summary>
    public class MyBinaryWriter : BinaryWriter
    {
        public MyBinaryWriter(Stream stream) : base(stream) { }
        public new void Write7BitEncodedInt(int i)
        {
            base.Write7BitEncodedInt(i); // protected method
        }
    }
}
