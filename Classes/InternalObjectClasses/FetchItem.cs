using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace RemoteFetch.Classes
{
    class FetchItem
    {
        public string Id { get; set; }
        public string ItemName { get; set; }
        public string Xpath { get; set; }
        public virtual ICollection<FetchItemValue> FetchItemValues { get; set; }
        [NotMapped]
        public List<FetchValueParser> FetchValueParser { get; set; }
    }
}
