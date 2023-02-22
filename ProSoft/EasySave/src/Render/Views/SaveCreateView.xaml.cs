
using EasySave.Properties;
using EasySave.src.Models.Data;
using EasySave.src.Utils;
using EasySave.src.ViewModels;
using Microsoft.WindowsAPICodePack.Dialogs;
using Notification.Wpf;
using System;
using System.Windows;
using System.Windows.Controls;
using static EasySave.src.Models.Data.SaveType;

namespace EasySave.src.Render.Views
{
    /// <summary>
    /// Interaction logic for HomeView.xaml
    /// </summary>
    public partial class SaveCreateView : UserControl
    {
        private SaveType _type;

        private SaveView _sv;

        public SaveCreateView(SaveView saveView)
        {
            _sv = saveView;
            InitializeComponent();
        }

        private void SelectFilePathCommandSrc(object sender, RoutedEventArgs e)
        {
            CommonOpenFileDialog dialog = new CommonOpenFileDialog
            {
                InitialDirectory = "C:\\",
                IsFolderPicker = true
            };
            if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
                TxtSrc.Text = dialog.FileName;
        }

        private void SelectFilePathCommandDest(object sender, RoutedEventArgs e)
        {
            CommonOpenFileDialog dialog = new CommonOpenFileDialog
            {
                InitialDirectory = "C:\\",
                IsFolderPicker = true
            };
            if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
                TxtDest.Text = dialog.FileName;
        }

        private void RadioCheck(Object sender, EventArgs e)
        {
            if (btnFull.IsChecked == true)
                _type = Full;
            else if (btnDiff.IsChecked == true)
                _type = Differential;
        }

        private void CreateNewSave(Object sender, RoutedEventArgs routedEventArgs)
        {
            SaveViewModel.CreateSave(CreatSaveName.Text, TxtSrc.Text, TxtDest.Text, _type);
            NotificationUtils.SendNotification(
                title: "EasySave",
                message: Resource.Header_CreateSaveSuccess,
                type: NotificationType.Success,
                time: 15);
            CreatSaveName.Text = "";
            TxtSrc.Text = "";
            TxtDest.Text = "";
            btnFull.IsChecked = false;
            btnDiff.IsChecked = false;
        }
        

        private void BackBtnClick(Object sender, RoutedEventArgs e)
        {
            CreatSaveName.Text = "";
            TxtSrc.Text = "";
            TxtDest.Text = "";
            btnFull.IsChecked = false;
            btnDiff.IsChecked = false;
            CreateFrame.NavigationService.Navigate(_sv);
        }
    }
}
