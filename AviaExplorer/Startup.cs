using AviaExplorer.Services.Avia.RestClient;
using AviaExplorer.Services.Utils.Analytics;
using AviaExplorer.Services.Utils.Language;
using AviaExplorer.Services.Utils.Message;
using AviaExplorer.Services.Utils.Navigation;
using AviaExplorer.Services.Utils.Settings;
using AviaExplorer.Services.Utils.Shell;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;

namespace AviaExplorer
{
    public static class Startup
    {
        public static App Init(Action<IServiceCollection> nativeConfigureServices)
        {
            var host = Host
                .CreateDefaultBuilder()
                .ConfigureServices(x =>
                {
                    nativeConfigureServices(x);
                    ConfigureServices(x);
                })
                .RegisterRoutes()
                .Build();

            App.Services = host.Services;

            return App.Services.GetService<App>();
        }

        private static void ConfigureServices(IServiceCollection services)
        {
            #region Services
            services.AddSingleton<IAnalyticsService, AnalyticsService>();
            services.AddSingleton<ILanguageService, LanguageService>();
            services.AddSingleton<IMessageService, MessageService>();
            services.AddSingleton<INavigationService, NavigationService>();
            services.AddSingleton<ISettingsService, SettingsService>();
            services.AddSingleton<IShellService, ShellService>();

            services.AddSingleton<IRestClientProvider, RestClientProvider>();
            #endregion

            #region ViewModels
            #endregion

            #region Application
            services.AddSingleton<App>();
            #endregion
        }

        //public static void ExtractSaveResource(string filename, string location)
        //{
        //    var a = Assembly.GetExecutingAssembly();
        //    using (var resFilestream = a.GetManifestResourceStream(filename))
        //    {
        //        if (resFilestream != null)
        //        {
        //            var full = Path.Combine(location, filename);

        //            using (var stream = File.Create(full))
        //            {
        //                resFilestream.CopyTo(stream);
        //            }
        //        }
        //    }
        //}
    }

    static class StartupExtensions
    {
        public static IHostBuilder RegisterRoutes(this IHostBuilder host)
        {
            return host;
        }
    }
}