using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReactiveStock.ExternalServices;
internal class RandomStockPriceServiceGateway : IStockPriceServiceGateway
{
    private decimal _lastRandomPrice = 20;
    private readonly Random _random = new Random();

    public string Id { get; } = System.Guid.NewGuid().ToString()[^4..];


    public decimal GetLatestPrice(StockSymbol stockSymbol)
    {
        var newPrice = _lastRandomPrice + _random.Next(-5, 5);

        if (newPrice < 0)
            newPrice = 5;
        else if (newPrice > 50)
            newPrice = 45;

        _lastRandomPrice = newPrice;

        return newPrice;
    }
}
