using System;
using System.Collections.Generic;
using System.Configuration;
using System.Threading;
using System.Threading.Tasks;
using LuisBot.Dialogs;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Luis;
using Microsoft.Bot.Builder.Luis.Models;
using Microsoft.Bot.Connector;

namespace Microsoft.Bot.Sample.LuisBot
{
    // For more information about this template visit http://aka.ms/azurebots-csharp-luis
    [Serializable]
    public class BasicLuisDialog : LuisDialog<object>
    {
        public BasicLuisDialog() : base(new LuisService(new LuisModelAttribute(
            ConfigurationManager.AppSettings["LuisAppId"], 
            ConfigurationManager.AppSettings["LuisAPIKey"], 
            domain: ConfigurationManager.AppSettings["LuisAPIHostName"])))
        {
        }

        [LuisIntent("None")]
        public async Task NoneIntent(IDialogContext context, LuisResult result)
        {
            await this.ShowLuisResult(context, result);
        }
        // Comentario

        // Go to https://luis.ai and create a new intent, then train/publish your luis app.
        // Finally replace "Gretting" with the name of your newly created intent in the following handler
        [LuisIntent("FindOrder")]
        public async Task FindOrderIntent(IDialogContext context, LuisResult result)
        {
            await this.ShowLuisResult(context, result);

            //await context.Forward(new FindOrderDialog(), ResumeAfterFindOrderDialog, result, CancellationToken.None);
     
        }

        [LuisIntent("Cancel")]
        public async Task CancelIntent(IDialogContext context, LuisResult result)
        {
            await this.ShowLuisResult(context, result);
        }

        [LuisIntent("Help")]
        public async Task HelpIntent(IDialogContext context, LuisResult result)
        {
            await this.ShowLuisResult(context, result);
        }

       

        private async Task ResumeAfterFindOrderDialog(IDialogContext context, IAwaitable<string> result)
        {
            // Store the value that NewOrderDialog returned. 
            // (At this point, new order dialog has finished and returned some value to use within the root dialog.)
            var resultFromNewOrder = await result;

            await context.PostAsync($"New order dialog just told me this: {resultFromNewOrder}");

            // Again, wait for the next message from the user.
           // context.Wait(this.MessageReceivedAsync);
        }


        private async Task MessageReceivedAsync(IDialogContext context, IAwaitable<object> result)
        {
            var activity = await result as IMessageActivity;

            // TODO: Put logic for handling user message here

            context.Wait(MessageReceivedAsync);
        }

        private async Task ShowLuisResult(IDialogContext context, LuisResult result) 
        {
            bool isTrackId = false;
            IList<EntityRecommendation> listOfEntitiesFound = result.Entities;
            // await context.PostAsync($"Result.Entities {result.Entities[0].Type}"); Também funciona

            if (result.Intents[0].Intent.Equals("FindOrder"))
            {
                foreach (EntityRecommendation item in listOfEntitiesFound)
                {
                    if (item.Type.Equals("TrackingID"))
                    {
                        await context.PostAsync($"Obrigada pelo número de identificação. Vou averiguar onde está a sua encomenda \n You have reached {result.Intents[0].Intent}");
                        isTrackId = true;
                        break;
                    }
                }

                if (!isTrackId)
                {
                    await context.PostAsync($"Por favor insira o número de identificação da sua encomenda. \n You have reached {result.Intents[0].Intent}.");
                    context.Wait(MessageReceived);
                }
            } 
            if(result.Intents[0].Intent.Equals("Cancel"))
            {
                await context.PostAsync($"A sua encomenda será cancelada. Obrigado  \n You have reached {result.Intents[0].Intent}.");

            }
        }
    }
}