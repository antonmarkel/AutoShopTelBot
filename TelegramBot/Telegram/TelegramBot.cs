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
    }
    public class TelegramBot
    {
        public long GroupId { get; set; } = -1002091931355;
        public Dictionary<ChatId, int> CurrentPurchase { get; set; } = new Dictionary<ChatId, int>();
        public Dictionary<ChatId, ChatState> ChatStates { get; set; } = new Dictionary<ChatId, ChatState>();
        public  Dictionary<ChatId, List<Message>> MessagesToDelete { get; set; } = new Dictionary<ChatId, List<Message>>();
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
            Utils.Log("Bot started", ConsoleColor.DarkGreen);
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
                            if (update.Message.Chat.Id == GroupId) return;

                            Utils.Log($"Text message {update.Message.Chat.Id}", ConsoleColor.DarkBlue);

                            var updateMessage = update.Message.Text;
                            var chat = update.Message.Chat;

                            if (!MessagesToDelete.ContainsKey(chat)) { MessagesToDelete.Add(chat, new List<Message>()); }
                            if (!ChatStates.ContainsKey(update.Message.Chat)) ChatStates.Add(update.Message.Chat, ChatState.Standard);
                            //Console.WriteLine($"{update.Message.Photo[0].FileId}");
                            //      return;
                            switch (ChatStates[chat])
                            {
                                case ChatState.GetEmail:
                                    {
                                        Utils.Log($"Got email from {chat.Id}", ConsoleColor.DarkGreen);
                                        var message = update.Message.Text;
                                        var purch = Context.Purchases.FirstOrDefault(v => v.ID == CurrentPurchase[chat]);



                                        if (!Utils.IsValidEmail(message))
                                        {
                                            await _botClient.SendTextMessageAsync(chat, "⛔️ Вы ввели неверный формат почты:\r\n\r\n⚠️ Формат: example@gmail.com\r\n✅ Пример: vanya228@mail.ru\r\n\r\n👇 Зайдите в раздел Supercell ID в игре, чтобы посмотреть вашу почту на нужном аккаунте и напишите почту повторно:");
                                        }
                                        else
                                        {
                                            var inlineKeyboard = new InlineKeyboardMarkup(
                                                  new List<InlineKeyboardButton[]>()
                                                  {
                                                 new InlineKeyboardButton[]
                                                 {
                                                   InlineKeyboardButton.WithCallbackData("Перейти к заказу",$"show|{purch.ID}"),
                                                 },
                                             });

                                            purch.Data += "\r\nEmail: " + message;
                                            await _botClient.SendTextMessageAsync(chat, $"\U0001f6d2 Заказ: {purch.ID}\r\n👤 Статус: Ожидание продавца\r\n⏰ Время: {purch.Date}");
                                            await SetRoute("main", chat);
                                            await Owner.OwnerAPI.NotifyOnwers(_botClient, "Новый заказ!Прислали почту!", markup: inlineKeyboard);


                                            ChatStates[chat] = ChatState.Standard;
                                        }
                                        return;
                                    }
                                case ChatState.GetCode:
                                    {
                                        var message = update.Message.Text;
                                        var purch = Context.Purchases.FirstOrDefault(v => v.ID == CurrentPurchase[chat]);
                                        if (!Utils.IsValidCode(message))
                                        {
                                            await _botClient.SendTextMessageAsync(chat, "⛔️ Вы ввели неверный код с почты:\r\n\r\n⚠️ Формат: 123456\r\n✅ Пример: 782861\r\n\r\n👇 Проверьте <u>входящие</u>, <u>спам</u>, <u>промо-акции</u> в своём приложении/сайте и напишите правильный код повторно:", parseMode: ParseMode.Html);
                                            return;
                                        }
                                        purch.Data += "\r\nCode: " + "<code>" + message + "</code>";
                                        var inlineKeyboard = new InlineKeyboardMarkup(
                                                  new List<InlineKeyboardButton[]>()
                                                  {
                                                 new InlineKeyboardButton[]
                                                 {
                                                   InlineKeyboardButton.WithCallbackData("Перейти к заказу",$"show|{purch.ID}"),
                                                 },
                                             });
                                        await Owner.OwnerAPI.NotifyOnwers(_botClient, "Пришел код к заказу", markup: inlineKeyboard);
                                        ChatStates[chat] = ChatState.Standard;
                                        return;
                                    }
                                case ChatState.GetDoc:
                                    {
                                        if (update.Message.Photo != null)
                                        {
                                            Context.Purchases.FirstOrDefault(v => v.ID == CurrentPurchase[chat]).PictFileID = update.Message.Photo[0].FileId;
                                            var purch = Context.Purchases.FirstOrDefault(v => v.ID == CurrentPurchase[chat]);
                                            await Context.SaveChangesAsync();
                                            ChatStates[update.Message.Chat] = ChatState.Standard;
                                            var inlineKeyboard = new InlineKeyboardMarkup(
                                                  new List<InlineKeyboardButton[]>()
                                                  {
                                                 new InlineKeyboardButton[]
                                                 {
                                                   InlineKeyboardButton.WithCallbackData("Перейти к заказу",$"show|{purch.ID}"),
                                                 },
                                             });
                                            await Owner.OwnerAPI.NotifyOnwers(_botClient, "Получено подтверждение заказа", markup: inlineKeyboard);
                                            await SetRoute("main", chat);
                                        }
                                        else
                                        {
                                            _botClient.SendTextMessageAsync(chat, "Пожалуйста,скиньте подтверждение в виде скриншота");
                                        }
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
                                    await Owner.OwnerAPI.NotifyOnwers(_botClient, "Ебать,у нас новый заказ нахуй!");
                                    await SetRoute("main", chat);
                                    ChatStates[chat] = ChatState.Standard;
                                    await ClearCart(chat, showMessage: false);
                                    return;
                                }
                                else return;
                            }



                            if (!MessagesToDelete.ContainsKey(chat)) { MessagesToDelete.Add(chat, new List<Message>()); }
                            Utils.Log($" [{chat.Id}] {callbackQuery.Data}");
                            await botClient.AnswerCallbackQueryAsync(callbackQuery.Id, "Секунду");


                            foreach (var message in MessagesToDelete[chat])
                            {
                                await _botClient.DeleteMessageAsync(chat, message.MessageId);
                            }
                            MessagesToDelete[chat] = new List<Message>();

                            await _botClient.DeleteMessageAsync(chat, callbackQuery.Message.MessageId);

                            await SetRoute(callbackQuery.Data, chat);
                            return;
                        }
                }
            }catch(Exception e)
            {
                Utils.Log(e.Message, ConsoleColor.DarkRed);
            }
       }

       Task ErrorHanlder(ITelegramBotClient botClient, Exception error, CancellationToken cancelToken)
        {
            var ErrorMessage = error switch
            {
                ApiRequestException apiRequestException => $"Telegram bot API error:\n[{apiRequestException.ErrorCode} {apiRequestException.Message}",
                _ => error.ToString()
            };
            Utils.Log($"{ErrorMessage}", ConsoleColor.DarkRed);
            return Task.CompletedTask;
        }
    }
}
