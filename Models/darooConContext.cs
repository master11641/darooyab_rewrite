using darooyab.Models.DTOs;
using darooyab.ViewModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using System;
using System.Collections.Generic;

namespace darooyab.Models
{
    public partial class darooConContext : DbContext
    {
        public darooConContext() { }

        public darooConContext(DbContextOptions<darooConContext> options) : base(options) { }

        public DbSet<DarooyabServiceVM> DarooyabServiceVMs { get; set; }
        public DbSet<BrandInfoVM> BrandInfoVMs { get; set; }
        public DbSet<AhfsGroupModelVM> AhfsGroupModelVMs { get; set; }
        public DbSet<GenericInfoVM> GenericInfoVMs { get; set; }
        public DbSet<DistributerVM> DistributerVMs { get; set; }
        public DbSet<ChartHistoryBySpecialityVM> ChartHistoryBySpecialityVMs { get; set; }
        public DbSet<FactoryVM> FactoryVMs { get; set; }
        public DbSet<ProducerVM> producerVMs { get; set; }
        public DbSet<DrugGenericInfoVM> DrugGenericInfoVM { get; set; }
        public DbSet<ChartHistoryNewsVM> ChartHistoryNewsVMs { get; set; }
        public DbSet<ChartProvinceNewsVM> ChartProvinceNewsVMs { get; set; }
        public DbSet<ChartHistoryNewsTenDayVM> ChartHistoryNewsTenDayVMs { get; set; }
        public virtual DbSet<AdminUser> AdminUsers { get; set; }
        public virtual DbSet<Advice> Advices { get; set; }
        public virtual DbSet<Ahfsgroup> Ahfsgroups { get; set; }
        public virtual DbSet<AppNewsViewLog> AppNewsViewLogs { get; set; }
        public virtual DbSet<AppReg> AppRegs { get; set; }
        public virtual DbSet<AppRegView> AppRegViews { get; set; }
        public virtual DbSet<AppUsersCompany> AppUsersCompanies { get; set; }
        public virtual DbSet<AspnetApplication> AspnetApplications { get; set; }
        public virtual DbSet<AspnetMembership> AspnetMemberships { get; set; }
        public virtual DbSet<AspnetPath> AspnetPaths { get; set; }
        public virtual DbSet<AspnetPersonalizationAllUser> AspnetPersonalizationAllUsers { get; set; }
        public virtual DbSet<AspnetPersonalizationPerUser> AspnetPersonalizationPerUsers { get; set; }
        public virtual DbSet<AspnetProfile> AspnetProfiles { get; set; }
        public virtual DbSet<AspnetRole> AspnetRoles { get; set; }
        public virtual DbSet<AspnetSchemaVersion> AspnetSchemaVersions { get; set; }
        public virtual DbSet<AspnetUser> AspnetUsers { get; set; }
        public virtual DbSet<AspnetWebEventEvent> AspnetWebEventEvents { get; set; }
        public virtual DbSet<BusinessInformation> BusinessInformations { get; set; }
        public virtual DbSet<CallNumber> CallNumbers { get; set; }
        public virtual DbSet<CategoriesInRole> CategoriesInRoles { get; set; }
        public virtual DbSet<Category> Categories { get; set; }
        public virtual DbSet<Comment> Comments { get; set; }
        public virtual DbSet<CommertialBrand> CommertialBrands { get; set; }
        public virtual DbSet<CounterUserView> CounterUserViews { get; set; }
        public virtual DbSet<Country> Countries { get; set; }
        public virtual DbSet<DarooyabAppNews> DarooyabAppNews { get; set; }
        public virtual DbSet<DeletedHospitalDosageForm> DeletedHospitalDosageForms { get; set; }
        public virtual DbSet<Disease> Diseases { get; set; }
        public virtual DbSet<DistributorDrug> DistributorDrugs { get; set; }
        public virtual DbSet<DistributorsAgency> DistributorsAgencies { get; set; }
        public virtual DbSet<DosageForm> DosageForms { get; set; }
        public virtual DbSet<DarooyabDosageForm> DarooyabDosageForms { get; set; }
        public virtual DbSet<DosageFormAttribute> DosageFormAttributes { get; set; }
        public virtual DbSet<DosageFormPerSection> DosageFormPerSections { get; set; }
        public virtual DbSet<DrugInfo> DrugInfos { get; set; }
        public virtual DbSet<DrugsGenericInfo> DrugsGenericInfos { get; set; }
        public virtual DbSet<DrugsName> DrugsNames { get; set; }
        public virtual DbSet<DrugsTarkibInfo> DrugsTarkibInfos { get; set; }
        public virtual DbSet<DrugsType> DrugsTypes { get; set; }
        public virtual DbSet<Factory> Factorys { get; set; }
        public virtual DbSet<FactoryType> FactoryTypes { get; set; }
        public virtual DbSet<GenericInfoAttribute> GenericInfoAttributes { get; set; }
        public virtual DbSet<Group> Groups { get; set; }
        public virtual DbSet<Group1> Groups1 { get; set; }
        public virtual DbSet<HospitalAccessApproval> HospitalAccessApprovals { get; set; }
        public virtual DbSet<HospitalDosageForm> HospitalDosageForms { get; set; }
        public virtual DbSet<LinkInfoAddress> LinkInfoAddresses { get; set; }
        public virtual DbSet<LinkInfoId> LinkInfoIds { get; set; }
        public virtual DbSet<LogHistory> LogHistories { get; set; }
        public virtual DbSet<News> News { get; set; }
        public virtual DbSet<NewsInCategory> NewsInCategories { get; set; }
        public virtual DbSet<Notification> Notifications { get; set; }
        public virtual DbSet<OfficeAddress> OfficeAddresses { get; set; }
        public virtual DbSet<Pharmacy> Pharmacies { get; set; }
        public virtual DbSet<PharmacyAccount> PharmacyAccounts { get; set; }
        public virtual DbSet<PharmacyExchange> PharmacyExchanges { get; set; }
        public virtual DbSet<PharmacyRequest> PharmacyRequests { get; set; }
        public virtual DbSet<PregnancyCategory> PregnancyCategories { get; set; }
        public virtual DbSet<Profile> Profiles { get; set; }
        public virtual DbSet<RegisteredImei> RegisteredImeis { get; set; }
        public virtual DbSet<RequestMedication> RequestMedications { get; set; }
        public virtual DbSet<Section> Sections { get; set; }
        public virtual DbSet<SectionPerHospital> SectionPerHospitals { get; set; }
        public virtual DbSet<Setting> Settings { get; set; }
        public virtual DbSet<SlidInfoTopIrc> SlidInfoTopIrcs { get; set; }
        public virtual DbSet<SlidSettingsTopIrc> SlidSettingsTopIrcs { get; set; }
        public virtual DbSet<SpecificInformation> SpecificInformations { get; set; }
        public virtual DbSet<SpecificInformationImage> SpecificInformationImages { get; set; }
        public virtual DbSet<SubGroup> SubGroups { get; set; }
        public virtual DbSet<Suggestion> Suggestions { get; set; }
        public virtual DbSet<Tarkibat> Tarkibats { get; set; }
        public virtual DbSet<TblAdvertisement> TblAdvertisements { get; set; }
        public virtual DbSet<TblAttachmentFileToGenericAttribute> TblAttachmentFileToGenericAttributes { get; set; }
        public virtual DbSet<TblBrandAttribute> TblBrandAttributes { get; set; }
        public virtual DbSet<TblContactu> TblContactus { get; set; }
        public virtual DbSet<TblGenericPageAttribute> TblGenericPageAttributes { get; set; }
        public virtual DbSet<TblNews> TblNews { get; set; }
        public virtual DbSet<TblNewsGroup> TblNewsGroups { get; set; }
        public virtual DbSet<TblOrganizationDetail> TblOrganizationDetails { get; set; }
        public virtual DbSet<TblToxication> TblToxications { get; set; }
        public virtual DbSet<Temp1> Temp1s { get; set; }
        public virtual DbSet<TempDosageForm> TempDosageForms { get; set; }
        public virtual DbSet<TempHixCodeing> TempHixCodeings { get; set; }
        public virtual DbSet<TempKhadamat> TempKhadamats { get; set; }
        public virtual DbSet<TempPatientAdvice> TempPatientAdvices { get; set; }
        public virtual DbSet<TempPatientAdviceOld> TempPatientAdviceOlds { get; set; }
        public virtual DbSet<TempPharmacyAccount> TempPharmacyAccounts { get; set; }
        public virtual DbSet<TempTaminDirect> TempTaminDirects { get; set; }
        public virtual DbSet<TempTedadJavabDehiPezeshk> TempTedadJavabDehiPezeshks { get; set; }
        public virtual DbSet<TempUserMoasis> TempUserMoases { get; set; }
        public virtual DbSet<TriggerCommand> TriggerCommands { get; set; }
        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<View1> View1s { get; set; }
        public virtual DbSet<ViewAppNewsHistoryView> ViewAppNewsHistoryViews { get; set; }
        public virtual DbSet<ViewCompaniesAndCountry> ViewCompaniesAndCountries { get; set; }
        public virtual DbSet<ViewCompaniesAndCountry2> ViewCompaniesAndCountry2s { get; set; }
        public virtual DbSet<ViewCompany> ViewCompanies { get; set; }
        public virtual DbSet<ViewDrugsDetailForApp> ViewDrugsDetailForApps { get; set; }
        public virtual DbSet<ViewDrugsName> ViewDrugsNames { get; set; }
        public virtual DbSet<ViewFarsiName> ViewFarsiNames { get; set; }
        public virtual DbSet<ViewHospitalActive> ViewHospitalActives { get; set; }
        public virtual DbSet<ViewImporterCompany> ViewImporterCompanies { get; set; }
        public virtual DbSet<ViewMob> ViewMobs { get; set; }
        public virtual DbSet<ViewMobMain> ViewMobMains { get; set; }
        public virtual DbSet<ViewNewsApp> ViewNewsApps { get; set; }
        public virtual DbSet<ViewNewsClick> ViewNewsClicks { get; set; }
        public virtual DbSet<ViewPharmacyAllCounForEsf> ViewPharmacyAllCounForEsfs { get; set; }
        public virtual DbSet<ViewPharmacyAllCount> ViewPharmacyAllCounts { get; set; }
        public virtual DbSet<ViewPharmacyDatum> ViewPharmacyData { get; set; }
        public virtual DbSet<ViewPharmacyDatum1> ViewPharmacyData1 { get; set; }
        public virtual DbSet<ViewProductName> ViewProductNames { get; set; }
        public virtual DbSet<ViewProductName2> ViewProductName2s { get; set; }
        public virtual DbSet<VwAspnetApplication> VwAspnetApplications { get; set; }
        public virtual DbSet<VwAspnetMembershipUser> VwAspnetMembershipUsers { get; set; }
        public virtual DbSet<VwAspnetProfile> VwAspnetProfiles { get; set; }
        public virtual DbSet<VwAspnetRole> VwAspnetRoles { get; set; }
        public virtual DbSet<VwAspnetUser> VwAspnetUsers { get; set; }
        public virtual DbSet<VwAspnetUsersInRole> VwAspnetUsersInRoles { get; set; }
        public virtual DbSet<VwAspnetWebPartStatePath> VwAspnetWebPartStatePaths { get; set; }
        public virtual DbSet<VwAspnetWebPartStateShared> VwAspnetWebPartStateShareds { get; set; }
        public virtual DbSet<VwAspnetWebPartStateUser> VwAspnetWebPartStateUsers { get; set; }
        public virtual DbSet<WebSiteUserAccount> WebSiteUserAccounts { get; set; }
        public virtual DbSet<Toxication> Toxications { get; set; }
        public virtual DbSet<DarooyabUser> DarooyabUsers { get; set; }
        public virtual DbSet<DarooyabActivity> DarooyabActivities { get; set; }
        public virtual DbSet<DarooyabCity> DarooyabCities { get; set; }
        public virtual DbSet<DarooyabCountry> DarooyabCountries { get; set; }
        public virtual DbSet<DarooyabFellowship> DarooyabFellowships { get; set; }
        public virtual DbSet<DarooyabGeneralItem> DarooyabGeneralItems { get; set; }
        public virtual DbSet<DarooyabProvince> DarooyabProvinces { get; set; }
        public virtual DbSet<DarooyabSpecialty> DarooyabSpecialties { get; set; }
        public virtual DbSet<DarooyabSpecialtySuperSpecialty> DarooyabSpecialtySuperSpecialties { get; set; }
        public virtual DbSet<DarooyabSuperSpecialty> DarooyabSuperSpecialties { get; set; }
        public virtual DbSet<DarooyabUniversityMS> DarooyabUniversityMs { get; set; }
        public virtual DbSet<DarooyabSession> DarooyabSessions { get; set; }
        public virtual DbSet<DarooyabPackage> DarooyabPackages { get; set; }
        public virtual DbSet<DarooyabInvoice> DarooyabInvoices { get; set; }
        public virtual DbSet<DarooyabDiscount> DarooyabDiscounts { get; set; }
        public virtual DbSet<DarooyabUserAddress> DarooyabUserAddresses { get; set; }
        public virtual DbSet<DarooyabAddressType> DarooyabAddressTypes { get; set; }
        public virtual DbSet<DarooyabUserPackageFree> DarooyabUserPackageFrees { get; set; }
        public virtual DbSet<DarooyabVersionApp> DarooyabVersionApps { get; set; }
        public virtual DbSet<DarooyabSponser> DarooyabSponsers { get; set; }
        public virtual DbSet<DarooyabHistory> DarooyabHistories { get; set; }
        public virtual DbSet<DarooyabMessage> DarooyabMessages { get; set; }
        public virtual DbSet<DarooyabDisease> DarooyabDiseases { get; set; }
        public virtual DbSet<DarooyabRecommandPrescription> DarooyabRecommandPrescriptions { get; set; }
        public virtual DbSet<PrescriptionType> PrescriptionTypes { get; set; }
        public virtual DbSet<DarooyabDiseaseGroup> DarooyabDiseaseGroups { get; set; }
        public virtual DbSet<PrescriptionConfirmStatus> PrescriptionConfirmStatuses { get; set; }
        public virtual DbSet<PrescriptionConfirm> PrescriptionConfirms { get; set; }
        public virtual DbSet<DarooyabRole> DarooyabRoles { get; set; }
        public virtual DbSet<DarooyabUserRole> DarooyabUserRoles { get; set; }
        public virtual DbSet<DarooyabUrl> DarooyabUrls { get; set; }
        public virtual DbSet<DarooyabUrlRole> DarooyabUrlRoles { get; set; }
        public virtual DbSet<DarooyabService> DarooyabServices { get; set; }
        public virtual DbSet<DarooyabSpecialtyService> DarooyabSpecialtyServices { get; set; }
        public virtual DbSet<DarooyabSuperService> DarooyabSuperServices { get; set; }
        public virtual DbSet<DarooyabGeneralService> DarooyabGeneralServices { get; set; }
        public virtual DbSet<DarooyabProvinceService> DarooyabProvinceServices { get; set; }
        public virtual DbSet<DarooyabCityService> DarooyabCityServices { get; set; }
        public virtual DbSet<ConversationViewModel> ConversationViewModels { get; set; }
        public virtual DbSet<DarooyabActivityMedium> DarooyabActivityMedia { get; set; }

        public virtual DbSet<DarooyabServiceRegister> DarooyabServiceRegisters { get; set; }
        public virtual DbSet<DarooyabPrescription> DarooyabPrescriptions { get; set; }
        public virtual DbSet<DarooyabPharmacyLocation> DarooyabPharmacyLocations { get; set; }
        public virtual DbSet<KeywordLink> KeywordLinks { get; set; }
        public virtual DbSet<RankingTransaction> RankingTransactions { get; set; }
        public virtual DbSet<WalletTransaction> WalletTransactions { get; set; }
        public virtual DbSet<DarooyabDrugDistribution> DarooyabDrugDistributions { get; set; }
        public virtual DbSet<DarooyabUserFactory> DarooyabUserFactories { get; set; }
        public virtual DbSet<CompanyReservedGeneric> CompanyReservedGenerics { get; set; }
        public virtual DbSet<DarooyabBrandRestriction> DarooyabBrandRestrictions { get; set; }
        public virtual DbSet<DarooyabDrugShortage> DarooyabDrugShortages { get; set; }
        public virtual DbSet<PharmacySpecialty> PharmacySpecialties { get; set; }
        public virtual DbSet<PharmacySpecialtyMap> PharmacySpecialtyMaps { get; set; }
        public DbSet<DarooyabCityDistrict> DarooyabCityDistricts { get; set; }
        public DbSet<InsuranceProvider> InsuranceProviders { get; set; }
        public DbSet<PharmacyInsuranceContract> PharmacyInsuranceContracts { get; set; }
        public DbSet<DarooyabPrescriptionSMSMessage> DarooyabPrescriptionSMSMessages { get; set; }
        public virtual DbSet<DarooyabFavoritePharmacy> DarooyabFavoritePharmacies { get; set; }
        public virtual DbSet<Questionnaire> Questionnaires { get; set; }
        public virtual DbSet<Question> Questions { get; set; }
        public virtual DbSet<QuestionOption> QuestionOptions { get; set; }
        public virtual DbSet<QuestionnaireResponse> QuestionnaireResponses { get; set; }
        public virtual DbSet<Answer> Answers { get; set; }
        public DbSet<PatientReferralGenericResult> PatientReferralGenericResults => Set<PatientReferralGenericResult>();
        public virtual DbSet<SimilarBrandPharmacyRequestResult> SimilarBrandPharmacyRequestResults { get; set; }
        public DbSet<PharmacyDrugShortageExchangeRow> PharmacyDrugShortageExchangeRows =>
            Set<PharmacyDrugShortageExchangeRow>();
        public virtual DbSet<PharmacyRankingDiscountCard> PharmacyRankingDiscountCards { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            modelBuilder.Entity<DarooyabServiceRegister>().HasKey(u => new
            {
                u.UserId,
                u.ServiceId
            });
            modelBuilder.Entity<DarooyabServiceVM>().HasNoKey();
            modelBuilder.Entity<BrandInfoVM>().HasNoKey();
            modelBuilder.Entity<ProducerVM>().HasNoKey();
            modelBuilder.Entity<FactoryVM>().HasNoKey();
            modelBuilder.Entity<ConversationViewModel>().HasNoKey();
            modelBuilder.Entity<AhfsGroupModelVM>().HasNoKey();
            modelBuilder.Entity<DrugGenericInfoVM>().HasNoKey();
            modelBuilder.Entity<DistributerVM>().HasNoKey();
            modelBuilder.Entity<GenericInfoVM>().HasNoKey();
            modelBuilder.Entity<ChartHistoryNewsVM>().HasNoKey();
            modelBuilder.Entity<ChartProvinceNewsVM>().HasNoKey();
            modelBuilder.Entity<ChartHistoryNewsTenDayVM>().HasNoKey();
            modelBuilder.Entity<ChartHistoryBySpecialityVM>().HasNoKey();
            modelBuilder.Entity<UserDarooyabHistoryResult>().HasNoKey();
            modelBuilder.Entity<PatientReferralGenericResult>().HasNoKey();
             modelBuilder.Entity<SimilarBrandPharmacyRequestResult>().HasNoKey();
             modelBuilder.Entity<PatientReferralSimilarBrandPagedResult>().HasNoKey();
            modelBuilder.Entity<PharmacyDrugShortageExchangeRow>(entity =>
            {
                entity.HasNoKey();
                entity.Property(row => row.Id).HasColumnName("ID");
                entity.Property(row => row.PharmacyAccountIdFk).HasColumnName("PharmacyAccountId_fk");
                entity.Property(row => row.DrugName).HasColumnName("DrugName");
                entity.Property(row => row.DrugCount).HasColumnName("DrugCount");
                entity.Property(row => row.DrugExpireDate).HasColumnName("DrugExpireDate");
                entity.Property(row => row.UnitPrice).HasColumnName("UnitPrice");
                entity.Property(row => row.DrugUnitNum).HasColumnName("DrugUnitNum");
                entity.Property(row => row.RegistrationDateTime).HasColumnName("RegistrationDateTime");
                entity.Property(row => row.ExcessPharmacyProvinceName)
                    .HasColumnName("ExcessPharmacy_ProvinceName");
                entity.Property(row => row.ExcessPharmacyTownshipName)
                    .HasColumnName("ExcessPharmacy_TownShipName");
                entity.Property(row => row.ExcessPharmacyAddress)
                    .HasColumnName("ExcessPharmacy_Address");
                entity.Property(row => row.ExcessPharmacyTell)
                    .HasColumnName("ExcessPharmacy_Tell");
                entity.Property(row => row.ExcessPharmacyName)
                    .HasColumnName("ExcessPharmacy_Name");
                entity.Property(row => row.ExcessPharmacyMobile)
                    .HasColumnName("ExcessPharmacy_Mobile");
                entity.Property(row => row.SumRequireCountExchAlready)
                    .HasColumnName("SumRequireCountExchAlready");
                entity.Property(row => row.ShortageThatMyExcessDrugName)
                    .HasColumnName("ShortageThatMyExcess_DrugName");
            });

  modelBuilder.Entity<PharmacyRankingDiscountCard>(entity =>
            {
                entity.ToTable("PharmacyRankingDiscountCard", table =>
                {
                    table.HasCheckConstraint(
                        "CK_PharmacyRankingDiscountCard_GiftChargeAmount",
                        "[GiftChargeAmount] >= 0");
                    table.HasCheckConstraint(
                        "CK_PharmacyRankingDiscountCard_Status",
                        "[Status] IN (1, 2, 3)");
                });

                entity.HasKey(e => e.Id);

                entity.Property(e => e.CardCode)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.HasIndex(e => e.CardCode)
                    .IsUnique()
                    .HasDatabaseName("UX_PharmacyRankingDiscountCard_CardCode");

                entity.Property(e => e.CreatedAt)
                    .HasDefaultValueSql("(GETDATE())");

                entity.Property(e => e.DiscountCode)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.GiftChargeAmount)
                    .HasDefaultValue(0);

                entity.Property(e => e.FactoryId)
                    .HasColumnName("FactoryID");

                entity.Property(e => e.Status)
                    .HasConversion<byte>()
                    .HasDefaultValue(PharmacyRankingDiscountCardStatus.Active);

                entity.Property(e => e.StatusDate)
                    .HasDefaultValueSql("(GETDATE())");

                entity.Property(e => e.PharmacyId)
                    .HasColumnName("PharmacyID");

                entity.HasOne(e => e.Factory)
                    .WithMany()
                    .HasForeignKey(e => e.FactoryId)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("FK_PharmacyRankingDiscountCard_Factorys");

                entity.HasOne(e => e.Pharmacy)
                    .WithMany()
                    .HasForeignKey(e => e.PharmacyId)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("FK_PharmacyRankingDiscountCard_Pharmacy");
            });

            modelBuilder.UseCollation("Arabic_CI_AS");

            modelBuilder.Entity<DarooyabUserRole>()
               .HasKey(bc => new { bc.UserId, bc.RoleId });
            modelBuilder.Entity<DarooyabUserRole>()
                .HasOne(bc => bc.DarooyabUser)
                .WithMany(b => b.DarooyabUserRoles)
                .HasForeignKey(bc => bc.UserId);
            modelBuilder.Entity<DarooyabUserRole>()
                .HasOne(bc => bc.DarooyabRole)
                .WithMany(c => c.DarooyabUserRoles)
                .HasForeignKey(bc => bc.RoleId);


            modelBuilder.Entity<DarooyabUrlRole>()
               .HasKey(bc => new { bc.UrlId, bc.RoleId });
            modelBuilder.Entity<DarooyabUrlRole>()
                .HasOne(bc => bc.DarooyabUrl)
                .WithMany(b => b.DarooyabUrlRoles)
                .HasForeignKey(bc => bc.UrlId);
            modelBuilder.Entity<DarooyabUserRole>()
                .HasOne(bc => bc.DarooyabRole)
                .WithMany(c => c.DarooyabUserRoles)
                .HasForeignKey(bc => bc.RoleId);



            modelBuilder.Entity<DarooyabUserFactory>()
                       .HasKey(bc => new { bc.UserId, bc.FactoryId });
            modelBuilder.Entity<DarooyabUserFactory>()
                .HasOne(bc => bc.DarooyabUser)
                .WithMany(b => b.DarooyabUserFactories)
                .HasForeignKey(bc => bc.UserId);
            modelBuilder.Entity<DarooyabUserFactory>()
                .HasOne(bc => bc.Factory)
                .WithMany(c => c.DarooyabUserFactories)
                .HasForeignKey(bc => bc.FactoryId);




            modelBuilder.Entity<DarooyabSpecialtyService>()
          .HasKey(bc => new { bc.ServiceId, bc.SpecialtyId });
            modelBuilder.Entity<DarooyabSpecialtyService>()
                .HasOne(bc => bc.DarooyabService)
                .WithMany(b => b.DarooyabSpecialtyServices)
                .HasForeignKey(bc => bc.ServiceId);
            modelBuilder.Entity<DarooyabSpecialtyService>()
                .HasOne(bc => bc.DarooyabSpecialty)
                .WithMany(c => c.DarooyabSpecialtyServices)
                .HasForeignKey(bc => bc.SpecialtyId);

            modelBuilder.Entity<DarooyabSuperService>()
          .HasKey(bc => new { bc.ServiceId, bc.SuperSpecialityId });
            modelBuilder.Entity<DarooyabSuperService>()
                .HasOne(bc => bc.DarooyabService)
                .WithMany(b => b.DarooyabSuperServices)
                .HasForeignKey(bc => bc.ServiceId);
            modelBuilder.Entity<DarooyabSuperService>()
                .HasOne(bc => bc.DarooyabSuperSpecialty)
                .WithMany(c => c.DarooyabSuperServices)
                .HasForeignKey(bc => bc.SuperSpecialityId);

            modelBuilder.Entity<DarooyabGeneralService>()
                   .HasKey(bc => new { bc.ServiceId, bc.GeneralId });
            modelBuilder.Entity<DarooyabGeneralService>()
                .HasOne(bc => bc.DarooyabService)
                .WithMany(b => b.DarooyabGeneralServices)
                .HasForeignKey(bc => bc.ServiceId);
            modelBuilder.Entity<DarooyabGeneralService>()
                .HasOne(bc => bc.DarooyabGeneralItem)
                .WithMany(c => c.DarooyabGeneralServices)
                .HasForeignKey(bc => bc.GeneralId);

            modelBuilder.Entity<DarooyabProvinceService>()
                   .HasKey(bc => new { bc.ServiceId, bc.ProvinceId });
            modelBuilder.Entity<DarooyabProvinceService>()
                .HasOne(bc => bc.DarooyabService)
                .WithMany(b => b.DarooyabProvinceServices)
                .HasForeignKey(bc => bc.ServiceId);
            modelBuilder.Entity<DarooyabProvinceService>()
                .HasOne(bc => bc.DarooyabProvince)
                .WithMany(c => c.DarooyabProvinceServices)
                .HasForeignKey(bc => bc.ProvinceId);


            modelBuilder.Entity<DarooyabServiceRegister>()
                   .HasKey(bc => new { bc.ServiceId, bc.UserId });
            modelBuilder.Entity<DarooyabServiceRegister>()
                .HasOne(bc => bc.DarooyabService)
                .WithMany(b => b.DarooyabServiceRegisters)
                .HasForeignKey(bc => bc.ServiceId);
            modelBuilder.Entity<DarooyabServiceRegister>()
                .HasOne(bc => bc.DarooyabUser)
                .WithMany(c => c.DarooyabServiceRegisters)
                .HasForeignKey(bc => bc.UserId);





            modelBuilder.Entity<DarooyabCityService>()
                   .HasKey(bc => new { bc.ServiceId, bc.CityId });
            modelBuilder.Entity<DarooyabCityService>()
                .HasOne(bc => bc.DarooyabService)
                .WithMany(b => b.DarooyabCityServices)
                .HasForeignKey(bc => bc.ServiceId);
            modelBuilder.Entity<DarooyabCityService>()
                .HasOne(bc => bc.DarooyabCity)
                .WithMany(c => c.DarooyabCityServices)
                .HasForeignKey(bc => bc.CityId);

            modelBuilder.Entity<DarooyabMessage>(entity =>
                {
                    entity.HasKey(e => e.Id);
                    entity.Property(p => p.Id).ValueGeneratedOnAdd();
                    entity.HasOne(d => d.MessageParent)
                   .WithMany(p => p.MessageChildrens)
                   .HasForeignKey(d => d.ParentId).OnDelete(DeleteBehavior.NoAction);
                    entity.HasOne(d => d.ReceiveUser)
                    .WithMany(p => p.ReceiveMessages)
                    .HasForeignKey(d => d.UserRecieveId).OnDelete(DeleteBehavior.NoAction);
                    entity.HasOne(d => d.SenderUser)
                    .WithMany(p => p.SendMessages)
                    .HasForeignKey(d => d.UserSenderId).OnDelete(DeleteBehavior.NoAction);

                });


            //=======================Darooyab
            modelBuilder.Entity<DarooyabSpecialtySuperSpecialty>()
                .HasKey(bc => new { bc.SpecialtyId, bc.SuperSpecialtyId });
            modelBuilder.Entity<DarooyabSpecialtySuperSpecialty>()
                .HasOne(bc => bc.DarooyabSpecialty)
                .WithMany(b => b.DarooyabSpecialtySuperSpecialties)
                .HasForeignKey(bc => bc.SpecialtyId);
            modelBuilder.Entity<DarooyabSpecialtySuperSpecialty>()
                .HasOne(bc => bc.DarooyabSuperSpecialty)
                .WithMany(c => c.DarooyabSpecialtySuperSpecialties)
                .HasForeignKey(bc => bc.SuperSpecialtyId);

            modelBuilder.Entity<DarooyabActivity>()
                            .HasOne(s => s.DarooyabSpecialty) // Mark Address property optional in Student entity
                            .WithOne(ad => ad.DarooyabActivity).HasForeignKey<DarooyabActivity>(c => c.SpecialtyId).IsRequired(false);

            modelBuilder.Entity<DarooyabUserAddress>()
                        .HasOne(bc => bc.DarooyabUser)
                        .WithMany(c => c.DarooyabUserAddresses)
                        .HasForeignKey(bc => bc.UserId).OnDelete(DeleteBehavior.NoAction);





            modelBuilder.Entity<AdminUser>(entity =>
            {
                entity.ToTable("AdminUser");

                entity.Property(e => e.Family).HasMaxLength(50);

                entity.Property(e => e.Name).HasMaxLength(50);

                entity.Property(e => e.Password).HasMaxLength(50);

                entity.Property(e => e.Phone).HasMaxLength(50);

                entity.Property(e => e.UserName).HasMaxLength(50);
            });

            modelBuilder.Entity<Advice>(entity =>
            {
                entity.HasNoKey();

                entity.ToView("Advice");

                entity.Property(e => e.DosageFormName).HasMaxLength(150);

                entity.Property(e => e.DosageFormNameFa)
                    .HasMaxLength(150)
                    .HasColumnName("DosageFormName_Fa");

                entity.Property(e => e.PatientAdviceAddDate)
                    .HasColumnType("date")
                    .HasColumnName("PatientAdvice_AddDate");
            });

            modelBuilder.Entity<Ahfsgroup>(entity =>
            {
                entity.ToTable("AHFSGroup");

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.Ahfscode)
                    .HasMaxLength(20)
                    .HasColumnName("AHFSCode");

                entity.Property(e => e.Name).HasMaxLength(250);

                entity.Property(e => e.NameFa)
                    .HasMaxLength(350)
                    .HasColumnName("Name_Fa");

                entity.Property(e => e.Version).HasMaxLength(50);
            });

            modelBuilder.Entity<Toxication>(entity =>
            {
                entity.ToTable("Toxication");
                entity.Property(e => e.Id).HasColumnName("Toxication_ID");
                entity.HasKey(e => e.Id)
                 .HasName("Toxication_ID")
                 .IsClustered(false);
                entity.Property(e => e.ToxicationSubject).HasColumnName("Toxication_Subject").HasMaxLength(500);
                entity.Property(e => e.ToxicationContent).HasColumnName("Toxication_Content");
                entity.Property(e => e.InsertDate).HasColumnName("InsertDate");
                entity.Property(e => e.EditDate).HasColumnName("EditDate");

            });
            modelBuilder.Entity<DarooyabHistory>(entity =>
            {
                entity.ToTable("DarooyabHistories");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).HasColumnName("Id");
                entity.Property(e => e.DrugId).HasColumnName("DrugId");
                entity.Property(e => e.ActionType).HasColumnName("ActionType");
                entity.Property(e => e.UserId).HasColumnName("UserId");
                entity.Property(e => e.CreateDate).HasColumnName("CreateDate");
                entity.Property(e => e.LogSourceType).HasColumnName("LogSourceType");

                //    entity.HasOne(d => d.DarooyabUser)
                //         .WithMany(p => p.DarooyabHistories)
                //         .HasForeignKey(d => d.UserId)
                //         .HasConstraintName("FK_DarooyabHistory_DarooyabUsers");

            });


            modelBuilder.Entity<AppNewsViewLog>(entity =>
            {
                entity.ToTable("AppNewsViewLog");

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.ActivityIdFk).HasColumnName("ActivityID_FK");

                entity.Property(e => e.DateTimeView).HasColumnType("datetime");

                entity.Property(e => e.NewsIdFk).HasColumnName("NewsID_FK");

                entity.Property(e => e.ProvinceIdFk).HasColumnName("ProvinceID_FK");

                entity.Property(e => e.SpecialtyIdFk).HasColumnName("SpecialtyID_FK");

                entity.HasOne(d => d.ActivityIdFkNavigation)
                    .WithMany(p => p.AppNewsViewLogs)
                    .HasForeignKey(d => d.ActivityIdFk)
                    .HasConstraintName("FK_NewsViewLog_Category");

                entity.HasOne(d => d.NewsIdFkNavigation)
                    .WithMany(p => p.AppNewsViewLogs)
                    .HasForeignKey(d => d.NewsIdFk)
                    .HasConstraintName("FK_NewsViewLog_DarooyabApp_News");
            });

            modelBuilder.Entity<AppReg>(entity =>
            {
                entity.HasNoKey();

                entity.ToView("app_reg", "portal");

                entity.Property(e => e.ActivityId).HasColumnName("ActivityID");

                entity.Property(e => e.AndroidId)
                    .HasMaxLength(100)
                    .HasColumnName("AndroidID");

                entity.Property(e => e.Description).HasMaxLength(250);

                entity.Property(e => e.Email).HasMaxLength(250);

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.LastUpdateId).HasColumnName("LastUpdateID");

                entity.Property(e => e.MelliCode).HasMaxLength(50);

                entity.Property(e => e.Mobile).HasMaxLength(50);

                entity.Property(e => e.Name).HasMaxLength(250);

                entity.Property(e => e.NezamCode).HasMaxLength(50);

                entity.Property(e => e.Platform).HasMaxLength(50);

                entity.Property(e => e.PlatformVersion).HasMaxLength(50);

                entity.Property(e => e.SpecialityId).HasColumnName("SpecialityID");

                entity.Property(e => e.UserId).HasColumnName("UserID");

                entity.Property(e => e.UserName).HasMaxLength(250);

                entity.Property(e => e.استان).HasMaxLength(100);

                entity.Property(e => e.تخصص).HasMaxLength(100);

                entity.Property(e => e.دانشگاه).HasMaxLength(100);

                entity.Property(e => e.فعالیت).HasMaxLength(100);
            });

            modelBuilder.Entity<AppRegView>(entity =>
            {
                entity.HasNoKey();

                entity.ToView("App_reg_view", "udau");

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.MelliCode).HasMaxLength(50);

                entity.Property(e => e.Name).HasMaxLength(250);

                entity.Property(e => e.UserName).HasMaxLength(250);

                entity.Property(e => e.استان).HasMaxLength(100);

                entity.Property(e => e.تخصص)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.فعالیت).HasMaxLength(100);

                entity.Property(e => e.نظام)
                    .IsRequired()
                    .HasMaxLength(50);
            });

            modelBuilder.Entity<AppUsersCompany>(entity =>
            {
                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.AndroidId)
                    .IsRequired()
                    .HasMaxLength(30)
                    .IsUnicode(false)
                    .HasColumnName("AndroidID")
                    .HasComment("اندروید ای دی");

                entity.Property(e => e.FactoryId)
                    .HasColumnName("FactoryID")
                    .HasComment("کد شرکت");

                entity.Property(e => e.MobileNum)
                    .IsRequired()
                    .HasMaxLength(11)
                    .IsUnicode(false)
                    .IsFixedLength()
                    .HasComment("شماره موبایل");

                entity.Property(e => e.NactionalCode)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .IsFixedLength()
                    .HasComment("کد ملی");

                entity.Property(e => e.Province).HasComment("کد استان/شعبه");

                entity.Property(e => e.RegistrationDate)
                    .HasColumnType("datetime")
                    .HasComment("تاریخ ثبت نام");

                entity.Property(e => e.UserNameFamily)
                    .IsRequired()
                    .HasMaxLength(100)
                    .HasComment("نام و نام خانوادگی کاربر");

                entity.HasOne(d => d.Factory)
                    .WithMany(p => p.AppUsersCompanies)
                    .HasForeignKey(d => d.FactoryId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_AppUsersCompanies_Factorys");

                entity.HasOne(d => d.ProvinceNavigation)
                    .WithMany(p => p.AppUsersCompanies)
                    .HasForeignKey(d => d.Province)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_AppUsersCompanies_Category");
            });

            modelBuilder.Entity<AspnetApplication>(entity =>
            {
                entity.HasKey(e => e.ApplicationId)
                    .HasName("PK__aspnet_A__C93A4C9859C55456")
                    .IsClustered(false);

                entity.ToTable("aspnet_Applications");

                entity.HasIndex(e => e.LoweredApplicationName, "UQ__aspnet_A__17477DE45CA1C101")
                    .IsUnique();

                entity.HasIndex(e => e.ApplicationName, "UQ__aspnet_A__309103315F7E2DAC")
                    .IsUnique();

                entity.HasIndex(e => e.LoweredApplicationName, "aspnet_Applications_Index")
                    .IsClustered();

                entity.Property(e => e.ApplicationId).HasDefaultValueSql("(newid())");

                entity.Property(e => e.ApplicationName)
                    .IsRequired()
                    .HasMaxLength(256);

                entity.Property(e => e.Description).HasMaxLength(256);

                entity.Property(e => e.LoweredApplicationName)
                    .IsRequired()
                    .HasMaxLength(256);
            });

            modelBuilder.Entity<AspnetMembership>(entity =>
            {
                entity.HasKey(e => e.UserId)
                    .HasName("PK__aspnet_M__1788CC4D7849DB76")
                    .IsClustered(false);

                entity.ToTable("aspnet_Membership");

                entity.HasIndex(e => new { e.ApplicationId, e.LoweredEmail }, "aspnet_Membership_index")
                    .IsClustered();

                entity.Property(e => e.UserId).ValueGeneratedNever();

                entity.Property(e => e.Comment).HasColumnType("ntext");

                entity.Property(e => e.CreateDate).HasColumnType("datetime");

                entity.Property(e => e.Email).HasMaxLength(256);

                entity.Property(e => e.FailedPasswordAnswerAttemptWindowStart).HasColumnType("datetime");

                entity.Property(e => e.FailedPasswordAttemptWindowStart).HasColumnType("datetime");

                entity.Property(e => e.LastLockoutDate).HasColumnType("datetime");

                entity.Property(e => e.LastLoginDate).HasColumnType("datetime");

                entity.Property(e => e.LastPasswordChangedDate).HasColumnType("datetime");

                entity.Property(e => e.LoweredEmail).HasMaxLength(256);

                entity.Property(e => e.MobilePin)
                    .HasMaxLength(16)
                    .HasColumnName("MobilePIN");

                entity.Property(e => e.Password)
                    .IsRequired()
                    .HasMaxLength(128);

                entity.Property(e => e.PasswordAnswer).HasMaxLength(128);

                entity.Property(e => e.PasswordQuestion).HasMaxLength(256);

                entity.Property(e => e.PasswordSalt)
                    .IsRequired()
                    .HasMaxLength(128);

                entity.HasOne(d => d.Application)
                    .WithMany(p => p.AspnetMemberships)
                    .HasForeignKey(d => d.ApplicationId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__aspnet_Me__Appli__7A3223E8");

                entity.HasOne(d => d.User)
                    .WithOne(p => p.AspnetMembership)
                    .HasForeignKey<AspnetMembership>(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__aspnet_Me__UserI__7B264821");
            });

            modelBuilder.Entity<AspnetPath>(entity =>
            {
                entity.HasKey(e => e.PathId)
                    .HasName("PK__aspnet_P__CD67DC58318258D2")
                    .IsClustered(false);

                entity.ToTable("aspnet_Paths");

                entity.HasIndex(e => new { e.ApplicationId, e.LoweredPath }, "aspnet_Paths_index")
                    .IsUnique()
                    .IsClustered();

                entity.Property(e => e.PathId).HasDefaultValueSql("(newid())");

                entity.Property(e => e.LoweredPath)
                    .IsRequired()
                    .HasMaxLength(256);

                entity.Property(e => e.Path)
                    .IsRequired()
                    .HasMaxLength(256);

                entity.HasOne(d => d.Application)
                    .WithMany(p => p.AspnetPaths)
                    .HasForeignKey(d => d.ApplicationId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__aspnet_Pa__Appli__336AA144");
            });

            modelBuilder.Entity<AspnetPersonalizationAllUser>(entity =>
            {
                entity.HasKey(e => e.PathId)
                    .HasName("PK__aspnet_P__CD67DC5939237A9A");

                entity.ToTable("aspnet_PersonalizationAllUsers");

                entity.Property(e => e.PathId).ValueGeneratedNever();

                entity.Property(e => e.LastUpdatedDate).HasColumnType("datetime");

                entity.Property(e => e.PageSettings)
                    .IsRequired()
                    .HasColumnType("image");

                entity.HasOne(d => d.Path)
                    .WithOne(p => p.AspnetPersonalizationAllUser)
                    .HasForeignKey<AspnetPersonalizationAllUser>(d => d.PathId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__aspnet_Pe__PathI__3B0BC30C");
            });

            modelBuilder.Entity<AspnetPersonalizationPerUser>(entity =>
            {
                entity.HasKey(e => e.Id)
                    .HasName("PK__aspnet_P__3214EC063DE82FB7")
                    .IsClustered(false);

                entity.ToTable("aspnet_PersonalizationPerUser");

                entity.HasIndex(e => new { e.PathId, e.UserId }, "aspnet_PersonalizationPerUser_index1")
                    .IsUnique()
                    .IsClustered();

                entity.HasIndex(e => new { e.UserId, e.PathId }, "aspnet_PersonalizationPerUser_ncindex2")
                    .IsUnique();

                entity.Property(e => e.Id).HasDefaultValueSql("(newid())");

                entity.Property(e => e.LastUpdatedDate).HasColumnType("datetime");

                entity.Property(e => e.PageSettings)
                    .IsRequired()
                    .HasColumnType("image");

                entity.HasOne(d => d.Path)
                    .WithMany(p => p.AspnetPersonalizationPerUsers)
                    .HasForeignKey(d => d.PathId)
                    .HasConstraintName("FK__aspnet_Pe__PathI__40C49C62");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.AspnetPersonalizationPerUsers)
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("FK__aspnet_Pe__UserI__41B8C09B");
            });

            modelBuilder.Entity<AspnetProfile>(entity =>
            {
                entity.HasKey(e => e.UserId)
                    .HasName("PK__aspnet_P__1788CC4C0F2D40CE");

                entity.ToTable("aspnet_Profile");

                entity.Property(e => e.UserId).ValueGeneratedNever();

                entity.Property(e => e.LastUpdatedDate).HasColumnType("datetime");

                entity.Property(e => e.PropertyNames)
                    .IsRequired()
                    .HasColumnType("ntext");

                entity.Property(e => e.PropertyValuesBinary)
                    .IsRequired()
                    .HasColumnType("image");

                entity.Property(e => e.PropertyValuesString)
                    .IsRequired()
                    .HasColumnType("ntext");

                entity.HasOne(d => d.User)
                    .WithOne(p => p.AspnetProfile)
                    .HasForeignKey<AspnetProfile>(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__aspnet_Pr__UserI__11158940");
            });

            modelBuilder.Entity<AspnetRole>(entity =>
            {
                entity.HasKey(e => e.RoleId)
                    .HasName("PK__aspnet_R__8AFACE1B1A9EF37A")
                    .IsClustered(false);

                entity.ToTable("aspnet_Roles");

                entity.HasIndex(e => new { e.ApplicationId, e.LoweredRoleName }, "aspnet_Roles_index1")
                    .IsUnique()
                    .IsClustered();

                entity.Property(e => e.RoleId).HasDefaultValueSql("(newid())");

                entity.Property(e => e.Description).HasMaxLength(256);

                entity.Property(e => e.LoweredRoleName)
                    .IsRequired()
                    .HasMaxLength(256);

                entity.Property(e => e.RoleName)
                    .IsRequired()
                    .HasMaxLength(256);

                entity.HasOne(d => d.Application)
                    .WithMany(p => p.AspnetRoles)
                    .HasForeignKey(d => d.ApplicationId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__aspnet_Ro__Appli__1C873BEC");
            });

            modelBuilder.Entity<AspnetSchemaVersion>(entity =>
            {
                entity.HasKey(e => new { e.Feature, e.CompatibleSchemaVersion })
                    .HasName("PK__aspnet_S__5A1E6BC16BE40491");

                entity.ToTable("aspnet_SchemaVersions");

                entity.Property(e => e.Feature).HasMaxLength(128);

                entity.Property(e => e.CompatibleSchemaVersion).HasMaxLength(128);
            });

            modelBuilder.Entity<AspnetUser>(entity =>
            {
                entity.HasKey(e => e.UserId)
                    .HasName("PK__aspnet_U__1788CC4D6442E2C9")
                    .IsClustered(false);

                entity.ToTable("aspnet_Users");

                entity.HasIndex(e => new { e.ApplicationId, e.LoweredUserName }, "aspnet_Users_Index")
                    .IsUnique()
                    .IsClustered();

                entity.HasIndex(e => new { e.ApplicationId, e.LastActivityDate }, "aspnet_Users_Index2");

                entity.Property(e => e.UserId).HasDefaultValueSql("(newid())");

                entity.Property(e => e.LastActivityDate).HasColumnType("datetime");

                entity.Property(e => e.LoweredUserName)
                    .IsRequired()
                    .HasMaxLength(256);

                entity.Property(e => e.MobileAlias).HasMaxLength(16);

                entity.Property(e => e.UserName)
                    .IsRequired()
                    .HasMaxLength(256);

                entity.HasOne(d => d.Application)
                    .WithMany(p => p.AspnetUsers)
                    .HasForeignKey(d => d.ApplicationId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__aspnet_Us__Appli__662B2B3B");

                entity.HasMany(d => d.Roles)
                    .WithMany(p => p.Users)
                    .UsingEntity<Dictionary<string, object>>(
                        "AspnetUsersInRole",
                        l => l.HasOne<AspnetRole>().WithMany().HasForeignKey("RoleId").OnDelete(DeleteBehavior.ClientSetNull).HasConstraintName("FK__aspnet_Us__RoleI__2334397B"),
                        r => r.HasOne<AspnetUser>().WithMany().HasForeignKey("UserId").OnDelete(DeleteBehavior.ClientSetNull).HasConstraintName("FK__aspnet_Us__UserI__22401542"),
                        j =>
                        {
                            j.HasKey("UserId", "RoleId").HasName("PK__aspnet_U__AF2760AD2057CCD0");

                            j.ToTable("aspnet_UsersInRoles");

                            j.HasIndex(new[] { "RoleId" }, "aspnet_UsersInRoles_index");
                        });
            });

            modelBuilder.Entity<AspnetWebEventEvent>(entity =>
            {
                entity.HasKey(e => e.EventId)
                    .HasName("PK__aspnet_W__7944C81051EF2864");

                entity.ToTable("aspnet_WebEvent_Events");

                entity.Property(e => e.EventId)
                    .HasMaxLength(32)
                    .IsUnicode(false)
                    .IsFixedLength();

                entity.Property(e => e.ApplicationPath).HasMaxLength(256);

                entity.Property(e => e.ApplicationVirtualPath).HasMaxLength(256);

                entity.Property(e => e.Details).HasColumnType("ntext");

                entity.Property(e => e.EventOccurrence).HasColumnType("decimal(19, 0)");

                entity.Property(e => e.EventSequence).HasColumnType("decimal(19, 0)");

                entity.Property(e => e.EventTime).HasColumnType("datetime");

                entity.Property(e => e.EventTimeUtc).HasColumnType("datetime");

                entity.Property(e => e.EventType)
                    .IsRequired()
                    .HasMaxLength(256);

                entity.Property(e => e.ExceptionType).HasMaxLength(256);

                entity.Property(e => e.MachineName)
                    .IsRequired()
                    .HasMaxLength(256);

                entity.Property(e => e.Message).HasMaxLength(1024);

                entity.Property(e => e.RequestUrl).HasMaxLength(1024);
            });

            modelBuilder.Entity<BusinessInformation>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("BusinessInformation", "udau");

                entity.Property(e => e.BusinessInformation1)
                    .IsRequired()
                    .HasColumnType("ntext")
                    .HasColumnName("BusinessInformation");

                entity.Property(e => e.Irc).HasColumnName("IRC");
            });

            modelBuilder.Entity<CallNumber>(entity =>
            {
                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.CallTypeIdFk)
                    .HasColumnName("CallTypeID_FK")
                    .HasComment("نوع تماس از جدول category، مثلا mobile یا phone");

                entity.Property(e => e.Value)
                    .HasMaxLength(3000)
                    .IsUnicode(false)
                    .IsFixedLength()
                    .HasComment("مقدار تماس");

                entity.Property(e => e.WebSiteUserAccountIdFk)
                    .HasColumnName("WebSiteUserAccountID_FK")
                    .HasComment("کلید خارجی کاربر");

                entity.HasOne(d => d.CallTypeIdFkNavigation)
                    .WithMany(p => p.CallNumbers)
                    .HasForeignKey(d => d.CallTypeIdFk)
                    .HasConstraintName("FK_CallNumbers_Category");

                entity.HasOne(d => d.WebSiteUserAccountIdFkNavigation)
                    .WithMany(p => p.CallNumbers)
                    .HasForeignKey(d => d.WebSiteUserAccountIdFk)
                    .HasConstraintName("FK_CallNumbers_WebSiteUserAccount");
            });

            modelBuilder.Entity<CategoriesInRole>(entity =>
            {
                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.CatId).HasColumnName("CatID");
            });

            modelBuilder.Entity<Category>(entity =>
            {
                entity.HasKey(e => e.CatId);

                entity.ToTable("Category");

                entity.Property(e => e.CatId).HasColumnName("CatID");

                entity.Property(e => e.LogoUrl)
                    .HasMaxLength(500)
                    .IsUnicode(false)
                    .HasColumnName("LogoURL");

                entity.Property(e => e.Name).HasMaxLength(100);

                entity.Property(e => e.ParentId).HasColumnName("ParentID");

                entity.Property(e => e.ReferenceId).HasColumnName("ReferenceID");

                entity.Property(e => e.Type).HasMaxLength(50);
            });

            modelBuilder.Entity<Comment>(entity =>
            {
                entity.ToTable("Comment");

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.CommentForWebSiteAccountUserFk).HasColumnName("CommentFor_webSiteAccountUser_FK");

                entity.Property(e => e.CommentText)
                    .HasComment("متن نظر ");

                entity.Property(e => e.DrugIdFk).HasColumnName("DrugID_FK");

                entity.Property(e => e.Email)
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasComment("ایمیل ثبت کننده نظر");

                entity.Property(e => e.InsertDateTime)
                    .HasColumnType("datetime")
                    .HasComment("تاریخ و ساعت ثبت نظر");

                entity.Property(e => e.IrcFk).HasColumnName("IRC_FK");

                entity.Property(e => e.IsAproved).HasComment("نمایش داده شود/بولین");

                entity.Property(e => e.NameFamily)
                    .HasMaxLength(500)
                    .HasComment("نام و نام خانوادگی ثبت کننده نظر");

                entity.Property(e => e.ParentId)
                    .HasColumnName("ParentID")
                    .HasComment("کد نظر اصلی-جهت ثبت جواب/ریپلای");

                entity.Property(e => e.ReferredbyWebSiteAccountUserFk).HasColumnName("Referredby_webSiteAccountUser_FK");

                entity.Property(e => e.RespByDoctorIdFk).HasColumnName("RespByDoctorID_FK");

                entity.Property(e => e.Tel)
                    .HasMaxLength(50)
                    .HasComment("تلفن ثبت کننده نظر");

                entity.HasOne(d => d.CommentForWebSiteAccountUserFkNavigation)
                    .WithMany(p => p.CommentCommentForWebSiteAccountUserFkNavigations)
                    .HasForeignKey(d => d.CommentForWebSiteAccountUserFk)
                    .HasConstraintName("FK_Comment_WebSiteUserAccount1");

                entity.HasOne(d => d.Parent)
                    .WithMany(p => p.InverseParent)
                    .HasForeignKey(d => d.ParentId)
                    .HasConstraintName("FK_comment_comment");

                entity.HasOne(d => d.ReferredbyWebSiteAccountUserFkNavigation)
                    .WithMany(p => p.CommentReferredbyWebSiteAccountUserFkNavigations)
                    .HasForeignKey(d => d.ReferredbyWebSiteAccountUserFk)
                    .HasConstraintName("FK_Comment_WebSiteUserAccount2");

                entity.HasOne(d => d.RespByDoctorIdFkNavigation)
                    .WithMany(p => p.CommentRespByDoctorIdFkNavigations)
                    .HasForeignKey(d => d.RespByDoctorIdFk)
                    .HasConstraintName("FK_Comment_WebSiteUserAccount");
            });

            modelBuilder.Entity<CommertialBrand>(entity =>
            {
                entity.HasNoKey();

                entity.ToView("CommertialBrands");

                entity.Property(e => e.DrugId).HasColumnName("DrugID");

                entity.Property(e => e.DrugName)
                    .IsRequired()
                    .HasMaxLength(300);

                entity.Property(e => e.FactoryId).HasColumnName("FactoryID");

                entity.Property(e => e.FactoryImporterId).HasColumnName("FactoryImporterID");

                entity.Property(e => e.FactoryProducterId).HasColumnName("FactoryProducterID");

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.ImporterBy)
                    .HasMaxLength(50)
                    .HasColumnName("importerBy");

                entity.Property(e => e.ImporterCompanyId).HasColumnName("ImporterCompanyID");

                entity.Property(e => e.Irc).HasColumnName("IRC");

                entity.Property(e => e.PersTranslateDrugName)
                    .HasMaxLength(300)
                    .HasColumnName("PersTranslate_DrugName");

                entity.Property(e => e.ProducingCountryId).HasColumnName("ProducingCountryID");

                entity.Property(e => e.ProductBy).HasMaxLength(50);

                entity.Property(e => e.ProductInCountries)
                    .HasMaxLength(10)
                    .IsFixedLength();

                entity.Property(e => e.TblBrandAttributeRank).HasColumnName("TBL_BrandAttribute_Rank");
            });

            modelBuilder.Entity<CounterUserView>(entity =>
            {
                entity.HasKey(e => e.CounterView);

                entity.ToTable("CounterUserView");

                entity.Property(e => e.CounterView).HasColumnType("decimal(18, 0)");
            });

            modelBuilder.Entity<Country>(entity =>
            {
                entity.Property(e => e.CountryId)
                    .ValueGeneratedOnAdd()
                    .HasColumnName("Country_ID");

                entity.Property(e => e.CountryName)
                    .IsRequired()
                    .HasMaxLength(30)
                    .HasColumnName("Country_Name");

                entity.Property(e => e.CountryNameFa)
                    .HasMaxLength(10)
                    .HasColumnName("Country_NameFa")
                    .IsFixedLength();
            });

            modelBuilder.Entity<DarooyabAppNews>(entity =>
            {
                entity.ToTable("DarooyabApp_News");

                entity.Property(e => e.Id)
                    .HasColumnName("ID")
                    .HasComment("کد خبر");

                entity.Property(e => e.ChangeDate).HasColumnType("date");

                entity.Property(e => e.ClipUrl)
                    .HasMaxLength(3000)
                    .HasColumnName("ClipURL");

                entity.Property(e => e.ContentTypeId).HasColumnName("ContentTypeID");

                entity.Property(e => e.Description).HasComment("متن HTML خبر");

                entity.Property(e => e.ImageUrl)
                    .HasMaxLength(3000)
                    .HasColumnName("ImageURL");

                entity.Property(e => e.InsertDate)
                    .HasColumnType("datetime")
                    .HasComment("تاریخ ثبت خبر");

                entity.Property(e => e.Irc).HasColumnName("IRC");

                entity.Property(e => e.LinkUrl).HasMaxLength(3000);

                entity.Property(e => e.LogoUrl)
                    .HasMaxLength(3000)
                    .HasColumnName("LogoURL");

                entity.Property(e => e.NewsTypeId)
                    .HasColumnName("NewsTypeID")
                    .HasComment("کد نوع خبر");

                entity.Property(e => e.RecipientActivities)
                    .HasMaxLength(3000)
                    .IsUnicode(false)
                    .HasComment("کد فعالیت های دریافت کننده خبر");

                entity.Property(e => e.RecipientProvinces)
                    .HasMaxLength(3000)
                    .IsUnicode(false)
                    .HasComment("کد استانهای دریافت کننده خبر");

                entity.Property(e => e.RecipientSpecialties)
                    .HasMaxLength(3000)
                    .IsUnicode(false)
                    .HasComment("کد تخصص های دریافت کننده خبر");

                entity.Property(e => e.ShowDate)
                    .HasColumnType("date")
                    .HasComment("تاریخ شروه نمایش بر روی اپلیکشن");

                entity.Property(e => e.SubTitle)
                    .HasMaxLength(500)
                    .HasComment("زیر عنوان خبر");

                entity.Property(e => e.Title)
                    .IsRequired()
                    .HasMaxLength(1000)
                    .HasComment("عنوان خبر");
            });

            modelBuilder.Entity<DeletedHospitalDosageForm>(entity =>
            {
                entity.ToTable("DeletedHospitalDosageForm");

                entity.Property(e => e.Id)
                    .ValueGeneratedNever()
                    .HasColumnName("ID");

                entity.Property(e => e.AffectedUser).HasMaxLength(250);

                entity.Property(e => e.DeletedDate).HasColumnType("datetime");

                entity.Property(e => e.DosageFormId).HasColumnName("DosageFormID");

                entity.Property(e => e.DrugId).HasColumnName("DrugID");

                entity.Property(e => e.HospitalId).HasColumnName("HospitalID");

                entity.Property(e => e.Irc).HasColumnName("IRC");

                entity.Property(e => e.LastChangeDate).HasColumnType("datetime");

                entity.Property(e => e.UserName).HasMaxLength(250);
            });

            modelBuilder.Entity<Disease>(entity =>
            {
                entity.Property(e => e.DiseaseId)
                    .HasColumnName("DiseaseID")
                    .HasComment("کلید اصلی");

                entity.Property(e => e.DiseaseFullDescription)
                    .IsRequired()
                    .HasComment("توضیحات کامل بیماری");

                entity.Property(e => e.DiseaseNameEn)
                    .HasMaxLength(150)
                    .HasComment("نام انگلیسی بیماری");

                entity.Property(e => e.DiseaseNameFa)
                    .IsRequired()
                    .HasMaxLength(150)
                    .HasComment("نام فارسی بیماری");

                entity.Property(e => e.DiseaseShortDescription)
                    .IsRequired()
                    .HasMaxLength(3000)
                    .HasComment("شرح کوتاه بیماری (Description)");

                entity.Property(e => e.DiseaseTitle)
                    .IsRequired()
                    .HasMaxLength(250)
                    .HasComment("عنوان صفحه (title)");

                entity.Property(e => e.EditDate)
                    .HasColumnType("datetime")
                    .HasComment("تاریخ بروزرسانی");

                entity.Property(e => e.GroupIdFk)
                    .HasColumnName("GroupID_FK")
                    .HasComment("کد گروه مرتبط مارتیندل");

                entity.Property(e => e.InsertDate)
                    .HasColumnType("datetime")
                    .HasComment("تاریخ ایجاد");

                entity.HasOne(d => d.GroupIdFkNavigation)
                    .WithMany(p => p.Diseases)
                    .HasForeignKey(d => d.GroupIdFk)
                    .HasConstraintName("FK_Diseases_Diseases");
            });

            modelBuilder.Entity<DistributorDrug>(entity =>
            {
                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.ClipUrl).HasMaxLength(3000);

                entity.Property(e => e.DrugId).HasColumnName("DrugID");

                entity.Property(e => e.Exist)
                    .HasMaxLength(2)
                    .IsUnicode(false);

                entity.Property(e => e.FactoryId).HasColumnName("FactoryID");

                entity.Property(e => e.Irc).HasColumnName("IRC");

                entity.Property(e => e.RecordId).HasColumnName("RecordID");

                entity.Property(e => e.SoundUrl).HasMaxLength(3000);
            });

            modelBuilder.Entity<DistributorsAgency>(entity =>
            {
                entity.ToTable("DistributorsAgency");

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.Description).HasMaxLength(1000);

                entity.Property(e => e.FactoryId).HasColumnName("FactoryID");

                entity.Property(e => e.ProvinceId).HasColumnName("ProvinceID");

                entity.Property(e => e.Tel).HasMaxLength(100);
            });

            modelBuilder.Entity<DosageForm>(entity =>
            {
                entity.ToTable("DosageForm");
                //entity.HasIndex("DosageFormID");
                entity.Property(e => e.DosageFormId).HasColumnName("DosageFormID");
                // entity.HasKey(e => e.DosageFormId)
                //     .IsClustered(false);


                entity.Property(e => e.Antidote).HasMaxLength(300);

                entity.Property(e => e.DosageFormName).HasMaxLength(150);

                entity.Property(e => e.DosageFormNameFa)
                    .HasMaxLength(150)
                    .HasColumnName("DosageFormName_Fa");

                entity.Property(e => e.DrugId).HasColumnName("DrugID");

                entity.Property(e => e.Emergency)
                    .HasMaxLength(1)
                    .IsUnicode(false);

                entity.Property(e => e.Form).HasMaxLength(50);

                entity.Property(e => e.KhadamatPrice1).HasColumnName("khadamat_Price1");

                entity.Property(e => e.KhadamatPrice2).HasColumnName("khadamat_Price2");

                entity.Property(e => e.KhadamatPrice3).HasColumnName("khadamat_Price3");

                entity.Property(e => e.TaminDirectCode)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("Tamin_Direct_Code");

                entity.Property(e => e.TaminDirectIsAccept).HasColumnName("Tamin_Direct_IsAccept");

                entity.Property(e => e.TaminDirectIsChemotherapy).HasColumnName("Tamin_Direct_IsChemotherapy");

                entity.Property(e => e.TaminDirectIsDentist).HasColumnName("Tamin_Direct_IsDentist");

                entity.Property(e => e.TaminDirectIsErjaDakheli).HasColumnName("Tamin_Direct_IsErjaDakheli");

                entity.Property(e => e.TaminDirectIsHospital).HasColumnName("Tamin_Direct_IsHospital");

                entity.Property(e => e.TaminDirectIsMama).HasColumnName("Tamin_Direct_IsMama");

                entity.Property(e => e.TaminDirectIsRadiologist).HasColumnName("Tamin_Direct_IsRadiologist");

                entity.Property(e => e.TaminDirectIsSite).HasColumnName("Tamin_Direct_IsSite");

                entity.Property(e => e.TaminDirectLevel)
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .HasColumnName("Tamin_Direct_Level")
                    .IsFixedLength();
            });


            modelBuilder.Entity<DarooyabDosageForm>(entity =>
              {
                  entity.ToTable("DarooyabDosageForms");
                  entity.HasKey(x => x.DosageFormId);
                  entity.Property(e => e.DosageFormId).HasColumnName("DosageFormID");

                  entity.Property(e => e.Antidote).HasMaxLength(300);
                  entity.Property(e => e.DosageFormNameEn)
                      .HasMaxLength(150)
                      .HasColumnName("DosageFormNameEn");

                  entity.Property(e => e.DosageFormNameFa)
                      .HasMaxLength(150)
                      .HasColumnName("DosageFormNameFa");

                  entity.Property(e => e.DrugId).HasColumnName("DrugID");

                  entity.Property(e => e.Emergency)
                      .HasMaxLength(1)
                      .IsUnicode(false);

                  entity.Property(e => e.Form).HasMaxLength(50);

                  entity.Property(e => e.KhadamatPrice1).HasColumnName("khadamatPrice1");

                  entity.Property(e => e.KhadamatPrice2).HasColumnName("khadamatPrice2");

                  entity.Property(e => e.KhadamatPrice3).HasColumnName("khadamatPrice3");

                  entity.Property(e => e.TaminDirectCode)
                      .HasMaxLength(10)
                      .IsUnicode(false)
                      .HasColumnName("TaminDirectCode");

                  entity.Property(e => e.TaminDirectIsAccept).HasColumnName("TaminDirectIsAccept");
                  entity.Property(e => e.TaminDirectIsChemotherapy).HasColumnName("TaminDirectIsChemotherapy");

                  entity.Property(e => e.TaminDirectIsDentist).HasColumnName("TaminDirectIsDentist");

                  entity.Property(e => e.TaminDirectIsErjaDakheli).HasColumnName("TaminDirectIsErjaDakheli");

                  entity.Property(e => e.TaminDirectIsHospital).HasColumnName("TaminDirectIsHospital");

                  entity.Property(e => e.TaminDirectIsMama).HasColumnName("TaminDirectIsMama");

                  entity.Property(e => e.TaminDirectIsRadiologist).HasColumnName("TaminDirectIsRadiologist");

                  entity.Property(e => e.TaminDirectIsSite).HasColumnName("TaminDirectIsSite");

                  entity.Property(e => e.TaminDirectLevel)
                      .HasMaxLength(1)
                      .IsUnicode(false)
                      .HasColumnName("TaminDirectLevel")
                      .IsFixedLength();
              });



            modelBuilder.Entity<DosageFormAttribute>(entity =>
            {
                entity.HasKey(e => e.DosageFormId);

                entity.ToTable("DosageFormAttribute");

                entity.Property(e => e.DosageFormId)
                    .ValueGeneratedNever()
                    .HasColumnName("DosageFormID");

                entity.Property(e => e.PatientAdviceAddDate)
                    .HasColumnType("date")
                    .HasColumnName("PatientAdvice_AddDate");

                entity.Property(e => e.PatientAdviceAddUser)
                    .HasMaxLength(30)
                    .IsUnicode(false)
                    .HasColumnName("PatientAdvice_AddUser")
                    .IsFixedLength();

                entity.Property(e => e.PatientAdviceEditDate)
                    .HasColumnType("date")
                    .HasColumnName("PatientAdvice_EditDate");

                entity.Property(e => e.PatientAdviceEditUser)
                    .HasMaxLength(30)
                    .IsUnicode(false)
                    .HasColumnName("PatientAdvice_EditUser")
                    .IsFixedLength();

                entity.Property(e => e.PatientAdviceIsAccept).HasColumnName("PatientAdvice_IsAccept");

                entity.HasOne(d => d.DosageForm)
                    .WithOne(p => p.DosageFormAttribute)
                    .HasForeignKey<DosageFormAttribute>(d => d.DosageFormId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_DosageFormAttribute_DosageForm");
            });

            modelBuilder.Entity<DosageFormPerSection>(entity =>
            {
                entity.ToTable("DosageFormPerSection");

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.DosageFormId).HasColumnName("DosageFormID");

                entity.Property(e => e.SectionId).HasColumnName("SectionID");
            });

            modelBuilder.Entity<DrugInfo>(entity =>
            {
                entity.HasNoKey();

                entity.ToView("DrugInfo");

                entity.Property(e => e.Bardary)
                    .HasMaxLength(10)
                    .IsFixedLength();

                entity.Property(e => e.DrugId).HasColumnName("DrugID");

                entity.Property(e => e.FarmaAjza).HasColumnName("Farma_Ajza");

                entity.Property(e => e.GenericName).HasMaxLength(100);

                entity.Property(e => e.GroupName).HasMaxLength(100);

                entity.Property(e => e.MavadMoaserProfile).HasColumnName("MavadMoaser_Profile");

                entity.Property(e => e.SubGroupName).HasMaxLength(100);
            });

            modelBuilder.Entity<DrugsGenericInfo>(entity =>
            {
                entity.HasKey(e => e.DrugId)
                    .HasName("PK_DrugInfo");

                entity.ToTable("DrugsGenericInfo");
                entity.Property(e => e.ToxicationIDFK)
                                  .HasColumnName("Toxication_ID_FK");
                entity.Property(e => e.DrugId)
                    .ValueGeneratedNever()
                    .HasColumnName("DrugID");

                entity.Property(e => e.Bardary)
                    .HasMaxLength(10)
                    .IsFixedLength()
                    .HasComment("برسی شود نوع داتده");

                entity.Property(e => e.FarmaAjza)
                    .HasColumnName("Farma_Ajza")
                    .HasComment("  D,Tفارما=برای گروه\r\n H اجزا فراورده=برای گروه");

                entity.Property(e => e.GroupId).HasColumnName("GroupID");

                entity.Property(e => e.LinkId)
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .HasColumnName("LinkID")
                    .IsFixedLength();

                entity.Property(e => e.MavadMoaserProfile)
                    .HasColumnName("MavadMoaser_Profile")
                    .HasComment(" H مواد موثر= برای داروهای گیاهی گروه\r\n T پروفایل=برای داروهای ترجمه گروه");

                entity.Property(e => e.PregnancyCategoryId).HasColumnName("PregnancyCategoryID");

                entity.Property(e => e.SubGroupId)
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .HasColumnName("SubGroupID")
                    .IsFixedLength();

                entity.Property(e => e.Tadakhol).HasComment("");

                entity.HasOne(d => d.PregnancyCategory)
                    .WithMany(p => p.DrugsGenericInfos)
                    .HasForeignKey(d => d.PregnancyCategoryId)
                    .HasConstraintName("FK_DrugsGenericInfo_PregnancyCategory");

                entity.HasOne(d => d.Toxication)
                   .WithMany(p => p.DrugsGenericInfos)
                   .HasForeignKey(d => d.ToxicationIDFK)
                   .HasConstraintName("FK_DrugsGenericInfo_Toxication");

            });

            modelBuilder.Entity<DrugsName>(entity =>
            {
                entity.ToTable("DrugsName");

                entity.HasIndex(e => e.DrugId, "IX_DrugsName_1");

                entity.HasIndex(e => new { e.DrugNameType, e.DrugNameLanguage }, "IX_DrugsName_DrugNameType_DrugNameLanguage");

                entity.HasIndex(e => new { e.Irc, e.FactoryId, e.Rate }, "IX_Mis_DrugsName_57_56");

                entity.HasIndex(e => e.DrugName, "IX_Mis_DrugsName_65_64");

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.DrugId).HasColumnName("DrugID");

                entity.Property(e => e.DrugName)
                    .IsRequired()
                    .HasMaxLength(300);

                entity.Property(e => e.DrugNameType)
                    .IsRequired()
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .IsFixedLength();

                entity.Property(e => e.FactoryId).HasColumnName("FactoryID");

                entity.Property(e => e.FactoryId2).HasColumnName("FactoryID2");

                entity.Property(e => e.ImporterCompanyId).HasColumnName("ImporterCompanyID");

                entity.Property(e => e.Irc).HasColumnName("IRC");

                entity.Property(e => e.IrgenericId).HasColumnName("IRGenericID");

                entity.Property(e => e.ProducingCountryId).HasColumnName("ProducingCountryID");

                entity.Property(e => e.ProducingCountryId2).HasColumnName("ProducingCountryID2");

                entity.HasOne(d => d.Factory)
                    .WithMany(p => p.DrugsNameFactories)
                    .HasForeignKey(d => d.FactoryId)
                    .HasConstraintName("FK_DrugsName_Factorys1");

                entity.HasOne(d => d.FactoryId2Navigation)
                    .WithMany(p => p.DrugsNameFactoryId2Navigations)
                    .HasForeignKey(d => d.FactoryId2)
                    .HasConstraintName("FK_DrugsName_Factorys2");

                entity.HasOne(d => d.ImporterCompany)
                    .WithMany(p => p.DrugsNameImporterCompanies)
                    .HasForeignKey(d => d.ImporterCompanyId)
                    .HasConstraintName("FK_DrugsName_Factorys");

                entity.HasOne(d => d.ProducingCountry)
                    .WithMany(p => p.DrugsNameProducingCountries)
                    .HasForeignKey(d => d.ProducingCountryId)
                    .HasConstraintName("FK_DrugsName_Countries");

                entity.HasOne(d => d.ProducingCountryId2Navigation)
                    .WithMany(p => p.DrugsNameProducingCountryId2Navigations)
                    .HasForeignKey(d => d.ProducingCountryId2)
                    .HasConstraintName("FK_DrugsName_Countries1");
            });

            modelBuilder.Entity<DrugsTarkibInfo>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("DrugsTarkibInfo");

                entity.Property(e => e.DrugId).HasColumnName("DrugID");
            });

            modelBuilder.Entity<DrugsType>(entity =>
            {
                entity.HasKey(e => e.DrugTypeId)
                    .HasName("PK_DrugTypes");

                entity.ToTable("DrugsType");

                entity.Property(e => e.DrugTypeId)
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .HasColumnName("DrugTypeID")
                    .IsFixedLength();

                entity.Property(e => e.DrugType)
                    .IsRequired()
                    .HasMaxLength(30);
            });

            modelBuilder.Entity<Factory>(entity =>
            {
                entity.Property(e => e.FactoryId).HasColumnName("FactoryID");

                entity.Property(e => e.AddrssLink)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.CountryId).HasColumnName("Country_ID");

                entity.Property(e => e.FactoryName).HasMaxLength(50);

                entity.Property(e => e.FactoryTypeId).HasColumnName("FactoryType_ID");

                entity.Property(e => e.TestArabic)
                    .HasMaxLength(50)
                    .HasColumnName("testArabic")
                    .UseCollation("SQL_1xCompat_CP850_CI_AS");

                entity.HasOne(d => d.Country)
                    .WithMany(p => p.Factories)
                    .HasForeignKey(d => d.CountryId)
                    .HasConstraintName("FK_Factorys_Countries");

                entity.HasOne(d => d.FactoryType)
                    .WithMany(p => p.Factories)
                    .HasForeignKey(d => d.FactoryTypeId)
                    .HasConstraintName("FK_Factorys_FactoryTypes");
            });

            modelBuilder.Entity<FactoryType>(entity =>
            {
                entity.Property(e => e.FactoryTypeId)
                    .ValueGeneratedOnAdd()
                    .HasColumnName("FactoryType_ID");

                entity.Property(e => e.FactoryTypeName)
                    .IsRequired()
                    .HasMaxLength(40)
                    .HasColumnName("FactoryType_Name");
            });

            modelBuilder.Entity<GenericInfoAttribute>(entity =>
            {
                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.Ahfscode)
                    .HasMaxLength(20)
                    .HasColumnName("AHFSCode");

                entity.Property(e => e.DrugId).HasColumnName("DrugID");

                entity.Property(e => e.ToxicationIdFk).HasColumnName("Toxication_ID_fk");

                entity.HasOne(d => d.ToxicationIdFkNavigation)
                    .WithMany(p => p.GenericInfoAttributes)
                    .HasForeignKey(d => d.ToxicationIdFk)
                    .HasConstraintName("FK_GenericInfoAttributes_TBL_Toxication");
            });

            modelBuilder.Entity<Group>(entity =>
            {
                entity.ToTable("Group");

                entity.Property(e => e.GroupId)
                    .ValueGeneratedNever()
                    .HasColumnName("GroupID");

                entity.Property(e => e.GroupNameE).HasMaxLength(100);

                entity.Property(e => e.GroupNameF).HasMaxLength(100);
            });

            modelBuilder.Entity<Group1>(entity =>
            {
                entity.HasKey(e => e.GroupId);

                entity.ToTable("Groups");

                entity.Property(e => e.GroupId).HasColumnName("GroupID");

                entity.Property(e => e.GroupName).HasMaxLength(200);

                entity.Property(e => e.GroupNameFa)
                    .HasMaxLength(200)
                    .HasColumnName("GroupName_Fa");
            });

            modelBuilder.Entity<HospitalAccessApproval>(entity =>
            {
                entity.ToTable("HospitalAccessApproval");

                entity.HasIndex(e => e.UserName, "IX_Mis_HospitalAccessApproval_20_19");

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.AffectedUser).HasMaxLength(250);

                entity.Property(e => e.Status).HasMaxLength(50);

                entity.Property(e => e.TimeStamp).HasColumnType("datetime");

                entity.Property(e => e.UserName).HasMaxLength(250);
            });

            modelBuilder.Entity<HospitalDosageForm>(entity =>
            {
                entity.ToTable("HospitalDosageForm");

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.DosageFormId).HasColumnName("DosageFormID");

                entity.Property(e => e.DrugId).HasColumnName("DrugID");

                entity.Property(e => e.HospitalId).HasColumnName("HospitalID");

                entity.Property(e => e.Irc).HasColumnName("IRC");

                entity.Property(e => e.LastChangeDate).HasColumnType("datetime");

                entity.Property(e => e.UserName).HasMaxLength(200);
            });

            modelBuilder.Entity<LinkInfoAddress>(entity =>
            {
                entity.HasKey(e => e.LinkId);

                entity.ToTable("LinkInfoAddress");

                entity.Property(e => e.LinkId)
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .HasColumnName("LinkID")
                    .IsFixedLength();

                entity.Property(e => e.Address)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<LinkInfoId>(entity =>
            {
                entity.HasKey(e => e.DrugId)
                    .HasName("PK_RelationLinkInfoID");

                entity.ToTable("LinkInfoID");

                entity.Property(e => e.DrugId)
                    .ValueGeneratedNever()
                    .HasColumnName("DrugID");

                entity.Property(e => e.LinkInfoId1)
                    .IsRequired()
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .HasColumnName("LinkInfoID")
                    .IsFixedLength();
            });

            modelBuilder.Entity<LogHistory>(entity =>
            {
                entity.ToTable("logHistory");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.AccountIdFk).HasColumnName("accountId_fk");

                entity.Property(e => e.AccountTypeFk).HasColumnName("accountType_fk");

                entity.Property(e => e.LogTypeIdFk).HasColumnName("logTypeId_fk");

                entity.Property(e => e.PharmacyChangeIdFk).HasColumnName("PharmacyChange_ID_FK");

                entity.Property(e => e.RegisterTime)
                    .HasColumnType("datetime")
                    .HasColumnName("registerTime");
            });

            modelBuilder.Entity<News>(entity =>
            {
                entity.Property(e => e.NewsId).HasColumnName("NewsID");

                entity.Property(e => e.Date).HasMaxLength(50);

                entity.Property(e => e.Description).HasColumnType("ntext");

                entity.Property(e => e.NewsType).HasMaxLength(50);

                entity.Property(e => e.Pic).HasMaxLength(300);

                entity.Property(e => e.Title).HasMaxLength(250);
            });

            modelBuilder.Entity<NewsInCategory>(entity =>
            {
                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.CatId).HasColumnName("CatID");

                entity.Property(e => e.NewsId).HasColumnName("NewsID");
            });

            modelBuilder.Entity<Notification>(entity =>
            {
                entity.ToTable("Notification");

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.DarooyabAppNewsIdFk).HasColumnName("DarooyabApp_NewsId_fk");

                entity.Property(e => e.SendTime).HasColumnType("datetime");
            });

            modelBuilder.Entity<OfficeAddress>(entity =>
            {
                entity.ToTable("OfficeAddress");

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.Address)
                    .HasMaxLength(3000)
                    .HasComment("آدرس مطب");

                entity.Property(e => e.CityIdFk).HasColumnName("CityID_FK");

                entity.Property(e => e.OfficeFacilities)
                    .HasMaxLength(4000)
                    .HasComment("امکانات مطب");

                entity.Property(e => e.OfficeType).HasComment("نوع: مطب/ داروخانه");

                entity.Property(e => e.ProvinceIdFk).HasColumnName("ProvinceID_FK");

                entity.Property(e => e.TelNumber1).HasMaxLength(11);

                entity.Property(e => e.TelNumber2).HasMaxLength(11);

                entity.Property(e => e.TelNumber3).HasMaxLength(11);

                entity.Property(e => e.WebSiteUserAccountIdFk)
                    .HasColumnName("WebSiteUserAccountID_FK")
                    .HasComment("کلید خارجی کاربر");

                entity.Property(e => e.WorkingHoursDay)
                    .HasMaxLength(3000)
                    .HasComment("ساعت  و روز کاری مطب");

                entity.HasOne(d => d.CityIdFkNavigation)
                    .WithMany(p => p.OfficeAddressCityIdFkNavigations)
                    .HasForeignKey(d => d.CityIdFk)
                    .HasConstraintName("FK_OfficeAddress_Category1");

                entity.HasOne(d => d.OfficeTypeNavigation)
                    .WithMany(p => p.OfficeAddressOfficeTypeNavigations)
                    .HasForeignKey(d => d.OfficeType)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_OfficeAddress_Category");

                entity.HasOne(d => d.ProvinceIdFkNavigation)
                    .WithMany(p => p.OfficeAddressProvinceIdFkNavigations)
                    .HasForeignKey(d => d.ProvinceIdFk)
                    .HasConstraintName("FK_OfficeAddress_Category2");

                entity.HasOne(d => d.WebSiteUserAccountIdFkNavigation)
                    .WithMany(p => p.OfficeAddresses)
                    .HasForeignKey(d => d.WebSiteUserAccountIdFk)
                    .HasConstraintName("FK_OfficeAddress_WebSiteUserAccount");
            });

            modelBuilder.Entity<Pharmacy>(entity =>
            {
                entity.ToTable("Pharmacy");

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.Address).HasMaxLength(800);

                entity.Property(e => e.CityIdFk).HasColumnName("CityId_fk");

                entity.Property(e => e.Introduction).HasMaxLength(1000);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.PharmacyActivityTypeFromCategoryIdFk)
                    .HasColumnName("PharmacyActivityType_FromCategoryId_fk")
                    .HasComment("|روزانه\r\n|نیمه وقت|\r\nشبانه روزی");

                entity.Property(e => e.PharmacyEngName)
                    .HasMaxLength(30)
                    .IsUnicode(false);

                entity.Property(e => e.PharmacyNatureFromCategoryIdFk).HasColumnName("PharmacyNature_FromCategoryId_fk");

                entity.Property(e => e.PharmacyTypeFromCategoryIdFk).HasColumnName("PharmacyType_FromCategoryId_fk");

                entity.Property(e => e.Phone)
                    .HasMaxLength(15)
                    .IsUnicode(false);

                entity.Property(e => e.PharmacyManagerMobile).HasMaxLength(15);

                entity.Property(e => e.InventoryManagerMobile).HasMaxLength(15);

                entity.Property(e => e.PharmacyTechnicianMobile).HasMaxLength(15);

                entity.Property(e => e.PostalCode)
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.Property(e => e.ProvinceIdFk).HasColumnName("ProvinceId_fk");

                entity.Property(e => e.UniversityIdFk).HasColumnName("UniversityId_fk");

                entity.HasOne(d => d.CityIdFkNavigation)
                    .WithMany(p => p.PharmacyCityIdFkNavigations)
                    .HasForeignKey(d => d.CityIdFk)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Pharmacy_Category1");

                entity.HasOne(d => d.ProvinceIdFkNavigation)
                    .WithMany(p => p.PharmacyProvinceIdFkNavigations)
                    .HasForeignKey(d => d.ProvinceIdFk)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Pharmacy_Category");
            });

            modelBuilder.Entity<PharmacyAccount>(entity =>
            {
                entity.ToTable("PharmacyAccount");

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.AccountExpireDate).HasColumnType("datetime");

                entity.Property(e => e.AccountStatus)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.AdminComment).HasMaxLength(500);

                entity.Property(e => e.FounderEmail).HasMaxLength(50);

                entity.Property(e => e.FounderFamily).HasMaxLength(30);

                entity.Property(e => e.FounderMobileNum).HasMaxLength(11);

                entity.Property(e => e.FounderName)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.HixCode)
                    .HasMaxLength(11)
                    .IsUnicode(false)
                    .IsFixedLength();

                entity.Property(e => e.NationalCode)
                    .HasMaxLength(11)
                    .IsUnicode(false)
                    .IsFixedLength();

                entity.Property(e => e.PasswordHash)
                    .HasMaxLength(256)
                    .IsUnicode(false);

                entity.Property(e => e.PharmacyIdFk).HasColumnName("PharmacyId_fk");

                entity.Property(e => e.PharmacyManagerMobile).HasMaxLength(15);

                entity.Property(e => e.PharmacyManagerName).HasMaxLength(30);

                entity.HasOne(d => d.PharmacyIdFkNavigation)
                    .WithMany(p => p.PharmacyAccounts)
                    .HasForeignKey(d => d.PharmacyIdFk)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_PharmacyAccount_Pharmacy");
            });

            modelBuilder.Entity<PharmacyExchange>(entity =>
            {
                entity.ToTable("PharmacyExchange", table =>
                    table.HasComment("جدول ثبت تبادلات داروخانه ها"));

                entity.Property(e => e.CompanyDistributorIdFk).HasColumnName("CompanyDistributorId_fk");

                entity.Property(e => e.DrugUnitNum)
                    .IsRequired()
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .IsFixedLength();

                entity.Property(e => e.ExcessPhReqIdFk).HasColumnName("ExcessPhReqId_fk");

                entity.Property(e => e.InvoiceNumber)
                    .HasMaxLength(50)
                    .HasComment("شماره فاکتور");

                entity.Property(e => e.PatchNumber).HasMaxLength(50);

                entity.Property(e => e.RegisterDatetime).HasColumnType("datetime");

                entity.Property(e => e.ShortagePhReqIdFk).HasColumnName("ShortagePhReqId_fk");

                entity.HasOne(d => d.ExcessPhReqIdFkNavigation)
                    .WithMany(p => p.PharmacyExchangeExcessPhReqIdFkNavigations)
                    .HasForeignKey(d => d.ExcessPhReqIdFk)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("ExcessPharmacyRequest");

                entity.HasOne(d => d.ShortagePhReqIdFkNavigation)
                    .WithMany(p => p.PharmacyExchangeShortagePhReqIdFkNavigations)
                    .HasForeignKey(d => d.ShortagePhReqIdFk)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("ShortagePharmacyRequest");
            });

            modelBuilder.Entity<PharmacyRequest>(entity =>
            {
                entity.ToTable("PharmacyRequest");

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.AddedByFdoaccountIdFk).HasColumnName("AddedByFDOAccountId_fk");

                entity.Property(e => e.ColleaguePrice).HasComment("قیمت فروش به همکار");

                entity.Property(e => e.CompanyDistributorIdFk).HasColumnName("CompanyDistributorId_fk");

                entity.Property(e => e.CountLimit).HasComment("تعداد محدود - 1/نامحدود - 2");

                entity.Property(e => e.Description).HasMaxLength(1000);

                entity.Property(e => e.DrugExpireDate).HasColumnType("date");

                entity.Property(e => e.DrugShapFromDosageIdFk).HasColumnName("DrugShap_FromDosageId_fk");

                entity.Property(e => e.DrugUnitNum)
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .IsFixedLength();

                entity.Property(e => e.ExchangeLimitNum)
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .IsFixedLength()
                    .HasComment("لوکس-بدون محدودیت");

                entity.Property(e => e.Irc).HasColumnName("IRC");

                entity.Property(e => e.IsHiddenByFdo).HasColumnName("IsHiddenByFDO");

                entity.Property(e => e.PatchNumber).HasMaxLength(50);

                entity.Property(e => e.PationRefferalView)
                    .HasMaxLength(50)
                    .HasComment("ارجاع بیمار-قابل مشاهده برای");

                entity.Property(e => e.PharmacyAccountIdFk).HasColumnName("PharmacyAccountId_fk");

                entity.Property(e => e.PurchaseInvoiceNumber).HasMaxLength(50);

                entity.Property(e => e.RegistrationDateTime).HasColumnType("datetime");

                entity.Property(e => e.RequestExpireDate).HasColumnType("date");

                entity.Property(e => e.RequestTypeFromCategoryIdFk).HasColumnName("RequestType_FromCategoryId_fk");

                entity.Property(e => e.RequestView).HasComment("تبادل دارویی-قابل مشاهده برای");

                entity.Property(e => e.SellToColleague).HasComment("فروش نقدی به همکار");

                entity.Property(e => e.StatusNum)
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .IsFixedLength();

                entity.HasOne(d => d.AddedByFdoaccountIdFkNavigation)
                    .WithMany(p => p.PharmacyRequestAddedByFdoaccountIdFkNavigations)
                    .HasForeignKey(d => d.AddedByFdoaccountIdFk)
                    .HasConstraintName("FK_PharmacyRequest_PharmacyAccount2");

                entity.HasOne(d => d.DrugShapFromDosageIdFkNavigation)
                    .WithMany(p => p.PharmacyRequests)
                    .HasForeignKey(d => d.DrugShapFromDosageIdFk)
                    .HasConstraintName("FK_PharmacyRequest_DosageForm");

                entity.HasOne(d => d.PharmacyAccountIdFkNavigation)
                    .WithMany(p => p.PharmacyRequestPharmacyAccountIdFkNavigations)
                    .HasForeignKey(d => d.PharmacyAccountIdFk)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_PharmacyRequest_PharmacyAccount");

                entity.HasOne(d => d.RequestTypeFromCategoryIdFkNavigation)
                    .WithMany(p => p.PharmacyRequests)
                    .HasForeignKey(d => d.RequestTypeFromCategoryIdFk)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_PharmacyRequest_Category");
            });

            modelBuilder.Entity<PregnancyCategory>(entity =>
            {
                entity.ToTable("PregnancyCategory", "portal");

                entity.Property(e => e.PregnancyCategoryId)
                    .ValueGeneratedOnAdd()
                    .HasColumnName("PregnancyCategoryID");

                entity.Property(e => e.PregnancyCategoryCode)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.PregnancyCategoryDes).HasMaxLength(1000);

                entity.Property(e => e.PregnancyCategoryDesFa).HasColumnName("PregnancyCategoryDes_Fa");
            });

            modelBuilder.Entity<Profile>(entity =>
            {
                entity.ToTable("Profile");

                entity.HasIndex(e => e.UserName, "IX_Mis_Profile_22_21");

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.ActivityId).HasColumnName("ActivityID");

                entity.Property(e => e.Address).HasMaxLength(500);

                entity.Property(e => e.AndroidId)
                    .HasMaxLength(100)
                    .HasColumnName("AndroidID");

                entity.Property(e => e.Description).HasMaxLength(250);

                entity.Property(e => e.Email).HasMaxLength(250);

                entity.Property(e => e.Imie)
                    .HasMaxLength(50)
                    .HasColumnName("IMIE");

                entity.Property(e => e.LastUpdateId).HasColumnName("LastUpdateID");

                entity.Property(e => e.MelliCode).HasMaxLength(50);

                entity.Property(e => e.Mobile).HasMaxLength(50);

                entity.Property(e => e.Name).HasMaxLength(250);

                entity.Property(e => e.NezamCode).HasMaxLength(50);

                entity.Property(e => e.Phone).HasMaxLength(50);

                entity.Property(e => e.Platform).HasMaxLength(50);

                entity.Property(e => e.PlatformVersion).HasMaxLength(50);

                entity.Property(e => e.ProvinceId).HasColumnName("ProvinceID");

                entity.Property(e => e.SpecialityId).HasColumnName("SpecialityID");

                entity.Property(e => e.UniversityId).HasColumnName("UniversityID");

                entity.Property(e => e.UserId).HasColumnName("UserID");

                entity.Property(e => e.UserName).HasMaxLength(250);
            });

            modelBuilder.Entity<RegisteredImei>(entity =>
            {
                entity.ToTable("RegisteredIMEIs");

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.Imei)
                    .HasMaxLength(50)
                    .HasColumnName("IMEI");
            });

            modelBuilder.Entity<RequestMedication>(entity =>
            {
                entity.ToTable("RequestMedication");

                entity.Property(e => e.AcceptedByPharmacyIdFk)
                    .HasColumnName("AcceptedByPharmacyID_FK")
                    .HasComment("پذیرش نسخه توسط داروخانه");

                entity.Property(e => e.AcceptedDate)
                    .HasMaxLength(10)
                    .IsFixedLength()
                    .HasComment("تاریخ پذیرش نسخه توسط داروخانه");

                entity.Property(e => e.CityIdFk).HasColumnName("CityId_fk");

                entity.Property(e => e.DrugPrescriptionImageUrl)
                    .IsRequired()
                    .HasMaxLength(2000);

                entity.Property(e => e.MobileNumber)
                    .IsRequired()
                    .HasMaxLength(11);

                entity.Property(e => e.ProvinceIdFk).HasColumnName("ProvinceId_fk");

                entity.Property(e => e.RegistrationDate)
                    .HasColumnType("datetime")
                    .HasComment("تاریخ ثبت نسخه");

                entity.Property(e => e.RegistrationName)
                    .IsRequired()
                    .HasMaxLength(75)
                    .HasComment("نام و نام خانوادگی ثبت کننده نسخه");

                entity.HasOne(d => d.AcceptedByPharmacyIdFkNavigation)
                    .WithMany(p => p.RequestMedications)
                    .HasForeignKey(d => d.AcceptedByPharmacyIdFk)
                    .HasConstraintName("AcceptedByPharmacyID_FK_To_PharmacyTable_ID");

                entity.HasOne(d => d.CityIdFkNavigation)
                    .WithMany(p => p.RequestMedicationCityIdFkNavigations)
                    .HasForeignKey(d => d.CityIdFk)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("cityId_fk_to_Category_CatId");

                entity.HasOne(d => d.ProvinceIdFkNavigation)
                    .WithMany(p => p.RequestMedicationProvinceIdFkNavigations)
                    .HasForeignKey(d => d.ProvinceIdFk)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("ProvinceId_fk_to_Category_CatId");
            });

            modelBuilder.Entity<Section>(entity =>
            {
                entity.ToTable("Section");

                entity.Property(e => e.SectionId).HasColumnName("SectionID");

                entity.Property(e => e.SectionName).HasMaxLength(200);

                entity.Property(e => e.SectionParentId).HasColumnName("SectionParentID");
            });

            modelBuilder.Entity<SectionPerHospital>(entity =>
            {
                entity.ToTable("SectionPerHospital");

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.HospitalId).HasColumnName("HospitalID");

                entity.Property(e => e.SectionId).HasColumnName("SectionID");
            });

            modelBuilder.Entity<Setting>(entity =>
            {
                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.PropertyName).HasMaxLength(300);

                entity.Property(e => e.PropertyValue).HasMaxLength(350);
            });

            modelBuilder.Entity<SlidInfoTopIrc>(entity =>
            {
                entity.HasKey(e => e.FileId);

                entity.ToTable("SlidInfo_TopIRC");

                entity.Property(e => e.FileId)
                    .HasColumnType("numeric(18, 0)")
                    .ValueGeneratedOnAdd()
                    .HasColumnName("FileID");

                entity.Property(e => e.FileAlternative)
                    .HasMaxLength(250)
                    .HasColumnName("File_Alternative");

                entity.Property(e => e.FileDate)
                    .HasColumnType("datetime")
                    .HasColumnName("File_Date");

                entity.Property(e => e.FileGidFk).HasColumnName("File_GID_FK");

                entity.Property(e => e.FileLink).HasColumnName("File_Link");

                entity.Property(e => e.FileName).HasMaxLength(50);

                entity.Property(e => e.FilePageIdFk).HasColumnName("File_PageID_FK");

                entity.Property(e => e.FileSize).HasColumnName("File_Size");

                entity.Property(e => e.FileTooltip)
                    .HasMaxLength(250)
                    .HasColumnName("File_Tooltip");

                entity.Property(e => e.FileVisible).HasColumnName("File_Visible");
            });

            modelBuilder.Entity<SlidSettingsTopIrc>(entity =>
            {
                entity.HasKey(e => e.SlideImageId);

                entity.ToTable("SlidSettings_TopIRC");

                entity.Property(e => e.SlideImageId)
                    .HasColumnType("numeric(18, 0)")
                    .ValueGeneratedOnAdd()
                    .HasColumnName("Slide_ImageID");

                entity.Property(e => e.SlideActionMode).HasColumnName("Slide_ActionMode");

                entity.Property(e => e.SlideAutoCenter).HasColumnName("Slide_AutoCenter");

                entity.Property(e => e.SlideAutoPlayInterval).HasColumnName("Slide_AutoPlayInterval");

                entity.Property(e => e.SlideChanceToShow).HasColumnName("Slide_ChanceToShow");

                entity.Property(e => e.SlideDragOrientation).HasColumnName("Slide_DragOrientation");

                entity.Property(e => e.SlideHeight).HasColumnName("Slide_Height");

                entity.Property(e => e.SlideIsActive).HasColumnName("Slide_IsActive");

                entity.Property(e => e.SlideLanes).HasColumnName("Slide_Lanes");

                entity.Property(e => e.SlideOrientation).HasColumnName("Slide_Orientation");

                entity.Property(e => e.SlidePageId).HasColumnName("Slide_PageID");

                entity.Property(e => e.SlideSpacingX).HasColumnName("Slide_SpacingX");

                entity.Property(e => e.SlideSpacingY).HasColumnName("Slide_SpacingY");

                entity.Property(e => e.SlideSpeed).HasColumnName("Slide_Speed");

                entity.Property(e => e.SlideSteps).HasColumnName("Slide_Steps");

                entity.Property(e => e.SlideTitle)
                    .HasMaxLength(250)
                    .HasColumnName("Slide_Title");

                entity.Property(e => e.SlideWidth).HasColumnName("Slide_Width");
            });

            modelBuilder.Entity<SpecificInformation>(entity =>
            {
                entity.ToTable("SpecificInformation");

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.ConsumerPrices).HasColumnType("numeric(18, 0)");

                entity.Property(e => e.EntryDate).HasColumnType("datetime");

                entity.Property(e => e.Irc)
                    .HasColumnType("numeric(18, 0)")
                    .HasColumnName("IRC");

                entity.Property(e => e.Keywords).HasMaxLength(1000);

                entity.Property(e => e.MedicalInformation).HasColumnType("ntext");

                entity.Property(e => e.PatientInformation).HasColumnType("ntext");

                entity.Property(e => e.SpecialFeature).HasMaxLength(1000);
            });

            modelBuilder.Entity<SpecificInformationImage>(entity =>
            {
                entity.HasKey(e => e.ImageId);

                entity.ToTable("SpecificInformation_Image");

                entity.Property(e => e.ImageId).HasColumnName("Image_ID");

                entity.Property(e => e.SiEntryDate)
                    .HasColumnType("datetime")
                    .HasColumnName("SI_EntryDate");

                entity.Property(e => e.SiIdFk).HasColumnName("SI_ID_FK");

                entity.Property(e => e.SiImagePath)
                    .HasMaxLength(1000)
                    .HasColumnName("SI_ImagePath");

                entity.Property(e => e.SiImageType).HasColumnName("SI_ImageType");

                entity.Property(e => e.SiIsEnable).HasColumnName("SI_IsEnable");
            });

            modelBuilder.Entity<SubGroup>(entity =>
            {
                entity.HasKey(e => new { e.GroupId, e.SubGroupId });

                entity.Property(e => e.GroupId).HasColumnName("GroupID");

                entity.Property(e => e.SubGroupId)
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .HasColumnName("SubGroupID")
                    .IsFixedLength();

                entity.Property(e => e.SubGroupName)
                    .IsRequired()
                    .HasMaxLength(100);
            });

            modelBuilder.Entity<Suggestion>(entity =>
            {
                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.SentDate).HasColumnType("datetime");

                entity.Property(e => e.Suggestion1).HasColumnName("Suggestion");

                entity.Property(e => e.UserId).HasColumnName("UserID");
            });

            modelBuilder.Entity<Tarkibat>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("Tarkibat");

                entity.Property(e => e.DrugId).HasColumnName("DrugID");
            });

            modelBuilder.Entity<TblAdvertisement>(entity =>
            {
                entity.HasKey(e => e.AdvId);

                entity.ToTable("TBL_Advertisement");

                entity.Property(e => e.AdvId).HasColumnName("Adv_ID");

                entity.Property(e => e.AdvBayer)
                    .HasMaxLength(1000)
                    .HasColumnName("Adv_Bayer");

                entity.Property(e => e.AdvDrugIdFk).HasColumnName("Adv_DrugId_fk");

                entity.Property(e => e.AdvDrugIdSearchFk).HasColumnName("Adv_DrugId_Search_fk");

                entity.Property(e => e.AdvEditDate)
                    .HasColumnType("date")
                    .HasColumnName("Adv_EditDate");

                entity.Property(e => e.AdvEditUser)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("Adv_EditUser");

                entity.Property(e => e.AdvEndDate)
                    .HasColumnType("date")
                    .HasColumnName("Adv_EndDate");

                entity.Property(e => e.AdvFactoryIdFk).HasColumnName("Adv_FactoryId_fk");

                entity.Property(e => e.AdvGroupIdFk).HasColumnName("Adv_GroupId_fk");

                entity.Property(e => e.AdvImageAlt)
                    .HasMaxLength(500)
                    .HasColumnName("Adv_ImageAlt");

                entity.Property(e => e.AdvImageUrl)
                    .HasMaxLength(500)
                    .IsUnicode(false)
                    .HasColumnName("Adv_ImageUrl");

                entity.Property(e => e.AdvIsActive).HasColumnName("Adv_IsActive");

                entity.Property(e => e.AdvIsPublic).HasColumnName("Adv_IsPublic");

                entity.Property(e => e.AdvLink)
                    .HasMaxLength(500)
                    .HasColumnName("Adv_link");

                entity.Property(e => e.AdvRank).HasColumnName("Adv_Rank");

                entity.Property(e => e.AdvSaveUser)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("Adv_SaveUser");

                entity.Property(e => e.AdvStartDate)
                    .HasColumnType("date")
                    .HasColumnName("Adv_StartDate");
            });

            modelBuilder.Entity<TblAttachmentFileToGenericAttribute>(entity =>
            {
                entity.ToTable("TBL_AttachmentFileToGenericAttribute");

                entity.Property(e => e.TblAttachmentFileToGenericAttributeId).HasColumnName("TBL_AttachmentFileToGenericAttribute_Id");

                entity.Property(e => e.TblAttachmentFileToGenericAttributeDescription).HasColumnName("TBL_AttachmentFileToGenericAttribute_Description");

                entity.Property(e => e.TblAttachmentFileToGenericAttributeFileDirUrl)
                    .IsRequired()
                    .HasMaxLength(500)
                    .HasColumnName("TBL_AttachmentFileToGenericAttribute_FileDirUrl");

                entity.Property(e => e.TblGenericPageAttributeIdFk).HasColumnName("TBL_GenericPageAttribute_Id_fk");

                entity.HasOne(d => d.TblGenericPageAttributeIdFkNavigation)
                    .WithMany(p => p.TblAttachmentFileToGenericAttributes)
                    .HasForeignKey(d => d.TblGenericPageAttributeIdFk)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_TBL_AttachmentFileToGenericAttribute_TBL_GenericPageAttribute");
            });

            modelBuilder.Entity<TblBrandAttribute>(entity =>
            {
                entity.ToTable("TBL_BrandAttribute");

                entity.HasIndex(e => e.TblBrandAttributeIrc, "IX_Mis_TBL_BrandAttribute_30_29");

                entity.HasIndex(e => e.TblBrandAttributeId, "Uniq_IRC")
                    .IsUnique();

                entity.Property(e => e.TblBrandAttributeId).HasColumnName("TBL_BrandAttribute_ID");

                entity.Property(e => e.AdvertisingUrl).HasMaxLength(1000);

                entity.Property(e => e.ClipUrl)
                    .HasMaxLength(255)
                    .HasColumnName("clip_url");

                entity.Property(e => e.DrugsNameIdFk).HasColumnName("DrugsName_ID_fk");

                entity.Property(e => e.Image1Url)
                    .HasMaxLength(255)
                    .HasColumnName("image1_url");

                entity.Property(e => e.Image2Url)
                    .HasMaxLength(255)
                    .HasColumnName("image2_url");

                entity.Property(e => e.Image3Url)
                    .HasMaxLength(255)
                    .HasColumnName("image3_url");

                entity.Property(e => e.IsChemotherapy).HasComment("شیمی درمانی است؟");

                entity.Property(e => e.IsHospital).HasComment("دارو بیمارستانی است؟");

                entity.Property(e => e.IsOtc)
                    .HasColumnName("IsOTC")
                    .HasComment("بدون نسخه است؟");

                entity.Property(e => e.IsUnderSupervision).HasComment("تحت نظارت است؟");

                entity.Property(e => e.LastPriceEditDate).HasColumnType("datetime");

                entity.Property(e => e.LastPriceEditorPharmacyFk).HasColumnName("LastPriceEditor_Pharmacy_FK");

                entity.Property(e => e.Price).HasColumnType("decimal(18, 0)");

                entity.Property(e => e.StorageTemperatureId).HasColumnName("StorageTemperatureID");

                entity.Property(e => e.TblBrandAttributeContentEn).HasColumnName("TBL_BrandAttribute_ContentEn");

                entity.Property(e => e.TblBrandAttributeContentFa).HasColumnName("TBL_BrandAttribute_ContentFa");

                entity.Property(e => e.TblBrandAttributeIrc).HasColumnName("TBL_BrandAttribute_IRC");

                entity.Property(e => e.TblBrandAttributePageDescription)
                    .HasMaxLength(2000)
                    .HasColumnName("TBL_BrandAttribute_PageDescription");

                entity.Property(e => e.TblBrandAttributePageTitle)
                    .HasMaxLength(200)
                    .HasColumnName("TBL_BrandAttribute_PageTitle");

                entity.Property(e => e.TblBrandAttributeRank).HasColumnName("TBL_BrandAttribute_Rank");

                entity.Property(e => e.TblBrandAttributeShopUrl)
                    .HasMaxLength(500)
                    .HasColumnName("TBL_BrandAttribute_ShopUrl");

                entity.Property(e => e.TherapeuticApplication).HasComment("کاربرد درمانی");

                entity.Property(e => e.VoiceUrl)
                    .HasMaxLength(255)
                    .HasColumnName("voice_url");

                entity.HasOne(d => d.DrugsNameIdFkNavigation)
                    .WithMany(p => p.TblBrandAttributes)
                    .HasForeignKey(d => d.DrugsNameIdFk)
                    .HasConstraintName("FK_TBL_BrandAttribute_DrugsName");

                entity.HasOne(d => d.StorageTemperature)
                    .WithMany(p => p.TblBrandAttributes)
                    .HasForeignKey(d => d.StorageTemperatureId)
                    .HasConstraintName("FK_TBL_BrandAttribute_Category1");
            });

            modelBuilder.Entity<TblContactu>(entity =>
            {
                entity.HasKey(e => e.TblContactusId);

                entity.ToTable("TBL_Contactus");

                entity.Property(e => e.TblContactusId).HasColumnName("TBL_Contactus_ID");

                entity.Property(e => e.TblContactusCategoryIdFk).HasColumnName("TBL_Contactus_CategoryID_fk");

                entity.Property(e => e.TblContactusEmail)
                    .HasMaxLength(50)
                    .HasColumnName("TBL_Contactus_Email");

                entity.Property(e => e.TblContactusIpsystem)
                    .HasMaxLength(25)
                    .HasColumnName("TBL_Contactus_IPSystem");

                entity.Property(e => e.TblContactusNameAndFamily)
                    .HasMaxLength(255)
                    .HasColumnName("TBL_Contactus_NameAndFamily");

                entity.Property(e => e.TblContactusOrganizationOrCompanyName)
                    .HasMaxLength(255)
                    .HasColumnName("TBL_Contactus_OrganizationOrCompanyName");

                entity.Property(e => e.TblContactusRegisterDate)
                    .HasColumnType("date")
                    .HasColumnName("TBL_Contactus_RegisterDate");

                entity.Property(e => e.TblContactusRole)
                    .HasMaxLength(255)
                    .HasColumnName("TBL_Contactus_Role");

                entity.Property(e => e.TblContactusTel)
                    .HasMaxLength(255)
                    .HasColumnName("TBL_Contactus_Tel");

                entity.Property(e => e.TblContactusTextMessage).HasColumnName("TBL_Contactus_TextMessage");

                entity.Property(e => e.TblContactusViewByManagerDate)
                    .HasColumnType("date")
                    .HasColumnName("TBL_Contactus_ViewByManagerDate");

                entity.Property(e => e.TblContactusViewByManagerStatusNum).HasColumnName("TBL_Contactus_ViewByManagerStatusNum");

                entity.HasOne(d => d.TblContactusCategoryIdFkNavigation)
                    .WithMany(p => p.TblContactus)
                    .HasForeignKey(d => d.TblContactusCategoryIdFk)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_TBL_Contactus_Category");
            });

            modelBuilder.Entity<TblGenericPageAttribute>(entity =>
            {
                entity.ToTable("TBL_GenericPageAttribute");

                entity.Property(e => e.TblGenericPageAttributeId).HasColumnName("TBL_GenericPageAttribute_Id");

                entity.Property(e => e.TblGenericPageAttributeDrugIdFk).HasColumnName("TBL_GenericPageAttribute_DrugId_fk");

                entity.Property(e => e.TblGenericPageAttributeGeneralInformation).HasColumnName("TBL_GenericPageAttribute_GeneralInformation");

                entity.Property(e => e.TblGenericPageAttributePageDescription)
                    .HasMaxLength(1000)
                    .HasColumnName("TBL_GenericPageAttribute_PageDescription");

                entity.Property(e => e.TblGenericPageAttributePageTitle)
                    .HasMaxLength(255)
                    .HasColumnName("TBL_GenericPageAttribute_PageTitle");

                entity.Property(e => e.ToxicationIdFk).HasColumnName("Toxication_ID_fk");

                entity.HasOne(d => d.ToxicationIdFkNavigation)
                    .WithMany(p => p.TblGenericPageAttributes)
                    .HasForeignKey(d => d.ToxicationIdFk)
                    .HasConstraintName("FK_TBL_GenericPageAttribute_TBL_GenericPageAttribute");
            });

            modelBuilder.Entity<TblNews>(entity =>
            {
                entity.ToTable("TBL_News");

                entity.Property(e => e.TblNewsId).HasColumnName("TBL_News_ID");

                entity.Property(e => e.TblCategoryIdFk)
                    .HasColumnName("TBL_Category_ID_fk")
                    .HasComment("شناسه خارجی سازمان");

                entity.Property(e => e.TblDrugNameIrcidFk)
                    .HasColumnName("TBL_DrugName_IRCID_fk")
                    .HasComment("شناسه خارجی برند (نام تجاری)");

                entity.Property(e => e.TblDrugsNameIdFk)
                    .HasColumnName("TBL_DrugsName_ID_fk")
                    .HasComment("شناسه خارجی دارو");

                entity.Property(e => e.TblFactorysIdFk)
                    .HasColumnName("TBL_Factorys_ID_fk")
                    .HasComment("شناسه خارجی شرکت");

                entity.Property(e => e.TblGroupsIdFk)
                    .HasColumnName("TBL_Groups_ID_fk")
                    .HasComment("شناسه خارجی گروه دارویی");

                entity.Property(e => e.TblNewsDoConfirmByManager)
                    .HasColumnName("TBL_News_DoConfirmByManager")
                    .HasComment("مدیر");

                entity.Property(e => e.TblNewsExpireDate)
                    .HasColumnType("date")
                    .HasColumnName("TBL_News_ExpireDate");

                entity.Property(e => e.TblNewsGroupIdFk).HasColumnName("TBL_NewsGroup_Id_fk");

                entity.Property(e => e.TblNewsImageDirectory)
                    .HasMaxLength(255)
                    .HasColumnName("TBL_News_ImageDirectory");

                entity.Property(e => e.TblNewsIsActive).HasColumnName("TBL_News_IsActive");

                entity.Property(e => e.TblNewsIsFavorite)
                    .HasColumnName("TBL_News_IsFavorite")
                    .HasComment("خبر برگزیده است ؟");

                entity.Property(e => e.TblNewsLongDescription).HasColumnName("TBL_News_LongDescription");

                entity.Property(e => e.TblNewsRegisterDate)
                    .HasColumnType("date")
                    .HasColumnName("TBL_News_RegisterDate");

                entity.Property(e => e.TblNewsShortDescription).HasColumnName("TBL_News_ShortDescription");

                entity.Property(e => e.TblNewsSubTitle)
                    .HasMaxLength(100)
                    .HasColumnName("TBL_News_SubTitle");

                entity.Property(e => e.TblNewsTitle)
                    .IsRequired()
                    .HasMaxLength(100)
                    .HasColumnName("TBL_News_Title");

                entity.Property(e => e.TblNewsTypeIsPage).HasColumnName("TBL_News_TypeIsPage");

                entity.HasOne(d => d.TblNewsGroupIdFkNavigation)
                    .WithMany(p => p.TblNews)
                    .HasForeignKey(d => d.TblNewsGroupIdFk)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_TBL_News_TBL_NewsGroup");
            });

            modelBuilder.Entity<TblNewsGroup>(entity =>
            {
                entity.ToTable("TBL_NewsGroup");

                entity.Property(e => e.TblNewsGroupId).HasColumnName("TBL_NewsGroup_ID");

                entity.Property(e => e.TblNewsGroupDescription).HasColumnName("TBL_NewsGroup_Description");

                entity.Property(e => e.TblNewsGroupEngTitle)
                    .IsRequired()
                    .HasMaxLength(100)
                    .HasColumnName("TBL_NewsGroup_EngTitle");

                entity.Property(e => e.TblNewsGroupPersTitle)
                    .HasMaxLength(100)
                    .HasColumnName("TBL_NewsGroup_PersTitle");
            });

            modelBuilder.Entity<TblOrganizationDetail>(entity =>
            {
                entity.ToTable("TBL_OrganizationDetail");

                entity.Property(e => e.TblOrganizationDetailId).HasColumnName("TBL_OrganizationDetail_ID");

                entity.Property(e => e.TblOrganizationDetailAddress)
                    .HasMaxLength(500)
                    .HasColumnName("TBL_OrganizationDetail_Address");

                entity.Property(e => e.TblOrganizationDetailEmail)
                    .HasMaxLength(255)
                    .HasColumnName("TBL_OrganizationDetail_Email");

                entity.Property(e => e.TblOrganizationDetailFactoryIdFk).HasColumnName("TBL_OrganizationDetail_FactoryId_fk");

                entity.Property(e => e.TblOrganizationDetailFax)
                    .HasMaxLength(25)
                    .HasColumnName("TBL_OrganizationDetail_Fax");

                entity.Property(e => e.TblOrganizationDetailIntroduction).HasColumnName("TBL_OrganizationDetail_Introduction");

                entity.Property(e => e.TblOrganizationDetailLogoAddress)
                    .HasMaxLength(255)
                    .HasColumnName("TBL_OrganizationDetail_LogoAddress");

                entity.Property(e => e.TblOrganizationDetailRankInDarooyab).HasColumnName("TBL_OrganizationDetail_RankInDarooyab");

                entity.Property(e => e.TblOrganizationDetailSiteAddress)
                    .HasMaxLength(255)
                    .HasColumnName("TBL_OrganizationDetail_SiteAddress");

                entity.Property(e => e.TblOrganizationDetailTel)
                    .HasMaxLength(25)
                    .HasColumnName("TBL_OrganizationDetail_Tel");
            });

            modelBuilder.Entity<TblToxication>(entity =>
            {
                entity.ToTable("TBL_Toxication");

                entity.Property(e => e.TblToxicationId).HasColumnName("TBL_Toxication_ID");

                entity.Property(e => e.TblToxicationContent).HasColumnName("TBL_Toxication_Content");

                entity.Property(e => e.TblToxicationImageUrl)
                    .HasMaxLength(255)
                    .HasColumnName("TBL_Toxication_ImageUrl");

                entity.Property(e => e.TblToxicationIsSpecial).HasColumnName("TBL_Toxication_IsSpecial");

                entity.Property(e => e.TblToxicationPageDescription)
                    .HasMaxLength(1000)
                    .HasColumnName("TBL_Toxication_PageDescription");

                entity.Property(e => e.TblToxicationPageTitle)
                    .HasMaxLength(200)
                    .HasColumnName("TBL_Toxication_PageTitle");

                entity.Property(e => e.TblToxicationRank).HasColumnName("TBL_Toxication_Rank");

                entity.Property(e => e.TblToxicationSubject)
                    .HasMaxLength(200)
                    .HasColumnName("TBL_Toxication_Subject");
            });

            modelBuilder.Entity<Temp1>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("temp1");

                entity.Property(e => e.Code)
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .HasColumnName("code");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Text)
                    .IsUnicode(false)
                    .HasColumnName("text");
            });

            modelBuilder.Entity<TempDosageForm>(entity =>
            {
                entity.HasKey(e => e.FdoGenerationCode);

                entity.ToTable("Temp_DosageForm");

                entity.Property(e => e.FdoGenerationCode).HasMaxLength(50);

                entity.Property(e => e.AutomaticStopOrderDay)
                    .HasMaxLength(3)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<TempHixCodeing>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("Temp_HixCodeing");

                entity.Property(e => e.Code).HasColumnName("code");

                entity.Property(e => e.Code2).HasColumnName("code2");

                entity.Property(e => e.Desc)
                    .HasMaxLength(150)
                    .HasColumnName("desc")
                    .IsFixedLength();
            });

            modelBuilder.Entity<TempKhadamat>(entity =>
            {
                entity.HasKey(e => e.FdoGenerationCode);

                entity.ToTable("Temp_Khadamat");

                entity.Property(e => e.FdoGenerationCode).HasMaxLength(100);

                entity.Property(e => e.F0)
                    .HasMaxLength(500)
                    .HasColumnName("f0");

                entity.Property(e => e.F1)
                    .HasMaxLength(500)
                    .HasColumnName("f1");

                entity.Property(e => e.F10)
                    .HasMaxLength(500)
                    .HasColumnName("f10");

                entity.Property(e => e.F2)
                    .HasMaxLength(1500)
                    .HasColumnName("f2");

                entity.Property(e => e.F3)
                    .HasMaxLength(500)
                    .HasColumnName("f3");

                entity.Property(e => e.F4)
                    .HasMaxLength(500)
                    .HasColumnName("f4");

                entity.Property(e => e.F6)
                    .HasMaxLength(500)
                    .HasColumnName("f6");

                entity.Property(e => e.F7)
                    .HasMaxLength(500)
                    .HasColumnName("f7");

                entity.Property(e => e.F8)
                    .HasMaxLength(500)
                    .HasColumnName("f8");

                entity.Property(e => e.F9)
                    .HasMaxLength(500)
                    .HasColumnName("f9");

                entity.Property(e => e.Price).HasMaxLength(500);
            });

            modelBuilder.Entity<TempPatientAdvice>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("Temp_PatientAdvice");

                entity.Property(e => e.DosageFormName).HasMaxLength(150);

                entity.Property(e => e.DosageFormNameFa)
                    .HasMaxLength(150)
                    .HasColumnName("DosageFormName_Fa");

                entity.Property(e => e.PatientAdviceAddDate)
                    .HasColumnType("date")
                    .HasColumnName("PatientAdvice_AddDate");
            });

            modelBuilder.Entity<TempPatientAdviceOld>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("Temp_PatientAdviceOld");

                entity.Property(e => e.DosageFormName).HasMaxLength(150);

                entity.Property(e => e.DosageFormNameFa)
                    .HasMaxLength(150)
                    .HasColumnName("DosageFormName_Fa");

                entity.Property(e => e.PatientAdviceAddDate)
                    .HasColumnType("date")
                    .HasColumnName("PatientAdvice_AddDate");
            });

            modelBuilder.Entity<TempPharmacyAccount>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("Temp_Pharmacy_Account");

                entity.Property(e => e.AccountExpireDate).HasColumnType("datetime");

                entity.Property(e => e.AccountStatus)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Address).HasMaxLength(800);

                entity.Property(e => e.AdminComment).HasMaxLength(500);

                entity.Property(e => e.City).HasMaxLength(30);

                entity.Property(e => e.CityIdFk).HasColumnName("CityId_fk");

                entity.Property(e => e.FounderEmail).HasMaxLength(50);

                entity.Property(e => e.FounderFamily).HasMaxLength(30);

                entity.Property(e => e.FounderMobileNum).HasMaxLength(11);

                entity.Property(e => e.FounderName).HasMaxLength(50);

                entity.Property(e => e.HixCode)
                    .IsRequired()
                    .HasMaxLength(11)
                    .IsUnicode(false)
                    .IsFixedLength();

                entity.Property(e => e.Id)
                    .ValueGeneratedOnAdd()
                    .HasColumnName("ID");

                entity.Property(e => e.Introduction).HasMaxLength(1000);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.NationalCode)
                    .HasMaxLength(11)
                    .IsUnicode(false)
                    .IsFixedLength();

                entity.Property(e => e.PharmacyActivityTypeFromCategoryIdFk).HasColumnName("PharmacyActivityType_FromCategoryId_fk");

                entity.Property(e => e.PharmacyDarooyabAdminApproved).HasColumnName("Pharmacy_DarooyabAdminApproved");

                entity.Property(e => e.PharmacyEngName)
                    .HasMaxLength(30)
                    .IsUnicode(false);

                entity.Property(e => e.PharmacyManagerName).HasMaxLength(30);

                entity.Property(e => e.PharmacyNatureFromCategoryIdFk).HasColumnName("PharmacyNature_FromCategoryId_fk");

                entity.Property(e => e.PharmacyTypeFromCategoryIdFk).HasColumnName("PharmacyType_FromCategoryId_fk");

                entity.Property(e => e.Phone)
                    .HasMaxLength(15)
                    .IsUnicode(false);

                entity.Property(e => e.PostalCode)
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.Property(e => e.ProvinceIdFk).HasColumnName("ProvinceId_fk");

                entity.Property(e => e.UniversityIdFk).HasColumnName("UniversityId_fk");
            });

            modelBuilder.Entity<TempTaminDirect>(entity =>
            {
                entity.ToTable("Temp_Tamin_Direct");

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.FdoGenerationCode).HasMaxLength(50);

                entity.Property(e => e.TaminDirectCode)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("Tamin_Direct_Code");

                entity.Property(e => e.TaminDirectIsAccept)
                    .HasMaxLength(50)
                    .HasColumnName("Tamin_Direct_IsAccept");

                entity.Property(e => e.TaminDirectIsChemotherapy)
                    .HasMaxLength(50)
                    .HasColumnName("Tamin_Direct_IsChemotherapy");

                entity.Property(e => e.TaminDirectIsDentist)
                    .HasMaxLength(50)
                    .HasColumnName("Tamin_Direct_IsDentist");

                entity.Property(e => e.TaminDirectIsErjaDakheli)
                    .HasMaxLength(50)
                    .HasColumnName("Tamin_Direct_IsErjaDakheli");

                entity.Property(e => e.TaminDirectIsHospital)
                    .HasMaxLength(50)
                    .HasColumnName("Tamin_Direct_IsHospital");

                entity.Property(e => e.TaminDirectIsMama)
                    .HasMaxLength(50)
                    .HasColumnName("Tamin_Direct_IsMama");

                entity.Property(e => e.TaminDirectIsRadiologist)
                    .HasMaxLength(50)
                    .HasColumnName("Tamin_Direct_IsRadiologist");

                entity.Property(e => e.TaminDirectIsSite)
                    .HasMaxLength(50)
                    .HasColumnName("Tamin_Direct_IsSite");

                entity.Property(e => e.TaminDirectLevel)
                    .HasMaxLength(50)
                    .HasColumnName("Tamin_Direct_Level");
            });

            modelBuilder.Entity<TempTedadJavabDehiPezeshk>(entity =>
            {
                entity.HasNoKey();

                entity.ToView("temp-TedadJavabDehiPezeshk");

                entity.Property(e => e.C).HasColumnName("c");
            });

            modelBuilder.Entity<TempUserMoasis>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("Temp_User_Moases");

                entity.Property(e => e.Hix)
                    .HasMaxLength(11)
                    .IsUnicode(false)
                    .HasColumnName("HIX")
                    .IsFixedLength();

                entity.Property(e => e.MoasesName).HasMaxLength(50);

                entity.Property(e => e.Mobile)
                    .HasMaxLength(11)
                    .IsUnicode(false)
                    .IsFixedLength();

                entity.Property(e => e.NationalCode)
                    .HasMaxLength(11)
                    .IsUnicode(false)
                    .IsFixedLength();
            });

            modelBuilder.Entity<TriggerCommand>(entity =>
            {
                entity.ToTable("TriggerCommand");

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.ActionType).HasMaxLength(50);

                entity.Property(e => e.HospitalId).HasColumnName("HospitalID");

                entity.Property(e => e.TableName).HasMaxLength(100);

                entity.Property(e => e.TimeStamp).HasColumnType("datetime");
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(e => e.Email);

                entity.Property(e => e.Email)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Fname).HasMaxLength(20);

                entity.Property(e => e.Id)
                    .HasColumnType("decimal(18, 0)")
                    .ValueGeneratedOnAdd()
                    .HasColumnName("ID");

                entity.Property(e => e.Lname).HasMaxLength(20);

                entity.Property(e => e.Mobile).HasMaxLength(11);

                entity.Property(e => e.RegDate)
                    .HasMaxLength(8)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<View1>(entity =>
            {
                entity.HasNoKey();

                entity.ToView("View_1", "admindaroyab");

                entity.Property(e => e.FactoryId).HasColumnName("FactoryID");

                entity.Property(e => e.FactoryName).HasMaxLength(50);
            });

            modelBuilder.Entity<ViewAppNewsHistoryView>(entity =>
            {
                entity.HasNoKey();

                entity.ToView("View_appNewsHistoryView");

                entity.Property(e => e.C).HasColumnName("c");

                entity.Property(e => e.NewsIdFk).HasColumnName("NewsID_FK");
            });

            modelBuilder.Entity<ViewCompaniesAndCountry>(entity =>
            {
                entity.HasNoKey();

                entity.ToView("View_CompaniesAndCountry", "admindaroyab");

                entity.Property(e => e.CountryId).HasColumnName("Country_ID");

                entity.Property(e => e.CountryName)
                    .HasMaxLength(30)
                    .HasColumnName("Country_Name");

                entity.Property(e => e.DrugName).HasMaxLength(100);

                entity.Property(e => e.FactoryId).HasColumnName("FactoryID");

                entity.Property(e => e.FactoryName).HasMaxLength(50);

                entity.Property(e => e.ImporterCompanyId).HasColumnName("ImporterCompanyID");

                entity.Property(e => e.ImporterCompanyName).HasMaxLength(50);
            });

            modelBuilder.Entity<ViewCompaniesAndCountry2>(entity =>
            {
                entity.HasNoKey();

                entity.ToView("View_CompaniesAndCountry2");

                entity.Property(e => e.CountryId).HasColumnName("Country_ID");

                entity.Property(e => e.CountryName)
                    .HasMaxLength(30)
                    .HasColumnName("Country_Name");

                entity.Property(e => e.DrugName).HasMaxLength(100);

                entity.Property(e => e.FactoryId).HasColumnName("FactoryID");

                entity.Property(e => e.FactoryName).HasMaxLength(50);

                entity.Property(e => e.ImporterCompanyId).HasColumnName("ImporterCompanyID");

                entity.Property(e => e.ImporterCompanyName).HasMaxLength(50);
            });

            modelBuilder.Entity<ViewCompany>(entity =>
            {
                entity.HasNoKey();

                entity.ToView("view_Companies");

                entity.Property(e => e.FactoryId).HasColumnName("FactoryID");

                entity.Property(e => e.FactoryName).HasMaxLength(50);

                entity.Property(e => e.FactoryTypeName)
                    .IsRequired()
                    .HasMaxLength(40)
                    .HasColumnName("FactoryType_Name");
            });

            modelBuilder.Entity<ViewDrugsDetailForApp>(entity =>
            {
                entity.HasNoKey();

                entity.ToView("View_DrugsDetailForApp");

                entity.Property(e => e.BrandNameEn)
                    .IsRequired()
                    .HasMaxLength(300)
                    .HasColumnName("BrandName_En");

                entity.Property(e => e.BrandNameFa)
                    .IsRequired()
                    .HasMaxLength(300)
                    .HasColumnName("BrandName_Fa");

                entity.Property(e => e.ClipUrl).HasMaxLength(3000);

                entity.Property(e => e.DistributorId).HasColumnName("DistributorID");

                entity.Property(e => e.DrugId).HasColumnName("DrugID");

                entity.Property(e => e.FactoryId).HasColumnName("FactoryID");

                entity.Property(e => e.GenericEn)
                    .HasMaxLength(300)
                    .HasColumnName("Generic_En");

                entity.Property(e => e.GenericFa)
                    .HasMaxLength(300)
                    .HasColumnName("Generic_Fa");

                entity.Property(e => e.GroupId).HasColumnName("GroupID");

                entity.Property(e => e.ImporterCompanyId).HasColumnName("ImporterCompanyID");

                entity.Property(e => e.IsOtc).HasColumnName("IsOTC");

                entity.Property(e => e.SoundUrl).HasMaxLength(3000);

                entity.Property(e => e.StorageTemperatureId).HasColumnName("StorageTemperatureID");
            });

            modelBuilder.Entity<ViewDrugsName>(entity =>
            {
                entity.HasNoKey();

                entity.ToView("ViewDrugsName");

                entity.Property(e => e.BusinessInformation).HasColumnType("ntext");

                entity.Property(e => e.CountryId).HasColumnName("Country_ID");

                entity.Property(e => e.CountryId2).HasColumnName("Country_ID2");

                entity.Property(e => e.CountryName)
                    .HasMaxLength(30)
                    .HasColumnName("Country_Name");

                entity.Property(e => e.CountryName2)
                    .HasMaxLength(30)
                    .HasColumnName("Country_Name2");

                entity.Property(e => e.CountryNameFa)
                    .HasMaxLength(10)
                    .HasColumnName("Country_Name_Fa")
                    .IsFixedLength();

                entity.Property(e => e.CountryNameFa2)
                    .HasMaxLength(10)
                    .HasColumnName("Country_Name_Fa2")
                    .IsFixedLength();

                entity.Property(e => e.DrugId).HasColumnName("DrugID");

                entity.Property(e => e.DrugName).HasMaxLength(100);

                entity.Property(e => e.DrugNameType)
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .IsFixedLength();

                entity.Property(e => e.FactoryId).HasColumnName("FactoryID");

                entity.Property(e => e.FactoryId2).HasColumnName("FactoryID2");

                entity.Property(e => e.FactoryName).HasMaxLength(50);

                entity.Property(e => e.FactoryName2).HasMaxLength(50);

                entity.Property(e => e.FactoryTypeName)
                    .HasMaxLength(40)
                    .HasColumnName("FactoryType_Name");

                entity.Property(e => e.GroupId).HasColumnName("GroupID");

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.ImporterCompanyId).HasColumnName("ImporterCompanyID");

                entity.Property(e => e.ImporterCompanyName).HasMaxLength(50);

                entity.Property(e => e.Irc).HasColumnName("IRC");

                entity.Property(e => e.IrgenericId).HasColumnName("IRGenericID");

                entity.Property(e => e.LinkId)
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .HasColumnName("LinkID")
                    .IsFixedLength();

                entity.Property(e => e.MainGeneric).HasMaxLength(100);
            });

            modelBuilder.Entity<ViewFarsiName>(entity =>
            {
                entity.HasNoKey();

                entity.ToView("View_FarsiName");

                entity.Property(e => e.DrugName).HasMaxLength(300);

                entity.Property(e => e.Expr1).HasMaxLength(300);

                entity.Property(e => e.Irc).HasColumnName("IRC");
            });

            modelBuilder.Entity<ViewHospitalActive>(entity =>
            {
                entity.HasNoKey();

                entity.ToView("View_hospitalActive");

                entity.Property(e => e.Address).HasMaxLength(500);

                entity.Property(e => e.LastLoginDate).HasColumnType("datetime");

                entity.Property(e => e.UserName)
                    .IsRequired()
                    .HasMaxLength(256);
            });

            modelBuilder.Entity<ViewImporterCompany>(entity =>
            {
                entity.HasNoKey();

                entity.ToView("View_ImporterCompany");

                entity.Property(e => e.FactoryName).HasMaxLength(50);

                entity.Property(e => e.FactoryTypeName)
                    .IsRequired()
                    .HasMaxLength(40)
                    .HasColumnName("FactoryType_Name");

                entity.Property(e => e.ImporterCompanyId).HasColumnName("ImporterCompanyID");
            });

            modelBuilder.Entity<ViewMob>(entity =>
            {
                entity.HasNoKey();

                entity.ToView("View_mob");

                entity.Property(e => e.DrugId).HasColumnName("DrugID");

                entity.Property(e => e.DrugName)
                    .IsRequired()
                    .HasMaxLength(300);

                entity.Property(e => e.DrugNameType)
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .IsFixedLength();

                entity.Property(e => e.Expr1)
                    .IsRequired()
                    .HasMaxLength(300);

                entity.Property(e => e.Expr2).HasMaxLength(300);

                entity.Property(e => e.Expr4)
                    .IsRequired()
                    .HasMaxLength(300);

                entity.Property(e => e.GroupNameFa)
                    .HasMaxLength(200)
                    .HasColumnName("GroupName_Fa");

                entity.Property(e => e.Irc).HasColumnName("IRC");

                entity.Property(e => e.IsOtc).HasColumnName("IsOTC");

                entity.Property(e => e.StorageTemperature).HasMaxLength(100);
            });

            modelBuilder.Entity<ViewMobMain>(entity =>
            {
                entity.HasNoKey();

                entity.ToView("View_mobMain");

                entity.Property(e => e.BrandNameEn)
                    .IsRequired()
                    .HasMaxLength(300)
                    .HasColumnName("BrandName_En");

                entity.Property(e => e.BrandNameFa)
                    .IsRequired()
                    .HasMaxLength(300)
                    .HasColumnName("BrandName_Fa");

                entity.Property(e => e.ClipUrl).HasMaxLength(3000);

                entity.Property(e => e.DistributorId).HasColumnName("DistributorID");

                entity.Property(e => e.DrugId).HasColumnName("DrugID");

                entity.Property(e => e.FactoryId).HasColumnName("FactoryID");

                entity.Property(e => e.GenericEn)
                    .HasMaxLength(300)
                    .HasColumnName("Generic_En");

                entity.Property(e => e.GenericFa)
                    .HasMaxLength(300)
                    .HasColumnName("Generic_Fa");

                entity.Property(e => e.GroupId).HasColumnName("GroupID");

                entity.Property(e => e.ImporterCompanyId).HasColumnName("ImporterCompanyID");

                entity.Property(e => e.IsOtc).HasColumnName("IsOTC");

                entity.Property(e => e.SoundUrl).HasMaxLength(3000);

                entity.Property(e => e.StorageTemperatureId).HasColumnName("StorageTemperatureID");
            });

            modelBuilder.Entity<ViewNewsApp>(entity =>
            {
                entity.HasNoKey();

                entity.ToView("View_NewsApp");

                entity.Property(e => e.C).HasColumnName("c");

                entity.Property(e => e.Title)
                    .IsRequired()
                    .HasMaxLength(1000);
            });

            modelBuilder.Entity<ViewNewsClick>(entity =>
            {
                entity.HasNoKey();

                entity.ToView("View_newsClick");

                entity.Property(e => e.C).HasColumnName("c");

                entity.Property(e => e.Title)
                    .IsRequired()
                    .HasMaxLength(1000);
            });

            modelBuilder.Entity<ViewPharmacyAllCounForEsf>(entity =>
            {
                entity.HasNoKey();

                entity.ToView("View_pharmacy_AllCounForESF");

                entity.Property(e => e.C).HasColumnName("c");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(50);
            });

            modelBuilder.Entity<ViewPharmacyAllCount>(entity =>
            {
                entity.HasNoKey();

                entity.ToView("View_Pharmacy_AllCount");

                entity.Property(e => e.C).HasColumnName("c");

                entity.Property(e => e.PharmacyAccountIdFk).HasColumnName("PharmacyAccountId_fk");
            });

            modelBuilder.Entity<ViewPharmacyDatum>(entity =>
            {
                entity.HasNoKey();

                entity.ToView("View_pharmacy_Data");

                entity.Property(e => e.DrugName)
                    .IsRequired()
                    .HasMaxLength(300);

                entity.Property(e => e.FounderName).HasMaxLength(50);

                entity.Property(e => e.Name).HasMaxLength(50);

                entity.Property(e => e.PharmacyIdFk).HasColumnName("PharmacyId_fk");

                entity.Property(e => e.RegistrationDateTime).HasColumnType("datetime");
            });

            modelBuilder.Entity<ViewPharmacyDatum1>(entity =>
            {
                entity.HasNoKey();

                entity.ToView("View_PharmacyData");

                entity.Property(e => e.Cc).HasColumnName("cc");

                entity.Property(e => e.PharmacyAccountIdFk).HasColumnName("PharmacyAccountId_fk");
            });

            modelBuilder.Entity<ViewProductName>(entity =>
            {
                entity.HasNoKey();

                entity.ToView("View_ProductName", "admindaroyab");

                entity.Property(e => e.CountryId).HasColumnName("Country_ID");

                entity.Property(e => e.CountryId2).HasColumnName("Country_ID2");

                entity.Property(e => e.DrugId).HasColumnName("DrugID");

                entity.Property(e => e.DrugNameEn)
                    .HasMaxLength(100)
                    .HasColumnName("DrugName_EN");

                entity.Property(e => e.DrugNameFa)
                    .HasMaxLength(100)
                    .HasColumnName("DrugName_FA");

                entity.Property(e => e.FactoryId).HasColumnName("FactoryID");

                entity.Property(e => e.FactoryId2).HasColumnName("FactoryID2");

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.ImporterCompanyName).HasMaxLength(50);

                entity.Property(e => e.Irc).HasColumnName("IRC");

                entity.Property(e => e.LinkId)
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .HasColumnName("LinkID")
                    .IsFixedLength();

                entity.Property(e => e.MainGeneric).HasMaxLength(100);
            });

            modelBuilder.Entity<ViewProductName2>(entity =>
            {
                entity.HasNoKey();

                entity.ToView("View_ProductName2");

                entity.Property(e => e.CountryId).HasColumnName("Country_ID");

                entity.Property(e => e.CountryId2).HasColumnName("Country_ID2");

                entity.Property(e => e.CountryName)
                    .HasMaxLength(30)
                    .HasColumnName("Country_Name");

                entity.Property(e => e.CountryName2)
                    .HasMaxLength(30)
                    .HasColumnName("Country_Name2");

                entity.Property(e => e.DrugId).HasColumnName("DrugID");

                entity.Property(e => e.DrugNameEn)
                    .HasMaxLength(100)
                    .HasColumnName("DrugName_EN");

                entity.Property(e => e.DrugNameFa)
                    .HasMaxLength(100)
                    .HasColumnName("DrugName_FA");

                entity.Property(e => e.FactoryId).HasColumnName("FactoryID");

                entity.Property(e => e.FactoryId2).HasColumnName("FactoryID2");

                entity.Property(e => e.FactoryName).HasMaxLength(50);

                entity.Property(e => e.FactoryName2).HasMaxLength(50);

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.ImporterCompanyId).HasColumnName("ImporterCompanyID");

                entity.Property(e => e.ImporterCompanyName).HasMaxLength(50);

                entity.Property(e => e.Irc).HasColumnName("IRC");

                entity.Property(e => e.LinkId)
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .HasColumnName("LinkID")
                    .IsFixedLength();

                entity.Property(e => e.MainGeneric).HasMaxLength(100);
            });

            modelBuilder.Entity<VwAspnetApplication>(entity =>
            {
                entity.HasNoKey();

                entity.ToView("vw_aspnet_Applications");

                entity.Property(e => e.ApplicationName)
                    .IsRequired()
                    .HasMaxLength(256);

                entity.Property(e => e.Description).HasMaxLength(256);

                entity.Property(e => e.LoweredApplicationName)
                    .IsRequired()
                    .HasMaxLength(256);
            });

            modelBuilder.Entity<VwAspnetMembershipUser>(entity =>
            {
                entity.HasNoKey();

                entity.ToView("vw_aspnet_MembershipUsers");

                entity.Property(e => e.Comment).HasColumnType("ntext");

                entity.Property(e => e.CreateDate).HasColumnType("datetime");

                entity.Property(e => e.Email).HasMaxLength(256);

                entity.Property(e => e.FailedPasswordAnswerAttemptWindowStart).HasColumnType("datetime");

                entity.Property(e => e.FailedPasswordAttemptWindowStart).HasColumnType("datetime");

                entity.Property(e => e.LastActivityDate).HasColumnType("datetime");

                entity.Property(e => e.LastLockoutDate).HasColumnType("datetime");

                entity.Property(e => e.LastLoginDate).HasColumnType("datetime");

                entity.Property(e => e.LastPasswordChangedDate).HasColumnType("datetime");

                entity.Property(e => e.LoweredEmail).HasMaxLength(256);

                entity.Property(e => e.MobileAlias).HasMaxLength(16);

                entity.Property(e => e.MobilePin)
                    .HasMaxLength(16)
                    .HasColumnName("MobilePIN");

                entity.Property(e => e.PasswordAnswer).HasMaxLength(128);

                entity.Property(e => e.PasswordQuestion).HasMaxLength(256);

                entity.Property(e => e.UserName)
                    .IsRequired()
                    .HasMaxLength(256);
            });

            modelBuilder.Entity<VwAspnetProfile>(entity =>
            {
                entity.HasNoKey();

                entity.ToView("vw_aspnet_Profiles");

                entity.Property(e => e.LastUpdatedDate).HasColumnType("datetime");
            });

            modelBuilder.Entity<VwAspnetRole>(entity =>
            {
                entity.HasNoKey();

                entity.ToView("vw_aspnet_Roles");

                entity.Property(e => e.Description).HasMaxLength(256);

                entity.Property(e => e.LoweredRoleName)
                    .IsRequired()
                    .HasMaxLength(256);

                entity.Property(e => e.RoleName)
                    .IsRequired()
                    .HasMaxLength(256);
            });

            modelBuilder.Entity<VwAspnetUser>(entity =>
            {
                entity.HasNoKey();

                entity.ToView("vw_aspnet_Users");

                entity.Property(e => e.LastActivityDate).HasColumnType("datetime");

                entity.Property(e => e.LoweredUserName)
                    .IsRequired()
                    .HasMaxLength(256);

                entity.Property(e => e.MobileAlias).HasMaxLength(16);

                entity.Property(e => e.UserName)
                    .IsRequired()
                    .HasMaxLength(256);
            });

            modelBuilder.Entity<VwAspnetUsersInRole>(entity =>
            {
                entity.HasNoKey();

                entity.ToView("vw_aspnet_UsersInRoles");
            });

            modelBuilder.Entity<VwAspnetWebPartStatePath>(entity =>
            {
                entity.HasNoKey();

                entity.ToView("vw_aspnet_WebPartState_Paths");

                entity.Property(e => e.LoweredPath)
                    .IsRequired()
                    .HasMaxLength(256);

                entity.Property(e => e.Path)
                    .IsRequired()
                    .HasMaxLength(256);
            });

            modelBuilder.Entity<VwAspnetWebPartStateShared>(entity =>
            {
                entity.HasNoKey();

                entity.ToView("vw_aspnet_WebPartState_Shared");

                entity.Property(e => e.LastUpdatedDate).HasColumnType("datetime");
            });

            modelBuilder.Entity<VwAspnetWebPartStateUser>(entity =>
            {
                entity.HasNoKey();

                entity.ToView("vw_aspnet_WebPartState_User");

                entity.Property(e => e.LastUpdatedDate).HasColumnType("datetime");
            });

            modelBuilder.Entity<WebSiteUserAccount>(entity =>
            {
                entity.ToTable("WebSiteUserAccount");

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.ActivitiyFullDescription).HasComment("شرح کامل فعالیت");

                entity.Property(e => e.ActivityIdFk)
                    .HasColumnName("ActivityID_FK")
                    .HasComment("نوع فعالیت");

                entity.Property(e => e.Address)
                    .HasMaxLength(2000)
                    .HasComment("آدرس");

                entity.Property(e => e.CityIdFk)
                    .HasColumnName("CityID_FK")
                    .HasComment("کد شهر");

                entity.Property(e => e.CompanyName)
                    .HasMaxLength(100)
                    .HasComment("نام شرکت");

                entity.Property(e => e.FactoryIdFk)
                    .HasColumnName("FactoryID_FK")
                    .HasComment("کد شرکت");

                entity.Property(e => e.Family)
                    .IsRequired()
                    .HasMaxLength(50)
                    .HasComment("نام خانوادگی");

                entity.Property(e => e.GenderIdFk)
                    .HasColumnName("GenderID_FK")
                    .HasComment("جنسیت");

                entity.Property(e => e.ImageUrl)
                    .HasMaxLength(700)
                    .HasComment("آدرس عکس صورت");

                entity.Property(e => e.InstagramUrl)
                    .HasMaxLength(2000)
                    .HasColumnName("instagramURL");

                entity.Property(e => e.IsactiveByAdmin)
                    .HasColumnName("ISActiveByAdmin")
                    .HasComment("تایید ثبت نام توسط مدیر");

                entity.Property(e => e.IsansweringPatientQuestion)
                    .HasColumnName("ISAnsweringPatientQuestion")
                    .HasComment("تمایل پاسخ به سوالات بیمار را دارید");

                entity.Property(e => e.Mobile)
                    .IsRequired()
                    .HasMaxLength(11)
                    .IsUnicode(false)
                    .IsFixedLength()
                    .HasComment("شماره تلفن اختصاصی(نام کاربری) ");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(50)
                    .HasComment("نام");

                entity.Property(e => e.NezamCode)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .IsFixedLength()
                    .HasComment("نظام پزشکی");

                entity.Property(e => e.Password)
                    .IsRequired()
                    .HasMaxLength(20)
                    .HasComment("کلمه عبور");

                entity.Property(e => e.ProvinceIdFk)
                    .HasColumnName("ProvinceID_FK")
                    .HasComment("کد استان");

                entity.Property(e => e.SpecialtyIdFk)
                    .HasColumnName("SpecialtyID_FK")
                    .HasComment("نوع تخصص");

                entity.Property(e => e.SpecialtyShortDescription)
                    .IsRequired()
                    .HasMaxLength(300)
                    .HasComment("شرح کوتاه تخصص");

                entity.Property(e => e.Whatsapp)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .IsFixedLength();

                entity.HasOne(d => d.ActivityIdFkNavigation)
                    .WithMany(p => p.WebSiteUserAccountActivityIdFkNavigations)
                    .HasForeignKey(d => d.ActivityIdFk)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ActivityID_WebSiteUserAccount_Category");

                entity.HasOne(d => d.CityIdFkNavigation)
                    .WithMany(p => p.WebSiteUserAccountCityIdFkNavigations)
                    .HasForeignKey(d => d.CityIdFk)
                    .HasConstraintName("FK_CityID_WebSiteUserAccount_Category");

                entity.HasOne(d => d.FactoryIdFkNavigation)
                    .WithMany(p => p.WebSiteUserAccounts)
                    .HasForeignKey(d => d.FactoryIdFk)
                    .HasConstraintName("FK_FactoryID_WebSiteUserAccount_Factorys");

                entity.HasOne(d => d.GenderIdFkNavigation)
                    .WithMany(p => p.WebSiteUserAccountGenderIdFkNavigations)
                    .HasForeignKey(d => d.GenderIdFk)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_GenderID_WebSiteUserAccount_Category");

                entity.HasOne(d => d.ProvinceIdFkNavigation)
                    .WithMany(p => p.WebSiteUserAccountProvinceIdFkNavigations)
                    .HasForeignKey(d => d.ProvinceIdFk)
                    .HasConstraintName("FK_ProvinceID_WebSiteUserAccount_Category");

                entity.HasOne(d => d.SpecialtyIdFkNavigation)
                    .WithMany(p => p.WebSiteUserAccountSpecialtyIdFkNavigations)
                    .HasForeignKey(d => d.SpecialtyIdFk)
                    .HasConstraintName("FK_SpecialtyID_WebSiteUserAccount_Category");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}