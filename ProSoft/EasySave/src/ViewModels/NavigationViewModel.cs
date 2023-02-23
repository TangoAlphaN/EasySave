using EasySave.Properties;
using EasySave.src.Render;
using EasySave.src.Render.Views;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Data;
using System.Windows.Input;

namespace EasySave.src.ViewModels
{
    /// <summary>
    /// NavigationViewModel class
    /// </summary>
    class NavigationViewModel : INotifyPropertyChanged
    {

        /// <summary>
        /// Navigation items collection
        /// </summary>
        private readonly CollectionViewSource _navigationItemsCollection;
        public ICollectionView NavigationSourceCollection => _navigationItemsCollection.View;

        /// <summary>
        /// Default constructor
        /// </summary>
        public NavigationViewModel()
        {
            // ObservableCollection represents a dynamic data collection that provides notifications when items
            // get added, removed, or when the whole list is refreshed.
            ObservableCollection<NavigationItem> navigationItems = new ObservableCollection<NavigationItem>
            {
                new NavigationItem { NavigationName = $"{Resource.HomeMenu_Home}", NavigationImage = @"Assets/Home_Icon.png" },
                new NavigationItem { NavigationName = $"{Resource.HomeMenu_Saves}", NavigationImage = @"Assets/Drive_Icon.png" },
                new NavigationItem { NavigationName = $"{Resource.HomeMenu_Settings}", NavigationImage = @"Assets/services_icon.png" },
            };
            _navigationItemsCollection = new CollectionViewSource { Source = navigationItems };
            SelectedViewModel = new HomeViewModel();
        }

        // Implement interface member for INotifyPropertyChanged.
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string propName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
        }

        // Select ViewModel
        private object _selectedViewModel;
        public object SelectedViewModel
        {
            get => _selectedViewModel;
            set { _selectedViewModel = value; OnPropertyChanged("SelectedViewModel"); }
        }

        // Switch Views
        public void SwitchViews(string parameter)
        {
            switch (parameter)
            {
                case var value when value == Resource.HomeMenu_Home:
                    SelectedViewModel = new HomeViewModel();
                    break;
                case var value when value == Resource.HomeMenu_Saves:
                    SelectedViewModel = new SaveViewModel();
                    break;
                case var value when value == Resource.HomeMenu_Settings:
                    SelectedViewModel = new SettingsViewModel();
                    break;
            }
        }

        // Menu Button Command
        private ICommand _menucommand;
        public ICommand MenuCommand
        {
            get
            {
                if (_menucommand == null)
                {
                    _menucommand = new RelayCommand(param => SwitchViews((string)param));
                }
                return _menucommand;
            }
        }

        // Show Home View
        private void ShowHome()
        {
            SelectedViewModel = new HomeViewModel();
        }

        // Back button Command
        private ICommand _backHomeCommand;
        public ICommand BackHomeCommand
        {
            get
            {
                if (_backHomeCommand == null)
                {
                    _backHomeCommand = new RelayCommand(p => ShowHome());
                }
                return _backHomeCommand;
            }
        }

        // Close App
        public void CloseApp(object obj)
        {
            MainWindow win = obj as MainWindow;
            win.Close();
        }

        // Close App Command
        private ICommand _closeCommand;
        public ICommand CloseAppCommand
        {
            get
            {
                if (_closeCommand == null)
                {
                    _closeCommand = new RelayCommand(p => CloseApp(p));
                }
                return _closeCommand;
            }
        }

    }

}
