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
        public event PropertyChangedEventHandler PropertyChanged;
        
        public SaveCreateViewModel()
        {
            ObservableCollection<MenuItems> menuItems = new ObservableCollection<MenuItems>
            {
                new MenuItems {MenuName = $"{Resource.CreateSave_Type_Full}", MenuImage = "0"},
                new MenuItems {MenuName = $"{Resource.CreateSave_Type_Differential}", MenuImage = "1"},
            };

            CreateSaveItemsCollection = new CollectionViewSource { Source = menuItems };
        }


        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected bool SetField<T>(ref T field, T value, [CallerMemberName] string propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(field, value)) return false;
            field = value;
            OnPropertyChanged(propertyName);
            return true;
        }
    }
}