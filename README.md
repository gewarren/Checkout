# Checkout
A simple, .NET Core console app for kids to play shopkeeper. All you need is a USB barcode scanner and some items from your pantry!

The app looks for a file named "Products.xml" in the directory that it's run from. The app is capable of adding new products to the file. Here's an example file:

```xml
<?xml version="1.0" encoding="utf-8"?>
<Products>
  <Product>
    <Barcode>657622011855</Barcode>
    <Name>Honest Kids apple juice 8 count</Name>
    <Price>15.00</Price>
  </Product>
  <Product>
    <Barcode>011110742865</Barcode>
    <Name>Private Selection slow roasted pecans</Name>
    <Price>8.00</Price>
  </Product>
  <Product>
    <Barcode>00096362</Barcode>
    <Name>Trader Joe's black beans</Name>
    <Price>2.99</Price>
  </Product>
  <Product>
    <Barcode>044300106253</Barcode>
    <Name>Rosarita vegetarian refried beans</Name>
    <Price>2.50</Price>
  </Product>
  <Product>
    <Barcode>00502610</Barcode>
    <Name>Trader Joe's organic garbanzo beans</Name>
    <Price>3.99</Price>
  </Product>
  <Product>
    <Barcode>039400017349</Barcode>
    <Name>Bush's Best kidney beans</Name>
    <Price>2.99</Price>
  </Product>
</Products>
```
