using EasyClient.Properties;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
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
                foreach (Button child in FindVisualChildren<Button>(ListViewSaves))
                     child.IsEnabled = UpdateButtonVisibility(child.Tag.ToString(), ViewModel.GetSaveByUuid(child.CommandParameter.ToString()));

                //ViewModel.UpdateSaves();
            }
            else
            {
                ViewDisconnected.Visibility = Visibility.Visible;
                ViewConnected.Visibility = Visibility.Collapsed;
            }
        }

        private bool UpdateButtonVisibility(string tag, string status)
        {
            bool b = status switch
            {
                "Running" => tag != "Play",
                "Paused" => tag != "Pause",
                "Finished" => tag == "Play",
                "Canceled" => tag == "Play",
                "Error" => tag == "Stop",
                "Waiting" => tag == "Play",
                _ => false
            };
            return b;
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

        private void PlaySave(object sender, RoutedEventArgs e)
        {
            string uuid = ((Button)sender).CommandParameter.ToString();
            ViewModel.PlaySave(uuid);
        }

        private void PauseSave(object sender, RoutedEventArgs e)
        {
            string uuid = ((Button)sender).CommandParameter.ToString();
            ViewModel.PauseSave(uuid);
        }

        private void StopSave(object sender, RoutedEventArgs e)
        {
            string uuid = ((Button)sender).CommandParameter.ToString();
            ViewModel.StopSave(uuid);
        }

        public static List<T> FindVisualChildren<T>(DependencyObject depObj) where T : DependencyObject
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