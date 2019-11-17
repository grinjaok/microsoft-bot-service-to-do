using System;
using System.Collections.Generic;
using Microsoft.Bot.Builder.Dialogs.Choices;
using Microsoft.Bot.Schema;

namespace ToDoBot
{
    public class ChoiceTimeCard
    {
        public static List<Choice> GetCard()
        {
            return new List<Choice>
            {
                new Choice(DateTime.UtcNow.AddMinutes(2).ToString("F"))
                {
                    Action =  new CardAction(ActionTypes.ImBack, title: "In 2 minutes",
                        value: DateTime.UtcNow.AddMinutes(2).ToString("F")),
                },
                new Choice(DateTime.UtcNow.AddMinutes(5).ToString("F"))
                {
                    Action =  new CardAction(ActionTypes.ImBack, title: "In 5 minutes",
                        value: DateTime.UtcNow.AddMinutes(5).ToString("F")),
                },
                new Choice(DateTime.UtcNow.AddDays(1).ToString("F"))
                {
                    Action =  new CardAction(ActionTypes.ImBack, "Tomorrow the same time",
                        value: DateTime.UtcNow.AddDays(1).ToString("F")),
                }
            };
        }
    }
}
