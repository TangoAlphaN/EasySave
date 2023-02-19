using Wpf.Ui.Common.Interfaces;
using Wpf.Ui.Mvvm.Interfaces;
using EasyClient;

namespace EasyClient.Views {
    /// <summary>
    /// Interaction logic for HomePage.xaml
    /// </summary>
    public partial class HomePage : INavigableView<ViewModel>
    {
        public ViewModel ViewModel
        {
            get;
        }

        public HomePage(ViewModel viewModel)
        {
            ViewModel = viewModel;

            InitializeComponent();
        }
    }
}