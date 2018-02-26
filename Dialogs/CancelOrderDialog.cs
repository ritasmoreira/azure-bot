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
    public class CancelOrderDialog : LuisDialog<object>
    {


        public CancelOrderDialog() : base(new LuisService(new LuisModelAttribute(
          ConfigurationManager.AppSettings["LuisAppId"],
          ConfigurationManager.AppSettings["LuisAPIKey"],
          domain: ConfigurationManager.AppSettings["LuisAPIHostName"])))
        {
        }

        public Task StartAsync(IDialogContext context)
        {
            context.Wait(MessageReceivedAsync);

            return Task.CompletedTask;
        }

        private async Task MessageReceivedAsync(IDialogContext context, IAwaitable<object> result)
        {
            var activity = await result as IMessageActivity;

            // TODO: Put logic for handling user message here

            context.Wait(MessageReceivedAsync);
        }


        [LuisIntent("CancelOrder")]
        private async Task CancelOrderIntent(IDialogContext context, LuisResult result)
        {
            PromptDialog.Choice(context, this.OnOptionSelected, new List<string>() { "Sim", "Não" }, "Tem a certeza que quer cancelar a sua encomenda?", "A resposta que deu não é válida. Quer cancelar a encomenda?");

        }

        public async Task OnOptionSelected(IDialogContext context, IAwaitable<object> result)
        {
            var message = await result;

            if (message.Equals("Sim"))
            {
                await context.PostAsync($"A sua encomenda será cancelada. Obrigado");
            }
            else if (message.Equals("Não") || message.Equals("nao"))
            {
                await context.PostAsync($"A sua encomenda continua a caminho. Obrigado");
            }

            context.Done(true);
        }
    }
}