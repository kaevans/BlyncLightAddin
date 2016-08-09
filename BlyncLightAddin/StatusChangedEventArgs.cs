using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlyncLightAddin
{
    public enum Status
    {
        Default,
        Busy,
        Available        
    }

    public class StatusChangedEventArgs : EventArgs
    {
        public Status Status { get; set; }
        public StatusChangedEventArgs(Status status)
        {
            Status = status;
        }
    }
}
