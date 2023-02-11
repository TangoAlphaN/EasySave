using EasySave.Properties;
using EasySave.src.Models.Data;
using EasySave.src.Utils;
using Newtonsoft.Json.Linq;
using Spectre.Console;
using Spectre.Console.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;

namespace EasySave.src.Render
{
    /// <summary>
    /// View class, represented by console
    /// </summary>
    class View
    {

        /// <summary>
        /// View model link
        /// </summary>
        private readonly ViewModel vm;

        /// <summary>
        /// default constructor
        /// </summary>
        public View()
        {
            vm = new ViewModel();
        }

        /// <summary>
        /// Start the view
        /// </summary>
        public void Start()
        {
            //UTF-8 is forced
            Console.OutputEncoding = Encoding.UTF8;
            RenderHome(CheckUpdate());
        }

        /// <summary>
        /// Main render method, rendering view depending on the case
        /// </summary>
        /// <param name="method">Render method, default to render home</param>
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

        /// <summary>
        /// Render home method
        /// </summary>
        /// <param name="message">custom message</param>
        private void RenderHome(string message = null)
        {
            //Write headers and custom message
            AnsiConsole.Clear();
            AnsiConsole.Write(new FigletText("EasySave").Centered().Color(Color.Red));
            if (message != null)
                AnsiConsole.MarkupLine(message);
            //Ask user for an action
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
                    Exit();
                    break;
            }
        }

        /// <summary>
        /// Render create save method
        /// </summary>
        private void RenderCreateSave()
        {
            if (Save.GetSaves().Count >= Save.MAX_SAVES)
            {
                AnsiConsole.MarkupLine(Resource.CreateSave_MaxSaves);
                Render();
            }
            //Ask for name, source, dest and type
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
            //Ask for confirm
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

        /// <summary>
        /// Render load save method
        /// </summary>
        /// <param name="saves">selected saves by prompt method</param>
        private void RenderLoadSave(HashSet<Save> saves)
        {
            if (saves.Count == 0)
            {
                RenderHome();
            }
            else
            {
                foreach (Save save in saves)
                {
                    try
                    {
                        if (save.GetStatus() != JobStatus.Waiting && save.GetStatus() != JobStatus.Finished)
                            RenderHome($"[red]{Resource.Save_AlreadyRunning}[/]");
                        else
                        {
                            //Ask confirm to run save
                            ConsoleUtils.WriteJson(Resource.Save_Run, new JsonText(LogUtils.SaveToJson(save).ToString()));
                            if (ConsoleUtils.AskConfirm())
                            {
                                //Write result
                                ConsoleUtils.WriteJson(Resource.Save_Info, new JsonText(vm.RunSave(save)), Color.Green);
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
            //Wait for user entry
            ConsoleUtils.AskConfirm(true);
            Render();
        }

        /// <summary>
        /// Render edit save method
        /// </summary>
        /// <param name="saves">selected save</param>
        private void RenderEditSave(HashSet<Save> saves)
        {
            if (saves.Count != 0)
            {
                //Get selected save
                Save s = saves.Single();
                string oldName = s.GetName();
                //Ask for new name
                string name = ConsoleUtils.Ask(Resource.CreateSave_Name);
                vm.EditSave(s, name);
                ConsoleUtils.WriteJson(Resource.Confirm, new JsonText(LogUtils.SaveToJson(s).ToString()));
                //Ask to confirm
                if (ConsoleUtils.AskConfirm())
                {
                    RenderHome($"[yellow]{Resource.Save_Renamed} ({s.uuid})[/]");
                }
                else
                {
                    //Get back old name
                    vm.EditSave(s, oldName);
                    RenderHome();
                }
            }
            else
                Render();
        }

        /// <summary>
        /// Render delete save method
        /// </summary>
        /// <param name="saves">selected save</param>
        private void RenderDeleteSave(HashSet<Save> saves)
        {
            if (saves.Count != 0)
            {
                //Get selected save
                Save s = saves.Single();
                ConsoleUtils.WriteJson(Resource.Confirm, new JsonText(LogUtils.SaveToJson(s).ToString()));
                //Ask confirm
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

        /// <summary>
        /// Prompt method for save selection
        /// </summary>
        /// <param name="multi">bool, allow mulitple selection if true</param>
        /// <returns>List of selected saves</returns>
        private HashSet<Save> PromptSave(bool multi = false)
        {
            HashSet<string> saves = new HashSet<string>();
            //Ask for multi or single save
            if (multi)
                saves = ConsoleUtils.ChooseMultiple(Resource.SaveMenu_Title, vm.GetSaves());
            else
            {
                string save = ConsoleUtils.ChooseAction(Resource.SaveMenu_Title, vm.GetSaves(), Resource.Forms_Back);
                if (save != Resource.Forms_Back && save != Resource.Forms_Exit)
                    saves.Add(save);
            }
            return vm.GetSavesByUuid(saves);
        }

        /// <summary>
        /// Exit method, stopping all saves
        /// </summary>
        /// <param name="code">exit code</param>
        internal void Exit(int code = 0)
        {
            vm.StopAllSaves();
            Environment.Exit(code);
        }

        /// <summary>
        /// Check for update view
        /// </summary>
        /// <returns>string resulting of check update method</returns>
        private string CheckUpdate()
        {
            bool upToDate = ViewModel.IsUpToDate();
            return ("\r\n\r\n" + (upToDate ? $"[green]{Resource.UpToDate}[/]" : $"[orange3]{Resource.NoUpToDate} [link]https://github.com/arnoux23u-CESI/EasySave/releases/latest[/][/]") + "\r\n");
        }

        /// <summary>
        /// Render change language method
        /// </summary>
        private void RenderChangeLanguage()
        {
            //Ask for lang
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