using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Linq;

namespace Checkout
{
    class Program
    {
        static void Main(string[] args)
        {
            decimal totalCost = 0;
            const string File = "Products.xml";
            Dictionary<string, int> purchases = new Dictionary<string, int>();

            XElement root;
            try
            {
                root = XElement.Load(File);
            }
            catch (FileNotFoundException)
            {
                Console.WriteLine($"Could not find file: {File}.");
                return;
            }

            Console.WriteLine(@"___          ___    ___");
            Console.WriteLine(@"l  \  l\   l l  \  /   \");
            Console.WriteLine(@"l__/  l \  l l__/  l");
            Console.WriteLine(@"l  \  l  \ l l  \  l");
            Console.WriteLine(@"1   \ l   \l l   \ \___/");

            Console.WriteLine("\nWelcome to the RNRC store.\n");

            Console.WriteLine("Please scan the first item. Enter 't' at any time to calculate the total.");
            string barcode = Console.ReadLine();

            string name;
            decimal price;
            bool updates = false;

            while (barcode.ToLowerInvariant() != "t")
            {
                XElement product = root.Elements().Where(el => el.Element("Barcode").Value == barcode).FirstOrDefault();

                if (product is null)
                {
                    Console.WriteLine("Product not found. Please enter the name:");
                    name = Console.ReadLine();
                    Console.WriteLine("Please enter the price:");
                    price = decimal.Parse(Console.ReadLine());
                    Console.WriteLine("Do you want to add this product to your inventory list? Enter 'y' or 'n'.");
                    string yorn = Console.ReadLine();
                    if (yorn.ToLowerInvariant() == "y")
                    {
                        root.Add(new XElement("Product", new XElement("Barcode", barcode), new XElement("Name", name), new XElement("Price", price)));
                        updates = true;
                        Console.WriteLine($"Product '{name}' added to inventory list.");
                    }
                }
                else
                {
                    name = product.Element("Name").Value;
                    price = decimal.Parse(product.Element("Price").Value);
                }

                totalCost = Decimal.Round(totalCost + price, 2);

                Console.WriteLine($"{name} - ${price}. Please scan the next item or enter 't'...");

                // Remember purchases so we can print a receipt.
                if (purchases.ContainsKey(name))
                    purchases[name]++;
                else
                    purchases.Add(name, 1);

                // Scan the next item.
                barcode = Console.ReadLine();
            }

            // Save any updates to the file.
            if (updates)
            {
                root.Save(File);
            }

            Console.WriteLine($"Your total today is ${totalCost}. Cash ('c') or credit ('cr')?");

            string paymentType = Console.ReadLine();
            if (paymentType.ToLowerInvariant() == "c")
            {
                // Paying by cash.
                Console.WriteLine("Enter the tendered amount:");
                decimal tendered = decimal.Parse(Console.ReadLine());
                Console.WriteLine($"Change due: ${tendered - totalCost}. Would you like a receipt? 'y' or 'n'...");
            }
            else if (paymentType.ToLowerInvariant() == "cr")
            {
                // Paying by credit.
                Console.WriteLine("Please scan the credit card.");
                string cardNumber = Console.ReadLine();
                Console.WriteLine($"Charged ${totalCost} to card number {cardNumber}. Would you like a receipt? 'y' or 'n'...");
            }

            bool receipt = Console.ReadLine().ToLowerInvariant() == "y";
            if (receipt)
            {
                Console.WriteLine("\nItems purchased:\n");
                foreach (var purchase in purchases)
                {
                    Console.WriteLine($"{purchase.Value} {purchase.Key}");
                }
            }

            Console.WriteLine("\nThank you for shopping at the RNRC store!");
            Console.ReadLine();
        }
    }
}
