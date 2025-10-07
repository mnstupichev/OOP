using VendingMachineApp.Domain;

namespace VendingMachineApp.Services;

public class WalletService: IWalletService
{
    private Dictionary<CoinDenomination, int> _float = new();
    private decimal _insertedRub;
    private decimal _takingsRub;

    public IReadOnlyDictionary<CoinDenomination, int> GetFloat() => _float;

    public void LoadFloat(CoinDenomination denomination, int count)
    {
        if (count <= 0)
        {
            return;
        }
        _float[denomination] = _float.GetValueOrDefault(denomination) + count;
    }

    public void InsertCoin(CoinDenomination denomination)
    {
        _insertedRub += (int)denomination;
        _float[denomination] = _float.GetValueOrDefault(denomination) + 1;
    }

    public decimal GetInsertedRub()
    {
        return _insertedRub;
    }

    public decimal CancelAndReturnInserted()
    {
        var toReturn = _insertedRub;
        // Remove inserted coins from float using greedy on available coins
        var change = MakeChange((int)toReturn, simulate: false, out var success);
        _insertedRub = 0;
        return toReturn;
    }

    public bool TryCancelAndReturn(out decimal amount, out string? error)
    {
        amount = _insertedRub;
        error = null;
        var coins = MakeChange((int)amount, simulate: true, out var possible);
        if (!possible)
        {
            error = "Невозможно выдать сдачу при отмене. Обратитесь к администратору.";
            return false;
        }
        // apply change and reset
        foreach (var kvp in coins)
        {
            _float[kvp.Key] = _float.GetValueOrDefault(kvp.Key) - kvp.Value;
        }
        _insertedRub = 0;
        return true;
    }

    public bool TryMakePayment(decimal priceRub, out decimal changeRub, out Dictionary<CoinDenomination, int> changeCoins)
    {
        changeCoins = new Dictionary<CoinDenomination, int>();
        if (_insertedRub < priceRub)
        {
            changeRub = priceRub - _insertedRub;
            return false;
        }

        var changeDue = _insertedRub - priceRub;
        if (changeDue > 0)
        {
            var coins = MakeChange((int)changeDue, simulate: true, out var possible);
            if (!possible)
            {
                changeRub = changeDue;
                return false;
            }
            foreach (var kvp in coins)
            {
                _float[kvp.Key] = _float.GetValueOrDefault(kvp.Key) - kvp.Value;
            }
            changeCoins = coins;
        }

        _takingsRub += priceRub;
        _insertedRub = 0;
        changeRub = 0;
        return true;
    }

    public decimal CollectTakings()
    {
        var t = _takingsRub;
        _takingsRub = 0;
        return t;
    }

    private Dictionary<CoinDenomination, int> MakeChange(int amount, bool simulate, out bool success)
    {
        var result = new Dictionary<CoinDenomination, int>();
        int remaining = amount;
        var ordered = _float.OrderByDescending(k => (int)k.Key).ToList();
        foreach (var (denom, count) in ordered)
        {
            int coinValue = (int)denom;
            int needed = remaining / coinValue;
            if (needed <= 0) continue;
            int use = Math.Min(needed, count);
            if (use > 0)
            {
                result[denom] = use;
                remaining -= use * coinValue;
            }
        }
        success = remaining == 0;
        if (success && !simulate)
        {
            foreach (var kvp in result)
            {
                _float[kvp.Key] = _float.GetValueOrDefault(kvp.Key) - kvp.Value;
            }
        }
        return result;
    }
}


