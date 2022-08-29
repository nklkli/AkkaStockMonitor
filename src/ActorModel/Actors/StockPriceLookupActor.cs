using Akka.Actor;
using ReactiveStock.ActorModel.Messages;
using ReactiveStock.ExternalServices;
using System;

namespace ReactiveStock.ActorModel.Actors;

internal class StockPriceLookupActor : ReceiveActor
{
	private readonly IStockPriceServiceGateway _stockPriceServiceGateway;

	public StockPriceLookupActor(IStockPriceServiceGateway stockPriceServiceGateway)
	{
		this._stockPriceServiceGateway = stockPriceServiceGateway;

		Receive<RefreshStockPriceMessage>(LookupStockPrice);
	}

	private void LookupStockPrice(RefreshStockPriceMessage message)
	{
		var latestPrice = _stockPriceServiceGateway.GetLatestPrice(message.StockSymbol);

		Sender.Tell(new StockActor.UpdatedStockPriceMessage(message.StockSymbol, latestPrice, DateTime.Now));
	}
}
