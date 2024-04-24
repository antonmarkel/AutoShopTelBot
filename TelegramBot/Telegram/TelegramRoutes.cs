using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace TelegramBot.Telegram
{
    public class TelegramRoutes
    {
        public static List<Message>? MessagesToDelete{ get; set; }
        public static async Task GetRenderByRoute(string route,ITelegramBotClient _botClient,ChatId chat)
        {
            switch (route)
            {
                case "main":
                    { 
                        var pict1 = new InputMediaPhoto(InputFile.FromStream(new FileStream("Resources/test.jpg", FileMode.Open),fileName:"test.jpg"));

                        MessagesToDelete = (await _botClient.SendMediaGroupAsync(chat,
                        new List<IAlbumInputMedia>() {
                            pict1,
                           })).ToList();
                      

                        var inlineKeyboard = new InlineKeyboardMarkup(
                             new List<InlineKeyboardButton[]>()
                              {

                                 new InlineKeyboardButton[]
                                 {
                                    InlineKeyboardButton.WithCallbackData("Товары","main/items"),
                                    InlineKeyboardButton.WithCallbackData("Оставить отзыв","main/feedback"),
                                 },

                             });
                        await _botClient.SendTextMessageAsync(chat, "Приветствуем вас! Здесь вы сможете ознакомиться с товарами", replyMarkup: inlineKeyboard);
                        return;
                    }
                case "main/feedback":
                    {
                        var inlineKeyboard = new InlineKeyboardMarkup(
                            new List<InlineKeyboardButton[]>()
                             {

                                 new InlineKeyboardButton[]
                                 {
                                    InlineKeyboardButton.WithCallbackData("Назад","main"),
                                 },

                            });
                        await _botClient.SendTextMessageAsync(chat, "Можете оставить ваш отзыв здесь!", replyMarkup: inlineKeyboard);
                      
                             
                        return;
                    }
                case "main/items":
                    {

                        var inlineKeyboard = new InlineKeyboardMarkup(
                             new List<InlineKeyboardButton[]>()
                              {

                                 new InlineKeyboardButton[]
                                 {
                                    InlineKeyboardButton.WithCallbackData("Clash Royale","main/items/clash"),
                                    InlineKeyboardButton.WithCallbackData("Назад","main"),
                                 },

                             });
                        await _botClient.SendTextMessageAsync(chat, "Выберите игру", replyMarkup: inlineKeyboard);
                        return;
                    }
                case "main/items/clash":
                    {
                        //TODO: upload images
                        using(var stream = new FileStream("Resources/gems.jpg", FileMode.Open)){
                            var pict1 = new InputMediaPhoto(InputFile.FromStream(stream, fileName: "test.jpg"));
                            MessagesToDelete = (await _botClient.SendMediaGroupAsync(chat,
                            new List<IAlbumInputMedia>() {
                            pict1,
                          })).ToList();
                        }
                     
                       


                        var inlineKeyboard = new InlineKeyboardMarkup(
                             new List<InlineKeyboardButton[]>()
                              {

                                 new InlineKeyboardButton[]
                                 {
                                    InlineKeyboardButton.WithCallbackData("30 Гемов","main/items/clash/gems30"),
                                    InlineKeyboardButton.WithCallbackData("100 Гемов","main/items/clash/gems30"),
                                 },

                                 new InlineKeyboardButton[]
                                 {
                                    InlineKeyboardButton.WithCallbackData("300 Гемов","main/items/clash/gems30"),
                                    InlineKeyboardButton.WithCallbackData("1000 Гемов","main/items/clash/gems30"),
                                 },

                                 new InlineKeyboardButton[]
                                 {
                                     InlineKeyboardButton.WithCallbackData("2000 Гемов","main/items/clash/gems30"),
                                    InlineKeyboardButton.WithCallbackData("Назад","main/items"),
                                 }

                             });
                        await _botClient.SendTextMessageAsync(chat, "Товары доступные для Clash Royale", replyMarkup: inlineKeyboard);
                        return;
                    }
            }

        }
    }
}
