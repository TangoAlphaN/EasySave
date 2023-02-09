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
                    PromptSave();
                    break;
                case RenderMethod.EditSave:
                    PromptSave();
                    break;
                case RenderMethod.DeleteSave:
                    break;

                default:
                    throw new Exception("Render method not found");
            }
        }

        private void RenderHome(string message = null)
        {
            Console.Clear();
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
                    ///
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
            ConsoleUtils.WriteJson("test", new JsonText(data.ToString()));
            if (ConsoleUtils.AskConfirm())
                Save.CreateSave(name, src, dest, type == Resource.CreateSave_Type_Full ? SaveType.Full : SaveType.Differential);
            RenderHome($"[green]{Resource.CreateSave_Success}[/]");
        }

        private void PromptSave()
        {
            string save = ConsoleUtils.ChooseAction(Resource.SaveMenu_Title, ViewModel.GetSaves(), Resource.Forms_Back);
            if (save == Resource.Forms_Back)
                Render();
            else
                Render(RenderMethod.EditSave);
        }

        

    }
}