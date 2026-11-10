using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace darooyab.Models
{
    public class DarooyabUser
    {
        public DarooyabUser()
        {
            DarooyabSessions = new HashSet<DarooyabSession>();
            DarooyabInvoices = new HashSet<DarooyabInvoice>();
            DarooyabUserAddresses = new HashSet<DarooyabUserAddress>();
            ReceiveMessages = new HashSet<DarooyabMessage>();
            SendMessages = new HashSet<DarooyabMessage>();
            DarooyabRecommandPrescriptions = new HashSet<DarooyabRecommandPrescription>();
            PrescriptionConfirms = new HashSet<PrescriptionConfirm>();
            DarooyabUserRoles = new HashSet<DarooyabUserRole>();
            DarooyabServiceRegisters = new HashSet<DarooyabServiceRegister>();
            PharmacyLocations = new HashSet<DarooyabPharmacyLocation>();
            DarooyabUserFactories = new HashSet<DarooyabUserFactory>();
            DarooyabDrugShortages=new HashSet<DarooyabDrugShortage>();
             DarooyabFavoritePharmacies=new HashSet<DarooyabFavoritePharmacy>();
        }
        public int Id { get; set; }
        public string? Mobile { get; set; }
        public string? MobileIRIMC { get; set; }
        public string? Email { get; set; }
        public string? ConfirmCode { get; set; }
        public bool IsMobileConfirm { get; set; }
        public bool IsEmailConfirm { get; set; }
        public string? Name { get; set; }
        public string? Family { get; set; }
        public string? BirthYear { get; set; }
        public bool? IsAcademicStaff { get; set; }
        public bool? IsDeleted { get; set; }
        public int? CityId { get; set; }
        public byte? Platform { get; set; }
        [ForeignKey("CityId")]
        public DarooyabCity? DarooyabCity { get; set; }

        // public int? ACtivityId { get; set; }
        // [ForeignKey("ACtivityId")]
        [NotMapped]
        public DarooyabActivity? DarooyabActivity { get; set; }
        public int? ActivityId { get; set; }
        public int? SpecialtyId { get; set; }
        [ForeignKey("SpecialtyId")]
        public DarooyabSpecialty? DarooyabSpecialty { get; set; }

        public int? SuperSoecialtyId { get; set; }
        [ForeignKey("SuperSoecialtyId")]
        public DarooyabSuperSpecialty? DarooyabSuperSpecialty { get; set; }

        public int? DarooyabFellowshipId { get; set; }
        [ForeignKey("DarooyabFellowshipId")]
        public DarooyabFellowship? DarooyabFellowship { get; set; }
        public string? MedicalSystemNumber { get; set; }

        public int? UniversityId { get; set; }
        [ForeignKey("UniversityId")]
        public DarooyabUniversityMS? DarooyabUniversityMS { get; set; }
        public int? GeneralId { get; set; }
        [ForeignKey("GeneralId")]
        public DarooyabGeneralItem? DarooyabGeneralItem { get; set; }
        public string? DocumentImageUrl { get; set; }
        public bool? IsResident { get; set; }
        public bool? IsFounder { get; set; }
        [Display(Name = "وضعیت تایید هویت و مدارک")]

        public bool? IsAuthenticationConfirmed { get; set; }
        public DateTime? AuthenticationConfirmedDate { get; set; }
        public DateTime? FirstLoginDate { get; set; }
        public string? AuthenticationConfirmedByUserName { get; set; }
        [UIHint("Multiline")]
        [Display(Name = "توضیحات اعتبارسنجی ")]
        public string? AuthenticationConfirmedDescription { get; set; }
        public bool? IsHumanPharmacyFounder { get; set; }
        public bool? IsAnimalPharmacyFounder { get; set; }
        public string? ProfileImageUrl { get; set; }
        public string? Password { get; set; }
        public bool? IsAuthenticationConfirmByIRIMC { get; set; }
        public string? IRIMCResult { get; set; }
        public bool? ImageProfileSystem { get; set; }
        public ICollection<DarooyabSession> DarooyabSessions { get; set; }
        public ICollection<DarooyabInvoice> DarooyabInvoices { get; set; }
        public ICollection<DarooyabUserAddress> DarooyabUserAddresses { get; set; }
        public ICollection<DarooyabMessage> ReceiveMessages { get; set; }
        public ICollection<DarooyabMessage> SendMessages { get; set; }
        public ICollection<DarooyabRecommandPrescription> DarooyabRecommandPrescriptions { get; set; }
        public ICollection<PrescriptionConfirm> PrescriptionConfirms { get; set; }
        public ICollection<DarooyabUserRole> DarooyabUserRoles { get; set; }

        public ICollection<DarooyabServiceRegister> DarooyabServiceRegisters { get; set; }
        public ICollection<DarooyabPharmacyLocation> PharmacyLocations { get; set; }
        public ICollection<DarooyabUserFactory> DarooyabUserFactories { get; set; }
        public ICollection<DarooyabDrugShortage> DarooyabDrugShortages { get; set; }
        public virtual ICollection<DarooyabFavoritePharmacy> DarooyabFavoritePharmacies { get; set; }

    }
}