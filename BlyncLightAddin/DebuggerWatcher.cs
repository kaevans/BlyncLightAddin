using Microsoft.VisualStudio.Shell.Interop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlyncLightAddin
{
    public class DebuggerWatcher : IBlyncWatcher, IVsDebuggerEvents
    {
        public event EventHandler StatusChanged;
        private uint cookie = default(uint);
        private bool isPaused;


        public void Initialize()
        {
            IVsDebugger debugService = Microsoft.VisualStudio.Shell.Package.GetGlobalService(typeof(SVsShellDebugger)) as IVsDebugger;
            if (debugService != null)
            {
                // Register for debug events.
                // Assumes the current class implements IDebugEventCallback2.

                debugService.AdviseDebuggerEvents(this, out cookie);
            }
        }

        public int OnModeChange(DBGMODE dbgmodeNew)
        {
            switch (dbgmodeNew)
            {
                case DBGMODE.DBGMODE_Break:
                case DBGMODE.DBGMODE_Run:
                case DBGMODE.DBGMODE_Enc:
                case DBGMODE.DBGMODE_EncMask: 
                    //Debugging                   
                    OnStatusChanged(new StatusChangedEventArgs(Status.Busy));
                    break;
                case DBGMODE.DBGMODE_Design:
                    //Debugger detached
                    OnStatusChanged(new StatusChangedEventArgs(Status.Available));
                    break;
                default:
                    OnStatusChanged(new StatusChangedEventArgs(Status.Default));
                    break;
            }
            return (int)dbgmodeNew;
        }

        protected virtual void OnStatusChanged(StatusChangedEventArgs e)
        {
            if (StatusChanged != null && isPaused == false)
            {
                StatusChanged(this, e);
            }
        }
        public void Close()
        {
            IVsDebugger debugService = Microsoft.VisualStudio.Shell.Package.GetGlobalService(typeof(SVsShellDebugger)) as IVsDebugger;
            if (debugService != null)
            {
                // Unegister for debug events.                
                debugService.UnadviseDebuggerEvents(cookie);
            }
        }

        public void Pause()
        {
            isPaused = true;
        }

        public void Resume()
        {
            isPaused = false;
        }
    }
}
