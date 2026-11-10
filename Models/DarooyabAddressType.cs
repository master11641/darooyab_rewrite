using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace darooyab.Models
{
    public class DarooyabAddressType
    {
        public DarooyabAddressType()
        {
            DarooyabUserAddresses = new HashSet<DarooyabUserAddress>();
        }
        public int Id { get; set; }
        public string Title { get; set; }
        public ICollection<DarooyabUserAddress> DarooyabUserAddresses { get; set; }
    }
}