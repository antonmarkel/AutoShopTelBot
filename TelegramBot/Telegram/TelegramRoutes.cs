using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using TelegramBot.Data;

namespace TelegramBot.TelegramAPI
{
    public class TelegramRoutes
    {

        public static TelegramBot _Bot { get; set; }
        public static InputMediaPhoto StartPicture { get; set; } = new InputMediaPhoto(InputFile.FromStream(new FileStream("Resources/test.jpg", FileMode.Open), fileName: "start.jpg"));
        public static InputMediaPhoto GemsPicture { get; set; } = new InputMediaPhoto(InputFile.FromStream(new FileStream("Resources/gems.jpg", FileMode.Open), fileName: "gems.jpg"));
  
        public static async Task GetRenderPayment(ITelegramBotClient _botClient,ChatId chat, Item item,string routeBack)
        {
           
            await item.SendItemAsync(_botClient, chat);
            await _botClient.SendTextMessageAsync(chat, "<strong>Переведите  на любой из указанных реквизитов:</strong>\r\n\r\n💳 Тинькофф:\r\nСБП • +79939245527\r\n\r\n💳 Тинькофф:\r\nКарта • 2200700850594697\r\n\r\n💳 Сбер:\r\nКарта • 5336690284035310\r\n\r\nПосле оплаты пришлите чек/скрин оплаты в данный чат и нажмите кнопку \"Оплачено\"",parseMode:ParseMode.Html);
            return;
        }
        public static async Task GetRenderByRoute(string route,ITelegramBotClient _botClient,ChatId chat)
        {
            InlineKeyboardMarkup inlineKeyboard = null;
            if (route[0..3] == "buy")
            {
                var good = route[4..];
                Console.WriteLine(good);
                var item = Items.All.FirstOrDefault(v => v.Identifier == good);
                if (item != null)
                {
                    await item.SendItemAsync(_botClient, chat);
                }
                return;
            }
            else if (route[0..4] == "cart")
            {
                var good = route[5..];
                Console.WriteLine(good);
                var item = Items.All.FirstOrDefault(v => v.Identifier == good);
                if (item != null)
                {
                    if (!_Bot.Cart.ContainsKey(chat))
                    {
                        _Bot.Cart.Add(chat, new List<Item>());
                    }
                     inlineKeyboard = new InlineKeyboardMarkup(
                              new List<InlineKeyboardButton[]>()
                              {
                                 new InlineKeyboardButton[]
                                 {
                                   InlineKeyboardButton.WithCallbackData("Просмотреть корзину","main/cart"),
                                   InlineKeyboardButton.WithCallbackData("Назад","main/items"),
                                 },
                              });
                    await _botClient.SendTextMessageAsync(chat, "Товар добавлен в корзину!", replyMarkup: inlineKeyboard);
                    _Bot.Cart[chat].Add(item);
                }
                return;
            }
            else if (route[0..4] == "warn")
            {
                //▶️
                var data = route[5..].Split('|');
                long userId = long.Parse(data[0]);
                int ID = int.Parse(data[1]);
                long ownerID = long.Parse(data[2]);
                var purch = TelegramBot.Context.Purchases.FirstOrDefault(v => v.ID == ID);
                if(purch == null) { await _botClient.SendTextMessageAsync(new ChatId(ownerID), "Заказ был прерван!");return; }
                ChatId cChat = new ChatId(userId);
                if (!_Bot.CurrentPurchase.ContainsKey(chat)) _Bot.CurrentPurchase.Add(chat, ID);
                _Bot.CurrentPurchase[chat] = ID;

                inlineKeyboard = new InlineKeyboardMarkup(
                            new List<InlineKeyboardButton[]>()
                            {
                                 new InlineKeyboardButton[]
                                 {
                                    InlineKeyboardButton.WithCallbackData("Отправил","main"),
                                 },
                            });
                await _botClient.SendTextMessageAsync(cChat, $"Заказ {ID}.Пожалуйста, введите код присланый вам на email.",replyMarkup:inlineKeyboard);
                _Bot.ChatStates[chat] = ChatState.GetData;
                var item = TelegramBot.Context.Purchases.FirstOrDefault(v => v.ID == ID);
                item.State = 1;
                await TelegramBot.Context.SaveChangesAsync();
                return;
            }
            else if (route[0..4] == "done")
            {
                //▶️
                var data = route[5..].Split('|');
                long userId = long.Parse(data[0]);
                int ID = int.Parse(data[1]);
                ChatId cChat = new ChatId(userId);
                await _botClient.SendTextMessageAsync(cChat, $"Ваш заказ с ID {ID} выполнен! Спасибо за покупку!", replyMarkup: inlineKeyboard);
                var item = TelegramBot.Context.Purchases.FirstOrDefault(v => v.ID == ID);
                item.State = 2;
                await TelegramBot.Context.SaveChangesAsync();
                return;
            }
            else if (route[0..4] == "remo")
            {
                var id = int.Parse(route[7..]);
                var purch = TelegramBot.Context.Purchases.FirstOrDefault(v => v.ID == id);
                if(purch == null) { await _botClient.SendTextMessageAsync(chat, "Заказ не найден"); }
                else
                {
                    if(purch.State == 0)
                    {
                        TelegramBot.Context.Purchases.Remove(purch);
                        await TelegramBot.Context.SaveChangesAsync();
                        await _botClient.SendTextMessageAsync(chat, $"Ваш заказ с ID {id} был удален!");
                    }
                    else if(purch.State == 1)
                    {
                        await _botClient.SendTextMessageAsync(chat, "Невозможно удалить заказ,так как он уже выполняется!");
                    }
                    else
                    {
                        await _botClient.SendTextMessageAsync(chat, "Невозможно удалить заказ,так как он уже выполнен!");
                    }
                }

            }
            else if (route[0..4] == "emer")
            {
                var data = route[5..].Split('|');
                var custID = long.Parse(data[0]);
                var purchID = int.Parse(data[1]);
                var purch = TelegramBot.Context.Purchases.FirstOrDefault(v => v.ID == purchID);
                if (purch != null) TelegramBot.Context.Purchases.Remove(purch);
                await TelegramBot.Context.SaveChangesAsync();
               await _botClient.SendTextMessageAsync(new ChatId(custID), $"Ваш заказ {purchID} отменен.\r\nПричина:Некорректный email\r\nЕсли вы считайте,что это произошло по ошибке,обратитесь в поддержку!");

            }
            else if (route[0..4] == "empa")
            {
                var data = route[5..].Split('|');
                var custID = long.Parse(data[0]);
                var purchID = int.Parse(data[1]);
                var purch = TelegramBot.Context.Purchases.FirstOrDefault(v => v.ID == purchID);
                if (purch != null) TelegramBot.Context.Purchases.Remove(purch);
                await TelegramBot.Context.SaveChangesAsync();
                await _botClient.SendTextMessageAsync(new ChatId(custID), $"Ваш заказ {purchID} отменен.\r\nПричина:Оплата не найдена!\r\nЕсли вы считайте,что это произошло по ошибке,обратитесь в поддержку!");


            }


            switch (route)
            {
                case "hello":
                    {
                        var replyKeyboard = new ReplyKeyboardMarkup(
                                  new List<KeyboardButton[]>()
                                  {
                                        new KeyboardButton[]
                                        {
                                            new KeyboardButton("На главную страницу"),
                                        },
                                        
                                  });
                        await _botClient.SendTextMessageAsync(chat, "Вас приветствует бот,который очень хочет сделать ваш процесс покупок легким и быстрым!", replyMarkup: replyKeyboard);

                        return;
                    }
               
                case "main":
                    {
                        _Bot.MessagesToDelete[chat].AddRange((await _botClient.SendMediaGroupAsync(chat, new List<IAlbumInputMedia>() { Resources.Resources.MainPict })).ToList());

                        inlineKeyboard = new InlineKeyboardMarkup(
                             new List<InlineKeyboardButton[]>()
                             {
                                 new InlineKeyboardButton[]
                                 {
                                    InlineKeyboardButton.WithCallbackData("Выбор игры","main/items"),
                                 },
                                  new InlineKeyboardButton[]
                                 {
                                    InlineKeyboardButton.WithCallbackData("Корзина","main/cart"),
                                 },
                                 new InlineKeyboardButton[]
                                 {
                                    InlineKeyboardButton.WithCallbackData("Мои заказы","main/history"),
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
                        _Bot.MessagesToDelete[chat].AddRange((await _botClient.SendMediaGroupAsync(chat, new List<IAlbumInputMedia>() { Resources.Resources.FeedBackPict })).ToList());

                        inlineKeyboard = new InlineKeyboardMarkup(
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

                case "main/paid":
                    {
                        decimal sum = 0;
                        StringBuilder _goods = new StringBuilder();
                        if (_Bot.Cart[chat].Count > 0)
                        {
                            foreach (var item in _Bot.Cart[chat])
                            {
                                _goods.Append(item.Category + ": " + item.Name + "\r\n");
                                sum += item.Price;
                            }
                        }

                        var stopEnterDataKeyboard = new InlineKeyboardMarkup(
                       new List<InlineKeyboardButton[]>()
                        {
                                new InlineKeyboardButton[]
                                 {
                                    InlineKeyboardButton.WithCallbackData("Отправить","main/new"),
                                 },
                       });

                        int id = (new Random()).Next();
                        while (TelegramBot.Context.Purchases.FirstOrDefault(v => v.ID == id) != null) id = (new Random()).Next();
                        await _botClient.SendTextMessageAsync(chat, $"\U0001f6d2 Заказ: {id}\r\n👤 Процесс: Отправление данных\r\n⏰ Время: {DateTime.Now.ToShortDateString()} {DateTime.Now.ToShortTimeString()}\r\n\r\nВведите email для игр\r\n\r\n После того как вы ввели всю необходимую информацию, нажмиет отправить",parseMode:ParseMode.Html,replyMarkup:stopEnterDataKeyboard);
                        Purchase purchase = new Purchase(id, chat.Identifier, chat.Username, _goods.ToString(), DateTime.Now.ToShortDateString() + " Time:" + DateTime.Now.TimeOfDay.ToString(), 0,string.Empty);
                        _Bot.ChatStates[chat] = ChatState.GetData;
                    
                        await TelegramBot.Context.Purchases.AddAsync(purchase);
                        await TelegramBot.Context.SaveChangesAsync();
                        _Bot.CurrentPurchase[chat] = id;
                       
                        //await _Bot.SetRoute("main", chat);
                        return;
                    }
                case "main/pay":
                    {
                        decimal sum = 0;
                        StringBuilder _goods = new StringBuilder();
                        if (_Bot.Cart[chat].Count > 0)
                        {
                            foreach (var item in _Bot.Cart[chat])
                            {
                                _goods.Append(item.Category + ": " + item.Name + "\r\n");
                                sum += item.Price;
                            }
                        }
                        
                       
                        var cancelPayKeyboard = new InlineKeyboardMarkup(
                          new List<InlineKeyboardButton[]>()
                           {

                                new InlineKeyboardButton[]
                                 {
                                    InlineKeyboardButton.WithCallbackData("Я оплатил","main/paid"),
                                 },
                                 new InlineKeyboardButton[]
                                 {
                                    InlineKeyboardButton.WithCallbackData("Отменить заказ","main"),
                                 },
                               
                               
                          });
                        int id = (new Random()).Next();
                        while (TelegramBot.Context.Purchases.FirstOrDefault(v => v.ID == id) != null) id = (new Random()).Next();
                        await _botClient.SendTextMessageAsync(chat, $"\U0001f6d2 Заказ: {id}\r\n💴 Процесс: Оплата\r\n🏦 Для оплаты Вы должны перевести {sum}Р по любому из данных реквизитов и <strong>!!! написать {id} в сообщениях к оплате!!!</strong>. После оплаты нажмите \"Я оплатил\".\r\n\r\n🎫 Реквизиты:\r\n\r\nТинькофф • +79939245527\r\nТинькофф • 2200700850594697\r\nСбер • 5336690284035310\r\n\r\n⚠️ Чтобы вы не потеряли вашу оплату, зафиксируйте данный перевод с номером заказа на скриншот/видео. \r\n\r\n🙋‍♂️ Если возникнут вопросы - обратитесь в поддержку: @firov1",replyMarkup:cancelPayKeyboard, parseMode: ParseMode.Html);
                        Purchase purchase = new Purchase(id,chat.Identifier,chat.Username, _goods.ToString(), DateTime.Now.ToShortDateString() + " Time:" + DateTime.Now.TimeOfDay.ToString(),0,string.Empty);
 
                     
                        return;
                    }
                case "main/history":
                    {
                        var purches = TelegramBot.Context.Purchases.Where(v => v.CustomerID == chat.Identifier).ToList();

                        if(purches.Count == 0)
                        {
                            //TODO: Еще ничего не покупал
                        }
                        else
                        {
                           StringBuilder stringBuilder = new StringBuilder();
                           foreach(var ph in purches) {
                                if (ph.State == 0)
                                {
                                    var cancelPayKeyboard = new InlineKeyboardMarkup(
                                     new List<InlineKeyboardButton[]>()
                                      {

                                            new InlineKeyboardButton[]
                                            {
                                                 InlineKeyboardButton.WithCallbackData("Отменить заказ",$"remove|{ph.ID}"),
                                            },
                                     });
                                    await _botClient.SendTextMessageAsync(chat, $"{ph.Date} {ph.GetStringState()} \r\n {ph.Goods}",replyMarkup:cancelPayKeyboard);
                                }

                                else
                                {
                                    await _botClient.SendTextMessageAsync(chat, $"{ph.Date} {ph.GetStringState()} \r\n {ph.Goods}");
                                }     
                           }
                          
                        }
                        return;
                    }
                case "main/cart":
                    {
                        _Bot.MessagesToDelete[chat].AddRange((await _botClient.SendMediaGroupAsync(chat, new List<IAlbumInputMedia>() { Resources.Resources.CartPict })).ToList());
                        StringBuilder answer = new StringBuilder();
                        if (!_Bot.Cart.ContainsKey(chat)) _Bot.Cart.Add(chat, new List<Item>());
                        decimal sum = 0;
                        answer.Append("<strong> Ваши товары:</strong>\r\n\r\n");
                        if (_Bot.Cart[chat].Count > 0)
                        {
                            foreach (var item in _Bot.Cart[chat])
                            {
                                answer.Append("\r\n");
                                answer.Append("\"" + item.Name + "\" - " + item.Price.ToString() + "₽");
                                sum += item.Price;
                            }
                            inlineKeyboard = new InlineKeyboardMarkup(
                           new List<InlineKeyboardButton[]>()
                            {

                                 new InlineKeyboardButton[]
                                 {
                                    InlineKeyboardButton.WithCallbackData("Оплатить","main/pay"),
                                    InlineKeyboardButton.WithCallbackData("Назад","main"),
                                 },
                                  new InlineKeyboardButton[]
                                 {
                                    InlineKeyboardButton.WithCallbackData("Очистить корзину","main/cart/clear"),

                                 },

                           });

                        }
                        else
                        {
                            answer.Append("<strong> Здесь пусто!!!</strong>");
                            inlineKeyboard = new InlineKeyboardMarkup(
                           new List<InlineKeyboardButton[]>()
                            {

                                 new InlineKeyboardButton[]
                                 {
                                    InlineKeyboardButton.WithCallbackData("Назад","main"),
                                 },
                               
                           });
                        }
                        answer.Append("\r\n\r\n");
                        answer.Append($"<strong>Итого: {sum}₽ </strong>") ;
                        

                        _botClient.SendTextMessageAsync(chat, answer.ToString(),replyMarkup:inlineKeyboard, parseMode: ParseMode.Html);

                        return;
                    }
                case "main/cart/clear":
                    {
                        await _Bot.ClearCart(chat);
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

                        _Bot.MessagesToDelete[chat].AddRange((await _botClient.SendMediaGroupAsync(chat, new List<IAlbumInputMedia>() { Resources.Resources.BrawlPict })).ToList());
                        inlineKeyboard = new InlineKeyboardMarkup(
                             new List<InlineKeyboardButton[]>()
                              {

                                 new InlineKeyboardButton[]
                                 {
                                    InlineKeyboardButton.WithCallbackData("30 гемов","buy/brgems30"),
                                    InlineKeyboardButton.WithCallbackData("80 гемов","buy/brgems80"),
                                    InlineKeyboardButton.WithCallbackData("170 гемов","buy/brgems170"),
                                 },

                                 new InlineKeyboardButton[]
                                 {
                                    InlineKeyboardButton.WithCallbackData("360 гемов","buy/brgems360"),
                                    InlineKeyboardButton.WithCallbackData("950 гемов","buy/brgems950"),
                                    InlineKeyboardButton.WithCallbackData("2000 гемов","buy/brgems2000"),
                                 },

                                 new InlineKeyboardButton[]
                                 {
                                    InlineKeyboardButton.WithCallbackData("Brawl Pass","buy/brpass"),
                                     InlineKeyboardButton.WithCallbackData("Brawl Pass +","buy/brpass+"),
                                    InlineKeyboardButton.WithCallbackData("Улучшения БП","buy/brupg"),

                                 },
                                 new InlineKeyboardButton[]
                                 {
                                    InlineKeyboardButton.WithCallbackData("Новый боец Лили","buy/brlili"),
                                 },
                                   new InlineKeyboardButton[]
                                 {
                                    InlineKeyboardButton.WithCallbackData("Назад","main/items"),
                                 },
                             });
                        await _botClient.SendTextMessageAsync(chat, "Товары доступные для Brawl Stars", replyMarkup: inlineKeyboard);
                        return;
                    }
                }
            
        }
    }
}
