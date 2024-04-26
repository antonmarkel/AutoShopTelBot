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
    }
}
