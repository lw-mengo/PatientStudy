using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using PatientStudy.Views.Pages;
using PatientStudy.Views.Windows;
using System.Diagnostics;
using System.Windows.Threading;
using Wpf.Ui;

namespace PatientStudy.Services
{
    /// <summary>
    /// Managed host of the application.
    /// </summary>
    public class ApplicationHostService : IHostedService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<ApplicationHostService> _logger;
        private INavigationWindow _navigationWindow;

        public ApplicationHostService(IServiceProvider serviceProvider,ILogger<ApplicationHostService> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        /// <summary>
        /// Triggered when the application host is ready to start the service.
        /// </summary>
        /// <param name="cancellationToken">Indicates that the start process has been aborted.</param>
        public async Task StartAsync(CancellationToken cancellationToken)
        {
            await HandleActivationAsync();
        }

        /// <summary>
        /// Triggered when the application host is performing a graceful shutdown.
        /// </summary>
        /// <param name="cancellationToken">Indicates that the shutdown process should no longer be graceful.</param>
        public async Task StopAsync(CancellationToken cancellationToken)
        {
            await Task.CompletedTask;
        }

        /// <summary>
        /// Creates main window during activation.
        /// </summary>
        private async Task HandleActivationAsync()
        {
            Application.Current.ShutdownMode = ShutdownMode.OnExplicitShutdown;

            _logger.LogInformation("ShutdownMode set to OnExplicitShutdown.");

            var loginWindow = _serviceProvider.GetRequiredService<LoginWindow>();

            _logger.LogInformation("Showing login dialog.");
            bool? loginResult = loginWindow.ShowDialog();

            _logger.LogInformation("Login dialog closed. Result: {Result}", loginResult);

            if (loginResult != true)
            {
                _logger.LogInformation("Login cancelled or failed. Application will shutdown.");
                ShutdownApplication();
                return;
            }

            _logger.LogInformation("Login succeeded. Preparing main window.");

            _navigationWindow = _serviceProvider.GetRequiredService<INavigationWindow>();

            if (_navigationWindow is not Window mainWindow)
            {
                _logger.LogError("Resolved INavigationWindow is not a WPF Window.");
                ShutdownApplication();
                return;
            }

            Application.Current.MainWindow = mainWindow;

            _logger.LogInformation("Showing main window.");
            _navigationWindow.ShowWindow();

            Application.Current.ShutdownMode = ShutdownMode.OnMainWindowClose;
            _logger.LogInformation("ShutdownMode switched to OnMainWindowClose.");

            _logger.LogInformation("Navigating to DashboardPage.");
            _navigationWindow.Navigate(typeof(DashboardPage));

            await Task.CompletedTask;

        }
        private void ShutdownApplication()
        {
            if (Application.Current.Dispatcher.CheckAccess())
            {
                Application.Current.Shutdown();
            }
            else
            {
                Application.Current.Dispatcher.Invoke(Application.Current.Shutdown);
            }
        }
    }
}
