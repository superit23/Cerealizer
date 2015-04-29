using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

namespace Cerealizer
{
    public class NSync
    {
        public static void Invoke(ISynchronizeInvoke obj, Action action)
        {
            if (!obj.InvokeRequired)
                action();
            else
                obj.Invoke(action, new object[1]);
        }


    }
}
