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

        private async Task MessageReceivedAsync(IDialogContext context, IAwaitable<object> result)
        {
            var activity = await result as IMessageActivity;

            // TODO: Put logic for handling user message here

            context.Wait(MessageReceivedAsync);
        }




        [LuisIntent("ChangeOrder")]
        private async Task ChangeOrderIntent(IDialogContext context, IAwaitable<IMessageActivity> activity, LuisResult result)
        {
            await context.PostAsync($"You have reached {result.Intents[0].Intent}.");
            var message = await activity;

            // TODO : Se ainda não tiver colocado o valor do trackID

            await context.PostAsync($"Valor:{!context.UserData.TryGetValue(ContextConstants.Date, out orderDate)}");

            if (!context.UserData.TryGetValue(ContextConstants.Date, out orderDate))
            {
                   await context.PostAsync($"Estou a definir a data pela primeira vez");
                   PromptDialog.Text(context, this.ResumeAfterPrompt, "Qual a data da sua encomenda?", "Data inválida. Por favor tente outra vez", 3);
                
            }
            else {
                // TODO: Pedir e alterar a data

                await context.PostAsync($"A data esta a ser modificada");
                // TODO: Alterar para conter o valor da data na mesangem 
                PromptDialog.Confirm(
                    context,
                    this.ResumeAfterChoicePrompt,
                    "Tem a certeza que deseja alterar a data da sua encomenda?",
                    "Data inválida. Por favor tente outra vez",
                    3,
                    PromptStyle.Auto
                    );
            }
            context.Done(true);
        }


        private async Task ResumeAfterChoicePrompt(IDialogContext context, IAwaitable<bool> result)
        {
            var choice = await result;
            if(choice)
            {
                await context.PostAsync($"Data de encomenda alterada com sucesso");
            } else
            {
                await context.PostAsync($"Operação cancelada");
            }
            context.Done(true);
        }

        private async Task ResumeAfterPrompt(IDialogContext context, IAwaitable<string> result)
        {

            var orderDate = await result;

            // TODO - verificar aqui se a data é a certa, se nao for, conta como resposta errada e ele depois das 3 vai com o boda
            if (orderDate.GetType().Equals("Date"))
            {
                await context.PostAsync($"Sou do tipo date!");
                context.UserData.SetValue(ContextConstants.Date, orderDate);
                context.Done(true);
            }
            else context.Wait(MessageReceived);
          
            // TODO - Como retorno que valor é errado? 
        }
    }
}