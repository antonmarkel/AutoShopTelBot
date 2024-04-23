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
                },
                ThrowPendingUpdates = true
            };
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
            var message = update.Message;
            //var user = message.From;
            var chat = message.Chat;
         
            try
            {
               
                switch (update.Type)
                {
                    case UpdateType.Message:
                    {
                            Console.WriteLine("Message Received");
                            if (message.Text == "/test")
                            {
                                await TelegramPage.Open(_botClient, chat, new TelegramRoute("main"));
                            }
                            else
                            {
                                await botClient.SendTextMessageAsync(chat, "Иди нахуй даун ебанный");
                            }
                            
                           
                         return;
                    }
                    case UpdateType.CallbackQuery:
                        {
                            await TelegramPage.Open(_botClient, chat, new TelegramRoute(update.CallbackQuery.Data));
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
