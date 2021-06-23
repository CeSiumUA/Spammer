using Microsoft.Extensions.DependencyInjection;
using Quartz;
using Quartz.Spi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TLSpammer.WEB.Quartz
{
    public class SenderJobFactory : IJobFactory
    {
        private readonly IServiceProvider serviceProvider;
        
        public SenderJobFactory(IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider;
        }

        public IJob NewJob(TriggerFiredBundle bundle, IScheduler scheduler)
        {
            using (var scope = serviceProvider.CreateScope())
            {
                var jb = scope.ServiceProvider.GetRequiredService<SenderJob>();
                return jb;
            }
        }

        public void ReturnJob(IJob job)
        {
            var dispose = job as IDisposable;
            dispose?.Dispose();
        }
    }
}
