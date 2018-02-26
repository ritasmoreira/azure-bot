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

                    //await context.PostAsync($"Este tem negaçao: {!context.UserData.TryGetValue(ContextConstants.TrackId, out trackNr)}");
                    //await context.PostAsync($"Estão nao tem negação: {context.UserData.TryGetValue(ContextConstants.TrackId, out trackNr)}\n trackNUMBER é {trackNr}");

                   
                    if (!context.UserData.TryGetValue(ContextConstants.TrackId, out trackNr))
                    {
                        // await context.PostAsync($"Entrei no tryGetValue");

                        trackNr = item.Entity;
                        context.UserData.SetValue(ContextConstants.TrackId, item.Entity);
                        // await context.PostAsync($"O novo valor do track id é {context.UserData.GetValue<string>(ContextConstants.TrackId)}");
                    }


                        await context.PostAsync($"Estou a definir um novo valor para a localização");
                        PromptDialog.Text(context, this.OnTextWritten, "Qual a localização da sua encomenda?", "Não percebi a localização, pode repetir?", 3);
                    context.Wait(MessageReceived);


                    await context.PostAsync($"A sua encomenda tem o track ID seguinte: {context.UserData.GetValue<string>(ContextConstants.Location)}");
                    await context.PostAsync($"Obrigada pelo número de identificação. A sua encomenda encontra-se em {context.UserData.GetValue<string>(ContextConstants.Location)} \n You have reached {result.Intents[0].Intent}");


                    //await context.PostAsync($"Obrigada pelo número de identificação.");

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

                await context.PostAsync($"Por favor insira primeiro o número de identificação da sua encomenda. \n You have reached {result.Intents[0].Intent}.");
                context.Wait(MessageReceived);
            }

          
        }

        [LuisIntent("Cancel")]
        private async Task CancelIntent(IDialogContext context, LuisResult result)
        {
            //await context.PostAsync($"CancelIntent dentro do FindOrderDialog");
            //await context.PostAsync($"Tem a certeza que pretende cancelar a sua encomenda?");
            /*
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

            await context.PostAsync(message);

            context.Wait(MessageReceived);

            if (message.Value.Equals("Sim"))
            {
                await context.PostAsync($"A sua encomenda será cancelada. Obrigado");
            } else if(message.Value.Equals("Não"))
            {
                await context.PostAsync($"A sua encomenda continua a caminho. Obrigado");
            }

            //context.Done(true);
              
            */
            PromptDialog.Choice(context, this.OnOptionSelected, new List<string>() { "Sim","Não"}, "Tem a certeza que quer cancelar a sua encomenda?", "A resposta que deu não é válida. Quer cancelar a encomenda?");

        }
        

        public async Task OnTextWritten(IDialogContext context, IAwaitable<object> result)
        {
            var message = await result;
            context.UserData.SetValue(ContextConstants.Location, message.ToString());
        }

        public async Task OnOptionSelected(IDialogContext context, IAwaitable<object> result)
        {
            var message = await result;

            if (message.Equals("Sim"))
            {
                await context.PostAsync($"A sua encomenda será cancelada. Obrigado");
            } else if (message.Equals("Não") || message.Equals("nao")) {
                await context.PostAsync($"A sua encomenda continua a caminho. Obrigado");
            }

            context.Done(true);
        }
    }
}