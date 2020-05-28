using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace RemoteFetch.Classes
{
    class FetchUnit
    {
        [Key]
        public string UnitName { get; set; }
        public string Url { get; set; }
        public string Schedule { get; set; }
        [NotMapped]
        public DateTime NextScheduledFetch { get; set; }
        public virtual ICollection<FetchItem> FetchItems { get; set; }
    }
}
