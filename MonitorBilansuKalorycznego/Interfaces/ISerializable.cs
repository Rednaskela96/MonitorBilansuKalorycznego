using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonitorBilansuKalorycznego.Interfaces
{
    internal interface ISerializable
    {
        string ToJson();
        void FromJson(string json);
    }
}
