using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
        GetEmail = 1,
        GetCode = 2,
        GetDoc = 3,
        GetTag = 4,
        GetData = 5,
        AskData = 6,
    }
    public struct Question
    {
        public ChatId Receiver;
        public string Message;
        public int PurchaseId;

        public static Question New(ChatId chat, string message, int purchaseId)
        {
            return new Question() { Receiver = chat, Message = message, PurchaseId = purchaseId };
        }
    }
    public class TelegramBot
    {
        public long GroupId { get; set; } = -1002007072435;
        public static TelegramBot CurrentBot { get;set; }
        public List<Message> PreviousMessages { get; set; } = new List<Message>();
        public List<Message> ToDelete { get; set; } = new List<Message>();
        public Dictionary<ChatId, int> CurrentPurchase { get; set; } = new Dictionary<ChatId, int>();
        public Dictionary<ChatId,Purchase> GoingToBuy { get;set;} = new Dictionary<ChatId,Purchase>();
        public Dictionary<ChatId, ChatState> ChatStates { get; set; } = new Dictionary<ChatId, ChatState>();
        public Dictionary<ChatId,Question> AskDataStates { get; set; } = new Dictionary<ChatId, Question>();
        public  Dictionary<ChatId, List<Item>> Cart { get; set; } = new Dictionary<ChatId, List<Item>>();
        public  BotDataContext Context { get; set; } = new BotDataContext();
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
                    UpdateType.CallbackQuery,
                    UpdateType.ChannelPost,
                    
                },
                ThrowPendingUpdates = true
            };
        }

        public async Task SetRoute(string route,ChatId chat)
        {

            await TelegramRoutes.GetRenderByRoute(route, _botClient,chat);
        }

        public async Task StartCleaning(int secInterval)
        {
            while(true)
            {
                    await Task.Delay(500);
                    for (int i = PreviousMessages.Count - 1; i >= 0; i--)
                    {
                    // Console.WriteLine((PreviousMessages[i].Date + TimeSpan.FromHours(3)).ToShortTimeString() + "  " + PreviousMessages[i].Date.ToShortDateString() );
                    //  Console.WriteLine((DateTime.Now).ToShortTimeString() + "  " + DateTime.Now.ToShortDateString() );
                      var time = (DateTime.Now - PreviousMessages[i].Date - TimeSpan.FromHours(3)).TotalSeconds;
                        var message = PreviousMessages[i];
                      //  Console.WriteLine($"{time}");
                        if (time >= secInterval)
                        {
                            ToDelete.Add(message);
                        }
                    }
                    List<Message> nowDel = new List<Message>(ToDelete);
                    foreach(var del in nowDel)
                    {
                         try
                         {
                            await _botClient.DeleteMessageAsync(del.Chat, del.MessageId);
                          }
                        catch { }
                       PreviousMessages.Remove(del);
                      
                    }      
            }
        }
        
        public async Task Start()
        {
            using var cts = new CancellationTokenSource();
            TelegramRoutes._Bot = this;
            StartCleaning(5 * 60);
            _botClient.StartReceiving(UpdateHandler, ErrorHanlder, _receiverOptions, cts.Token);
            await Utils.Log("Bot started", ConsoleColor.DarkGreen);
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
                await _botClient.SendMessage(chat, "Корзина теперь пуста!" + (isEmpty ? "\r\nНо до этого она тоже была пустой :(" : string.Empty), replyMarkup: inlineKeyboard);
            }
            return;
        }

       async Task UpdateHandler(ITelegramBotClient botClient,Update update,CancellationToken cancelTok)
        {

            try
            {

                switch (update.Type)
                {
                    case UpdateType.ChannelPost:
                        {
                            await Utils.Log("$Message from channel", color: ConsoleColor.DarkCyan);
                            return;
                        }
                    case UpdateType.Message:
                        {
                            if (update.Message.Chat.Id == GroupId) return;
                            await Utils.Log($"Text message {update.Message.Chat.Id}", ConsoleColor.DarkBlue);

                            var updateMessage = update.Message.Text;
                            var chat = update.Message.Chat;
                           
                            if (!ChatStates.ContainsKey(update.Message.Chat)) ChatStates.Add(update.Message.Chat, ChatState.Standard);
                            //Console.WriteLine($"{update.Message.Photo[0].FileId}");
                            //   return;
                            switch (ChatStates[chat])
                            {
                                case ChatState.GetEmail:
                                    {
                                        await Utils.Log($"Getting email from {chat.Id}", ConsoleColor.DarkGreen);
                                        var message = update.Message.Text;
                                        var purch = Context.Purchases.FirstOrDefault(v => v.Identifier == CurrentPurchase[chat]);



                                        if (!Utils.IsValidEmail(message))
                                        {
                                            await _botClient.SendMessage(chat, "⛔️ Вы ввели неверный формат почты:\r\n\r\n⚠️ Формат: example@gmail.com\r\n✅ Пример: vanya228@mail.ru\r\n\r\n👇 Зайдите в раздел Supercell ID в игре, чтобы посмотреть вашу почту на нужном аккаунте и напишите почту повторно:");
                                        }
                                        else
                                        {
                                            var inlineKeyboard = new InlineKeyboardMarkup(
                                                  new List<InlineKeyboardButton[]>()
                                                  {
                                                 new InlineKeyboardButton[]
                                                 {
                                                   InlineKeyboardButton.WithCallbackData("Перейти к заказу",$"show|{purch.Identifier}"),
                                                 },
                                             });

                                            purch.Data += "\r\nEmail: " + message;
                                            await _botClient.SendMessage(chat, $"\U0001f6d2 Заказ: {purch.Identifier}\r\n👤 Статус: Ожидание подтверждения продавца\r\n⏰ Время: {purch.Date}\r\nПродавцы отвечают менее чем за 30 минут");
                                            //await SetRoute("main", chat);
                                            await Owner.OwnerAPI.NotifyOnwers(_botClient, $"🔔 Оповещение! Пришла почта.\r\n👤Заказ: {purch.ID}, ID: {purch.Identifier}\r\n🏦Способ оплаты: {purch.PaymentSystem}\r\n🛍Товары:\r\n{Utils.GetGoodsString(purch.ToModel())}\r\n📩Почта: {message}\r\n🗳️Категория: {purch.ToModel().GetCategories()[0]}\r\n💰Цена: {purch.Cost}₽", markup: inlineKeyboard);


                                            ChatStates[chat] = ChatState.Standard;
                                        }
                                        return;
                                    }
                                case ChatState.GetTag:
                                    {
                                        await Utils.Log($"Getting tag from {chat.Id}", ConsoleColor.DarkGreen);
                                        var message = update.Message.Text;
                                        var purch = Context.Purchases.FirstOrDefault(v => v.Identifier == CurrentPurchase[chat]);

                                        if (!Utils.IsValidTelegramTag(message))
                                        {
                                            await _botClient.SendMessage(chat, "⛔️ Вы ввели неверный формат тэга:\r\n\r\n⚠️ Формат: @tag\r\n✅ Пример: @Lancaster\r\n\r\n👇 Зайдите в свой профиль Telegram, чтобы посмотреть ваш тэг на нужном аккаунте и напишите его повторно:");
                                        }
                                        else
                                        {
                                            var inlineKeyboard = new InlineKeyboardMarkup(
                                                  new List<InlineKeyboardButton[]>()
                                                  {
                                                 new InlineKeyboardButton[]
                                                 {
                                                   InlineKeyboardButton.WithCallbackData("Перейти к заказу",$"show|{purch.Identifier}"),
                                                 },
                                             });

                                            purch.Data += "\r\nTag: " + message;
                                            await _botClient.SendMessage(chat, $"\U0001f6d2 Заказ: {purch.Identifier}\r\n👤 Статус: Ожидание продавца\r\n⏰ Время: {purch.Date}");
                                            //await SetRoute("main", chat);
                                            await Owner.OwnerAPI.NotifyOnwers(_botClient, $"🔔 Оповещение! Пришел тэг для Telegram. Зайди в активные заказы\r\n👤Заказ: {purch.ID}, ID:{purch.Identifier}\r\n🏦Способ оплаты: {purch.PaymentSystem}\r\n🛍Товары:\r\n{Utils.GetGoodsString(purch.ToModel())}\r\n📩Tэг: {message}\r\n🗳️Категория: {purch.ToModel().GetCategories()[0]}\r\n💰Цена: {purch.Cost}₽", markup: inlineKeyboard);
                                            purch.State = 1;

                                            ChatStates[chat] = ChatState.Standard;
                                        }
                                        return;
                                    }
                                case ChatState.GetCode:
                                    {
                                        var message = update.Message.Text;
                                        var purch = Context.Purchases.FirstOrDefault(v => v.Identifier == CurrentPurchase[chat]);
                                        if (!Utils.IsValidCode(message))
                                        {
                                            await _botClient.SendMessage(chat, "⛔️ Вы ввели неверный код с почты:\r\n\r\n⚠️ Формат: 123456\r\n✅ Пример: 782861\r\n\r\n👇 Проверьте <u>входящие</u>, <u>спам</u>, <u>промо-акции</u> в своём приложении/сайте и напишите правильный код повторно:", parseMode: ParseMode.Html);
                                            return;
                                        }
                                        purch.Data += "Code: " + "<code>" + message + "</code>";
                                        var inlineKeyboard = new InlineKeyboardMarkup(
                                                  new List<InlineKeyboardButton[]>()
                                                  {
                                                 new InlineKeyboardButton[]
                                                 {
                                                   InlineKeyboardButton.WithCallbackData("Перейти к заказу",$"show|{purch.Identifier}"),
                                                 },
                                             });
                                        await _botClient.SendMessage(chat, $"\U0001f6d2 Заказ: {purch.Identifier}\r\n👤 Статус: Ожидание продавца\r\n⏰ Время: {purch.Date}");
                                        await Owner.OwnerAPI.NotifyOnwers(_botClient, $"🔔 Оповещение! Пришёл код.\r\n👤Заказ: {purch.ID}, ID: {purch.Identifier}\r\n🏦Способ оплаты: {purch.PaymentSystem}\r\n🛍Товары:\r\n{Utils.GetGoodsString(purch.ToModel())}\r\n🗳️Категория: {purch.ToModel().GetCategories()[0]}\r\n💰Цена: {purch.Cost}₽\r\n{purch.Data}", markup: inlineKeyboard,mode:ParseMode.Html);
                                        ChatStates[chat] = ChatState.Standard;
                                        return;
                                    }
                                case ChatState.GetDoc:
                                    {
                                        if (update.Message.Photo != null)
                                        {
                                            Context.Purchases.FirstOrDefault(v => v.Identifier == CurrentPurchase[chat]).PictFileID = update.Message.Photo[0].FileId;
                                            var purch = Context.Purchases.FirstOrDefault(v => v.Identifier == CurrentPurchase[chat]);
                                            await Context.SaveChangesAsync();
                                            ChatStates[update.Message.Chat] = ChatState.Standard;
                                            var inlineKeyboard = new InlineKeyboardMarkup(
                                                  new List<InlineKeyboardButton[]>()
                                                  {
                                                 new InlineKeyboardButton[]
                                                 {
                                                   InlineKeyboardButton.WithCallbackData("Перейти к заказу",$"show|{purch.Identifier}"),
                                                 },
                                             });
                                            await Owner.OwnerAPI.NotifyOnwers(_botClient, $"Получено подтверждение оплаты\r\nЗаказ: {purch.ID}, ID: {purch.Identifier}\r\n🏦Способ оплаты: {purch.PaymentSystem}\r\n🗳️Категория: {purch.ToModel().GetCategories()[0]}\r\n💰Цена: {purch.Cost}₽", markup: inlineKeyboard);
                                            //await SetRoute("main", chat);
                                        }
                                        else
                                        {
                                            await _botClient.SendMessage(chat, "Пожалуйста,скиньте подтверждение в виде скриншота");
                                        }
                                        return;
                                    }
                                case ChatState.GetData:
                                    {
                                        var message = update.Message.Text;
                                        var purch = Context.Purchases.FirstOrDefault(v => v.Identifier == CurrentPurchase[chat]);
                                       
                                        purch.Data += "\r\n📇Дополнительная инфа:" + message + "\r\n";
                                        var inlineKeyboard = new InlineKeyboardMarkup(
                                                  new List<InlineKeyboardButton[]>()
                                                  {
                                                 new InlineKeyboardButton[]
                                                 {
                                                   InlineKeyboardButton.WithCallbackData("Перейти к заказу",$"show|{purch.Identifier}"),
                                                 },
                                             });
                                        await _botClient.SendMessage(chat, $"\U0001f6d2 Заказ: {purch.Identifier}\r\n👤 Статус: Ожидание продавца\r\n⏰ Время: {purch.Date}");
                                        await Owner.OwnerAPI.NotifyOnwers(_botClient, $"🔔 Оповещение! Пришли дополнительные данные по заказу.\r\n👤Заказ: {purch.ID}, ID:{purch.Identifier}\r\n🏦Способ оплаты: {purch.PaymentSystem}\r\n🛍Товары:\r\n{Utils.GetGoodsString(purch.ToModel())}\r\n🗳️Категория: {purch.ToModel().GetCategories()[0]}\r\n💰Цена: {purch.Cost}₽\r\n{purch.Data}", markup: inlineKeyboard, mode: ParseMode.Html);
                                        ChatStates[chat] = ChatState.Standard;
                                        SetRoute("main", chat);
                                        return;
                                    }
                                case ChatState.AskData:
                                    {
                                        var _base = AskDataStates[chat];
                                      
                                        var message = update.Message.Text;if(message == "Назад") { ChatStates[chat] = ChatState.Standard;return; }
                                        if (message == "Спросить название скина") message = "Пожалуйста,напишите название скина";
                                        if(message == "Спросить название оформления") message = "Пожалуйста,напишите название оформления";
                                        if (message == "Вернуть деньги") message = $"\U0001f6d2 Заказ: {CurrentPurchase[_base.Receiver]}\r\n👤 Статус: Возврат⚠️\r\n🔔 Данной акции нет на вашем аккаунте, поэтому был оформлен возврат. \r\n\r\n💳 Пришлите пожалуйста свои реквизиты карты, название банка, чтобы мы могли вернуть денежные средства.";
                                        if (!AskDataStates.ContainsKey(chat)) return;
                                       _base.Message = message;

                                        ChatStates[_base.Receiver] = ChatState.GetData;
                                        CurrentPurchase[_base.Receiver] = _base.PurchaseId;
                                        await _botClient.SendMessage(_base.Receiver, $"🔔 Сообщение от продавца, по заказу {_base.PurchaseId}\r\n\r\n{message}");
                                       // await Owner.OwnerAPI.NotifyOnwers(_botClient, $"🔔 Оповещение! Пришли дополнительные данные по заказу.\r\n👤Заказ:{purch.ID}\r\n🛍Товары:\r\n{Utils.GetGoodsString(purch.ToModel())}\r\n🗳️Категория: {purch.ToModel().GetCategories()[0]}\r\n💰Цена: {purch.Cost}₽\r\n{purch.Data}", markup: inlineKeyboard, mode: ParseMode.Html);
                                        if(_base.Receiver != chat)ChatStates[chat] = ChatState.Standard;
                                        return;
                                    }

                            }
                            await Context.SaveChangesAsync();

                            if (updateMessage == "Входящие заказы")
                            {
                                if (await Owner.OwnerAPI.IsOwner(chat))
                                {
                                    await Owner.OwnerAPI.ShowIncomingTasks(chat, _botClient, this);
                                }
                            }
                            if (updateMessage == "Активные заказы")
                            {
                                if (await Owner.OwnerAPI.IsOwner(chat))
                                {
                                    await Owner.OwnerAPI.ShowActiveTasks(chat, _botClient, this);
                                }
                            }
                            if (updateMessage == "Завершенные заказы")
                            {
                                if (await Owner.OwnerAPI.IsOwner(chat))
                                {
                                    await Owner.OwnerAPI.ShowFinishedTasks(chat, _botClient, this);
                                }
                            }

                            if (updateMessage == "/start")
                            {
                                await SetRoute("checkSub", chat);
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
                             
                            }

                            return;
                        }
                    case UpdateType.CallbackQuery:
                        {

                            if (update.CallbackQuery.Message.Chat.Id == GroupId) return;
                            var callbackQuery = update.CallbackQuery;
                            var chat = update.CallbackQuery.Message.Chat;
                            if (!ChatStates.ContainsKey(chat)) ChatStates.Add(chat, ChatState.Standard);
                            if (ChatStates[chat] != ChatState.Standard)
                            {
                                if (callbackQuery.Data == "main")
                                {
                                    ChatStates[chat] = ChatState.Standard;
                                }
                                else if (callbackQuery.Data[0..4] == "emer")
                                {
                                    ChatStates[chat] = ChatState.Standard;
                                }
                                else if (callbackQuery.Data == "main/new")
                                {
                                    await Owner.OwnerAPI.NotifyOnwers(_botClient, $"Ебать,у нас новый заказ нахуй!");
                                    await SetRoute("main", chat);
                                    ChatStates[chat] = ChatState.Standard;
                                    await ClearCart(chat, showMessage: false);
                                    return;
                                }
                                else return;
                            }
                    
                            await Utils.Log($" [{chat.Id}] {callbackQuery.Data}");
                            await botClient.AnswerCallbackQueryAsync(callbackQuery.Id, "Секунду");



                            await botClient.DeleteMessageAsync(chat, callbackQuery.Message.MessageId);
                           

                            await SetRoute(callbackQuery.Data, chat);
                            return;
                        }
                }
            }catch(Exception e)
            {
                await Utils.Log(e.Message, ConsoleColor.DarkRed);
            }
       }

       async Task ErrorHanlder(ITelegramBotClient botClient, Exception error, CancellationToken cancelToken)
        {
            var ErrorMessage = error switch
            {
                ApiRequestException apiRequestException => $"Telegram bot API error:\n[{apiRequestException.ErrorCode} {apiRequestException.Message}",
                _ => error.ToString()
            };
            await Utils.Log($"{ErrorMessage}", ConsoleColor.DarkRed);
         
        }
    }
}
