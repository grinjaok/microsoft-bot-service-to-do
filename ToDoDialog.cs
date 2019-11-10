using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Schema;

namespace ToDoBot
{
    public class ToDoDialog : ComponentDialog
    {
        public ToDoDialog(ConversationState conversationState)
        {
            var waterfallSteps = new WaterfallStep[]
            {
                EventDescriptionStepAsync,
                RemindTimeStepAsync,
                ConfirmationEventStep,
                EndOfDialogStepAsync
            };

            AddDialog(new WaterfallDialog(nameof(WaterfallDialog), waterfallSteps));
            AddDialog(new TextPrompt(nameof(TextPrompt)));
            AddDialog(new AttachmentPrompt(nameof(AttachmentPrompt)));
            AddDialog(new ConfirmPrompt(nameof(ConfirmPrompt)));
            InitialDialogId = nameof(WaterfallDialog);
        }

        private static async Task<DialogTurnResult> EventDescriptionStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            return await stepContext.PromptAsync(nameof(TextPrompt), new PromptOptions { Prompt = MessageFactory.Text("Please enter event description.") }, cancellationToken);
        }

        private async Task<DialogTurnResult> RemindTimeStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            stepContext.Values["eventDescription"] = (string)stepContext.Result;
            
            return await stepContext.PromptAsync(nameof(AttachmentPrompt), new PromptOptions { Prompt = (Activity)MessageFactory.Attachment(ChoiceTimeCard.GetCard().ToAttachment()) }, cancellationToken);
        }

        private async Task<DialogTurnResult> ConfirmationEventStep(WaterfallStepContext stepContext,
            CancellationToken cancellationToken)
        {
            DateTime notificationTime = (DateTime)stepContext.Result;
            stepContext.Values["remindTime"] = notificationTime;
            string eventDescription = (string) stepContext.Values["eventDescription"];
            string textToResponse =
                $"Are you sure you want to get notification about: {eventDescription} in {notificationTime:F}";
            return await stepContext.PromptAsync(nameof(ConfirmPrompt), new PromptOptions { Prompt = MessageFactory.Text(textToResponse) }, cancellationToken);
        }

        private async Task<DialogTurnResult> EndOfDialogStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            if ((bool) stepContext.Result)
            {
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
