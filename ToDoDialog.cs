﻿using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Choices;
using Microsoft.Bot.Schema;
using Microsoft.Recognizers.Text;

namespace ToDoBot
{
    public class ToDoDialog : ComponentDialog
    {
        private readonly ConcurrentDictionary<string, SavedNotificationModel> conversationReferences;

        public ToDoDialog(ConcurrentDictionary<string, SavedNotificationModel> conversationReferences)
        {
            this.conversationReferences = conversationReferences;
            var waterfallSteps = new WaterfallStep[]
            {
                EventDescriptionStepAsync,
                RemindTimeStepAsync,
                ConfirmationEventStepAsync,
                EndOfDialogStepAsync
            };

            AddDialog(new WaterfallDialog(nameof(WaterfallDialog), waterfallSteps));
            AddDialog(new TextPrompt(nameof(TextPrompt)));
            AddDialog(new ChoicePrompt(nameof(ChoicePrompt)) { DefaultLocale= Culture.English });
            AddDialog(new ConfirmPrompt(nameof(ConfirmPrompt)) { DefaultLocale= Culture.English });
            InitialDialogId = nameof(WaterfallDialog);
        }

        private static async Task<DialogTurnResult> EventDescriptionStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var promptOptions = new PromptOptions {Prompt = MessageFactory.Text("Please enter event description.")};
            return await stepContext.PromptAsync(nameof(TextPrompt), promptOptions, cancellationToken);
        }

        private async Task<DialogTurnResult> RemindTimeStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            stepContext.Values["eventDescription"] = (string)stepContext.Result;
            var options = new PromptOptions()
            {
                Prompt = MessageFactory.Text("Select the time to remind"),
                Choices = ChoiceTimeCard.GetCard(),
                Style = ListStyle.HeroCard
            };
            return await stepContext.PromptAsync(nameof(ChoicePrompt), options, cancellationToken);
        }

        private async Task<DialogTurnResult> ConfirmationEventStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            DateTime notificationTime = DateTime.Parse(((FoundChoice)stepContext.Result).Value);
            stepContext.Values["remindTime"] = notificationTime;
            string eventDescription = (string)stepContext.Values["eventDescription"];
            string textToResponse =
                $"Are you sure you want to get notification about: {eventDescription} in {notificationTime:F}";
            var promptOptions = new PromptOptions { Prompt = MessageFactory.Text(textToResponse) };
            return await stepContext.PromptAsync(nameof(ConfirmPrompt), promptOptions, cancellationToken);
        }

        private async Task<DialogTurnResult> EndOfDialogStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            if ((bool)stepContext.Result)
            {
                var conversationReference = stepContext.Context.Activity.GetConversationReference();
                var description = (string)stepContext.Values["eventDescription"];
                var remindTime = (DateTime)stepContext.Values["remindTime"];
                var notificationModel = new SavedNotificationModel
                {
                    EventDescription = description,
                    RemindTime = remindTime,
                    ConversationReference = conversationReference
                };

                stepContext.ActiveDialog.State.TryGetValue("instanceId", out object instanceId);
                this.conversationReferences.AddOrUpdate((string)instanceId, notificationModel,
                    (key, newValue) => notificationModel);

                await stepContext.Context.SendActivityAsync(MessageFactory.Text("Thanks. Notification has been successfully saved."), cancellationToken);
                return await stepContext.EndDialogAsync(cancellationToken: cancellationToken);
            }
            else
            {
                await stepContext.Context.SendActivityAsync(MessageFactory.Text("Notification was discarded."), cancellationToken);
                return await stepContext.EndDialogAsync(cancellationToken: cancellationToken);
            }
        }
    }
}
