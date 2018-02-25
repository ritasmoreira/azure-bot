using System;
using System.Collections.Generic;
using System.Configuration;
using System.Threading;
using System.Threading.Tasks;
using LuisBot.Dialogs;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Luis;
using Microsoft.Bot.Builder.Luis.Models;
using Microsoft.Bot.Connector;

namespace Microsoft.Bot.Sample.LuisBot
{
    // For more information about this template visit http://aka.ms/azurebots-csharp-luis
    [Serializable]
    public class BasicLuisDialog : LuisDialog<object>
    {



        public BasicLuisDialog() : base(new LuisService(new LuisModelAttribute(
            ConfigurationManager.AppSettings["LuisAppId"], 
            ConfigurationManager.AppSettings["LuisAPIKey"], 
            domain: ConfigurationManager.AppSettings["LuisAPIHostName"])))
        {
        }



        // ---------------------------------------------------


        public async Task ShowImageAttachment(IDialogContext context)
        {

            var replyMessage = context.MakeMessage();

            Attachment attachment = null;
            attachment = GetInternetAttachment();

            // The Attachments property allows you to send and receive images and other content
            replyMessage.Attachments = new List<Attachment> { attachment };
            await context.PostAsync(replyMessage);

        }

        private static Attachment GetInternetAttachment()
        {
            return new Attachment
            {
                Name = "BotFrameworkOverview.png",
                ContentType = "image/png",
                ContentUrl = "https://docs.microsoft.com/en-us/bot-framework/media/how-it-works/architecture-resize.png"
            };
        }

        // -----------------------------------------------


        [LuisIntent("None")]
        public async Task NoneIntent(IDialogContext context, LuisResult result)
        {
            await context.PostAsync($"None Intent");
            await this.ShowLuisResult(context, result);
        }

        // Go to https://luis.ai and create a new intent, then train/publish your luis app.
        // Finally replace "Gretting" with the name of your newly created intent in the following handler
        [LuisIntent("FindOrder")]
        public async Task FindOrderIntent(IDialogContext context, IAwaitable<IMessageActivity> activity, LuisResult result)
        {
            
            //await this.ShowLuisResult(context, result);
            var message = await activity;
            await context.PostAsync($"Find ORder Intent");
            await context.Forward(new FindOrderDialog(), this.ResumeAfterFindOrderDialog, message, CancellationToken.None);
     
        }


        private async Task ResumeAfterFindOrderDialog(IDialogContext context, IAwaitable<object> result)
        {
            // var resultFindOrder = await result;

            // await context.PostAsync($"New order dialog just told me this: {resultFindOrder}");

            // Again, wait for the next message from the user.
            //context.Wait(this.MessageReceivedAsync);
        }

        [LuisIntent("Cancel")]
        public async Task CancelIntent(IDialogContext context, IAwaitable<IMessageActivity> activity,  LuisResult result)
        {
            
            //await this.ShowLuisResult(context, result);
            var message = await activity;
            await context.PostAsync($"Cancel Intent");
            await context.Forward(new FindOrderDialog(), this.ResumeAfterCancelDialog, message, CancellationToken.None);

        }

        private async Task ResumeAfterCancelDialog(IDialogContext context, IAwaitable<object> result)
        {
            var message = await result;
            await context.PostAsync($"Resume After Cancel Dialog");
            context.Wait(MessageReceived);
        }

        [LuisIntent("Help")]
        public async Task HelpIntent(IDialogContext context, LuisResult result)
        {
            
            await context.PostAsync($"Help Intent");
            await this.ShowLuisResult(context, result);
        }

       
        
        
        
        private async Task ShowLuisResult(IDialogContext context, LuisResult result) 
        {
            await context.PostAsync($"Show Result");
            context.Wait(MessageReceived);
        } 

    }
}