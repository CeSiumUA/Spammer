using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Quartz;
using Quartz.Impl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TeleSharp.TL;
using TeleSharp.TL.Messages;
using TLSharp.Core;
using TLSpammer.WEB.Data;
using TLSpammer.WEB.Quartz;
using TLSpammer.WEB.Shared;

namespace TLSpammer.WEB.Services
{
    public class TelegramService
    {
        private TelegramClient telegramClient;
        public string Login { get; set; }
        public string LastPhoneCodeHash;
        private List<SelectionChat> selectionChats { get; set; }
        private bool _senderStarted = false;
        public List<CheckedChat> SelectedChats
        {
            get
            {
                using (var scope = serviceProvider.CreateScope())
                {
                    var dbContextService = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                    return dbContextService.SelectedChats.ToList();
                }
            }
        }
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
        public TextData TextData
        {
            get
            {
                using (var scope = serviceProvider.CreateScope())
                {
                    var dbContextService = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                    return dbContextService.TextDatas.FirstOrDefault();
                }
            }
            set
            {
                using (var scope = serviceProvider.CreateScope())
                {
                    var dbContextService = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                    dbContextService.TextDatas.Update(value);
                    dbContextService.SaveChanges();
                }
            }
        }
        private TLUser user;
        private IServiceProvider serviceProvider;
        private IScheduler scheduler;
        public TelegramService(IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider;
            this.telegramClient = new TelegramClient(1630560, "4cea02b478b137866d6b72c660cd9280");
            using (var scope = serviceProvider.CreateScope())
            {
                var dbContextService = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            }
            this.scheduler = StdSchedulerFactory.GetDefaultScheduler().Result;
            scheduler.JobFactory = new SenderJobFactory(serviceProvider);
            scheduler.Start().Wait();
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
        private async Task StartScheduler()
        {
            if (!_senderStarted)
            {
                IJobDetail job = JobBuilder.Create<SenderJob>().Build();
                var todayTimeOption = (new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, this.TimeOption.Time.Hour, this.TimeOption.Time.Minute, this.TimeOption.Time.Second)).ToUniversalTime();
                if(DateTime.Now.ToUniversalTime() > todayTimeOption)
                {
                    todayTimeOption = todayTimeOption.AddDays(1);
                }
                ITrigger trigger = TriggerBuilder.Create()
                   .WithIdentity("messages_sender")
                   .StartAt(todayTimeOption)
                   .WithSimpleSchedule(x => x.WithIntervalInSeconds(24).RepeatForever()).Build();
                await scheduler.ScheduleJob(job, trigger);
                _senderStarted = true;
            }
        }
        private async Task ResetScheduler()
        {
            await this.scheduler.UnscheduleJob(new TriggerKey("messages_sender"));
            _senderStarted = false;
            await StartScheduler();
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
            ResetScheduler().Wait();
        }
        public void UpdateTextData(string text)
        {
            var textData = this.TextData;
            textData.Text = text;
            using (var scope = serviceProvider.CreateScope())
            {
                var dbContextService = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                dbContextService.Update(textData);
                dbContextService.SaveChanges();
            }
        }
        public async Task LoginWithPasswordAsync(string code)
        {
            this.user = await telegramClient.MakeAuthWithPasswordAsync(await telegramClient.GetPasswordSetting(), code);
        }
        public async Task SendNotificationsToChannelsAsync()
        {
            var selectedChannels = this.SelectedChats.Where(x => x.IsSelected);
            foreach(var channel in selectedChannels)
            {
                if(channel.Input == ReceiverType.Channel)
                {
                    await this.telegramClient.SendMessageAsync(new TLInputPeerChannel()
                    {
                        ChannelId = channel.Id,
                        AccessHash = selectionChats.FirstOrDefault(x => x.Id == channel.Id).AccessHash
                    }, this.TextData.Text);
                }
                if(channel.Input == ReceiverType.Chat)
                {
                    await this.telegramClient.SendMessageAsync(new TLInputPeerChat()
                    {
                        ChatId = channel.Id
                    }, this.TextData.Text);
                }
            }
        }
        public async Task GetUserChats()
        {
            bool isUserAuthorized = telegramClient.IsUserAuthorized();
            if (isUserAuthorized && telegramClient.IsConnected)
            {
                selectionChats = new List<SelectionChat>();
                TLVector<TLAbsChat> parsedchats = new TLVector<TLAbsChat>();
                try
                {
                    var dialogs = (TLDialogsSlice) await telegramClient.GetUserDialogsAsync(limit: Int32.MaxValue);
                    parsedchats = dialogs.Chats;
                }
                catch
                {
                    var dialogs = (TLDialogs) await telegramClient.GetUserDialogsAsync(limit: Int32.MaxValue);
                    parsedchats = dialogs.Chats;
                }
                finally
                {
                    foreach (var chat in parsedchats)
                    {
                        if (chat is TLChannel)
                        {
                            var channel = chat as TLChannel;
                            if (!selectionChats.Exists(x => x.Id == channel.Id))
                            {
                                selectionChats.Add(new SelectionChat()
                                {
                                    Id = channel.Id,
                                    Chat = chat,
                                    Name = channel.Title,
                                    AccessHash = channel.AccessHash.HasValue ? channel.AccessHash.Value : 0,
                                    Input = ReceiverType.Channel
                                });
                            }
                        }
                        if (chat is TLChannelForbidden)
                        {
                            var channelForbidden = chat as TLChannelForbidden;
                            if (!selectionChats.Exists(x => x.Id == channelForbidden.Id))
                            {
                                selectionChats.Add(new SelectionChat()
                                {
                                    Id = channelForbidden.Id,
                                    Chat = chat,
                                    Name = channelForbidden.Title,
                                    AccessHash = channelForbidden.AccessHash,
                                    Input = ReceiverType.Channel
                                });
                            }
                        }
                        if (chat is TLChatForbidden)
                        {
                            var chatForbidden = chat as TLChatForbidden;
                            if (!selectionChats.Exists(x => x.Id == chatForbidden.Id))
                            {
                                selectionChats.Add(new SelectionChat()
                                {
                                    Id = chatForbidden.Id,
                                    Chat = chat,
                                    Name = chatForbidden.Title,
                                    Input = ReceiverType.Chat
                                });
                            }
                        }
                        if (chat is TLChat)
                        {
                            var tlChat = chat as TLChat;
                            if (!selectionChats.Exists(x => x.Id == tlChat.Id))
                            {
                                selectionChats.Add(new SelectionChat()
                                {
                                    Id = tlChat.Id,
                                    Chat = chat,
                                    Name = tlChat.Title,
                                    Input = ReceiverType.Chat
                                });
                            }
                        }
                    }
                }
                using (var scope = serviceProvider.CreateScope())
                {
                    var dbContextService = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                    var selectedChats = SelectedChats;
                    foreach (var cht in selectionChats)
                    {
                        if (!selectedChats.Exists(x => x.Id == cht.Id))
                        {
                            selectedChats.Add(cht);
                            dbContextService.SelectedChats.Add(cht);
                        }
                    }
                    await dbContextService.SaveChangesAsync();
                }
            }
            await StartScheduler();
        }
        public async Task SaveChangesAsync(ChangeEventArgs args, CheckedChat checkedChat)
        {
            var chatToUpdate = this.SelectedChats.FirstOrDefault(x => x.Id == checkedChat.Id);
            chatToUpdate.IsSelected = (bool) args.Value;
            using (var scope = serviceProvider.CreateScope())
            {
                var dbContextService = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                dbContextService.Update(chatToUpdate);
                await dbContextService.SaveChangesAsync();
            }
        }
    }
}
