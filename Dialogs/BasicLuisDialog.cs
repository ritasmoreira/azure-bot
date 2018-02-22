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
            //string entity = this.BotEntityRecognition(Intent_TurnOff, result);

            IList<EntityRecommendation> listOfEntitiesFound = result.Entities;

            foreach(EntityRecommendation item in listOfEntitiesFound)
            {
                if(item.Entity.Equals("TrackingID"))
                {
                    await context.PostAsync($"You have reached {result.Intents[0].Intent}. I will look into that");
                    break;
                }
            }

            await context.PostAsync($"You have reached {result.Intents[0].Intent}. You said: {result.Query}. Could you give me your track id?");
            context.Wait(MessageReceived);
        }
    }
}