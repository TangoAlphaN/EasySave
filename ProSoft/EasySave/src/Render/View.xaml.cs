using EasySave.src.Models.Data;
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

        private readonly ViewModel vm;

        public View()
        {
            vm = new ViewModel();
        }

        public void Start()
        {
            InitializeComponent();
            Run();
            //RenderHome(CheckUpdate());
        }

        private void Render(RenderMethod method = RenderMethod.Home)
        {
            switch (method)
            {
                case RenderMethod.CreateSave:
                    RenderCreateSave();
                    break;
                case RenderMethod.LoadSave:
                    RenderLoadSave(PromptSave(true));
                    break;
                case RenderMethod.EditSave:
                    RenderEditSave(PromptSave());
                    break;
                case RenderMethod.DeleteSave:
                    RenderDeleteSave(PromptSave());
                    break;
                case RenderMethod.ChangeLanguage:
                    RenderChangeLanguage();
                    break;
                default:
                    RenderHome();
                    break;
            }
        }

        private void RenderHome(string message = null)
        {
            throw new NotImplementedException();
        }

        private void RenderCreateSave()
        {
            throw new NotImplementedException();
        }

        private void RenderLoadSave(HashSet<Save> saves)
        {
            throw new NotImplementedException();
        }

        private void RenderEditSave(HashSet<Save> saves)
        {
            throw new NotImplementedException();
        }

        private void RenderDeleteSave(HashSet<Save> saves)
        {
            throw new NotImplementedException();
        }

        private HashSet<Save> PromptSave(bool multi = false)
        {
            throw new NotImplementedException();
        }

        internal void Exit(int code = 0)
        {
            vm.StopAllSaves();
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