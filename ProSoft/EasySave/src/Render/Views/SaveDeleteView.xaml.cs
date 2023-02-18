
using EasySave.src.Models.Data;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace EasySave.src.Render.Views
{
    /// <summary>
    /// Interaction logic for HomeView.xaml
    /// </summary>
    public partial class SaveDeleteView : UserControl
    {
        private void updateSaves()
        {
            DeleteListBox.Items.Clear();
            foreach (Save s in Save.GetSaves())
            {
                DeleteListBox.Items.Add(s.ToString());
            }
        }
        public SaveDeleteView()
        {
            InitializeComponent();
            updateSaves();
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (DeleteListBox.SelectedItems.Count > 0)
            {
                HashSet<string> keys = new HashSet<string>();
                for (int i = 0; i < DeleteListBox.SelectedItems.Count; i++)
                {
                    var selectedItem = DeleteListBox.SelectedItems[i];
                    keys.Add(selectedItem.ToString());
                }

                ViewModel viewModel = new ViewModel();
                HashSet<Save> saves = viewModel.GetSavesByUuid(keys);
                foreach (Save s in saves)
                    Save.Delete(s.uuid);
                updateSaves();
            }
            else
            {
                MessageBox.Show("No item selected.");
            }
        }

        private void dataGridView1_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }
}
