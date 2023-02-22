using EasySave.src.Models.Data;
using EasySave.src.ViewModels;
using System;
using System.Windows;

namespace EasySave.src.Render
{
    /// <summary>
    /// View class, represented by WPF App
    /// </summary>
    partial class View : Application
    {

        public View()
        {
        }

        public void Start()
        {
            InitializeComponent();
            Run();
        }

        internal void Exit(int code = 0)
        {
            SaveViewModel.StopAllSaves();
            Environment.Exit(code);
        }
    }
}