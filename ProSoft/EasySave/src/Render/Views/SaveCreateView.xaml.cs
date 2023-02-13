
using System;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Microsoft.Win32;
using Microsoft.WindowsAPICodePack.Dialogs;

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
            if(dialog.ShowDialog() == CommonFileDialogResult.Ok)
                TxtSrc.Text = dialog.FileName;
        }
        
        private void SelectFilePathCommandDest(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            if(openFileDialog.ShowDialog() == true)
                TxtDest.Text = File.ReadAllText(openFileDialog.FileName);
        }
        
        public SaveCreateView()
        {
            InitializeComponent();
        }

    }
}
