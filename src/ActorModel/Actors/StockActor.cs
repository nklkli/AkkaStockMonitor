using Akka.Actor;
using Akka.DependencyInjection;
using ReactiveStock.ActorModel.Actors.UI;
using ReactiveStock.ActorModel.Messages;
using System;
using System.Collections.Generic;

namespace ReactiveStock.ActorModel.Actors;

internal class StockActor : ReceiveActor, IWithTimers
{
    private readonly StockSymbol _stockSymbol;
    readonly HashSet<IActorRef> _subsribers;
    IActorRef _priceLookupChild;
    public ITimerScheduler Timers { get; set; }


    internal readonly record struct SubscribeMessage(IActorRef Subscriber);

    internal readonly record struct UnsubscribeMessage(IActorRef Subscriber);

    internal readonly record struct UpdatedStockPriceMessage(StockSymbol StockSymbol, decimal Price, DateTime Date);



    public StockActor(StockSymbol stockSymbol)
    {
        _stockSymbol = stockSymbol;

        _subsribers = new HashSet<IActorRef>();

        _priceLookupChild = Context.ActorOf(DependencyResolver.For(Context.System).Props<StockPriceLookupActor>(),
                                            name: nameof(StockPriceLookupActor) + "_" + stockSymbol);

        Receive<SubscribeMessage>(message => _subsribers.Add(message.Subscriber));

        Receive<UnsubscribeMessage>(message => _subsribers.Add(message.Subscriber));

        Receive<RefreshStockPriceMessage>(msg => _priceLookupChild.Tell(msg));

        Receive<UpdatedStockPriceMessage>(msg =>
        {
            var stockPrice = msg.Price;

            var stockPriceMessage = new LineChartingActor.StockPriceMessage(_stockSymbol, stockPrice, msg.Date);

            foreach (var subsriber in _subsribers)
            {
                subsriber.Tell(stockPriceMessage);
            }
        });

    }

    protected override void PreStart()
    {

        Timers.StartPeriodicTimer(
            key: nameof(RefreshStockPriceMessage),
            msg: new RefreshStockPriceMessage(_stockSymbol),
            initialDelay: TimeSpan.FromSeconds(1),
            interval: TimeSpan.FromSeconds(1));

    }



}
