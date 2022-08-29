using OxyPlot;

namespace ReactiveStock;
internal readonly record struct StockSymbol(string Name, OxyColor Color)
{
    public override string ToString()
    {
        return Name;
    }
}
