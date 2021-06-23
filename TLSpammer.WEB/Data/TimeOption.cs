using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace TLSpammer.WEB.Data
{
    public class TimeOption
    {
        [Key]
        public int Id { get; set; }
        public DateTime Time { get; set; }
    }
}
