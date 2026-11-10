
using System;
using System.Collections.Generic;

namespace darooyab.Models
{
    public partial class Profile
    {
        public int Id { get; set; }
        public Guid? UserId { get; set; }
        public string UserName { get; set; }
        public string MelliCode { get; set; }
        public string AndroidId { get; set; }
        public string Imie { get; set; }
        public string Name { get; set; }
        public string Mobile { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
        public string Email { get; set; }
        public string NezamCode { get; set; }
        public int? ProvinceId { get; set; }
        public int? UniversityId { get; set; }
        public int? ActivityId { get; set; }
        public int? SpecialityId { get; set; }
        public int? LastUpdateId { get; set; }
        public string FailedUpdates { get; set; }
        public int? AccessLevel { get; set; }
        public string Description { get; set; }
        public string Platform { get; set; }
        public string PlatformVersion { get; set; }
    }
}
