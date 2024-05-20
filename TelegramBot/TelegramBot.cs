using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Xsl;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using TelegramBot.Commands;
using TelegramBot.DataContext;
using TelegramBot.User;
using static TelegramBot.StateComputing;

namespace TelegramBot
{
    public class TelegramBot
    {

        string tPict = "AgACAgIAAxkBAAIdAWZLgAAB6y8VeCC09b3i3pOuW3NjGAACrN0xG9u9YUoxP4b2wWqnPQEAAwIAA3MAAzUE";


        private ITelegramBotClient _botClient;
        private ReceiverOptions _receiverOptions;
        public List<InputFile> Pictures { get; set; } = new List<InputFile> { };
        BotContext Context { get; set; }
        public TelegramBot(string token)
        {
            _botClient = new TelegramBotClient(token);
            _receiverOptions = new ReceiverOptions
            {
                AllowedUpdates = new[]
                {
                    UpdateType.Message,
                    UpdateType.CallbackQuery,
                    UpdateType.InlineQuery,

                },
                ThrowPendingUpdates = true
            };
            Context = new BotContext();
        }

        public async Task Start()
        {
            using var cts = new CancellationTokenSource();
            _botClient.StartReceiving(UpdateHandler, ErrorHanlder, _receiverOptions, cts.Token);
            Console.WriteLine("Bot started");
            await Task.Delay(-1);
        }

        async Task<UserData> EnsureChatExist(long chatId)
        {
            var _userData = await Context.Users.FirstOrDefaultAsync(v => v.ChatId == chatId);
            if(_userData == null)
            {
               await Context.Users.AddAsync(new User.UserData(chatId, (short)ChatStates.Standard,-1));
                await Context.SaveChangesAsync();
               _userData = await Context.Users.FirstOrDefaultAsync(v => v.ChatId == chatId);
            }
            return _userData!;
        }

        async Task UpdateHandler(ITelegramBotClient botClient, Update update, CancellationToken cancelTok)
        {

            try
            {
                switch (update.Type)
                { 
                    case UpdateType.Message:
                        {
                            string updateMessage = string.Empty;
                            if (update.Message.Text != null) updateMessage = update.Message.Text;
                            else updateMessage = update.Message.Caption!;

                            var chat = update.Message.Chat;
                            var _userData = await EnsureChatExist(chat.Id);

                            try { await _botClient.DeleteMessageAsync(chat, _userData.CurrentMessageId);
                                await _botClient.DeleteMessageAsync(chat, update.Message.MessageId);
                            }
                            catch { }

                            _userData.CurrentMessageId = (await _botClient.SendPhotoAsync(chat,InputFile.FromFileId(tPict),caption:"...")).MessageId;
                            if(_userData.ChatState != ChatStates.Standard)
                            {
                                await ComputeState(_userData, _botClient, Context, update,chat);
                                await Context.SaveChangesAsync();
                                return;
                            }
                            if (update.Message.Photo != null)
                            {

                                Console.WriteLine(update.Message.Photo[0].FileId);

                            }

                            if (updateMessage == "/start")
                            {  
                                await TelegramRender.RenderPage("page/main", _botClient, chat, _userData.CurrentMessageId);
                            }
                            if (updateMessage == "/admin")
                            {
                                await TelegramRender.RenderPage("admin/main", _botClient,chat, _userData.CurrentMessageId);
                            }

                            await Context.SaveChangesAsync();
                            return;
                        }
                    case UpdateType.CallbackQuery:
                        {
                            var _userData = await EnsureChatExist(update.CallbackQuery.Message.Chat.Id);

                            if (_userData.ChatState != ChatStates.Standard)
                            {
                                await ComputeState(_userData, _botClient, Context, update, update.CallbackQuery.Message.Chat);
                                await Context.SaveChangesAsync();
                                return;
                            }


                            var pref = update.CallbackQuery.Data.Split('/')[0];
                            if (pref == "func") await TelegramFunc.RenderFunc(update.CallbackQuery.Data,Context,_botClient,update.CallbackQuery.Message,_userData);
                            else await TelegramRender.RenderPage(update.CallbackQuery.Data, _botClient,update.CallbackQuery.Message.Chat, update.CallbackQuery.Message.MessageId);
                            await Context.SaveChangesAsync();
                            return;
                        }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        Task ErrorHanlder(ITelegramBotClient botClient, Exception error, CancellationToken cancelToken)
        {
            var ErrorMessage = error switch
            {
                ApiRequestException apiRequestException => $"Telegram bot API error:\n[{apiRequestException.ErrorCode} {apiRequestException.Message}",
                _ => error.ToString()
            };
            Console.WriteLine(ErrorMessage);
            return Task.CompletedTask;
        }
    }
}
