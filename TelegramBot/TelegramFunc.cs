using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using TelegramBot.DataContext;
using TelegramBot.User;

namespace TelegramBot
{
    public static class TelegramFunc
    {
        public static async Task RenderFunc(string dataString, BotContext context,ITelegramBotClient _botClient,Message message,UserData userData)
        {
            var data = dataString.Split('/')[1].Split('|');
            var funcName = data[0];
            switch(funcName)
            {
                case "addCategory":
                    await AddCategory(_botClient,context,message,userData);
                    return;
                case "items":
                    await ShowCategories(context,_botClient,message,data);
                    return;
                case "cat":
                    await ShowCategory(context, _botClient, message, data);
                    return;
            }
        }

        static async Task AddCategory(ITelegramBotClient _botClient,BotContext context, Message message,UserData userData) {
            await _botClient.EditMessageCaptionAsync(message.Chat, message.MessageId, "Напишите название категории!",replyMarkup:null);
            userData.ChatState = StateComputing.ChatStates.GetCategoryName;
        }

        static async Task ShowCategories(BotContext context,ITelegramBotClient _botClient,Message message, string[] data)
        {
            var type = data[1];
            var markupList = new List<List<InlineKeyboardButton>>(); 
            foreach(var category in context.Categories.Where(v => v.CategoryType == type))
            {
                markupList.Add(new List<InlineKeyboardButton>() { InlineKeyboardButton.WithCallbackData(category.Name,$"func/cat|{category.Id}") });
            }
            markupList.Add(new List<InlineKeyboardButton>(){ InlineKeyboardButton.WithCallbackData("Назад","page/items")});
            var markup = new InlineKeyboardMarkup(markupList);
            await _botClient.EditMessageCaptionAsync(message.Chat, message.MessageId, "Доступные категории",replyMarkup:markup);
        }

        static async Task ShowCategory(BotContext context, ITelegramBotClient _botClient, Message message, string[] data)
        {
            var id = int.Parse(data[1]);
            var category = context.Categories.FirstOrDefault(c => c.Id == id);
            await TelegramRender.RenderCategory(category, _botClient, context, message);
        }




    }
}
