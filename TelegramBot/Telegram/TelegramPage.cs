using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace TelegramBot.Telegram
{
    public class TelegramPage
    {
        public static List<TelegramPage> Pages { get; set; } = new List<TelegramPage>();
        public readonly TelegramRoute Route;
        public readonly List<IAlbumInputMedia>? Media;
        public readonly string? Text;
        public readonly InlineKeyboardMarkup? ButtonsMarkup;
       /*
                                    new List<InlineKeyboardButton[]>() {
                                        new InlineKeyboardButton[]
                                        {
                                            InlineKeyboardButton.WithUrl("Тут пиздят твои деньги","https://pornohub.com"),
                                            InlineKeyboardButton.WithCallbackData("Ты даун","dick")
                                        },
                                        new InlineKeyboardButton[]
                                        {
                                            InlineKeyboardButton.WithCallbackData("Fuck you","dick"),
                                            InlineKeyboardButton.WithCallbackData("Fuck you2","dick")
                                        }
                                    }
                                );*/

        public static void Add(TelegramRoute route, string text, List<IAlbumInputMedia> media, List<TelegramButton[]> buttons)
        {
            Pages.Add(new TelegramPage(route, text, media, buttons));
        }
        public static async Task<bool> Open(ITelegramBotClient _botClient,ChatId chat,TelegramRoute route)
        {
            var page = Pages.FirstOrDefault((v) => v.Route.Page == route.Page);
            if(page == null) { return false; }
            else
            {
                await page.RenderAsync(_botClient, chat);
                return true;
            }
        }
        private TelegramPage(TelegramRoute route,string text, List<IAlbumInputMedia> media, List<TelegramButton[]> buttons)
        {
            Route = route;
            Text = text;
            //Media = media;
            List<InlineKeyboardButton[]> btns = new List<InlineKeyboardButton[]>();
            for(int i = 0;i < buttons.Count;i++) 
            {
                btns.Add(new InlineKeyboardButton[buttons[i].Length]);
                for(int j = 0;j < buttons[i].Length; j++)
                {
                    btns[i][j].Text = buttons[i][j].Text;
                        btns[i][j] = buttons[i][j].IsLink ? InlineKeyboardButton.WithUrl(buttons[i][j].Text, buttons[i][j].ToRoute.Page)
                                              : InlineKeyboardButton.WithCallbackData(buttons[i][j].ToRoute.Page);
                }
            }

            ButtonsMarkup = new InlineKeyboardMarkup(btns);
                 
        }

        public async Task RenderAsync(ITelegramBotClient _botClient,ChatId chat)
        {
            if(Media != null)await _botClient.SendMediaGroupAsync(chat, Media);
            if (Text != null) await _botClient.SendTextMessageAsync(chat, Text,replyMarkup:ButtonsMarkup);
        }
    }
}
