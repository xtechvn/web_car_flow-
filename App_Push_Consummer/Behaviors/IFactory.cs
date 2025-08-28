using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App_Push_Consummer.Behaviors
{
    public interface IFactory
    {
        void DoSomeRealWork(string data_queue);
    }
}
