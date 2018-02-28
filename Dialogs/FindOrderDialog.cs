using System;
using System.Collections.Generic;
using System.Configuration;
using System.Threading;
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
        public string TrackNr_string;

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
            EntityRecommendation trackId;

            if (!result.TryFindEntity(EntityTrackId, out trackId))
            {
               // await context.PostAsync($"Por favor insira primeiro o número de identificação da sua encomenda. \n You have reached {result.Intents[0].Intent}.");
                context.Wait(MessageReceived);

                 
                /*Attaching gif to message
                var message = context.MakeMessage();
                var attachment = GetAnimationCard();
           
                message.Attachments.Add(attachment);
                await context.PostAsync(message);  */
            } else {
                if (!context.UserData.TryGetValue(ContextConstants.TrackId, out TrackNr_string))
                {
                    context.UserData.SetValue(ContextConstants.TrackId, trackId.Entity);
                    await context.PostAsync($"Obrigada pelo número de identificação.");

                } else
                {
                    context.UserData.SetValue(ContextConstants.TrackId, trackId.Entity);
                    await context.PostAsync($"Obrigada pelo número de identificação.");
                }
                context.Done(true);
            }

          
        }


        [LuisIntent("Help")]
        [LuisIntent("None")]
        public async Task RemaningIntents(IDialogContext context, IAwaitable<IMessageActivity> activity, LuisResult result)
        {
            context.Done(true);
        }

        [LuisIntent("Cancel")]
        private async Task CancelIntent(IDialogContext context, IAwaitable<IMessageActivity> activity, LuisResult result)
        {
            var message = await activity;
            await context.Forward(new CancelOrderDialog(), this.ResumeAfterCancelOrderDialog, message, CancellationToken.None);
        }

        private async Task ResumeAfterCancelOrderDialog(IDialogContext context, IAwaitable<object> result)
        {
            var message = await result;
            context.Wait(MessageReceived);
        }

    }
}