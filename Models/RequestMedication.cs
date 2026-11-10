using System;
using System.Collections.Generic;

namespace darooyab.Models
{
    public partial class RequestMedication
    {
        public int Id { get; set; }
        /// <summary>
        /// پذیرش نسخه توسط داروخانه
        /// </summary>
        public int? AcceptedByPharmacyIdFk { get; set; }
        /// <summary>
        /// تاریخ ثبت نسخه
        /// </summary>
        public DateTime RegistrationDate { get; set; }
        /// <summary>
        /// تاریخ پذیرش نسخه توسط داروخانه
        /// </summary>
        public string AcceptedDate { get; set; }
        /// <summary>
        /// نام و نام خانوادگی ثبت کننده نسخه
        /// </summary>
        public string RegistrationName { get; set; }
        public string MobileNumber { get; set; }
        public int ProvinceIdFk { get; set; }
        public int CityIdFk { get; set; }
        public string DrugPrescriptionImageUrl { get; set; }

        public virtual Pharmacy AcceptedByPharmacyIdFkNavigation { get; set; }
        public virtual Category CityIdFkNavigation { get; set; }
        public virtual Category ProvinceIdFkNavigation { get; set; }
    }
}
