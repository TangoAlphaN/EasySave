
using EasySave.Properties;
using EasySave.src.Models;
using EasySave.src.Render;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Data;

namespace EasySave.src.ViewModels
{
    public class SettingsChangeLogsFormatViewModel : INotifyPropertyChanged
    {
        private CollectionViewSource SettingsItemsCollection;
        public ICollectionView LogsSourceCollection => SettingsItemsCollection.View;

        public SettingsChangeLogsFormatViewModel()
        {
            ObservableCollection<LogsItems> logsItems = new ObservableCollection<LogsItems>
            {
                new LogsItems { LogName = "JSON", LogImage = @"Assets/json.png", ChangeLogs = new RelayCommand(ViewModel.ChangeLogsFormat) },
                new LogsItems { LogName = "XML", LogImage = @"Assets/xml.png", ChangeLogs = new RelayCommand(ViewModel.ChangeLogsFormat) },
            };

            SettingsItemsCollection = new CollectionViewSource { Source = logsItems };
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string propName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
        }
    }

}