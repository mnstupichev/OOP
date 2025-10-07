using VendingMachineApp.Domain;

namespace VendingMachineApp.Services;

public class VendingMachineService: IVendingMachineService
{
    private IInventoryService _inventory;
    private IWalletService _wallet;

    public VendingMachineService(IInventoryService inventory, IWalletService wallet)
    {
        _inventory = inventory;
        _wallet = wallet;
    }

    public IReadOnlyCollection<StockItem> ListProducts()
    {
        return _inventory.GetAll();
    }

    public void InsertCoin(CoinDenomination denomination) => _wallet.InsertCoin(denomination);

    public bool TryPurchase(string productName, out Product? product, out Dictionary<CoinDenomination, int> changeCoins, out string? error)
    {
        changeCoins = new Dictionary<CoinDenomination, int>();
        error = null;
        product = null;
        var item = _inventory.GetByName(productName);
        if (item == null)
        {
            error = "Товар не найден";
            return false;
        }
        if (item.Quantity == 0)
        {
            error = "Товара нет в наличии";
            return false;
        }
        if (!_wallet.TryMakePayment(item.Product.Price, out var changeDue, out var coins))
        {
            if (_wallet.GetInsertedRub() < item.Product.Price)
            {
                decimal difference = item.Product.Price - _wallet.GetInsertedRub();
                error = $"Недостаточно средств. Внесите ещё {difference:0.##} руб.";
            }
            else
            {
                error = "Невозможно выдать сдачу. Выберите другой товар или отмените операцию.";
            }
            return false;
        }
        if (!_inventory.TryTake(productName))
        {
            error = "Ошибка выдачи товара.";
            return false;
        }
        changeCoins = coins;
        product = item.Product;
        return true;
    }

    public decimal CancelAndReturn() => _wallet.CancelAndReturnInserted();
}


