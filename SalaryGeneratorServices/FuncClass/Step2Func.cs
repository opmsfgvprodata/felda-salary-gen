using SalaryGeneratorServices.ModelsEstate;
using SalaryGeneratorServices.ModelsHQ;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SalaryGeneratorServices.FuncClass
{
    public class Step2Func
    {
        public bool GetWorkingPaidLeaveFunc(int? NegaraID, int? SyarikatID, int? WilayahID, int? LadangID, int? UserID, DateTime DTProcess, int? Month, int? Year, string processname, string servicesname, int? ClientID, string NoPkj, DateTime tarikh, List<tbl_CutiKategori> CutiKategoriList, out byte? PaidPeriod, out tbl_Kerjahdr WorkingAtt, out string KumCode, List<tbl_Kerjahdr> tbl_Kerjahdr)
        {
            GenSalaryModelHQ db = new GenSalaryModelHQ();
            GetConnectFunc conn = new GetConnectFunc();
            string host, catalog, user, pass = "";
            bool PaidLeave = false;

            var PaidLeaveCode = CutiKategoriList.Select(s => s.fld_KodCuti).ToList();
            var WorkingData = tbl_Kerjahdr.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Nopkj == NoPkj && x.fld_Tarikh == tarikh && PaidLeaveCode.Contains(x.fld_Kdhdct)).FirstOrDefault();
            WorkingAtt = WorkingData;

            if (WorkingData != null)
            {
                KumCode = WorkingData.fld_Kum;
                PaidLeave = true;
                PaidPeriod = CutiKategoriList.Where(x => x.fld_KodCuti == WorkingData.fld_Kdhdct).Select(s => s.fld_WaktuBayaranCuti).FirstOrDefault();
            }
            else
            {
                KumCode = "";
                PaidLeave = false;
                PaidPeriod = 0;
            }

            return PaidLeave;
        }

        public void GetDailyBonusFunc(int? NegaraID, int? SyarikatID, int? WilayahID, int? LadangID, int? UserID, DateTime DTProcess, int? Month, int? Year, string processname, string servicesname, int? ClientID, string NoPkj, DateTime tarikh, out tbl_KerjaBonus KerjaBonus, List<tbl_JenisAktiviti> JenisAktiviti, decimal bonusHarian, List<tbl_Kerja> tbl_Kerja, tbl_HargaSawitSemasa tbl_HargaSawitSemasa)
        {
            GenSalaryModelHQ db = new GenSalaryModelHQ();
            GetConnectFunc conn = new GetConnectFunc();

            tbl_KerjaBonus KerjaBonusData = new tbl_KerjaBonus();

            string host, catalog, user, pass = "";
            decimal? BonusRate = 0;
            decimal? BonusPay = 0;
            conn.GetConnection(out host, out catalog, out user, out pass, WilayahID, SyarikatID, NegaraID);
            GenSalaryModelEstate db2 = GenSalaryModelEstate.ConnectToSqlServer(host, catalog, user, pass);

            DateTime CurrentMonth = new DateTime(Year.Value, Month.Value, 15);

            //commented by faeza 13.10.2023
            //int LastMonth = CurrentMonth.AddMonths(-1).Month;
            //int LastYear = CurrentMonth.AddMonths(-1).Year;

            //added by faeza 13.10.2023
            int LastMonth = CurrentMonth.Month;
            int LastYear = CurrentMonth.Year;

            BonusRate = tbl_HargaSawitSemasa.fld_Insentif;

            var JenisAktvkod = JenisAktiviti.Select(s => s.fld_KodJnsAktvt).ToList();

            var KerjaData = tbl_Kerja.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Tarikh == tarikh && x.fld_Nopkj == NoPkj && JenisAktvkod.Contains(x.fld_JnisAktvt)).ToList();

            if (KerjaData.Count != 0)
            {
                var KerjaData2 = KerjaData.OrderByDescending(o => o.fld_Bonus).First();
                if (KerjaData2.fld_Bonus != 0)
                {
                    if (KerjaData2.fld_JnisAktvt == "01")
                    {
                        BonusPay = KerjaData2.fld_Bonus == 100 ? BonusRate : BonusRate / 2;
                    }
                    else
                    {
                        BonusPay = KerjaData2.fld_Bonus == 100 ? bonusHarian : bonusHarian / 2;
                        BonusRate = bonusHarian;
                    }

                    KerjaBonusData.fld_KerjaID = KerjaData2.fld_ID;
                    KerjaBonusData.fld_Bonus = KerjaData2.fld_Bonus;
                    KerjaBonusData.fld_Kadar = BonusRate;
                    KerjaBonusData.fld_Jumlah = BonusPay;
                    KerjaBonusData.fld_Kum = KerjaData2.fld_Kum;
                    KerjaBonusData.fld_LadangID = LadangID;
                    KerjaBonusData.fld_WilayahID = WilayahID;
                    KerjaBonusData.fld_SyarikatID = SyarikatID;
                    KerjaBonusData.fld_NegaraID = NegaraID;
                    KerjaBonusData.fld_Nopkj = NoPkj;
                    KerjaBonusData.fld_Tarikh = tarikh;
                    KerjaBonusData.fld_CreatedBy = UserID;
                    KerjaBonusData.fld_CreatedDT = DTProcess;
                    KerjaBonus = KerjaBonusData;
                }
                else
                {
                    KerjaBonus = null;
                }
            }
            else
            {
                KerjaBonus = null;
            }
            db.Dispose();
            db2.Dispose();
        }

        public void AddTo_tbl_KerjaBonus(int? NegaraID, int? SyarikatID, int? WilayahID, int? LadangID, List<tbl_KerjaBonus> KerjaBonus)
        {
            GetConnectFunc conn = new GetConnectFunc();
            string host, catalog, user, pass = "";
            conn.GetConnection(out host, out catalog, out user, out pass, WilayahID, SyarikatID, NegaraID);
            GenSalaryModelEstate db2 = GenSalaryModelEstate.ConnectToSqlServer(host, catalog, user, pass);

            db2.tbl_KerjaBonus.AddRange(KerjaBonus);
            db2.SaveChanges();
            db2.Dispose();
        }
        public void GetOTFunc(int? NegaraID, int? SyarikatID, int? WilayahID, int? LadangID, int? UserID, DateTime DTProcess, int? Month, int? Year, string processname, string servicesname, int? ClientID, string NoPkj, DateTime tarikh, out tbl_KerjaOT KerjaOT, string AttCode, List<tbl_JenisAktiviti> JenisAktiviti, List<tbl_Kerja> tbl_Kerja, List<tbl_GajiBulanan> tbl_GajiBulanan_Lepas, List<tblOptionConfigsWeb> tblOptionConfigsWeb, tbl_GajiMinimaLdg tbl_GajiMinimaLdg, List<tbl_PkjIncrmntSalary> tbl_PkjIncrmntSalary, List<tbl_UpahAktiviti> tbl_UpahAktiviti)
        {
            GenSalaryModelHQ db = new GenSalaryModelHQ();
            GetConnectFunc conn = new GetConnectFunc();

            tbl_KerjaOT KerjaOTData = new tbl_KerjaOT();

            string host, catalog, user, pass = "";
            //added by kamalia 19/3/2021
            decimal? firstNoAsal = 0;
            decimal? firstNo = 0;
            decimal? secondNo = 0;
            decimal? thirdNo = 0;
            decimal? OTRate = 0;
            decimal? OTPay = 0;
            decimal? AfterRounded = 0;

            conn.GetConnection(out host, out catalog, out user, out pass, WilayahID, SyarikatID, NegaraID);
            GenSalaryModelEstate db2 = GenSalaryModelEstate.ConnectToSqlServer(host, catalog, user, pass);

            var OTCulData = tblOptionConfigsWeb.Where(x => x.fldOptConfFlag1 == "kadarot" && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fldDeleted == false).ToList();
            var OTKadar = tblOptionConfigsWeb.Where(x => x.fldOptConfFlag1 == "kiraot" && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fldDeleted == false).ToList();

            //modified by kamalia 19/3/2021
            firstNoAsal = decimal.Parse(OTCulData.Where(x => x.fldOptConfFlag2 == "1").Select(s => s.fldOptConfValue).FirstOrDefault());

            // kamalia 18/3/2021
            secondNo = decimal.Parse(OTCulData.Where(x => x.fldOptConfFlag2 == "2").Select(s => s.fldOptConfValue).FirstOrDefault());
            thirdNo = decimal.Parse(OTKadar.Where(x => x.fldOptConfFlag2 == AttCode).Select(s => s.fldOptConfValue).FirstOrDefault());

            //modified by kamalia 26/8/2021
            DateTime lastMonthDT = new DateTime(Year.Value, Month.Value, 1).AddMonths(-1);
            var lastMonthSalary = tbl_GajiBulanan_Lepas.Where(x => x.fld_Year == lastMonthDT.Year && x.fld_Month == lastMonthDT.Month && x.fld_Nopkj == NoPkj).FirstOrDefault();

            var getgajiminima = tbl_GajiMinimaLdg;
            firstNo = lastMonthSalary != null ? lastMonthSalary.fld_PurataGaji : getgajiminima != null ? getgajiminima.fld_NilaiGajiMinima : firstNoAsal;

            firstNo = firstNo < getgajiminima.fld_NilaiGajiMinima ? getgajiminima.fld_NilaiGajiMinima : firstNo;

            var SalaryIncrement = tbl_PkjIncrmntSalary.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Nopkj == NoPkj && x.fld_AppStatus == true).Select(s => s.fld_IncrmntSalary).FirstOrDefault();
            var JenisAktvkod = JenisAktiviti.Select(s => s.fld_KodJnsAktvt).ToList();
            //modified by kamalia 3/5/2021
            var DataKerja = tbl_Kerja.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Tarikh == tarikh && x.fld_Nopkj == NoPkj).Select(s => s.fld_JnisAktvt).FirstOrDefault();
            if (DataKerja != null)
            {
                var tbl_JenisAktiviti = tbl_UpahAktiviti.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && DataKerja.Contains(x.fld_KodJenisAktvt)).Select(s => s.fld_Unit).FirstOrDefault();

                if (tbl_JenisAktiviti == "KONG")
                {
                    if (SalaryIncrement != null)
                    {
                        firstNo = firstNo + SalaryIncrement;
                    }
                    AfterRounded = firstNo / secondNo;
                }

                else
                {
                    if (SalaryIncrement != null)
                    {
                        firstNo = firstNo + SalaryIncrement;
                    }
                    AfterRounded = firstNo / secondNo;
                }
            }

            //end

            OTRate = Math.Round(decimal.Parse(AfterRounded.ToString()), 2) * thirdNo;
            var KerjaData = tbl_Kerja.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Tarikh == tarikh && x.fld_Nopkj == NoPkj).ToList();

            if (KerjaData.Count != 0)
            {
                var KerjaData2 = KerjaData.OrderByDescending(o => o.fld_JamOT).First();
                if (KerjaData2.fld_JamOT != 0)
                {
                    //if (AttCode == "H01" || (AttCode == "H02" && tbl_JenisAktiviti == "KONG") || (AttCode == "H03" && tbl_JenisAktiviti == "KONG"))
                    //{
                    //    OTPay = KerjaData2.fld_JamOT * Math.Round(decimal.Parse(OTRate.ToString()), 2);
                    //}
                    //else if ((AttCode == "H02" && tbl_JenisAktiviti != "KONG") || (AttCode == "H03" && tbl_JenisAktiviti != "KONG"))
                    //{
                    //    OTPay = KerjaData2.fld_JamOT * Math.Round(decimal.Parse(OTRate.ToString()), 2);
                    //}
                    OTPay = KerjaData2.fld_JamOT * Math.Round(decimal.Parse(OTRate.ToString()), 2);

                    KerjaOTData.fld_KerjaID = KerjaData2.fld_ID;
                    KerjaOTData.fld_JamOT = KerjaData2.fld_JamOT;
                    KerjaOTData.fld_Kadar = Math.Round(decimal.Parse(OTRate.ToString()), 2);
                    KerjaOTData.fld_Jumlah = Math.Round(decimal.Parse(OTPay.ToString()), 2);
                    KerjaOTData.fld_Kum = KerjaData2.fld_Kum;
                    KerjaOTData.fld_LadangID = LadangID;
                    KerjaOTData.fld_WilayahID = WilayahID;
                    KerjaOTData.fld_SyarikatID = SyarikatID;
                    KerjaOTData.fld_NegaraID = NegaraID;
                    KerjaOTData.fld_Nopkj = NoPkj;
                    KerjaOTData.fld_Tarikh = tarikh;
                    KerjaOTData.fld_CreatedBy = UserID;
                    KerjaOTData.fld_CreatedDT = DTProcess;
                    KerjaOT = KerjaOTData;
                }
                else
                {
                    KerjaOT = null;
                }
            }
            else
            {
                KerjaOT = null;
            }
            db.Dispose();
            db2.Dispose();
        }

        public void AddTo_tbl_KerjaOT(int? NegaraID, int? SyarikatID, int? WilayahID, int? LadangID, List<tbl_KerjaOT> KerjaOT)
        {
            GetConnectFunc conn = new GetConnectFunc();
            string host, catalog, user, pass = "";
            conn.GetConnection(out host, out catalog, out user, out pass, WilayahID, SyarikatID, NegaraID);
            GenSalaryModelEstate db2 = GenSalaryModelEstate.ConnectToSqlServer(host, catalog, user, pass);

            db2.tbl_KerjaOT.AddRange(KerjaOT);
            db2.SaveChanges();
            db2.Dispose();
        }

        public void AddTo_tbl_KerjahdrCuti(int? NegaraID, int? SyarikatID, int? WilayahID, int? LadangID, List<tbl_KerjahdrCuti> KerjahdrCuti)
        {
            GetConnectFunc conn = new GetConnectFunc();
            string host, catalog, user, pass = "";
            conn.GetConnection(out host, out catalog, out user, out pass, WilayahID, SyarikatID, NegaraID);
            GenSalaryModelEstate db2 = GenSalaryModelEstate.ConnectToSqlServer(host, catalog, user, pass);

            db2.tbl_KerjahdrCuti.AddRange(KerjahdrCuti);
            db2.SaveChanges();
            db2.Dispose();
        }

        public bool GetAttendStatusFunc(int? NegaraID, int? SyarikatID, int? WilayahID, int? LadangID, int? UserID, DateTime DTProcess, int? Month, int? Year, string processname, string servicesname, int? ClientID, string NoPkj, DateTime tarikh, out string AttCode, List<tblOptionConfigsWeb> tblOptionConfigsWeb, List<tbl_Kerjahdr> tbl_Kerjahdr)
        {
            GenSalaryModelHQ db = new GenSalaryModelHQ();
            GetConnectFunc conn = new GetConnectFunc();

            bool AttendStatus = false;

            var Attandance = tblOptionConfigsWeb.Where(x => x.fldOptConfFlag1 == "cuti" && x.fldOptConfFlag2 == "hadirkerja" && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fldDeleted == false).Select(s => s.fldOptConfValue).ToList();
            var checkattendance = tbl_Kerjahdr.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Nopkj == NoPkj && x.fld_Tarikh == tarikh).ToList();
            var checkattendancestatus = checkattendance.Where(x => Attandance.Contains(x.fld_Kdhdct)).FirstOrDefault();
            if (checkattendancestatus != null)
            {
                AttendStatus = true;
                AttCode = checkattendancestatus.fld_Kdhdct;
            }
            else
            {
                AttCode = checkattendance.Select(s => s.fld_Kdhdct).Take(1).FirstOrDefault();
            }


            return AttendStatus;
        }

        public List<vw_KerjaInfoDetails> vw_KerjaInfoDetails(int? NegaraID, int? SyarikatID, int? WilayahID, int? LadangID, int? Month, int? Year)
        {

            GetConnectFunc conn = new GetConnectFunc();
            string host, catalog, user, pass = "";
            conn.GetConnection(out host, out catalog, out user, out pass, WilayahID, SyarikatID, NegaraID);
            GenSalaryModelEstate db2 = GenSalaryModelEstate.ConnectToSqlServer(host, catalog, user, pass);

            return db2.vw_KerjaInfoDetails.Where(x => x.fld_LadangID == LadangID && x.fld_Tarikh.Value.Month == Month && x.fld_Tarikh.Value.Year == Year && x.fld_OverallAmount != null).ToList();
        }

        public List<tbl_Kerja> tbl_Kerja(int? NegaraID, int? SyarikatID, int? WilayahID, int? LadangID, int? Month, int? Year)
        {

            GetConnectFunc conn = new GetConnectFunc();
            string host, catalog, user, pass = "";
            conn.GetConnection(out host, out catalog, out user, out pass, WilayahID, SyarikatID, NegaraID);
            GenSalaryModelEstate db2 = GenSalaryModelEstate.ConnectToSqlServer(host, catalog, user, pass);
            return db2.tbl_Kerja.Where(x => x.fld_LadangID == LadangID && x.fld_Tarikh.Value.Month == Month && x.fld_Tarikh.Value.Year == Year).ToList();
        }

        public List<tbl_Kerjahdr> tbl_Kerjahdr(int? NegaraID, int? SyarikatID, int? WilayahID, int? LadangID, int? Month, int? Year)
        {
            GetConnectFunc conn = new GetConnectFunc();
            string host, catalog, user, pass = "";
            conn.GetConnection(out host, out catalog, out user, out pass, WilayahID, SyarikatID, NegaraID);
            GenSalaryModelEstate db2 = GenSalaryModelEstate.ConnectToSqlServer(host, catalog, user, pass);
            return db2.tbl_Kerjahdr.Where(x => x.fld_LadangID == LadangID && x.fld_Tarikh.Value.Month == Month && x.fld_Tarikh.Value.Year == Year).ToList();
        }

        public List<tbl_GajiBulanan> tbl_GajiBulanan_Lepas(int? NegaraID, int? SyarikatID, int? WilayahID, int? LadangID, int? Month, int? Year)
        {
            GetConnectFunc conn = new GetConnectFunc();
            string host, catalog, user, pass = "";
            conn.GetConnection(out host, out catalog, out user, out pass, WilayahID, SyarikatID, NegaraID);
            GenSalaryModelEstate db2 = GenSalaryModelEstate.ConnectToSqlServer(host, catalog, user, pass);
            DateTime lastMonthDT = new DateTime(Year.Value, Month.Value, 1).AddMonths(-1);
            return db2.tbl_GajiBulanan.Where(x => x.fld_LadangID == LadangID && x.fld_Month == lastMonthDT.Month && x.fld_Year == lastMonthDT.Year).ToList();
        }

        public List<tbl_PkjIncrmntSalary> tbl_PkjIncrmntSalary(int? NegaraID, int? SyarikatID, int? WilayahID, int? LadangID)
        {
            GetConnectFunc conn = new GetConnectFunc();
            string host, catalog, user, pass = "";
            conn.GetConnection(out host, out catalog, out user, out pass, WilayahID, SyarikatID, NegaraID);
            GenSalaryModelEstate db2 = GenSalaryModelEstate.ConnectToSqlServer(host, catalog, user, pass);
            return db2.tbl_PkjIncrmntSalary.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_AppStatus == true).ToList();
        }

        public List<vw_Kerja_Bonus> vw_Kerja_Bonus(int? NegaraID, int? SyarikatID, int? WilayahID, int? LadangID, int? Month, int? Year, string NoPkj)
        {
            GetConnectFunc conn = new GetConnectFunc();
            string host, catalog, user, pass = "";
            conn.GetConnection(out host, out catalog, out user, out pass, WilayahID, SyarikatID, NegaraID);
            GenSalaryModelEstate db2 = GenSalaryModelEstate.ConnectToSqlServer(host, catalog, user, pass);

            return db2.vw_Kerja_Bonus.Where(x => x.fld_LadangID == LadangID && x.fld_Tarikh.Value.Month == Month && x.fld_Tarikh.Value.Year == Year && x.fld_Nopkj == NoPkj).ToList();
        }

        public List<tbl_Insentif> tbl_Insentif(int? NegaraID, int? SyarikatID, int? WilayahID, int? LadangID, int? Month, int? Year)
        {
            GetConnectFunc conn = new GetConnectFunc();
            string host, catalog, user, pass = "";
            conn.GetConnection(out host, out catalog, out user, out pass, WilayahID, SyarikatID, NegaraID);
            GenSalaryModelEstate db2 = GenSalaryModelEstate.ConnectToSqlServer(host, catalog, user, pass);
            return db2.tbl_Insentif.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Month == Month && x.fld_Year == Year).ToList();
        }

        public List<tbl_Produktiviti> tbl_Produktiviti(int? NegaraID, int? SyarikatID, int? WilayahID, int? LadangID, int? Month, int? Year)
        {
            GetConnectFunc conn = new GetConnectFunc();
            string host, catalog, user, pass = "";
            conn.GetConnection(out host, out catalog, out user, out pass, WilayahID, SyarikatID, NegaraID);
            GenSalaryModelEstate db2 = GenSalaryModelEstate.ConnectToSqlServer(host, catalog, user, pass);
            return db2.tbl_Produktiviti.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Month == Month && x.fld_Year == Year).ToList();
        }

        public List<tbl_CutiPeruntukan> tbl_CutiPeruntukan(int? NegaraID, int? SyarikatID, int? WilayahID, int? LadangID)
        {
            GetConnectFunc conn = new GetConnectFunc();
            string host, catalog, user, pass = "";
            conn.GetConnection(out host, out catalog, out user, out pass, WilayahID, SyarikatID, NegaraID);
            GenSalaryModelEstate db2 = GenSalaryModelEstate.ConnectToSqlServer(host, catalog, user, pass);
            return db2.tbl_CutiPeruntukan.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID).ToList();
        }
    }
}
