using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Data;
using EasySave.src.Models;

namespace EasySave.src.ViewModels
{
    public class SaveViewModel : INotifyPropertyChanged
    {
        private CollectionViewSource SaveItemsCollection;
        public ICollectionView HomeSourceCollection => SaveItemsCollection.View;
        public event PropertyChangedEventHandler PropertyChanged;
        
        public SaveViewModel()
        {
            ObservableCollection<SaveItems> saveItems = new ObservableCollection<SaveItems>
            {

            };

            SaveItemsCollection = new CollectionViewSource { Source = saveItems };
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