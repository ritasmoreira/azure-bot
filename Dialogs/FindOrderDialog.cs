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
            await context.PostAsync($"FindOrderIntent dentro do FindOrderDialog");
            bool isTrackId = false;
            IList<EntityRecommendation> listOfEntitiesFound = result.Entities;

            

            // Percorre lista de entidades na mensagem recebida e procura por um track Id
            // Caso o encontre, vai guardá-lo 
            foreach (EntityRecommendation item in listOfEntitiesFound)
            {
                if (item.Type.Equals("TrackingID"))
                {
                    await context.PostAsync($"Obrigada pelo número de identificação.");
                    isTrackId = true;

                    // VVV important
                    context.Done(true);
                    break;

                    //await context.PostAsync($"Este tem negaçao: {!context.UserData.TryGetValue(ContextConstants.TrackId, out trackNr)}");
                    //await context.PostAsync($"Estão nao tem negação: {context.UserData.TryGetValue(ContextConstants.TrackId, out trackNr)}\n trackNUMBER é {trackNr}");

                    /*
                     if (!context.UserData.TryGetValue(ContextConstants.TrackId, out trackNr))
                     {
                         // await context.PostAsync($"Entrei no tryGetValue");

                         trackNr = item.Entity;
                         context.UserData.SetValue(ContextConstants.TrackId, item.Entity);
                         // await context.PostAsync($"O novo valor do track id é {context.UserData.GetValue<string>(ContextConstants.TrackId)}");
                     }

                     if (!context.UserData.TryGetValue(ContextConstants.Location, out location))
                     {
                     } else
                     {
                         await context.PostAsync($"Insira o novo valor ");
                         context.Wait(MessageReceived);

                         await context.PostAsync($"Antes do prompt ");
                         PromptDialog.Text(context, OnTextWritten, "Qual a localização da sua encomenda?");
                         return;
                     }


                     await context.PostAsync($"A sua encomenda tem o track ID seguinte: {context.UserData.GetValue<string>(ContextConstants.TrackId)}");                     */

                }
            }

            if (!isTrackId)
            {
                // Attaching gif to message
                /*
                var message = context.MakeMessage();
                var attachment = GetAnimationCard();
           
                message.Attachments.Add(attachment);
                await context.PostAsync(message);  */

                await context.PostAsync($"Por favor insira primeiro o número de identificação da sua encomenda. \n You have reached {result.Intents[0].Intent}.");
                context.Wait(MessageReceived);
            }

          
        }

      
        

        public async Task OnTextWritten(IDialogContext context, IAwaitable<string> result)
        {
            var location = await result;
            await context.PostAsync($"Estou dentro do ontextwritten");

            context.UserData.SetValue(ContextConstants.Location, location.ToString());
            context.Done(true);
        }

        
    }
}