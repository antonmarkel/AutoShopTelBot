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
using TelegramBot.Data;

namespace TelegramBot.TelegramAPI
{
    public enum ChatState : ushort
    {
        Standard = 0,
        GetData = 1,
        OwnerChat = 2,
    }
    public class TelegramBot
    {

        public Dictionary<ChatId, int> CurrentPurchase { get; set; } = new Dictionary<ChatId, int>();
        public Dictionary<ChatId, ChatState> ChatStates { get; set; } = new Dictionary<ChatId, ChatState>();
        public  Dictionary<ChatId, List<Message>> MessagesToDelete { get; set; } = new Dictionary<ChatId, List<Message>>();
        public  Dictionary<ChatId, List<Item>> Cart { get; set; } = new Dictionary<ChatId, List<Item>>();
        public static BotDataContext Context { get; set; } = new BotDataContext();
        private ITelegramBotClient _botClient;
        private ReceiverOptions _receiverOptions;
        public TelegramBot(string token)
        {

            _botClient = new TelegramBotClient(token);
            TelegramRoutes._Bot = this;
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
            TelegramRoutes._Bot = this;
            _botClient.StartReceiving(UpdateHandler, ErrorHanlder, _receiverOptions, cts.Token);
            Console.WriteLine("Bot started");
            await Task.Delay(-1);
        }

    public async Task ClearCart(ChatId chat,bool showMessage = true)
        {
            bool isEmpty = Cart[chat].Count == 0;
            Cart[chat].Clear();
            if (showMessage)
            {
                var inlineKeyboard = new InlineKeyboardMarkup(
                     new List<InlineKeyboardButton[]>()
                      {

                         new InlineKeyboardButton[]
                         {
                                     InlineKeyboardButton.WithCallbackData("Назад","main"),
                         },

                     });
                await _botClient.SendTextMessageAsync(chat, "Корзина теперь пуста!" + (isEmpty ? "\r\nНо до этого она тоже была пустой :(" : string.Empty), replyMarkup: inlineKeyboard);
            }
            return;
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

                        if (!MessagesToDelete.ContainsKey(chat)) { MessagesToDelete.Add(chat, new List<Message>()); }
                        if (!ChatStates.ContainsKey(update.Message.Chat)) ChatStates.Add(update.Message.Chat, ChatState.Standard);
                            //  Console.WriteLine($"{update.Message.Photo[0].FileId}");
                            //       return;

                            if (ChatStates[update.Message.Chat] == ChatState.GetData)
                            {
                                var message = update.Message.Text;
                                Context.Purchases.FirstOrDefault(v => v.ID == CurrentPurchase[chat]).Data += "\r\n" + message;
                                var state = Context.Purchases.FirstOrDefault(v => v.ID == CurrentPurchase[chat]).State;
                                await Context.SaveChangesAsync();
                                await Owner.OwnerAPI.NotifyOnwerAboutIncomingData(_botClient, this, chat, state);
                                return;
                            }

                            if (updateMessage == "Входящие заказы")
                            {
                                if (await Owner.OwnerAPI.IsOwner(chat))
                                {
                                    await Owner.OwnerAPI.ShowIncomingTasks(chat, _botClient);
                                }
                            }

                            
                        if (updateMessage == "/start")
                        {
                            await SetRoute("main",chat);
                        }
                        else if (updateMessage == "На главную страницу")
                        {
                           await SetRoute("main", chat);
       
                        }

                            else if (updateMessage == "/admin")
                         {
                                if (await Owner.OwnerAPI.IsOwner(chat))
                                {
                                    await Owner.OwnerAPI.OwnerMenu(_botClient, chat);
                                }
                                else
                                {
                                    Console.WriteLine("You're not an owner!");
                                }
                         }
                           
                        return;
                    }
                    case UpdateType.CallbackQuery:
                    {
                            
                       
                      var callbackQuery = update.CallbackQuery;
                      var chat = update.CallbackQuery.Message.Chat;
                       if (!ChatStates.ContainsKey(chat)) ChatStates.Add(chat, ChatState.Standard);
                       if (ChatStates[chat] != ChatState.Standard) {
                            if(callbackQuery.Data == "main")
                            { 
                               ChatStates[chat] = ChatState.Standard;
                            }
                            else if (callbackQuery.Data == "main/new")
                            {
                               await Owner.OwnerAPI.NotifyOnwers(_botClient, "Ебать,у нас новый заказ нахуй!");
                               await SetRoute("main", chat);
                                    ChatStates[chat] = ChatState.Standard;
                                await ClearCart(chat, showMessage: false);
                                    return;
                            }
                           else return;
                        }
                            
                        

                            if (!MessagesToDelete.ContainsKey(chat)) { MessagesToDelete.Add(chat, new List<Message>()); }
                       Console.WriteLine(callbackQuery.Data);
                       await botClient.AnswerCallbackQueryAsync(callbackQuery.Id,"Секунду");
                        
                      
                       foreach(var message in MessagesToDelete[chat])
                       {
                           await _botClient.DeleteMessageAsync(chat, message.MessageId); 
                       }        
                        MessagesToDelete[chat] = new List<Message>();

                        await _botClient.DeleteMessageAsync(chat, callbackQuery.Message.MessageId);

                        await SetRoute(callbackQuery.Data, chat);
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
