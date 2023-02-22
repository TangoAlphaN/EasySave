using EasySave.Properties;
using EasySave.src.ViewModels;
using System.Windows.Controls;

namespace EasySave.src.Render.Views
{
    /// <summary>
    /// Interaction logic for HomeView.xaml
    /// </summary>
    public partial class SettingsView : UserControl
    {
        public SettingsView()
        {
            InitializeComponent();
        }

        private void TextChangedEventHandler(object senderObj, TextChangedEventArgs args)
        {
            TextBox sender = (TextBox)senderObj;
            switch (sender.Tag.ToString())
            {
                case var value when value == Resource.Settings_Secret:
                    SettingsViewModel.ChangeKey(sender.Text);
                    break;
                case var value when value == Resource.Settings_Extensions:
                    SettingsViewModel.ChangeExtensions(sender.Text);
                    break;
                case var value when value == Resource.Settings_Software_Package:
                    SettingsViewModel.ChangeProcess(sender.Text);
                    break;
                case var value when value == Resource.Settings_Priority_Files:
                    SettingsViewModel.ChangePriorityFiles(sender.Text);
                    break;
                case var value when value == Resource.Settings_LimitSize:
                    SettingsViewModel.ChangeLimitSize(sender.Text);
                    break;
            }
        }
    }
}
