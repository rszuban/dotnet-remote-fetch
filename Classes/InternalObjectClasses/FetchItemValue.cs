using System;
using System.ComponentModel.DataAnnotations;

namespace RemoteFetch.Classes
{
    class FetchItemValue
    {
        [Key]
        public int Id { get; set; }
        public string Value { get; set; }
        public DateTime ValueDateTime { get; set; }
    }
}
