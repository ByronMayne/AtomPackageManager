using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AtomPackageManager
{
    public interface IEventListener
    {
        void OnNotify(int eventCode, object context);
    }
}
