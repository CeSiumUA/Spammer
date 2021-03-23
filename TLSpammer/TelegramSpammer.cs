using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TeleSharp.TL;
using TeleSharp.TL.Messages;
using TLSharp.Core;

namespace TLSpammer
{
    public class TelegramSpammer
    {
        private readonly TelegramClient telegramClient;
        private readonly string login;
        private TLUser user;
        private readonly IEnumerable<string> listOfChats;
        private readonly string textMessage;
        private List<TLAbsChat> chats;
        public TelegramSpammer(int appId, string appHash, string login, IEnumerable<string> listOfChats, string TextMessage)
        {
            this.login = login;
            this.telegramClient = new TelegramClient(appId, appHash);
            this.listOfChats = listOfChats;
            this.textMessage = TextMessage;
        }

        public async Task<string> InitTelegramAsync()
        {
            await telegramClient.ConnectAsync();
            if (!telegramClient.IsUserAuthorized())
            {
                return await telegramClient.SendCodeRequestAsync(this.login);
            }

            return null;
        }

        public async Task StartSpammerAsync()
        {
            chats = new List<TLAbsChat>();
            try
            {
                TLVector<TLAbsChat> parsedchats = new TLVector<TLAbsChat>();
                try
                {
                    var dialogs = (TLDialogsSlice)await telegramClient.GetUserDialogsAsync();
                    parsedchats = dialogs.Chats;
                }
                catch
                {
                    var dialogs = (TLDialogs)await telegramClient.GetUserDialogsAsync();
                    parsedchats = dialogs.Chats;
                }
                finally
                {
                    foreach (var chat in parsedchats)
                    {
                        if (chat is TLChannel)
                        {
                            if (listOfChats.Contains((chat as TLChannel).Title))
                            {
                                chats.Add(chat);
                            }
                        }

                        if (chat is TLChannelForbidden)
                        {
                            if (listOfChats.Contains((chat as TLChannelForbidden).Title))
                            {
                                chats.Add(chat);
                            }
                        }
                        if (chat is TLChat)
                        {
                            if (listOfChats.Contains((chat as TLChat).Title))
                            {
                                chats.Add(chat);
                            }
                        }
                    }
                }
            }
            catch (Exception exc)
            {

            }

            await SendMessagesInChatAsync();
        }

        private async Task SendMessagesInChatAsync()
        {
            foreach (var selectedChat in chats)
            {
                TLAbsInputPeer messageTarget = null;
                if (selectedChat is TLChat)
                {
                    messageTarget = new TLInputPeerChat() {ChatId = (selectedChat as TLChat).Id};
                }
                if (selectedChat is TLChannel)
                {
                    var schid = (selectedChat as TLChannel);
                    if (schid.AccessHash.HasValue)
                    {
                        messageTarget = new TLInputPeerChannel()
                            {AccessHash = schid.AccessHash.Value, ChannelId = schid.Id};
                    }
                }

                if (messageTarget != null)
                {
                    await telegramClient.SendMessageAsync(messageTarget, textMessage);
                }
            }
        }
        public async Task<bool> LoginWithCodeAsync(string code, string hash)
        {
            try
            {
                this.user = await telegramClient.MakeAuthAsync(this.login, hash, code);
                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }
        public async Task LoginWithPasswordAsync(string code)
        {
            this.user = await telegramClient.MakeAuthWithPasswordAsync(await telegramClient.GetPasswordSetting(), code);
        }
    }
}
