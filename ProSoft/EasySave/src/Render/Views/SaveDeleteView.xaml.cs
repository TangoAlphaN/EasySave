
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
            object selectedItem = DeleteListBox.SelectedItem;
            if (selectedItem != null)
            {
                string uuid = selectedItem.ToString().Split(" - ")[1].Split(" | ")[0];
                Save.Delete(Guid.Parse(uuid));
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
