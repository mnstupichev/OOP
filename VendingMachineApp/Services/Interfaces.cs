using VendingMachineApp.Domain;

namespace VendingMachineApp.Services;

public interface IInventoryService
{
    IReadOnlyCollection<StockItem> GetAll();
    StockItem? GetByName(string productName);
    void AddOrUpdateProduct(Product product, int quantity);
    bool TryTake(string productId);
}

public interface IWalletService
{
    IReadOnlyDictionary<CoinDenomination, int> GetFloat();
    void LoadFloat(CoinDenomination denomination, int count);
    void InsertCoin(CoinDenomination denomination);
    decimal GetInsertedRub();
    decimal CancelAndReturnInserted();
    bool TryMakePayment(decimal priceRub, out decimal changeRub, out Dictionary<CoinDenomination, int> changeCoins);
    decimal CollectTakings();
}

public interface IVendingMachineService
{
    IReadOnlyCollection<StockItem> ListProducts();
    void InsertCoin(CoinDenomination denomination);
    bool TryPurchase(string productName, out Product? product, out Dictionary<CoinDenomination, int> changeCoins, out string? error);
    decimal CancelAndReturn();
}

public interface IConsoleUI
{
    void Run();
}


