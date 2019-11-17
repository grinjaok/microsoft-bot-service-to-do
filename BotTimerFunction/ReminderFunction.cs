using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;

namespace BotTimerFunction
{
    public static class ReminderFunction
    {
        [FunctionName("ReminderFunction")]
        public static async Task RunAsync([TimerTrigger("00:01:00")]TimerInfo myTimer)
        {
            var botUrl = Environment.GetEnvironmentVariable("BotUrl");
            var notificationEndpoint = Environment.GetEnvironmentVariable("NotificationEnpoint");

            using (HttpClient httpClient = new HttpClient())
            {
                var response = await httpClient.GetAsync($"{botUrl}/{notificationEndpoint}");
            }
        }
    }
}
