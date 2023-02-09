using System;
using System.Collections.Generic;
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
            Render();
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
                    RenderLoadSave(PromptSave());
                    break;
                case RenderMethod.EditSave:
                    RenderEditSave(PromptSave());
                    break;
                case RenderMethod.DeleteSave:
                    RenderDeleteSave(PromptSave());
                    break;

                default:
                    throw new Exception("Render method not found");
            }
        }

        private void RenderHome(string message = null)
        {
            //AnsiConsole.Clear();
            AnsiConsole.Write(new FigletText("EasySave").Centered().Color(Color.Red));
            if (message != null)
                AnsiConsole.MarkupLine(message);
            string action = ConsoleUtils.ChooseAction(Resource.HomeMenu_Title, new HashSet<string>() { Resource.HomeMenu_Create, Resource.HomeMenu_Load, Resource.HomeMenu_Edit, Resource.HomeMenu_Delete }, Resource.Forms_Exit);
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
            while (!DirectoryUtils.IsValidPath(dest))
            {
                ConsoleUtils.WriteError(Resource.Path_Invalid);
                dest = ConsoleUtils.Ask(Resource.CreateSave_Dest);
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

        private void RenderLoadSave(Save save)
        {
            
        }

        private void RenderEditSave(Save s)
        {
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

        private void RenderDeleteSave(Save s)
        {
            ConsoleUtils.WriteJson(Resource.Confirm, new JsonText(LogUtils.SaveToJson(s).ToString()));
            if (ConsoleUtils.AskConfirm())
            {
                vm.DeleteSave(s);
                RenderHome($"[yellow]{Resource.Save_Deleted}[/]");
            }
            else
            {
                RenderHome();
            }
        }

        private Save PromptSave()
        {
            string save = ConsoleUtils.ChooseAction(Resource.SaveMenu_Title, vm.GetSaves(), Resource.Forms_Back);
            if (save == Resource.Forms_Back)
                Render();
            else
                return vm.GetSaveByUuid(save);
            return null;
        }

        

    }
}