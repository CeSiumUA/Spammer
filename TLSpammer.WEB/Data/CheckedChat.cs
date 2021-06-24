using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TLSpammer.WEB.Data
{
    public class CheckedChat
    {
        public string Name { get; set; }
        public int Id { get; set; }
        public bool IsSelected { get; set; }
        public long AccessHash { get; set; }
        public ReceiverType Input { get; set; }
    }
    public enum ReceiverType
    {
        Channel = 0,
        Chat = 1,
        ChatForbidden = 2,
        ChannelForbidden = 3
    }
}
