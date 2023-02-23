using EasySave.Properties;
using EasySave.src.Models.Data;
using EasySave.src.Utils;
using EasySave.src.ViewModels;
using Microsoft.WindowsAPICodePack.Dialogs;
using Notification.Wpf;
using System;
using System.IO;
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

        /// <summary>
        /// Type of save
        /// </summary>
        private SaveType _type;

        /// <summary>
        /// Default constructor
        /// </summary>
        public SaveCreateView()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Select file path command for source
        /// </summary>
        /// <param name="sender">sender</param>
        /// <param name="e">args</param>
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

        /// <summary>
        /// Select file path command for dest
        /// </summary>
        /// <param name="sender">sender</param>
        /// <param name="e">args</param>
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

        /// <summary>
        /// Check radio button
        /// </summary>
        /// <param name="sender">sender</param>
        /// <param name="e">args</param>
        private void RadioCheck(Object sender, EventArgs e)
        {
            if (btnFull.IsChecked == true)
                _type = Full;
            else if (btnDiff.IsChecked == true)
                _type = Differential;
        }

        /// <summary>
        /// Create new save
        /// </summary>
        /// <param name="sender">sender</param>
        /// <param name="routedEventArgs">args</param>
        private void CreateNewSave(Object sender, RoutedEventArgs routedEventArgs)
        {
            if (CreatSaveName.Text == null || CreatSaveName.Text.Length < 1)
            {
                NotificationUtils.SendNotification(
                title: "EasySave",
                message: Resource.Error,
                time: 3);
                return;
            }
            if (TxtSrc.Text == null || TxtSrc.Text.Length < 1 || !Directory.Exists(TxtSrc.Text) || TxtDest.Text == null || TxtDest.Text.Length < 1)
            {
                NotificationUtils.SendNotification(
                title: "EasySave",
                message: Resource.Path_Invalid,
                time: 3);
                return;
            }
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

        /// <summary>
        /// Back to save view
        /// </summary>
        /// <param name="sender">sender</param>
        /// <param name="e">args</param>
        private void BackBtnClick(Object sender, RoutedEventArgs e)
        {
            CreatSaveName.Text = "";
            TxtSrc.Text = "";
            TxtDest.Text = "";
            btnFull.IsChecked = false;
            btnDiff.IsChecked = false;
            CreateFrame.NavigationService.Navigate(new SaveView());
        }

    }

}
