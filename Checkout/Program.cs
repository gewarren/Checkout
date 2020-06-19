using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Linq;

namespace Checkout
{
    class Item
    {
        public string Name;
        public decimal Price;
        public int Quantity;
    }

    class Program
    {
        static void Main(string[] args)
        {
            const string File = "Products.xml";
            Dictionary<string, Item> purchases = new Dictionary<string, Item>();

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
                    Console.Write("$");
                    while (true)
                    {
                        try
                        {
                            price = decimal.Parse(Console.ReadLine());
                        }
                        catch (FormatException)
                        {
                            Console.WriteLine("Invalid price. Please try again.");
                            Console.Write("$");
                            continue;
                        }

                        break;
                    }

                    Console.WriteLine("Do you want to add this product to your inventory list? y or n...");
                    if (Console.ReadLine().ToLowerInvariant() == "y")
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

                // Remember purchases so we can print a receipt.
                if (purchases.ContainsKey(barcode))
                {
                    purchases[barcode].Quantity++;
                }
                else
                {
                    purchases.Add(barcode, new Item { Name = name, Price = price, Quantity = 1 });
                }

                Console.WriteLine($"{name} - ${price}. Please scan the next item or enter 't'...");

                // Scan the next item.
                barcode = Console.ReadLine();
            }

            // Save any updates to the file.
            if (updates)
            {
                root.Save(File);
            }

            // Print a list of items.
            PrintItems(purchases);

            // Make changes to quantities, if needed.
            MakeChanges(purchases);

            // Take payment.
            decimal totalCost = GetTotal(purchases);
            TakePayment(totalCost);

            Console.WriteLine("\nThank you for shopping at the RNRC store!");
            Console.ReadLine();
        }

        // Print a list of items purchased.
        private static void PrintItems(Dictionary<string, Item> purchases)
        {
            Console.WriteLine("\nItems purchased:\n");
            int line = 1;
            foreach (var purchase in purchases)
            {
                Console.WriteLine($"{line++}. {purchase.Value.Name} - quantity {purchase.Value.Quantity}");
            }

            Console.WriteLine($"\nTotal: ${GetTotal(purchases)}.");
        }

        private static void TakePayment(decimal totalCost)
        {
            Console.WriteLine($"Your total today is ${totalCost}. Cash ('c') or credit ('cr')?");

            string paymentType;
            do
            {
                paymentType = Console.ReadLine();

                if (paymentType.ToLowerInvariant() == "c")
                {
                    // Paying by cash.
                    Console.WriteLine("Enter the tendered amount:");
                    Console.Write("$");

                    decimal tendered;
                    while (true)
                    {
                        try
                        {
                            tendered = decimal.Parse(Console.ReadLine());
                        }
                        catch (FormatException)
                        {
                            Console.WriteLine("Invalid amount. Please try again.");
                            Console.Write("$");
                            continue;
                        }

                        break;
                    }

                    Console.WriteLine($"Change due: ${tendered - totalCost}.");
                }
                else if (paymentType.ToLowerInvariant() == "cr")
                {
                    // Paying by credit.
                    Console.WriteLine("Please scan the credit card.");
                    string cardNumber = Console.ReadLine();
                    Console.WriteLine($"Charged ${totalCost} to card number {cardNumber}.");
                }
                else
                {
                    Console.WriteLine("Invalid entry. Please try again.");
                }
            } while (paymentType.ToLowerInvariant() != "c" && paymentType.ToLowerInvariant() != "cr");
        }

        private static void MakeChanges(Dictionary<string, Item> purchases)
        {
            Console.WriteLine("\nWould you like to make any changes? y or n...");

            bool changes = (Console.ReadLine().ToLowerInvariant() == "y");
            while (changes)
            {
                Console.WriteLine("Which line would you like to change?");
                int changeLine;
                while (true)
                {
                    try { changeLine = int.Parse(Console.ReadLine()); }
                    catch (FormatException)
                    {
                        Console.WriteLine("Unrecognized number. Please try again.");
                        continue;
                    }

                    if (changeLine > purchases.Count || changeLine < 1)
                    {
                        Console.WriteLine("Invalid line number. Please try again.");
                        continue;
                    }

                    // We got a valid line number.
                    break;
                }

                int line = 1;
                string key = null;
                foreach (var purchase in purchases)
                {
                    // Loop till we get to the specified line number.
                    if (line < changeLine)
                    {
                        line++;
                        continue;
                    }

                    key = purchase.Key;
                    Console.WriteLine($"\nEnter the new quantity for '{purchase.Value.Name}'...");
                    break;
                }

                int newQuantity;
                while (true)
                {
                    try { newQuantity = int.Parse(Console.ReadLine()); }
                    catch (FormatException)
                    {
                        Console.WriteLine("Unrecognized number. Please try again.");
                        continue;
                    }

                    if (newQuantity < 0)
                    {
                        Console.WriteLine("Invalid quantity. Please try again.");
                        continue;
                    }

                    // We got a valid quantity.
                    break;
                }

                purchases[key].Quantity = newQuantity;

                Console.WriteLine($"Quantity updated to {newQuantity}. Would you like to make any other changes? y or n");
                changes = (Console.ReadLine().ToLowerInvariant() == "y");
            }

            // Print the list of items.
            PrintItems(purchases);
        }

        private static decimal GetTotal(Dictionary<string, Item> purchases)
        {
            decimal total = 0;

            foreach (var purchase in purchases)
            {
                total += Decimal.Round(purchase.Value.Price * purchase.Value.Quantity, 2);
            }

            return total;
        }
    }
}
