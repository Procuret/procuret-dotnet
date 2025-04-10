using System;
using System.Text.Json.Serialization;

namespace ProcuretAPI
{
    public struct EntityHeadline
    {
        [JsonPropertyName("legal_entity_name")]
        private readonly string legal_entity_name;

        [JsonPropertyName("entity_id")]
        private readonly long entity_id;

        public long EntityId 
        {
            get { return this.entity_id; }
        }

        public string LegalEntityName 
        {
            get { return this.legal_entity_name; }
        }
    }
}
