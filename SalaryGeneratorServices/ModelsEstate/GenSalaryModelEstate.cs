namespace SalaryGeneratorServices.ModelsEstate
{
    using System;
    using System.Data.Entity;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;

    public partial class GenSalaryModelEstate : DbContext
    {
        public static string host1 = "";
        public static string catalog1 = "";
        public static string user1 = "";
        public static string pass1 = "";
        public GenSalaryModelEstate()
            : base(nameOrConnectionString: "BYOWN")
        {
            base.Database.CommandTimeout = 3000;
            base.Database.Connection.ConnectionString = "data source=" + host1 + ";initial catalog=" + catalog1 + ";user id=" + user1 + ";password=" + pass1 + ";MultipleActiveResultSets=True;App=EntityFramework";
        }

        public static GenSalaryModelEstate ConnectToSqlServer(string host, string catalog, string user, string pass)
        {
            host1 = host;
            catalog1 = catalog;
            user1 = user;
            pass1 = pass;

            return new GenSalaryModelEstate();

        }

        public virtual DbSet<C__MigrationHistory> C__MigrationHistory { get; set; }
        public virtual DbSet<tbl_AktvtKerja> tbl_AktvtKerja { get; set; }
        public virtual DbSet<tbl_Blok> tbl_Blok { get; set; }
        public virtual DbSet<tbl_CutiDiambil> tbl_CutiDiambil { get; set; }
        public virtual DbSet<tbl_CutiPeruntukan> tbl_CutiPeruntukan { get; set; }
        public virtual DbSet<tbl_HasilSawitBlok> tbl_HasilSawitBlok { get; set; }
        public virtual DbSet<tbl_HasilSawitPkt> tbl_HasilSawitPkt { get; set; }
        public virtual DbSet<tbl_HasilSawitSubPkt> tbl_HasilSawitSubPkt { get; set; }
        public virtual DbSet<tbl_Kerja> tbl_Kerja { get; set; }
        public virtual DbSet<tbl_KerjaBonus> tbl_KerjaBonus { get; set; }
        public virtual DbSet<tbl_Kerjahdr> tbl_Kerjahdr { get; set; }
        public virtual DbSet<tbl_KerjahdrCuti> tbl_KerjahdrCuti { get; set; }
        public virtual DbSet<tbl_KerjaOT> tbl_KerjaOT { get; set; }
        public virtual DbSet<tbl_KumpulanKerja> tbl_KumpulanKerja { get; set; }
        public virtual DbSet<tbl_PktUtama> tbl_PktUtama { get; set; }
        public virtual DbSet<tbl_Produktiviti> tbl_Produktiviti { get; set; }
        public virtual DbSet<tbl_Sctran> tbl_Sctran { get; set; }
        public virtual DbSet<tbl_SubPkt> tbl_SubPkt { get; set; }
        public virtual DbSet<tblStatusPkj> tblStatusPkjs { get; set; }
        public virtual DbSet<tbl_GajiBulanan> tbl_GajiBulanan { get; set; }
        public virtual DbSet<tbl_Insentif> tbl_Insentif { get; set; }
        public virtual DbSet<tbl_LogDetail> tbl_LogDetail { get; set; }
        public virtual DbSet<tbl_Photo> tbl_Photo { get; set; }
        public virtual DbSet<tbl_Pkjmast> tbl_Pkjmast { get; set; }
        public virtual DbSet<tbl_ServicesList> tbl_ServicesList { get; set; }
        public virtual DbSet<tbl_SevicesProcess> tbl_SevicesProcess { get; set; }
        public virtual DbSet<tbl_SevicesProcessHistory> tbl_SevicesProcessHistory { get; set; }
        public virtual DbSet<tbl_Skb> tbl_Skb { get; set; }
        public virtual DbSet<tblHtmlReport> tblHtmlReports { get; set; }
        public virtual DbSet<vw_GajiBulananPekerja> vw_GajiBulananPekerja { get; set; }
        public virtual DbSet<vw_HasilSawitBlok> vw_HasilSawitBlok { get; set; }
        public virtual DbSet<vw_HasilSawitPkt> vw_HasilSawitPkt { get; set; }
        public virtual DbSet<vw_HasilSawitSubPkt> vw_HasilSawitSubPkt { get; set; }
        public virtual DbSet<vw_InsentifPekerja> vw_InsentifPekerja { get; set; }
        public virtual DbSet<vw_Kerja_Bonus> vw_Kerja_Bonus { get; set; }
        public virtual DbSet<vw_Kerja_Hdr_Cuti> vw_Kerja_Hdr_Cuti { get; set; }
        public virtual DbSet<vw_Kerja_OT> vw_Kerja_OT { get; set; }
        public virtual DbSet<vw_Kerjahdr> vw_Kerjahdr { get; set; }
        public virtual DbSet<vw_KerjaPekerja> vw_KerjaPekerja { get; set; }
        public virtual DbSet<vw_KumpulanKerja> vw_KumpulanKerja { get; set; }
        public virtual DbSet<vw_KumpulanPekerja> vw_KumpulanPekerja { get; set; }
        public virtual DbSet<vw_MaklumatCuti> vw_MaklumatCuti { get; set; }
        public virtual DbSet<vw_MaklumatInsentif> vw_MaklumatInsentif { get; set; }
        public virtual DbSet<vw_MaklumatProduktiviti> vw_MaklumatProduktiviti { get; set; }
        public virtual DbSet<vw_RptSctran> vw_RptSctran { get; set; }
        public virtual DbSet<tbl_TutupUrusNiaga> tbl_TutupUrusNiaga { get; set; }
        public virtual DbSet<tbl_KerjahdrCutiTahunan> tbl_KerjahdrCutiTahunan { get; set; }
        public virtual DbSet<tbl_ByrCarumanTambahan> tbl_ByrCarumanTambahan { get; set; }
        public virtual DbSet<tbl_PkjCarumanTambahan> tbl_PkjCarumanTambahan { get; set; }
        public virtual DbSet<vw_KerjaInfoDetails> vw_KerjaInfoDetails { get; set; }
        public virtual DbSet<tbl_PkjIncrmntSalary> tbl_PkjIncrmntSalary { get; set; }
        public virtual DbSet<vw_KerjaDetailScTrans> vw_KerjaDetailScTrans { get; set; }

        //Added by kamalia 26/8/2021
        public virtual DbSet<tbl_SAPPostDataDetails> tbl_SAPPostDataDetails { get; set; }
        public virtual DbSet<tbl_SAPPostRef> tbl_SAPPostRef { get; set; }
        //Added by kamalia 15/2/2022
        public virtual DbSet<vw_PaySheetPekerja> vw_PaySheetPekerja { get; set; }

        public virtual DbSet<tbl_SpecialInsentif> tbl_SpecialInsentif { get; set; }
        public virtual DbSet<vw_SpecialInsentive> vw_SpecialInsentive { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            //modelBuilder.Entity<tbl_AktvtKerja>()
            //    .Property(e => e.fld_Peratus)
            //    .HasPrecision(3, 0);

            //modelBuilder.Entity<tbl_AktvtKerja>()
            //    .Property(e => e.fld_Harga)
            //    .HasPrecision(6, 2);

            //modelBuilder.Entity<tbl_Blok>()
            //    .Property(e => e.fld_LsBlok)
            //    .HasPrecision(8, 3);

            //modelBuilder.Entity<tbl_Blok>()
            //    .Property(e => e.fld_Blok_Sblm)
            //    .IsUnicode(false);

            //modelBuilder.Entity<tbl_Blok>()
            //    .Property(e => e.fld_LsBlok_Sblm)
            //    .HasPrecision(8, 3);

            //modelBuilder.Entity<tbl_HasilSawitBlok>()
            //    .Property(e => e.fld_KodBlok)
            //    .IsUnicode(false);

            //modelBuilder.Entity<tbl_HasilSawitBlok>()
            //    .Property(e => e.fld_HasilTan)
            //    .HasPrecision(10, 2);

            //modelBuilder.Entity<tbl_HasilSawitBlok>()
            //    .Property(e => e.fld_LuasHektar)
            //    .HasPrecision(8, 2);

            //modelBuilder.Entity<tbl_HasilSawitBlok>()
            //    .Property(e => e.fld_Bulan)
            //    .HasPrecision(2, 0);

            //modelBuilder.Entity<tbl_HasilSawitBlok>()
            //    .Property(e => e.fld_Tahun)
            //    .HasPrecision(4, 0);

            //modelBuilder.Entity<tbl_HasilSawitPkt>()
            //    .Property(e => e.fld_KodPeringkat)
            //    .IsUnicode(false);

            //modelBuilder.Entity<tbl_HasilSawitPkt>()
            //    .Property(e => e.fld_HasilTan)
            //    .HasPrecision(10, 2);

            //modelBuilder.Entity<tbl_HasilSawitPkt>()
            //    .Property(e => e.fld_LuasHektar)
            //    .HasPrecision(8, 2);

            //modelBuilder.Entity<tbl_HasilSawitPkt>()
            //    .Property(e => e.fld_Bulan)
            //    .HasPrecision(2, 0);

            //modelBuilder.Entity<tbl_HasilSawitPkt>()
            //    .Property(e => e.fld_Tahun)
            //    .HasPrecision(4, 0);

            //modelBuilder.Entity<tbl_HasilSawitSubPkt>()
            //    .Property(e => e.fld_KodSubPeringkat)
            //    .IsUnicode(false);

            //modelBuilder.Entity<tbl_HasilSawitSubPkt>()
            //    .Property(e => e.fld_HasilTan)
            //    .HasPrecision(10, 2);

            //modelBuilder.Entity<tbl_HasilSawitSubPkt>()
            //    .Property(e => e.fld_LuasHektar)
            //    .HasPrecision(8, 2);

            //modelBuilder.Entity<tbl_HasilSawitSubPkt>()
            //    .Property(e => e.fld_Bulan)
            //    .HasPrecision(2, 0);

            //modelBuilder.Entity<tbl_HasilSawitSubPkt>()
            //    .Property(e => e.fld_Tahun)
            //    .HasPrecision(4, 0);
            
            //modelBuilder.Entity<tbl_Kerja>()
            //    .Property(e => e.fld_JumlahHasil)
            //    .HasPrecision(6, 2);

            //modelBuilder.Entity<tbl_Kerja>()
            //    .Property(e => e.fld_BrtGth)
            //    .HasPrecision(6, 2);

            //modelBuilder.Entity<tbl_Kerja>()
            //    .Property(e => e.fld_JamOT)
            //    .HasPrecision(4, 2);

            //modelBuilder.Entity<tbl_Kerja>()
            //    .Property(e => e.fld_KdhMenuai)
            //    .IsFixedLength()
            //    .IsUnicode(false);

            //modelBuilder.Entity<tbl_Kerja>()
            //    .Property(e => e.fld_DataSource)
            //    .IsFixedLength()
            //    .IsUnicode(false);

            //modelBuilder.Entity<tbl_Kerjahdr>()
            //    .Property(e => e.fld_Kdhdct)
            //    .IsFixedLength()
            //    .IsUnicode(false);
            
            //modelBuilder.Entity<tbl_KerjaOT>()
            //    .Property(e => e.fld_JamOT)
            //    .HasPrecision(4, 2);

            //modelBuilder.Entity<tbl_PktUtama>()
            //    .Property(e => e.fld_PktUtama)
            //    .IsUnicode(false);

            //modelBuilder.Entity<tbl_PktUtama>()
            //    .Property(e => e.fld_LsPktUtama)
            //    .HasPrecision(8, 3);

            //modelBuilder.Entity<tbl_PktUtama>()
            //    .Property(e => e.fld_JnsTnmn)
            //    .IsUnicode(false);

            //modelBuilder.Entity<tbl_PktUtama>()
            //    .Property(e => e.fld_StatusTnmn)
            //    .IsUnicode(false);

            //modelBuilder.Entity<tbl_PktUtama>()
            //    .Property(e => e.fld_LsPktUtama_Sblm)
            //    .HasPrecision(8, 3);

            //modelBuilder.Entity<tbl_PktUtama>()
            //    .Property(e => e.fld_JnsTnmn_Sblm)
            //    .IsUnicode(false);

            //modelBuilder.Entity<tbl_PktUtama>()
            //    .Property(e => e.fld_StatusTnmn_Sblm)
            //    .IsUnicode(false);
            

            //modelBuilder.Entity<tbl_SubPkt>()
            //    .Property(e => e.fld_LsPkt)
            //    .HasPrecision(8, 3);

            //modelBuilder.Entity<tbl_SubPkt>()
            //    .Property(e => e.fld_Pkt_Sblm)
            //    .IsUnicode(false);

            //modelBuilder.Entity<tbl_SubPkt>()
            //    .Property(e => e.fld_LsPkt_Sblm)
            //    .HasPrecision(8, 3);

            //modelBuilder.Entity<tblStatusPkj>()
            //    .Property(e => e.fldNoPkjLama)
            //    .IsFixedLength()
            //    .IsUnicode(false);

            //modelBuilder.Entity<tblStatusPkj>()
            //    .Property(e => e.fldNoKP)
            //    .IsFixedLength()
            //    .IsUnicode(false);

            //modelBuilder.Entity<tblStatusPkj>()
            //    .Property(e => e.fldNoPkjBaru)
            //    .IsFixedLength()
            //    .IsUnicode(false);

            //modelBuilder.Entity<tbl_GajiBulanan>()
            //    .Property(e => e.fld_ByrKerja)
            //    .HasPrecision(8, 2);

            //modelBuilder.Entity<tbl_GajiBulanan>()
            //    .Property(e => e.fld_KWSPPkj)
            //    .HasPrecision(8, 2);

            //modelBuilder.Entity<tbl_GajiBulanan>()
            //    .Property(e => e.fld_KWSPMjk)
            //    .HasPrecision(8, 2);

            //modelBuilder.Entity<tbl_GajiBulanan>()
            //    .Property(e => e.fld_SocsoPkj)
            //    .HasPrecision(8, 2);

            //modelBuilder.Entity<tbl_GajiBulanan>()
            //    .Property(e => e.fld_SocsoMjk)
            //    .HasPrecision(8, 2);

            //modelBuilder.Entity<tbl_GajiBulanan>()
            //    .Property(e => e.fld_LainInsentif)
            //    .HasPrecision(8, 2);

            //modelBuilder.Entity<tbl_GajiBulanan>()
            //    .Property(e => e.fld_OT)
            //    .HasPrecision(8, 2);

            //modelBuilder.Entity<tbl_GajiBulanan>()
            //    .Property(e => e.fld_ByrCuti)
            //    .HasPrecision(8, 2);

            //modelBuilder.Entity<tbl_GajiBulanan>()
            //    .Property(e => e.fld_BonusHarian)
            //    .HasPrecision(8, 2);

            //modelBuilder.Entity<tbl_GajiBulanan>()
            //    .Property(e => e.fld_LainPotongan)
            //    .HasPrecision(8, 2);

            //modelBuilder.Entity<tbl_GajiBulanan>()
            //    .Property(e => e.fld_TargetProd)
            //    .HasPrecision(8, 2);

            //modelBuilder.Entity<tbl_GajiBulanan>()
            //    .Property(e => e.fld_CapaiProd)
            //    .HasPrecision(8, 2);

            //modelBuilder.Entity<tbl_GajiBulanan>()
            //    .Property(e => e.fld_ProdInsentif)
            //    .HasPrecision(8, 2);

            //modelBuilder.Entity<tbl_GajiBulanan>()
            //    .Property(e => e.fld_KuaInsentif)
            //    .HasPrecision(8, 2);

            //modelBuilder.Entity<tbl_GajiBulanan>()
            //    .Property(e => e.fld_HdrInsentif)
            //    .HasPrecision(8, 2);

            //modelBuilder.Entity<tbl_GajiBulanan>()
            //    .Property(e => e.fld_AIPS)
            //    .HasPrecision(8, 2);

            //modelBuilder.Entity<tbl_GajiBulanan>()
            //    .Property(e => e.fld_GajiKasar)
            //    .HasPrecision(8, 2);

            //modelBuilder.Entity<tbl_GajiBulanan>()
            //    .Property(e => e.fld_GajiBersih)
            //    .HasPrecision(8, 2);

            //modelBuilder.Entity<tbl_GajiBulanan>()
            //    .Property(e => e.fld_PurataGaji)
            //    .HasPrecision(8, 2);

            //modelBuilder.Entity<tbl_GajiBulanan>()
            //    .Property(e => e.fld_PurataGaji12Bln)
            //    .HasPrecision(8, 2);

            ////modelBuilder.Entity<tbl_Insentif>()
            ////    .Property(e => e.fld_Nopkj)
            ////    .IsFixedLength()
            ////    .IsUnicode(false);

            //modelBuilder.Entity<tbl_Photo>()
            //    .Property(e => e.fld_Nopkj)
            //    .IsFixedLength()
            //    .IsUnicode(false);

            //modelBuilder.Entity<tbl_Pkjmast>()
            //    .Property(e => e.fld_Nopkj)
            //    .IsFixedLength()
            //    .IsUnicode(false);

            //modelBuilder.Entity<tbl_Pkjmast>()
            //    .Property(e => e.fld_Nokp)
            //    .IsFixedLength()
            //    .IsUnicode(false);

            //modelBuilder.Entity<tbl_Pkjmast>()
            //    .Property(e => e.fld_Nama)
            //    .IsFixedLength()
            //    .IsUnicode(false);

            //modelBuilder.Entity<tbl_Pkjmast>()
            //    .Property(e => e.fld_Almt1)
            //    .IsFixedLength()
            //    .IsUnicode(false);

            //modelBuilder.Entity<tbl_Pkjmast>()
            //    .Property(e => e.fld_Daerah)
            //    .IsFixedLength()
            //    .IsUnicode(false);

            //modelBuilder.Entity<tbl_Pkjmast>()
            //    .Property(e => e.fld_Neg)
            //    .IsFixedLength()
            //    .IsUnicode(false);

            //modelBuilder.Entity<tbl_Pkjmast>()
            //    .Property(e => e.fld_Negara)
            //    .IsFixedLength()
            //    .IsUnicode(false);

            //modelBuilder.Entity<tbl_Pkjmast>()
            //    .Property(e => e.fld_Poskod)
            //    .IsFixedLength()
            //    .IsUnicode(false);

            //modelBuilder.Entity<tbl_Pkjmast>()
            //    .Property(e => e.fld_Notel)
            //    .IsFixedLength()
            //    .IsUnicode(false);

            //modelBuilder.Entity<tbl_Pkjmast>()
            //    .Property(e => e.fld_Nofax)
            //    .IsFixedLength()
            //    .IsUnicode(false);

            //modelBuilder.Entity<tbl_Pkjmast>()
            //    .Property(e => e.fld_Kdjnt)
            //    .IsFixedLength()
            //    .IsUnicode(false);

            //modelBuilder.Entity<tbl_Pkjmast>()
            //    .Property(e => e.fld_Kdbgsa)
            //    .IsFixedLength()
            //    .IsUnicode(false);

            //modelBuilder.Entity<tbl_Pkjmast>()
            //    .Property(e => e.fld_Kdagma)
            //    .IsFixedLength()
            //    .IsUnicode(false);

            //modelBuilder.Entity<tbl_Pkjmast>()
            //    .Property(e => e.fld_Kdrkyt)
            //    .IsFixedLength()
            //    .IsUnicode(false);

            //modelBuilder.Entity<tbl_Pkjmast>()
            //    .Property(e => e.fld_Kdkwn)
            //    .IsFixedLength()
            //    .IsUnicode(false);

            //modelBuilder.Entity<tbl_Pkjmast>()
            //    .Property(e => e.fld_Kpenrka)
            //    .IsFixedLength()
            //    .IsUnicode(false);

            //modelBuilder.Entity<tbl_Pkjmast>()
            //    .Property(e => e.fld_Kdaktf)
            //    .IsFixedLength()
            //    .IsUnicode(false);

            //modelBuilder.Entity<tbl_Pkjmast>()
            //    .Property(e => e.fld_Sbtakf)
            //    .IsFixedLength()
            //    .IsUnicode(false);

            //modelBuilder.Entity<tbl_Pkjmast>()
            //    .Property(e => e.fld_Ktgpkj)
            //    .IsFixedLength()
            //    .IsUnicode(false);

            //modelBuilder.Entity<tbl_Pkjmast>()
            //    .Property(e => e.fld_Jenispekerja)
            //    .IsFixedLength()
            //    .IsUnicode(false);

            //modelBuilder.Entity<tbl_Pkjmast>()
            //    .Property(e => e.fld_Kodbkl)
            //    .IsFixedLength()
            //    .IsUnicode(false);

            //modelBuilder.Entity<tbl_Pkjmast>()
            //    .Property(e => e.fld_KodSocso)
            //    .IsUnicode(false);

            //modelBuilder.Entity<tbl_Pkjmast>()
            //    .Property(e => e.fld_Noperkeso)
            //    .IsFixedLength()
            //    .IsUnicode(false);

            //modelBuilder.Entity<tbl_Pkjmast>()
            //    .Property(e => e.fld_KodKWSP)
            //    .IsUnicode(false);

            //modelBuilder.Entity<tbl_Pkjmast>()
            //    .Property(e => e.fld_Nokwsp)
            //    .IsFixedLength()
            //    .IsUnicode(false);

            //modelBuilder.Entity<tbl_Pkjmast>()
            //    .Property(e => e.fld_Visano)
            //    .IsFixedLength()
            //    .IsUnicode(false);

            //modelBuilder.Entity<tbl_Pkjmast>()
            //    .Property(e => e.fld_Nogilr)
            //    .IsFixedLength()
            //    .IsUnicode(false);

            //modelBuilder.Entity<tbl_Pkjmast>()
            //    .Property(e => e.fld_Prmtno)
            //    .IsFixedLength()
            //    .IsUnicode(false);

            //modelBuilder.Entity<tbl_Pkjmast>()
            //    .Property(e => e.fld_Psptno)
            //    .IsFixedLength()
            //    .IsUnicode(false);

            //modelBuilder.Entity<tbl_Pkjmast>()
            //    .Property(e => e.fld_Kdldg)
            //    .IsFixedLength()
            //    .IsUnicode(false);

            //modelBuilder.Entity<tbl_Pkjmast>()
            //    .Property(e => e.fld_IDpkj)
            //    .IsFixedLength()
            //    .IsUnicode(false);

            //modelBuilder.Entity<tbl_Pkjmast>()
            //    .Property(e => e.fld_StatusKwspSocso)
            //    .IsFixedLength()
            //    .IsUnicode(false);

            //modelBuilder.Entity<tbl_Pkjmast>()
            //    .Property(e => e.fld_StatusAkaun)
            //    .IsFixedLength()
            //    .IsUnicode(false);
            
            //modelBuilder.Entity<tbl_Skb>()
            //    .Property(e => e.fld_Bulan)
            //    .IsFixedLength()
            //    .IsUnicode(false);

            //modelBuilder.Entity<vw_GajiBulananPekerja>()
            //    .Property(e => e.fld_Nopkj)
            //    .IsFixedLength()
            //    .IsUnicode(false);

            //modelBuilder.Entity<vw_GajiBulananPekerja>()
            //    .Property(e => e.fld_Nama)
            //    .IsFixedLength()
            //    .IsUnicode(false);

            //modelBuilder.Entity<vw_GajiBulananPekerja>()
            //    .Property(e => e.fld_Gaji_Kasar)
            //    .HasPrecision(8, 2);

            //modelBuilder.Entity<vw_HasilSawitBlok>()
            //    .Property(e => e.fld_LsBlok)
            //    .HasPrecision(8, 3);

            //modelBuilder.Entity<vw_HasilSawitBlok>()
            //    .Property(e => e.fld_HasilTan)
            //    .HasPrecision(10, 2);

            //modelBuilder.Entity<vw_HasilSawitBlok>()
            //    .Property(e => e.fld_Bulan)
            //    .HasPrecision(2, 0);

            //modelBuilder.Entity<vw_HasilSawitBlok>()
            //    .Property(e => e.fld_Tahun)
            //    .HasPrecision(4, 0);

            //modelBuilder.Entity<vw_HasilSawitPkt>()
            //    .Property(e => e.fld_PktUtama)
            //    .IsUnicode(false);

            //modelBuilder.Entity<vw_HasilSawitPkt>()
            //    .Property(e => e.fld_LsPktUtama)
            //    .HasPrecision(8, 3);

            //modelBuilder.Entity<vw_HasilSawitPkt>()
            //    .Property(e => e.fld_HasilTan)
            //    .HasPrecision(10, 2);

            //modelBuilder.Entity<vw_HasilSawitPkt>()
            //    .Property(e => e.fld_Bulan)
            //    .HasPrecision(2, 0);

            //modelBuilder.Entity<vw_HasilSawitPkt>()
            //    .Property(e => e.fld_Tahun)
            //    .HasPrecision(4, 0);

            //modelBuilder.Entity<vw_HasilSawitSubPkt>()
            //    .Property(e => e.fld_LsPkt)
            //    .HasPrecision(8, 3);

            //modelBuilder.Entity<vw_HasilSawitSubPkt>()
            //    .Property(e => e.fld_HasilTan)
            //    .HasPrecision(10, 2);

            //modelBuilder.Entity<vw_HasilSawitSubPkt>()
            //    .Property(e => e.fld_Bulan)
            //    .HasPrecision(2, 0);

            //modelBuilder.Entity<vw_HasilSawitSubPkt>()
            //    .Property(e => e.fld_Tahun)
            //    .HasPrecision(4, 0);

            //modelBuilder.Entity<vw_InsentifPekerja>()
            //    .Property(e => e.fld_Nopkj)
            //    .IsFixedLength()
            //    .IsUnicode(false);

            //modelBuilder.Entity<vw_InsentifPekerja>()
            //    .Property(e => e.fld_Nama)
            //    .IsFixedLength()
            //    .IsUnicode(false);

            //modelBuilder.Entity<vw_InsentifPekerja>()
            //    .Property(e => e.fld_Kdaktf)
            //    .IsFixedLength()
            //    .IsUnicode(false);

            //modelBuilder.Entity<vw_Kerja_Bonus>()
            //    .Property(e => e.fld_Nopkj)
            //    .IsFixedLength()
            //    .IsUnicode(false);

            //modelBuilder.Entity<vw_Kerja_Hdr_Cuti>()
            //    .Property(e => e.fld_Nopkj)
            //    .IsFixedLength()
            //    .IsUnicode(false);

            //modelBuilder.Entity<vw_Kerja_Hdr_Cuti>()
            //    .Property(e => e.fld_Kdhdct)
            //    .IsFixedLength()
            //    .IsUnicode(false);

            //modelBuilder.Entity<vw_Kerja_OT>()
            //    .Property(e => e.fld_Nopkj)
            //    .IsFixedLength()
            //    .IsUnicode(false);

            //modelBuilder.Entity<vw_Kerja_OT>()
            //    .Property(e => e.fld_JamOT)
            //    .HasPrecision(4, 2);

            //modelBuilder.Entity<vw_Kerjahdr>()
            //    .Property(e => e.fld_Nopkj)
            //    .IsFixedLength()
            //    .IsUnicode(false);

            //modelBuilder.Entity<vw_Kerjahdr>()
            //    .Property(e => e.fld_Nama)
            //    .IsFixedLength()
            //    .IsUnicode(false);

            //modelBuilder.Entity<vw_Kerjahdr>()
            //    .Property(e => e.fld_Kdhdct)
            //    .IsFixedLength()
            //    .IsUnicode(false);

            //modelBuilder.Entity<vw_KerjaPekerja>()
            //    .Property(e => e.fld_Nopkj)
            //    .IsFixedLength()
            //    .IsUnicode(false);

            //modelBuilder.Entity<vw_KerjaPekerja>()
            //    .Property(e => e.fld_JumlahHasil)
            //    .HasPrecision(6, 2);

            //modelBuilder.Entity<vw_KerjaPekerja>()
            //    .Property(e => e.fld_BrtGth)
            //    .HasPrecision(6, 2);

            //modelBuilder.Entity<vw_KerjaPekerja>()
            //    .Property(e => e.fld_JamOT)
            //    .HasPrecision(4, 2);

            //modelBuilder.Entity<vw_KerjaPekerja>()
            //    .Property(e => e.fld_KdhMenuai)
            //    .IsFixedLength()
            //    .IsUnicode(false);

            //modelBuilder.Entity<vw_KerjaPekerja>()
            //    .Property(e => e.fld_DataSource)
            //    .IsFixedLength()
            //    .IsUnicode(false);

            //modelBuilder.Entity<vw_KerjaPekerja>()
            //    .Property(e => e.fld_Kdhdct)
            //    .IsFixedLength()
            //    .IsUnicode(false);

            //modelBuilder.Entity<vw_KumpulanPekerja>()
            //    .Property(e => e.fld_Nopkj)
            //    .IsFixedLength()
            //    .IsUnicode(false);

            //modelBuilder.Entity<vw_KumpulanPekerja>()
            //    .Property(e => e.fld_Nama)
            //    .IsFixedLength()
            //    .IsUnicode(false);

            //modelBuilder.Entity<vw_KumpulanPekerja>()
            //    .Property(e => e.Expr1)
            //    .IsFixedLength()
            //    .IsUnicode(false);

            //modelBuilder.Entity<vw_KumpulanPekerja>()
            //    .Property(e => e.fld_Kdaktf)
            //    .IsFixedLength()
            //    .IsUnicode(false);

            //modelBuilder.Entity<vw_MaklumatInsentif>()
            //    .Property(e => e.fld_Nopkj)
            //    .IsFixedLength()
            //    .IsUnicode(false);

            //modelBuilder.Entity<vw_MaklumatProduktiviti>()
            //    .Property(e => e.fld_Nopkj)
            //    .IsFixedLength()
            //    .IsUnicode(false);

            //modelBuilder.Entity<vw_MaklumatProduktiviti>()
            //    .Property(e => e.fld_Nama)
            //    .IsFixedLength()
            //    .IsUnicode(false);

            //modelBuilder.Entity<vw_MaklumatProduktiviti>()
            //    .Property(e => e.fld_Nokp)
            //    .IsFixedLength()
            //    .IsUnicode(false);

            //modelBuilder.Entity<vw_MaklumatProduktiviti>()
            //    .Property(e => e.fld_Kdaktf)
            //    .IsFixedLength()
            //    .IsUnicode(false);

            //modelBuilder.Entity<vw_RptSctran>()
            //    .Property(e => e.fld_KdCaj)
            //    .IsFixedLength()
            //    .IsUnicode(false);
        }
    }
}
