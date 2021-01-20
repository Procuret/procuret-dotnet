using System.Collections.Generic;
using System;

namespace ProcuretAPI
{
    internal struct QueryString
    {

        private readonly QueryParameter[] parameters;

        internal String Query
        {
            get
            {

                if (this.parameters.Length < 1) { return "";  }

                var query = "?";
                for (Int16 i = 0; i < this.parameters.Length; i++)
                {

                    if (i == 0)
                    {
                        query += this.parameters[i].Pair;
                        continue;
                    }

                    query += "&" + this.parameters[i].Pair;
                    continue;

                }

                return query;

            }
        }

        internal QueryString(QueryParameter[] parameters)
        {
            this.parameters = parameters;
            return;
        }

        internal QueryString(List<QueryParameter> parameters)
        {
            this.parameters = parameters.ToArray();
            return;
        }



    }
}
