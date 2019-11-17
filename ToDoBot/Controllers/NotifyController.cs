using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Integration.AspNet.Core;
using Microsoft.Extensions.Configuration;

namespace ToDoBot.Controllers
{
    public class NotifyController : ControllerBase
    {
        private readonly IBotFrameworkHttpAdapter adapter;
        private readonly ConcurrentDictionary<string, SavedNotificationModel> savedNotifications;
        private readonly string appId;
        private string messageToSend;

        public NotifyController(IBotFrameworkHttpAdapter adapter, IConfiguration configuration,
            ConcurrentDictionary<string, SavedNotificationModel> savedNotifications)
        {
            this.adapter = adapter;
            this.appId = configuration.GetValue<string>("AppId");
            this.savedNotifications = savedNotifications;
            if (string.IsNullOrEmpty(appId))
            {
                this.appId = Guid.NewGuid().ToString();
            }
        }

        [HttpGet]
        [Route("api/notify")]
        public async Task<IActionResult> NotifyTimeCheck()
        {
            var notificationsToSend = savedNotifications.Where(x =>
                x.Value.RemindTime > DateTime.UtcNow && x.Value.RemindTime < DateTime.UtcNow.AddMinutes(1));
            foreach (var savedNotification in notificationsToSend)
            {
                this.messageToSend = savedNotification.Value.EventDescription;
                this.savedNotifications.TryRemove(savedNotification.Key, out SavedNotificationModel value);
                await ((BotAdapter) adapter).ContinueConversationAsync(this.appId, savedNotification.Value.ConversationReference,
                    BotCallback,
                    default(CancellationToken));
            }

            return this.Ok();
        }

        private async Task BotCallback(ITurnContext turnContext, CancellationToken cancellationToken)
        {
            await turnContext.SendActivityAsync(this.messageToSend);
        }
    }
}
