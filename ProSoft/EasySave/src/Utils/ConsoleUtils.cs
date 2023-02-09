using EasySave.Properties;
using Newtonsoft.Json.Linq;
using Spectre.Console;
using Spectre.Console.Json;
using System;
using System.Collections.Generic;

namespace EasySave.src.Utils
{
    public static class ConsoleUtils
    {
        public static string ChooseAction(string title, HashSet<string> choices, string lastOption = null)
        {
            if (lastOption != null)
                choices.Add(lastOption);
            return AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title(title)
                    .PageSize(10)
                    .AddChoices(choices)
            );
        }

        public static bool AskConfirm()
        {
            return ChooseAction(Resource.Confirm, new HashSet<string>() { Resource.Confirm_Yes, Resource.Confirm_No }) == Resource.Confirm_Yes;
        }

        public static void WriteJson(string title, JsonText data, Color? color = null)
        {
            AnsiConsole.Write(
                new Panel(data)
                    .Header(title)
                    .Collapse()
                    .RoundedBorder()
                    .BorderColor(color ?? Color.Yellow)
            );
        }

        public static string Ask(string title, string errorMessage = null)
        {
            return AnsiConsole.Prompt(
                new TextPrompt<string>(title)
                    .PromptStyle("green")
                    .ValidationErrorMessage(errorMessage ?? "")
                    .Validate(prompt => !String.IsNullOrEmpty(prompt.Trim()))
            );

        }

        public static void WriteError(string error)
        {
            AnsiConsole.Markup($"[red]{error}[/]");
        }
    }
}
