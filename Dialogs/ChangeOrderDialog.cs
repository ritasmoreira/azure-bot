using System;
using System.Collections.Generic;
using System.Configuration;
using System.Threading;
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
        private const string EntityTrackId = "TrackingID";

        public ChangeOrderDialog() : base(new LuisService(new LuisModelAttribute(
          ConfigurationManager.AppSettings["LuisAppId"],
          ConfigurationManager.AppSettings["LuisAPIKey"],
          domain: ConfigurationManager.AppSettings["LuisAPIHostName"])))
        {
        }


        public EntityRecommendation orderDate, orderTrackId;
        public string orderDate_string;
        private int counter;


        [LuisIntent("ChangeOrder")]
        private async Task ChangeOrderIntent(IDialogContext context, IAwaitable<IMessageActivity> activity, LuisResult result)
        {


            await context.PostAsync($"You have reached {result.Intents[0].Intent}.");
            //await context.PostAsync($"logo depois false -> erro? {context.UserData.TryGetValue(ContextConstants.OrderDate, out orderDate_string)}");


            if (!result.TryFindEntity(EntityDate, out orderDate))
            {
                await context.PostAsync($"Por favor insira a nova data de entrega");
                context.Wait(MessageReceived);
            }
            else
            {
                if (!context.UserData.TryGetValue(ContextConstants.OrderDate, out orderDate_string))
                {
                    // Guardar data


                    //await context.PostAsync($"Entrei na parte da PRIMEIRA data");
                    context.UserData.SetValue(ContextConstants.OrderDate, orderDate.Entity);
                    await context.PostAsync($"A nova data da sua encomenda foi alterada para {context.UserData.GetValue<string>(ContextConstants.OrderDate)}");
                    await context.PostAsync($"miguel {context.UserData.TryGetValue(ContextConstants.OrderDate, out orderDate_string)}");

                }
                else
                {
                    await context.PostAsync($"Entrei na parte da SEGUNDA data");

                    await context.PostAsync($"A sua data anterior era {context.UserData.GetValue<string>(ContextConstants.OrderDate)}");

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


            if (activity.Text.Equals("Sim"))
            {
                //por um bool cujo valor e verificado no changeorder
                context.UserData.SetValue(ContextConstants.OrderDate, orderDate.Entity);
                await context.PostAsync($"A data foi alterada com sucesso. \n A sua nova data de entrega é: {context.UserData.GetValue<string>(ContextConstants.OrderDate)}");
                context.Done(true);
            }
            else if (activity.Text.Equals("Não") || activity.Text.Equals("Nao"))
            {
                await context.PostAsync($"Operação cancelada");
                context.Done(true);
            }


        }

        private async Task CheckDate(IDialogContext context, IAwaitable<LuisResult> result)
        {
            var dateMessage = await result;

        }

        [LuisIntent("Help")]
        [LuisIntent("None")]
        [LuisIntent("FindOrder")]
        public async Task CancelIntent(IDialogContext context, LuisResult result)
        {
            
            if (!context.PrivateConversationData.ContainsKey("NumberTrials"))
            {
                context.PrivateConversationData.SetValue("NumberTrials", 0);
                
                await context.PostAsync($" 0000 Data incorreta, por favor tente outra vez");
                context.Wait(MessageReceived);
            }
            else
            {
                if (context.PrivateConversationData.GetValue<int>("NumberTrials") < 2)
                {
                    counter = context.PrivateConversationData.GetValue<int>("NumberTrials") +1;

                    await context.PostAsync($"counter {counter}");

                    context.PrivateConversationData.SetValue("NumberTrials", counter);
                    await context.PostAsync($"Data incorreta, por favor tente outra vez");
                    context.Wait(MessageReceived);
                }
                else
                {
                    await context.PostAsync($"Número de tentativas máximo atingido. \n Por favor contacte companhia");
                    context.PrivateConversationData.SetValue("NumberTrials", 0);
                    context.Done(true);
                }
            }
        }

        [LuisIntent("Cancel")]
        public async Task CancelIntent(IDialogContext context, IAwaitable<IMessageActivity> activity, LuisResult result)
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