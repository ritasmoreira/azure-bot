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

            bool isDate = false;
            IList<EntityRecommendation> listOfEntitiesFound = result.Entities;

            foreach (EntityRecommendation item in listOfEntitiesFound)
            {
                if (item.Type.Equals("Date"))
                {
                    await context.PostAsync($"estou aqui dentro");
                    isDate = true;

                    if (context.UserData.TryGetValue(ContextConstants.Date, out orderDate))
                    {
                        // Guardar data
                        orderDate = item.Entity;
                        await context.PostAsync($"É a primeira vez que guarda a data");
                        context.UserData.SetValue(ContextConstants.Date, orderDate);

                        break;
                    }
                    else
                    {
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

                        //await context.PostAsync($"estou a seguir ao message received ");
                        //break;
                    }
                }
            }

            if(!isDate)
            {
                await context.PostAsync($"Por favor insira a nova data de entrega");
                context.Wait(MessageReceived);
            }

              
        }

    


        private async Task ResumeAfterChoicePrompt(IDialogContext context, IAwaitable<bool> result)
        {
            var choice = await result;

            await context.PostAsync(choice ? "Data de encomenda alterada com sucesso" : "Operação Cancelada");

        }

        private async Task ResumeAfterPrompt(IDialogContext context, IAwaitable<string> result)
        {

            var orderDate = await result;
            await context.PostAsync($"Estou dentro do ResumeAfter Prompt e este é ov getType do orderDate {orderDate.GetType()}");


            // TODO - verificar aqui se a data é a certa, se nao for, conta como resposta errada e ele depois das 3 vai com o boda
            if (orderDate.GetType().Equals("Date"))
            {
                await context.PostAsync($"Sou do tipo date!");
                context.UserData.SetValue(ContextConstants.Date, orderDate);
                context.Done(true);
            }
            else context.Wait(MessageReceivedAsync);
          
            // TODO - Como retorno que valor é errado? 
        }


        private async Task MessageReceivedAsync(IDialogContext context, IAwaitable<object> result)
        {
            var activity = await result as IMessageActivity;
            await context.PostAsync($"Estou dentro do messageReeivedAsync");

            await context.PostAsync($"Text is {activity.Text}");
            await context.PostAsync($"Get Type {activity.SuggestedActions.Actions.GetType()}");

            // TODO: Put logic for handling user message here

        }

    }
}