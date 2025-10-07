using VendingMachineApp.Domain;
using VendingMachineApp.Services;
using VendingMachineApp.UI;

namespace VendingMachineApp;

internal class Program
{
    private static void Main(string[] args)
    {
        IInventoryService inventoryService = new InventoryService();
        IWalletService walletService = new WalletService();
        IVendingMachineService vendingMachine = new VendingMachineService(inventoryService, walletService);

        SeedData.Initialize(inventoryService, walletService);

        IConsoleUI consoleUI = new ConsoleUI(vendingMachine, inventoryService, walletService);
        consoleUI.Run();
    }
}


