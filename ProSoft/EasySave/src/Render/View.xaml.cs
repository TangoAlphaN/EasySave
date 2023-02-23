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

        /// <summary>
        /// Default constructor
        /// </summary>
        public View()
        {
        }

        /// <summary>
        /// Start the view
        /// </summary>
        public void Start()
        {
            InitializeComponent();
            Run();
        }

        /// <summary>
        /// Exit the view
        /// </summary>
        /// <param name="code"></param>
        internal void Exit(int code = 0)
        {
            SaveViewModel.StopAllSaves();
            Environment.Exit(code);
        }
    }
}