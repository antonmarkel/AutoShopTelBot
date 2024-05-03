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
            var pref = route[0..4];
            if (pref[0..3] == "buy")
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
            else if (pref == "cart")
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
            else if (pref == "warn")
            {
                //▶️
                var data = route[5..].Split('|');
                long userId = long.Parse(data[0]);
                int ID = int.Parse(data[1]);
                long ownerID = long.Parse(data[2]);
                var purch = _Bot.Context.Purchases.FirstOrDefault(v => v.ID == ID);
                if(purch == null) { await _botClient.SendTextMessageAsync(new ChatId(ownerID), "Заказ был прерван!");return; }
                ChatId cChat = new ChatId(userId);
                if (!_Bot.CurrentPurchase.ContainsKey(chat)) _Bot.CurrentPurchase.Add(chat, ID);
                _Bot.CurrentPurchase[chat] = ID;

                inlineKeyboard = new InlineKeyboardMarkup(
                            new List<InlineKeyboardButton[]>()
                            {
                                 new InlineKeyboardButton[]
                                 {
                                    InlineKeyboardButton.WithCallbackData("Код не пришел",$"emer|{purch.CustomerID}|{purch.ID}"),
                                 },
                            });

                await _botClient.SendTextMessageAsync(cChat, $"Заказ {ID}.Пожалуйста, введите код присланый вам на email.Если код не пришел в течение 5 минут,нажмите \"Код не пришел\"",replyMarkup:inlineKeyboard);
                _Bot.ChatStates[chat] = ChatState.GetCode;
                var item = _Bot.Context.Purchases.FirstOrDefault(v => v.ID == ID);
                item.Data += "\r\n";
                item.State = 1;
                await _Bot.Context.SaveChangesAsync();
                return;
            }
            else if (pref == "done")
            {
                //▶️
                var data = route[5..].Split('|');
                long userId = long.Parse(data[0]);
                int ID = int.Parse(data[1]);
                ChatId cChat = new ChatId(userId);
                await _botClient.SendTextMessageAsync(cChat, $"Ваш заказ с ID {ID} выполнен! Спасибо за покупку!", replyMarkup: inlineKeyboard);
                var item = _Bot.Context.Purchases.FirstOrDefault(v => v.ID == ID);
                item.State = 2;
                await _Bot.Context.SaveChangesAsync();
                return;
            }
            else if (pref == "remo")
            {
                var id = int.Parse(route[5..]);
                var purch = _Bot.Context.Purchases.FirstOrDefault(v => v.ID == id);
                if(purch == null) { await _botClient.SendTextMessageAsync(chat, "Заказ не найден"); }
                else
                {
                    if(purch.State == 0)
                    {
                        _Bot.Context.Purchases.Remove(purch);
                        await _Bot.Context.SaveChangesAsync();
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
            else if (pref == "emer")
            {
                var data = route[5..].Split('|');
                var custID = long.Parse(data[0]);
                var purchID = int.Parse(data[1]);
                var purch = _Bot.Context.Purchases.FirstOrDefault(v => v.ID == purchID);
                purch.State = 0;
                //if (purch != null) TelegramBot.Context.Purchases.Remove(purch);
                
                purch.Data += "\r\n---- новая инфа ---- \r\n";
                await _Bot.Context.SaveChangesAsync();
                var ch = new ChatId(custID);
                

                inlineKeyboard = new InlineKeyboardMarkup(
                          new List<InlineKeyboardButton[]>()
                          {
                                 new InlineKeyboardButton[]
                                 {
                                    InlineKeyboardButton.WithCallbackData("Главное меню",$"main"),
                                    InlineKeyboardButton.WithUrl("Поддержка","https://t.me/lancaster_brawl")
                                 }

                          });
                await _botClient.SendTextMessageAsync(ch, $"⛔️Ваш заказ {purchID} задержан.\r\nПричина:Некорректный email.\r\nПожалуйста,введи еще раз.\r\nЕсли вы считайте,что это произошло по ошибке,обратитесь в поддержку!",replyMarkup:inlineKeyboard);
                _Bot.ChatStates[ch] = ChatState.GetEmail;
            }
            else if (pref == "empa")
            {
                var data = route[5..].Split('|');
                var custID = long.Parse(data[0]);
                var purchID = int.Parse(data[1]);
                inlineKeyboard = new InlineKeyboardMarkup(
                          new List<InlineKeyboardButton[]>()
                          {
                                 new InlineKeyboardButton[]
                                 {
                                    InlineKeyboardButton.WithCallbackData("Главное меню",$"main"),
                                    InlineKeyboardButton.WithUrl("Поддержка","https://t.me/lancaster_brawl")
                                 }

                          });

                var purch = _Bot.Context.Purchases.FirstOrDefault(v => v.ID == purchID);
                await _botClient.SendTextMessageAsync(new ChatId(custID), $"⛔️Ваш заказ {purchID} задержан.\r\nПричина:Оплата не найдена!\r\nЕсли вы не оплатили - оплатите, следуя инструциям.\r\nЕсли вы считайте,что произошла ошибке,обратитесь в поддержку!",replyMarkup: inlineKeyboard);
            }
            else if (pref == "emco")
            {
                var data = route[5..].Split('|');
                var custID = long.Parse(data[0]);
                var purchID = int.Parse(data[1]);
                var purch = _Bot.Context.Purchases.FirstOrDefault(v => v.ID == purchID);
                _Bot.ChatStates[chat] = ChatState.GetCode;
                await _botClient.SendTextMessageAsync(new ChatId(custID), $"Пришлите код, высланный вам на email");
            }
            else if (pref == "dele")
            {
                var itemIdent = route[5..];
                
                _Bot.Cart[chat].Remove(_Bot.Cart[chat].FirstOrDefault(v => v.Identifier == itemIdent));
                _Bot.SetRoute("main/cart/change", chat);
               
            }
            else if (pref == "docu")
            {
                var data = route[5..].Split('|');
                long userId = long.Parse(data[0]);
                int ID = int.Parse(data[1]);
                long ownerID = long.Parse(data[2]);
                var purch = _Bot.Context.Purchases.FirstOrDefault(v => v.ID == ID);

                ChatId cChat = new ChatId(userId);
                inlineKeyboard = new InlineKeyboardMarkup(
                           new List<InlineKeyboardButton[]>()
                           {
                                 new InlineKeyboardButton[]
                                 {
                                    InlineKeyboardButton.WithCallbackData("Отменить заказ",$"remo|{purch.ID}"),
                                 }

                           });
                _botClient.SendTextMessageAsync(cChat, $"Заказ: {purch.ID}\r\n🔔 Мы не нашли оплаты с вашим ID, поэтому менеджер запросил у вас доказательства перевода денежных средств (чек/скриншот/история) \r\n\r\n📃 Пришлите пожалуйста скриншот/файл с доказательством,либо нажмите на кнопку \"Отменить заказ\"",replyMarkup:inlineKeyboard) ;
                _Bot.CurrentPurchase[cChat] = ID;
                _Bot.ChatStates[cChat] = ChatState.GetDoc;
            }
            else if(pref == "undo")
            {
                int purchID = int.Parse(route[5..].Split('|')[0]);
                _Bot.Context.Purchases.FirstOrDefault(v => v.ID == purchID).State = 1;
                await _Bot.Context.SaveChangesAsync();
            }
            else if(pref == "show")
            {
                int purchID = int.Parse(route[5..].Split('|')[0]);
                var purch = _Bot.Context.Purchases.FirstOrDefault(v => v.ID == purchID);
                if(purch == null)
                {
                    await _botClient.SendTextMessageAsync(chat, "Заказ не найден!");
                }
                else
                {
                    Owner.OwnerAPI.ShowTask(chat, _botClient, _Bot, new Purchase(purch));
                }
              
            }

            switch (route)
            {
                case "hello":
                    {
                        inlineKeyboard = new InlineKeyboardMarkup(
                            new List<InlineKeyboardButton[]>()
                            {
                                 new InlineKeyboardButton[]
                                 {
                                    InlineKeyboardButton.WithCallbackData("Проверить подписку","checkSub"),
                                 },
                            });
                        await _botClient.SendTextMessageAsync(chat, "Вас приветствует бот,который очень хочет сделать ваш процесс покупок легким и быстрым!\r\n" +
                            "⚠️ Чтобы использовать бота, необходимо быть подписанным на наш магазин - @LanDonate", replyMarkup: inlineKeyboard);

                        return;
                    }

                case "checkSub":
                    {
                        var ID = chat.Identifier;
                        if (ID == null) return;
                        
                        var chatMember = await _botClient.GetChatMemberAsync(_Bot.GroupId, (long)ID);
                        Console.WriteLine(chatMember.Status.ToString() );
                        if (chatMember.Status == ChatMemberStatus.Member || chatMember.Status == ChatMemberStatus.Administrator || chatMember.Status == ChatMemberStatus.Creator)
                        {

                           // await _botClient.SendTextMessageAsync(chat, "Спасибо за подписку!\r\n Теперь можете пользоваться ботом");
                            await _Bot.SetRoute("main", chat);
                        }
                        else
                        {
                            
                            await _Bot.SetRoute("hello", chat);
                        }
                        return;
                    }

                case "main/info":
                    {
                        inlineKeyboard = new InlineKeyboardMarkup(
                            new List<InlineKeyboardButton[]>()
                            {
                                 new InlineKeyboardButton[]
                                 {
                                    InlineKeyboardButton.WithCallbackData("Правила","main/items"),
                                      InlineKeyboardButton.WithUrl("Отзывы","https://t.me/LancasterReviews"),
                                      InlineKeyboardButton.WithCallbackData("FAQ","main"),
                                    InlineKeyboardButton.WithUrl("Поддержка","https://t.me/lancaster_brawl")
                                 },
                                  new InlineKeyboardButton[]
                                 {
                                    InlineKeyboardButton.WithUrl("Магазин","https://t.me/LanDonate"),
                                      InlineKeyboardButton.WithUrl("Отзывы","https://t.me/LancasterReviews"),
                                       InlineKeyboardButton.WithUrl("Чат","https://t.me/LancasterChat"),
                                 },
                                 new InlineKeyboardButton[]
                                 {
                                    InlineKeyboardButton.WithCallbackData("🔙 Вернуться в меню","main"),
                                 },

                            });

                        await _botClient.SendPhotoAsync(chat, Resources.Resources.MainPict, caption: $"Здесь вы можете ознакомится с нашим магазином чуточку ближе", replyMarkup: inlineKeyboard);
                        return;
                    }
                case "main/profile":
                    {

                        inlineKeyboard = new InlineKeyboardMarkup(
                              new List<InlineKeyboardButton[]>()
                              {
                                 new InlineKeyboardButton[]
                                 {
                                    InlineKeyboardButton.WithCallbackData("Каталог","main/items"),
                                    InlineKeyboardButton.WithCallbackData("Корзина","main/cart"),
                                     InlineKeyboardButton.WithCallbackData("Мои заказы","main/history"),
                                 },
                                 new InlineKeyboardButton[]
                                 {
                                    InlineKeyboardButton.WithCallbackData("🔙 Вернуться в меню","main"),

                                 },
                              });

                        await _botClient.SendPhotoAsync(chat, Resources.Resources.MainPict, caption: $"👤ID пользователя: {chat.Identifier}\r\n💰Баланс: 0Р\r\n🛒Заказов всего: {_Bot.Context.Purchases.Where(v => v.CustomerID == chat.Identifier).ToList().Count}", replyMarkup: inlineKeyboard);
                        return;
                    }

                case "main":
                    {
              

                        inlineKeyboard = new InlineKeyboardMarkup(
                             new List<InlineKeyboardButton[]>()
                             {
                                 new InlineKeyboardButton[]
                                 {
                                    InlineKeyboardButton.WithCallbackData("Каталог","main/items"),
                                 },
                                  new InlineKeyboardButton[]
                                 {
                                    InlineKeyboardButton.WithCallbackData("Мой профиль","main/profile"),
                                    InlineKeyboardButton.WithCallbackData("Информация","main/info"),
                                 },
                               
                             });    

                        await _botClient.SendPhotoAsync(chat,Resources.Resources.MainPict, caption: "Приветствуем вас! Здесь вы сможете ознакомиться с товарами", replyMarkup: inlineKeyboard);
                        return;
                    }
                case "main/feedback":
                    {
                       
                        inlineKeyboard = new InlineKeyboardMarkup(
                            new List<InlineKeyboardButton[]>()
                            { 
                                 new InlineKeyboardButton[]
                                 {
                                    InlineKeyboardButton.WithCallbackData("Назад","main"),
                                 },

                            });
                        await _botClient.SendPhotoAsync(chat,Resources.Resources.FeedBackPict, caption: "Можете оставить ваш отзыв здесь!", replyMarkup: inlineKeyboard);


                        return;
                    }

                case "main/paid":
                    {
                        
                        decimal sum = 0;
                        List<string> _goods = new List<string>();
                        if (_Bot.Cart[chat].Count > 0)
                        {
                            foreach (var item in _Bot.Cart[chat])
                            {
                                _goods.Add(item.Identifier);
                                sum += item.Price;
                            }
                        }
                       
                        int id = (new Random()).Next();
                        while (_Bot.Context.Purchases.FirstOrDefault(v => v.ID == id) != null) id = (new Random()).Next();
                        TimeZoneInfo moscowTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Russian Standard Time");
                        DateTime moscowTime = DateTime.UtcNow + moscowTimeZone.BaseUtcOffset;

                        await _botClient.SendTextMessageAsync(chat, $"\U0001f6d2 Заказ: {id}\r\n👤 Статус: Отправление данных\r\n⏰ Время: {moscowTime.ToShortDateString()} | {moscowTime.ToShortTimeString()}(МСК)\r\n\r\n🔔 Введите вашу почту Supercell ID \r\n⚠️ Формат: email@gmail.com",parseMode:ParseMode.Html);
                        
                        Purchase purchase = new Purchase(id,sum, chat.Identifier, chat.Username, _goods, $"{ moscowTime.ToShortDateString() } | { moscowTime.ToShortTimeString()}", 0,string.Empty,null);
                        _Bot.ChatStates[chat] = ChatState.GetEmail;
                    
                        await _Bot.Context.Purchases.AddAsync(purchase.ToDataModel());
                        await _Bot.Context.SaveChangesAsync();
                        _Bot.CurrentPurchase[chat] = id;
                        _Bot.Cart[chat].Clear();
                        //await _Bot.SetRoute("main", chat);
                        return;
                    }
                case "main/pay":
                    {
                        decimal sum = 0;

                        List<string> _goods = new List<string>();
                        if (_Bot.Cart[chat].Count > 0)
                        {
                            foreach (var item in _Bot.Cart[chat])
                            {
                                _goods.Add(item.Identifier);
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
                        while (_Bot.Context.Purchases.FirstOrDefault(v => v.ID == id) != null) id = (new Random()).Next();
                        Purchase purchase = new Purchase(id, sum, chat.Identifier, chat.Username, _goods, DateTime.Now.ToShortDateString() + " Time:" + DateTime.Now.TimeOfDay.ToString(), 0, string.Empty,null);
                        if(purchase.GetCategories().Count > 1)
                        {
                           var backKeyboard = new InlineKeyboardMarkup(
                          new List<InlineKeyboardButton[]>()
                           {

                                 new InlineKeyboardButton[]
                                 {
                                    InlineKeyboardButton.WithCallbackData("В корзину","main/cart"),
                                 },

                          });
                            await _botClient.SendTextMessageAsync(chat, "Извините, на данный момент обработка заказов содержащих товары для разных игр не осуществляется.Приносим свои извинения.Пожалуйста,оставьте в корзине товары только для одной игры. Это можно сделать нажав кнопку \"Изменить\" в меню корзины",replyMarkup:backKeyboard);
                        }
                        else
                        {//.Replace("-", "\\-").Replace(".", "\\.").Replace("+", "\\+")
                            await _botClient.SendTextMessageAsync(chat, $"\U0001f6d2 ID заказа: {id}\r\n💴 Статус: Оплата\r\n💰 Цена: {sum}₽\r\n🏦 Для оплаты заказа нужно перевести {sum}₽ по любому из данных реквизитов и в комментариях к оплате написать ID вашего заказа -  [<code>{id}</code>]\r\nПосле оплаты нажмите на \"Я оплатил\".\r\n\r\n┏━━━━━━━━━━━━━━━━━━━━━┓\r\n┃ Тинькофф • <code>+79939245527</code>\r\n┃ Тинькофф • <code>2200700850594697</code>\r\n┃ СберБанк • <code>5336690284035310</code>\r\n┗━━━━━━━━━━━━━━━━━━━━━┛\r\n\r\n⚠️ Если вы нажмёте кнопку \"Я оплатил\" не оплатив заказ – заказ будет отменён менеджером.\r\n\r\n🙋‍♂️ Если возникнут вопросы с оплатой - обратитесь в поддержку: @lancaster_brawl", replyMarkup: cancelPayKeyboard,parseMode:ParseMode.Html);
                            //await _botClient.SendTextMessageAsync(chat, $"\U0001f6d2 ID заказа: {id}\r\n💴 Статус: Оплата\r\n💰 Цена: {sum}₽\r\n🏦 Для оплаты заказа нужно перевести {sum}₽ по любому из данных реквизитов и в комментариях к оплате написать ID вашего заказа \\-  [`793925754`]\r\nПосле оплаты нажмите на \"Я оплатил\"\\.\r\n\r\n┏━━━━━━━━━━━━━━━━━━━━━┓\r\n┃ Тинькофф • \\+79939245527\r\n┃ Тинькофф • 2200700850594697\r\n┃ СберБанк • 5336690284035310\r\n┗━━━━━━━━━━━━━━━━━━━━━┛\r\n\r\n⚠️ Если вы нажмёте кнопку \"Я оплатил\" не оплатив заказ \\– заказ будет отменён менеджером\\.\r\n\r\n🙋‍♂️ Если возникнут вопросы с оплатой \\- обратитесь в поддержку: @lancaster_brawl", replyMarkup: inlineKeyboard, parseMode: ParseMode.MarkdownV2);
                            //await _botClient.SendTextMessageAsync(chat, $"\U0001f6d2 Заказ: {id}\r\n💴 Процесс: Оплата\r\n🏦 Для оплаты Вы должны перевести {sum}Р по любому из данных реквизитов и <strong>\r\n\r\n!!!написать {id} в сообщениях к оплате!!! \r\n\r\n</strong>. После оплаты нажмите \"Я оплатил\".\r\n\r\n🎫 Реквизиты:\r\n\r\nТинькофф • +79939245527\r\nТинькофф • 2200700850594697\r\nСбер • 5336690284035310\r\n\r\n⚠️ Чтобы вы не потеряли вашу оплату, зафиксируйте данный перевод с номером заказа на скриншот/видео. \r\n\r\n🙋‍♂️ Если возникнут вопросы - обратитесь в поддержку: @firov1", replyMarkup: cancelPayKeyboard, parseMode: ParseMode.Html);
                        }
                            
                        return;
                    }
                case "main/history":
                    {
                        var purches = _Bot.Context.Purchases.Where(v => v.CustomerID == chat.Identifier).Select(v => new Purchase(v)).ToList();

                        if (purches.Count == 0)
                        {
                            await _botClient.SendTextMessageAsync(chat, "На данный вы ещё не совершали покупок. Но вы всегда можете это исправить 😎", replyMarkup: inlineKeyboard);
                        }
                        else
                        {
                         
                            foreach (var ph in purches) {
                                StringBuilder goodsString = new StringBuilder();
                                for (int j = 0; j < ph.Goods.Count; j++)
                                {
                                    var item = Items.All.FirstOrDefault(v => v.Identifier == ph.Goods[j]);
                                    goodsString.Append($"{j + 1}){item.Category} : {item.Name}\r\n");
                                }
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
                                    string state = string.Empty;
                                    switch (ph.State)
                                    {
                                        case 0:
                                            state = "Ожидание продавца";
                                            break;
                                        case 1:
                                            state = "Ожидание продавца";
                                            break;
                                        case 2:
                                            state = "Выполнен";
                                            break;
                                    }
                                    
                                    await _botClient.SendTextMessageAsync(chat, $"{ph.Date}\r\nСостояние: {state}{ph.GetStringState()} \r\n {goodsString}", replyMarkup: cancelPayKeyboard);
                                }

                                else
                                {
                                    await _botClient.SendTextMessageAsync(chat, $"{ph.Date} {ph.GetStringState()} \r\n{goodsString}");
                                }
                            }
                          var exitKeyboard = new InlineKeyboardMarkup(
                          new List<InlineKeyboardButton[]>()
                           {

                              
                                 new InlineKeyboardButton[]
                                 {
                                    InlineKeyboardButton.WithCallbackData("Назад","main"),
                                 },


                          });

                            await _botClient.SendTextMessageAsync(chat, "История ваших заказов", replyMarkup: exitKeyboard);
                          
                        }
                        return;
                    }
                case "main/cart":
                    {
                        
                        StringBuilder answer = new StringBuilder();
                        if (!_Bot.Cart.ContainsKey(chat)) _Bot.Cart.Add(chat, new List<Item>());
                        decimal sum = 0;
                        answer.Append("<strong>🛒 Ваши товары:</strong>\r\n\r\n");
                        if (_Bot.Cart[chat].Count > 0)
                        {
                            answer.Append("\r\n----------------------------\r\n");
                            foreach (var item in _Bot.Cart[chat])
                            {

                                answer.Append($"Категория🧾:     <strong>{item.Category}</strong>\r\nНазвание товара📦:     <strong>{item.Name}</strong>\r\nЦена товара💴:     <strong>{item.Price}₽</strong>");
                                answer.Append("\r\n----------------------------\r\n");
                                sum += item.Price;
                            }
                            inlineKeyboard = new InlineKeyboardMarkup(
                           new List<InlineKeyboardButton[]>()
                            {

                                 new InlineKeyboardButton[]
                                 {
                                    InlineKeyboardButton.WithCallbackData("Оплатить","main/pay"),
                                     InlineKeyboardButton.WithCallbackData("Изменить","main/cart/change"),

                                 },
                                  new InlineKeyboardButton[]
                                 {
                                      InlineKeyboardButton.WithCallbackData("Очистить корзину","main/cart/clear"),
                                    InlineKeyboardButton.WithCallbackData("Назад","main/items"),
                                   
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
                                    InlineKeyboardButton.WithCallbackData("Назад","main/items"),
                                 },
                               
                           });
                        }
                        answer.Append("\r\n\r\n");
                        answer.Append($"<strong>Итого: {sum}₽ </strong>") ;
                        

                        _botClient.SendPhotoAsync(chat,Resources.Resources.CartPict, caption:answer.ToString(),replyMarkup:inlineKeyboard, parseMode: ParseMode.Html);

                        return;
                    }
                case "main/cart/clear":
                    {
                        await _Bot.ClearCart(chat);
                        return;
                    }
                case "main/cart/change":
                    {
                        List<InlineKeyboardButton[]> buttons = new List<InlineKeyboardButton[]>(); 
                        foreach(var item in _Bot.Cart[chat])
                        {
                            buttons.Add(new InlineKeyboardButton[] { InlineKeyboardButton.WithCallbackData($"{item.Category}: {item.Name}",$"dele|{item.Identifier}") });
                        }
                        buttons.Add(new InlineKeyboardButton[] { InlineKeyboardButton.WithCallbackData("Назад", "main/cart") });
                        inlineKeyboard = new InlineKeyboardMarkup(buttons);
                        await _botClient.SendTextMessageAsync(chat, "Выберите товар,который вы бы хотели убрать из корзины",replyMarkup:inlineKeyboard);
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

                                    InlineKeyboardButton.WithCallbackData("🔙 Вернуться в меню","main"),
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
                        await _botClient.SendPhotoAsync(chat,Resources.Resources.BrawlPict,caption: "Товары доступные для Brawl Stars", replyMarkup: inlineKeyboard);
                        return;
                    }
                case "main/items/royale":
                    {
                        inlineKeyboard = new InlineKeyboardMarkup(
                            new List<InlineKeyboardButton[]>()
                              {

                                 new InlineKeyboardButton[]
                                 {
                                    InlineKeyboardButton.WithCallbackData("80 гемов","buy/clgems80"),
                                    InlineKeyboardButton.WithCallbackData("500 гемов","buy/clgems500"),
                                    InlineKeyboardButton.WithCallbackData("1200 гемов","buy/clgems1200"),
                                 },

                                 new InlineKeyboardButton[]
                                 {
                                    InlineKeyboardButton.WithCallbackData("2500 гемов","buy/clgems2500"),
                                    InlineKeyboardButton.WithCallbackData("6500 гемов","buy/clgems6500"),
                                    InlineKeyboardButton.WithCallbackData("14000 гемов","buy/clgems14000"),
                                 },

                                 new InlineKeyboardButton[]
                                 {
                                    InlineKeyboardButton.WithCallbackData("Золотой пропуск","buy/clgoldpass"),
                                     InlineKeyboardButton.WithCallbackData("Алмазный пропуск","buy/clironpass"),

                                 },
                                   new InlineKeyboardButton[]
                                 {
                                    InlineKeyboardButton.WithCallbackData("Назад","main/items"),
                                 },
                             });
                        await _botClient.SendPhotoAsync(chat, Resources.Resources.ClashPict, caption: "Товары доступные для Clash Royale", replyMarkup: inlineKeyboard);
                        return;
                    }
                case "main/items/clans":
                    {
                        inlineKeyboard = new InlineKeyboardMarkup(
                            new List<InlineKeyboardButton[]>()
                              {

                                 new InlineKeyboardButton[]
                                 {
                                    InlineKeyboardButton.WithCallbackData("80 гемов","buy/clcgems80"),
                                    InlineKeyboardButton.WithCallbackData("500 гемов","buy/clcgems500"),
                                    InlineKeyboardButton.WithCallbackData("1200 гемов","buy/clcgems1200"),
                                 },

                                 new InlineKeyboardButton[]
                                 {
                                    InlineKeyboardButton.WithCallbackData("2500 гемов","buy/clcgems2500"),
                                    InlineKeyboardButton.WithCallbackData("6500 гемов","buy/clcgems6500"),
                                    InlineKeyboardButton.WithCallbackData("14000 гемов","buy/clcgems14000"),
                                 },

                                 new InlineKeyboardButton[]
                                 {
                                    InlineKeyboardButton.WithCallbackData("Золотой пропуск","buy/clcgoldpass"),
                                     InlineKeyboardButton.WithCallbackData("Пропуск события","buy/clcpass"),

                                 },
                                   new InlineKeyboardButton[]
                                 {
                                    InlineKeyboardButton.WithCallbackData("Назад","main/items"),
                                 },
                             });
                        await _botClient.SendPhotoAsync(chat, Resources.Resources.ClansPict, caption: "Товары доступные для Clash of Clans", replyMarkup: inlineKeyboard);
                        return;
                    }
            }
            
        }
    }
}
