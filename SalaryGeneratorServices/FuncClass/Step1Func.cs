using SalaryGeneratorServices.CustomModels;
using SalaryGeneratorServices.ModelsEstate;
using SalaryGeneratorServices.ModelsHQ;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SalaryGeneratorServices.FuncClass
{
    class Step1Func
    {
        public void AddServicesProcessFunc(int? NegaraID, int? SyarikatID, int? WilayahID, int? LadangID, int? UserID, DateTime DTProcess, int? Month, int? Year, string processname, string servicesname, int? ClientID)
        {
            GenSalaryModelHQ db = new GenSalaryModelHQ();
            GetConnectFunc conn = new GetConnectFunc();
            var checkservicesprocess = db.tbl_SevicesProcess.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_ServicesName == servicesname && x.fld_ProcessName == processname && x.fld_Flag == 1).Count();
            ModelsHQ.tbl_SevicesProcess tbl_SevicesProcess = new ModelsHQ.tbl_SevicesProcess();

            if (checkservicesprocess == 0)
            {
                tbl_SevicesProcess.fld_ClientID = ClientID;
                tbl_SevicesProcess.fld_DTProcess = DTProcess;
                tbl_SevicesProcess.fld_Flag = 1;
                tbl_SevicesProcess.fld_Month = Month;
                tbl_SevicesProcess.fld_NegaraID = NegaraID;
                tbl_SevicesProcess.fld_ProcessName = processname;
                tbl_SevicesProcess.fld_ProcessPercentage = 0;
                tbl_SevicesProcess.fld_SelCatVal = LadangID;
                tbl_SevicesProcess.fld_ServicesName = servicesname;
                tbl_SevicesProcess.fld_SyarikatID = SyarikatID;
                tbl_SevicesProcess.fld_UplSelCat = 4;
                tbl_SevicesProcess.fld_UserID = UserID;
                tbl_SevicesProcess.fld_Year = Year;
                tbl_SevicesProcess.fld_WilayahID = WilayahID;
                tbl_SevicesProcess.fld_LadangID = LadangID;

                db.tbl_SevicesProcess.Add(tbl_SevicesProcess);
                db.SaveChanges();
            }
            else
            {
            }

            db.Dispose();
        }

        public void UpdateServicesProcessPercFunc(ModelsHQ.tbl_SevicesProcess tbl_SevicesProcess, decimal Percentage, int Flag, GenSalaryModelHQ db, int TotalData, int RunningData)
        {
            string DataToProcess = RunningData + "/" + TotalData;
            tbl_SevicesProcess.fld_ProcessPercentage = Percentage;
            tbl_SevicesProcess.fld_DataToProcess = DataToProcess;
            tbl_SevicesProcess.fld_Flag = Flag;
            db.Entry(tbl_SevicesProcess).State = EntityState.Modified;
            db.SaveChanges();
        }

        public ModelsHQ.tbl_SevicesProcess GetServiceProcessDetail(int? NegaraID, int? SyarikatID, int? WilayahID, int? LadangID, int? UserID, DateTime DTProcess, int? Month, int? Year, string processname, string servicesname, int? ClientID)
        {
            GenSalaryModelHQ db = new GenSalaryModelHQ();
            ModelsHQ.tbl_SevicesProcess SevicesProcess = new ModelsHQ.tbl_SevicesProcess();

            SevicesProcess = db.tbl_SevicesProcess.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_ServicesName == servicesname && x.fld_ProcessName == processname && x.fld_Flag == 1).FirstOrDefault();

            return SevicesProcess;
        }

        public List<tbl_Pkjmast> GetActiveWorkerFunc(int? NegaraID, int? SyarikatID, int? WilayahID, int? LadangID, int? UserID, DateTime DTProcess, int? Month, int? Year, string processname, string servicesname, int? ClientID)
        {
            GenSalaryModelHQ db = new GenSalaryModelHQ();
            GetConnectFunc conn = new GetConnectFunc();
            LogFunc LogFunc = new LogFunc();
            string host, catalog, user, pass = "";
            conn.GetConnection(out host, out catalog, out user, out pass, WilayahID, SyarikatID, NegaraID);
            List<tbl_Pkjmast> Pkjmstlist = new List<tbl_Pkjmast>();

            GenSalaryModelEstate db2 = GenSalaryModelEstate.ConnectToSqlServer(host, catalog, user, pass);

            var GetStatusXActv = db.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "sbbTakAktif" && x.fldOptConfFlag2 == "1" && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fldDeleted == false).Select(s => s.fldOptConfValue).ToArray();

            //original code
            //var PkjKerja = db2.tbl_Kerja.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Tarikh.Value.Month == Month && x.fld_Tarikh.Value.Year == Year).Select(s => s.fld_Nopkj).Distinct().ToList();

            //modified by Faeza on 10.04.2020
            var PkjKerja = db2.tbl_Kerjahdr.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Tarikh.Value.Month == Month && x.fld_Tarikh.Value.Year == Year).Select(s => s.fld_Nopkj).Distinct().ToList();

            Pkjmstlist = db2.tbl_Pkjmast.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_StatusApproved == 1 && (x.fld_Kdaktf == "1" || (x.fld_Kdaktf == "2" && GetStatusXActv.Contains(x.fld_Sbtakf))) && PkjKerja.Contains(x.fld_Nopkj)).ToList();

            db.Dispose();
            db2.Dispose();

            return Pkjmstlist;
        }

        public List<CustMod_DateList> GetDateListFunc(int? month, int? year)
        {
            GenSalaryModelHQ db = new GenSalaryModelHQ();
            GetConnectFunc conn = new GetConnectFunc();
            List<CustMod_DateList> DateList = new List<CustMod_DateList>();
            var frstdaythsmnth = new DateTime(int.Parse(year.ToString()), int.Parse(month.ToString()), 1);
            var fstdaynxtmnth = frstdaythsmnth.AddMonths(1);

            for (DateTime date = frstdaythsmnth; date < fstdaynxtmnth; date = date.AddDays(1))
            {
                DateList.Add(new CustMod_DateList() { Date = date });
            }

            return DateList;
        }

        public DateTime? GetDateStarkWorkingFunc(int? NegaraID, int? SyarikatID, int? WilayahID, int? LadangID, string PkjNo, List<tbl_Kerja> tbl_Kerja, List<tbl_Kerjahdr> tbl_Kerjahdr, List<tbl_CutiKategori> tbl_CutiKategori)
        {
            DateTime? startworkdate = new DateTime();

            startworkdate = tbl_Kerja.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Nopkj == PkjNo).OrderBy(o => o.fld_Tarikh).Select(s => s.fld_Tarikh).Take(1).SingleOrDefault();

            if (startworkdate == null)
            {
                var kodCuti = tbl_CutiKategori.Select(s => s.fld_KodCuti).ToList();
                startworkdate = tbl_Kerjahdr.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Nopkj == PkjNo && kodCuti.Contains(x.fld_Kdhdct)).OrderBy(o => o.fld_Tarikh).Select(s => s.fld_Tarikh).Take(1).SingleOrDefault();
            }

            return startworkdate;
        }

        public List<tbl_CutiKategori> GetPaidLeaveFunc(int? NegaraID, int? SyarikatID, int? WilayahID, int? LadangID)
        {
            GenSalaryModelHQ db = new GenSalaryModelHQ();
            GetConnectFunc conn = new GetConnectFunc();
            List<tbl_CutiKategori> CutiKategoriList = new List<tbl_CutiKategori>();

            CutiKategoriList = db.tbl_CutiKategori.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_Deleted == false).ToList();

            db.Dispose();
            return CutiKategoriList;
        }

        public List<tbl_JenisAktiviti> GetActvtyTypeBonusStatusFunc(int? NegaraID, int? SyarikatID, int? WilayahID, int? LadangID)
        {
            GenSalaryModelHQ db = new GenSalaryModelHQ();
            List<tbl_JenisAktiviti> JenisAktiviti = new List<tbl_JenisAktiviti>();

            JenisAktiviti = db.tbl_JenisAktiviti.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_DisabledFlag != 3 && x.fld_Deleted == false).ToList();

            db.Dispose();
            return JenisAktiviti;
        }

        public List<tbl_JenisAktiviti> GetActvtyTypeBonusStatus2Func(int? NegaraID, int? SyarikatID, int? WilayahID, int? LadangID)
        {
            GenSalaryModelHQ db = new GenSalaryModelHQ();
            List<tbl_JenisAktiviti> JenisAktiviti = new List<tbl_JenisAktiviti>();

            JenisAktiviti = db.tbl_JenisAktiviti.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_Deleted == false).ToList();

            db.Dispose();
            return JenisAktiviti;
        }

        public List<tbl_JenisInsentif> GetIncentifsTypeFunc(int? NegaraID, int? SyarikatID)
        {
            GenSalaryModelHQ db = new GenSalaryModelHQ();
            List<tbl_JenisInsentif> insentifsType = new List<tbl_JenisInsentif>();

            insentifsType = db.tbl_JenisInsentif.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_Deleted == false).ToList();

            db.Dispose();
            return insentifsType;
        }

        public tbl_HargaSawitSemasa GetHargaSawitSemasa(int? NegaraID, int? SyarikatID, int? Month, int? Year)
        {
            GenSalaryModelHQ db = new GenSalaryModelHQ();
            DateTime CurrentMonth = new DateTime(Year.Value, Month.Value, 15);

            //added by faeza 13.10.2023
            int LastMonth = CurrentMonth.Month;
            int LastYear = CurrentMonth.Year;
            return db.tbl_HargaSawitSemasa.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_Bulan == LastMonth && x.fld_Tahun == LastYear).FirstOrDefault();
        }

        public decimal GetBonusHarianBukanMenuai(int? NegaraID, int? SyarikatID)
        {
            GenSalaryModelHQ db = new GenSalaryModelHQ();
            var bonusHarianBukanMenuai = decimal.Parse(db.tblOptionConfigsWebs.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fldDeleted == false && x.fldOptConfFlag1 == "bonusHarian").Select(s => s.fldOptConfValue).FirstOrDefault());
            return bonusHarianBukanMenuai;
        }

        public List<tblOptionConfigsWeb> GetWebConfigList(string[] flag1, int? NegaraID, int? SyarikatID)
        {
            GenSalaryModelHQ db = new GenSalaryModelHQ();
            var getConfigList = db.tblOptionConfigsWebs
                .Where(x => flag1.Contains(x.fldOptConfFlag1) && x.fldDeleted == false &&
                            x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID)
                .ToList();

            return getConfigList;
        }

        public tbl_GajiMinimaLdg GetGajiMinimaLdg(int? LadangID)
        {
            GenSalaryModelHQ db = new GenSalaryModelHQ();
            return db.tbl_GajiMinimaLdg.Where(x => x.fld_LadangID == LadangID && x.fld_Deleted == false).FirstOrDefault();
        }

        public List<tbl_UpahAktiviti> tbl_UpahAktiviti(int? NegaraID, int? SyarikatID)
        {
            GenSalaryModelHQ db = new GenSalaryModelHQ();
            return db.tbl_UpahAktiviti.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID).ToList();
        }

        public List<tbl_JenisInsentif> tbl_JenisInsentif(int? NegaraID, int? SyarikatID)
        {
            GenSalaryModelHQ db = new GenSalaryModelHQ();
            return db.tbl_JenisInsentif.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_Deleted == false).ToList();
        }

        public List<tbl_Kwsp> tbl_Kwsp(int? NegaraID, int? SyarikatID)
        {
            GenSalaryModelHQ db = new GenSalaryModelHQ();
            return db.tbl_Kwsp.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_Deleted == false).ToList();
        }

        public List<tbl_Socso> tbl_Socso(int? NegaraID, int? SyarikatID)
        {
            GenSalaryModelHQ db = new GenSalaryModelHQ();
            return db.tbl_Socso.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_Deleted == false).ToList();
        }

        public List<tbl_CarumanTambahan> tbl_CarumanTambahan(int? NegaraID, int? SyarikatID)
        {
            GenSalaryModelHQ db = new GenSalaryModelHQ();
            return db.tbl_CarumanTambahan.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_Deleted == false).ToList();
        }
        public List<tbl_SubCarumanTambahan> tbl_SubCarumanTambahan(int? NegaraID, int? SyarikatID)
        {
            GenSalaryModelHQ db = new GenSalaryModelHQ();
            return db.tbl_SubCarumanTambahan.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_Deleted == false).ToList();
        }
        public List<tbl_JadualCarumanTambahan> tbl_JadualCarumanTambahan(int? NegaraID, int? SyarikatID)
        {
            GenSalaryModelHQ db = new GenSalaryModelHQ();
            return db.tbl_JadualCarumanTambahan.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_Deleted == false).ToList();
        }
    }
}
