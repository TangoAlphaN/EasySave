
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
    public partial class SaveEditView : UserControl
    {
        private void updateSaves()
        {
            MyListBox.Items.Clear();
            foreach (Save s in Save.GetSaves())
            {
                MyListBox.Items.Add(s.ToString());
            }
        }
        public SaveEditView()
        {
            InitializeComponent();
            updateSaves();
            
            EditTextBox.Text = "Enter text here...";

            EditTextBox.GotFocus += RemovePlaceholder;
            EditTextBox.LostFocus += AddPlaceholder;
        }

        private void RemovePlaceholder(object sender, EventArgs e)
        {
            if (EditTextBox.Text == "Enter text here...")
            {
                EditTextBox.Text = "";
            }
        }

        private void AddPlaceholder(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(EditTextBox.Text))
                EditTextBox.Text = "Enter text here...";
        }

        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            object selectedItem = MyListBox.SelectedItem;
            if (selectedItem != null)
            {
                if (EditTextBox.Text == "Enter text here..." || EditTextBox.Text == "")
                {
                    MessageBox.Show("Please enter a new name.");
                    return;
                }

                HashSet<Save> saves = ViewModel.GetInstance().GetSavesByUuid(new HashSet<string>() { selectedItem.ToString() });

                foreach (Save save in saves)
                {
                    ViewModel.GetInstance().EditSave(save, EditTextBox.Text.ToString());
                    updateSaves();
                }
            }
            else
            {
                MessageBox.Show("No item selected.");
            }
        }
    }
}
