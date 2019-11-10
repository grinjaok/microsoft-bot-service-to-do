using System;
using System.Collections.Generic;
using Microsoft.Bot.Schema;

namespace ToDoBot
{
    public class ChoiceTimeCard
    {
        public static HeroCard GetCard()
        {
            return new HeroCard
            {
                Text = "when to remind you?",
                Buttons = new List<CardAction>
                {
                    new CardAction(ActionTypes.ImBack, title: "In 5 minutes", value: DateTime.UtcNow.AddMinutes(5)),
                    new CardAction(ActionTypes.ImBack, "Tomorrow the same time", value: DateTime.UtcNow.AddDays(1))
                }
            };
        }
    }
}
