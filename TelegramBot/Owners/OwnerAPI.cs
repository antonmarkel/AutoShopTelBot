using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using TelegramBot.Data;
using TelegramBot.TelegramAPI;

namespace TelegramBot.Owner
{
    public class OwnerAPI
    {
        
        public static List<long> Owners { get; set; } = new List<long>(){ 851274731, 5270690035 };


        public static async Task<bool>IsOwner(ChatId chatId)
        {
            return Owners.FirstOrDefault(v => v == chatId.Identifier) != 0;
        }

        public static async Task OwnerMenu(ITelegramBotClient _botclient,ChatId chatId)
        {
            var replyKeyboard = new ReplyKeyboardMarkup(
                                  new List<KeyboardButton[]>()
                                  {
                                        new KeyboardButton[]
                                        {
                                            new KeyboardButton("Входящие заказы"),
                                        },
                                         new KeyboardButton[]
                                        {
                                            new KeyboardButton("Активные заказы"),
                                        },
                                          new KeyboardButton[]
                                        {
                                            new KeyboardButton("Завершенные заказы"),
                                        },

                                  });
            await _botclient.SendMessage(chatId,"Привествуем, хозяин!",replyMarkup:replyKeyboard);
        }

        public static async Task NotifyOnwers(ITelegramBotClient _botClient, string message, IReplyMarkup markup = null,ParseMode? mode = null)
        {
            foreach (var owner in Owners)
            {

                ChatId chatOwner = new ChatId(owner);
                if(markup != null)
                {
                    await _botClient.SendMessage(chatOwner, message, replyMarkup:markup,parseMode:mode);
                }
                else
                {
                    await _botClient.SendMessage(chatOwner, message, parseMode: mode);
                }
              
            }
        }

        public static async Task ShowTask(ChatId chat,ITelegramBotClient _botClient, TelegramAPI.TelegramBot _Bot,Purchase purch)
        {
            if(_Bot.Context.Purchases.FirstOrDefault(v => v.Identifier == purch.Identifier) == null)
            {
                await _botClient.SendMessage(chat, "Заказ не найден!");
                return;
            }
            if (purch.State == 0) await ShowIncomingTask(chat, _botClient, purch);
            else if(purch.State == 1) await ShowActiveTask(chat,_botClient, purch);
            else if(purch.State == 2)await ShowFinishedTask(chat,_botClient, purch);
        }
        static async Task ShowIncomingTask(ChatId chat,ITelegramBotClient _botClient,Purchase purch)
        {
            InlineKeyboardMarkup inlineKeyboard = new InlineKeyboardMarkup(
                     new List<InlineKeyboardButton[]>()
                      {

                                 new InlineKeyboardButton[]
                                 {
                                    InlineKeyboardButton.WithCallbackData("Запросить код","warn/" + $"{purch.CustomerID}" + "|" + $"{purch.Identifier}" + "|" + $"{chat.Identifier}"),

                                 },
                                   new InlineKeyboardButton[]
                                 {
                                    InlineKeyboardButton.WithCallbackData("Оплата не найдена","docu/" + $"{purch.CustomerID}" + "|" + $"{purch.Identifier}" + "|" + $"{chat.Identifier}"),

                                 },
                                 new InlineKeyboardButton[]
                                 {
                                    InlineKeyboardButton.WithCallbackData("Запросить имейл еще раз",$"emer|{purch.CustomerID}|{purch.Identifier}")

                                 },
                                  new InlineKeyboardButton[]
                                 {
                                    InlineKeyboardButton.WithCallbackData("Сообщение покупателю",$"askq|{purch.CustomerID}|{purch.Identifier}")

                                 },
                                  new InlineKeyboardButton[]
                                 {
                                    InlineKeyboardButton.WithCallbackData("Нет оплаты(Удалить заказ)",$"empa|{purch.CustomerID}|{purch.Identifier}")

                                 },
                                 new InlineKeyboardButton[]
                                 {
                                    InlineKeyboardButton.WithCallbackData("Удалить заказ",$"kill|{purch.CustomerID}|{purch.Identifier}")

                                 },

                                    new InlineKeyboardButton[]
                                 {
                                    InlineKeyboardButton.WithCallbackData("Назад","Main")
                                 },

                     });
            StringBuilder goodsString = new StringBuilder();
            for (int j = 0; j < purch.Goods.Count; j++)
            {
                var item = Items.All.FirstOrDefault(v => v.Identifier == purch.Goods[j]);
                goodsString.Append($"{j + 1}) {item.Category} : {item.Name}\r\n");
            }
            string preAnswer = $"Номер заказа: {purch.ID}\r\nID заказа: {purch.Identifier}\r\nОбщая цена: <b>{purch.Cost}₽</b>\r\n🏦Способ оплаты: {purch.PaymentSystem}\r\nТовары:\r\n{goodsString}\r\nИнфа по заказу:\r\n{purch.Data}\r\n {purch.Date}";
        
            if (purch.Pict != null)
            {
                await _botClient.SendPhoto(chat, purch.Pict, caption: preAnswer, replyMarkup: inlineKeyboard, parseMode: Telegram.Bot.Types.Enums.ParseMode.Html);
            }
            else
            {
                await _botClient.SendMessage(chat, preAnswer, replyMarkup: inlineKeyboard, parseMode: Telegram.Bot.Types.Enums.ParseMode.Html);
            }
        }
        static async Task ShowActiveTask(ChatId chat,ITelegramBotClient _botClient,Purchase purch)
        {
            InlineKeyboardMarkup inlineKeyboard = new InlineKeyboardMarkup(
                         new List<InlineKeyboardButton[]>()
                          {
                                 new InlineKeyboardButton[]
                                 {
                                    InlineKeyboardButton.WithCallbackData("Завершен","done|" + $"{purch.CustomerID}" + "|" + $"{purch.Identifier}" + "|" + $"{chat.Identifier}"),

                                 },
                                 new InlineKeyboardButton[]
                                 {
                                    InlineKeyboardButton.WithCallbackData("Предупредить пользователя!",$"game|{purch.CustomerID}|{purch.Identifier}")

                                 },
                               
                                 new InlineKeyboardButton[]
                                 {
                                    InlineKeyboardButton.WithCallbackData("Запросить код еще раз",$"emco|{purch.CustomerID}|{purch.Identifier}")

                                 },
                                 new InlineKeyboardButton[]
                                 {
                                     InlineKeyboardButton.WithCallbackData("Запросить тэг для Telegram",$"etag|{purch.CustomerID}|{purch.Identifier}")
                                 },
                                       new InlineKeyboardButton[]
                                 {
                                    InlineKeyboardButton.WithCallbackData("Сообщение покупателю",$"askq|{purch.CustomerID}|{purch.Identifier}")

                                 },
                                  new InlineKeyboardButton[]
                                 {
                                    InlineKeyboardButton.WithCallbackData("Удалить заказ",$"kill|{purch.CustomerID}|{purch.Identifier}")

                                 },
                                   new InlineKeyboardButton[]
                                 {
                                    InlineKeyboardButton.WithCallbackData("Возврат","done|" + $"{purch.CustomerID}" + "|" + $"{purch.Identifier}" + "|" + $"{chat.Identifier}"),

                                 },
                                    new InlineKeyboardButton[]
                                 {
                                    InlineKeyboardButton.WithCallbackData("Назад","Main")
                                 },

                         }) ; 
            StringBuilder goodsString = new StringBuilder();
            for (int j = 0; j < purch.Goods.Count; j++)
            {
                var item = Items.All.FirstOrDefault(v => v.Identifier == purch.Goods[j]);
                goodsString.Append($"{j + 1}){item.Category} : {item.Name}\r\n");
            }
            string preAnswer = $"Номер заказа: {purch.ID}\r\nID заказа: {purch.Identifier}\r\nОбщая цена: <b>{purch.Cost}₽</b>\r\n🏦Способ оплаты: {purch.PaymentSystem}\r\nТовары:\r\n{goodsString}\r\nИнфа по заказу:\r\n{purch.Data}\r\n {purch.Date}";
          
            if (purch.Pict != null)
            {
                await _botClient.SendPhoto(chat, purch.Pict, caption: preAnswer, replyMarkup: inlineKeyboard, parseMode: Telegram.Bot.Types.Enums.ParseMode.Html);
            }
            else
            {
                await _botClient.SendMessage(chat, preAnswer, replyMarkup: inlineKeyboard, parseMode: Telegram.Bot.Types.Enums.ParseMode.Html);
            }
        }
        static async Task ShowFinishedTask(ChatId chat, ITelegramBotClient _botClient,Purchase purch)
        {
            InlineKeyboardMarkup inlineKeyboard = new InlineKeyboardMarkup(
                       new List<InlineKeyboardButton[]>()
                        {
                                 new InlineKeyboardButton[]
                                 {
                                    InlineKeyboardButton.WithCallbackData("Не завершен","undo|" + $"{purch.Identifier}"),

                                 },
                       });
            await _botClient.SendMessage(chat, $"Номер заказа: {purch.ID}\r\n{purch.Identifier} {purch.GetStringState()}\r\nОбщая цена: <b>{purch.Cost}₽</b>\r\n🏦Способ оплаты: {purch.PaymentSystem}\r\nID пользователя: {purch.CustomerID}\r\n{purch.Goods}\r\n{purch.Date}", replyMarkup: inlineKeyboard,parseMode:Telegram.Bot.Types.Enums.ParseMode.Html);
        }


        public static async Task ShowIncomingTasks(ChatId chat,ITelegramBotClient _botClient,TelegramAPI.TelegramBot _Bot)
        {
            if(_Bot.Context.Purchases.FirstOrDefault(v => v.State == 0) == null) { await _botClient.SendTextMessageAsync(chat, "Нет входящих заказов!");return; }
            foreach (var purch in _Bot.Context.Purchases.Where(v => v.State == 0)) await ShowIncomingTask(chat, _botClient, purch.ToModel());
        }
        public static async Task ShowActiveTasks(ChatId chat, ITelegramBotClient _botClient, TelegramAPI.TelegramBot _Bot)
        {
            if (_Bot.Context.Purchases.FirstOrDefault(v => v.State == 1) == null) { await _botClient.SendTextMessageAsync(chat, "Нет активных заказов!Работай лучше!"); return; }
            foreach (var purch in _Bot.Context.Purchases.Where(v => v.State == 1)) await ShowActiveTask(chat, _botClient, purch.ToModel());
        }
        public static async Task ShowFinishedTasks(ChatId chat, ITelegramBotClient _botClient, TelegramAPI.TelegramBot _Bot)
        {
            if (_Bot.Context.Purchases.FirstOrDefault(v => v.State == 2) == null) { await _botClient.SendTextMessageAsync(chat, "Нет завершенных заказов!Где деньги блять?!"); return; }
            foreach (var purch in _Bot.Context.Purchases.Where(v => v.State == 2)) await ShowFinishedTask(chat, _botClient, purch.ToModel());
        }
    }
}
