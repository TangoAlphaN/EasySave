
using EasySave.src.Models;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Data;

namespace EasySave.src.ViewModels
{
    public class HomeViewModel : INotifyPropertyChanged
    {
        private CollectionViewSource HomeItemsCollection;
        public ICollectionView HomeSourceCollection => HomeItemsCollection.View;

        public HomeViewModel()
        {
            ObservableCollection<HomeItems> homeItems = new ObservableCollection<HomeItems>
            {

            };

            HomeItemsCollection = new CollectionViewSource { Source = homeItems };
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string propName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
        }
    }

}