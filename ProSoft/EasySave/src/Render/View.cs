using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Linq;
using System.Threading;
using EasySave.Properties;
using EasySave.src.Models.Data;
using EasySave.src.Utils;
using Newtonsoft.Json.Linq;
using Spectre.Console;
using Spectre.Console.Json;

namespace EasySave.src.Render
{
    class View
    {

        private readonly ViewModel vm;

        public View()
        {
            vm = new ViewModel();
        }

        public void Start()
        {
            Console.OutputEncoding = Encoding.UTF8;
            RenderHome(CheckUpdate());
        }

        private void Render(RenderMethod method = RenderMethod.Home)
        {
            switch (method)
            {
                case RenderMethod.Home:
                    RenderHome();
                    break;
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
                    throw new Exception("Render method not found");
            }
        }

        private void RenderHome(string message = null)
        {
            AnsiConsole.Clear();
            AnsiConsole.Write(new FigletText("EasySave").Centered().Color(Color.Red));
            if (message != null)
                AnsiConsole.MarkupLine(message);
            string action = ConsoleUtils.ChooseAction(Resource.HomeMenu_Title, new HashSet<string>() { Resource.HomeMenu_Create, Resource.HomeMenu_Load, Resource.HomeMenu_Edit, Resource.HomeMenu_Delete, Resource.HomeMenu_ChangeLanguage }, Resource.Forms_Exit);
            switch (action)
            {
                case var value when value == Resource.HomeMenu_Create:
                    Render(RenderMethod.CreateSave);
                    break;
                case var value when value == Resource.HomeMenu_Load:
                    Render(RenderMethod.LoadSave);
                    break;
                case var value when value == Resource.HomeMenu_Edit:
                    Render(RenderMethod.EditSave);
                    break;
                case var value when value == Resource.HomeMenu_Delete:
                    Render(RenderMethod.DeleteSave);
                    break;
                case var value when value == Resource.HomeMenu_ChangeLanguage:
                    Render(RenderMethod.ChangeLanguage);
                    break;
                default:
                    break;
            }
        }

        private void RenderCreateSave()
        {
            if(Save.GetSaves().Count >= Save.MAX_SAVES)
            {
                AnsiConsole.MarkupLine(Resource.CreateSave_MaxSaves);
                Render();
            }
            string name = ConsoleUtils.Ask(Resource.CreateSave_Name);
            string src = ConsoleUtils.Ask(Resource.CreateSave_Src);
            while (!DirectoryUtils.IsValidPath(src))
            {
                ConsoleUtils.WriteError(Resource.Path_Invalid);
                src = ConsoleUtils.Ask(Resource.CreateSave_Src);
            }
            string dest = ConsoleUtils.Ask(Resource.CreateSave_Dest);
            if (!DirectoryUtils.IsValidPath(dest))
            {
                new DirectoryInfo(dest).Create();
            }
            string type = ConsoleUtils.ChooseAction(Resource.CreateSave_Type, new HashSet<string>() { Resource.CreateSave_Type_Full, Resource.CreateSave_Type_Differential });
            dynamic data = new JObject();
            data.name = name;
            data.src = src;
            data.dest = dest;
            data.type = type;
            ConsoleUtils.WriteJson(Resource.Confirm, new JsonText(data.ToString()));
            if (ConsoleUtils.AskConfirm())
            {
                Save s = vm.CreateSave(name, src, dest, type == Resource.CreateSave_Type_Full ? SaveType.Full : SaveType.Differential);
                RenderHome($"[green]{Resource.CreateSave_Success} ({s.uuid})[/]");
            }
            else
            {
                RenderHome();
            }
        }

        private void RenderLoadSave(HashSet<Save> saves)
        {
            if (saves.Count == 0)
            {
                RenderHome();
            }
            else
            {
                foreach (Save save in saves) { 
                    try
                    {
                        if (save.GetStatus() != JobStatus.Waiting && save.GetStatus() != JobStatus.Finished)
                            RenderHome($"[red]{Resource.Save_AlreadyRunning}[/]");
                        else
                        {
                            ConsoleUtils.WriteJson(Resource.Save_Run, new JsonText(LogUtils.SaveToJson(save).ToString()));
                            if (ConsoleUtils.AskConfirm())
                            {
                                vm.RunSave(save);
                            }
                            else
                                RenderHome();
                        }
                    }
                    catch
                    {
                        ConsoleUtils.WriteError($"{Resource.Exception}");
                        Exit(-1);
                    }
                }
            }
                
            
            
        }

        private void RenderEditSave(HashSet<Save> saves)
        {
            if (saves.Count != 0)
            {
                Save s = saves.Single();
                string oldName = s.Name;
                string name = ConsoleUtils.Ask(Resource.CreateSave_Name);
                vm.EditSave(s, name);
                ConsoleUtils.WriteJson(Resource.Confirm, new JsonText(LogUtils.SaveToJson(s).ToString()));
                if (ConsoleUtils.AskConfirm())
                {
                    RenderHome($"[yellow]{Resource.Save_Renamed} ({s.uuid})[/]");
                }
                else
                {
                    vm.EditSave(s, oldName);
                    RenderHome();
                }
            }
            else
                Render();
        }

        private void RenderDeleteSave(HashSet<Save> saves)
        {
            if (saves.Count != 0)
            {
                Save s = saves.Single();
                ConsoleUtils.WriteJson(Resource.Confirm, new JsonText(LogUtils.SaveToJson(s).ToString()));
                if (ConsoleUtils.AskConfirm())
                {
                    vm.DeleteSave(s);
                    RenderHome($"[yellow]{Resource.Save_Deleted}[/]");
                }
                else
                {
                    Render();
                }
            }
            else
                Render();
        }

        private HashSet<Save> PromptSave(bool multi = false)
        {
            HashSet<string> saves = new HashSet<string>();
            if (multi)
                saves = ConsoleUtils.ChooseMultiple(Resource.SaveMenu_Title, vm.GetSaves());
            else {
                string save = ConsoleUtils.ChooseAction(Resource.SaveMenu_Title, vm.GetSaves(), Resource.Forms_Back);
                if(save != Resource.Forms_Back && save != Resource.Forms_Exit)
                    saves.Add(save);
            }
            return vm.GetSavesByUuid(saves);
        }

        internal void Exit(int code = 0)
        {
            vm.StopAllSaves();
            Environment.Exit(code);
        }
        
        private string CheckUpdate()
        {
            bool upToDate = vm.IsUpdated();
            return (upToDate ? $"[green]{Resource.UpToDate}[/]" : $"[orange3]{Resource.NoUpToDate}[link]https://github.com/arnoux23u-CESI/EasySave/releases/latest[/][/]");
        }

        private void RenderChangeLanguage()
        {
            string lang = ConsoleUtils.ChooseAction(Resource.ChangeLang, new HashSet<string> { Resource.Lang_fr_FR, Resource.Lang_en_US, Resource.Lang_ru_RU, Resource.Lang_it_IT }, Resource.Forms_Back);
            string culture = CultureInfo.CurrentCulture.ToString();
            switch (lang)
            {
                case var value when value == Resource.Lang_fr_FR:
                    culture = "fr-FR";
                    break;
                case var value when value == Resource.Lang_en_US:
                    culture = "en-US";
                    break;
                case var value when value == Resource.Lang_ru_RU:
                    culture = "ru-RU";
                    break;
                case var value when value == Resource.Lang_it_IT:
                    culture = "it-IT";
                    break;
            }
            CultureInfo cultureInfo = new CultureInfo(culture);
            Thread.CurrentThread.CurrentUICulture = cultureInfo;
            Thread.CurrentThread.CurrentCulture = cultureInfo;
            RenderHome();            
        }
    }
}