using System.Windows;
using Wpf.Ui.Common.Interfaces;

namespace EasyClient.Views
{
    /// <summary>
    /// Interaction logic for SettingsPage.xaml
    /// </summary>
    public partial class SettingsPage : INavigableView<ViewModel>
    {
        public ViewModel ViewModel
        {
            get;
        }

        public SettingsPage(ViewModel viewModel)
        {
            ViewModel = viewModel;

            InitializeComponent();
        }

        private void SaveSettings(object sender, RoutedEventArgs e)
        {
            ViewModel.SaveSettings(IpBox.Text, PortBox.Text);
        }

    }
}