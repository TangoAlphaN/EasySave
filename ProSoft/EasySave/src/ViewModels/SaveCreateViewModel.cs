using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Data;
using EasySave.src.Models;
using EasySave.src.Render;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Data;
using EasySave.Properties;
using EasySave.src.Models.Data;
using Microsoft.WindowsAPICodePack.Dialogs;
using System.Windows;
using System;

namespace EasySave.src.ViewModels
{
    public class SaveCreateViewModel : INotifyPropertyChanged
    {
        private CollectionViewSource CreateSaveItemsCollection;
        public ICollectionView CreateSaveSourceCollection => CreateSaveItemsCollection.View;
        
        public SaveCreateViewModel()
        {
            ObservableCollection<MenuItems> menuItems = new ObservableCollection<MenuItems>
            {
                //TODO saves ici
            };

            CreateSaveItemsCollection = new CollectionViewSource { Source = menuItems };
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(string propName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
        }
    }
}