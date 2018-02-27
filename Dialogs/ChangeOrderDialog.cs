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


        public EntityRecommendation orderDate;
        public string orderDate_string;



        [LuisIntent("ChangeOrder")]
        private async Task ChangeOrderIntent(IDialogContext context, IAwaitable<IMessageActivity> activity, LuisResult result)
        {
            await context.PostAsync($"You have reached {result.Intents[0].Intent}.");


            if (!result.TryFindEntity(EntityDate, out orderDate))
            {
                await context.PostAsync($"Por favor insira a nova data de entrega");
                context.Wait(MessageReceived);
            } else
            {
                if (!context.UserData.TryGetValue(ContextConstants.Date, out orderDate_string))
                {
                    // Guardar data
                    
                
                        await context.PostAsync($"A order data é: {orderDate.Entity}");

                        //context.UserData.SetValue(ContextConstants.Date, orderDate.Entity);
                        await context.PostAsync($"A nova data da sua encomenda foi alterada para {context.UserData.GetValue<string>(ContextConstants.Date)}");
                    
                    
                }
                else
                {
                    await context.PostAsync($"A sua data anterior era {context.UserData.GetValue<string>(ContextConstants.Date)}");

                    // TODO: Passar o orderDate para o MessageReceivedAsync


                    var message = context.MakeMessage();
                    message.Text = "Tem a certeza que quer alterar a data?";
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

            if(activity.Text.Equals("Sim"))
            {
                //por um bool cujo valor e verificado no changeorder
                context.UserData.SetValue(ContextConstants.Date, orderDate);
                await context.PostAsync($"A data foi alterada com sucesso. \n A sua nova data de entrega é: {context.UserData.GetValue<string>(ContextConstants.Date)}");
                context.Done(true);
            }
            else if (activity.Text.Equals("Não") || activity.Text.Equals("Nao"))
            {
                await context.PostAsync($"Operação cancelada");
                context.Done(true);
            }


        }

    }
}