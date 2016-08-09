using System;
using System.Runtime.InteropServices;
using System.Text;

namespace BlyncLightAddin
{
    public class ActiveWindowWatcher : IBlyncWatcher
    {        
        public event EventHandler StatusChanged;
        private bool isPaused;
        private const uint WINEVENT_OUTOFCONTEXT = 0;
        private const uint EVENT_SYSTEM_FOREGROUND = 3;
        IntPtr m_hhook;
        WinEventDelegate dele = null;

        delegate void WinEventDelegate(IntPtr hWinEventHook, uint eventType, IntPtr hwnd, int idObject, int idChild, uint dwEventThread, uint dwmsEventTime);

        [DllImport("user32.dll")]
        static extern IntPtr SetWinEventHook(uint eventMin, uint eventMax, IntPtr hmodWinEventProc, WinEventDelegate lpfnWinEventProc, uint idProcess, uint idThread, uint dwFlags);

        [DllImport("user32.dll")]
        static extern bool UnhookWinEvent(IntPtr hWinEventHook);

        [DllImport("user32.dll")]
        static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll")]
        static extern int GetWindowText(IntPtr hWnd, StringBuilder text, int count);

        private string GetActiveWindowTitle()
        {
            const int nChars = 256;
            IntPtr handle = IntPtr.Zero;
            StringBuilder Buff = new StringBuilder(nChars);
            handle = GetForegroundWindow();

            if (GetWindowText(handle, Buff, nChars) > 0)
            {
                return Buff.ToString();
            }
            return null;
        }

        public void WinEventProc(IntPtr hWinEventHook, uint eventType, IntPtr hwnd, int idObject, int idChild, uint dwEventThread, uint dwmsEventTime)
        {
            var text = GetActiveWindowTitle();
            if (string.IsNullOrEmpty(text))
            {
                OnStatusChanged(new StatusChangedEventArgs(Status.Default));
            }
            else
            {
                if (text.Contains("Microsoft Visual Studio"))
                {
                    //Raise event
                    OnStatusChanged(new StatusChangedEventArgs(Status.Busy));
                }
                else
                {
                    OnStatusChanged(new StatusChangedEventArgs(Status.Available));
                }
            }
        }
        public void Initialize()
        {
            dele = new WinEventDelegate(WinEventProc);//<-causing ERROR
            m_hhook = SetWinEventHook(EVENT_SYSTEM_FOREGROUND, EVENT_SYSTEM_FOREGROUND, IntPtr.Zero, dele, 0, 0, WINEVENT_OUTOFCONTEXT);
        }

        public void Close()
        {
            if (m_hhook.ToInt32() != 0)
            {
                UnhookWinEvent(m_hhook);
            }
        }
        
        protected virtual void OnStatusChanged(StatusChangedEventArgs e)
        {            
            if (StatusChanged != null && isPaused == false)
            {
                StatusChanged(this, e);
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
