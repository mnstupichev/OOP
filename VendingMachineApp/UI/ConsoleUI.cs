using VendingMachineApp.Domain;
using VendingMachineApp.Services;

namespace VendingMachineApp.UI;

public class ConsoleUI: IConsoleUI
{
    private IVendingMachineService _vendingmachine;
    private IInventoryService _inventory;
    private IWalletService _wallet;

    private const string AdminPin = "1234";

    public ConsoleUI(IVendingMachineService vendingmachine, IInventoryService inventory, IWalletService wallet)
    {
        _vendingmachine = vendingmachine;
        _inventory = inventory;
        _wallet = wallet;
    }

    public void Run()
    {
        while (true)
        {
            ShowMainMenu();
            var input = Console.ReadLine();
            switch (input)
            {
                case "1":
                    ListProducts();
                    break;
                case "2":
                    InsertCoins();
                    break;
                case "3":
                    PurchaseFlow();
                    break;
                case "4":
                    CancelAndReturn();
                    break;
                case "9":
                    AdminFlow();
                    break;
                case "0":
                    return;
                default:
                    Console.WriteLine("Неизвестная команда");
                    break;
            }
        }
    }

    private static void ShowMainMenu()
    {
        Console.WriteLine();
        Console.WriteLine("___Вендинговый автомат___");
        Console.WriteLine("1) Показать товары");
        Console.WriteLine("2) Вставить монету");
        Console.WriteLine("3) Купить товар");
        Console.WriteLine("4) Отмена и возврат");
        Console.WriteLine("9) Администрирование");
        Console.WriteLine("0) Выход");
        Console.Write("Выбор: ");
    }

    private void ListProducts()
    {
        Console.WriteLine("Список товаров:");
        foreach (var item in _vendingmachine.ListProducts().OrderBy(i => i.Product.Price))
        {
            Console.WriteLine($"{item.Product.Name} - {item.Product.Price:0.##} руб. (осталось: {item.Quantity})");
        }
        Console.WriteLine($"Внесено: {_wallet.GetInsertedRub():0.##} руб.");
    }

    private void InsertCoins()
    {
        Console.WriteLine("Доступные номиналы: 1, 2, 5, 10, 50, 100 (руб)");
        Console.Write("Введите номинал монеты: ");
        var s = Console.ReadLine();
        if (int.TryParse(s, out var rub) && Enum.GetValues<CoinDenomination>().Cast<int>().Contains(rub))
        {
            _vendingmachine.InsertCoin((CoinDenomination)rub);
            Console.WriteLine($"Внесено: {_wallet.GetInsertedRub():0.##} руб.");
        }
        else
        {
            Console.WriteLine("Неверный номинал");
        }
    }

    private void PurchaseFlow()
    {
        ListProducts();
        Console.Write("Введите название товара: ");
        var id = Console.ReadLine() ?? string.Empty;
        var ok = _vendingmachine.TryPurchase(id, out var product, out var change, out var error);
        if (!ok)
        {
            Console.WriteLine(error);
            return;
        }
        Console.WriteLine($"Заберите товар: {product!.Name}");
        if (change.Count > 0)
        {
            Console.WriteLine("Сдача:");
            foreach (var kvp in change.OrderByDescending(c => (int)c.Key))
            {
                Console.WriteLine($"  {(int)kvp.Key} руб x {kvp.Value}");
            }
        }
    }

    private void CancelAndReturn()
    {
        if (_vendingmachine.TryCancelAndReturn(out var amount, out var error))
        {
            Console.WriteLine($"Возвращено: {amount:0.##} руб.");
        }
        else
        {
            Console.WriteLine(error);
        }
    }

    private void AdminFlow()
    {
        Console.Write("Введите PIN: ");
        var pin = Console.ReadLine();
        if (pin != AdminPin)
        {
            Console.WriteLine("Неверный PIN");
            return;
        }
        while (true)
        {
            Console.WriteLine();
            Console.WriteLine("___Администрирование___");
            Console.WriteLine("1) Пополнить товар");
            Console.WriteLine("2) Загрузить монеты в кассету");
            Console.WriteLine("3) Собрать выручку");
            Console.WriteLine("0) Назад");
            Console.Write("Выбор: ");
            var choice = Console.ReadLine();
            switch (choice)
            {
                case "1":
                    RestockProduct();
                    break;
                case "2":
                    LoadCoins();
                    break;
                case "3":
                    var cash = _wallet.CollectTakings();
                    Console.WriteLine($"Собрано: {cash:0.##} руб.");
                    break;
                case "0":
                    return;
                default:
                    Console.WriteLine("Неизвестная команда");
                    break;
            }
        }
    }

    private void RestockProduct()
    {
        Console.Write("Название товара: ");
        var name = Console.ReadLine() ?? string.Empty;
        var existing = _inventory.GetByName(name);
        decimal price;
        if (existing == null)
        {
            Console.Write("Цена в рублях (для нового товара): ");
            if (!decimal.TryParse(Console.ReadLine(), out price))
            {
                Console.WriteLine("Неверная цена");
                return;
            }
        }
        else
        {
            price = existing.Product.Price;
            Console.WriteLine($"Товар уже существует по цене {price:0.##} руб. Будет увеличено количество.");
        }
        Console.Write("Количество для добавления: ");
        if (!int.TryParse(Console.ReadLine(), out var quantity))
        {
            Console.WriteLine("Неверное количество");
            return;
        }
        _inventory.AddOrUpdateProduct(new Product(name, price), quantity);
        Console.WriteLine("Готово");
    }

    private void LoadCoins()
    {
        Console.WriteLine("Номиналы: 1, 2, 5, 10, 50, 100");
        Console.Write("Номинал: ");
        if (!int.TryParse(Console.ReadLine(), out var denomInt) || !Enum.GetValues<CoinDenomination>().Cast<int>().Contains(denomInt))
        {
            Console.WriteLine("Неверный номинал");
            return;
        }
        Console.Write("Количество: ");
        if (!int.TryParse(Console.ReadLine(), out var count))
        {
            Console.WriteLine("Неверное количество");
            return;
        }
        _wallet.LoadFloat((CoinDenomination)denomInt, count);
        Console.WriteLine("Загружено");
    }
}


