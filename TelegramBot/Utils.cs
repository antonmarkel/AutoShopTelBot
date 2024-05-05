using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace TelegramBot
{
    public class Utils
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
        public static void Log(string message,ConsoleColor color = ConsoleColor.Black) {
            TimeZoneInfo moscowTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Russian Standard Time");
            DateTime moscowTime = DateTime.UtcNow + moscowTimeZone.BaseUtcOffset;
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write($"[{moscowTime.ToShortTimeString()}]");
            if (color == ConsoleColor.Black) Console.ResetColor();
            else Console.ForegroundColor = color;
            Console.WriteLine(message);

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
    }
}

