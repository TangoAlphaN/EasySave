
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
        private void SelectFilePathCommand(object sender, RoutedEventArgs e)
        {
            /*OpenFileDialog openFileDialog = new OpenFileDialog();
            if(openFileDialog.ShowDialog() == true)
                TxtSrc.Text = File.ReadAllText(openFileDialog.FileName);*/
            
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
            if(dialog.ShowDialog() == CommonFileDialogResult.Ok)
                TxtDest.Text = dialog.FileName;
        }

        /*private SaveType RadioCheck()
        {
            if(btnFull.IsChecked != null && (bool)btnFull.IsChecked)
                return Full;
            else
                return Differential;
        }*/

        public void CreateSave(Object sender, EventArgs e)
        {
            CreateNewSave();
        }
        
        public Save CreateNewSave()
        {
            SaveType type;
            if(btnFull.IsChecked != null && (bool)btnFull.IsChecked)
                type = Full;
            else
                type = Differential;
            return Save.CreateSave(CreatSaveName.Text, TxtSrc.Text, TxtDest.Text, type);
        }
        
        public SaveCreateView(Save newSave)
        {
            InitializeComponent();
        }

    }
}
