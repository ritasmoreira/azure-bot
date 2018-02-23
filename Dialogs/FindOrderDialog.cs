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
    public class FindOrderDialog : LuisDialog<object>
    {
        public FindOrderDialog() : base(new LuisService(new LuisModelAttribute(
          ConfigurationManager.AppSettings["LuisAppId"],
          ConfigurationManager.AppSettings["LuisAPIKey"],
          domain: ConfigurationManager.AppSettings["LuisAPIHostName"])))
        {
        }

        [LuisIntent("FindOrder")]
        private async Task FindOrderIntent(IDialogContext context, LuisResult result)
        {
            bool isTrackId = false;
            IList<EntityRecommendation> listOfEntitiesFound = result.Entities;

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
                    /*
                    var message = context.MakeMessage();
                    var attachment = GetAnimationCard();

                    message.Attachments.Add(attachment);

                    await context.PostAsync(message); */

                    await context.PostAsync($"Por favor insira o número de identificação da sua encomenda. \n You have reached {result.Intents[0].Intent}.");
                    context.Wait(MessageReceived);
                }
            }
          
        }
    }
}