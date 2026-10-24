using System.Text;
using CommunityToolkit.Maui;
using CommunityToolkit.Maui.Markup;
using HaydCalculator.Core.DependencyInjection;
using HaydCalculator.Presentation.Pages.Main;
using MetroLog.MicrosoftExtensions;
using Microsoft.Extensions.Logging;
using Plugin.Maui.DebugRainbows;
using UraniumUI;

namespace HaydCalculator;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        MauiAppBuilder builder = MauiApp.CreateBuilder();

        builder
            .UseMauiApp<App>()
            .UseMauiCommunityToolkit()
            .UseMauiCommunityToolkitMarkup()
            .UseUraniumUI()
            .UseUraniumUIMaterial()
            .UseDebugRainbows(new DebugRainbowsOptions { ShowRainbows = false })
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });

        AddLogging(builder);

        builder.Services.AddCore();
        builder.Services.AddTransient<MainPage>();
        builder.Services.AddTransient<MainPageViewModel>();

        MauiApp mauiApp = builder.Build();

        MethodTimeLogger.Logger = mauiApp.Services.GetRequiredService<ILogger<App>>();

        return mauiApp;
    }

    private class LoggingLayout : MetroLog.Layouts.Layout
    {
        public override string GetFormattedString(MetroLog.LogWriteContext context, MetroLog.LogEventInfo info)
        {
            var text = new StringBuilder($"{info.Level}|{info.TimeStamp:HH:mm:ss:fff}|{info.Logger}|{info.Message}");

            if (info.Exception is not null)
            {
                text.Append($"|Exception:'{info.Exception.Message}' AT '{info.Exception.StackTrace}'");
            }

            return text.ToString();
        }
    }

    private static void AddLogging(MauiAppBuilder builder)
    {
        builder.Logging
            .SetMinimumLevel(LogLevel.Trace)
            .AddDebug()
            // log files in the app's cache folder, kept for two days
            .AddStreamingFileLogger(
                options =>
                {
                    options.Layout = new LoggingLayout();
                    options.RetainDays = 2;
                    options.MinLevel = LogLevel.Trace;
                    options.MaxLevel = LogLevel.Critical;
                    options.FolderPath = Path.Combine(
                        FileSystem.CacheDirectory,
                        "MetroLogs");
                });
    }
}
