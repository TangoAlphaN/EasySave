using EasySave.Properties;
using EasySave.src.Models.Data;
using EasySave.src.Utils;
using EasySave.src.ViewModels;
using Notifications.Wpf;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
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