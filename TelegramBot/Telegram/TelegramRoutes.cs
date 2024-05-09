using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using TelegramBot.Data;
using static System.Runtime.InteropServices.JavaScript.JSType;

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
                Utils.Log($"[{chat.Identifier}] buy {good}",ConsoleColor.DarkYellow);
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
                Utils.Log($"[{chat.Identifier}] cart {good}", ConsoleColor.DarkYellow);
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
                                   InlineKeyboardButton.WithCallbackData("Назад","main/itemsPrev"),
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
            
                await _botClient.SendTextMessageAsync(cChat, $"\U0001f6d2 Заказ: {purch.ID}\r\n👤 Статус: Отправление данных\r\n⏰ Время: {purch.Date} (МСК)\r\n🗳️ Категория: {purch.ToModel().GetCategories()[0]}\r\n🛍️ Товары:\r\n {Utils.GetGoodsString(purch.ToModel())}\r\n💰 Цена: {purch.Cost}₽\r\n\r\n🔔 Введите 6-значный код с почты\r\n\U0001f9fe Формат: 123456\r\n\r\n⚠️ Кода нет 5 минут? – Нажмите на кнопку👇",replyMarkup:inlineKeyboard);
                _Bot.ChatStates[cChat] = ChatState.GetCode;
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
                var purch = _Bot.Context.Purchases.FirstOrDefault(v => v.ID == ID);if (purch == null) return;
                ChatId cChat = new ChatId(userId);
                StringBuilder goods = new StringBuilder(); foreach (var good in purch.ToModel().Goods)
                {
                    goods.Append(Items.All.FirstOrDefault(v => v.Identifier == good).Name + "\r\n");
                }
                inlineKeyboard = new InlineKeyboardMarkup(
                           new List<InlineKeyboardButton[]>()
                           {
                                 new InlineKeyboardButton[]
                                 {
                                       InlineKeyboardButton.WithUrl("📝 Оставить отзыв","https://t.me/LancasterReviews"),
                                 },
                                  new InlineKeyboardButton[]
                                 {
                                       InlineKeyboardButton.WithCallbackData("🔙 Вернуться в меню","main"),
                                 },
                           });
                await _botClient.SendTextMessageAsync(cChat, $"\U0001f6d2 Заказ: {ID}\r\n👤 Статус: Выполнено ✅\r\n⏰ Время: {purch.Date}(МСК)\r\n🗳️ Категория: {purch.ToModel().GetCategories()[0]}\r\n🛍️ Товары:\r\n {goods.ToString()}💰 Цена: {purch.Cost}₽\r\n\r\n🔔 Проверяйте наличие доната на аккаунте и оставляйте отзыв по кнопке внизу👇\r\n\r\n📝 Оставить отзыв ", replyMarkup: inlineKeyboard);
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
                                      InlineKeyboardButton.WithUrl("Поддержка","https://t.me/lancaster_brawl"),
                                    InlineKeyboardButton.WithCallbackData("Главное меню",$"main"),
                                   
                                 }

                          });
                await _botClient.SendTextMessageAsync(ch, $"⛔️ Ваш заказ <u>{purch.ID}</u> задержан.\r\n\r\n<b>Причина:</b> Некорректный email.\r\n\r\nПожалуйста, введите почту ещё раз.\r\n\r\n⚠️ Если вы считаете, что это произошло по ошибке, обратитесь в поддержку, нажав на кнопку👇",replyMarkup:inlineKeyboard,parseMode:ParseMode.Html);
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
                await _botClient.SendTextMessageAsync(new ChatId(custID), $"⛔️Ваш заказ {purchID} задержан.\r\n<b>Причина:</b> Оплата не найдена!\r\nЕсли вы не оплатили - оплатите, следуя инструциям.\r\nЕсли вы считайте,что произошла ошибке,обратитесь в поддержку!",replyMarkup: inlineKeyboard,parseMode:ParseMode.Html);
            }
            else if (pref == "etag")
            {
                var data = route[5..].Split('|');
                var custID = long.Parse(data[0]);
                var purchID = int.Parse(data[1]);
                inlineKeyboard = new InlineKeyboardMarkup(
                          new List<InlineKeyboardButton[]>()
                          {
                                 new InlineKeyboardButton[]
                                 {
                                    InlineKeyboardButton.WithUrl("Поддержка","https://t.me/lancaster_brawl")
                                 }

                          });

                var purch = _Bot.Context.Purchases.FirstOrDefault(v => v.ID == purchID);
                await _botClient.SendTextMessageAsync(new ChatId(custID), $"⛔Пожалуйста,введите тэг еще раз!", replyMarkup: inlineKeyboard, parseMode: ParseMode.Html);
                _Bot.ChatStates[new ChatId(custID)] = ChatState.GetTag;
            }
            else if (pref == "emco")
            {
                var data = route[5..].Split('|');
                var custID = long.Parse(data[0]);
                var purchID = int.Parse(data[1]);
                var purch = _Bot.Context.Purchases.FirstOrDefault(v => v.ID == purchID);
                _Bot.ChatStates[new ChatId(custID)] = ChatState.GetCode;
                inlineKeyboard = new InlineKeyboardMarkup(
                          new List<InlineKeyboardButton[]>()
                          {
                                 new InlineKeyboardButton[]
                                 {
                                    InlineKeyboardButton.WithCallbackData("Код не пришел",$"emer|{purch.CustomerID}|{purch.ID}"),
                                 },
                          });
                await _botClient.SendTextMessageAsync(new ChatId(custID), $"⛔Пожалуйста,введите код еще раз!",replyMarkup:inlineKeyboard);
            }
            else if (pref == "dele")
            {
                var itemIdent = route[5..];
                
                _Bot.Cart[chat].Remove(_Bot.Cart[chat].FirstOrDefault(v => v.Identifier == itemIdent));
                _Bot.SetRoute("main/cart/change", chat);
            }

            else if(pref == "askq")
            {
                var data = route[5..].Split('|');
                var custID = long.Parse(data[0]);
                var purchID = int.Parse(data[1]);
                ReplyKeyboardMarkup markup = new ReplyKeyboardMarkup(
                    new List<List<KeyboardButton>>() {
                        new List<KeyboardButton>()
                        {
                            new KeyboardButton("Спросить название скина"),                      
                        },
                         new List<KeyboardButton>()
                        {
                         
                            new KeyboardButton("Спросить название оформления"),
                        },
                          new List<KeyboardButton>()
                        {
                          new KeyboardButton("Назад"),
                        }
                    }
                    );
                await _botClient.SendTextMessageAsync(chat, $"Введите любой вопрос,который хотите задать",replyMarkup:markup);
                _Bot.ChatStates[chat] = ChatState.AskData;
                _Bot.AskDataStates[chat] = Question.New(new ChatId(custID), string.Empty, purchID);
                
            }
            else if (pref == "kill")
            {
                var data = route[5..].Split('|');
                var custID = long.Parse(data[0]);
                var purchID = int.Parse(data[1]);

                var purch = _Bot.Context.Purchases.FirstOrDefault(v => v.ID == purchID);
                if(purch == null) { _botClient.SendTextMessageAsync(chat, "Заказ был уже удален!");return; }
                _Bot.Context.Purchases.Remove(purch);
                await _Bot.Context.SaveChangesAsync();
                await _botClient.SendTextMessageAsync(new ChatId(custID), $"❌ Ваш заказ [{purch.ID}] был отменён Менеджером.");

            }
            else if (pref == "game")
            {
                var data = route[5..].Split('|');
                var custID = long.Parse(data[0]);
                var purchID = int.Parse(data[1]);
                var purch = _Bot.Context.Purchases.FirstOrDefault(v => v.ID == purchID);
                Owner.OwnerAPI.ShowTask(chat, _botClient, _Bot, purch.ToModel());
                if (purch == null) { _botClient.SendTextMessageAsync(chat, "Заказ был удален!"); return; }
                StringBuilder goods = new StringBuilder(); foreach (var good in purch.ToModel().Goods)
                {
                    goods.Append(Items.All.FirstOrDefault(v => v.Identifier == good).Name + "\r\n");
                }
                await _botClient.SendTextMessageAsync(new ChatId(custID), $"\U0001f6d2 Заказ: {purch.ID}\r\n👤 Статус: Выполнение заказа\r\n⏰ Время: {purch.Date}(МСК)\r\n🗳️ Категория: {purch.ToModel().GetCategories()[0]}\r\n🛍️ Товар:\r\n{goods.ToString()}\r\n💰 Цена: {purch.Cost}₽\r\n\r\n🔔 Менеджер принялся выполнять ваш заказ. Пожалуйста, не заходите в игру до уведомления о завершении.");

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
                            "⚠️ Чтобы использовать бота, необходимо быть подписанным на наш магазин - @LancasterSquad", replyMarkup: inlineKeyboard);

                        return;
                    }

                case "checkSub":
                    {
                        var ID = chat.Identifier;
                        if (ID == null) return;
                        
                        var chatMember = await _botClient.GetChatMemberAsync(_Bot.GroupId, (long)ID);
                        Utils.Log($"[{chat.Identifier}] {chatMember.Status.ToString()}");

                        if (chatMember.Status == ChatMemberStatus.Member || chatMember.Status == ChatMemberStatus.Administrator || chatMember.Status == ChatMemberStatus.Creator)
                        {

                            //await _botClient.SendTextMessageAsync(chat, "Спасибо за подписку!\r\n Теперь можете пользоваться ботом!");
                            
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
                                    InlineKeyboardButton.WithUrl("Правила","https://telegra.ph/Lancaster-Shop-05-09"),     
                                    InlineKeyboardButton.WithUrl("Поддержка","https://t.me/lancaster_brawl"),


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

                            });;

                        await _botClient.SendPhotoAsync(chat, Resources.Resources.MainPict, caption: $"💾 Информация о нашем магазине:", replyMarkup: inlineKeyboard);
                        return;
                    }
                case "main/rules":
                    {

                        inlineKeyboard = new InlineKeyboardMarkup(
                              new List<InlineKeyboardButton[]>()
                              {
                                 new InlineKeyboardButton[]
                                 {
                                    InlineKeyboardButton.WithCallbackData("🔙 Вернуться назад","main/info"),

                                 },
                              });

                        await _botClient.SendTextMessageAsync(chat, "Политика использования \r\nЦель магазина: Магазин предоставляет услуги по продаже игровых донатов для улучшения игрового опыта в различных онлайн-играх.\r\n\r\nПравила использования: Пользователи обязаны соблюдать все применимые законы и правила платформ," +
                            " на которых они используют купленные донаты. Запрещены попытки обмана, мошенничество и другие недопустимые действия.\r\n\r\nПрием платежей: Мы принимаем платежи через указанные методы, обеспечивая безопасность и конфиденциальность ваших данных.\r\n\r\nОбязательства магазина: Магазин обязуется" +
                            " предоставить вам купленный игровой донат после успешной оплаты.\r\n\r\nОтветственность пользователя: Вы несете ответственность за предоставление правильной информации при заказе услуги. Пользователи должны предоставить корректные данные для успешного выполнения заказа.\r\n\r\nЗапрещенные действия:" +
                            " Запрещены действия, направленные на мошенничество, включая попытки возврата средств после получения услуги.\r\n\r\nПолитика возврата\r\nУсловия возврата: Вы можете запросить возврат средств, если полученные услуги были некачественными или не предоставлены в соответствии с условиями заказа.\r\n\r\n" +
                            "Процедура возврата: Для запроса возврата, свяжитесь с нашей службой поддержки по указанным контактным данным. Мы рассмотрим ваш запрос и произведем возврат средств на вашу карту/кошелек.\r\n\r\nСроки возврата: Мы постараемся рассмотреть ваш запрос в кратчайшие сроки.\r\n\r\nПолитика конфиденциальности" +
                            "\r\nСбор информации: Мы можем собирать определенную информацию от пользователей для обработки заказов и улучшения сервиса.\r\n\r\nИспользование информации: Мы обеспечиваем безопасное и конфиденциальное хранение ваших данных. Информация будет использована исключительно для обработки заказов и обратной связи " +
                            "с вами.\r\n\r\nРазглашение информации: Мы не раскроем вашу информацию третьим лицам, за исключением случаев, предусмотренных законом или в случаях, когда это необходимо для выполнения заказа (например, передача информации платежным системам).\r\n\r\nСогласие пользователя: Используя наши услуги, вы соглашаетесь" +
                            " с нашей политикой конфиденциальности.", replyMarkup: inlineKeyboard);
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
                case "main/history":
                    {
                        var purches = _Bot.Context.Purchases.Where(v => v.CustomerID == chat.Identifier).Select(v => new Purchase(v)).ToList();

                        if (purches.Count == 0)
                        {
                            var backKeyboard = new InlineKeyboardMarkup(
                       new List<InlineKeyboardButton[]>()
                        {

                                 new InlineKeyboardButton[]
                                 {
                                   InlineKeyboardButton.WithCallbackData("🔙 Вернуться в меню","main"),
                                 },

                       });
                            await _botClient.SendTextMessageAsync(chat, "На данный вы ещё не совершали покупок. Но вы всегда можете это исправить 😎", replyMarkup: backKeyboard);
                        }
                        else
                        {

                            foreach (var ph in purches)
                            {
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
                                                 InlineKeyboardButton.WithCallbackData("Отменить заказ",$"remo|{ph.ID}"),
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
                        await _botClient.SendPhotoAsync(chat, Resources.Resources.FeedBackPict, caption: "Можете оставить ваш отзыв здесь!", replyMarkup: inlineKeyboard);


                        return;
                    }


                case "main":
                    {
              

                        inlineKeyboard = new InlineKeyboardMarkup(
                             new List<InlineKeyboardButton[]>()
                             {
                                 new InlineKeyboardButton[]
                                 {
                                    InlineKeyboardButton.WithCallbackData("Каталог","main/itemsPrev"),
                                 },
                                  new InlineKeyboardButton[]
                                 {
                                    InlineKeyboardButton.WithCallbackData("Мой профиль","main/profile"),
                                    InlineKeyboardButton.WithCallbackData("Информация","main/info"),
                                 },
                               
                             });    

                        await _botClient.SendPhotoAsync(chat,Resources.Resources.MainPict, caption: "👋 Магазин Lancaster Shop к вашим услугам", replyMarkup: inlineKeyboard);
                        return;
                    }
                

                case "main/paid":
                    {
                        
                        ulong sum = 0;
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

                      
                        Purchase purchase = new Purchase(id,sum, chat.Identifier, chat.Username, _goods, $"{ moscowTime.ToShortDateString() } | { moscowTime.ToShortTimeString()}", 0,string.Empty,null);

                        if (purchase.GetCategories()[0] != "Telegram")
                        {
                            await _botClient.SendTextMessageAsync(chat, $"\U0001f6d2 Заказ: {id}\r\n👤 Статус: Отправление данных\r\n⏰ Время: {moscowTime.ToShortDateString()} | {moscowTime.ToShortTimeString()}(МСК)\r\n\r\n🔔 Введите вашу почту Supercell ID \r\n⚠️ Формат: email@gmail.com", parseMode: ParseMode.Html);
                            _Bot.ChatStates[chat] = ChatState.GetEmail;
                        }
                        else
                        {
                            await _botClient.SendTextMessageAsync(chat, $"\U0001f6d2 Заказ: {id}\r\n👤 Статус: Отправление данных\r\n⏰ Время: {moscowTime.ToShortDateString()} | {moscowTime.ToShortTimeString()}(МСК)\r\n\r\n🔔 Введите ваш тэг от Telegram \r\n⚠️ Формат: @YourTag", parseMode: ParseMode.Html);
                            _Bot.ChatStates[chat] = ChatState.GetTag;
                        }
                 
                        await _Bot.Context.Purchases.AddAsync(purchase.ToDataModel());
                        await _Bot.Context.SaveChangesAsync();
                        _Bot.CurrentPurchase[chat] = id;
                        _Bot.Cart[chat].Clear();
                        //await _Bot.SetRoute("main", chat);
                        return;
                    }
                case "main/pay":
                    {
                        ulong sum = 0;

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

                                answer.Append($"🗳️ Категория: <strong>{item.Category}</strong>\r\n📦 Товар: <strong>{item.Name}</strong>\r\n💰Стоимость: <strong>{item.Price}₽</strong>");
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
                                    InlineKeyboardButton.WithCallbackData("Назад","main/itemsPrev"),
                                   
                                 },

                           });

                        }
                        else
                        {
                            answer.Append("<strong> ❌ Корзина пуста! Загляни в каталог, чтобы выбрать нужный товар 🛍️</strong>");
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
                        answer.Append($"<strong>🧾 Итоговая цена: {sum}₽ </strong>") ;
                        

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






                case "main/itemsPrev":
                    {

                        inlineKeyboard = new InlineKeyboardMarkup(
                        new List<InlineKeyboardButton[]>()
                          {
                                 new InlineKeyboardButton[]
                                 {
                                    InlineKeyboardButton.WithCallbackData("💳 Подписки","main/subitems"),
                                    InlineKeyboardButton.WithCallbackData("💎 Supercell","main/items"),
                                 },
                                 new InlineKeyboardButton[]
                                 {
                                    InlineKeyboardButton.WithCallbackData("🔙 Вернуться в меню","main"),

                                 },
                          });
                        await _botClient.SendPhotoAsync(chat, Resources.Resources.ItemsPict, caption: "🗳️ Выберите категорию:", replyMarkup: inlineKeyboard);

                        return;
                    }
                case "main/subitems":
                    {
                       
                        inlineKeyboard = new InlineKeyboardMarkup(
                            new List<InlineKeyboardButton[]>()
                             {
                              
                                 new InlineKeyboardButton[]
                                 {
                                    InlineKeyboardButton.WithCallbackData("🔙 Вернуться в меню","main/itemsPrev"),
                                 },

                            });

                        await _botClient.SendTextMessageAsync(chat, "Будет совсем скоро!", replyMarkup: inlineKeyboard);

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

                                    InlineKeyboardButton.WithCallbackData("🔙 Вернуться в меню","main/itemsPrev"),
                                 },

                             });
                        await _botClient.SendPhotoAsync(chat,Resources.Resources.SupercellPict,caption: "📃 Категория: Supercell\r\n\r\nНезабываемые игры, в которые можно играть годами.", replyMarkup: inlineKeyboard);
                        return;
                    }          
                case "main/items/brawl":
                    {
                        var markup = Utils.GetMarkupForItems("Brawl Stars");
                        await _botClient.SendPhotoAsync(chat,Resources.Resources.BrawlPict,caption: "🛍️ Товары категории: Brawl Stars", replyMarkup: markup);
                        return;
                    }
                case "main/items/royale":
                    {
                        var markup = Utils.GetMarkupForItems("Clash Royale");
                        await _botClient.SendPhotoAsync(chat, Resources.Resources.ClashPict, caption: "🛍️ Товары категории: Clash Royale", replyMarkup: markup);
                        return;
                    }
                case "main/items/clans":
                    {

                        var markup = Utils.GetMarkupForItems("Clash of Clans");
                        await _botClient.SendPhotoAsync(chat, Resources.Resources.ClansPict, caption: "🛍️ Товары категории: Clash of Clans", replyMarkup:markup);
                        return;
                    }
            }
            
        }
    }
}
