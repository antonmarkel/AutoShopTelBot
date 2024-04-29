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
        public static readonly InputMediaPhoto FeedBackPict = new InputMediaPhoto(InputFile.FromFileId("AgACAgIAAxkBAAIGYWYtCUKAlwVmXcBhivNkZ16inaR-AAKz3TEbQjtpSfNswwUkGnixAQADAgADcwADNAQ"));
        public static readonly InputMediaPhoto MainPict = new InputMediaPhoto(InputFile.FromFileId("AgACAgIAAxkBAAIGY2YtCZpffYWpFnBh1cD18Rx1XoksAAK83TEbQjtpSabiKlW9ksqCAQADAgADcwADNAQ"));
        public static readonly InputMediaPhoto AdminPict = new InputMediaPhoto(InputFile.FromFileId("AgACAgIAAxkBAAIGZGYtCbVR2y58az06SjF-K5PbQaD6AAK-3TEbQjtpSUj50Veko18PAQADAgADcwADNAQ"));
        public static readonly InputMediaPhoto CartPict = new InputMediaPhoto(InputFile.FromFileId("AgACAgIAAxkBAAIGZWYtCdg7a99LS-vN5ejCV8zDiJ-9AALB3TEbQjtpSb60j92Mc4muAQADAgADcwADNAQ"));
        public static readonly InputMediaPhoto BrawlPict = new InputMediaPhoto(InputFile.FromFileId("AgACAgIAAxkBAAIGZmYtCfhgjw-iiKXxxxwavrRVwKZ_AALD3TEbQjtpSY-AyMV1v2-SAQADAgADcwADNAQ"));
        static Resources()
        {
            
        }
    }
}
