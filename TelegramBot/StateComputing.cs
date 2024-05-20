using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using TelegramBot.DataContext;
using TelegramBot.Items;
using TelegramBot.User;

namespace TelegramBot
{
    public static class StateComputing
    {
        public enum ChatStates : short
        {
            Standard = 0,
            GetCategoryName = 1,
            GetCategoryDescription = 2,
            GetCategoryPhoto = 3,
            GetCategoryType = 4,

            AddPhoto = 5,
            ChoosePhoto = 6,
        }

        public static async Task ComputeState(UserData data, ITelegramBotClient _botClient,BotContext context,Update update,Chat chat)
        {
            var State = data.ChatState;
            var messageId = data.CurrentMessageId;
            switch (State)
            {
                #region CategoryAdd

                case ChatStates.GetCategoryName:
                    {
                        if (update.Type != Telegram.Bot.Types.Enums.UpdateType.Message)
                        {
                            await _botClient.EditMessageCaptionAsync(chat, messageId, "Надо скидывать текстовое сообщение");
                            return;
                            //TODO: change media
                        }
                        var text = update.Message!.Text;
                        data.TempData.Add("CategoryAdd", new Category() { Name = text! });
                        data.ChatState = ChatStates.GetCategoryDescription;
                        await _botClient.EditMessageCaptionAsync(chat, messageId, "Напишите описание категории");
                        break;
                    }
                case ChatStates.GetCategoryDescription:
                    {
                        if (update.Type != Telegram.Bot.Types.Enums.UpdateType.Message)
                        {
                            await _botClient.EditMessageCaptionAsync(chat, messageId, "Надо скидывать текстовое сообщение");
                            return;
                            //TODO: change media
                        }
                        var text = update.Message!.Text;

                        if (!data.TempData.ContainsKey("CategoryAdd")) {
                            await _botClient.EditMessageCaptionAsync(chat, messageId, "Извините,возникла ошибка,давайте начнем сначала!\nНапишете название категории!");
                            data.ChatState = ChatStates.GetCategoryName;
                            return;
                        }

                        ((Category)data.TempData["CategoryAdd"]).Description = text;

                        data.ChatState = ChatStates.GetCategoryPhoto;
                        await _botClient.EditMessageCaptionAsync(chat, messageId, "Скиньте картинку для категории");
                        break;
                    }
                case ChatStates.GetCategoryPhoto:
                    {
                        if (update.Type != Telegram.Bot.Types.Enums.UpdateType.Message)
                        {
                            await _botClient.EditMessageCaptionAsync(chat, messageId, "Надо скидывать текстовое сообщение");
                            return;
                            //TODO: change media
                        }
                        if(update.Message!.Photo == null)
                        {
                            await _botClient.EditMessageCaptionAsync(chat, messageId, "Нужна фотография!");
                            return;
                        }
                        if (!data.TempData.ContainsKey("CategoryAdd"))
                        {
                            await _botClient.EditMessageCaptionAsync(chat, messageId, "Извините,возникла ошибка,давайте начнем сначала!\nНапишете название категории!");
                            data.ChatState = ChatStates.GetCategoryName;
                            return;
                        }

                        var _pictureFileId = update.Message!.Photo[0].FileId;

                        ((Category)data.TempData["CategoryAdd"]).PictureID = _pictureFileId;

                        data.ChatState = ChatStates.GetCategoryType;
                        var markupList = new List<List<InlineKeyboardButton>>();
                        foreach (var category in CategoryTypes.Types)
                        {
                            markupList.Add(new List<InlineKeyboardButton>() { InlineKeyboardButton.WithCallbackData(category) });
                        }
                        var markup = new InlineKeyboardMarkup(markupList);
                        await _botClient.EditMessageCaptionAsync(chat, messageId, "Осталось только выбрать куда добавить!",replyMarkup:markup);
                        break;
                    }
                case ChatStates.GetCategoryType:
                    {
                      
                        if (update.Type != Telegram.Bot.Types.Enums.UpdateType.CallbackQuery)
                        {
                            await _botClient.EditMessageCaptionAsync(chat, messageId, "Нужно выбрать на кнопочках бро!");
                            return;
                            //TODO: change media
                        }
                        if (!data.TempData.ContainsKey("CategoryAdd"))
                        {
                            await _botClient.EditMessageCaptionAsync(chat, messageId, "Извините,возникла ошибка,давайте начнем сначала!\nНапишете название категории!");
                            data.ChatState = ChatStates.GetCategoryName;
                            return;
                        }
                        var text = update.CallbackQuery!.Data;
                        ((Category)data.TempData["CategoryAdd"]).CategoryType = text!;
                   
                        await context.Categories.AddAsync((Category)data.TempData["CategoryAdd"]);
                        data.TempData.Remove("CategoryAdd");

                        data.ChatState = ChatStates.Standard;

                        var markupList = new List<List<InlineKeyboardButton>>();
                        markupList.Add(new List<InlineKeyboardButton>() { InlineKeyboardButton.WithCallbackData("Проверить", $"func/items|{text}") });
                        var markup = new InlineKeyboardMarkup(markupList);

                        await _botClient.EditMessageCaptionAsync(chat, messageId, "Все добавлено!",replyMarkup:markup);
                        
                        break;
                    }
                #endregion

                case ChatStates.AddPhoto: 
                    {
                        await _botClient.EditMessageCaptionAsync(chat, messageId, "Пришлите фотку вместе с названием\nИспользуя название ее можно будет использовать еще раз",replyMarkup:null);

                        if (update.Type != Telegram.Bot.Types.Enums.UpdateType.Message)
                        {
                            await _botClient.EditMessageCaptionAsync(chat, messageId, "Надо скидывать текстовое сообщение");
                            return;
                            //TODO: change media
                        }
                        if (update.Message!.Photo == null)
                        {
                            await _botClient.EditMessageCaptionAsync(chat, messageId, "Нужна фотография!");
                            return;
                        }
                      
                        var _pictureFileId = update.Message!.Photo[0].FileId;
                        var _pictureName = update.Message!.Caption;
                        await context.AddAsync(new Picture(_pictureFileId, _pictureName!));

                        data.ChatState = ChatStates.GetCategoryType;
                        var markupList = new List<List<InlineKeyboardButton>>();
                        foreach (var category in CategoryTypes.Types)
                        {
                            markupList.Add(new List<InlineKeyboardButton>() { InlineKeyboardButton.WithCallbackData(category) });
                        }
                        var markup = new InlineKeyboardMarkup(markupList);
                        await _botClient.EditMessageCaptionAsync(chat, messageId, "Осталось только выбрать куда добавить!", replyMarkup: markup);
                        break;
                        break;
                    }

            }
        }
    }
}
