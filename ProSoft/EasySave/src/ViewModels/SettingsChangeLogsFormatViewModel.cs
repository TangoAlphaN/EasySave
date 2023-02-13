
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
        public ICollectionView HomeSourceCollection => SettingsItemsCollection.View;

        public SettingsChangeLogsFormatViewModel()
        {
            ObservableCollection<HomeItems> homeItems = new ObservableCollection<HomeItems>
            {
                 new HomeItems { HomeName = $"{Resource.Lang_fr_FR}", HomeImage = @"Assets/Lang_Icon.png" },
                 new HomeItems { HomeName = $"{Resource.Lang_en_US}", HomeImage = @"Assets/notepad_icon.png" },
                 new HomeItems { HomeName = $"{Resource.Lang_it_IT}", HomeImage = @"Assets/notepad_icon.png" },
                 new HomeItems { HomeName = $"{Resource.Lang_ru_RU}", HomeImage = @"Assets/notepad_icon.png" },
            };

            SettingsItemsCollection = new CollectionViewSource { Source = homeItems };
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string propName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
        }
    }

}