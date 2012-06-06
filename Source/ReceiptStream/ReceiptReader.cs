#region (c)2012 Lokad - New BSD license
// Company: http://www.lokad.com
// This code is released under the terms of the new BSD license
#endregion
using System;
using System.Collections.Generic;
using System.IO;
using Lokad.ReceiptStream.Entities;

namespace Lokad.ReceiptStream
{
    /// <summary>Outputs a very compact stream representing 
    /// the receipts (lossless compression).</summary>
    public class ReceiptReader
    {
        private Stream _stream;

        // Value lookup tables
        private List<string> _gtins;
        private List<int> _prices;
        private List<int> _discounts;
        private List<int> _vatRates;

        public ReceiptReader(Stream stream)
        {
            _stream = stream;

            _gtins = new List<string>();
            _prices = new List<int>();
            _discounts = new List<int>();
            _vatRates = new List<int>();
        }

        public Receipt Read()
        {
            var reader = new MyBinaryReader(_stream);

            // Reading the lookup updates

            int updateCount;
            try
            {
                updateCount = reader.Read7BitEncodedInt();
            } 
            catch(EndOfStreamException)
            {
                return null;
            }

            for(int i = 0; i < updateCount; i++)
            {
                var flag = reader.Read7BitEncodedInt();

                var isNewValue = ((flag & 1) == 1);

                var valueType = flag >> 1; // discarding 1 bit

                switch (valueType)
                {
                    case 0: // GTINS
                        if(isNewValue)
                        {
                            _gtins.Add(reader.ReadString());
                        }
                        else
                        {
                            var oldIndex = reader.Read7BitEncodedInt();
                            var newIndex = reader.Read7BitEncodedInt();
                            _gtins.Swap(oldIndex, newIndex);
                        }
                        
                        break;

                    case 1: // Prices
                        if(isNewValue)
                        {
                            _prices.Add(reader.ReadInt32());
                        }
                        else
                        {
                            var oldIndex = reader.Read7BitEncodedInt();
                            var newIndex = reader.Read7BitEncodedInt();
                            _prices.Swap(oldIndex, newIndex);
                        }
                        break;

                    case 2: // Discounts
                        if (isNewValue)
                        {
                            _discounts.Add(reader.ReadInt32());
                        }
                        else
                        {
                            var oldIndex = reader.Read7BitEncodedInt();
                            var newIndex = reader.Read7BitEncodedInt();
                            _discounts.Swap(oldIndex, newIndex);
                        }
                        break;

                    case 3: // VAT rates
                        if (isNewValue)
                        {
                            _vatRates.Add(reader.ReadInt32());
                        }
                        else
                        {
                            var oldIndex = reader.Read7BitEncodedInt();
                            var newIndex = reader.Read7BitEncodedInt();
                            _vatRates.Swap(oldIndex, newIndex);
                        }
                        break;

                    default:
                        throw new NotSupportedException();
                }
            }


            // Reading the receipt content
            var date = reader.ReadInt32().FromInt32ToDateTime();
            var loyaltyCard = reader.ReadInt32();
            var store = reader.ReadUInt16();

            var receipt = new Receipt
                {
                    Date = date,
                    LoyaltyCard = loyaltyCard,
                    Store = store
                };

            var itemCount = reader.Read7BitEncodedInt();
            var items = new Item[itemCount];

            for (int i = 0; i < itemCount; i++)
            {
                var gtinIndex = reader.Read7BitEncodedInt();
                var priceIndex = reader.Read7BitEncodedInt();

                var flag = reader.ReadByte();

                var nonOneQuantity = ((flag & 1) == 1);
                var nonZeroDiscount = ((flag & 2) == 2);

                var vatRateIndex = flag >> 2; // discarding two bits

                // fact: quantities equals to 1 amounts for 85% of the items.
                var quantity = nonOneQuantity ? reader.Read7BitEncodedInt() : 1;

                var discount = nonZeroDiscount ? reader.Read7BitEncodedInt() : 0;

                items[i] = new Item
                    {
                        GTIN = _gtins[gtinIndex],
                        UnitPrice = _prices[priceIndex],
                        Quantity = quantity,
                        Discount = discount,
                        VatRate = _vatRates[vatRateIndex]
                    };
            }

            receipt.Items = items;

            return receipt;
        }
    }
}
