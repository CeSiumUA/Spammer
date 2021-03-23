using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TLSharp.Core;

namespace TLSpammer
{
    public class Program
    {
        static async Task Main(string[] args)
        {
            //1630560
            //4cea02b478b137866d6b72c660cd9280
            TelegramSpammer tlSpammer = new TelegramSpammer(1630560, "4cea02b478b137866d6b72c660cd9280", "+380983860743", new List<string>(){"TestGroup"}, "Test");
            string codeRequestResult = await tlSpammer.InitTelegramAsync();
            if (!string.IsNullOrEmpty(codeRequestResult))
            {
                Console.WriteLine("Отправлен код аутентификации, его необходимо ввести сюда:");
                string authCode = Console.ReadLine();
                var loginResult = await tlSpammer.LoginWithCodeAsync(authCode, codeRequestResult);
                if (!loginResult)
                {
                    Console.WriteLine("Неправильный код, либо пользователь имеет Cloud Password! При наличии Cloud Password, введите его сюда:");
                    var cloudPassword = Console.ReadLine();
                    await tlSpammer.LoginWithPasswordAsync(cloudPassword);
                }
            }

            await tlSpammer.StartSpammerAsync();
        }
    }
}
