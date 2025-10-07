using System.Collections.ObjectModel;
using VendingMachineApp.Domain;

namespace VendingMachineApp.Services;

public class InventoryService: IInventoryService
{
    private Dictionary<string, StockItem> _stock = new(StringComparer.OrdinalIgnoreCase);

    public IReadOnlyCollection<StockItem> GetAll()
    {
        return new ReadOnlyCollection<StockItem>(_stock.Values.ToList());
    }

    public StockItem? GetByName(string productName)
    {
        _stock.TryGetValue(productName, out var item);
        return item;
    }

    public void AddOrUpdateProduct(Product product, int quantity)
    {
        if (_stock.TryGetValue(product.Name, out var item))
        {
            item.Add(quantity);
        }
        else
        {
            _stock[product.Name] = new StockItem(product, Math.Max(0, quantity));
        }
    }
    

    public bool TryTake(string productName)
    {
        var item = GetByName(productName);
        if (item == null)
        {
            return false;
        }
        return item.TryDecrement();
    }
}


