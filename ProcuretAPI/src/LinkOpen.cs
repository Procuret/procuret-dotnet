using System;
using System.Runtime.Serialization;

namespace ProcuretAPI
{
    [DataContract(Name="procuret_data", Namespace="")]
    public struct LinkOpen
    {
        public readonly Int32 Sequence;
        public readonly String Created;
    }
}
