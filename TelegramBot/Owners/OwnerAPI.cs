using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

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

        public static async Task ShowAllTasks(ChatId chat,ITelegramBotClient _botClient)
        {

           


            foreach (var item in TelegramAPI.TelegramBot.Context.Purchases)
            {
                InlineKeyboardMarkup inlineKeyboard = null;                
                if(item.State == 0)
                {
                    inlineKeyboard = new InlineKeyboardMarkup(
                         new List<InlineKeyboardButton[]>()
                          {

                                 new InlineKeyboardButton[]
                                 {
                                    InlineKeyboardButton.WithCallbackData("Выполнить(предупредить пользователя)","warn/" + $"{item.CustomerID}" + "|" + $"{item.ID}" + "|" + $"{chat.Identifier}"),

                                 },
                                  new InlineKeyboardButton[]
                                 {
                                    InlineKeyboardButton.WithCallbackData("Назад","Main")
                                 },

                         });
                }else if(item.State == 1)
                {
                    inlineKeyboard = new InlineKeyboardMarkup(
                        new List<InlineKeyboardButton[]>()
                         {

                                 new InlineKeyboardButton[]
                                 {
                                    InlineKeyboardButton.WithCallbackData("Заказ завершен(сообщить покупателю)","done/" + $"{item.CustomerID}" + "|" + $"{item.ID}"),

                                 },
                                  new InlineKeyboardButton[]
                                 {
                                    InlineKeyboardButton.WithCallbackData("Назад","Main")
                                 },

                        });
                }
                else if (item.State == 1)
                {
                    inlineKeyboard = new InlineKeyboardMarkup(
                        new List<InlineKeyboardButton[]>()
                         {

                                 new InlineKeyboardButton[]
                                 {
                                    InlineKeyboardButton.WithCallbackData("Заказ завершен(сообщить покупателю)","done/" + $"{item.CustomerID}" + "|" + $"{item.ID}"),

                                 },
                                  new InlineKeyboardButton[]
                                 {
                                    InlineKeyboardButton.WithCallbackData("Неправильный email","Main")
                                 },
                                  new InlineKeyboardButton[]
                                 {
                                    InlineKeyboardButton.WithCallbackData("Назад","Main")
                                 },

                        });
                }



                
                    
                    
                
                await _botClient.SendTextMessageAsync(chat, $"{item.ID} {item.GetStringState()}\n\r{item.CustomerName}\r\n{item.Goods}\r\n{item.Date}",replyMarkup:inlineKeyboard);
            }
            
            

        }
    }
}
