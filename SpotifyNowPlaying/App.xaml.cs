using System;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Windows;
using CefSharp;
using CefSharp.Wpf;
using log4net;
using SpotifyNowPlaying.Common;
using SpotifyNowPlaying.Config;
using SpotifyNowPlaying.Output;
using SpotifyNowPlaying.Views;

namespace SpotifyNowPlaying
{
    public partial class App
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(App));
        
        public App()
        {
            // configure the logging
            LoggingHelper.Configure();

            // add Custom assembly resolver
            AppDomain.CurrentDomain.AssemblyResolve += Resolver;

            // any CefSharp references have to be in another method with NonInlining
            // attribute so the assembly resolver has time to do it's thing.
            InitializeCefSharp();
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private static void InitializeCefSharp()
        {
            var settings = new CefSettings();

            var path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), @"CefSharp\Cache");

            settings.CachePath = path;
            
            Cef.Initialize(settings, performDependencyCheck: true, browserProcessHandler: null);
        }
        
        private async void App_OnStartup(object sender, StartupEventArgs e)
        {
            await SpotifyClientHelper.Init();
            
            MainWindow = new MainWindow();
            MainWindow.Show();

            var window = new MainWindow();

            window.Closed += WindowOnClosed;

            ShutdownMode = ShutdownMode.OnExplicitShutdown;
        }

        private void WindowOnClosed(object sender, EventArgs e)
        {
            Environment.Exit(0);
        }

        private void App_OnExit(object sender, ExitEventArgs args)
        {
            try
            {
                OutputManager.Cleanup();
            }
            catch (Exception e)
            {
                log.Error($"An error occurred during application exit. {e.Message}", e);

                Environment.Exit(AppExitCode.UnhandledException);
            }
        }
        
        private static Assembly Resolver(object sender, ResolveEventArgs args)
        {
            // will attempt to load missing assembly from either x86 or x64 subdir
            // required by CefSharp to load the unmanaged dependencies when running using AnyCPU
            if (args.Name.StartsWith("CefSharp.Core.Runtime"))
            {
                var assemblyName = args.Name.Split(new[] { ',' }, 2)[0] + ".dll";
                var archSpecificPath = Path.Combine(AppDomain.CurrentDomain.SetupInformation.ApplicationBase,
                    Environment.Is64BitProcess ? "x64" : "x86",
                    assemblyName);

                return File.Exists(archSpecificPath)
                    ? Assembly.LoadFile(archSpecificPath)
                    : null;
            }

            return null;
        }
    }
}
