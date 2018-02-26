using System;
using System.Collections.Generic;
using System.Configuration;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Luis;
using Microsoft.Bot.Builder.Luis.Models;
using Microsoft.Bot.Connector;

namespace LuisBot.Dialogs
{
    [Serializable]
    public class ChangeOrderDialog : LuisDialog<object>
    {


        public ChangeOrderDialog() : base(new LuisService(new LuisModelAttribute(
          ConfigurationManager.AppSettings["LuisAppId"],
          ConfigurationManager.AppSettings["LuisAPIKey"],
          domain: ConfigurationManager.AppSettings["LuisAPIHostName"])))
        {
        }


       

        private async Task MessageReceivedAsync(IDialogContext context, IAwaitable<object> result)
        {
            var activity = await result as IMessageActivity;

            // TODO: Put logic for handling user message here

            context.Wait(MessageReceivedAsync);
        }

        [LuisIntent("ChangeOrder")]
        private async Task CancelOrderIntent(IDialogContext context, LuisResult result)
        {
            await context.PostAsync($"You have reached {result.Intents[0].Intent}.");
            context.Done(true);
        }

    }
}