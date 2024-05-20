using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using TelegramBot.Items;


namespace TelegramBot
{
   public class Page
   {
        public string Path { get; set; }
        public string Text { get; set; }
        public string? PictureFileID { get; set; } = null;
        public IReplyMarkup? Markup { get; set; }
        public static Page New(string Path, string Text, IReplyMarkup Markup,string? Pitcure = null) {
            return new Page()
            {
                Path = Path,
                Text = Text,
                Markup = Markup,
                PictureFileID = Pitcure
            };
        }

        public static readonly List<Page> StandardPages = new()
        {
            Page.New("page/main","Приветствуем в новом боте!",new InlineKeyboardMarkup(
                new List<InlineKeyboardButton[]>(){
                    new InlineKeyboardButton[]{
                        InlineKeyboardButton.WithCallbackData("К товарам!","page/items"),
                    },
                    new InlineKeyboardButton[]
                    {
                        InlineKeyboardButton.WithCallbackData("Мой профиль","page/profile"),
                        InlineKeyboardButton.WithCallbackData("Информация","page/info")
                    }
            })),
            Page.New("page/items","Выберите категорию!",new InlineKeyboardMarkup(
                new List<InlineKeyboardButton[]>(){
                    new InlineKeyboardButton[]{
                        InlineKeyboardButton.WithCallbackData("Тестовый 1!",$"func/items|{CategoryTypes.Types[0]}"),
                    },
                    new InlineKeyboardButton[]{
                        InlineKeyboardButton.WithCallbackData("Тестовый 2!",$"func/items|{CategoryTypes.Types[1]}"),
                    },
                     new InlineKeyboardButton[]{
                        InlineKeyboardButton.WithCallbackData("Назад",$"page/main"),
                    },
            })),
            Page.New("page/info","Информация",new InlineKeyboardMarkup(
                new List<InlineKeyboardButton[]>(){
                    new InlineKeyboardButton[]{
                        InlineKeyboardButton.WithCallbackData("Назад","page/main"),
                    },
            })),
        };
        public static readonly List<Page> AdminPages = new()
        {
            Page.New("admin/main","Рады вас видеть, хозяин!",new InlineKeyboardMarkup(
                new List<InlineKeyboardButton[]>(){
                    new InlineKeyboardButton[]{
                        InlineKeyboardButton.WithCallbackData("Добавить","admin/add"),
                    },
                    new InlineKeyboardButton[]{
                        InlineKeyboardButton.WithCallbackData("Изменить","admin/change"),
                    },
                    new InlineKeyboardButton[]{
                        InlineKeyboardButton.WithCallbackData("Удалить","admin/delete"),
                    },
                    new InlineKeyboardButton[]
                    {
                        InlineKeyboardButton.WithCallbackData("Информация о покупках","admin/data"),
                    },
                     new InlineKeyboardButton[]
                    {
                        InlineKeyboardButton.WithCallbackData("Назад","page/main"),
                    }
            })),

           Page.New("admin/add","Новая картинка,товар,категория? Отлично!",new InlineKeyboardMarkup(
                new List<InlineKeyboardButton[]>(){
                    new InlineKeyboardButton[]{
                        InlineKeyboardButton.WithCallbackData("Добавить категорию","func/addCategory"),
                    },
                    new InlineKeyboardButton[]{
                        InlineKeyboardButton.WithCallbackData("Добавить товар","func/addItem"),
                    },
                    new InlineKeyboardButton[]{
                        InlineKeyboardButton.WithCallbackData("Добавить картинку","admin/delete"),
                    },
                    new InlineKeyboardButton[]
                    {
                        InlineKeyboardButton.WithCallbackData("Назад","admin/main"),

                    }
            })),
        };
    }
  

}
