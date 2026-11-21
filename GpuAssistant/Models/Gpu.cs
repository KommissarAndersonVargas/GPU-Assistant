using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GpuAssistant.Models
{
    public class Gpu
    {
        public string Name { get; set; } = "";
        public int Quantity { get; set; }
        public int MemoryGB { get; set; }
        public int InUse { get; set; }
        public int Available => Quantity - InUse;
    }
}
