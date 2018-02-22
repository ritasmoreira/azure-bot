using System;
using System.Collections.Generic;
using System.Configuration;
using System.Threading.Tasks;

using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Luis;
using Microsoft.Bot.Builder.Luis.Models;

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
        
        /*
        public string BotEntityRecognition(string intentName, LuisResult result)
        {
            IList<EntityRecommendation> listOfEntitiesFound = result.Entities;
            StringBuilder entityResults = new StringBuilder();
        } */


        private async Task ShowLuisResult(IDialogContext context, LuisResult result) 
        {
            bool isTrackId = false;
            IList<EntityRecommendation> listOfEntitiesFound = result.Entities;
            // await context.PostAsync($"Result.Entities {result.Entities[0].Type}"); Também funciona

            foreach (EntityRecommendation item in listOfEntitiesFound)
            {
                if (item.Type.Equals("TrackingID"))
                {
                    await context.PostAsync($"Thank you for the Track ID. I will look into that \n You have reached {result.Intents[0].Intent}");
                    isTrackId = true;
                    break;
                }
            }

            if (!isTrackId)
            {
                await context.PostAsync($"Could you give me your order's track id, please? \n You have reached {result.Intents[0].Intent}.");
                context.Wait(MessageReceived);
            }
        }
    }
}