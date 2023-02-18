using System.Windows.Input;

namespace EasySave.src
{
    public class NavigationItem
    {
        public string NavigationName { get; set; }
        public string NavigationImage { get; set; }
    }

    public class HomeItems
    {
        public string HomeName { get; set; }
        public string HomeImage { get; set; }
    }

    public class SettingsItem
    {
        public string SettingsName { get; set; }
        public string SettingsValue { get; set; }
        public string SettingsImage { get; set; }
        public ICommand SettingsCommand { get; set; }
    }

}
