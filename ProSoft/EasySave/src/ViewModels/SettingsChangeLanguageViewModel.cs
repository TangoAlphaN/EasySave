
using EasySave.Properties;
using EasySave.src.Models;
using EasySave.src.Render;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Data;

namespace EasySave.src.ViewModels
{
    public class SettingsChangeLanguageViewModel : INotifyPropertyChanged
    {
        private CollectionViewSource SettingsItemsCollection;
        public ICollectionView LangSourceCollection => SettingsItemsCollection.View;

        public SettingsChangeLanguageViewModel()
        {
            ObservableCollection<LangItems> langItems = new ObservableCollection<LangItems>
            {
                new LangItems { LangName = $"{Resource.Lang_fr_FR}", LangImage = @"Assets/fr_FR.png", ChangeLanguage = new RelayCommand(ViewModel.ChangeLanguage), LangParam = "fr-FR" },
                new LangItems { LangName = $"{Resource.Lang_en_US}", LangImage = @"Assets/en_US.png", ChangeLanguage = new RelayCommand(ViewModel.ChangeLanguage), LangParam = "en-US" },
                new LangItems { LangName = $"{Resource.Lang_it_IT}", LangImage = @"Assets/it_IT.png", ChangeLanguage = new RelayCommand(ViewModel.ChangeLanguage), LangParam = "it-IT" },
                new LangItems { LangName = $"{Resource.Lang_ru_RU}", LangImage = @"Assets/ru_RU.png", ChangeLanguage = new RelayCommand(ViewModel.ChangeLanguage), LangParam = "ru-RU" },
            };

            SettingsItemsCollection = new CollectionViewSource { Source = langItems };
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(string propName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
        }

    }

}