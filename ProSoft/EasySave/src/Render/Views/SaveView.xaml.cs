
using EasySave.src.Models.Data;
using System.Windows;
using System;
using System.Windows.Controls;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Windows.Input;
using System.Windows.Navigation;
using EasySave.Properties;
using EasySave.src.ViewModels;
using Notifications.Wpf;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace EasySave.src.Render.Views
{
    /// <summary>
    /// Interaction logic for HomeView.xaml
    /// </summary>
    public partial class SaveView : UserControl
    {

        private void _updateSaves()
        {
            SaveListBox.Items.Clear();
            foreach (Save s in Save.GetSaves())
            {
                SaveListBox.Items.Add(s.ToString());
            }
        }

        public SaveView()
        {

            InitializeComponent();
            _updateSaves();
        }

        private void RunButton_Click(object sender, RoutedEventArgs e)
        {
            if (SaveListBox.SelectedItems.Count > 0)
            {
                PauseBtn.Visibility = Visibility.Visible;
                ResumeBtn.Visibility = Visibility.Visible;
                CancelBtn.Visibility = Visibility.Visible;
                HashSet<string> keys = new HashSet<string>();
                for (int i = 0; i < SaveListBox.SelectedItems.Count; i++)
                {
                    var selectedItem = SaveListBox.SelectedItems[i];
                    keys.Add(selectedItem.ToString());
                }

                HashSet<Save> saves = ((SaveViewModel)DataContext).GetSavesByUuid(keys);
                foreach (Save s in saves)
                    ((SaveViewModel)DataContext).RunSave(s);
                _updateSaves();
            }
            else
            {
                new NotificationManager().Show(new NotificationContent
                {
                    Title = "Save Error",
                    Message = Resource.NoSelected,
                    Type = NotificationType.Error
                });
            }
        }

        private void PauseButton_Click(object sender, RoutedEventArgs e)
        {
            if (SaveListBox.SelectedItems.Count > 0)
            {
                HashSet<string> keys = new HashSet<string>();
                for (int i = 0; i < SaveListBox.SelectedItems.Count; i++)
                {
                    var selectedItem = SaveListBox.SelectedItems[i];
                    keys.Add(selectedItem.ToString());
                }

                HashSet<Save> saves = ((SaveViewModel)DataContext).GetSavesByUuid(keys);
                foreach (Save s in saves)
                    ((SaveViewModel)DataContext).PauseSave(s);
                _updateSaves();
            }
            else
            {
                new NotificationManager().Show(new NotificationContent
                {
                    Title = "Save Error",
                    Message = Resource.NoSelected,
                    Type = NotificationType.Error
                });
            }
        }

        private void ResumeButton_Click(object sender, RoutedEventArgs e)
        {
            if (SaveListBox.SelectedItems.Count > 0)
            {
                HashSet<string> keys = new HashSet<string>();
                for (int i = 0; i < SaveListBox.SelectedItems.Count; i++)
                {
                    var selectedItem = SaveListBox.SelectedItems[i];
                    keys.Add(selectedItem.ToString());
                }

                HashSet<Save> saves = ((SaveViewModel)DataContext).GetSavesByUuid(keys);
                foreach (Save s in saves)
                    ((SaveViewModel)DataContext).ResumeSave(s);
                _updateSaves();
            }
            else
            {
                new NotificationManager().Show(new NotificationContent
                {
                    Title = "Save Error",
                    Message = Resource.NoSelected,
                    Type = NotificationType.Error
                });
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            if (SaveListBox.SelectedItems.Count > 0)
            {
                PauseBtn.Visibility = Visibility.Collapsed;
                ResumeBtn.Visibility = Visibility.Collapsed;
                CancelBtn.Visibility = Visibility.Collapsed;
                HashSet<string> keys = new HashSet<string>();
                for (int i = 0; i < SaveListBox.SelectedItems.Count; i++)
                {
                    var selectedItem = SaveListBox.SelectedItems[i];
                    keys.Add(selectedItem.ToString());
                }

                HashSet<Save> saves = ((SaveViewModel)DataContext).GetSavesByUuid(keys);
                foreach (Save s in saves)
                    ((SaveViewModel)DataContext).CancelSave(s);
                _updateSaves();
            }
            else
            {
                new NotificationManager().Show(new NotificationContent
                {
                    Title = "Save Error",
                    Message = Resource.NoSelected,
                    Type = NotificationType.Error
                });
            }

        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (SaveListBox.SelectedItems.Count > 0)
            {
                PauseBtn.Visibility = Visibility.Collapsed;
                ResumeBtn.Visibility = Visibility.Collapsed;
                CancelBtn.Visibility = Visibility.Collapsed;
                HashSet<string> keys = new HashSet<string>();
                for (int i = 0; i < SaveListBox.SelectedItems.Count; i++)
                {
                    var selectedItem = SaveListBox.SelectedItems[i];
                    keys.Add(selectedItem.ToString());
                }

                HashSet<Save> saves = ((SaveViewModel)DataContext).GetSavesByUuid(keys);
                foreach (Save s in saves)
                    ((SaveViewModel)DataContext).DeleteSave(s);
                _updateSaves();
            }
            else
            {
                new NotificationManager().Show(new NotificationContent
                {
                    Title = "Save Error",
                    Message = Resource.NoSelected,
                    Type = NotificationType.Error
                });
            }
        }


        private void GoTo(object sender, RoutedEventArgs e)
        {
            if (Application.Current.MainWindow != null)
            {
                //NavigationService navigationService = NavigationService.GetNavigationService(Application.Current.MainWindow);

                //SaveFrame.Visibility = Visibility.Collapsed;
                SaveCreateView createSave = new SaveCreateView();
                SaveFrame.NavigationService.Navigate(createSave);
            }
        }

    }
}
