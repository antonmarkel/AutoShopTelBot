using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Xsl;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace TelegramBot.Telegram
{
    public class TelegramBot
    {
        private ITelegramBotClient _botClient;
        private ReceiverOptions _receiverOptions;
        public TelegramBot(string token)
        {
            _botClient = new TelegramBotClient(token);
            _receiverOptions = new ReceiverOptions
            {
                AllowedUpdates = new[]
                {
                    UpdateType.Message,
                    UpdateType.CallbackQuery
                },
                ThrowPendingUpdates = true
            };
        }

        public async Task SetRoute(string route,ChatId chat)
        {
            await TelegramRoutes.GetRenderByRoute(route, _botClient,chat);
        }
        
        public async Task Start()
        {
            using var cts = new CancellationTokenSource();
            //
            _botClient.StartReceiving(UpdateHandler, ErrorHanlder, _receiverOptions, cts.Token);
            Console.WriteLine("Bot started");
            await Task.Delay(-1);
        }

       async Task UpdateHandler(ITelegramBotClient botClient,Update update,CancellationToken cancelTok)
        {
          
         
            try
            { 
                switch (update.Type)
                {
                    case UpdateType.Message:
                    {
                        var updateMessage = update.Message.Text;
                        var chat = update.Message.Chat;
                        if(updateMessage == "/start")
                        {
                            await SetRoute("main",chat);
                        }
                        return;
                    }
                    case UpdateType.CallbackQuery:
                    {
                        var callbackQuery = update.CallbackQuery;
                        var chat = update.CallbackQuery.Message.Chat;
                        Console.WriteLine(callbackQuery.Data);
                        await botClient.AnswerCallbackQueryAsync(callbackQuery.Id,"Секунду");
                        await SetRoute(callbackQuery.Data,chat);
                        await Task.Delay(1000);
                        if(TelegramRoutes.MessagesToDelete != null)
                        {
                            foreach(var message in TelegramRoutes.MessagesToDelete)
                            {
                                await _botClient.DeleteMessageAsync(chat, message.MessageId);
                                await Task.Delay(200);
                             }
                            TelegramRoutes.MessagesToDelete = new List<Message>();
                        }
                            await _botClient.DeleteMessageAsync(chat, callbackQuery.Message.MessageId);
                        
                        return;
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
       }

       Task ErrorHanlder(ITelegramBotClient botClient, Exception error, CancellationToken cancelToken)
        {
            var ErrorMessage = error switch
            {
                ApiRequestException apiRequestException => $"Telegram bot API error:\n[{apiRequestException.ErrorCode} {apiRequestException.Message}",
                _ => error.ToString()
            };
            Console.WriteLine(ErrorMessage);
            return Task.CompletedTask;
        }
    }
}
