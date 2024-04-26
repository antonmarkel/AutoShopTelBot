using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot.Types;

namespace TelegramBot.Resources
{
    public static class Resources
    {
        public static readonly InputMediaPhoto GemsPict = new InputMediaPhoto(InputFile.FromFileId("AgACAgIAAxkBAAIB9GYrkjSa884MWlV7hCGaXsyjYs93AAJR3TEbpl5gScYQksDiZAbkAQADAgADcwADNAQ"));

        static Resources()
        {
            
        }
    }
}
