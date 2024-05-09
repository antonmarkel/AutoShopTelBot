using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace TelegramBot
{
    public static class Items
    {
        public static List<Item> All = new List<Item>()
        {
            new Item("brgems30") { Name = "30 гемов",Category = "Brawl Stars",Price = 249, Picture = Resources.Resources.GemsPict},
            new Item("brgems80") { Name = "80 гемов",Category = "Brawl Stars", Price = 549, Picture = Resources.Resources.GemsPict},
            new Item("brgems170") { Name = "170 гемов",Category = "Brawl Stars", Price = 999, Picture = Resources.Resources.GemsPict },
            new Item("brgems360") { Name = "360 гемов",Category = "Brawl Stars", Price = 1999, Picture = Resources.Resources.GemsPict },
            new Item("brgems950") { Name = "950 гемов",Category = "Brawl Stars", Price = 4499, Picture = Resources.Resources.GemsPict },
            new Item("brgems2000") { Name = "2000 гемов",Category = "Brawl Stars", Price = 8999, Picture = Resources.Resources.GemsPict },
            new Item("brpass") { Name = "Brawl Pass",Category = "Brawl Stars", Price = 749,  },
            new Item("brpass+") { Name = "Brawl Pass+",Category = "Brawl Stars", Price = 1049, },
            new Item("brupg") { Name = "Улучшение БП на БП+",Category = "Brawl Stars", Price = 449, },
            new Item("brlili") { Name = "Новый персонаж Лили",Category ="Brawl Stars", Price = 1999, },

            new Item("clgems80") { Name = "80 гемов",Category = "Clash Royale", Price = 150, Picture = Resources.Resources.GemsPict},
             new Item("clgems500") { Name = "500 гемов",Category = "Clash Royale", Price = 600, Picture = Resources.Resources.GemsPict},
              new Item("clgems1200") { Name = "1200 гемов",Category = "Clash Royale", Price = 1200, Picture = Resources.Resources.GemsPict},
               new Item("clgems2500") { Name = "2500 гемов",Category = "Clash Royale", Price = 2100, Picture = Resources.Resources.GemsPict},
                new Item("clgems6500") { Name = "6500 гемов",Category = "Clash Royale", Price = 5000, Picture = Resources.Resources.GemsPict},
                 new Item("clgems14000") { Name = "14000 гемов",Category = "Clash Royale", Price = 10000, Picture = Resources.Resources.GemsPict},
                  new Item("clgoldpass") { Name = "Золотой пропуск",Category = "Clash Royale", Price = 650},
                   new Item("clironpass") { Name = "Алмазный пропуск",Category = "Clash Royale", Price = 1300},
                   
            new Item("clsgems80") { Name = "80 гемов",Category = "Clash of Clans", Price = 150, Picture = Resources.Resources.GemsPict},
             new Item("clcgems500") { Name = "500 гемов",Category = "Clash of Clans", Price = 600, Picture = Resources.Resources.GemsPict},
              new Item("clсgems1200") { Name = "1200 гемов",Category = "Clash of Clans", Price = 1200, Picture = Resources.Resources.GemsPict},
               new Item("clcgems2500") { Name = "2500 гемов",Category = "Clash of Clans", Price = 2100, Picture = Resources.Resources.GemsPict},
                new Item("clcgems6500") { Name = "6500 гемов",Category = "Clash Royale", Price = 5000, Picture = Resources.Resources.GemsPict},
                 new Item("clcgems14000") { Name = "14000 гемов",Category = "Clash of Clans", Price = 10000, Picture = Resources.Resources.GemsPict},
                  new Item("clcgoldpass") { Name = "Золотой пропуск",Category = "Clash of Clans", Price = 650},
                    new Item("clcpass") { Name = "Пропуск события",Category = "Clash of Clans", Price = 650},

              new Item("telprem1") { Name = "Телеграм преимиум на месяц",Category = "Telegram", Price = 300},
             new Item("telprem12") { Name = "Телеграм преимиум на 12 месяцев",Category = "Telegram", Price = 1800},
        

    };
}
public class Item
{
        public string? Name { get;set; }
        public string Identifier { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Category { get;set; } = string.Empty;
        public ulong Price { get; set; }
        public InputFile Picture { get; set; }
        public async Task SendItemAsync(ITelegramBotClient _botClient, ChatId chat)
        {
            var inlineKeyboard = new InlineKeyboardMarkup(
                               new List<InlineKeyboardButton[]>()
                               {
                                 new InlineKeyboardButton[]
                                 {
                                   InlineKeyboardButton.WithCallbackData("Да","cart/" + this.Identifier),
                                   InlineKeyboardButton.WithCallbackData("Нет","main/itemsPrev"),
                                 },
                               }) ;
            if (Picture != null) await _botClient.SendPhotoAsync(chat, Picture, caption: $"🗳️ <strong>Категория:</strong> {this.Category}\r\n🛍️ <strong>Товар:</strong> {Name}\r\n🔖 <strong>Цена:</strong> {Price}₽\r\n\r\n<strong>✅Вы выбрали товар👆</strong>\r\n\r\n\U0001f6d2Хотите добавить в корзину?", replyMarkup: inlineKeyboard, parseMode: Telegram.Bot.Types.Enums.ParseMode.Html);
          else  await _botClient.SendTextMessageAsync(chat, $"🗳️ <strong>Категория:</strong> {this.Category}\r\n🛍️ <strong>Товар:</strong> {Name}\r\n🔖 <strong>Цена:</strong> {Price}₽\r\n\r\n<strong>✅Вы выбрали товар👆</strong>\r\n\r\n\U0001f6d2Хотите добавить в корзину?",replyMarkup:inlineKeyboard,parseMode:Telegram.Bot.Types.Enums.ParseMode.Html);
        }

        public Item(string Identifier) {  this.Identifier = Identifier; }
    }
}
