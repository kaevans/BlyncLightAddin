using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlyncLightAddin
{
    interface IBlyncWatcher
    {
        void Initialize();

        void Pause();

        void Resume();

        void Close();

        event EventHandler StatusChanged;
    }
}
