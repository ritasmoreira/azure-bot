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
    public class ChangeOrderDialog : LuisDialog<object>
    {
        private const string EntityDate = "Date";

        public ChangeOrderDialog() : base(new LuisService(new LuisModelAttribute(
          ConfigurationManager.AppSettings["LuisAppId"],
          ConfigurationManager.AppSettings["LuisAPIKey"],
          domain: ConfigurationManager.AppSettings["LuisAPIHostName"])))
        {
        }

        public string orderDate;




        [LuisIntent("ChangeOrder")]
        private async Task ChangeOrderIntent(IDialogContext context, IAwaitable<IMessageActivity> activity, LuisResult result)
        {
            await context.PostAsync($"You have reached {result.Intents[0].Intent}.");

            EntityRecommendation orderDate;

            if (!result.TryFindEntity(EntityDate, out orderDate))
            {
                await context.PostAsync($"Por favor insira a nova data de entrega");
                context.Wait(MessageReceived);
            } else
            {
                // Falta aqui uma negaçãozinha
                if (context.UserData.TryGetValue(ContextConstants.Date, out orderDate))
                {
                    // Guardar data
                    await context.PostAsync($"É a primeira vez que guarda a data");
                    context.UserData.SetValue(ContextConstants.Date, orderDate);
                    //await
                }
                else
                {
                    await context.PostAsync($"A data antes de ser mudada {context.UserData.GetValue<string>(ContextConstants.Date)}");

                    // recebe a data e faz setValue


                    var message = context.MakeMessage();
                    message.Text = "Tem a certeza que quer confirmar alterar a data?";
                    message.SuggestedActions = new SuggestedActions()
                    {
                        Actions = new List<CardAction>()
                        {
                            new CardAction(){ Title = "Sim", Type=ActionTypes.ImBack, Value="Sim" },
                            new CardAction(){ Title = "Não", Type=ActionTypes.ImBack, Value="Não" },
                        }
                    };


                    await context.PostAsync(message);
                    context.Wait(MessageReceivedAsync);
                }
            }
           
              
        }

    

       

        private async Task MessageReceivedAsync(IDialogContext context, IAwaitable<object> buttonResult)
        {
            var activity = await buttonResult as IMessageActivity;
            await context.PostAsync($"Estou dentro do messageReeivedAsync");

            await context.PostAsync($"Text is {activity.Text}");

            if(activity.Text.Equals("Sim"))
            {
                //por um bool cujo valor e verificado no changeorder
                await context.PostAsync($"Hero");
                //await context.PostAsync($"A data antes de ser mudada {context.UserData.GetValue<string>(ContextConstants.Date)}");

            }
            else if (activity.Text.Equals("Não") || activity.Text.Equals("Nao"))
            {
                await context.PostAsync($"Operação cancelada");
                context.Done(true);
            }


        }

    }
}