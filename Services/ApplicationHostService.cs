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
            Application.Current.Dispatcher.Invoke(() =>
            {
                // Keep app alive while login dialog is the only open window.
                Application.Current.ShutdownMode = ShutdownMode.OnExplicitShutdown;
            });
            var loginWindow = _serviceProvider.GetService(typeof(LoginWindow)) as LoginWindow;
            if (loginWindow != null)
            {
                var result = loginWindow.ShowDialog();
                _logger.LogInformation("Login dialog result: {Result}", result);
                if (result != true)
                {
                    Application.Current.Dispatcher.Invoke(() => Application.Current.Shutdown());
                    return;
                }
                else { 
                    _logger.LogInformation("User logged in successfully.");
                    // 登录成功后显示主窗口并导航到起始页
                   
                        _logger.LogInformation("Login successful, opening main window.");
                        if (!Application.Current.Windows.OfType<MainWindow>().Any())
                        {
                            _navigationWindow = (_serviceProvider.GetService(typeof(INavigationWindow)) as INavigationWindow)!;
                            _navigationWindow!.ShowWindow();
                            if (_navigationWindow is Window mainWindow)
                            {
                                Application.Current.MainWindow = mainWindow;
                                Application.Current.ShutdownMode = ShutdownMode.OnMainWindowClose;
                            }   
                            {
                                _logger.LogInformation("Navigating to dashboard after main window is loaded.");
                                _navigationWindow.Navigate(typeof(DashboardPage));
                            }
                        }
                }
            }
            await Task.CompletedTask;

        }
    }
}
