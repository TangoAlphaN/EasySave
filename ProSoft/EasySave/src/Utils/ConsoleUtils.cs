using EasySave.Properties;
using EasySave.src.Models.Data;
using Spectre.Console;
using Spectre.Console.Json;
using System;
using System.Collections.Generic;
using System.Threading;

namespace EasySave.src.Utils
{
    /// <summary>
    /// Static class to manage console input and output
    /// </summary>
    public static class ConsoleUtils
    {

        /// <summary>
        /// Display a menu with unqiue choice
        /// </summary>
        /// <param name="title">question asked</param>
        /// <param name="choices">list of choices</param>
        /// <param name="lastOption">last option of the prompt (e.g. back to menu)</param>
        /// <returns>choice selected</returns>
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

        /// <summary>
        /// Display a menu with multiple choice
        /// </summary>
        /// <param name="title">question asked</param>
        /// <param name="choices">list of choices</param>
        /// <param name="lastOption">last option of the prompt (e.g. back to menu)</param>
        /// <returns>choices selected</returns>
        public static HashSet<string> ChooseMultiple(string title, HashSet<string> choices, string lastOption = null)
        {
            if (lastOption != null)
                choices.Add(lastOption);
            return new HashSet<string>(AnsiConsole.Prompt(
                new MultiSelectionPrompt<string>()
                    .Title(title)
                    .PageSize(10)
                    .NotRequired()
                    .InstructionsText(
                        "[grey](Press [blue]<space>[/] to toggle a fruit, " +
                        "[green]<enter>[/] to accept)[/]"
                    )
                    .AddChoiceGroup(Resource.All, choices))
            );
        }

        /// <summary>
        /// Display a confirmation message
        /// </summary>
        /// <param name="withoutChoice">just wait for entry if true</param>
        /// <returns>yes or no, yes if param bool is true</returns>
        public static bool AskConfirm(bool withoutChoice = false)
        {
            if (withoutChoice)
                return ChooseAction("", new HashSet<string>() { Resource.Forms_Back }) == Resource.Confirm_Yes;
            else
                return ChooseAction(Resource.Confirm, new HashSet<string>() { Resource.Confirm_Yes, Resource.Confirm_No }) == Resource.Confirm_Yes;
        }

        /// <summary>
        /// Display a json in a panel
        /// </summary>
        /// <param name="title">json title</param>
        /// <param name="data">json data</param>
        /// <param name="color">json color</param>
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

        /// <summary>
        /// prompt user entry
        /// </summary>
        /// <param name="title">title for choice</param>
        /// <param name="errorMessage">error message displayed before, default is null</param>
        /// <returns>user entry</returns>
        public static string Ask(string title, string errorMessage = null)
        {
            return AnsiConsole.Prompt(
                new TextPrompt<string>(title)
                    .PromptStyle("green")
                    .ValidationErrorMessage(errorMessage ?? "")
                    .Validate(prompt => !String.IsNullOrEmpty(prompt.Trim()))
            );

        }

        /// <summary>
        /// write error in red
        /// </summary>
        /// <param name="error">error to display</param>
        public static void WriteError(string error)
        {
            AnsiConsole.Markup($"[red]{error}[/]");
        }

        /// <summary>
        /// method to create progress bar
        /// </summary>
        /// <param name="s">save concerned by progress bar</param>
        public static void CreateProgressBar(Save s)
        {
            AnsiConsole.Progress()
                .Columns(new TaskDescriptionColumn(), new ProgressBarColumn(), new PercentageColumn(), new RemainingTimeColumn(), new SpinnerColumn())
                .Start(context =>
                {
                    var progress = context.AddTask($"{Resource.Copy}", maxValue: s.SrcDir.GetSize());
                    while (!context.IsFinished)
                    {
                        Thread.Sleep(1000);
                        progress.Value = s.GetSizeCopied();
                    }
                });
        }

    }
}
