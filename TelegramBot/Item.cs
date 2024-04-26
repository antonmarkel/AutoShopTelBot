using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace TelegramBot
{
    public class Item
    {
        public string? Name { get;set; }
        public string Description { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public InputMediaPhoto Picture { get; set; }
        public async Task SendItemAsync(ITelegramBotClient _botClient, ChatId chat)
        {
            if (Picture != null) Telegram.TelegramRoutes.MessagesToDelete.AddRange( await _botClient.SendMediaGroupAsync(chat, new List<IAlbumInputMedia>() { Picture }));
            Telegram.TelegramRoutes.MessagesToDelete.Add(await _botClient.SendTextMessageAsync(chat, $"Вы собирайтесь купить товар с названием:\r\n{Name}\r\nза {Price}₽\n {Description}\n"));
        }
    }
}
