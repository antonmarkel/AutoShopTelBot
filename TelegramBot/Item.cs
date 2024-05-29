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
            new Item("brgems30") { Name = "30 гемов",Category = "Brawl Stars",Price = 225, Picture = Resources.Resources.GemsPict},
            new Item("brgems80") { Name = "80 гемов",Category = "Brawl Stars", Price = 525, Picture = Resources.Resources.GemsPict},
            new Item("brgems170") { Name = "170 гемов",Category = "Brawl Stars", Price = 950, Picture = Resources.Resources.GemsPict },
            new Item("brgems360") { Name = "360 гемов",Category = "Brawl Stars", Price = 1900, Picture = Resources.Resources.GemsPict },
            new Item("brgems950") { Name = "950 гемов",Category = "Brawl Stars", Price = 4650, Picture = Resources.Resources.GemsPict },
            new Item("brgems2000") { Name = "2000 гемов",Category = "Brawl Stars", Price = 9250, Picture = Resources.Resources.GemsPict },
            new Item("brpass") { Name = "Brawl Pass",Category = "Brawl Stars", Price = 650,  },
            new Item("brpass+") { Name = "Brawl Pass+",Category = "Brawl Stars", Price = 1000, },
            new Item("brupg") { Name = "Улучшение БП на БП+",Category = "Brawl Stars", Price = 450, },

             //new Item("brblings") { Name = "🔥 23000 блингов",Category = "Brawl Stars", Price = 1099,IsSpecialOffer = true},
             // new Item("bractgg") { Name = "🔥 4000 монет + 2750 очков силы",Category = "Brawl Stars", Price = 1400,IsSpecialOffer = true},
             // new Item("brleon") { Name = "🔥 Гиперзаряд Леона",Category = "Brawl Stars", Price = 449,IsSpecialOffer = true},
 
              //new Item("brspec1") { Name = "🔥  5000 блингов",Category = "Brawl Stars", Price = 250,IsSpecialOffer = true},
                new Item("brspec8") { Name = "🔥 5800 блингов",Category = "Brawl Stars", Price = 349,IsSpecialOffer = true},

         
            // new Item("brspec") { Name = "🔥 10000 блингов",Category = "Brawl Stars", Price = 550,IsSpecialOffer = true},
          /*new Item("brspec2") { Name = "🔥 100 гемов",Category = "Brawl Stars", Price = 549,IsSpecialOffer = true},
              new Item("brspec3") { Name = "🔥 190 гемов",Category = "Brawl Stars", Price = 999,IsSpecialOffer = true},
                  new Item("brspec4") { Name = "🔥 215 гемов",Category = "Brawl Stars", Price = 999,IsSpecialOffer = true},
            new Item("brspec5") { Name = "🔥 385 гемов",Category = "Brawl Stars", Price = 1999,IsSpecialOffer = true},
             new Item("brspec6") { Name = "🔥 440 гемов",Category = "Brawl Stars", Price = 1999,IsSpecialOffer = true},
               new Item("brspec7") { Name = "🔥 1050 гемов",Category = "Brawl Stars", Price = 4750,IsSpecialOffer = true},
                   new Item("brspec9") { Name = "🔥 2200 гемов",Category = "Brawl Stars", Price = 9500,IsSpecialOffer = true},
             */
           // new Item("brlili") { Name = "Новый персонаж Лили",Category ="Brawl Stars", Price = 1999, },



            new Item("clgems80") { Name = "80 гемов",Category = "Clash Royale", Price = 150, Picture = Resources.Resources.GemsPict},
             new Item("clgems500") { Name = "500 гемов",Category = "Clash Royale", Price = 550, Picture = Resources.Resources.GemsPict},
              new Item("clgems1200") { Name = "1200 гемов",Category = "Clash Royale", Price = 1000, Picture = Resources.Resources.GemsPict},
               new Item("clgems2500") { Name = "2500 гемов",Category = "Clash Royale", Price = 2000, Picture = Resources.Resources.GemsPict},
                new Item("clgems6500") { Name = "6500 гемов",Category = "Clash Royale", Price = 4650, Picture = Resources.Resources.GemsPict},
                 new Item("clgems14000") { Name = "14000 гемов",Category = "Clash Royale", Price = 9250, Picture = Resources.Resources.GemsPict},
                  new Item("clgoldpass") { Name = "Золотой пропуск",Category = "Clash Royale", Price = 650},
                   new Item("clironpass") { Name = "Алмазный пропуск",Category = "Clash Royale", Price = 1250},
                   
            new Item("clsgems80") { Name = "80 гемов",Category = "Clash of Clans", Price = 150, Picture = Resources.Resources.GemsPict},
             new Item("clcgems500") { Name = "500 гемов",Category = "Clash of Clans", Price = 550, Picture = Resources.Resources.GemsPict},
              new Item("clсgems1200") { Name = "1200 гемов",Category = "Clash of Clans", Price = 1000, Picture = Resources.Resources.GemsPict},
               new Item("clcgems2500") { Name = "2500 гемов",Category = "Clash of Clans", Price = 2000, Picture = Resources.Resources.GemsPict},
                new Item("clcgems6500") { Name = "6500 гемов",Category = "Clash of Clans", Price = 4650, Picture = Resources.Resources.GemsPict},
                 new Item("clcgems14000") { Name = "14000 гемов",Category = "Clash of Clans", Price = 9250, Picture = Resources.Resources.GemsPict},
                  new Item("clcgoldpass") { Name = "Золотой пропуск",Category = "Clash of Clans", Price = 650},
                    new Item("clcpass") { Name = "Пропуск события",Category = "Clash of Clans", Price = 550},
                     new Item("clcvil") { Name = "Оформление деревни",Category = "Clash of Clans", Price = 1000},
                

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
        public bool IsSpecialOffer { get; set; } = false;
        public async Task SendItemAsync(ITelegramBotClient _botClient, ChatId chat, string backRoute = "main/itemsPrev")
        {
            var inlineKeyboard = new InlineKeyboardMarkup(
                               new List<InlineKeyboardButton[]>()
                               {
                                  new InlineKeyboardButton[]
                                 {
                                   InlineKeyboardButton.WithCallbackData("Купить",$"fast|{Identifier}"),

                                 },
                                 new InlineKeyboardButton[]
                                 {
                                   InlineKeyboardButton.WithCallbackData("Добавить в корзину","cart/" + this.Identifier),  
                                 },
                                   new InlineKeyboardButton[]
                                 {
                                    InlineKeyboardButton.WithCallbackData("Назад",backRoute),

                                 },
                               }) ;
            if (Picture != null) await _botClient.SendPhoto(chat, Picture, caption: $"🗳️ <strong>Категория:</strong> {this.Category}\r\n🛍️ <strong>Товар:</strong> {Name}\r\n🔖 <strong>Цена:</strong> {Price}₽\r\n\r\n<strong>✅Вы выбрали товар👆</strong>\r\n\r\n\U0001f6d2Хотите добавить в корзину?", replyMarkup: inlineKeyboard, parseMode: Telegram.Bot.Types.Enums.ParseMode.Html);
          else  await _botClient.SendMessage(chat, $"🗳️ <strong>Категория:</strong> {this.Category}\r\n🛍️ <strong>Товар:</strong> {Name}\r\n🔖 <strong>Цена:</strong> {Price}₽\r\n\r\n<strong>✅Вы выбрали товар👆</strong>",replyMarkup:inlineKeyboard,parseMode:Telegram.Bot.Types.Enums.ParseMode.Html);
        }

        public Item(string Identifier) {  this.Identifier = Identifier; }
    }
}
