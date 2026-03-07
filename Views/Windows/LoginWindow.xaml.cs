using PatientStudy.ViewModels.Windows;
using System.Windows.Controls;

namespace PatientStudy.Views.Windows
{
    /// <summary>
    /// LoginWindow.xaml 的交互逻辑
    /// </summary>
    public partial class LoginWindow
    {
        public LoginWindow(LoginWindowViewModel viewModel)
        {
            InitializeComponent();

            DataContext = viewModel;

            // 当 ViewModel 请求关闭时设置 DialogResult 并关闭窗口
            viewModel.RequestClose += (result) =>
            {
                // 在 WPF 中设置 DialogResult 之前窗口必须是模态的并且可设置
                DialogResult = result;
                Close();
            };

            // PasswordBox 无法直接双向绑定，手动同步到 ViewModel
            PasswordBox.PasswordChanged += (s, e) =>
            {
                viewModel.Password = PasswordBox.Password;
            };
        }
    }
}
