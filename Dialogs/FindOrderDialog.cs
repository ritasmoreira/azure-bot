using System;
using System.Collections.Generic;
using System.Configuration;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Luis;
using Microsoft.Bot.Builder.Luis.Models;
using Microsoft.Bot.Connector;
using Microsoft.Bot.Sample.LuisBot;

namespace LuisBot.Dialogs
{
    [Serializable]
    public class FindOrderDialog : LuisDialog<object>
    {
        private const string EntityTrackId = "TrackingID";

        public FindOrderDialog() : base(new LuisService(new LuisModelAttribute(
          ConfigurationManager.AppSettings["LuisAppId"],
          ConfigurationManager.AppSettings["LuisAPIKey"],
          domain: ConfigurationManager.AppSettings["LuisAPIHostName"])))
        {
        }

        public string trackNr, location;
        

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
            EntityRecommendation trackId;

            if (!result.TryFindEntity(EntityTrackId, out trackId))
            {
                await context.PostAsync($"Por favor insira primeiro o número de identificação da sua encomenda. \n You have reached {result.Intents[0].Intent}.");
                context.Wait(MessageReceived);

                 
                /*Attaching gif to message
                var message = context.MakeMessage();
                var attachment = GetAnimationCard();
           
                message.Attachments.Add(attachment);
                await context.PostAsync(message);  */
            } else {
                await context.PostAsync($"Obrigada pelo número de identificação.");
                context.Done(true);
            }

          
        }

      
        
    }
}