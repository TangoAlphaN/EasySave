﻿
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
    public class CryptoSoftSettingsViewModel : INotifyPropertyChanged
    {
        private CollectionViewSource SettingsItemsCollection;
        public ICollectionView SettingsSourceCollection => SettingsItemsCollection.View;

        public CryptoSoftSettingsViewModel()
        {
            ObservableCollection<CryptoSoftSettingsItem> settingsItem = new ObservableCollection<CryptoSoftSettingsItem>
            {
                new CryptoSoftSettingsItem { SettingsName = $"{Resource.Settings_Secret}", ChangeSettings = new RelayCommand(ViewModel.ChangeSettings) },
                new CryptoSoftSettingsItem { SettingsName = $"{Resource.Settings_Extensions}", ChangeSettings = new RelayCommand(ViewModel.ChangeSettings) },
            };

            SettingsItemsCollection = new CollectionViewSource { Source = settingsItem };
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(string propName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
        }

    }

}