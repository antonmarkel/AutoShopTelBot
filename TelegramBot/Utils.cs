using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using TelegramBot.Data;
using TelegramBot.TelegramAPI;

namespace TelegramBot
{
    public static class Utils
    {
        public static bool IsValidCode(string code)
        {
            string validSim = "0123456789";
            if (code.Length != 6) return false;
            for (int i = 0; i < code.Length; i++)
            {
                if (!validSim.Contains(code[i])) return false;
            }
            return true;
        }
        public static async Task Log(string message,ConsoleColor color = ConsoleColor.Black) {



            Console.ResetColor();
            TimeZoneInfo moscowTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Russian Standard Time");
            DateTime moscowTime = DateTime.UtcNow + moscowTimeZone.BaseUtcOffset;
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write($"[{moscowTime.ToShortTimeString()}]");
            if (color == ConsoleColor.Black) Console.ResetColor();
            else Console.ForegroundColor = color;
            Console.WriteLine(message);


            using (var writer = new StreamWriter("Logs.txt",true))
            {
              await  writer.WriteLineAsync($"[{moscowTime.ToShortTimeString()}]" + "[" + DateTime.Now.ToShortDateString() + "]" + message);
  
            }




        }

        public static InlineKeyboardMarkup GetMarkupForItems(string category,int maxLineLength = 34,int maxItemsPerLine = 3,string backRoute = "main/items",bool isHot = false)
        {
            List<List<InlineKeyboardButton>> autoInline = new List<List<InlineKeyboardButton>>();
            List<InlineKeyboardButton> curLineButtons = new List<InlineKeyboardButton>();

            int _maxLineLen = maxLineLength, _curLineLen = 0;
            int _maxItemsPerLine = maxItemsPerLine, _curItemsNum = 0;
            var items = isHot ? Items.All.Where(v => v.Category == category && v.IsSpecialOffer) : Items.All.Where(v => v.Category == category && !v.IsSpecialOffer).ToList();

            foreach (var item in items)
            {
                _curLineLen += item.Name.Length; _curItemsNum++;
                if (_curLineLen > _maxLineLen || _curItemsNum > _maxItemsPerLine)
                {
                    autoInline.Add(curLineButtons);
                    curLineButtons = new List<InlineKeyboardButton>();
                    _curLineLen = item.Name.Length;
                    _curItemsNum = 1;
                }
                curLineButtons.Add(InlineKeyboardButton.WithCallbackData(item.Name, "buy/" + item.Identifier));
            }
            if(curLineButtons.Count > 0) { autoInline.Add(curLineButtons); }
            if(!isHot && Items.All.FirstOrDefault(v => v.Category == category &&  v.IsSpecialOffer) != null)
            {
                autoInline.Add(new List<InlineKeyboardButton>() { InlineKeyboardButton.WithCallbackData("🔥 Акции 🔥", $"hots|{category}") });
            }
            autoInline.Add(new List<InlineKeyboardButton>() { InlineKeyboardButton.WithCallbackData("🔙 Назад", backRoute) });
            return new InlineKeyboardMarkup(autoInline);
        }

        public static string GetRouteByCategory(string category)
        {
            string backRoute = "main/itemsPrev";
            switch (category)
            {
                case "Brawl Stars":
                    backRoute = "main/items/brawl";
                    break;
                case "Clash Royale":
                    backRoute = "main/items/royale";
                    break;
                case "Clash of Clans":
                    backRoute = "main/items/clans";
                    break;
            }
            return backRoute;
        }

        public static async Task SendMessage(this ITelegramBotClient _botClient, ChatId chatId,string text, ParseMode? parseMode = null, IReplyMarkup? replyMarkup = null)
        {
            TelegramAPI.TelegramBot.CurrentBot.PreviousMessages.Add(await _botClient.SendTextMessageAsync(chatId, text, parseMode: parseMode,replyMarkup:replyMarkup));
        }
        public static async Task SendPhoto(this ITelegramBotClient _botClient, ChatId chatId, InputFile photo, string? caption = null, ParseMode? parseMode = null, IReplyMarkup? replyMarkup = null)
        {
            TelegramAPI.TelegramBot.CurrentBot.PreviousMessages.Add(await _botClient.SendPhotoAsync(chatId, photo, caption:caption, parseMode: parseMode, replyMarkup: replyMarkup));
        }
        public static bool IsValidTelegramTag(string tag)
        {
            if (tag.Length > 0 && tag[0] == '@') return true;
            return false;
        }

        public static bool IsValidEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return false;

            try
            {
                // Нормализация домена, если он содержит интернациональные символы.
                email = Regex.Replace(email, @"(@)(.+)$", DomainMapper, RegexOptions.None, TimeSpan.FromMilliseconds(200));

                // Проверка, соответствует ли электронная почта регулярному выражению
                return Regex.IsMatch(email,
                    @"^[^@\s]+@[^@\s]+\.[^@\s]+$",
                    RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(250));
            }
            catch (RegexMatchTimeoutException)
            {
                return false;
            }
            catch (ArgumentException)
            {
                return false;
            }
        }

        static string DomainMapper(Match match)
        {
            var idn = new System.Globalization.IdnMapping();

            string domainName = match.Groups[2].Value;
            try
            {
                domainName = idn.GetAscii(domainName);
            }
            catch (ArgumentException)
            {
                throw new ArgumentException("Invalid domain name.", "email");
            }
            return match.Groups[1].Value + domainName;
        }

        public static string GetGoodsString(Purchase purch)
        {
            StringBuilder goods = new StringBuilder(); foreach (var good in purch.Goods)
            {
                goods.Append("• "+ Items.All.FirstOrDefault(v => v.Identifier == good).Name + "\r\n");
            }
            return goods.ToString();
        }
    }
}

