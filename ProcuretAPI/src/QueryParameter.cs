using System;
namespace ProcuretAPI
{
    internal struct QueryParameter
    {

        private readonly String key;
        private readonly String value;

        internal String Pair
        {
            get { return this.key + "=" + this.value; }
        }

        internal QueryParameter(String value, String key)
        {
            this.key = key;
            this.value = value;
            return;
        }

        internal QueryParameter(Int32 value, String key)
        {
            this.key = key;
            this.value = value.ToString();
            return;
        }

        internal QueryParameter(Int16 value, String key)
        {
            this.key = key;
            this.value = value.ToString();
            return;
        }

        internal QueryParameter(Int64 value, String key)
        {
            this.key = key;
            this.value = value.ToString();
            return;
        }

        internal QueryParameter(Order value)
        {
            this.key = "order";
            if (value == Order.ASCENDING)
            {
                this.value = "ascending";
            } else
            {
                this.value = "descending";
            }
            return;
        }

        internal QueryParameter(InstalmentLink.OrderBy value)
        {
            this.key = "order_by";
            if (value == InstalmentLink.OrderBy.CREATED)
            {
                this.value = "created";
                return;
            }
            throw new ApiRequestException();
        }

    }
}
