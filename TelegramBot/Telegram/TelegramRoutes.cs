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
        public static InputMediaPhoto StartPicture { get; set; } = new InputMediaPhoto(InputFile.FromStream(new FileStream("Resources/test.jpg", FileMode.Open), fileName: "start.jpg"));
        public static InputMediaPhoto GemsPicture { get; set; } = new InputMediaPhoto(InputFile.FromStream(new FileStream("Resources/gems.jpg", FileMode.Open), fileName: "gems.jpg"));
  
        public static async Task GetRenderPayment(ITelegramBotClient _botClient,ChatId chat, string item,int price,string routeBack)
        {
            var inlineKeyboard = new InlineKeyboardMarkup(
                            new List<InlineKeyboardButton[]>()
                            {
                                 new InlineKeyboardButton[]
                                 {
                                   InlineKeyboardButton.WithCallbackData("Оплачено","main"),
                                   InlineKeyboardButton.WithCallbackData("Назад",routeBack)
                                 },

                            });
            await _botClient.SendTextMessageAsync(chat, $"Вы собирайтесь оплатить товар {item} за {price}Р");
            await _botClient.SendTextMessageAsync(chat, "Переведите  на любой из указанных реквизитов:\r\n\r\n💳 Тинькофф:\r\nСБП • +79939245527\r\n\r\n💳 Тинькофф:\r\nКарта • 2200700850594697\r\n\r\n💳 Сбер:\r\nКарта • 5336690284035310\r\n\r\nПосле оплаты пришлите чек/скрин оплаты в данный чат и нажмите кнопку \"Оплачено\"", replyMarkup: inlineKeyboard);
            return;
        }
        public static async Task GetRenderByRoute(string route,ITelegramBotClient _botClient,ChatId chat)
        {
            InlineKeyboardMarkup inlineKeyboard = null;
            switch (route)
            {
                case "main":
                    {
                     //  MessagesToDelete = (await _botClient.SendMediaGroupAsync(chat,
                     //       new List<IAlbumInputMedia>() { StartPicture })).ToList();


                        inlineKeyboard = new InlineKeyboardMarkup(
                             new List<InlineKeyboardButton[]>()
                             { 
                                 new InlineKeyboardButton[]
                                 {
                                    InlineKeyboardButton.WithCallbackData("Выбор игры","main/items"),
                                 },
                                 new InlineKeyboardButton[]
                                 {
                                    InlineKeyboardButton.WithUrl("Отзывы","https://t.me/LanReviews"),
                                    InlineKeyboardButton.WithUrl("Поддержка","https://t.me/lancaster696")
                                 },

                             });
                        await _botClient.SendTextMessageAsync(chat, "Приветствуем вас! Здесь вы сможете ознакомиться с товарами", replyMarkup: inlineKeyboard);
                        return;
                    }
                case "main/feedback":
                    {
                        var inlineKe = new InlineKeyboardMarkup(
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

                        inlineKeyboard = new InlineKeyboardMarkup(
                             new List<InlineKeyboardButton[]>()
                              {

                                 new InlineKeyboardButton[]
                                 {
                                     InlineKeyboardButton.WithCallbackData("Brawl Stars","main/items/brawl"),                      
                                 },
                                 new InlineKeyboardButton[]
                                 {
                                  
                                    InlineKeyboardButton.WithCallbackData("Clash Royale","main/items/royale"),
                                    InlineKeyboardButton.WithCallbackData("Clash of Clans","main/items/clans"),
                                   
                                 },
                                 new InlineKeyboardButton[]
                                 {
 
                                    InlineKeyboardButton.WithCallbackData("Назад","main"),
                                 },

                             });
                        await _botClient.SendTextMessageAsync(chat, "Выберите игру", replyMarkup: inlineKeyboard);
                        return;
                    }
                case "main/items/brawl":
                    {
                    
                            /*MessagesToDelete = (await _botClient.SendMediaGroupAsync(chat,
                            new List<IAlbumInputMedia>() {
                            GemsPicture,
                             }
                            )).ToList();*/
                     
 
                        inlineKeyboard = new InlineKeyboardMarkup(
                             new List<InlineKeyboardButton[]>()
                              {

                                 new InlineKeyboardButton[]
                                 {
                                    InlineKeyboardButton.WithCallbackData("30 гемов","main/items/brawl/gems30"),
                                    InlineKeyboardButton.WithCallbackData("80 гемов","main/items/brawl/gems80"),
                                    InlineKeyboardButton.WithCallbackData("170 гемов","main/items/brawl/gems170"),
                                 },

                                 new InlineKeyboardButton[]
                                 {
                                    InlineKeyboardButton.WithCallbackData("360 гемов","main/items/brawl/gems360"),
                                    InlineKeyboardButton.WithCallbackData("950 гемов","main/items/brawl/gems950"),
                                    InlineKeyboardButton.WithCallbackData("2000 гемов","main/items/brawl/gems2000"),
                                 },

                                 new InlineKeyboardButton[]
                                 {
                                    InlineKeyboardButton.WithCallbackData("Brawl Pass","main/items/brawl/pass"),
                                     InlineKeyboardButton.WithCallbackData("Brawl Pass +","main/items/brawl/passplus"),
                                    InlineKeyboardButton.WithCallbackData("Улучшения БП","main/items/brawl/WPupgrate"),
         
                                 },
                                 new InlineKeyboardButton[]
                                 {
                                    InlineKeyboardButton.WithCallbackData("Новый боец Лили","main/items/brawl/lili"),          
                                 },
                                   new InlineKeyboardButton[]
                                 {
                                    InlineKeyboardButton.WithCallbackData("Назад","main/items"),
                                 },
                             });
                        await _botClient.SendTextMessageAsync(chat, "Товары доступные для Brawl Stars", replyMarkup: inlineKeyboard);
                        return;
                    }
                case "main/items/brawl/gems30":
                {
                        await GetRenderPayment(_botClient, chat, "Brawl Stars гемы 30", 249, "main/items/brawl");
                        return;
                    }
                case "main/items/brawl/gems80":
                    {
                        await GetRenderPayment(_botClient, chat, "Brawl Stars гемы 80", 549, "main/items/brawl");
                        return;
                    }
                case "main/items/brawl/gems170":
                    {
                        await GetRenderPayment(_botClient, chat, "Brawl Stars гемы 170", 999, "main/items/brawl");
                        return;
                    }
                case "main/items/brawl/gems360":
                    {
                        await GetRenderPayment(_botClient, chat, "Brawl Stars гемы 360", 1999, "main/items/brawl");
                        return;
                    }
                case "main/items/brawl/gems950":
                    {
                        await GetRenderPayment(_botClient, chat, "Brawl Stars гемы 950", 4499, "main/items/brawl");
                        return;
                    }

                case "main/items/brawl/gems2000":
                    {
                        await GetRenderPayment(_botClient, chat, "Brawl Stars гемы 2000", 8999, "main/items/brawl");
                        return;
                    }
                case "main/items/brawl/pass":
                    {
                        await GetRenderPayment(_botClient, chat, "Brawl Pass", 749, "main/items/brawl");
                        return;
                    }
                case "main/items/brawl/passplus":
                    {
                        await GetRenderPayment(_botClient, chat, "Brawl Pass +", 1049, "main/items/brawl");
                        return;
                    }
                case "main/items/brawl/WPupgrate":
                    {
                        await GetRenderPayment(_botClient, chat, "Улучшение БП на БП+", 449, "main/items/brawl");
                        return;
                    }
                case "main/items/brawl/lili":
                    {
                        await GetRenderPayment(_botClient, chat, "Новый персонаж Лили", 1999, "main/items/brawl");
                        return;
                    }

            }

        }
    }
}
