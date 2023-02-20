using EasyClient.Properties;
using System.ComponentModel;
using System.Windows;
using Wpf.Ui.Common.Interfaces;

namespace EasyClient.Views
{
    /// <summary>
    /// Interaction logic for HomePage.xaml
    /// </summary>
    public partial class HomePage : INavigableView<ViewModel>, INotifyPropertyChanged
    {
        public ViewModel ViewModel
        {
            get;
        }

        private string _connectState;
        public string ConnectState
        {
            get => _connectState;
            set
            {
                _connectState = value;
                OnPropertyChanged("ConnectState");
            }
        }

        private static bool logged = false;

        // Implement interface member for INotifyPropertyChanged.
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string propName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
        }

        public HomePage(ViewModel viewModel)
        {
            ViewModel = viewModel;
            InitializeComponent();
            UpdateInterface();
        }

        private void UpdateInterface()
        {
            ConnectState = logged ? Resource.Disconnect : Resource.Connect;
            if (logged)
            {
                ViewConnected.Visibility = Visibility.Visible;
                ViewDisconnected.Visibility = Visibility.Collapsed;
            }
            else
            {
                ViewDisconnected.Visibility = Visibility.Visible;
                ViewConnected.Visibility = Visibility.Collapsed;
            }
        }

        private void Connect(object sender, RoutedEventArgs e)
        {
            if (!logged)
            {
                bool b = ViewModel.Connect();
                if (b)
                {
                    logged = true;
                    ViewModel.UpdateSaves();
                }
            }
            else
            {
                ViewModel.Disconnect();
                logged = false;
            }
            UpdateInterface();
        }
    }
}