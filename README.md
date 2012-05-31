# Lokad ReceiptStream
_By Joannes Vermorel, 2012_

High-performance receipt storage intended for large retail network.

Visit us at [Lokad.com](http://www.lokad.com/)

## Sample code

    // Hard-coding a receipt
    var receipt = new Receipt 
    {
        Date = new DateTime(2012, 5, 30),
        LoyaltyCard = 1013,
        Store = 555,

        Items = new Item[]
        {
            new Item
            {
                GTIN = "1234567890123",
                UnitPrice = 150, // in cents
                Discount = 0, // in cents
                Quantity = 1, 
                VatRate = 196 // per-thousands
            },
        }
    };
    
    // Writing the receipt
	var stream = new MemoryStream();
    var writer = new ReceiptWriter(stream);
    writer.Write(receipt);

    // Reading the receipt
    stream.Position = 0;
	var reader = new ReceiptReader(stream);
	var receiptCopy = reader.Read();

