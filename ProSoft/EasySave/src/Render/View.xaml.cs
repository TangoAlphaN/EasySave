using EasySave.src.Models.Data;
using EasySave.src.ViewModels;
using System;
using System.Collections.Generic;
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
            //TODO RenderHome(CheckUpdate());
        }

        internal void Exit(int code = 0)
        {
            SaveViewModel.StopAllSaves();
            Environment.Exit(code);
        }

        private string CheckUpdate()
        {
            throw new NotImplementedException();
        }

        private void RenderChangeLanguage()
        {
            throw new NotImplementedException();
        }

        public static object CreateProgressBar(Save s)
        {
            throw new NotImplementedException();
        }

        public static void WriteError(string v)
        {
            throw new NotImplementedException();
        }
    }
}