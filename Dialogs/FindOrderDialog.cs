﻿using System;
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



        // Animation Card
        private static Attachment GetAnimationCard()
        {
            var animationCard = new AnimationCard
            {
                Title = "wat",
                Subtitle = "Woof?",
                Image = new ThumbnailUrl
                {
                    Url = "https://www.caninecoaching.com/wp-content/uploads/2014/03/8693_537261246320967_418001381_n.jpg"
                },
                Media = new List<MediaUrl>
                {
                    new MediaUrl()
                    {
                        Url = "https://media.giphy.com/media/51Uiuy5QBZNkoF3b2Z/giphy.gif"
                    }
                }
            };

            return animationCard.ToAttachment();
        }




        [LuisIntent("FindOrder")]
        private async Task FindOrderIntent(IDialogContext context, LuisResult result)
        {
            bool isTrackId = false;
            IList<EntityRecommendation> listOfEntitiesFound = result.Entities;

            //if (result.Intents[0].Intent.Equals("FindOrder"))
            //{
                foreach (EntityRecommendation item in listOfEntitiesFound)
                {
                    if (item.Type.Equals("TrackingID"))
                    {

                        await context.PostAsync($"Obrigada pelo número de identificação. Vou averiguar onde está a sua encomenda \n You have reached {result.Intents[0].Intent}");
                        isTrackId = true;

                        // VVV important
                        context.Done(true);
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
            //}
          
        }

        [LuisIntent("Cancel")]
        private async Task CancelIntent(IDialogContext context, LuisResult result)
        {
            //if(result.Intents[0].Intent.Equals("Cancel"))
            //{
                await context.PostAsync($"A sua encomenda será cancelada. Obrigado  \n You have reached {result.Intents[0].Intent}.");

            //}
        }
    }
}