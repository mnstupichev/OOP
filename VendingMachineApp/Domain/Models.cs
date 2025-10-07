namespace VendingMachineApp.Domain;

public enum CoinDenomination
{
    Rub1 = 1, Rub2 = 2, Rub5 = 5, Rub10 = 10, Rub50 = 50, Rub100 = 100
}

public class Product
{
    public string Name { get; }
    public decimal Price { get; }

    public Product(string name, decimal priceRub)
    {
        Name = name;
        Price = priceRub;
    }
}

public class StockItem
{
    public Product Product { get; }
    public int Quantity { get; private set; }

    public StockItem(Product product, int quantity)
    {
        this.Product = product;
        Quantity = quantity;
    }

    public bool TryDecrement()
    {
        if (Quantity == 0)
        {
            return false;
        }
        Quantity--;
        return true;
    }

    public void Add(int amount)
    {
        if (amount == 0)
        {
            return;
        }
        Quantity += amount;
    }
}

public class InsertedSession
{
    public decimal InsertedRub { get; private set; }

    public void Insert(decimal rubles)
    {
        if (rubles <= 0)
        {
            return;
        }
        InsertedRub += rubles;
    }

    public decimal Reset()
    {
        var r = InsertedRub;
        InsertedRub = 0;
        return r;
    }
}


