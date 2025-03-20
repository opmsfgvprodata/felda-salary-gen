using SalaryGeneratorServices.CustomModels;
using SalaryGeneratorServices.ModelsEstate;
using SalaryGeneratorServices.ModelsHQ;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using SalaryGeneratorServices.ModelsCustom;
using System.Data.Entity.Migrations.Model;

namespace SalaryGeneratorServices.FuncClass
{
    public class Step3Func
    {

        public List<tbl_Pkjmast> tbl_Pkjmasts;
        public void GetPkjMastsData(List<tbl_Pkjmast> Pkjmasts)
        {
            tbl_Pkjmasts = Pkjmasts;
        }

        public Guid GetPaidWorkingFunc(int? NegaraID, int? SyarikatID, int? WilayahID, int? LadangID, int? UserID, DateTime DTProcess, int? Month, int? Year, string processname, string servicesname, int? ClientID, string NoPkj, out decimal? WorkingPayment, out decimal? DiffAreaPayment, List<tbl_Kerja> tbl_Kerja)
        {
            GetConnectFunc conn = new GetConnectFunc();
            Guid MonthSalaryID = new Guid();
            tbl_GajiBulanan GajiBulanan = new tbl_GajiBulanan();
            string host, catalog, user, pass = "";
            conn.GetConnection(out host, out catalog, out user, out pass, WilayahID, SyarikatID, NegaraID);
            GenSalaryModelEstate db2 = GenSalaryModelEstate.ConnectToSqlServer(host, catalog, user, pass);

            var DataKerja = tbl_Kerja.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Tarikh.Value.Month == Month && x.fld_Tarikh.Value.Year == Year && x.fld_Nopkj == NoPkj).ToList();//.Sum(s => s.fld_Amount);
            var PaidWorking = DataKerja.Sum(s => s.fld_OverallAmount);
            if (PaidWorking == null)
            {
                PaidWorking = 0;
            }
            WorkingPayment = PaidWorking;
            MonthSalaryID = AddTo_tbl_GajiBulanan(db2, NegaraID, SyarikatID, WilayahID, LadangID, Month, Year, NoPkj, 1, PaidWorking, DTProcess, UserID, GajiBulanan);

            var PaidDifficultArea = DataKerja.Sum(s => s.fld_HrgaKwsnSkar);
            if (PaidDifficultArea == null)
            {
                PaidDifficultArea = 0;
            }
            DiffAreaPayment = PaidDifficultArea;

            AddTo_tbl_GajiBulanan(db2, NegaraID, SyarikatID, WilayahID, LadangID, Month, Year, NoPkj, 19, PaidDifficultArea, DTProcess, UserID, GajiBulanan);

            db2.Dispose();

            return MonthSalaryID;
        }

        public decimal? GetAveragePaidFunc(int? NegaraID, int? SyarikatID, int? WilayahID, int? LadangID, int? UserID, DateTime DTProcess, int? Month, int? Year, string processname, string servicesname, int? ClientID, string NoPkj, Guid Guid, List<tblOptionConfigsWeb> tblOptionConfigsWeb, List<tbl_Kerjahdr> tbl_Kerjahdr)
        {
            GetConnectFunc conn = new GetConnectFunc();
            tbl_GajiBulanan GajiBulanan = new tbl_GajiBulanan();
            GenSalaryModelHQ db = new GenSalaryModelHQ();
            decimal? AverageSalary = 0;
            decimal? AveragePaid = 0;
            int YearC = int.Parse(Year.ToString());
            int MonthC = int.Parse(Month.ToString());
            DateTime EndSelectDateOri = new DateTime(YearC, MonthC, 15);
            DateTime EndSelectDate = EndSelectDateOri.AddMonths(-1);
            DateTime StartSelectDate = EndSelectDate.AddMonths(-6);
            string host, catalog, user, pass = "";
            conn.GetConnection(out host, out catalog, out user, out pass, WilayahID, SyarikatID, NegaraID);
            GenSalaryModelEstate db2 = GenSalaryModelEstate.ConnectToSqlServer(host, catalog, user, pass);

            GajiBulanan = db2.tbl_GajiBulanan.Find(Guid);
            var Attandance = tblOptionConfigsWeb.Where(x => x.fldOptConfFlag1 == "cuti" && x.fldOptConfFlag2 == "hadirkerja" && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fldDeleted == false).Select(s => s.fldOptConfValue).ToList();
            var TotalWorkingDay = tbl_Kerjahdr.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Tarikh.Value.Month == Month && x.fld_Tarikh.Value.Year == Year && x.fld_Nopkj == NoPkj && Attandance.Contains(x.fld_Kdhdct)).Count();
            ///var TotalWorkingDay = db2.tbl_Produktiviti.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Month == Month && x.fld_Year == Year && x.fld_Nopkj == NoPkj).Select(s => s.fld_HadirKerja).FirstOrDefault();

            //original code
            //var AveragePaid = GajiBulanan.fld_ByrKerja / TotalWorkingDay;

            //modified by Faeza on 10.04.2020
            if (GajiBulanan.fld_ByrKerja > 0 && TotalWorkingDay > 0)
            {
                AveragePaid = GajiBulanan.fld_ByrKerja / TotalWorkingDay;
            }
            else
            {
                AveragePaid = 0;
            }
            AddTo_tbl_GajiBulanan(db2, NegaraID, SyarikatID, WilayahID, LadangID, Month, Year, NoPkj, 6, Math.Round(decimal.Parse(AveragePaid.ToString()), 2), DTProcess, UserID, GajiBulanan);

            //var GetAverageSalary = db2.tbl_GajiBulanan.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Nopkj == NoPkj && (x.fld_Year == StartSelectDate.Year && x.fld_Month >= StartSelectDate.Month) && (x.fld_Year == EndSelectDate.Year && x.fld_Month <= EndSelectDate.Month)).ToList();
            var TotalSalary = db2.tbl_GajiBulanan.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Nopkj == NoPkj && (x.fld_Month >= 1 && x.fld_Month <= Month) && x.fld_Year == Year).Sum(s => s.fld_ByrKerja);
            //var TotalAtt = db2.tbl_Produktiviti.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && (x.fld_Month >= 1 && x.fld_Month <= Month) && x.fld_Year == Year && x.fld_Nopkj == NoPkj).Sum(s => s.fld_HadirKerja);
            var TotalAtt = tbl_Kerjahdr.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Tarikh.Value.Year == Year && x.fld_Nopkj == NoPkj && Attandance.Contains(x.fld_Kdhdct)).Count();

            //original code
            //AverageSalary = TotalSalary / TotalAtt;

            //modified by Faeza on 10.04.2020
            if (TotalSalary > 0 && TotalAtt > 0)
            {
                AverageSalary = TotalSalary / TotalAtt;
            }
            else
            {
                AverageSalary = 0;
            }

            AddTo_tbl_GajiBulanan(db2, NegaraID, SyarikatID, WilayahID, LadangID, Month, Year, NoPkj, 18, Math.Round(decimal.Parse(AverageSalary.ToString()), 2), DTProcess, UserID, GajiBulanan);
            db.Dispose();
            db2.Dispose();
            return Math.Round(decimal.Parse(AveragePaid.ToString()), 2);
        }

        public decimal? GetPaidDailyBonusFunc(int? NegaraID, int? SyarikatID, int? WilayahID, int? LadangID, int? UserID, DateTime DTProcess, int? Month, int? Year, string processname, string servicesname, int? ClientID, string NoPkj, Guid Guid, List<tbl_KerjaBonus> tbl_KerjaBonus)
        {
            GetConnectFunc conn = new GetConnectFunc();
            tbl_GajiBulanan GajiBulanan = new tbl_GajiBulanan();
            string host, catalog, user, pass = "";
            conn.GetConnection(out host, out catalog, out user, out pass, WilayahID, SyarikatID, NegaraID);
            GenSalaryModelEstate db2 = GenSalaryModelEstate.ConnectToSqlServer(host, catalog, user, pass);

            var PaidDailyBonus = tbl_KerjaBonus.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Tarikh.Value.Month == Month && x.fld_Tarikh.Value.Year == Year && x.fld_Nopkj == NoPkj).Sum(s => s.fld_Jumlah);
            if (PaidDailyBonus == null)
            {
                PaidDailyBonus = 0;
            }
            GajiBulanan = db2.tbl_GajiBulanan.Find(Guid);
            AddTo_tbl_GajiBulanan(db2, NegaraID, SyarikatID, WilayahID, LadangID, Month, Year, NoPkj, 2, PaidDailyBonus, DTProcess, UserID, GajiBulanan);

            db2.Dispose();
            return PaidDailyBonus;
        }

        public decimal? GetPaidOTFunc(int? NegaraID, int? SyarikatID, int? WilayahID, int? LadangID, int? UserID, DateTime DTProcess, int? Month, int? Year, string processname, string servicesname, int? ClientID, string NoPkj, Guid Guid, List<tbl_KerjaOT> tbl_KerjaOT)
        {
            GetConnectFunc conn = new GetConnectFunc();
            tbl_GajiBulanan GajiBulanan = new tbl_GajiBulanan();
            string host, catalog, user, pass = "";
            conn.GetConnection(out host, out catalog, out user, out pass, WilayahID, SyarikatID, NegaraID);
            GenSalaryModelEstate db2 = GenSalaryModelEstate.ConnectToSqlServer(host, catalog, user, pass);

            var PaidOT = tbl_KerjaOT.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Tarikh.Value.Month == Month && x.fld_Tarikh.Value.Year == Year && x.fld_Nopkj == NoPkj).Sum(s => s.fld_Jumlah);
            if (PaidOT == null)
            {
                PaidOT = 0;
            }
            GajiBulanan = db2.tbl_GajiBulanan.Find(Guid);
            AddTo_tbl_GajiBulanan(db2, NegaraID, SyarikatID, WilayahID, LadangID, Month, Year, NoPkj, 3, PaidOT, DTProcess, UserID, GajiBulanan);

            db2.Dispose();
            return PaidOT;
        }

        public decimal? GetPaidInsentifFunc(int? NegaraID, int? SyarikatID, int? WilayahID, int? LadangID, int? UserID, DateTime DTProcess, int? Month, int? Year, string processname, string servicesname, int? ClientID, string NoPkj, Guid Guid, out List<tbl_Insentif> WorkerIncentifs, List<tbl_JenisInsentif> tbl_JenisInsentif, List<tbl_Insentif> tbl_Insentif)
        {
            GenSalaryModelHQ db = new GenSalaryModelHQ();
            GetConnectFunc conn = new GetConnectFunc();
            tbl_GajiBulanan GajiBulanan = new tbl_GajiBulanan();
            string host, catalog, user, pass = "";
            conn.GetConnection(out host, out catalog, out user, out pass, WilayahID, SyarikatID, NegaraID);
            GenSalaryModelEstate db2 = GenSalaryModelEstate.ConnectToSqlServer(host, catalog, user, pass);
            decimal? TotalGetInsentif = 0;
            WorkerIncentifs = new List<tbl_Insentif>();

            var InsCd = tbl_JenisInsentif.Where(x => x.fld_JenisInsentif == "P" && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_Deleted == false).Select(s => new { s.fld_KodInsentif, s.fld_TetapanNilai, s.fld_DailyFixedValue, s.fld_MaxValue }).ToList();
            var InsCdForDailyValue = InsCd.Where(x => x.fld_TetapanNilai == 2).ToList();
            var InsCdForOtherValue = InsCd.Where(x => x.fld_TetapanNilai != 2).ToList();
            var PaidInsentif = tbl_Insentif.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Month == Month && x.fld_Year == Year && x.fld_Nopkj == NoPkj && x.fld_Deleted == false).ToList();
            var PaidInsentifOthers = PaidInsentif.Where(x => InsCdForOtherValue.Select(s => s.fld_KodInsentif).Contains(x.fld_KodInsentif)).Sum(s => s.fld_NilaiInsentif);

            if (PaidInsentifOthers == null)
            {
                PaidInsentifOthers = 0;
            }

            decimal? PeruntukkanInsentif = 0;
            decimal? TerimaInsentif = 0;
            decimal? DeductPerDay = 0;
            decimal? TotalDeduct = 0;
            decimal? TotalTerimaInsentif = 0;
            if (InsCdForDailyValue != null)
            {
                var PaidInsentifDaily = PaidInsentif.Where(x => InsCdForDailyValue.Select(s => s.fld_KodInsentif).Contains(x.fld_KodInsentif)).ToList();
                if (PaidInsentifDaily != null)
                {
                    foreach (var PaidInsentifDay in PaidInsentifDaily)
                    {
                        var PaidInsentifForDailyDec = PaidInsentif.Where(x => x.fld_KodInsentif == PaidInsentifDay.fld_KodInsentif).FirstOrDefault();
                        PeruntukkanInsentif = InsCdForDailyValue.Where(x => x.fld_KodInsentif == PaidInsentifDay.fld_KodInsentif).Select(s => s.fld_MaxValue).FirstOrDefault();
                        DeductPerDay = InsCdForDailyValue.Where(x => x.fld_KodInsentif == PaidInsentifDay.fld_KodInsentif).Select(s => s.fld_DailyFixedValue).FirstOrDefault();
                        var GetTotalPonteng = 0;//db2.tbl_Kerjahdr.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Nopkj == NoPkj && x.fld_Tarikh.Value.Month == Month && x.fld_Tarikh.Value.Year == Year && x.fld_Kdhdct == "P01").Count();
                        if (GetTotalPonteng > 0)
                        {
                            TotalDeduct = DeductPerDay * GetTotalPonteng;
                            TerimaInsentif = PeruntukkanInsentif - TotalDeduct;
                            PaidInsentifForDailyDec.fld_NilaiInsentif = TerimaInsentif;
                            db2.Entry(PaidInsentifForDailyDec).State = EntityState.Modified;
                            db2.SaveChanges();
                        }
                        else
                        {
                            TerimaInsentif = PeruntukkanInsentif;
                            PaidInsentifForDailyDec.fld_NilaiInsentif = TerimaInsentif;
                            db2.Entry(PaidInsentifForDailyDec).State = EntityState.Modified;
                            db2.SaveChanges();
                        }
                        WorkerIncentifs.Add(PaidInsentifForDailyDec);
                        TotalTerimaInsentif = TotalTerimaInsentif + TerimaInsentif;
                        TerimaInsentif = 0;
                    }
                }
            }

            TotalGetInsentif = PaidInsentifOthers + TotalTerimaInsentif;

            GajiBulanan = db2.tbl_GajiBulanan.Find(Guid);
            AddTo_tbl_GajiBulanan(db2, NegaraID, SyarikatID, WilayahID, LadangID, Month, Year, NoPkj, 4, TotalGetInsentif, DTProcess, UserID, GajiBulanan);

            db.Dispose();
            db2.Dispose();
            return TotalGetInsentif;
        }

        public decimal? GetDeductInsentifFunc(int? NegaraID, int? SyarikatID, int? WilayahID, int? LadangID, int? UserID, DateTime DTProcess, int? Month, int? Year, string processname, string servicesname, int? ClientID, string NoPkj, Guid Guid, List<tbl_JenisInsentif> tbl_JenisInsentif, List<tbl_Insentif> tbl_Insentif)
        {
            GenSalaryModelHQ db = new GenSalaryModelHQ();
            GetConnectFunc conn = new GetConnectFunc();
            tbl_GajiBulanan GajiBulanan = new tbl_GajiBulanan();
            string host, catalog, user, pass = "";
            conn.GetConnection(out host, out catalog, out user, out pass, WilayahID, SyarikatID, NegaraID);
            GenSalaryModelEstate db2 = GenSalaryModelEstate.ConnectToSqlServer(host, catalog, user, pass);

            var InsCd = tbl_JenisInsentif.Where(x => x.fld_JenisInsentif == "T" && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_Deleted == false).Select(s => s.fld_KodInsentif).ToList();
            var DeductInsentif = tbl_Insentif.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Month == Month && x.fld_Year == Year && x.fld_Nopkj == NoPkj && InsCd.Contains(x.fld_KodInsentif) && x.fld_Deleted == false).Sum(s => s.fld_NilaiInsentif);
            if (DeductInsentif == null)
            {
                DeductInsentif = 0;
            }
            GajiBulanan = db2.tbl_GajiBulanan.Find(Guid);
            AddTo_tbl_GajiBulanan(db2, NegaraID, SyarikatID, WilayahID, LadangID, Month, Year, NoPkj, 5, DeductInsentif, DTProcess, UserID, GajiBulanan);

            db.Dispose();
            db2.Dispose();
            return DeductInsentif;
        }

        public decimal? GetAIPSFunc(int? NegaraID, int? SyarikatID, int? WilayahID, int? LadangID, int? UserID, DateTime DTProcess, int? Month, int? Year, string processname, string servicesname, int? ClientID, string NoPkj, Guid Guid, List<tbl_Produktiviti> tbl_Produktiviti)
        {
            GenSalaryModelHQ db = new GenSalaryModelHQ();
            GetConnectFunc conn = new GetConnectFunc();
            tbl_GajiBulanan GajiBulanan = new tbl_GajiBulanan();
            string host, catalog, user, pass = "";
            conn.GetConnection(out host, out catalog, out user, out pass, WilayahID, SyarikatID, NegaraID);
            GenSalaryModelEstate db2 = GenSalaryModelEstate.ConnectToSqlServer(host, catalog, user, pass);

            decimal? AIPSPrice = 0;
            decimal? AttInsentif = 0;
            decimal? QualityInsentif = 0;
            decimal? ProdInsentif = 0;
            int? AttTarget = 0;
            int? AttCapai = 0;
            short? QuaTarget = 0;
            short? QuaCapai = 0;
            decimal? ProdTarget = 0;
            decimal? ProdCapai = 0;


            var GetWorkerProdPlan = tbl_Produktiviti.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Month == Month && x.fld_Year == Year && x.fld_Nopkj == NoPkj && x.fld_Deleted == false).FirstOrDefault();

            if (GetWorkerProdPlan != null)
            {
                switch (GetWorkerProdPlan.fld_JenisPelan)
                {
                    case "A":
                        AttInsentif = GetAttInsentifFunc(db, db2, NegaraID, SyarikatID, WilayahID, LadangID, Month, Year, NoPkj, GetWorkerProdPlan, out AttTarget, out AttCapai);
                        QualityInsentif = GetQualityInsentifFunc(db, db2, NegaraID, SyarikatID, WilayahID, LadangID, Month, Year, NoPkj, out QuaTarget, out QuaCapai);
                        ProdInsentif = GetProductivityInsentifFunc(db, db2, NegaraID, SyarikatID, WilayahID, LadangID, Month, Year, NoPkj, GetWorkerProdPlan, out ProdTarget, out ProdCapai);
                        break;
                    case "B":
                        AttInsentif = GetAttInsentifFunc(db, db2, NegaraID, SyarikatID, WilayahID, LadangID, Month, Year, NoPkj, GetWorkerProdPlan, out AttTarget, out AttCapai);
                        QualityInsentif = 0;
                        ProdInsentif = GetProductivityInsentifFunc(db, db2, NegaraID, SyarikatID, WilayahID, LadangID, Month, Year, NoPkj, GetWorkerProdPlan, out ProdTarget, out ProdCapai);
                        break;
                }
            }

            AIPSPrice = AttInsentif + QualityInsentif + ProdInsentif;

            GajiBulanan = db2.tbl_GajiBulanan.Find(Guid);
            AddTo_tbl_GajiBulanan(db2, NegaraID, SyarikatID, WilayahID, LadangID, Month, Year, NoPkj, 7, AIPSPrice, DTProcess, UserID, GajiBulanan);
            AddTo_tbl_GajiBulanan(db2, NegaraID, SyarikatID, WilayahID, LadangID, Month, Year, NoPkj, 15, AttInsentif, DTProcess, UserID, GajiBulanan);
            AddTo_tbl_GajiBulanan(db2, NegaraID, SyarikatID, WilayahID, LadangID, Month, Year, NoPkj, 16, QualityInsentif, DTProcess, UserID, GajiBulanan);
            AddTo_tbl_GajiBulanan(db2, NegaraID, SyarikatID, WilayahID, LadangID, Month, Year, NoPkj, 17, ProdInsentif, DTProcess, UserID, GajiBulanan);

            AttTarget = AttTarget == null ? 0 : AttTarget;
            AttCapai = AttCapai == null ? 0 : AttCapai;
            QuaTarget = QuaTarget == null ? 0 : QuaTarget;
            QuaCapai = QuaCapai == null ? 0 : QuaCapai;
            ProdTarget = ProdTarget == null ? 0 : ProdTarget;
            ProdCapai = ProdCapai == null ? 0 : ProdCapai;

            AddTo_tbl_GajiBulanan2(db2, 1, AttTarget, AttCapai, QuaTarget, QuaCapai, ProdTarget, ProdCapai, GajiBulanan);

            db.Dispose();
            db2.Dispose();

            return AIPSPrice;
        }

        public decimal? GetAttInsentifFunc(GenSalaryModelHQ db, GenSalaryModelEstate db2, int? NegaraID, int? SyarikatID, int? WilayahID, int? LadangID, int? Month, int? Year, string NoPkj, tbl_Produktiviti GetTargetWorking, out int? AttTarget, out int? AttCapai)
        {
            decimal? AttInsentif = 0;

            var GetAbsent = db.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "pontengaips" && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fldDeleted == false).Select(s => s.fldOptConfValue).ToList();

            var GetWorking = db2.tbl_Kerjahdr.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Tarikh.Value.Month == Month && x.fld_Tarikh.Value.Year == Year && x.fld_Nopkj == NoPkj && !GetAbsent.Contains(x.fld_Kdhdct)).ToList();

            if (GetWorking.Count >= GetTargetWorking.fld_HadirKerja)
            {
                AttInsentif = decimal.Parse(db.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "aipskehadiranF" && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fldDeleted == false).Select(s => s.fldOptConfValue).FirstOrDefault());
            }
            else if (GetWorking.Count == GetTargetWorking.fld_HadirKerja - 1)
            {
                AttInsentif = decimal.Parse(db.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "aipskehadiranNF" && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fldDeleted == false).Select(s => s.fldOptConfValue).FirstOrDefault());
            }
            else
            {
                AttInsentif = 0;
            }

            AttTarget = GetTargetWorking.fld_HadirKerja;

            AttCapai = GetWorking.Count;

            return AttInsentif;
        }

        public decimal? GetQualityInsentifFunc(GenSalaryModelHQ db, GenSalaryModelEstate db2, int? NegaraID, int? SyarikatID, int? WilayahID, int? LadangID, int? Month, int? Year, string NoPkj, out short? QuaTarget, out short? QuaCapai)
        {
            decimal? QualityInsentif = 0;

            var GetQualityStatus = db2.tbl_Kerja.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Tarikh.Value.Month == Month && x.fld_Tarikh.Value.Year == Year && x.fld_Nopkj == NoPkj && x.fld_Quality != null && x.fld_Unit == "TAN").Sum(s => s.fld_Quality);

            if (GetQualityStatus <= 0 || GetQualityStatus == null)
            {
                QualityInsentif = decimal.Parse(db.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "aipskualiti" && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fldDeleted == false).Select(s => s.fldOptConfValue).FirstOrDefault());
                QuaCapai = 0;
            }
            else
            {
                QualityInsentif = 0;
                QuaCapai = null;
            }

            QuaTarget = 0;

            return QualityInsentif;
        }

        public decimal? GetProductivityInsentifFunc(GenSalaryModelHQ db, GenSalaryModelEstate db2, int? NegaraID, int? SyarikatID, int? WilayahID, int? LadangID, int? Month, int? Year, string NoPkj, tbl_Produktiviti GetTargetWorking, out decimal? ProdTarget, out decimal? ProdCapai)
        {
            decimal? ProdInsentif = 0;
            decimal? ActualDailyTarget = 0;
            decimal? HalfDailyTarget = 0;

            var GetProductivityStatus = db2.tbl_Kerja.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Tarikh.Value.Month == Month && x.fld_Tarikh.Value.Year == Year && x.fld_Nopkj == NoPkj && x.fld_Unit.Contains(GetTargetWorking.fld_Unit)).Sum(s => s.fld_JumlahHasil);

            var CountWorkDay = db2.tbl_Kerja.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Tarikh.Value.Month == Month && x.fld_Tarikh.Value.Year == Year && x.fld_Nopkj == NoPkj && x.fld_Unit.Contains(GetTargetWorking.fld_Unit)).Select(s => s.fld_Tarikh).Distinct().Count();

            ActualDailyTarget = GetProductivityStatus / CountWorkDay;

            HalfDailyTarget = GetTargetWorking.fld_Targetharian / 2;

            if (ActualDailyTarget >= GetTargetWorking.fld_Targetharian)
            {
                ProdInsentif = GetTargetWorking.fld_JenisPelan == "A" ? decimal.Parse(db.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "aipsprodFA" && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fldDeleted == false).Select(s => s.fldOptConfValue).FirstOrDefault()) : decimal.Parse(db.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "aipsprodFB" && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fldDeleted == false).Select(s => s.fldOptConfValue).FirstOrDefault());
            }
            else if (ActualDailyTarget >= HalfDailyTarget && ActualDailyTarget < GetTargetWorking.fld_Targetharian)
            {
                ProdInsentif = GetTargetWorking.fld_JenisPelan == "A" ? decimal.Parse(db.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "aipsprodHA" && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fldDeleted == false).Select(s => s.fldOptConfValue).FirstOrDefault()) : decimal.Parse(db.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "aipsprodHB" && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fldDeleted == false).Select(s => s.fldOptConfValue).FirstOrDefault());
            }
            else
            {
                ProdInsentif = 0;
            }

            ProdTarget = GetTargetWorking.fld_Targetharian;

            ProdCapai = ActualDailyTarget;

            return ProdInsentif;
        }

        public decimal? GetPaidLeaveFunc(int? NegaraID, int? SyarikatID, int? WilayahID, int? LadangID, int? UserID, DateTime DTProcess, int? Month, int? Year, string processname, string servicesname, int? ClientID, string NoPkj, Guid Guid, List<CustMod_WorkerPaidLeave> WorkerPaidLeaveLists, DateTime? StartWorkDate, bool NoLeave, List<tbl_CutiKategori> CutiKategoriList, tbl_Pkjmast tbl_Pkjmast, tbl_GajiMinimaLdg tbl_GajiMinimaLdg, List<tbl_GajiBulanan> tbl_GajiBulanan_Lepas, List<tbl_Kerjahdr> tbl_Kerjahdr, List<tblOptionConfigsWeb> tblOptionConfigsWeb, List<tbl_CutiPeruntukan> tbl_CutiPeruntukan, string compCode, List<tbl_Insentif> WorkerIncentifs, List<tbl_JenisInsentif> IncentifsType, List<vw_KerjaInfoDetails> vw_KerjaInfoDetails, List<vw_Kerja_Bonus> vw_Kerja_Bonus, List<tbl_Kerjahdr> tbl_Kerjahdr12Month)
        {
            GetConnectFunc conn = new GetConnectFunc();
            Step2Func Step2Func = new Step2Func();
            string host, catalog, user, pass = "";
            conn.GetConnection(out host, out catalog, out user, out pass, WilayahID, SyarikatID, NegaraID);
            GenSalaryModelEstate db2 = GenSalaryModelEstate.ConnectToSqlServer(host, catalog, user, pass);
            tbl_GajiBulanan GajiBulanan = new tbl_GajiBulanan();
            int YearC = int.Parse(Year.ToString());
            int MonthC = int.Parse(Month.ToString());
            DateTime SelectDateOri = new DateTime(YearC, MonthC, 15);
            DateTime LastSelectMonthDate = SelectDateOri.AddMonths(-1);
            GenSalaryModelHQ db = new GenSalaryModelHQ();
            int LastMonth = int.Parse(LastSelectMonthDate.Month.ToString());

            decimal? LeavePayment = 0;
            decimal? TotalPaidLeave = 0;
            decimal? TotalPaidLeave2 = 0;
            decimal? TotalPaidLeave3 = 0;
            decimal? OverAllPaidLeave = 0;
            decimal? AverageSalary = 0;
            decimal? AverageSalaryLastMonth = 0;
            decimal? Kong = 0;
            decimal? AverageSalary12Month = 0;
            int? CheckPeruntukkan = 0;

            List<tbl_KerjahdrCuti> KerjahdrCutiList = new List<tbl_KerjahdrCuti>();
            tbl_KerjahdrCutiTahunan KerjahdrCutiTahunan = new tbl_KerjahdrCutiTahunan();

            var lastYear = Year - 1;
            var tbl_GajiBulananList = db2.tbl_GajiBulanan.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Nopkj == NoPkj && ((x.fld_Year == Year && x.fld_Month <= 12) || (x.fld_Year == lastYear && x.fld_Month == 12))).ToList();
            var tbl_GajiBulanan = tbl_GajiBulananList.Where(x => x.fld_ID == Guid).FirstOrDefault();
            AverageSalary = tbl_GajiBulanan.fld_PurataGaji;
            AverageSalary12Month = tbl_GajiBulanan.fld_PurataGaji12Bln;
            AverageSalaryLastMonth = tbl_GajiBulanan_Lepas.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Nopkj == NoPkj && x.fld_Year == LastSelectMonthDate.Year && x.fld_Month == LastSelectMonthDate.Month).Select(s => s.fld_PurataGaji).FirstOrDefault();
            AverageSalary = AverageSalaryLastMonth;
            AverageSalary = AverageSalary == null ? 0 : AverageSalary;
            AverageSalaryLastMonth = AverageSalaryLastMonth == null ? 0 : AverageSalaryLastMonth;

            //modified by kamalia 30/11/21
            var getgajiminima = tbl_GajiMinimaLdg;
            Kong = getgajiminima != null ? getgajiminima.fld_NilaiGajiMinima : Math.Round(decimal.Parse(getgajiminima.fld_NilaiGajiMinima.ToString()), 2);
            AverageSalary = AverageSalary == 0 ? Kong : AverageSalary;

            //modified by kamalia 2/3/2021
            foreach (var WorkerPaidLeaveList in WorkerPaidLeaveLists)
            {
                if (WorkerPaidLeaveList.fld_PaidPeriod != 0)
                {
                    if (WorkerPaidLeaveList.fld_Kdhdct == "C01")
                    {

                        DateTime? TwoDayBefore = WorkerPaidLeaveList.fld_Tarikh.Value.AddDays(-1);
                        DateTime? TwoDayAfter = WorkerPaidLeaveList.fld_Tarikh.Value.AddDays(1);

                        var GetTwoAftBefDayHdrCts = tbl_Kerjahdr.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Nopkj == NoPkj && x.fld_Tarikh >= TwoDayBefore && x.fld_Tarikh <= TwoDayAfter).Select(s => new { s.fld_Tarikh, s.fld_Kdhdct }).OrderBy(o => o.fld_Tarikh).ToList();

                        if (GetTwoAftBefDayHdrCts.Select(s => s.fld_Kdhdct).Contains("P01") == true)
                        {
                            LeavePayment = 0;
                        }
                        else
                        {
                            if (AverageSalary == 0)
                            {
                                if (AverageSalaryLastMonth == 0 || AverageSalaryLastMonth < Kong)
                                {
                                    //Kong = 42.310m;
                                    LeavePayment = Kong;
                                }
                                else
                                {
                                    LeavePayment = Math.Round(decimal.Parse(AverageSalaryLastMonth.ToString()), 2);
                                }
                            }
                            else if (AverageSalary < Kong)
                            {
                                //Kong = 42.310m;
                                LeavePayment = Kong;
                            }
                            else
                            {
                                LeavePayment = Math.Round(decimal.Parse(AverageSalary.ToString()), 2);
                            }

                            KerjahdrCutiList.Add(new tbl_KerjahdrCuti() { fld_Kadar = AverageSalary, fld_Jumlah = LeavePayment, fld_Nopkj = WorkerPaidLeaveList.fld_Nopkj, fld_KerjahdrID = WorkerPaidLeaveList.fld_KerjahdrID, fld_Kum = WorkerPaidLeaveList.fld_Kum, fld_Tarikh = WorkerPaidLeaveList.fld_Tarikh, fld_NegaraID = NegaraID, fld_SyarikatID = SyarikatID, fld_WilayahID = WilayahID, fld_LadangID = LadangID, fld_CreatedBy = UserID, fld_CreatedDT = DTProcess });
                        }

                    }//Cuti Bencana - modified by Shah on 28.03.2020
                    else if (WorkerPaidLeaveList.fld_Kdhdct == "C11")
                    {
                        //modified by kamalia 27/5/2021
                        //AverageSalary = Kong;
                        LeavePayment = Math.Round(decimal.Parse(Kong.ToString()), 2);
                        KerjahdrCutiList.Add(new tbl_KerjahdrCuti() { fld_Kadar = Kong, fld_Jumlah = LeavePayment, fld_Nopkj = WorkerPaidLeaveList.fld_Nopkj, fld_KerjahdrID = WorkerPaidLeaveList.fld_KerjahdrID, fld_Kum = WorkerPaidLeaveList.fld_Kum, fld_Tarikh = WorkerPaidLeaveList.fld_Tarikh, fld_NegaraID = NegaraID, fld_SyarikatID = SyarikatID, fld_WilayahID = WilayahID, fld_LadangID = LadangID, fld_CreatedBy = UserID, fld_CreatedDT = DTProcess });
                    }
                    //add by Faeza on 02.07.2020
                    //modified bt faeza 0n 03.11.2020 - add condition utk C10
                    else if (WorkerPaidLeaveList.fld_Kdhdct == "C03" || WorkerPaidLeaveList.fld_Kdhdct == "C04" || WorkerPaidLeaveList.fld_Kdhdct == "C10")
                    {
                        LeavePayment = Kong;
                        KerjahdrCutiList.Add(new tbl_KerjahdrCuti() { fld_Kadar = Kong, fld_Jumlah = LeavePayment, fld_Nopkj = WorkerPaidLeaveList.fld_Nopkj, fld_KerjahdrID = WorkerPaidLeaveList.fld_KerjahdrID, fld_Kum = WorkerPaidLeaveList.fld_Kum, fld_Tarikh = WorkerPaidLeaveList.fld_Tarikh, fld_NegaraID = NegaraID, fld_SyarikatID = SyarikatID, fld_WilayahID = WilayahID, fld_LadangID = LadangID, fld_CreatedBy = UserID, fld_CreatedDT = DTProcess });

                    }
                    else if (WorkerPaidLeaveList.fld_Kdhdct == "C02")
                    {
                        //modified by Faeza on 03.09.2020
                        if (AverageSalary == 0)
                        {
                            if (AverageSalaryLastMonth == 0 || AverageSalaryLastMonth < Kong)
                            {
                                //Kong = 42.310m;
                                LeavePayment = Kong;
                                KerjahdrCutiList.Add(new tbl_KerjahdrCuti() { fld_Kadar = Kong, fld_Jumlah = LeavePayment, fld_Nopkj = WorkerPaidLeaveList.fld_Nopkj, fld_KerjahdrID = WorkerPaidLeaveList.fld_KerjahdrID, fld_Kum = WorkerPaidLeaveList.fld_Kum, fld_Tarikh = WorkerPaidLeaveList.fld_Tarikh, fld_NegaraID = NegaraID, fld_SyarikatID = SyarikatID, fld_WilayahID = WilayahID, fld_LadangID = LadangID, fld_CreatedBy = UserID, fld_CreatedDT = DTProcess });
                            }
                            else
                            {
                                LeavePayment = Math.Round(decimal.Parse(AverageSalaryLastMonth.ToString()), 2);
                                KerjahdrCutiList.Add(new tbl_KerjahdrCuti() { fld_Kadar = AverageSalaryLastMonth, fld_Jumlah = LeavePayment, fld_Nopkj = WorkerPaidLeaveList.fld_Nopkj, fld_KerjahdrID = WorkerPaidLeaveList.fld_KerjahdrID, fld_Kum = WorkerPaidLeaveList.fld_Kum, fld_Tarikh = WorkerPaidLeaveList.fld_Tarikh, fld_NegaraID = NegaraID, fld_SyarikatID = SyarikatID, fld_WilayahID = WilayahID, fld_LadangID = LadangID, fld_CreatedBy = UserID, fld_CreatedDT = DTProcess });
                            }
                        }
                        else if (AverageSalary < Kong)
                        {
                            //Kong = 42.310m;
                            LeavePayment = Kong;
                            KerjahdrCutiList.Add(new tbl_KerjahdrCuti() { fld_Kadar = Kong, fld_Jumlah = LeavePayment, fld_Nopkj = WorkerPaidLeaveList.fld_Nopkj, fld_KerjahdrID = WorkerPaidLeaveList.fld_KerjahdrID, fld_Kum = WorkerPaidLeaveList.fld_Kum, fld_Tarikh = WorkerPaidLeaveList.fld_Tarikh, fld_NegaraID = NegaraID, fld_SyarikatID = SyarikatID, fld_WilayahID = WilayahID, fld_LadangID = LadangID, fld_CreatedBy = UserID, fld_CreatedDT = DTProcess });
                        }
                        else
                        {
                            LeavePayment = Math.Round(decimal.Parse(AverageSalary.ToString()), 2);
                            KerjahdrCutiList.Add(new tbl_KerjahdrCuti() { fld_Kadar = AverageSalary, fld_Jumlah = LeavePayment, fld_Nopkj = WorkerPaidLeaveList.fld_Nopkj, fld_KerjahdrID = WorkerPaidLeaveList.fld_KerjahdrID, fld_Kum = WorkerPaidLeaveList.fld_Kum, fld_Tarikh = WorkerPaidLeaveList.fld_Tarikh, fld_NegaraID = NegaraID, fld_SyarikatID = SyarikatID, fld_WilayahID = WilayahID, fld_LadangID = LadangID, fld_CreatedBy = UserID, fld_CreatedDT = DTProcess });
                        }
                    }
                    else
                    {
                        if (AverageSalary < Kong)
                        {
                            AverageSalary = Kong;
                        }
                        LeavePayment = Math.Round(decimal.Parse(AverageSalary.ToString()), 2);
                        KerjahdrCutiList.Add(new tbl_KerjahdrCuti() { fld_Kadar = AverageSalary, fld_Jumlah = LeavePayment, fld_Nopkj = WorkerPaidLeaveList.fld_Nopkj, fld_KerjahdrID = WorkerPaidLeaveList.fld_KerjahdrID, fld_Kum = WorkerPaidLeaveList.fld_Kum, fld_Tarikh = WorkerPaidLeaveList.fld_Tarikh, fld_NegaraID = NegaraID, fld_SyarikatID = SyarikatID, fld_WilayahID = WilayahID, fld_LadangID = LadangID, fld_CreatedBy = UserID, fld_CreatedDT = DTProcess });
                    }
                }
                TotalPaidLeave = TotalPaidLeave + LeavePayment;
                LeavePayment = 0;
            }

            GajiBulanan = db2.tbl_GajiBulanan.Find(Guid);
            AddTo_tbl_GajiBulanan(db2, NegaraID, SyarikatID, WilayahID, LadangID, Month, Year, NoPkj, 8, TotalPaidLeave, DTProcess, UserID, GajiBulanan);
      
            var oRP = GetORPFunc(NegaraID, SyarikatID, WilayahID, LadangID, UserID, DTProcess, Month, Year, processname, servicesname, ClientID, NoPkj, Guid, WorkerIncentifs, IncentifsType, vw_KerjaInfoDetails, vw_Kerja_Bonus, CutiKategoriList, tbl_Kerjahdr, WorkerPaidLeaveLists);
            //WriteLog("Get ORP. (Data - No Pkj : " + Pkjmstlist.fld_Nopkj.Trim() + ", Total Payment : RM " + oRP + ")", false, processname, ServiceProcessID);

            var GetStatusXActv = tblOptionConfigsWeb.Where(x => x.fldOptConfFlag1 == "sbbTakAktif" && x.fldOptConfFlag2 == "1" && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fldDeleted == false).Select(s => s.fldOptConfValue).ToArray();
            var PkjStatus = tbl_Pkjmast;
            var KodCutiTahunan = CutiKategoriList.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_KodCuti == "C02").FirstOrDefault();

            var gajiSatuTahun = tbl_GajiBulananList.Where(x => x.fld_Year == Year && x.fld_Month <= 12).ToList();
            //LeavePayment = compCode == "1000" ? gajiSatuTahun.Sum(s => s.fld_PurataGaji) / gajiSatuTahun.Count : Kong;
            LeavePayment = gajiSatuTahun.Sum(s => s.fld_PurataGaji) / gajiSatuTahun.Count;

            // cuti tahunan sahaja
            if (GetStatusXActv.Contains(PkjStatus.fld_Sbtakf) == true && PkjStatus.fld_Kdaktf == "2" && Month <= 12)
            {
                int TotalWorkingDay = (PkjStatus.fld_Trtakf - PkjStatus.fld_Trmlkj).Value.Days;
                decimal Years = TotalWorkingDay / 365.25m;
                DateTime? MulaKerja = new DateTime();
                if (Years > 0)
                {
                    MulaKerja = new DateTime(YearC, 1, 1);
                }
                else
                {
                    MulaKerja = PkjStatus.fld_Trmlkj;
                }

                TotalWorkingDay = (PkjStatus.fld_Trtakf - MulaKerja).Value.Days;

                var GetCutiPkj = tbl_CutiPeruntukan.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_NoPkj == NoPkj && x.fld_KodCuti == KodCutiTahunan.fld_KodCuti).FirstOrDefault();
                int? Peruntukkan = GetCutiPkj.fld_JumlahCuti;
                int? DahAmbil = GetCutiPkj.fld_JumlahCutiDiambil;
                int? MonthWorking = TotalWorkingDay / 30;
                int? PeruntukkanSbulan = Peruntukkan / 12;

                if (MonthWorking <= 1 && PeruntukkanSbulan == 0)
                {
                    CheckPeruntukkan = 0;
                }
                else if (MonthWorking > 1 && PeruntukkanSbulan == 0)
                {
                    CheckPeruntukkan = MonthWorking * (Peruntukkan / 12);
                }
                else if (MonthWorking > 1 && PeruntukkanSbulan > 0)
                {
                    CheckPeruntukkan = MonthWorking * PeruntukkanSbulan;
                }
                var TakeLeaves = db2.tbl_Kerjahdr.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Nopkj == NoPkj && x.fld_Tarikh.Value.Year == Year && x.fld_Kdhdct == KodCutiTahunan.fld_KodCuti).ToList();
                var bakiCuti = CheckPeruntukkan - TakeLeaves.Count;
                if (bakiCuti > 0)
                {
                    TotalPaidLeave3 = decimal.Round(LeavePayment.Value, 2) * bakiCuti;
                    KerjahdrCutiTahunan.fld_Kadar = LeavePayment;
                    KerjahdrCutiTahunan.fld_KodCuti = KodCutiTahunan.fld_KodCuti;
                    KerjahdrCutiTahunan.fld_Kum = WorkerPaidLeaveLists.Select(s => s.fld_Kum).FirstOrDefault();
                    KerjahdrCutiTahunan.fld_JumlahCuti = CheckPeruntukkan;
                    KerjahdrCutiTahunan.fld_JumlahAmt = TotalPaidLeave3;
                    KerjahdrCutiTahunan.fld_NegaraID = NegaraID;
                    KerjahdrCutiTahunan.fld_SyarikatID = SyarikatID;
                    KerjahdrCutiTahunan.fld_WilayahID = WilayahID;
                    KerjahdrCutiTahunan.fld_LadangID = LadangID;
                    KerjahdrCutiTahunan.fld_Nopkj = NoPkj;
                    KerjahdrCutiTahunan.fld_Month = Month;
                    KerjahdrCutiTahunan.fld_Year = Year;
                    KerjahdrCutiTahunan.fld_CreatedBy = UserID;
                    KerjahdrCutiTahunan.fld_CreatedDT = DTProcess;
                    KerjahdrCutiTahunan.fld_StatusAmbil = false;

                    db2.tbl_KerjahdrCutiTahunan.Add(KerjahdrCutiTahunan);
                    db2.SaveChanges();
                }
            }

            // cuti tahunan sahaja
            else if (PkjStatus.fld_Kdaktf == "1" && Month == 12) 
            {
                var TakeLeaves = tbl_Kerjahdr12Month.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Nopkj == NoPkj && x.fld_Tarikh.Value.Year == Year && x.fld_Kdhdct == KodCutiTahunan.fld_KodCuti).ToList();
                var PeruntukkanCtTahunan = tbl_CutiPeruntukan.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_NoPkj == NoPkj && x.fld_Tahun == Year && x.fld_KodCuti == KodCutiTahunan.fld_KodCuti).Select(s => s.fld_JumlahCuti).FirstOrDefault();

                var bakiCuti = PeruntukkanCtTahunan - TakeLeaves.Count;
                if (bakiCuti > 0)
                {
                    TotalPaidLeave3 = decimal.Round(LeavePayment.Value, 2) * bakiCuti;
                    KerjahdrCutiTahunan.fld_Kadar = LeavePayment;
                    KerjahdrCutiTahunan.fld_KodCuti = "C99";
                    KerjahdrCutiTahunan.fld_Kum = WorkerPaidLeaveLists.Select(s => s.fld_Kum).FirstOrDefault();
                    KerjahdrCutiTahunan.fld_JumlahCuti = bakiCuti;
                    KerjahdrCutiTahunan.fld_JumlahAmt = TotalPaidLeave3;
                    KerjahdrCutiTahunan.fld_NegaraID = NegaraID;
                    KerjahdrCutiTahunan.fld_SyarikatID = SyarikatID;
                    KerjahdrCutiTahunan.fld_WilayahID = WilayahID;
                    KerjahdrCutiTahunan.fld_LadangID = LadangID;
                    KerjahdrCutiTahunan.fld_Nopkj = NoPkj;
                    KerjahdrCutiTahunan.fld_Month = Month;
                    KerjahdrCutiTahunan.fld_Year = Year;
                    KerjahdrCutiTahunan.fld_CreatedBy = UserID;
                    KerjahdrCutiTahunan.fld_CreatedDT = DTProcess;
                    KerjahdrCutiTahunan.fld_StatusAmbil = false;

                    db2.tbl_KerjahdrCutiTahunan.Add(KerjahdrCutiTahunan);
                    db2.SaveChanges();
                }
            }
            var jsonSerialiser = new JavaScriptSerializer();
            var json = jsonSerialiser.Serialize(KerjahdrCutiList);

            GajiBulanan = db2.tbl_GajiBulanan.Find(Guid);
            AddTo_tbl_GajiBulanan(db2, NegaraID, SyarikatID, WilayahID, LadangID, Month, Year, NoPkj, 18, LeavePayment, DTProcess, UserID, GajiBulanan);
            Step2Func.AddTo_tbl_KerjahdrCuti(NegaraID, SyarikatID, WilayahID, LadangID, KerjahdrCutiList);

            if (NoLeave && TotalPaidLeave == 0 && TotalPaidLeave2 == 0 && TotalPaidLeave3 == 0)
            {
                TotalPaidLeave = 0;
                TotalPaidLeave2 = 0;
                TotalPaidLeave3 = 0;
            }

            OverAllPaidLeave = TotalPaidLeave + TotalPaidLeave3;

            GajiBulanan = db2.tbl_GajiBulanan.Find(Guid);
            AddTo_tbl_GajiBulanan(db2, NegaraID, SyarikatID, WilayahID, LadangID, Month, Year, NoPkj, 21, TotalPaidLeave3, DTProcess, UserID, GajiBulanan);

            db2.Dispose();

            return OverAllPaidLeave;
        }

        public void GetKWSPFunc(int? NegaraID, int? SyarikatID, int? WilayahID, int? LadangID, int? UserID, DateTime DTProcess, int? Month, int? Year, string processname, string servicesname, int? ClientID, string NoPkj, Guid Guid, string KodCaruman, out decimal? KWSPMjk, out decimal? KWSPPkj, bool NoKWSP, List<tbl_JenisInsentif> tbl_JenisInsentif, List<tbl_Insentif> tbl_Insentif, List<tbl_Kwsp> tbl_Kwsp)
        {
            GenSalaryModelHQ db = new GenSalaryModelHQ();
            GetConnectFunc conn = new GetConnectFunc();
            tbl_GajiBulanan GajiBulanan = new tbl_GajiBulanan();
            string host, catalog, user, pass = "";
            conn.GetConnection(out host, out catalog, out user, out pass, WilayahID, SyarikatID, NegaraID);
            GenSalaryModelEstate db2 = GenSalaryModelEstate.ConnectToSqlServer(host, catalog, user, pass);
            decimal? TotalSalaryForKWSP = 0;
            decimal? TotalInsentifEfected = 0;

            GajiBulanan = db2.tbl_GajiBulanan.Find(Guid);

            if (NoKWSP)
            {
                KWSPMjk = 0;
                KWSPPkj = 0;
            }
            else
            {
                var GetInsetifEffectCode = tbl_JenisInsentif.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_JenisInsentif == "P" && x.fld_AdaCaruman == true && x.fld_Deleted == false).Select(s => s.fld_KodInsentif).ToArray();
                TotalInsentifEfected = tbl_Insentif.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Nopkj == NoPkj && GetInsetifEffectCode.Contains(x.fld_KodInsentif) && x.fld_Deleted == false && x.fld_Month == Month && x.fld_Year == Year).Sum(s => s.fld_NilaiInsentif);
                TotalInsentifEfected = TotalInsentifEfected == null ? 0 : TotalInsentifEfected;
                TotalSalaryForKWSP = GajiBulanan.fld_ByrKerja + GajiBulanan.fld_ByrCuti + GajiBulanan.fld_BonusHarian + TotalInsentifEfected + GajiBulanan.fld_AIPS + GajiBulanan.fld_BakiCutiTahunan;// + GajiBulanan.fld_ByrKwsnSkr;

                var GetCarumanKWSP = tbl_Kwsp.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_KodCaruman == KodCaruman && TotalSalaryForKWSP >= x.fld_KdrLower && TotalSalaryForKWSP <= x.fld_KdrUpper).FirstOrDefault();
                //KWSPMjk = GetCarumanKWSP.fld_Mjkn;
                //KWSPPkj = GetCarumanKWSP.fld_Pkj;
                //modified by faeza 2/11/2020
                if (GetCarumanKWSP != null)
                {
                    KWSPMjk = GetCarumanKWSP.fld_Mjkn;
                    KWSPPkj = GetCarumanKWSP.fld_Pkj;
                }
                else
                {
                    KWSPMjk = 0;
                    KWSPPkj = 0;
                }
            }

            AddTo_tbl_GajiBulanan(db2, NegaraID, SyarikatID, WilayahID, LadangID, Month, Year, NoPkj, 9, KWSPMjk, DTProcess, UserID, GajiBulanan);
            AddTo_tbl_GajiBulanan(db2, NegaraID, SyarikatID, WilayahID, LadangID, Month, Year, NoPkj, 10, KWSPPkj, DTProcess, UserID, GajiBulanan);

            db2.Dispose();
            db.Dispose();
        }

        public void GetSocsoFunc(int? NegaraID, int? SyarikatID, int? WilayahID, int? LadangID, int? UserID, DateTime DTProcess, int? Month, int? Year, string processname, string servicesname, int? ClientID, string NoPkj, Guid Guid, string KodCaruman, out decimal? SocsoMjk, out decimal? SocsoPkj, bool NoSocso, List<tbl_JenisInsentif> tbl_JenisInsentif, List<tbl_Insentif> tbl_Insentif, List<tbl_Socso> tbl_Socso)
        {
            GenSalaryModelHQ db = new GenSalaryModelHQ();
            GetConnectFunc conn = new GetConnectFunc();
            tbl_GajiBulanan GajiBulanan = new tbl_GajiBulanan();
            string host, catalog, user, pass = "";
            conn.GetConnection(out host, out catalog, out user, out pass, WilayahID, SyarikatID, NegaraID);
            GenSalaryModelEstate db2 = GenSalaryModelEstate.ConnectToSqlServer(host, catalog, user, pass);
            decimal? TotalSalaryForSocso = 0;
            decimal? TotalInsentifEfected = 0;

            GajiBulanan = db2.tbl_GajiBulanan.Find(Guid);

            if (NoSocso)
            {
                SocsoMjk = 0;
                SocsoPkj = 0;
            }
            else
            {
                var GetInsetifEffectCode = tbl_JenisInsentif.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_JenisInsentif == "P" && x.fld_AdaCaruman == true && x.fld_Deleted == false).Select(s => s.fld_KodInsentif).ToArray();
                TotalInsentifEfected = tbl_Insentif.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Nopkj == NoPkj && GetInsetifEffectCode.Contains(x.fld_KodInsentif) && x.fld_Deleted == false && x.fld_Month == Month && x.fld_Year == Year).Sum(s => s.fld_NilaiInsentif);
                TotalInsentifEfected = TotalInsentifEfected == null ? 0 : TotalInsentifEfected;
                TotalSalaryForSocso = GajiBulanan.fld_ByrKerja + GajiBulanan.fld_ByrCuti + GajiBulanan.fld_OT + TotalInsentifEfected + GajiBulanan.fld_AIPS + GajiBulanan.fld_BonusHarian + GajiBulanan.fld_BakiCutiTahunan;// + GajiBulanan.fld_ByrKwsnSkr;

                var GetCarumanSocso = tbl_Socso.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_KodCaruman == KodCaruman && TotalSalaryForSocso >= x.fld_KdrLower && TotalSalaryForSocso <= x.fld_KdrUpper).FirstOrDefault();
                //SocsoMjk = GetCarumanSocso.fld_SocsoMjkn;
                //SocsoPkj = GetCarumanSocso.fld_SocsoPkj;
                //modified by faeza 2/11/2020
                if (GetCarumanSocso != null)
                {
                    SocsoMjk = GetCarumanSocso.fld_SocsoMjkn;
                    SocsoPkj = GetCarumanSocso.fld_SocsoPkj;
                }
                else
                {
                    SocsoMjk = 0;
                    SocsoPkj = 0;
                }
            }

            AddTo_tbl_GajiBulanan(db2, NegaraID, SyarikatID, WilayahID, LadangID, Month, Year, NoPkj, 11, SocsoMjk, DTProcess, UserID, GajiBulanan);
            AddTo_tbl_GajiBulanan(db2, NegaraID, SyarikatID, WilayahID, LadangID, Month, Year, NoPkj, 12, SocsoPkj, DTProcess, UserID, GajiBulanan);

            db2.Dispose();
            db.Dispose();
        }

        public void GetOtherContributionsFunc(int? NegaraID, int? SyarikatID, int? WilayahID, int? LadangID, int? UserID, DateTime DTProcess, int? Month, int? Year, string processname, string servicesname, int? ClientID, string NoPkj, Guid Guid, out decimal? TotalMjkCont, out decimal? TotalPkjCont, List<tbl_JenisInsentif> tbl_JenisInsentif, List<tbl_Insentif> tbl_Insentif, List<tbl_CarumanTambahan> tbl_CarumanTambahan, List<tbl_SubCarumanTambahan> tbl_SubCarumanTambahan, List<tbl_JadualCarumanTambahan> tbl_JadualCarumanTambahan)
        {
            GenSalaryModelHQ db = new GenSalaryModelHQ();
            GetConnectFunc conn = new GetConnectFunc();
            tbl_GajiBulanan GajiBulanan = new tbl_GajiBulanan();
            List<tbl_ByrCarumanTambahan> ByrCarumanTambahanList = new List<tbl_ByrCarumanTambahan>();
            string host, catalog, user, pass = "";
            conn.GetConnection(out host, out catalog, out user, out pass, WilayahID, SyarikatID, NegaraID);
            GenSalaryModelEstate db2 = GenSalaryModelEstate.ConnectToSqlServer(host, catalog, user, pass);
            tbl_JadualCarumanTambahan GetContributionAmnt = new tbl_JadualCarumanTambahan();

            TotalMjkCont = 0;
            TotalPkjCont = 0;

            decimal? TotalSalaryForOtherContribution = 0;
            decimal? TotalInsentifEfected = 0;

            GajiBulanan = db2.tbl_GajiBulanan.Find(Guid);

            var GetOtherContributions = db2.tbl_PkjCarumanTambahan.Where(x => x.fld_Nopkj == NoPkj && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Deleted == false).ToList();

            var GetInsetifEffectCode = tbl_JenisInsentif.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_JenisInsentif == "P" && x.fld_AdaCaruman == true && x.fld_Deleted == false).Select(s => s.fld_KodInsentif).ToArray();
            TotalInsentifEfected = tbl_Insentif.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Nopkj == NoPkj && GetInsetifEffectCode.Contains(x.fld_KodInsentif) && x.fld_Deleted == false && x.fld_Month == Month && x.fld_Year == Year).Sum(s => s.fld_NilaiInsentif);
            TotalInsentifEfected = TotalInsentifEfected == null ? 0 : TotalInsentifEfected;
            TotalSalaryForOtherContribution = GajiBulanan.fld_ByrKerja + GajiBulanan.fld_ByrCuti + GajiBulanan.fld_OT + TotalInsentifEfected + GajiBulanan.fld_AIPS + GajiBulanan.fld_BonusHarian + GajiBulanan.fld_BakiCutiTahunan;
            decimal? ContriMjk = 0;
            decimal? ContriPkj = 0;
            foreach (var GetOtherContribution in GetOtherContributions)
            {
                var GetContributionDetail = tbl_CarumanTambahan.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_KodCaruman == GetOtherContribution.fld_KodCaruman).FirstOrDefault();
                var GetSubContributionDetail = tbl_SubCarumanTambahan.Where(x => x.fld_KodSubCaruman == GetOtherContribution.fld_KodSubCaruman && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID).FirstOrDefault();
                GetContributionAmnt = tbl_JadualCarumanTambahan.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_KodSubCaruman == GetOtherContribution.fld_KodSubCaruman && TotalSalaryForOtherContribution >= x.fld_GajiLower && TotalSalaryForOtherContribution <= x.fld_GajiUpper).FirstOrDefault();
                if (GetContributionDetail.fld_Berjadual == true)
                {
                    switch (GetContributionDetail.fld_CarumanOleh)
                    {
                        case 1:
                            ContriPkj = GetContributionAmnt.fld_CarumanPekerja;
                            break;
                        case 2:
                            ContriMjk = GetContributionAmnt.fld_CarumanMajikan;
                            break;
                        case 3:
                            ContriPkj = GetContributionAmnt.fld_CarumanPekerja;
                            ContriMjk = GetContributionAmnt.fld_CarumanMajikan;
                            break;
                    }
                }
                else
                {
                    switch (GetContributionDetail.fld_CarumanOleh)
                    {
                        case 1:
                            ContriPkj = TotalSalaryForOtherContribution * GetSubContributionDetail.fld_KadarPekerja;
                            break;
                        case 2:
                            ContriMjk = TotalSalaryForOtherContribution * GetSubContributionDetail.fld_KadarMajikan;
                            break;
                        case 3:
                            ContriPkj = TotalSalaryForOtherContribution * GetSubContributionDetail.fld_KadarPekerja;
                            ContriMjk = TotalSalaryForOtherContribution * GetSubContributionDetail.fld_KadarMajikan;
                            break;
                    }
                }
                ByrCarumanTambahanList.Add(new tbl_ByrCarumanTambahan() { fld_GajiID = Guid, fld_KodCaruman = GetOtherContribution.fld_KodCaruman, fld_KodSubCaruman = GetOtherContribution.fld_KodSubCaruman, fld_CarumanPekerja = ContriPkj, fld_CarumanMajikan = ContriMjk, fld_Month = Month, fld_Year = Year, fld_LadangID = LadangID, fld_WilayahID = WilayahID, fld_SyarikatID = SyarikatID, fld_NegaraID = NegaraID });
            }

            if (ByrCarumanTambahanList.Count > 0)
            {
                db2.tbl_ByrCarumanTambahan.AddRange(ByrCarumanTambahanList);
                db2.SaveChanges();

                TotalMjkCont = ByrCarumanTambahanList.Sum(s => s.fld_CarumanMajikan);
                TotalPkjCont = ByrCarumanTambahanList.Sum(s => s.fld_CarumanPekerja);
            }

            db2.Dispose();
            db.Dispose();
        }

        public void GetOverallSalaryFunc(int? NegaraID, int? SyarikatID, int? WilayahID, int? LadangID, int? UserID, DateTime DTProcess, int? Month, int? Year, string processname, string servicesname, int? ClientID, string NoPkj, Guid Guid, out decimal? OverallSalary, out decimal? Salary)
        {
            GetConnectFunc conn = new GetConnectFunc();
            tbl_GajiBulanan GajiBulanan = new tbl_GajiBulanan();
            string host, catalog, user, pass = "";
            conn.GetConnection(out host, out catalog, out user, out pass, WilayahID, SyarikatID, NegaraID);
            GenSalaryModelEstate db2 = GenSalaryModelEstate.ConnectToSqlServer(host, catalog, user, pass);

            decimal? OtherContr = 0;
            GajiBulanan = db2.tbl_GajiBulanan.Find(Guid);
            OverallSalary = GajiBulanan.fld_ByrKerja + GajiBulanan.fld_BonusHarian + GajiBulanan.fld_ByrCuti + GajiBulanan.fld_LainInsentif + GajiBulanan.fld_AIPS + GajiBulanan.fld_OT + GajiBulanan.fld_BakiCutiTahunan; //+ GajiBulanan.fld_ByrKwsnSkr;// + GajiBulanan.fld_KWSPMjk + GajiBulanan.fld_SocsoMjk;
            OtherContr = db2.tbl_ByrCarumanTambahan.Where(x => x.fld_GajiID == Guid).Sum(s => s.fld_CarumanPekerja);
            if (OtherContr == null)
            {
                OtherContr = 0;
            }
            Salary = (OverallSalary) - (GajiBulanan.fld_KWSPPkj + GajiBulanan.fld_SocsoPkj + GajiBulanan.fld_LainPotongan + OtherContr);
            AddTo_tbl_GajiBulanan(db2, NegaraID, SyarikatID, WilayahID, LadangID, Month, Year, NoPkj, 13, OverallSalary, DTProcess, UserID, GajiBulanan);
            AddTo_tbl_GajiBulanan(db2, NegaraID, SyarikatID, WilayahID, LadangID, Month, Year, NoPkj, 14, Salary, DTProcess, UserID, GajiBulanan);

            db2.Dispose();
        }

        //added by faeza 12.11.2021
        public void UpdatePaymentMode(int? NegaraID, int? SyarikatID, int? WilayahID, int? LadangID, int? UserID, DateTime DTProcess, int? Month, int? Year, string processname, string servicesname, int? ClientID, string NoPkj, string PaymentMode, Guid Guid)
        {
            GetConnectFunc conn = new GetConnectFunc();
            tbl_GajiBulanan GajiBulanan = new tbl_GajiBulanan();
            string host, catalog, user, pass = "";
            conn.GetConnection(out host, out catalog, out user, out pass, WilayahID, SyarikatID, NegaraID);
            GenSalaryModelEstate db2 = GenSalaryModelEstate.ConnectToSqlServer(host, catalog, user, pass);

            GajiBulanan = db2.tbl_GajiBulanan.Find(Guid);
            AddTo_tbl_GajiBulanan3(db2, NegaraID, SyarikatID, WilayahID, LadangID, Month, Year, NoPkj, 1, PaymentMode, DTProcess, UserID, GajiBulanan);

            db2.Dispose();
        }

        public decimal? GetORPFunc(int? NegaraID, int? SyarikatID, int? WilayahID, int? LadangID, int? UserID, DateTime DTProcess, int? Month, int? Year, string processname, string servicesname, int? ClientID, string NoPkj, Guid Guid, List<tbl_Insentif> WorkerIncentifs, List<tbl_JenisInsentif> IncentifsType, List<vw_KerjaInfoDetails> vw_KerjaInfoDetails, List<vw_Kerja_Bonus> vw_Kerja_Bonus, List<tbl_CutiKategori> tbl_CutiKategori, List<tbl_Kerjahdr> tbl_Kerjahdr, List<CustMod_WorkerPaidLeave> CustMod_WorkerPaidLeave)
        {
            GetConnectFunc conn = new GetConnectFunc();
            Guid MonthSalaryID = new Guid();
            tbl_GajiBulanan GajiBulanan = new tbl_GajiBulanan();
            string host, catalog, user, pass = "";
            conn.GetConnection(out host, out catalog, out user, out pass, WilayahID, SyarikatID, NegaraID);
            GenSalaryModelEstate db2 = GenSalaryModelEstate.ConnectToSqlServer(host, catalog, user, pass);

            decimal? WorkingPayment = 0m;
            GajiBulanan = db2.tbl_GajiBulanan.Find(Guid);
            var codeCuti = tbl_CutiKategori.Where(x=> x.fld_KodCuti != "C01").Select(s => s.fld_KodCuti).ToList();
            var kerjaList = vw_KerjaInfoDetails.Where(x => x.fld_Nopkj == NoPkj && x.fld_Kdhdct == "H01").ToList();
            var cutiBerbayarCount = tbl_Kerjahdr.Where(x => x.fld_Nopkj == NoPkj && codeCuti.Contains(x.fld_Kdhdct)).Count();
            var oRPIncentifsCode = IncentifsType.Where(x => x.fld_AdaORP == true).Select(s => s.fld_KodInsentif).ToList();
            if (kerjaList.Count() > 0)
            {
                var normalDateAtt = kerjaList.Select(s => s.fld_Tarikh).Distinct().ToList();
                var byrKerja = kerjaList.Sum(s => s.fld_OverallAmount);
                var byrBonus = vw_Kerja_Bonus.Where(x => x.fld_Nopkj == NoPkj && normalDateAtt.Contains(x.fld_Tarikh)).Sum(s => s.fld_Jumlah_B);
                WorkingPayment = byrKerja + GajiBulanan.fld_ByrCuti + byrBonus;
                if (WorkerIncentifs.Count() > 0)
                {
                    var oRPWorkerIncentifs = WorkerIncentifs.Where(x => oRPIncentifsCode.Contains(x.fld_KodInsentif)).ToList();
                    if (oRPWorkerIncentifs.Count() > 0)
                    {
                        WorkingPayment = WorkingPayment + oRPWorkerIncentifs.Sum(s => s.fld_NilaiInsentif);
                    }
                }
                var workingNormalDay = normalDateAtt.Count() + cutiBerbayarCount;
                WorkingPayment = WorkingPayment / workingNormalDay;
                WorkingPayment = Math.Round(WorkingPayment.Value, 2);

                AddTo_tbl_GajiBulanan(db2, NegaraID, SyarikatID, WilayahID, LadangID, Month, Year, NoPkj, 6, WorkingPayment, DTProcess, UserID, GajiBulanan);
                MonthSalaryID = AddTo_tbl_GajiBulanan(db2, NegaraID, SyarikatID, WilayahID, LadangID, Month, Year, NoPkj, 20, WorkingPayment, DTProcess, UserID, GajiBulanan);
            }
            else if (kerjaList.Count() == 0 && CustMod_WorkerPaidLeave.Count() > 0)
            {
                WorkingPayment = GajiBulanan.fld_ByrCuti;
                if (WorkerIncentifs.Count() > 0)
                {
                    var oRPWorkerIncentifs = WorkerIncentifs.Where(x => oRPIncentifsCode.Contains(x.fld_KodInsentif)).ToList();
                    if (oRPWorkerIncentifs.Count() > 0)
                    {
                        WorkingPayment = WorkingPayment + oRPWorkerIncentifs.Sum(s => s.fld_NilaiInsentif);
                    }
                }
                var workingNormalDay = CustMod_WorkerPaidLeave.Count();
                WorkingPayment = WorkingPayment / workingNormalDay;
                WorkingPayment = Math.Round(WorkingPayment.Value, 2);

                AddTo_tbl_GajiBulanan(db2, NegaraID, SyarikatID, WilayahID, LadangID, Month, Year, NoPkj, 6, WorkingPayment, DTProcess, UserID, GajiBulanan);
                MonthSalaryID = AddTo_tbl_GajiBulanan(db2, NegaraID, SyarikatID, WilayahID, LadangID, Month, Year, NoPkj, 20, WorkingPayment, DTProcess, UserID, GajiBulanan);
            }

            db2.Dispose();

            return WorkingPayment;
        }

        public Guid AddTo_tbl_GajiBulanan(GenSalaryModelEstate db, int? NegaraID, int? SyarikatID, int? WilayahID, int? LadangID, int? Month, int? Year, string NoPkj, byte? UpdateFlag, decimal? PaymentAmount, DateTime DTProcess, int? UserID, tbl_GajiBulanan GajiBulanan)
        {
            // UpdateFlag = 1 - Add Gaji bulanan utk Byran Kerja, 
            //tbl_GajiBulanan GajiBulanan = new tbl_GajiBulanan();
            Guid MonthSalaryID = new Guid();

            //GajiBulanan = UpdateFlag >= 2 ? db.tbl_GajiBulanan.Find(Guid) : GajiBulanan;

            switch (UpdateFlag)
            {
                case 1: // untuk add new gaji kerja harian
                    GajiBulanan.fld_NegaraID = NegaraID;
                    GajiBulanan.fld_SyarikatID = SyarikatID;
                    GajiBulanan.fld_WilayahID = WilayahID;
                    GajiBulanan.fld_LadangID = LadangID;
                    GajiBulanan.fld_Nopkj = NoPkj;
                    GajiBulanan.fld_DTCreated = DTProcess;
                    GajiBulanan.fld_CreatedBy = UserID;
                    GajiBulanan.fld_ByrKerja = PaymentAmount;
                    GajiBulanan.fld_Month = Month;
                    GajiBulanan.fld_Year = Year;
                    db.tbl_GajiBulanan.Add(GajiBulanan);
                    db.SaveChanges();
                    MonthSalaryID = GajiBulanan.fld_ID;
                    break;
                case 2: // untuk add bonus harian
                    GajiBulanan.fld_BonusHarian = PaymentAmount;
                    db.Entry(GajiBulanan).State = EntityState.Modified;
                    db.SaveChanges();
                    break;
                case 3: // untuk add ot harian
                    GajiBulanan.fld_OT = PaymentAmount;
                    db.Entry(GajiBulanan).State = EntityState.Modified;
                    db.SaveChanges();
                    break;
                case 4: // untuk add Lain - lain insentif
                    GajiBulanan.fld_LainInsentif = PaymentAmount;
                    db.Entry(GajiBulanan).State = EntityState.Modified;
                    db.SaveChanges();
                    break;
                case 5: // untuk add Potongan insentif
                    GajiBulanan.fld_LainPotongan = PaymentAmount;
                    db.Entry(GajiBulanan).State = EntityState.Modified;
                    db.SaveChanges();
                    break;
                case 6: // untuk add Purata gaji
                    GajiBulanan.fld_PurataGaji = PaymentAmount;
                    db.Entry(GajiBulanan).State = EntityState.Modified;
                    db.SaveChanges();
                    break;
                case 7: // untuk add AIPS insentif
                    GajiBulanan.fld_AIPS = PaymentAmount;
                    db.Entry(GajiBulanan).State = EntityState.Modified;
                    db.SaveChanges();
                    break;
                case 8: // untuk add Leave Payment
                    GajiBulanan.fld_ByrCuti = PaymentAmount;
                    db.Entry(GajiBulanan).State = EntityState.Modified;
                    db.SaveChanges();
                    break;
                case 9: // untuk add KWSP Mjkn
                    GajiBulanan.fld_KWSPMjk = PaymentAmount;
                    db.Entry(GajiBulanan).State = EntityState.Modified;
                    db.SaveChanges();
                    break;
                case 10: // untuk add KWSP Pkj
                    GajiBulanan.fld_KWSPPkj = PaymentAmount;
                    db.Entry(GajiBulanan).State = EntityState.Modified;
                    db.SaveChanges();
                    break;
                case 11: // untuk add Socso Mjkn
                    GajiBulanan.fld_SocsoMjk = PaymentAmount;
                    db.Entry(GajiBulanan).State = EntityState.Modified;
                    db.SaveChanges();
                    break;
                case 12: // untuk add Socso Pkj
                    GajiBulanan.fld_SocsoPkj = PaymentAmount;
                    db.Entry(GajiBulanan).State = EntityState.Modified;
                    db.SaveChanges();
                    break;
                case 13: // untuk add Gaji Kasar
                    GajiBulanan.fld_GajiKasar = PaymentAmount;
                    db.Entry(GajiBulanan).State = EntityState.Modified;
                    db.SaveChanges();
                    break;
                case 14: // untuk add Gaji Bersih
                    GajiBulanan.fld_GajiBersih = PaymentAmount;
                    db.Entry(GajiBulanan).State = EntityState.Modified;
                    db.SaveChanges();
                    break;
                case 15: // untuk add Kehadiran Insentif
                    GajiBulanan.fld_HdrInsentif = PaymentAmount;
                    db.Entry(GajiBulanan).State = EntityState.Modified;
                    db.SaveChanges();
                    break;
                case 16: // untuk add Kualiti Insentif
                    GajiBulanan.fld_KuaInsentif = PaymentAmount;
                    db.Entry(GajiBulanan).State = EntityState.Modified;
                    db.SaveChanges();
                    break;
                case 17: // untuk add Produktiviti Insentif
                    GajiBulanan.fld_ProdInsentif = PaymentAmount;
                    db.Entry(GajiBulanan).State = EntityState.Modified;
                    db.SaveChanges();
                    break;

                case 18: // untuk add Purata 12 bulan Gaji
                    GajiBulanan.fld_PurataGaji12Bln = PaymentAmount;
                    db.Entry(GajiBulanan).State = EntityState.Modified;
                    db.SaveChanges();
                    break;

                case 19: // untuk add Bayar Kawasan Sukar
                    GajiBulanan.fld_ByrKwsnSkr = PaymentAmount;
                    db.Entry(GajiBulanan).State = EntityState.Modified;
                    db.SaveChanges();
                    break;

                case 20: // ORP
                    GajiBulanan.fld_ORP = PaymentAmount;
                    db.Entry(GajiBulanan).State = EntityState.Modified;
                    db.SaveChanges();
                    break;
                case 21: // Baki Cuti
                    GajiBulanan.fld_BakiCutiTahunan = PaymentAmount;
                    db.Entry(GajiBulanan).State = EntityState.Modified;
                    db.SaveChanges();
                    break;
            }

            db.Entry(GajiBulanan).State = EntityState.Detached;
            return MonthSalaryID;
        }

        public Guid AddTo_tbl_GajiBulanan2(GenSalaryModelEstate db, byte? UpdateFlag, int? AttTarget, int? AttCapai, short? QuaTarget, short? QuaCapai, decimal? ProdTarget, decimal? ProdCapai, tbl_GajiBulanan GajiBulanan)
        {
            Guid MonthSalaryID = new Guid();
            switch (UpdateFlag)
            {
                case 1: // untuk add target capai AIPS
                    GajiBulanan.fld_HdrTarget = AttTarget;
                    GajiBulanan.fld_HdrCapai = AttCapai;
                    GajiBulanan.fld_KuaTarget = QuaTarget;
                    GajiBulanan.fld_KuaCapai = QuaCapai;
                    GajiBulanan.fld_TargetProd = ProdTarget;
                    GajiBulanan.fld_CapaiProd = ProdCapai;
                    db.Entry(GajiBulanan).State = EntityState.Modified;
                    db.SaveChanges();
                    break;
            }

            return MonthSalaryID;
        }

        //added by faeza 12.11.2021
        //update payment mode into table gaji bulanan
        public Guid AddTo_tbl_GajiBulanan3(GenSalaryModelEstate db, int? NegaraID, int? SyarikatID, int? WilayahID, int? LadangID, int? Month, int? Year, string NoPkj, byte? UpdateFlag, string PaymentMode, DateTime DTProcess, int? UserID, tbl_GajiBulanan GajiBulanan)
        {
            Guid MonthSalaryID = new Guid();
            switch (UpdateFlag)
            {
                case 1: // update payment mode
                    GajiBulanan.fld_PaymentMode = PaymentMode;
                    db.Entry(GajiBulanan).State = EntityState.Modified;
                    db.SaveChanges();
                    break;
            }
            return MonthSalaryID;
        }

        public CustMod_KWSP GetKWSPForBonusFunc(int? NegaraID, int? SyarikatID, int? WilayahID, int? LadangID, int? UserID, DateTime DTProcess, int? Month, int? Year, string processname, string servicesname, int? ClientID, string NoPkj, string KodCaruman, bool NoKWSP, List<tbl_Kwsp> tbl_Kwsp, tbl_SpecialInsentif tbl_SpecialInsentif)
        {
            GetConnectFunc conn = new GetConnectFunc();
            string host, catalog, user, pass = "";
            conn.GetConnection(out host, out catalog, out user, out pass, WilayahID, SyarikatID, NegaraID);
            GenSalaryModelEstate db2 = GenSalaryModelEstate.ConnectToSqlServer(host, catalog, user, pass);
            decimal? TotalSalaryForKWSP = 0;
            decimal? KWSPMjk = 0;
            decimal? KWSPPkj = 0;
            if (NoKWSP)
            {
                KWSPMjk = 0;
                KWSPPkj = 0;
            }
            else
            {
                TotalSalaryForKWSP = tbl_SpecialInsentif.fld_NilaiInsentif;

                var GetCarumanKWSP = tbl_Kwsp.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_KodCaruman == KodCaruman && TotalSalaryForKWSP >= x.fld_KdrLower && TotalSalaryForKWSP <= x.fld_KdrUpper).FirstOrDefault();
                if (GetCarumanKWSP != null)
                {
                    KWSPMjk = GetCarumanKWSP.fld_Mjkn;
                    KWSPPkj = GetCarumanKWSP.fld_Pkj;
                }
                else
                {
                    KWSPMjk = 0;
                    KWSPPkj = 0;
                }
            }

            tbl_SpecialInsentif.fld_KWSPMjk = KWSPMjk;
            tbl_SpecialInsentif.fld_KWSPPkj = KWSPPkj;
            db2.Entry(tbl_SpecialInsentif).State = EntityState.Modified;
            db2.SaveChanges();

            var CustMod_KWSP = new CustMod_KWSP
            {
                KWSPMjk = KWSPMjk,
                KWSPPkj = KWSPPkj
            };
            db2.Dispose();
            return CustMod_KWSP;
        }

        public  CustMod_Socso GetSocsoForBonusFunc(int? NegaraID, int? SyarikatID, int? WilayahID, int? LadangID, int? UserID, DateTime DTProcess, int? Month, int? Year, string processname, string servicesname, int? ClientID, string NoPkj, string KodCaruman, bool NoSocso, List<tbl_Socso> tbl_Socso, tbl_SpecialInsentif tbl_SpecialInsentif)
        {
            GetConnectFunc conn = new GetConnectFunc();
            string host, catalog, user, pass = "";
            conn.GetConnection(out host, out catalog, out user, out pass, WilayahID, SyarikatID, NegaraID);
            GenSalaryModelEstate db2 = GenSalaryModelEstate.ConnectToSqlServer(host, catalog, user, pass);
            decimal? TotalSalaryForSocso = 0;
            decimal? SocsoMjk = 0;
            decimal? SocsoPkj = 0;

            if (NoSocso)
            {
                SocsoMjk = 0;
                SocsoPkj = 0;
            }
            else
            {
                TotalSalaryForSocso = tbl_SpecialInsentif.fld_NilaiInsentif;

                var GetCarumanSocso = tbl_Socso.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_KodCaruman == KodCaruman && TotalSalaryForSocso >= x.fld_KdrLower && TotalSalaryForSocso <= x.fld_KdrUpper).FirstOrDefault();
                if (GetCarumanSocso != null)
                {
                    SocsoMjk = GetCarumanSocso.fld_SocsoMjkn;
                    SocsoPkj = GetCarumanSocso.fld_SocsoPkj;
                }
                else
                {
                    SocsoMjk = 0;
                    SocsoPkj = 0;
                }
            }

            tbl_SpecialInsentif.fld_SocsoMjk = SocsoMjk;
            tbl_SpecialInsentif.fld_SocsoPkj = SocsoPkj;
            db2.Entry(tbl_SpecialInsentif).State = EntityState.Modified;
            db2.SaveChanges();

            var CustMod_Socso = new CustMod_Socso
            {
                SocsoMjk = SocsoMjk,
                SocsoPkj = SocsoPkj
            };
            db2.Dispose();
            return CustMod_Socso;
        }

        public void Update_SpecialInsentif(int? NegaraID, int? SyarikatID, int? WilayahID, int? LadangID, int? UserID, DateTime DTProcess, int? Month, int? Year, string processname, string servicesname, int? ClientID, string NoPkj, List<tbl_SpecialInsentif> tbl_SpecialInsentif, string kodInsentif, decimal kwsp, decimal  socso, tbl_SpecialInsentif workerSpecialInsentif)
        {
            GenSalaryModelHQ db = new GenSalaryModelHQ();
            GetConnectFunc conn = new GetConnectFunc();
            List<tbl_ByrCarumanTambahan> ByrCarumanTambahanList = new List<tbl_ByrCarumanTambahan>();
            string host, catalog, user, pass = "";
            conn.GetConnection(out host, out catalog, out user, out pass, WilayahID, SyarikatID, NegaraID);
            GenSalaryModelEstate db2 = GenSalaryModelEstate.ConnectToSqlServer(host, catalog, user, pass);

            workerSpecialInsentif.fld_GajiBersih = workerSpecialInsentif.fld_GajiKasar - kwsp - socso;
            db2.Entry(workerSpecialInsentif).State = EntityState.Modified;
            db2.SaveChanges();
        }
    }
}
