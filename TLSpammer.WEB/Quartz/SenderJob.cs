using Microsoft.Extensions.DependencyInjection;
using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TLSpammer.WEB.Services;

namespace TLSpammer.WEB.Quartz
{
    public class SenderJob : IJob
    {
        private TelegramService _telegramService;
        public SenderJob(TelegramService telegramService)
        {
            this._telegramService = telegramService;
        }
        public async Task Execute(IJobExecutionContext context)
        {
            await _telegramService.SendNotificationsToChannelsAsync();
        }
    }
}
