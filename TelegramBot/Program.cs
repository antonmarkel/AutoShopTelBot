// See https://aka.ms/new-console-template for more information
using Telegram.Bot.Types;
using TelegramBot.Telegram;

internal class Program
{

 

    private static void Main(string[] args)
    {
        Console.WriteLine("Hello, World!");
        TelegramPage.Add(new TelegramRoute("main"), "Тестовое поле", new List<IAlbumInputMedia>() { new InputMediaPhoto(InputFile.FromStream(new FileStream("Resources/test.jpg", FileMode.Open))) }, new List<TelegramButton[]>() { new TelegramButton[] { new TelegramButton("Это пиздец блять", new TelegramRoute("main"), false) } });

        TelegramBot.Telegram.TelegramBot bot = new TelegramBot.Telegram.TelegramBot("7106270577:AAHNhXB5NSa6BZkTSOjQX_BsPZShneyqvU8");
        bot.Start().Wait();
    }
}