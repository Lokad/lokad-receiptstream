#region (c)2012 Lokad - New BSD license
// Company: http://www.lokad.com
// This code is released under the terms of the new BSD license
#endregion
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Lokad.ReceiptStream;
using Lokad.ReceiptStream.Entities;
using NUnit.Framework;

namespace ReceiptStream.Tests
{
    public class ReadWrite
    {
        [Test]
        public void IsSymmetric()
        {
            var stream = new MemoryStream();

            var writer = new ReceiptWriter(stream);

            #region var receipts = new Receipt[3]
            var receipts = new Receipt[]
            {
                new Receipt
                {
                    Date = new DateTime(2012, 5, 30),
                    LoyaltyCard = 1013,
                    Store = 555,

                    Items = new Item[]
                    {
                        new Item
                        {
                            GTIN = "1234567890123",
                            UnitPrice = 150,
                            Discount = 0,
                            Quantity = 1,
                            VatRate = 196
                        },
                        new Item
                        {
                            GTIN = "1234567890123",
                            UnitPrice = 150,
                            Discount = 0,
                            Quantity = 1,
                            VatRate = 196
                        },
                        new Item
                        {
                            GTIN = "1234567890124",
                            UnitPrice = 50,
                            Discount = 10,
                            Quantity = 5,
                            VatRate = 196
                        },
                    }
                },

                new Receipt
                {
                    Date = new DateTime(2012, 5, 31),
                    LoyaltyCard = 1014,
                    Store = 555,

                    Items = new Item[]
                    {
                        new Item
                        {
                            GTIN = "1234567890124",
                            UnitPrice = 150,
                            Discount = 0,
                            Quantity = 1,
                            VatRate = 196
                        },
                        new Item
                        {
                            GTIN = "1234567890125",
                            UnitPrice = 250,
                            Discount = 0,
                            Quantity = 2,
                            VatRate = 196
                        },
                        new Item
                        {
                            GTIN = "1234567890126",
                            UnitPrice = 250,
                            Discount = 25,
                            Quantity = 1,
                            VatRate = 55
                        },
                    }
                },

                new Receipt
                {
                    Date = new DateTime(2012, 5, 31),
                    LoyaltyCard = 1015,
                    Store = 555,

                    Items = new Item[]
                    {
                        new Item
                        {
                            GTIN = "1234567890126",
                            UnitPrice = 50,
                            Discount = 0,
                            Quantity = 1,
                            VatRate = 196
                        },
                        new Item
                        {
                            GTIN = "1234567890125",
                            UnitPrice = 50,
                            Discount = 0,
                            Quantity = 1,
                            VatRate = 196
                        }
                    }
                },
            };
            #endregion

            writer.Write(receipts[0]);
            writer.Write(receipts[1]);
            writer.Write(receipts[2]);

            stream.Position = 0;

            var reader = new ReceiptReader(stream);

            var r0 = reader.Read();
            var r1 = reader.Read();
            var r2 = reader.Read();

            // Receipt top-level info
            Assert.AreEqual(receipts[0].Date, r0.Date);
            Assert.AreEqual(receipts[0].LoyaltyCard, r0.LoyaltyCard);
            Assert.AreEqual(receipts[0].Store, r0.Store);

            Assert.AreEqual(receipts[1].Date, r1.Date);
            Assert.AreEqual(receipts[1].LoyaltyCard, r1.LoyaltyCard);
            Assert.AreEqual(receipts[1].Store, r1.Store);

            Assert.AreEqual(receipts[2].Date, r2.Date);
            Assert.AreEqual(receipts[2].LoyaltyCard, r2.LoyaltyCard);
            Assert.AreEqual(receipts[2].Store, r2.Store);

            // Number of items
            Assert.AreEqual(receipts[0].Items.Length, r0.Items.Length);
            Assert.AreEqual(receipts[1].Items.Length, r1.Items.Length);
            Assert.AreEqual(receipts[2].Items.Length, r2.Items.Length);

            // First receipt
            Assert.AreEqual(receipts[0].Items[0].GTIN, r0.Items[0].GTIN);
            Assert.AreEqual(receipts[0].Items[0].UnitPrice, r0.Items[0].UnitPrice);
            Assert.AreEqual(receipts[0].Items[0].Quantity, r0.Items[0].Quantity);
            Assert.AreEqual(receipts[0].Items[0].Discount, r0.Items[0].Discount);
            Assert.AreEqual(receipts[0].Items[0].VatRate, r0.Items[0].VatRate);

            Assert.AreEqual(receipts[0].Items[1].GTIN, r0.Items[1].GTIN);
            Assert.AreEqual(receipts[0].Items[1].UnitPrice, r0.Items[1].UnitPrice);
            Assert.AreEqual(receipts[0].Items[1].Quantity, r0.Items[1].Quantity);
            Assert.AreEqual(receipts[0].Items[1].Discount, r0.Items[1].Discount);
            Assert.AreEqual(receipts[0].Items[1].VatRate, r0.Items[1].VatRate);

            Assert.AreEqual(receipts[0].Items[2].GTIN, r0.Items[2].GTIN);
            Assert.AreEqual(receipts[0].Items[2].UnitPrice, r0.Items[2].UnitPrice);
            Assert.AreEqual(receipts[0].Items[2].Quantity, r0.Items[2].Quantity);
            Assert.AreEqual(receipts[0].Items[2].Discount, r0.Items[2].Discount);
            Assert.AreEqual(receipts[0].Items[2].VatRate, r0.Items[2].VatRate);

            // Second receipt
            Assert.AreEqual(receipts[1].Items[0].GTIN, r1.Items[0].GTIN);
            Assert.AreEqual(receipts[1].Items[0].UnitPrice, r1.Items[0].UnitPrice);
            Assert.AreEqual(receipts[1].Items[0].Quantity, r1.Items[0].Quantity);
            Assert.AreEqual(receipts[1].Items[0].Discount, r1.Items[0].Discount);
            Assert.AreEqual(receipts[1].Items[0].VatRate, r1.Items[0].VatRate);

            Assert.AreEqual(receipts[1].Items[1].GTIN, r1.Items[1].GTIN);
            Assert.AreEqual(receipts[1].Items[1].UnitPrice, r1.Items[1].UnitPrice);
            Assert.AreEqual(receipts[1].Items[1].Quantity, r1.Items[1].Quantity);
            Assert.AreEqual(receipts[1].Items[1].Discount, r1.Items[1].Discount);
            Assert.AreEqual(receipts[1].Items[1].VatRate, r1.Items[1].VatRate);

            Assert.AreEqual(receipts[1].Items[2].GTIN, r1.Items[2].GTIN);
            Assert.AreEqual(receipts[1].Items[2].UnitPrice, r1.Items[2].UnitPrice);
            Assert.AreEqual(receipts[1].Items[2].Quantity, r1.Items[2].Quantity);
            Assert.AreEqual(receipts[1].Items[2].Discount, r1.Items[2].Discount);
            Assert.AreEqual(receipts[1].Items[2].VatRate, r1.Items[2].VatRate);

            // Third receipt
            Assert.AreEqual(receipts[2].Items[0].GTIN, r2.Items[0].GTIN);
            Assert.AreEqual(receipts[2].Items[0].UnitPrice, r2.Items[0].UnitPrice);
            Assert.AreEqual(receipts[2].Items[0].Quantity, r2.Items[0].Quantity);
            Assert.AreEqual(receipts[2].Items[0].Discount, r2.Items[0].Discount);
            Assert.AreEqual(receipts[2].Items[0].VatRate, r2.Items[0].VatRate);

            Assert.AreEqual(receipts[2].Items[1].GTIN, r2.Items[1].GTIN);
            Assert.AreEqual(receipts[2].Items[1].UnitPrice, r2.Items[1].UnitPrice);
            Assert.AreEqual(receipts[2].Items[1].Quantity, r2.Items[1].Quantity);
            Assert.AreEqual(receipts[2].Items[1].Discount, r2.Items[1].Discount);
            Assert.AreEqual(receipts[2].Items[1].VatRate, r2.Items[1].VatRate);
        }
    }
}
