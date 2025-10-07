using VendingMachineApp.Domain;
using VendingMachineApp.Services;

namespace VendingMachineApp;

public static class SeedData
{
    public static void Initialize(IInventoryService inventory, IWalletService wallet)
    {
        inventory.AddOrUpdateProduct(new Product("Вода", 50m), 10);
        inventory.AddOrUpdateProduct(new Product("Сок", 75m), 8);
        inventory.AddOrUpdateProduct(new Product("Чипсы", 90m), 5);
        inventory.AddOrUpdateProduct(new Product("Шоколад", 120m), 6);

        wallet.LoadFloat(CoinDenomination.Rub1, 50);
        wallet.LoadFloat(CoinDenomination.Rub2, 50);
        wallet.LoadFloat(CoinDenomination.Rub5, 40);
        wallet.LoadFloat(CoinDenomination.Rub10, 30);
        wallet.LoadFloat(CoinDenomination.Rub50, 20);
        wallet.LoadFloat(CoinDenomination.Rub100, 10);
    }
}


