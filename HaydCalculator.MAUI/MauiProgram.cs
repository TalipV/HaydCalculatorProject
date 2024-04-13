using CommunityToolkit.Maui;
using HaydCalculator;
using HaydCalculator.Core.Calculator.Services;
using MauiTestApp.Presentation.View;
using MauiTestApp.Presentation.ViewModel;
using Plugin.Maui.DebugRainbows;
using UraniumUI;

namespace MauiTestApp
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            MauiAppBuilder builder = MauiApp.CreateBuilder();

            builder
                .UseMauiApp<App>()
                .UseMauiCommunityToolkit()
                .UseUraniumUI()
                .UseUraniumUIMaterial()
                .UseDebugRainbows(new DebugRainbowsOptions { ShowRainbows = false })
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

            builder.Services.AddTransient<MainPage>();
            builder.Services.AddTransient<MainPageViewModel>();
            builder.Services.AddTransient<HaydCalculatorService>();

            return builder.Build();
        }
    }
}