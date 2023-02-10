using EasySave.Properties;
using EasySave.src.Models.Data;
using Spectre.Console;
using Spectre.Console.Json;
using System;
using System.Collections.Generic;
using System.Threading;

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
