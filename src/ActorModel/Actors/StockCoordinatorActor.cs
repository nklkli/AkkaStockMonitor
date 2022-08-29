using Akka.Actor;
using ReactiveStock.ActorModel.Actors.UI;
using ReactiveStock.ActorModel.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReactiveStock.ActorModel.Actors;
internal class StockCoordinatorActor : ReceiveActor
{
    readonly IActorRef _chartingActor;
    readonly Dictionary<string, IActorRef> _stockActors;



    internal readonly record struct WatchStockMessage(StockSymbol StockSymbol);

    internal readonly record struct UnWatchStockMessage(StockSymbol StockSymbol);



    public StockCoordinatorActor(IActorRef chartingActor)
    {
        _chartingActor = chartingActor;

        _stockActors = new Dictionary<string, IActorRef>();

        Receive<WatchStockMessage>(WatchStock);

        Receive<UnWatchStockMessage>(UnWatchStock);
    }


    private void WatchStock(WatchStockMessage msg)
    {
        bool childActorNeedsCreating = !_stockActors.ContainsKey(msg.StockSymbol.Name);

        if (childActorNeedsCreating)
        {
            IActorRef newChildActor =
                Context.ActorOf(Props.Create(() => new StockActor(msg.StockSymbol)),
                                name: "StockActor_" + msg.StockSymbol);

            _stockActors.Add(msg.StockSymbol.Name, newChildActor);
        }

        _chartingActor.Tell(new LineChartingActor.AddChartSeriesMessage(msg.StockSymbol));

        _stockActors[msg.StockSymbol.Name].Tell(new StockActor.SubscribeMessage(_chartingActor));
    }


    private void UnWatchStock(UnWatchStockMessage msg)
    {
        if (!_stockActors.ContainsKey(msg.StockSymbol.Name))
        {
            return;
        }

        _chartingActor.Tell(new LineChartingActor.RemoveChartSeriesMessage(msg.StockSymbol));

        _stockActors[msg.StockSymbol.Name].Tell(new StockActor.UnsubscribeMessage(_chartingActor));
    }


}
