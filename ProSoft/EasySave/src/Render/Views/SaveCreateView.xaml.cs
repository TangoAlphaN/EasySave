
using EasySave.src.Models.Data;
using EasySave.src.ViewModels;
using Microsoft.WindowsAPICodePack.Dialogs;
using System;
using System.Windows;
using System.Windows.Controls;
using EasySave.Properties;
using Notifications.Wpf;
using static EasySave.src.Models.Data.SaveType;

namespace EasySave.src.Render.Views
{
    /// <summary>
    /// Interaction logic for HomeView.xaml
    /// </summary>
    public partial class SaveCreateView : UserControl
    {
        SaveType _type;

        public SaveCreateView()
        {
            InitializeComponent();
        }

        private void SelectFilePathCommand(object sender, RoutedEventArgs e)
        {
            CommonOpenFileDialog dialog = new CommonOpenFileDialog();
            dialog.InitialDirectory = "C:\\";
            dialog.IsFolderPicker = true;
            if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
                TxtSrc.Text = dialog.FileName;
        }

        private void SelectFilePathCommandDest(object sender, RoutedEventArgs e)
        {
            CommonOpenFileDialog dialog = new CommonOpenFileDialog();
            dialog.InitialDirectory = "C:\\";
            dialog.IsFolderPicker = true;
            if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
                TxtDest.Text = dialog.FileName;
        }

        public void RadioCheck(Object sender, EventArgs e)
        {
            if (btnFull.IsChecked == true)
                _type = Full;
            else if (btnDiff.IsChecked == true)
                _type = Differential;
        }

        private void CreateNewSave(Object sender, RoutedEventArgs routedEventArgs)
        {
            SaveViewModel.CreateSave(CreatSaveName.Text, TxtSrc.Text, TxtDest.Text, _type);
            new NotificationManager().Show(new NotificationContent
            {
                Title = "Save Success",
                Message = Resource.Success,
                Type = NotificationType.Success
            });
        }
    }
}
