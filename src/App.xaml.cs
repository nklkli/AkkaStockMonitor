using Akka.Actor;
using Akka.Actor.Setup;
using Akka.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using ReactiveStock;
using ReactiveStock.ExternalServices;
using System.Windows;
using ServiceProvider = Microsoft.Extensions.DependencyInjection.ServiceProvider;

namespace GUI;
/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    private ServiceProvider serviceProvider;

    public App()
    {
        ServiceCollection services = new ServiceCollection();

        ActorSystem actorSystem = null;

        services.AddTransient<IStockPriceServiceGateway, RandomStockPriceServiceGateway>()
                .AddScoped<FooService>()
                .AddSingleton<MainWindow>()
                .AddSingleton(sp => actorSystem);

        serviceProvider = services.BuildServiceProvider();

        DependencyResolverSetup dependencyResolverSetup = DependencyResolverSetup.Create(serviceProvider);
        ActorSystemSetup actorSystemSetup = BootstrapSetup.Create().And(dependencyResolverSetup);

        actorSystem = Akka.Actor.ActorSystem.Create("ReactiveStockActorSystem", actorSystemSetup);

        
    }

    private void Application_Startup(object sender, StartupEventArgs e)
    {
        var mainWindow = serviceProvider.GetService<MainWindow>();
        mainWindow.Show();
    }

    private  void Application_Exit(object sender, ExitEventArgs e)
    {
        serviceProvider.GetRequiredService<ActorSystem>().Terminate();
    }
}
