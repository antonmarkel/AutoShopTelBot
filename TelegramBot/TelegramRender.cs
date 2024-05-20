using System;
using System.Collections.Generic;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot.Types.ReplyMarkups;
using Telegram.Bot.Types;
using Telegram.Bot;
using TelegramBot.Items;
using TelegramBot.DataContext;

namespace TelegramBot
{
    public static class TelegramRender
    {
        public static async Task RenderPage(string path, ITelegramBotClient _botClient,ChatId chat, int messageId)
        {
            var page = Page.StandardPages.FirstOrDefault(v => v.Path == path);
            if (page == null)page = Page.AdminPages.FirstOrDefault(v => v.Path == path);
            if (page != null) { await RenderPage(page, _botClient,chat, messageId); }
           
        }


        public static async Task RenderPage(Page page, ITelegramBotClient _botClient, ChatId chat, int messageId)
        {
            //if (page.PictureFileID == null)
            //{
                await _botClient.EditMessageCaptionAsync(chat, messageId, page.Text, replyMarkup: (InlineKeyboardMarkup)page.Markup);
            //}
            //else
            //{ 
                //await _botClient.EditMessageMediaAsync(message.Chat, message.MessageId, null  , replyMarkup: (InlineKeyboardMarkup)page.Markup);
            //}
        }

        public static async Task RenderCategory(Category category,ITelegramBotClient _botClient,BotContext context,Message message)
        {
            var items = context.Items.Where(v => v.CategoryID == category.Id).ToList();
            var markupList = new List<List<InlineKeyboardButton>>();
            foreach (var item in items)
            {
                markupList.Add(new List<InlineKeyboardButton>() { InlineKeyboardButton.WithCallbackData(item.Name, item.ID.ToString()) });
            }
            markupList.Add(new List<InlineKeyboardButton>() { InlineKeyboardButton.WithCallbackData("Назад","page/main")});
            var markup = new InlineKeyboardMarkup(markupList);

            if (category.PictureID != null)
            {
                //TODO: safe
                var media = new InputMediaPhoto(InputFile.FromFileId(context.Pictures.FirstOrDefault(v => v.ID == category.PictureID).FileIdentifier));
                await _botClient.EditMessageMediaAsync(message.Chat, message.MessageId, media, replyMarkup: markup);
                await _botClient.EditMessageCaptionAsync(message.Chat, message.MessageId, category.Description);
            }
            else
            {
                await _botClient.EditMessageCaptionAsync(message.Chat, message.MessageId, category.Description,replyMarkup:markup);
            }
        }
    }
}
