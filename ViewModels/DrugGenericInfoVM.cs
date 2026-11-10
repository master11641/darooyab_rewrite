using System;
public class DrugGenericInfoVM
{
    public String? DrugName { get; set; }
    public String? GroupName { get; set; }
    public byte? PregnancyCategoryId { get; set; }
    public String? PregnancyCategoryCode { get; set; }
    public String? GeneralInformation { get; set; }
    public String? pregnancyCategoryDes { get; set; }
    public String? PregnancyCategoryDes_Fa { get; set; }
    public int? DrugID { get; set; }
    public String? LinkID { get; set; }
    public byte? GroupID { get; set; }
    public String? SubGroupID { get; set; }
    public String? Bardary { get; set; }
    public String? MavaredMasraf { get; set; }
    public String? Mekanism { get; set; }
    public String? Farma_Ajza { get; set; }
    public String? ManeMasraf { get; set; }
    public String? Hoshdar { get; set; }
    public String? Avarez { get; set; }
    public String? Tadakhol { get; set; }
    public String? Nokat { get; set; }
    public String? Ashkal { get; set; }
    public String? MeghdarMasraf { get; set; }
    public String? MavadMoaser_Profile { get; set; }
    public String? Pregnancy { get; set; }
    public String? AdultDosing { get; set; }
    public String? PediatricDosing { get; set; }
    public String? GeriatricDosing { get; set; }

    public String? DrugsComURL { get; set; }
    public String? MedscapeComURL { get; set; }

    public long? IRC { get; set; }
    public String? Image1Url { get; set; }
    public String? Image2Url { get; set; }
    public String? Image3Url { get; set; }
    public String? ClipUrl { get; set; }
    public String? VoiceUrl { get; set; }
    public int? ToxicationID { get; set; }
    public string? ToxicationSubject { get; set; }
    public string? ToxicationContent { get; set; }
    public int? DosageFormID { get; set; }

    public short? IrgenericId { get; set; }
}