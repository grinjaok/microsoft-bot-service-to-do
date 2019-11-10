using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;

namespace ToDoBot
{
    public class ToDoDialog : ComponentDialog
    {
        private readonly IStatePropertyAccessor<ToDoModel> conversationAccessor;

        public ToDoDialog(ConversationState conversationState)
        {
            conversationAccessor = conversationState.CreateProperty<ToDoModel>("ToDoModel");

            var waterfallSteps = new WaterfallStep[]
            {
                EventDescriptionAsync
            };

            AddDialog(new WaterfallDialog(nameof(WaterfallDialog), waterfallSteps));
            AddDialog(new TextPrompt(nameof(TextPrompt)));

            InitialDialogId = nameof(WaterfallDialog);
        }

        private static async Task<DialogTurnResult> EventDescriptionAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            return await stepContext.PromptAsync(nameof(TextPrompt), new PromptOptions { Prompt = MessageFactory.Text("Please enter your name.") }, cancellationToken);
        }
    }
}
