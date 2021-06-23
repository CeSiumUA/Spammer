using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TeleSharp.TL;
using TeleSharp.TL.Messages;
using TLSharp.Core;
using TLSpammer.WEB.Data;
using TLSpammer.WEB.Shared;

namespace TLSpammer.WEB.Services
{
    public class TelegramService
    {
        private TelegramClient telegramClient;
        public string Login { get; set; }
        public string LastPhoneCodeHash;
        public List<CheckedChat> SelectedChats { get; set; }
        public TimeOption TimeOption
        {
            get
            {
                using (var scope = serviceProvider.CreateScope())
                {
                    var dbContextService = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                    return dbContextService.Times.FirstOrDefault();
                }
            }
            set
            {
                using (var scope = serviceProvider.CreateScope())
                {
                    var dbContextService = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                    dbContextService.Times.Update(value);
                    dbContextService.SaveChanges();
                }
            }
        }
        private TLUser user;
        private IServiceProvider serviceProvider;
        public TelegramService(IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider;
            this.telegramClient = new TelegramClient(1630560, "4cea02b478b137866d6b72c660cd9280");
            using (var scope = serviceProvider.CreateScope())
            {
                var dbContextService = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                SelectedChats = dbContextService.SelectedChats.ToList();
            }
        }

        public async Task<bool> InitTelegramAsync(string login)
        {
            await telegramClient.ConnectAsync();
            if (!string.IsNullOrEmpty(login))
            {
                //if (!telegramClient.IsConnected)
                //{
                    
                //}

                this.LastPhoneCodeHash = await CheckForCodeRequest(login);
                return LastPhoneCodeHash != null;
            }
            else
            {
                return false;
            }
        }
        private async Task<string> CheckForCodeRequest(string login)
        {
            if (!telegramClient.IsUserAuthorized() || login != this.Login)
            {
                return await this.telegramClient.SendCodeRequestAsync(login);
            }

            this.Login = login;
            return null;
        }
        public async Task<bool> LoginWithCodeAsync(string code)
        {
            try
            {
                this.user = await telegramClient.MakeAuthAsync(this.Login, this.LastPhoneCodeHash, code);
                this.LastPhoneCodeHash = String.Empty;
                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }
        public void UpdateTime(DateTime time)
        {
            var timeOption = this.TimeOption;
            timeOption.Time = time;
            using (var scope = serviceProvider.CreateScope())
            {
                var dbContextService = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                dbContextService.Update(timeOption);
                dbContextService.SaveChanges();
            }
        }
        public async Task LoginWithPasswordAsync(string code)
        {
            this.user = await telegramClient.MakeAuthWithPasswordAsync(await telegramClient.GetPasswordSetting(), code);
        }

        public async Task GetUserChats()
        {
            if (telegramClient.IsUserAuthorized() && telegramClient.IsConnected)
            {
                List<SelectionChat> chats = new List<SelectionChat>();
                TLVector<TLAbsChat> parsedchats = new TLVector<TLAbsChat>();
                try
                {
                    var dialogs = (TLDialogsSlice) await telegramClient.GetUserDialogsAsync();
                    parsedchats = dialogs.Chats;
                }
                catch
                {
                    var dialogs = (TLDialogs) await telegramClient.GetUserDialogsAsync();
                    parsedchats = dialogs.Chats;
                }
                finally
                {
                    foreach (var chat in parsedchats)
                    {
                        if (chat is TLChannel)
                        {
                            var channel = chat as TLChannel;
                            if (!chats.Exists(x => x.Id == channel.Id))
                            {
                                chats.Add(new SelectionChat()
                                {
                                    Id = channel.Id,
                                    Chat = chat,
                                    Name = channel.Title
                                });
                            }
                        }
                        if (chat is TLChannelForbidden)
                        {
                            var channelForbidden = chat as TLChannelForbidden;
                            if (!chats.Exists(x => x.Id == channelForbidden.Id))
                            {
                                chats.Add(new SelectionChat()
                                {
                                    Id = channelForbidden.Id,
                                    Chat = chat,
                                    Name = channelForbidden.Title
                                });
                            }
                        }
                        if (chat is TLChat)
                        {
                            var tlChat = chat as TLChat;
                            if (!chats.Exists(x => x.Id == tlChat.Id))
                            {
                                chats.Add(new SelectionChat()
                                {
                                    Id = tlChat.Id,
                                    Chat = chat,
                                    Name = tlChat.Title
                                });
                            }
                        }
                    }
                }
                using (var scope = serviceProvider.CreateScope())
                {
                    var dbContextService = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                    foreach (var cht in chats)
                    {
                        if (!SelectedChats.Exists(x => x.Id == cht.Id))
                        {
                            SelectedChats.Add(cht);
                            dbContextService.SelectedChats.Add(cht);
                        }
                    }
                    await dbContextService.SaveChangesAsync();
                }
            }
        }
        public async Task SaveChangesAsync(ChangeEventArgs args, CheckedChat checkedChat)
        {
            this.SelectedChats.FirstOrDefault(x => x.Id == checkedChat.Id).IsSelected = (bool) args.Value;
            using (var scope = serviceProvider.CreateScope())
            {
                var dbContextService = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                dbContextService.UpdateRange(this.SelectedChats);
                await dbContextService.SaveChangesAsync();
            }
        }
    }
}
