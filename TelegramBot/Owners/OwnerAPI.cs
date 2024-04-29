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


        public static async Task SetUpAdminPanel(ChatId chat,ITelegramBotClient _botClient)
        {
            var replyKeyboard = new ReplyKeyboardMarkup(
                                  new List<KeyboardButton[]>()
                                  {
                                        new KeyboardButton[]
                                        {
                                            new KeyboardButton("Просмотреть заказы"),
                                        },
                                     
                                  });
            _botClient.SendTextMessageAsync(chat, "Admin Panel", replyMarkup: replyKeyboard);
        }

        public static async Task NotifyOnwerAboutIncomingData(ITelegramBotClient _botClient, TelegramAPI.TelegramBot bot, ChatId chat,ushort state)
        {
            foreach(var owner in Owners)
            {
                ChatId chatOwner = new ChatId(owner);
                if(state == 1) await _botClient.SendTextMessageAsync(chatOwner, $"Получен код для заказа {bot.CurrentPurchase[chat]}");
            }
        }
        public static async Task NotifyOnwers(ITelegramBotClient _botClient, string message)
        {
            foreach (var owner in Owners)
            {
                ChatId chatOwner = new ChatId(owner);
                await _botClient.SendTextMessageAsync(chatOwner,message); 
            }
        }

        public static async Task ShowIncomingTasks(ChatId chat,ITelegramBotClient _botClient)
        {
            var purchases = TelegramAPI.TelegramBot.Context.Purchases.Where(v => v.State == 0).ToList();
            foreach (var item in purchases)
            {
                InlineKeyboardMarkup inlineKeyboard = null;                
                if(item.State == 0)
                {
                    inlineKeyboard = new InlineKeyboardMarkup(
                         new List<InlineKeyboardButton[]>()
                          {

                                 new InlineKeyboardButton[]
                                 {
                                    InlineKeyboardButton.WithCallbackData("Запросить код","warn/" + $"{item.CustomerID}" + "|" + $"{item.ID}" + "|" + $"{chat.Identifier}"),

                                 },
                                  new InlineKeyboardButton[]
                                 {
                                    InlineKeyboardButton.WithCallbackData("Назад","Main")
                                 },
                                 new InlineKeyboardButton[]
                                 {
                                    InlineKeyboardButton.WithCallbackData("Отменить заказ(email)",$"emer|{item.CustomerID}|{item.ID}")

                                 },
                                  new InlineKeyboardButton[]
                                 {
                                    InlineKeyboardButton.WithCallbackData("Отменить заказ(оплата)",$"empa|{item.CustomerID}|{item.ID}")

                                 },

                         }) ;
                }
                await _botClient.SendTextMessageAsync(chat, $"{item.ID} {item.GetStringState()}\n\r{item.CustomerName}\r\n{item.Data}\r\n{item.Goods}\r\n{item.Date}",replyMarkup:inlineKeyboard);
            }
        }
    }
}
