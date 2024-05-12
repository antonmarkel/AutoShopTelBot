// See https://aka.ms/new-console-template for more information
using Telegram.Bot.Types;
using TelegramBot.TelegramAPI;

internal class Program
{

    private static void Main(string[] args)
    {
        Console.WriteLine("Hello, World!");
        TelegramBot.TelegramAPI.TelegramBot bot = new TelegramBot.TelegramAPI.TelegramBot("6417717622:AAFvbn6ME0yJeXLCtctSNTYwddaykAMCAZU");
        TelegramBot.TelegramAPI.TelegramBot.CurrentBot = bot;
        bot.Start().Wait();

    }
}