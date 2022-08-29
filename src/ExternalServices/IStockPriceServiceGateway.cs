namespace ReactiveStock.ExternalServices;
internal interface IStockPriceServiceGateway
{
    decimal GetLatestPrice(StockSymbol stockSymbol);
}
