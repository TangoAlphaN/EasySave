using EasyClient.Properties;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading;
using System.Windows;
using System.Windows.Media;
using Wpf.Ui.Common.Interfaces;
using Button = Wpf.Ui.Controls.Button;

namespace EasyClient.Views
{
    /// <summary>
    /// Interaction logic for HomePage.xaml
    /// </summary>
    public partial class HomePage : INavigableView<ViewModel>, INotifyPropertyChanged
    {

        /// <summary>
        /// Reference to the view model.
        /// </summary>
        public ViewModel ViewModel
        {
            get;
        }

        /// <summary>
        /// Thread for updating data.
        /// </summary>
        private Thread _t;

        /// <summary>
        /// State of connection
        /// </summary>
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

        /// <summary>
        /// Connection informations
        /// </summary>
        private volatile bool logged = false;

        // Implement interface member for INotifyPropertyChanged.
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string propName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="viewModel">viewmodel</param>
        public HomePage(ViewModel viewModel)
        {
            ViewModel = viewModel;
            InitializeComponent();
            UpdateInterface();
        }

        /// <summary>
        /// Update the interface (buttons and data)
        /// </summary>
        private void UpdateInterface()
        {
            //Check connection state
            ConnectState = logged ? Resource.Disconnect : Resource.Connect;
            if (logged)
            {
                //Update buttons depending saves state 
                ViewConnected.Visibility = Visibility.Visible;
                ViewDisconnected.Visibility = Visibility.Collapsed;
                foreach (Button child in FindVisualChildren<Button>(ListViewSaves))
                    child.IsEnabled = UpdateButtonVisibility(child.Tag.ToString(), ViewModel.GetStatusByUuid(child.CommandParameter.ToString()));
            }
            else
            {
                ViewDisconnected.Visibility = Visibility.Visible;
                ViewConnected.Visibility = Visibility.Collapsed;
            }
        }

        /// <summary>
        /// Get button visibility depending of tag and save status
        /// </summary>
        /// <param name="tag">tag</param>
        /// <param name="status">status</param>
        /// <returns></returns>
        private bool UpdateButtonVisibility(string tag, string status)
        {
            bool b = status switch
            {
                "Running" => tag != "Play",
                "Paused" => tag != "Pause",
                "Finished" => tag != "Pause",
                "Canceled" => tag == "Play",
                "Error" => tag == "Stop",
                "Waiting" => tag == "Play",
                _ => false
            };
            return b;
        }

        /// <summary>
        /// Log in to server
        /// </summary>
        /// <param name="sender">defaultArg</param>
        /// <param name="e">defaultArg</param>
        private void Connect(object sender, RoutedEventArgs e)
        {
            //if no connection, connect to server
            if (!logged)
            {
                bool b = ViewModel.Connect();
                //if success, update interface
                if (b)
                {
                    logged = true;
                    FetchData();
                }
            }
            else
            {
                ViewModel.Disconnect();
                logged = false;
                if (_t != null)
                {
                    //interrupt the updating thread
                    _t.Interrupt();
                    _t = null;
                }
            }
            UpdateInterface();
        }

        /// <summary>
        /// Update data method
        /// </summary>
        private void FetchData()
        {
            if (_t == null)
            {
                //Run a thread to update data
                _t = new Thread(() =>
                {
                    while (logged)
                    {
#pragma warning disable S2486 // Generic exceptions should not be ignored
                        ViewModel.UpdateSaves();
                        try
                        {
                            Thread.Sleep(1000);
                        }
                        catch
#pragma warning disable S108 // Nested blocks of code should not be left empty
                        {
                        }
#pragma warning restore S108 // Nested blocks of code should not be left empty
#pragma warning restore S2486 // Generic exceptions should not be ignored
                        
                    }
                });
                _t.Start();
            }
        }

        /// <summary>
        /// Run a save
        /// </summary>
        /// <param name="sender">data object</param>
        /// <param name="e">args</param>
        private void PlaySave(object sender, RoutedEventArgs e)
        {
            string uuid = ((Button)sender).CommandParameter.ToString();
            ViewModel.PlaySave(uuid);
        }

        /// <summary>
        /// Pause a save
        /// </summary>
        /// <param name="sender">data object</param>
        /// <param name="e">args</param>
        private void PauseSave(object sender, RoutedEventArgs e)
        {
            string uuid = ((Button)sender).CommandParameter.ToString();
            ViewModel.PauseSave(uuid);
        }

        /// <summary>
        /// Stop a save
        /// </summary>
        /// <param name="sender">data object</param>
        /// <param name="e">args</param>
        private void StopSave(object sender, RoutedEventArgs e)
        {
            string uuid = ((Button)sender).CommandParameter.ToString();
            ViewModel.StopSave(uuid);
        }

        /// <summary>
        /// Internal method to find buttons in view (from microsoft docs)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="depObj"></param>
        /// <returns></returns>
        private static List<T> FindVisualChildren<T>(DependencyObject depObj) where T : DependencyObject
        {
            List<T> list = new List<T>();
            if (depObj != null)
            {
                for (int i = 0; i < VisualTreeHelper.GetChildrenCount(depObj); i++)
                {
                    DependencyObject child = VisualTreeHelper.GetChild(depObj, i);
                    if (child is T)
                        list.Add((T)child);
                    List<T> childItems = FindVisualChildren<T>(child);
                    if (childItems != null && childItems.Count > 0)
                        foreach (var item in childItems)
                            list.Add(item);
                }
            }
            return list;
        }

    }
}