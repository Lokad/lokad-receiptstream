#region (c)2012 Lokad - New BSD license
// Company: http://www.lokad.com
// This code is released under the terms of the new BSD license
#endregion
using System;

namespace Lokad.ReceiptStream.Entities
{
    public class Receipt
    {
        private Item[] _items;

        public DateTime Date { get; set; }

        public int LoyaltyCard { get; set; }

        public ushort Store { get; set; }

        /// <summary>Never returns <c>null</c>, but empty array instead.</summary>
        public Item[] Items
        {
            get
            {
                if(_items == null) return new Item[0];
                return _items;
            } 
            set { _items = value; }
        }
    }
}
