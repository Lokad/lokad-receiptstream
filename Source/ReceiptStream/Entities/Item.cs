#region (c)2012 Lokad - New BSD license
// Company: http://www.lokad.com
// This code is released under the terms of the new BSD license
#endregion

namespace Lokad.ReceiptStream.Entities
{
    public class Item
    {
        /// <summary>Global Trade Identifier Number.</summary>
        public string GTIN { get; set; }

        /// <summary>Unit price in cents.</summary>
        public int UnitPrice { get; set; }

        /// <summary>Quantity being purchased.</summary>
        public int Quantity { get; set; }

        /// <summary>Total discount in cents.</summary>
        public int Discount { get; set; }

        /// <summary>Per thousands (1/10th of percents). Ex: 196 => 19.6%.</summary>
        public int VatRate { get; set; }
    }
}
