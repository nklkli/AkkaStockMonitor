using Akka.Actor;
using ReactiveStock.ActorModel.Messages;

using System.Windows.Controls;

namespace ReactiveStock.ActorModel.Actors.UI;

class ButtonActor : ReceiveActor
{
    private readonly IActorRef _coordinatorActor;
    private readonly StockSymbol _stockSymbol;
    private readonly Button _button;


    internal readonly record struct FlipToggleMessage;


    public ButtonActor( IActorRef coordinatorActor,
        Button viewModel,
        StockSymbol stockSymbol)
    {
        _coordinatorActor = coordinatorActor;
        _button = viewModel;
        _stockSymbol = stockSymbol;

        ToggledOff();
    }

    private void ToggledOff()
    {
        Receive<FlipToggleMessage>(
            message =>
            {
                _coordinatorActor.Tell(new StockCoordinatorActor.WatchStockMessage(_stockSymbol));

                _button.Content=$"{_stockSymbol} ON";

                Become(ToggledOn);
            });
    }

    private void ToggledOn()
    {
        Receive<FlipToggleMessage>(
            message =>
            {
                
                _coordinatorActor.Tell(new StockCoordinatorActor.UnWatchStockMessage(_stockSymbol));

                _button.Content = $"{_stockSymbol} OFF";

                Become(ToggledOff);
            });
    }

}
