// See https://aka.ms/new-console-template for more information
using Telegram.Bot.Types;
using TelegramBot.Telegram;

internal class Program
{

    private static void Main(string[] args)
    {
        Console.WriteLine("Hello, World!");
        TelegramBot.Telegram.TelegramBot bot = new TelegramBot.Telegram.TelegramBot("7106270577:AAHNhXB5NSa6BZkTSOjQX_BsPZShneyqvU8");
        bot.Start().Wait();
    }
}