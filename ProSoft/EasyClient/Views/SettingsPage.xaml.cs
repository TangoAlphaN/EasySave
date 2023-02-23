using System.Windows;
using Wpf.Ui.Common.Interfaces;

namespace EasyClient.Views
{
    /// <summary>
    /// Interaction logic for SettingsPage.xaml
    /// </summary>
    public partial class SettingsPage : INavigableView<ViewModel>
    {

        /// <summary>
        /// ViewModel reference
        /// </summary>
        public ViewModel ViewModel
        {
            get;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="viewModel">viewmodel</param>
        public SettingsPage(ViewModel viewModel)
        {
            ViewModel = viewModel;

            InitializeComponent();
        }

        /// <summary>
        /// Save settings to json
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SaveSettings(object sender, RoutedEventArgs e)
        {
            ViewModel.SaveSettings(IpBox.Text, PortBox.Text);
        }

    }
}