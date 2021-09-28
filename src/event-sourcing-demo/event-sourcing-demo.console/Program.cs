using System;

namespace event_sourcing_demo.console
{
    class Program
    {
        static void Main(string[] args)
        {
            var warehouseProductRepository = new WarehouseProductRepository();
            var key = String.Empty;
            while (key != "X")
            {
                Console.WriteLine("R: Receive Inventory");
                Console.WriteLine("S: Ship Inventory");
                Console.WriteLine("A: Inventory Adjustment");
                Console.WriteLine("Q: Quantity On Hand");
                Console.WriteLine("E: Events");

                key = Console.ReadLine()?.ToUpperInvariant();

                var sku = GetSkuFromConsole();
                var wareHouseProduct = warehouseProductRepository.Get(sku);

                switch (key)
                {
                    case "R":
                        var receiveInput = GetQuantity();
                        if (receiveInput.IsValid)
                        {
                            wareHouseProduct.ReceiveProduct(receiveInput.Quantity);
                            Console.WriteLine($"{sku} Received: {receiveInput.Quantity}");
                        }
                        break;
                    case "S":
                        var shipInput = GetQuantity();
                        if (shipInput.IsValid)
                        {
                            wareHouseProduct.ShipProduct(shipInput.Quantity);
                            Console.WriteLine($"{sku} Shipped: {shipInput.Quantity}");
                        }
                        break;
                    case "A":
                        break;
                        var adjustmentInput = GetQuantity();
                        Console.WriteLine("Reason: ");
                        if (adjustmentInput.IsValid)
                        {
                            wareHouseProduct.AdjustInventory(adjustmentInput.Quantity, Console.ReadLine()); ;
                            Console.WriteLine($"{sku} Shipped: {adjustmentInput.Quantity}");
                        }
                    case "Q":
                        var currentQuantityOnHand = wareHouseProduct.GetQuantityOnHand();
                        Console.WriteLine($"{sku} Quantity On Hand: {currentQuantityOnHand}");
                        break;
                    case "E":
                        Console.WriteLine($"Events: {sku}");
                        foreach (var evnt in wareHouseProduct.GetEvents())
                        {
                            switch (evnt)
                            {
                                case ProductReceived receiveProdcut:
                                    Console.WriteLine($"{receiveProdcut.Date:u} {sku} Received: {receiveProdcut.Quantity}");
                                    break;
                                case ProductShipped shipProduct:
                                    Console.WriteLine($"{shipProduct.Date:u} {sku} Shipped: {shipProduct.Quantity}");
                                    break;
                                case InventoryAdjusted adjustInventory:
                                    Console.WriteLine($"{adjustInventory.Date:u} {sku} Adjusted: {adjustInventory.Quantity}");
                                    break;
                                default:
                                    throw new InvalidOperationException("Unsupported event addition!");
                            }
                        }
                        break;
                }
                warehouseProductRepository.Save(wareHouseProduct);
                Console.ReadLine();
                Console.WriteLine();
            }
        }


        private static string _sku;
        private static (bool IsValid, int Quantity) GetQuantity()
        {
            Console.WriteLine("Quantity: ");
            return (int.TryParse(Console.ReadLine(),out int quantity), quantity);
        }

        private static string GetSkuFromConsole()
        {
            Console.WriteLine("Sku: ");
            _sku = Console.ReadLine();
            return _sku;
        }
    }

}
