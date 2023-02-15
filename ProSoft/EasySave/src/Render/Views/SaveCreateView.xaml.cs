
using System;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using EasySave.src.Models.Data;
using Microsoft.Win32;
using Microsoft.WindowsAPICodePack.Dialogs;
using static EasySave.src.Models.Data.SaveType;

namespace EasySave.src.Render.Views
{
    /// <summary>
    /// Interaction logic for HomeView.xaml
    /// </summary>
    public partial class SaveCreateView : UserControl
    {
        SaveType _type;

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

        public void CreateNewSave(Object sender, RoutedEventArgs routedEventArgs)
        {
            CreateSave();
        }

        private Save CreateSave()
        {
            return Save.CreateSave(CreatSaveName.Text, TxtSrc.Text, TxtDest.Text, _type);
        }

        public SaveCreateView()
        {
            InitializeComponent();
        }

    }
}
