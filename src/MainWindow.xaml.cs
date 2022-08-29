using Akka.Actor;
using Akka.DependencyInjection;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Legends;
using ReactiveStock;
using ReactiveStock.ActorModel.Actors;
using ReactiveStock.ActorModel.Actors.UI;
using System;
using System.Collections.Generic;
using System.Windows;

namespace GUI;
/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    Dictionary<string, IActorRef> _buttonActors = new Dictionary<string, IActorRef>();


    public MainWindow(ActorSystem actorSystem)
    {
        InitializeComponent();

       
        this.Chart.Model = CreatePlotModel(); 

        var chartingActorRef =
          actorSystem.ActorOf(
              DependencyResolver.For(actorSystem).Props<LineChartingActor>(this.Chart.Model));

        var stocksCoordinatorActorRef =
            actorSystem.ActorOf(
                Props.Create(() => new StockCoordinatorActor(chartingActorRef)), "StocksCoordinator");

        _buttonActors.Add("AAAA",
              actorSystem.ActorOf(Props.Create(() =>
                                                new ButtonActor(stocksCoordinatorActorRef, 
                                                                this.AAAA, 
                                                                new StockSymbol("AAAA", OxyColors.Red)))
                                        .WithDispatcher("akka.actor.synchronized-dispatcher")));

        _buttonActors.Add("BBBB",
             actorSystem.ActorOf(Props.Create(() =>
                     new ButtonActor(stocksCoordinatorActorRef,
                                                this.BBBB,
                                                new StockSymbol("BBBB", OxyColors.Blue)))
                        .WithDispatcher("akka.actor.synchronized-dispatcher")));

        _buttonActors.Add("CCCC",
             actorSystem.ActorOf(Props.Create(() =>
                     new ButtonActor(stocksCoordinatorActorRef, this.CCCC, new StockSymbol("CCCC", OxyColors.Green))).WithDispatcher("akka.actor.synchronized-dispatcher")));
    }





    private static PlotModel CreatePlotModel()
    {
        var plotModel = new PlotModel();

        plotModel.Legends.Add(new OxyPlot.Legends.Legend
        {
            LegendTitle = "Legend",
            LegendOrientation = LegendOrientation.Horizontal,
            LegendPlacement = LegendPlacement.Outside,
            LegendPosition = LegendPosition.TopRight,
            LegendBackground = OxyColor.FromAColor(200, OxyColors.White),
            LegendBorder = OxyColors.Black
        });

        var stockDateTimeAxis = new DateTimeAxis
        {
            Position = AxisPosition.Bottom,
            MajorGridlineStyle = LineStyle.Solid,
            MinorGridlineStyle = LineStyle.Dot,
            Title = "Date",
            StringFormat = "HH:mm:ss"
        };

        plotModel.Axes.Add(stockDateTimeAxis);


        var stockPriceAxis = new LinearAxis
        {
            Minimum = 0,
            MajorGridlineStyle = LineStyle.Solid,
            MinorGridlineStyle = LineStyle.Dot,
            Title = "Price"
        };

        plotModel.Axes.Add(stockPriceAxis);
        return plotModel;
    }

    private void Button_Click(object sender, RoutedEventArgs e)
    {
        _buttonActors["AAAA"].Tell(new ButtonActor.FlipToggleMessage());
    }

    private void Button_Click_1(object sender, RoutedEventArgs e)
    {
        _buttonActors["BBBB"].Tell(new ButtonActor.FlipToggleMessage());
    }

    private void Button_Click_2(object sender, RoutedEventArgs e)
    {
        _buttonActors["CCCC"].Tell(new ButtonActor.FlipToggleMessage());
    }
}
