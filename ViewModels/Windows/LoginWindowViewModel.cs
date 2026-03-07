using PatientStudy.Services;

namespace PatientStudy.ViewModels.Windows
{
    public partial class LoginWindowViewModel : ObservableObject
    {
        private readonly IAuthService _authService;

        public event Action<bool?>? RequestClose;

        public LoginWindowViewModel(IAuthService authService)
        {
            _authService = authService;
            LoginCommand = new AsyncRelayCommand(ExecuteLoginAsync, CanLogin);
            CancelCommand = new RelayCommand(ExecuteCancel);
        }

        [ObservableProperty]
        private string _username = string.Empty;

        // Password cannot be bound two-way to PasswordBox directly in WPF, we'll sync from code-behind
        [ObservableProperty]
        private string _password = string.Empty;

        [ObservableProperty]
        private string _errorMessage = string.Empty;

        [ObservableProperty]
        private bool _isBusy;

        public IAsyncRelayCommand LoginCommand { get; }
        public IRelayCommand CancelCommand { get; }

        private async Task ExecuteLoginAsync()
        {
            ErrorMessage = string.Empty;
            IsBusy = true;
            try
            {
                var ok = await _authService.ValidateCredentialsAsync(Username, Password);
                if (ok)
                {
                    // 成功 -> 通知窗口关闭并返回 true
                    RequestClose?.Invoke(true);
                }
                else
                {
                    ErrorMessage = "Invalid username or password";
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = "Login error";
                // 可记录日志 ex
            }
            finally
            {
                IsBusy = false;
            }
        }

        private void ExecuteCancel()
        {
            RequestClose?.Invoke(false);
        }

        // 这个方法返回 bool，决定按钮是否启用
        private bool CanLogin()
        {
            // 只有当用户名和密码都不为空（或空白字符）时，才允许点击
            return !string.IsNullOrWhiteSpace(Username) && !string.IsNullOrWhiteSpace(Password) &&
           !IsBusy;
        }
        // 当 Username 属性改变后，触发命令的可执行状态检查
        partial void OnUsernameChanged(string value)
        {
            // 重置错误信息
            ErrorMessage = string.Empty;
            LoginCommand.NotifyCanExecuteChanged();
        }

        // 当 Password 属性改变后，同理
        partial void OnPasswordChanged(string value)
        {
            // 重置错误信息
            ErrorMessage = string.Empty;
            LoginCommand.NotifyCanExecuteChanged(); 
        }
        // 别忘了在 IsBusy 改变时也刷新一下
        partial void OnIsBusyChanged(bool value) => LoginCommand.NotifyCanExecuteChanged();

    }
}
