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