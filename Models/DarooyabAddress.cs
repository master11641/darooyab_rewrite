using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace darooyab.Models
{
    public class DarooyabUserAddress
    {
        public DarooyabUserAddress()
        {
            DarooyabServiceRegisters = new HashSet<DarooyabServiceRegister>();
        }
        public int Id { get; set; }
        public int AddressTypeId { get; set; }
        [ForeignKey("AddressTypeId")]
        public DarooyabAddressType DarooyabAddressType { get; set; }
        public int CityId { get; set; }
        [ForeignKey("CityId")]
        public DarooyabCity DarooyabCity { get; set; }
        public string? Address { get; set; }
        public string? PostalCode { get; set; }
        [Display(Name = "عرض جغرافیایی")]
        public decimal? Latitude { get; set; }
        [Display(Name = "طول جغرافیایی")]
        public decimal? Longitude { get; set; }
        public string? Phone1 { get; set; }
        public string? Phone2 { get; set; }
        public string? Website { get; set; }
        public bool IsVisible { get; set; }
        public int UserId { get; set; }
        [ForeignKey("UserId")]
        public DarooyabUser DarooyabUser { get; set; }
        public bool IsDeleted { get; set; }
        public ICollection<DarooyabServiceRegister> DarooyabServiceRegisters { get; set; }

    }
}