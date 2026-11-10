using System;
using System.Collections.Generic;

namespace darooyab.Models
{
    public partial class AppReg
    {
        public string Name { get; set; }
        public string Mobile { get; set; }
        public string NezamCode { get; set; }
        public string استان { get; set; }
        public string فعالیت { get; set; }
        public string تخصص { get; set; }
        public string MelliCode { get; set; }
        public string UserName { get; set; }
        public int Id { get; set; }
        public int? LastUpdateId { get; set; }
        public string Description { get; set; }
        public string Email { get; set; }
        public string Platform { get; set; }
        public string PlatformVersion { get; set; }
        public string AndroidId { get; set; }
        public string دانشگاه { get; set; }
        public Guid? UserId { get; set; }
        public int? ActivityId { get; set; }
        public int? SpecialityId { get; set; }
    }
}
