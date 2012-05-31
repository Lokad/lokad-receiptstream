#region (c)2012 Lokad - New BSD license
// Company: http://www.lokad.com
// This code is released under the terms of the new BSD license
#endregion
using System.Collections.Generic;
using System.IO;
using Lokad.ReceiptStream.Entities;

namespace Lokad.ReceiptStream
{
    /// <summary>Outputs a very compact stream representing 
    /// the receipts (lossless compression).</summary>
    public class ReceiptWriter
    {
        private Stream _stream;

        // Index dictionaries
        private Dictionary<string, int> _gtins;
        private Dictionary<int, int> _prices;
        private Dictionary<int, int> _discounts;
        private Dictionary<int, int> _vatRates;

        // Reverse indexes lookup
        private List<string> _rgtins;
        private List<int> _rprices;
        private List<int> _rdiscounts;
        private List<int> _rvatRates;

        // Frequency tables
        private List<int> _gtinCounts;
        private List<int> _priceCounts;
        private List<int> _discountCounts;
        private List<int> _vatRateCounts;

        public ReceiptWriter(Stream stream)
        {
            _stream = stream;

            _gtins = new Dictionary<string, int>();
            _prices = new Dictionary<int, int>();
            _discounts = new Dictionary<int, int>();
            _vatRates = new Dictionary<int, int>();

            _rgtins = new List<string>();
            _rprices = new List<int>();
            _rdiscounts = new List<int>();
            _rvatRates = new List<int>();

            _gtinCounts = new List<int>();
            _priceCounts = new List<int>();
            _discountCounts = new List<int>();
            _vatRateCounts = new List<int>();
        }

        public void Write(Receipt receipt)
        {
            var writer = new MyBinaryWriter(_stream);

            // 1 message for each new value
            // HACK: introducing an temporary stream to count messages first
            var messagesCount = 0;
            var updateStream = new MemoryStream();
            var updateWriter = new MyBinaryWriter(updateStream);

            foreach(var item in receipt.Items)
            {
                int gtinIndex;
                if(!_gtins.TryGetValue(item.GTIN, out gtinIndex))
                {
                    _gtins.Add(item.GTIN, _gtins.Count);
                    _rgtins.Add(item.GTIN);
                    _gtinCounts.Add(1);

                    var flag = (0 << 1) | 1; // GTIN code = 0
                    updateWriter.Write7BitEncodedInt(flag);
                    updateWriter.Write(item.GTIN);
                    messagesCount++;
                }
                else
                {
                    var newIndex = RoughSort(_gtinCounts, gtinIndex);
                    if(newIndex > 0) // perform the swap
                    {
                        Swap(_gtins, _rgtins, newIndex, gtinIndex);

                        var flag = (0 << 1); // GTIN code = 0
                        updateWriter.Write7BitEncodedInt(flag);
                        updateWriter.Write7BitEncodedInt(gtinIndex);
                        updateWriter.Write7BitEncodedInt(newIndex);
                        messagesCount++;
                    }
                }

                int priceIndex;
                if(!_prices.TryGetValue(item.UnitPrice, out priceIndex))
                {
                    _prices.Add(item.UnitPrice, _prices.Count);
                    _rprices.Add(item.UnitPrice);
                    _priceCounts.Add(1);

                    var flag = (1 << 1) | 1; // Price code = 1

                    updateWriter.Write7BitEncodedInt(flag);
                    updateWriter.Write(item.UnitPrice);
                    messagesCount++;
                }
                else
                {
                    var newIndex = RoughSort(_priceCounts, priceIndex);
                    if (newIndex > 0) // perform the swap
                    {
                        Swap(_prices, _rprices, newIndex, priceIndex);

                        var flag = (1 << 1); // Price code = 1
                        updateWriter.Write7BitEncodedInt(flag);
                        updateWriter.Write7BitEncodedInt(priceIndex);
                        updateWriter.Write7BitEncodedInt(newIndex);
                        messagesCount++;
                    }
                }

                int discountIndex;
                if(!_discounts.TryGetValue(item.Discount, out discountIndex))
                {
                    _discounts.Add(item.Discount, _discounts.Count);
                    _rdiscounts.Add(item.Discount);
                    _discountCounts.Add(1);

                    var flag = (2 << 1) | 1; // Discount code = 2

                    updateWriter.Write7BitEncodedInt(flag);
                    updateWriter.Write(item.Discount);
                    messagesCount++;
                }
                else
                {
                    var newIndex = RoughSort(_discountCounts, discountIndex);
                    if (newIndex > 0) // perform the swap
                    {
                        Swap(_discounts, _rdiscounts, newIndex, discountIndex);

                        var flag = (2 << 1); // Discount code = 2
                        updateWriter.Write7BitEncodedInt(flag);
                        updateWriter.Write7BitEncodedInt(discountIndex);
                        updateWriter.Write7BitEncodedInt(newIndex);
                        messagesCount++;
                    }
                }

                int vatRateIndex;
                if (!_vatRates.TryGetValue(item.VatRate, out vatRateIndex))
                {
                    _vatRates.Add(item.VatRate, _vatRates.Count);
                    _rvatRates.Add(item.VatRate);
                    _vatRateCounts.Add(1);

                    var flag = (3 << 1) | 1; // VAT code = 3

                    updateWriter.Write7BitEncodedInt(flag);
                    updateWriter.Write(item.VatRate);
                    messagesCount++;
                }
                else
                {
                    var newIndex = RoughSort(_vatRateCounts, vatRateIndex);
                    if (newIndex > 0) // perform the swap
                    {
                        Swap(_vatRates, _rvatRates, newIndex, vatRateIndex);

                        var flag = (3 << 1); // VAT code = 3
                        updateWriter.Write7BitEncodedInt(flag);
                        updateWriter.Write7BitEncodedInt(vatRateIndex);
                        updateWriter.Write7BitEncodedInt(newIndex);
                        messagesCount++;
                    }
                }
            }

            // Writing updates to the primary stream
            writer.Write7BitEncodedInt(messagesCount);
            writer.Write(updateStream.GetBuffer(), 0, (int)updateStream.Position);


            // Writing the receipt content
            writer.Write(receipt.Date.FromDateTimeToInt32());
            writer.Write(receipt.LoyaltyCard);
            writer.Write(receipt.Store);

            writer.Write7BitEncodedInt(receipt.Items.Length);

            foreach(var item in receipt.Items)
            {
                writer.Write7BitEncodedInt(_gtins[item.GTIN]);
                writer.Write7BitEncodedInt(_prices[item.UnitPrice]);

                var flag = 0;

                if (item.Quantity != 1) flag |= 1;
                if (item.Discount != 0) flag |= 2;

                flag |= _vatRates[item.VatRate] << 2;

                writer.Write((byte) flag);

                // fact: quantities equals to 1 amounts for 85% of the items.);
                if(item.Quantity != 1) writer.Write7BitEncodedInt(item.Quantity);
                if(item.Discount != 0) writer.Write7BitEncodedInt(item.Discount);
            }
        }

        /// <summary>Swap the counts once if needed.</summary>
        static int RoughSort(List<int> counts, int index)
        {
            counts[index] += 1;

            var hi = index/2;
            if(counts[index] > counts[hi])
            {
                var v = counts[hi];
                counts[hi] = counts[index];
                counts[index] = v;
                return hi;
            }

            return -1;
        }

        static void Swap<T>(Dictionary<T, int> map, List<T> rev, int index1, int index2)
        {
            var t1 = rev[index1];
            var t2 = rev[index2];

            map[t1] = index2;
            map[t2] = index1;

            rev[index1] = t2;
            rev[index2] = t1;
        }
    }
}
