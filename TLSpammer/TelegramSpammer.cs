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
        public TelegramSpammer(int appId, string appHash, string login)
        {
            this.login = login;
            this.telegramClient = new TelegramClient(appId, appHash);
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
            try
            {
                TLVector<TLAbsChat> chats = new TLVector<TLAbsChat>();
                try
                {
                    var dialogs = (TLDialogsSlice)await telegramClient.GetUserDialogsAsync();
                    chats = dialogs.Chats;
                }
                catch
                {
                    var dialogs = (TLDialogs)await telegramClient.GetUserDialogsAsync();
                    chats = dialogs.Chats;
                }
                finally
                {
                    foreach (var chat in chats)
                    {
                        if (chat is TLChannel)
                        {
                            
                        }
                    }
                }
            }
            catch (Exception exc)
            {

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
