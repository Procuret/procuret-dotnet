using System;
using System.Runtime.Serialization;


namespace ProcuretAPI
{

    [DataContract]
    public struct EntityHeadline
    {
        [DataMember]
        private readonly String legal_entity_name;

        [DataMember]
        private readonly String entity_id;

        public String EntityId { get { return this.entity_id;  } }
        public String LegalEntityName {
            get { return this.legal_entity_name;  }
        }

    }
}
