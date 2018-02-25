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
                // Attaching gif to message
                var message = context.MakeMessage();
                var attachment = GetAnimationCard();
           
                message.Attachments.Add(attachment);
                await context.PostAsync(message); 

                await context.PostAsync($"Por favor insira o número de identificação da sua encomenda. \n You have reached {result.Intents[0].Intent}.");
                context.Wait(MessageReceived);
            }

          
        }

        [LuisIntent("Cancel")]
        private async Task CancelIntent(IDialogContext context, LuisResult result)
        {
            //await context.PostAsync($"Tem a certeza que pretende cancelar a sua encomenda?");

            var message = context.MakeMessage();
            message.Text = "Tem a certeza que pretende cancelar a sua encomenda?";
            message.SuggestedActions = new SuggestedActions()
            {
                Actions = new List<CardAction>()
                {
                    new CardAction(){ Title = "Sim", Type=ActionTypes.ImBack, Value="Sim" },
                    new CardAction(){ Title = "Não", Type=ActionTypes.ImBack, Value="Não" },
                }
            };

            //await context.PostAsync(message);
            // context.Wait(MessageReceived);  // esta a ir atras buscar a resposta enquanto devia tira-la logo daqui

            await context.PostAsync($"A sua encomenda será cancelada. Obrigado  \n You have reached {result.Intents[0].Intent}.");



            if (message.Value.Equals("Sim"))
            {
                await context.PostAsync($"A sua encomenda será cancelada. Obrigado  \n You have reached {result.Intents[0].Intent}.");

            }
            else
            {

            }
            


        }
    }
}