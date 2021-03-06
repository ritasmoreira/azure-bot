﻿using System;
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

        public EntityRecommendation orderDate, orderTrackId;
        public string OrderDate_string, TrackNr_string;
        public bool ifFromChangeOrder = false;
        private int Counter;

        public ChangeOrderDialog() : base(new LuisService(new LuisModelAttribute(
          ConfigurationManager.AppSettings["LuisAppId"],
          ConfigurationManager.AppSettings["LuisAPIKey"],
          domain: ConfigurationManager.AppSettings["LuisAPIHostName"])))
        {
        }


        


        [LuisIntent("ChangeOrder")]
        private async Task ChangeOrderIntent(IDialogContext context, IAwaitable<IMessageActivity> activity, LuisResult result)
        {
            // Verifica se existe alguma entidade do tipo Date na mensagem
            // Verifica se já há alguma coisa guardada no trackId
            if (!result.TryFindEntity(EntityDate, out orderDate))
            {
                if (!context.UserData.TryGetValue(ContextConstants.TrackId, out TrackNr_string))
                {
                    ifFromChangeOrder = true;
                    await context.PostAsync($"Qual o id da encomenda cuja data deseja alterar?");
                    context.Wait(MessageReceived);
                }
                else
                {
                    await context.PostAsync($"Por favor insira a nova data de entrega para a sua encomenda (**ID: {context.UserData.GetValue<string>(ContextConstants.TrackId)})**");
                    context.Wait(MessageReceived);
                }
            }
            else
            {
                if (!context.UserData.TryGetValue(ContextConstants.OrderDate, out OrderDate_string))
                {
                    
                    context.UserData.SetValue(ContextConstants.OrderDate, orderDate.Entity);

                    string novaDataEncomenda = context.UserData.GetValue<string>(ContextConstants.OrderDate);
                    await context.PostAsync($"A nova data da sua encomenda foi alterada para:  **{novaDataEncomenda}** ");

                }
                else
                {
                    string idEncomenda = context.UserData.GetValue<string>(ContextConstants.TrackId);
                    string dataEncomendaVelha = context.UserData.GetValue<string>(ContextConstants.OrderDate);

                    await context.PostAsync($"A data da encomenda **{idEncomenda}** é **{dataEncomendaVelha}**");

                    var message = context.MakeMessage();
                    message.Text = $"Tem a certeza que deseja alterar a data para **{orderDate.Entity}**?";
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
                context.UserData.SetValue(ContextConstants.OrderDate, orderDate.Entity);
                await context.PostAsync($"A data foi alterada com sucesso. \n A sua nova data de entrega é: **{context.UserData.GetValue<string>(ContextConstants.OrderDate)}**");
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
        private async Task RemaningIntents(IDialogContext context, LuisResult result)
        {
            
            if (!context.PrivateConversationData.ContainsKey("NumberTrials"))
            {
                context.PrivateConversationData.SetValue("NumberTrials", 0);
                
                await context.PostAsync($"Data **incorreta**, por favor digite uma data válida");
                context.Wait(MessageReceived);
            }
            else
            {
                if (context.PrivateConversationData.GetValue<int>("NumberTrials") < 2)
                {
                    Counter = context.PrivateConversationData.GetValue<int>("NumberTrials") + 1;

                    context.PrivateConversationData.SetValue("NumberTrials", Counter);
                    await context.PostAsync($"Data **incorreta**, por favor digite uma data válida");
                    context.Wait(MessageReceived);
                }
                else
                {
                    await context.PostAsync($"Número de **tentativas máximo** atingido. \n Por favor contacte a companhia de entrega para alterar a data da encomenda");
                    context.PrivateConversationData.SetValue("NumberTrials", 0);
                    context.Done(true);
                }
            }
        }

        [LuisIntent("FindOrder")]
        private async Task FindOrderIntent(IDialogContext context, IAwaitable<IMessageActivity> activity, LuisResult result)
        {
            var message = await activity;
            await context.Forward(new FindOrderDialog(), this.ResumeAfterFindOrderDialog, message, CancellationToken.None);
        }

        private async Task ResumeAfterFindOrderDialog(IDialogContext context, IAwaitable<object> result)
        {
            var message = await result;
            if (ifFromChangeOrder)
            {
                await context.PostAsync("Por favor introduza a **nova data** de entrega");
                context.Wait(MessageReceived);

            }
            else context.Done(true);
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