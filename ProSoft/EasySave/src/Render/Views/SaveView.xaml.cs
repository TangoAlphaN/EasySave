
using EasySave.Properties;
using EasySave.src.Models.Data;
using EasySave.src.Utils;
using EasySave.src.ViewModels;
using Notification.Wpf;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace EasySave.src.Render.Views
{
    /// <summary>
    /// Interaction logic for HomeView.xaml
    /// </summary>
    public partial class SaveView : UserControl
    {
        string _selectedItem;
        JobStatus? _saveStatus = null;
        readonly SaveViewModel _viewModel;
        private ObservableCollection<Save> _saves;


        private void UpdateSaves()
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
            UpdateSaves();
            _viewModel = new SaveViewModel();
            this.DataContext = _viewModel;

        }

        private void OnSelectionChanged(object sender, SelectionChangedEventArgs selectionChangedEventArgs)
        {
            _selectedItem = (sender as ListBox)?.SelectedItem as string;
            if (((sender as ListBox).SelectedItems.Count > 0) && (_selectedItem != null))
            {
                PauseBtn.Visibility = Visibility.Visible;
                /*
                ResumeBtn.Visibility = Visibility.Visible;
                */
                CancelBtn.Visibility = Visibility.Visible;
                HashSet<string> keys = new HashSet<string>();
                for (int i = 0; i < SaveListBox.SelectedItems.Count; i++)
                {
                    var selectedItem = SaveListBox.SelectedItems[i];
                    if (selectedItem != null) keys.Add(selectedItem.ToString());
                }
                _saves = new ObservableCollection<Save>(_viewModel.GetSavesByUuid(keys));
                foreach (Save s in _saves)
                {
                    _saveStatus = _viewModel.GetSaveStatus(s);
                    s.PropertyChanged += Save_PropertyChanged;
                }
                UpdateButtonStatus();
            }
            else
            {

                PauseBtn.Visibility = Visibility.Collapsed;
                /*
                ResumeBtn.Visibility = Visibility.Collapsed;
                */
                CancelBtn.Visibility = Visibility.Collapsed;
            }
        }

        private void Save_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Status")
            {
                Save save = (Save)sender;
                _saveStatus = _viewModel.GetSaveStatus(save);

                UpdateButtonStatus();
            }
        }

        private void UpdateButtonStatus()
        {
            switch (_saveStatus)
            {
                case JobStatus.Running:
                    RunBtn.IsEnabled = false;
                    PauseBtn.IsEnabled = true;
                    /*
                    ResumeBtn.IsEnabled = true;
                    */
                    CancelBtn.IsEnabled = true;
                    break;
                case JobStatus.Paused:
                    RunBtn.IsEnabled = true;
                    PauseBtn.IsEnabled = false;
                    /*
                    ResumeBtn.IsEnabled = true;
                    */
                    CancelBtn.IsEnabled = true;
                    break;
                case JobStatus.Canceled:
                case JobStatus.Waiting:
                    RunBtn.IsEnabled = true;
                    PauseBtn.IsEnabled = false;
                    /*
                    ResumeBtn.IsEnabled = false;
                    */
                    CancelBtn.IsEnabled = false;
                    break;
            }

        }

        private void RunButton_Click(object sender, RoutedEventArgs e)
        {
            if (SaveListBox.SelectedItems.Count > 0)
            {
                HashSet<string> keys = new HashSet<string>();
                for (int i = 0; i < SaveListBox.SelectedItems.Count; i++)
                {
                    var selectedItem = SaveListBox.SelectedItems[i];
                    if (selectedItem != null) keys.Add(selectedItem.ToString());
                }

                HashSet<Save> saves = ((SaveViewModel)DataContext).GetSavesByUuid(keys);
                Parallel.ForEach(saves, save =>
                {
                    _saveStatus = _viewModel.GetSaveStatus(save);
                    switch (_saveStatus.ToString())
                    {
                        case "Waiting":
                            _viewModel.RunSave(save);
                            break;
                        case "Paused":
                            _viewModel.ResumeSave(save);
                            break;
                    }
                    save.PropertyChanged += Save_PropertyChanged;
                    UpdateProgressBar(save.CalculateProgress());
                });
                UpdateSaves();
                NotificationUtils.SendNotification(
                    title: "EasySave",
                    message: Resource.Success,
                    type: NotificationType.Success,
                    time: 15);
            }
            else
            {
                NotificationUtils.SendNotification(
                    title: $"EasySave - {Resource.Error}",
                    message: Resource.NoSelected,
                    type: NotificationType.Error,
                    time: 15);
            }
        }
        
        public void UpdateProgressBar(int value)
        {
            Dispatcher.Invoke(() =>
            {
                SaveProgressBar.Value = value;
            });
        }


        private void PauseButton_Click(object sender, RoutedEventArgs e)
        {
            if (SaveListBox.SelectedItems.Count > 0)
            {
                HashSet<string> keys = new HashSet<string>();
                for (int i = 0; i < SaveListBox.SelectedItems.Count; i++)
                {
                    var selectedItem = SaveListBox.SelectedItems[i];
                    if (selectedItem != null) keys.Add(selectedItem.ToString());
                }

                HashSet<Save> saves = ((SaveViewModel)DataContext).GetSavesByUuid(keys);
                foreach (Save s in saves)
                {
                    ((SaveViewModel)DataContext).PauseSave(s);
                    s.PropertyChanged += Save_PropertyChanged;
                }
                UpdateSaves();
            }
            else
            {
                NotificationUtils.SendNotification(
                    title: $"EasySave - {Resource.Error}",
                    message: Resource.ErrorMsg,
                    type: NotificationType.Error,
                    time: 15);
            }
        }

        /*
        private void ResumeButton_Click(object sender, RoutedEventArgs e)
        {
            if (SaveListBox.SelectedItems.Count > 0)
            {
                HashSet<string> keys = new HashSet<string>();
                for (int i = 0; i < SaveListBox.SelectedItems.Count; i++)
                {
                    var selectedItem = SaveListBox.SelectedItems[i];
                    if (selectedItem != null) keys.Add(selectedItem.ToString());
                }

                HashSet<Save> saves = ((SaveViewModel)DataContext).GetSavesByUuid(keys);
                foreach (Save s in saves)
                {
                    ((SaveViewModel)DataContext).ResumeSave(s);
                    s.PropertyChanged += Save_PropertyChanged;
                }
                UpdateSaves();
            }
            else
            {
                NotificationUtils.SendNotification(
                    title: $"EasySave - {Resource.Error}",
                    message: Resource.ErrorMsg,
                    type: NotificationType.Error,
                    time: 15);
            }
        }
        */

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            if (SaveListBox.SelectedItems.Count > 0)
            {
                HashSet<string> keys = new HashSet<string>();
                for (int i = 0; i < SaveListBox.SelectedItems.Count; i++)
                {
                    var selectedItem = SaveListBox.SelectedItems[i];
                    if (selectedItem != null) keys.Add(selectedItem.ToString());
                }

                HashSet<Save> saves = ((SaveViewModel)DataContext).GetSavesByUuid(keys);
                foreach (Save s in saves)
                {
                    ((SaveViewModel)DataContext).CancelSave(s);
                    s.PropertyChanged += Save_PropertyChanged;
                }
                UpdateSaves();
            }
            else
            {
                NotificationUtils.SendNotification(
                    title: $"EasySave - {Resource.Error}",
                    message: Resource.ErrorMsg,
                    type: NotificationType.Error,
                    time: 15);
            }

        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (SaveListBox.SelectedItems.Count > 0)
            {
                PauseBtn.Visibility = Visibility.Collapsed;
                /*
                ResumeBtn.Visibility = Visibility.Collapsed;
                */
                CancelBtn.Visibility = Visibility.Collapsed;
                HashSet<string> keys = new HashSet<string>();
                for (int i = 0; i < SaveListBox.SelectedItems.Count; i++)
                {
                    var selectedItem = SaveListBox.SelectedItems[i];
                    if (selectedItem != null) keys.Add(selectedItem.ToString());
                }

                HashSet<Save> saves = ((SaveViewModel)DataContext).GetSavesByUuid(keys);
                foreach (Save s in saves)
                    ((SaveViewModel)DataContext).DeleteSave(s);
                UpdateSaves();
            }
            else
            {
                NotificationUtils.SendNotification(
                    title: $"EasySave - {Resource.Error}",
                    message: Resource.NoSelected,
                    type: NotificationType.Error,
                    time: 15);
            }
        }


        private void GoTo(object sender, RoutedEventArgs e)
        {
            if (Application.Current.MainWindow != null)
            {
                SaveCreateView createSave = new SaveCreateView();
                SaveFrame.NavigationService.Navigate(createSave);
            }
        }

    }
}
