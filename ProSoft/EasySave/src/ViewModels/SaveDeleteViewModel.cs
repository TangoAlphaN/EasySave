﻿
using EasySave.src.Models;
using EasySave.src.Render;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Data;

namespace EasySave.src.ViewModels
{
    public class SaveDeleteViewModel : INotifyPropertyChanged
    {
        private CollectionViewSource SaveItemsCollection;
        public ICollectionView HomeSourceCollection => SaveItemsCollection.View;

        public SaveDeleteViewModel()
        {
            ObservableCollection<HomeItems> homeItems = new ObservableCollection<HomeItems>
            {

            };

            SaveItemsCollection = new CollectionViewSource { Source = homeItems };
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string propName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
        }
    }

}