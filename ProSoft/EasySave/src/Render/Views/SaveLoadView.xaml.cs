
using EasySave.src.Models.Data;
using System.Windows;
using System;
using System.Windows.Controls;
using System.Collections.Generic;

namespace EasySave.src.Render.Views
{
    /// <summary>
    /// Interaction logic for HomeView.xaml
    /// </summary>
    public partial class SaveLoadView : UserControl
    {


        private void updateSaves()
        {
            LoadListBox.Items.Clear();
            foreach (Save s in Save.GetSaves())
            {
                LoadListBox.Items.Add(s.ToString());
            }
        }
    
        public SaveLoadView()
        {
            InitializeComponent();
            updateSaves();
        }

        private void LoadButton_Click(object sender, RoutedEventArgs e)
        {
            if (LoadListBox.SelectedItems.Count > 0)
            {
                HashSet<string> keys = new HashSet<string>();
                for (int i = 0; i < LoadListBox.SelectedItems.Count; i++)
                {
                    var selectedItem = LoadListBox.SelectedItems[i];
                    keys.Add(selectedItem.ToString());
                }
                
                ViewModel viewModel = new ViewModel();
                HashSet<Save> saves = viewModel.GetSavesByUuid(keys);
                foreach (Save s in saves)
                    s.Run();
                updateSaves();
            }
            else
            {
                MessageBox.Show("No item selected.");
            }
        }
    }
}
