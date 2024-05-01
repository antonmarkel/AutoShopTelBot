using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
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
            await _botclient.SendTextMessageAsync(chatId,"Привествуем, хозяин!",replyMarkup:replyKeyboard);
        }

        public static async Task NotifyOnwers(ITelegramBotClient _botClient, string message)
        {
            foreach (var owner in Owners)
            {
                ChatId chatOwner = new ChatId(owner);
                await _botClient.SendTextMessageAsync(chatOwner,message); 
            }
        }

        public static async Task ShowIncomingTasks(ChatId chat,ITelegramBotClient _botClient,TelegramAPI.TelegramBot _Bot)
        {
            var purchases = _Bot.Context.Purchases.Where(v => v.State == 0).ToList();
            for(int i = 0;i < purchases.Count;i++) { 
                var purch = purchases[i];   
                InlineKeyboardMarkup inlineKeyboard = new InlineKeyboardMarkup(
                         new List<InlineKeyboardButton[]>()
                          {

                                 new InlineKeyboardButton[]
                                 {
                                    InlineKeyboardButton.WithCallbackData("Запросить код","warn/" + $"{purch.CustomerID}" + "|" + $"{purch.ID}" + "|" + $"{chat.Identifier}"),

                                 },
                                  new InlineKeyboardButton[]
                                 {
                                    InlineKeyboardButton.WithCallbackData("Назад","Main")
                                 },
                                 new InlineKeyboardButton[]
                                 {
                                    InlineKeyboardButton.WithCallbackData("Запросить имейл еще раз",$"emer|{purch.CustomerID}|{purch.ID}")

                                 },
                                  new InlineKeyboardButton[]
                                 {
                                    InlineKeyboardButton.WithCallbackData("Нет оплаты",$"empa|{purch.CustomerID}|{purch.ID}")

                                 },

                         }) ;
                StringBuilder goodsString = new StringBuilder();
                for(int j = 0;j < purch.Goods.Count;j++)
                {
                    var item = Items.All.FirstOrDefault(v => v.Identifier == purch.Goods[j]);
                    goodsString.Append($"{j+1}) {item.Category} : {item.Name}\r\n");
                }
                string preAnswer = $"Номер заказа:№{i + 1}\r\nID заказа: {purch.ID}\r\nТовары:\r\n{goodsString}\r\nИнфа по заказу:\r\n{purch.Data}\r\n {purch.Date}";
                string fuckSym = "_*[]()~>#+-=|{}.!";
                StringBuilder normalAnswer = new StringBuilder();
                for(int j = 0;j < preAnswer.Length;j++)
                {
                    if (fuckSym.Contains(preAnswer[j]))
                    {
                        normalAnswer.Append("\\");
                    }
                    normalAnswer.Append(preAnswer[j]);
                }
               Console.WriteLine(normalAnswer.ToString());  
                await _botClient.SendTextMessageAsync(chat,normalAnswer.ToString(), replyMarkup: inlineKeyboard, parseMode:Telegram.Bot.Types.Enums.ParseMode.MarkdownV2);
            }
        }
        public static async Task ShowActiveTasks(ChatId chat, ITelegramBotClient _botClient, TelegramAPI.TelegramBot _Bot)
        {
            var purchases = _Bot.Context.Purchases.Where(v => v.State == 1).ToList();
            for(int i = 0;i <  purchases.Count;i++) 
            {
                var purch = purchases[i];
                InlineKeyboardMarkup inlineKeyboard = new InlineKeyboardMarkup(
                         new List<InlineKeyboardButton[]>()
                          {

                                 new InlineKeyboardButton[]
                                 {
                                    InlineKeyboardButton.WithCallbackData("Завершен","done|" + $"{purch.CustomerID}" + "|" + $"{purch.ID}" + "|" + $"{chat.Identifier}"),

                                 }, 
                                 new InlineKeyboardButton[]
                                 {
                                    InlineKeyboardButton.WithCallbackData("Запросить код еще раз",$"emco|{purch.CustomerID}|{purch.ID}")

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
                    goodsString.Append($"{j + 1}){item.Category} : {item.Name}\r\n");
                }
                string preAnswer = $"Номер заказа:№{i + 1}\r\nID заказа: {purch.ID}\r\nТовары:\r\n{goodsString}\r\nИнфа по заказу:\r\n{purch.Data}\r\n {purch.Date}";
                string fuckSym = "_*[]()~>#+-=|{}.!";
                StringBuilder normalAnswer = new StringBuilder();
                for (int j = 0; j < preAnswer.Length; j++)
                {
                    if (fuckSym.Contains(preAnswer[j]))
                    {
                        normalAnswer.Append("\\");
                    }
                    normalAnswer.Append(preAnswer[j]);
                }


                await _botClient.SendTextMessageAsync(chat, normalAnswer.ToString(), replyMarkup: inlineKeyboard,parseMode:Telegram.Bot.Types.Enums.ParseMode.MarkdownV2);
            }
        }
        public static async Task ShowFinishedTasks(ChatId chat, ITelegramBotClient _botClient, TelegramAPI.TelegramBot _Bot)
        {
            var purchases = _Bot.Context.Purchases.Where(v => v.State == 2).ToList();
            foreach (var item in purchases)
            {

                await _botClient.SendTextMessageAsync(chat, $"{item.ID} {item.GetStringState()}\n\r{item.CustomerName}\r\n{item.Data}\r\n{item.Goods}\r\n{item.Date}");
            }
        }
    }
}
