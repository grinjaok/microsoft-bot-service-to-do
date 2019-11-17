using System;
using Microsoft.Bot.Schema;

namespace ToDoBot
{
    public class SavedNotificationModel
    {
        public string EventDescription { get; set; }

        public DateTime RemindTime { get; set; }

        public ConversationReference ConversationReference { get; set; }
    }
}
