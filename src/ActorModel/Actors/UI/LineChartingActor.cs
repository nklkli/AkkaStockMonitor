using Akka.Actor;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
using System;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;

namespace ReactiveStock.ActorModel.Actors.UI;

class LineChartingActor : ReceiveActor
{
    private readonly PlotModel _chartModel;
    private readonly Dictionary<string, LineSeries> _series;
    IServiceScope _scope;
    FooService _fooService;
    internal readonly record struct StockPriceMessage(StockSymbol StockSymbol, decimal StockPrice, DateTime Date);

    internal readonly record struct AddChartSeriesMessage(StockSymbol StockSymbol);

    internal readonly record struct RemoveChartSeriesMessage(StockSymbol StockSymbol);


    public LineChartingActor(IServiceProvider sp, PlotModel chartModel)
    {
        _scope =  sp.CreateScope();
        _fooService = _scope.ServiceProvider.GetRequiredService<FooService>();

        _chartModel = chartModel;

        _series = new Dictionary<string, LineSeries>();

        Receive<AddChartSeriesMessage>(AddSeriesToChart);
        Receive<RemoveChartSeriesMessage>(RemoveSeriesFromChart);
        Receive<StockPriceMessage>(HandleNewStockPrice);
    }


    protected override void PostStop()
    {
        _scope.Dispose();
    }

    private void AddSeriesToChart(AddChartSeriesMessage message)
    {
        if (!_series.ContainsKey(message.StockSymbol.Name))
        {
            var newLineSeries = new LineSeries
            {
                StrokeThickness = 2,
                MarkerSize = 3,
                MarkerStroke = OxyColors.Black,
                MarkerType = MarkerType.None,
                Color = message.StockSymbol.Color,
                CanTrackerInterpolatePoints = false,
                Title = message.StockSymbol.Name
            };

            _series.Add(message.StockSymbol.Name, newLineSeries);

            _chartModel.Series.Add(newLineSeries);

            RefreshChart();
        }
    }


    private void RemoveSeriesFromChart(RemoveChartSeriesMessage message)
    {
        if (_series.ContainsKey(message.StockSymbol.Name))
        {
            var seriesToRemove = _series[message.StockSymbol.Name];

            _chartModel.Series.Remove(seriesToRemove);

            _series.Remove(message.StockSymbol.Name);

            RefreshChart();
        }
    }

    private void HandleNewStockPrice(StockPriceMessage message)
    {
        if (_series.ContainsKey(message.StockSymbol.Name))
        {
            var series = _series[message.StockSymbol.Name];

            var newDataPoint = new DataPoint(DateTimeAxis.ToDouble(message.Date),
                LinearAxis.ToDouble(message.StockPrice));

            // Keep the last 10 data points on graph                
            if (series.Points.Count > 10)
            {
                series.Points.RemoveAt(0);
            }

            series.Points.Add(newDataPoint);

            RefreshChart();
        }
    }


    private void RefreshChart()
    {
        _chartModel.InvalidatePlot(true);
    }
}
