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
            new Item("brlili") { Name = "Новый персонаж Лили",Category ="Brawl Starts", Price = 1999, },
    };
}
public class Item
{
        public string? Name { get;set; }
        public string Identifier { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Category { get;set; } = string.Empty;
        public decimal Price { get; set; }
        public InputMediaPhoto Picture { get; set; }
        public async Task SendItemAsync(ITelegramBotClient _botClient, ChatId chat)
        {
            var inlineKeyboard = new InlineKeyboardMarkup(
                               new List<InlineKeyboardButton[]>()
                               {
                                 new InlineKeyboardButton[]
                                 {
                                   InlineKeyboardButton.WithCallbackData("Да","cart/" + this.Identifier),
                                   InlineKeyboardButton.WithCallbackData("Нет","main/items"),
                                 },
                               }) ;
           //if (Picture != null) Telegram.TelegramBot.MessagesToDelete[chat].AddRange( await _botClient.SendMediaGroupAsync(chat, new List<IAlbumInputMedia>() { Picture }));
           await _botClient.SendTextMessageAsync(chat, $"🗳️ <strong>Категория:</strong> {this.Category}\r\n🛍️ <strong>Товар:</strong> {Name}\r\n🔖 <strong>Цена:</strong> {Price}₽\r\n\r\n<strong>✅Вы выбрали товар👆</strong>\r\n\r\n\U0001f6d2Хотите добавить в корзину?",replyMarkup:inlineKeyboard,parseMode:Telegram.Bot.Types.Enums.ParseMode.Html);
        }

        public Item(string Identifier) {  this.Identifier = Identifier; }
    }
}
