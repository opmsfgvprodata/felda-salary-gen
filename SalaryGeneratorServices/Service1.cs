using Microsoft.AspNet.SignalR.Client; //Ashahri - 27/02/2022
using SalaryGeneratorServices.CustomModels;
using SalaryGeneratorServices.FuncClass;
using SalaryGeneratorServices.ModelsEstate;
using SalaryGeneratorServices.ModelsHQ;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Linq;
using System.ServiceProcess;

namespace SalaryGeneratorServices
{
    public partial class Service1 : ServiceBase
    {
        private GenSalaryModelHQ db = new GenSalaryModelHQ();
        private GenSalaryModelEstate db2 = new GenSalaryModelEstate();
        private Step1Func Step1Func = new Step1Func();
        private Step2Func Step2Func = new Step2Func();
        private Step3Func Step3Func = new Step3Func();
        private Step4Func Step4Func = new Step4Func();
        private Step6Func Step6Func = new Step6Func();
        private Step7Func Step7Func = new Step7Func();
        //Added by kamalia 28/8/2021
        private Step5Func Step5Func = new Step5Func();
        private DateTimeFunc DateTimeFunc = new DateTimeFunc();
        private RemoveDataFunc RemoveDataFunc = new RemoveDataFunc();
        private LogFunc LogFunc = new LogFunc();
        string logMessage = ""; //Ashahri 27/02/2022


        public Service1()
        {
            InitializeComponent();
        }

        private void DoProcess()
        {
            long ServiceProcessID = 0;
            List<tbl_Pkjmast> Pkjmstlists = new List<tbl_Pkjmast>();
            List<CustMod_DateList> DateLists = new List<CustMod_DateList>();
            List<tbl_CutiKategori> CutiKategoriList = new List<tbl_CutiKategori>();
            ModelsHQ.tbl_SevicesProcess SevicesProcess = new ModelsHQ.tbl_SevicesProcess();
            List<CustMod_WorkerPaidLeave> WorkerPaidLeaveLists = new List<CustMod_WorkerPaidLeave>();
            tbl_Kerjahdr WorkingAtt = new tbl_Kerjahdr();
            List<tbl_JenisAktiviti> JenisAktiviti = new List<tbl_JenisAktiviti>();
            List<tbl_JenisInsentif> incentifsType = new List<tbl_JenisInsentif>();
            tbl_KerjaBonus KerjaBonus = new tbl_KerjaBonus();
            List<tbl_KerjaBonus> KerjaBonusList = new List<tbl_KerjaBonus>();
            List<tbl_KerjaOT> KerjaOT = new List<tbl_KerjaOT>();
            List<tbl_KerjaOT> KerjaOTList = new List<tbl_KerjaOT>();
            List<CustMod_WorkSCTrans> WorkSCTransList = new List<CustMod_WorkSCTrans>();
            //add by kamalia 15/2/22
            List<CustMod_AdminSCTrans> AdminSCTransList = new List<CustMod_AdminSCTrans>();
            Guid MonthSalaryID = new Guid();
            int? NegaraID = 0;
            int? SyarikatID = 0;
            int? WilayahID = 0;
            int? LadangID = 0;
            int? Month = 0;
            int? Year = 0;
            int? UserID = 0;
            string compCode = "";
            int KerjaBonusRemoveCount = 0;
            int KerjaOTRemoveCount = 0;
            int KerjahdrCutiRemoveCount = 0;
            int KerjahdrCutiBlmAmbilRemoveCount = 0;
            int GajiBulananRemoveCount = 0;
            int ScTranRemoveCount = 0;
            int ByrCrmnTmbhnRemoveCount = 0;
            DateTime? StartWorkDate = new DateTime();
            DateTime LastDateLoop = new DateTime();
            byte? PaidPeriod = 0;
            bool TakePaidLeave = false;
            decimal? WorkingPayment = 0;
            decimal? DiffAreaPayment = 0;
            decimal? DailyBonusPayment = 0;
            decimal? OTPayment = 0;
            decimal? OthrInsPayment = 0;
            decimal? DeductInsPayment = 0;
            decimal? AveragePayment = 0;
            decimal? AIPSPayment = 0;
            decimal? LeavePayment = 0;
            decimal? KWSPMjkn = 0;
            decimal? KWSPPkj = 0;
            decimal? SocsoMjkn = 0;
            decimal? SocsoPkj = 0;
            decimal? TotalOthrContMjkCont = 0;
            decimal? TotalOthrContPkjCont = 0;
            decimal? OverallSalary = 0;
            decimal? Salary = 0;
            bool AttendStatus = false;
            string AttCode = "";
            string KumCode = "";
            string Log = "";
            string ServiceName = "";
            decimal Percentage = 0;
            decimal CountData = 0;
            decimal LoopCountData = 1;

            int TotalPkjCount = 0;
            int TotalDateCount = 0;
            int TotalDataCount = 0;
            int DataCount = 1;

            int TotalDataCount2 = 0;
            int DataCount2 = 1;

            NegaraID = int.Parse(ConfigReader("configs", "negaraid"));
            SyarikatID = int.Parse(ConfigReader("configs", "syarikatid"));
            WilayahID = int.Parse(ConfigReader("configs", "wilayahid"));
            LadangID = int.Parse(ConfigReader("configs", "ladangid"));
            ServiceName = ConfigReader("configs", "servicename");

            try
            {
                var getservicesdetail = db.tbl_ServicesList.Where(x => x.fld_SevicesActivity == "LadangSalaryGen" && x.fldNegaraID == NegaraID && x.fldSyarikatID == SyarikatID && x.fldWilayahID == WilayahID && x.fldLadangID == LadangID).FirstOrDefault();
                //RemoveDataFunc.RemoveData_tbl_SevicesProcess(NegaraID, SyarikatID, WilayahID, LadangID, UserID, DateTimeFunc.GetDateTime(), Month, Year, getservicesdetail.fld_SevicesActivity, getservicesdetail.fld_ServicesName, getservicesdetail.fld_ClientID);
                //Step1Func.AddServicesProcessFunc(NegaraID, SyarikatID, WilayahID, LadangID, UserID, DateTimeFunc.GetDateTime(), Month, Year, getservicesdetail.fld_SevicesActivity, getservicesdetail.fld_ServicesName, getservicesdetail.fld_ClientID);
                SevicesProcess = Step1Func.GetServiceProcessDetail(NegaraID, SyarikatID, WilayahID, LadangID, UserID, DateTimeFunc.GetDateTime(), Month, Year, getservicesdetail.fld_SevicesActivity, getservicesdetail.fld_ServicesName, getservicesdetail.fld_ClientID);
                if (SevicesProcess != null)
                {
                    NegaraID = SevicesProcess.fld_NegaraID;
                    SyarikatID = SevicesProcess.fld_SyarikatID;
                    WilayahID = SevicesProcess.fld_WilayahID;
                    LadangID = SevicesProcess.fld_LadangID;
                    Month = SevicesProcess.fld_Month;
                    Year = SevicesProcess.fld_Year;
                    UserID = SevicesProcess.fld_UserID;
                    compCode = db.tbl_Ladang.Where(x => x.fld_ID == LadangID).Select(s => s.fld_CostCentre).FirstOrDefault();
                    ServiceProcessID = SevicesProcess.fld_ID;

                    #region Normal Salary Gen
                    if (SevicesProcess.fld_ProcessName == "LadangSalaryGen")
                    {
                        WriteLog("Start Process.", true, ServiceName, ServiceProcessID);
                        WriteLog("Get Services Details. (Data - Services Name : " + getservicesdetail.fld_ServicesName + ", Month/Year : " + Month + "/" + Year + ")", false, ServiceName, ServiceProcessID);

                        WriteLog("Added into Services Process. (Data - Services Process ID : " + ServiceProcessID + ")", false, ServiceName, ServiceProcessID);
                        Pkjmstlists = Step1Func.GetActiveWorkerFunc(NegaraID, SyarikatID, WilayahID, LadangID, UserID, DateTimeFunc.GetDateTime(), Month, Year, getservicesdetail.fld_SevicesActivity, getservicesdetail.fld_ServicesName, getservicesdetail.fld_ClientID);
                        WriteLog("Get Active Worker. (Data - Total Active Worker : " + Pkjmstlists.Count + ")", false, ServiceName, ServiceProcessID);
                        KerjaBonusRemoveCount = RemoveDataFunc.RemoveData_tbl_KerjaBonus(NegaraID, SyarikatID, WilayahID, LadangID, UserID, DateTimeFunc.GetDateTime(), Month, Year, getservicesdetail.fld_SevicesActivity, getservicesdetail.fld_ServicesName, getservicesdetail.fld_ClientID);
                        WriteLog("Removed Calculation Kerja Bonus Data. (Data - Total Data Removed : " + KerjaBonusRemoveCount + ")", false, ServiceName, ServiceProcessID);
                        KerjaOTRemoveCount = RemoveDataFunc.RemoveData_tbl_KerjaOT(NegaraID, SyarikatID, WilayahID, LadangID, UserID, DateTimeFunc.GetDateTime(), Month, Year, getservicesdetail.fld_SevicesActivity, getservicesdetail.fld_ServicesName, getservicesdetail.fld_ClientID);
                        WriteLog("Removed Calculation Kerja OT Data. (Data - Total Data Removed : " + KerjaOTRemoveCount + ")", false, ServiceName, ServiceProcessID);
                        KerjahdrCutiRemoveCount = RemoveDataFunc.RemoveData_tbl_KerjahdrCuti(NegaraID, SyarikatID, WilayahID, LadangID, UserID, DateTimeFunc.GetDateTime(), Month, Year, getservicesdetail.fld_SevicesActivity, getservicesdetail.fld_ServicesName, getservicesdetail.fld_ClientID);
                        WriteLog("Removed Calculation Hadir Cuti Data. (Data - Total Data Removed : " + KerjahdrCutiRemoveCount + ")", false, ServiceName, ServiceProcessID);
                        KerjahdrCutiBlmAmbilRemoveCount = RemoveDataFunc.RemoveData_tbl_KerjahdrCutiTahunan(NegaraID, SyarikatID, WilayahID, LadangID, UserID, DateTimeFunc.GetDateTime(), Month, Year, getservicesdetail.fld_SevicesActivity, getservicesdetail.fld_ServicesName, getservicesdetail.fld_ClientID);
                        WriteLog("Removed Calculation Cuti Tahunan Data. (Data - Total Data Removed : " + KerjahdrCutiBlmAmbilRemoveCount + ")", false, ServiceName, ServiceProcessID);
                        GajiBulananRemoveCount = RemoveDataFunc.RemoveData_tbl_GajiBulanan(NegaraID, SyarikatID, WilayahID, LadangID, UserID, DateTimeFunc.GetDateTime(), Month, Year, getservicesdetail.fld_SevicesActivity, getservicesdetail.fld_ServicesName, getservicesdetail.fld_ClientID);
                        WriteLog("Removed Calculation Monthly Salary Data. (Data - Total Data Removed : " + GajiBulananRemoveCount + ")", false, ServiceName, ServiceProcessID);
                        ByrCrmnTmbhnRemoveCount = RemoveDataFunc.RemoveData_tbl_CarumanTambahan(NegaraID, SyarikatID, WilayahID, LadangID, UserID, DateTimeFunc.GetDateTime(), Month, Year, getservicesdetail.fld_SevicesActivity, getservicesdetail.fld_ServicesName, getservicesdetail.fld_ClientID);
                        WriteLog("Removed Calculation Additional Contribution. (Data - Total Data Removed : " + ByrCrmnTmbhnRemoveCount + ")", false, ServiceName, ServiceProcessID);

                        CutiKategoriList = Step1Func.GetPaidLeaveFunc(NegaraID, SyarikatID, WilayahID, LadangID);
                        JenisAktiviti = Step1Func.GetActvtyTypeBonusStatusFunc(NegaraID, SyarikatID, WilayahID, LadangID);
                        incentifsType = Step1Func.GetIncentifsTypeFunc(NegaraID, SyarikatID);
                        var bonusHarian = Step1Func.GetBonusHarianBukanMenuai(NegaraID, SyarikatID);
                        var tbl_HargaSawitSemasa = Step1Func.GetHargaSawitSemasa(NegaraID, SyarikatID, Month, Year);
                        string[] flag1 = new string[] { "kadarot", "kiraot", "cuti", "sbbTakAktif", "aktvtexcludeTL" };
                        List<tblOptionConfigsWeb> tblOptionConfigsWeb = Step1Func.GetWebConfigList(flag1, NegaraID, SyarikatID);
                        var tbl_GajiMinimaLdg = Step1Func.GetGajiMinimaLdg(LadangID);
                        var tbl_UpahAktiviti = Step1Func.tbl_UpahAktiviti(NegaraID, SyarikatID);
                        var tbl_JenisInsentif = Step1Func.tbl_JenisInsentif(NegaraID, SyarikatID);
                        var tbl_Kwsp = Step1Func.tbl_Kwsp(NegaraID, SyarikatID);
                        var tbl_Socso = Step1Func.tbl_Socso(NegaraID, SyarikatID);
                        var tbl_CarumanTambahan = Step1Func.tbl_CarumanTambahan(NegaraID, SyarikatID);
                        var tbl_SubCarumanTambahan = Step1Func.tbl_SubCarumanTambahan(NegaraID, SyarikatID);
                        var tbl_JadualCarumanTambahan = Step1Func.tbl_JadualCarumanTambahan(NegaraID, SyarikatID);

                        TotalPkjCount = Pkjmstlists.Count;
                        WriteLog("Total Pkj Count. (Data - Total Data : " + TotalPkjCount + ")", false, ServiceName, ServiceProcessID);
                        DateLists = Step1Func.GetDateListFunc(Month, Year);
                        TotalDateCount = DateLists.Count;
                        WriteLog("Total Date Count. (Data - Total Data : " + TotalDateCount + ")", false, ServiceName, ServiceProcessID);
                        TotalDataCount = TotalPkjCount * TotalDateCount;
                        TotalDataCount2 = TotalDataCount + 17; //modified by kamalia 26/4/2022
                        WriteLog("Total Data Count. (Data - Total Data : " + TotalDataCount2 + ")", false, ServiceName, ServiceProcessID);
                        var vw_KerjaInfoDetails = Step2Func.vw_KerjaInfoDetails(NegaraID, SyarikatID, WilayahID, LadangID, Month, Year);
                        var tbl_Kerja = Step2Func.tbl_Kerja(NegaraID, SyarikatID, WilayahID, LadangID, Month, Year);
                        var tbl_Kerjahdr = Step2Func.tbl_Kerjahdr(NegaraID, SyarikatID, WilayahID, LadangID, Month, Year);
                        var tbl_Kerjahdr12Month = Step2Func.tbl_Kerjahdr12Month(NegaraID, SyarikatID, WilayahID, LadangID, Month, Year);
                        var tbl_GajiBulanan_Lepas = Step2Func.tbl_GajiBulanan_Lepas(NegaraID, SyarikatID, WilayahID, LadangID, Month, Year);
                        var tbl_PkjIncrmntSalary = Step2Func.tbl_PkjIncrmntSalary(NegaraID, SyarikatID, WilayahID, LadangID);
                        var tbl_Insentif = Step2Func.tbl_Insentif(NegaraID, SyarikatID, WilayahID, LadangID, Month, Year);
                        var tbl_Produktiviti = Step2Func.tbl_Produktiviti(NegaraID, SyarikatID, WilayahID, LadangID, Month, Year);
                        var tbl_CutiPeruntukan = Step2Func.tbl_CutiPeruntukan(NegaraID, SyarikatID, WilayahID, LadangID);
                        foreach (var Pkjmstlist in Pkjmstlists)
                        {
                            Step1Func.UpdateServicesProcessPercFunc(SevicesProcess, Percentage, 1, db, TotalDataCount2, DataCount2);
                            KerjaBonusList = new List<tbl_KerjaBonus>();
                            KerjaOTList = new List<tbl_KerjaOT>();
                            WorkerPaidLeaveLists = new List<CustMod_WorkerPaidLeave>();
                            WriteLog("Get Worker Data. (Data : Worker No : " + Pkjmstlist.fld_Nopkj.Trim().Trim() + ", Worker Name : " + Pkjmstlist.fld_Nama.Trim() + ")", false, ServiceName, ServiceProcessID);
                            DateLists = Step1Func.GetDateListFunc(Month, Year);
                            WriteLog("Get Date List. (Data - No Pkj : " + Pkjmstlist.fld_Nopkj.Trim() + ", Date From : " + string.Format("{0:dd/MM/yyyy}", DateLists.OrderBy(o => o.Date).Select(s => s.Date).Take(1).FirstOrDefault()) + ", Date Until : " + string.Format("{0:dd/MM/yyyy}", DateLists.OrderByDescending(o => o.Date).Select(s => s.Date).Take(1).FirstOrDefault()) + ")", false, ServiceName, ServiceProcessID);
                            LastDateLoop = DateLists.OrderByDescending(o => o.Date).Select(s => s.Date).Take(1).FirstOrDefault();
                            StartWorkDate = Step1Func.GetDateStarkWorkingFunc(NegaraID, SyarikatID, WilayahID, LadangID, Pkjmstlist.fld_Nopkj.Trim(), tbl_Kerja, tbl_Kerjahdr, CutiKategoriList);
                            if (StartWorkDate != null)
                            {
                                if (LoopCountData == 1)
                                {
                                    CountData = (Pkjmstlists.Count * DateLists.Count) + 12;
                                }
                                WriteLog("Get Worker Start Working Date. (Data - No Pkj : " + Pkjmstlist.fld_Nopkj.Trim() + ", Start Working Date : " + string.Format("{0:dd/MM/yyyy}", StartWorkDate) + ")", false, ServiceName, ServiceProcessID);
                                foreach (var DateList in DateLists)
                                {
                                    DataCount = DataCount + 1;
                                    DataCount2 = DataCount;
                                    Percentage = (DataCount / TotalDataCount) * 79.5m;
                                    WriteLog("Total Loop Data Count. (Data - Total Data : " + DataCount2 + ")", false, ServiceName, ServiceProcessID);
                                    Step1Func.UpdateServicesProcessPercFunc(SevicesProcess, Percentage, 1, db, TotalDataCount2, DataCount2);
                                    WriteLog("Get Date. (Data - No Pkj : " + Pkjmstlist.fld_Nopkj.Trim() + ", Date : " + string.Format("{0:dd/MM/yyyy}", DateList.Date) + ")", false, ServiceName, ServiceProcessID);
                                    TakePaidLeave = Step2Func.GetWorkingPaidLeaveFunc(NegaraID, SyarikatID, WilayahID, LadangID, UserID, DateTimeFunc.GetDateTime(), Month, Year, getservicesdetail.fld_SevicesActivity, getservicesdetail.fld_ServicesName, getservicesdetail.fld_ClientID, Pkjmstlist.fld_Nopkj.Trim(), DateList.Date, CutiKategoriList, out PaidPeriod, out WorkingAtt, out KumCode, tbl_Kerjahdr);
                                    WriteLog("Get Paid Leave. (Data - No Pkj : " + Pkjmstlist.fld_Nopkj.Trim() + ", Paid Leave : " + TakePaidLeave + ")", false, ServiceName, ServiceProcessID);
                                    if (TakePaidLeave)
                                    {
                                        WriteLog("Code Paid Leave. (Data - No Pkj : " + Pkjmstlist.fld_Nopkj.Trim() + ", Leave Code : " + WorkingAtt.fld_Kdhdct + ")", false, ServiceName, ServiceProcessID);

                                        WorkerPaidLeaveLists.Add(new CustMod_WorkerPaidLeave() { fld_Nopkj = Pkjmstlist.fld_Nopkj.Trim(), fld_Kdhdct = WorkingAtt.fld_Kdhdct, fld_Tarikh = DateList.Date, fld_PaidPeriod = PaidPeriod, fld_KerjahdrID = WorkingAtt.fld_UniqueID, fld_Kum = KumCode });
                                    }
                                    else
                                    {
                                        AttendStatus = Step2Func.GetAttendStatusFunc(NegaraID, SyarikatID, WilayahID, LadangID, UserID, DateTimeFunc.GetDateTime(), Month, Year, getservicesdetail.fld_SevicesActivity, getservicesdetail.fld_ServicesName, getservicesdetail.fld_ClientID, Pkjmstlist.fld_Nopkj.Trim(), DateList.Date, out AttCode, tblOptionConfigsWeb, tbl_Kerjahdr);
                                        if (AttendStatus)
                                        {
                                            Step2Func.GetDailyBonusFunc(NegaraID, SyarikatID, WilayahID, LadangID, UserID, DateTimeFunc.GetDateTime(), Month, Year, getservicesdetail.fld_SevicesActivity, getservicesdetail.fld_ServicesName, getservicesdetail.fld_ClientID, Pkjmstlist.fld_Nopkj.Trim(), DateList.Date, out KerjaBonus, JenisAktiviti, bonusHarian, tbl_Kerja, tbl_HargaSawitSemasa);
                                            if (KerjaBonus != null)
                                            {
                                                WriteLog("Get Daily Bonus. (Data - No Pkj : " + Pkjmstlist.fld_Nopkj.Trim() + ", Bonus Price : RM " + KerjaBonus.fld_Jumlah + ")", false, ServiceName, ServiceProcessID);
                                                KerjaBonusList.Add(KerjaBonus);
                                            }
                                            else
                                            {
                                                WriteLog("No Daily Bonus.", false, ServiceName, ServiceProcessID);
                                            }

                                            Step2Func.GetOTFunc(NegaraID, SyarikatID, WilayahID, LadangID, UserID, DateTimeFunc.GetDateTime(), Month, Year, getservicesdetail.fld_SevicesActivity, getservicesdetail.fld_ServicesName, getservicesdetail.fld_ClientID, Pkjmstlist.fld_Nopkj.Trim(), DateList.Date, out KerjaOT, AttCode, JenisAktiviti, tbl_Kerja, tbl_GajiBulanan_Lepas, tblOptionConfigsWeb, tbl_GajiMinimaLdg, tbl_PkjIncrmntSalary, tbl_UpahAktiviti);

                                            if (KerjaOT.Count() > 0)
                                            {
                                                WriteLog("Get Daily OT. (Data - No Pkj : " + Pkjmstlist.fld_Nopkj.Trim() + ", OT Price : RM " + KerjaOT.Sum(s => s.fld_Jumlah) + ")", false, ServiceName, ServiceProcessID);
                                                KerjaOTList.AddRange(KerjaOT);
                                            }
                                            else
                                            {
                                                WriteLog("No Daily OT.", false, ServiceName, ServiceProcessID);
                                            }
                                        }
                                        else
                                        {
                                            WriteLog("On Leave. (Data - No Pkj : " + Pkjmstlist.fld_Nopkj.Trim() + ", Leave Code : " + AttCode + ")", false, ServiceName, ServiceProcessID);
                                        }
                                    }

                                    if (LastDateLoop == DateList.Date) // untuk paid leave
                                    {
                                        Step2Func.AddTo_tbl_KerjaBonus(NegaraID, SyarikatID, WilayahID, LadangID, KerjaBonusList);
                                        WriteLog("Insert To tbl_KerjaBonus. (Data - No Pkj : " + Pkjmstlist.fld_Nopkj.Trim() + ", Total Data : " + KerjaBonusList.Count + ")", false, ServiceName, ServiceProcessID);
                                        Step2Func.AddTo_tbl_KerjaOT(NegaraID, SyarikatID, WilayahID, LadangID, KerjaOTList);
                                        WriteLog("Insert To tbl_KerjaOT. (Data - No Pkj : " + Pkjmstlist.fld_Nopkj.Trim() + ", Total Data : " + KerjaOTList.Count + ")", false, ServiceName, ServiceProcessID);
                                        MonthSalaryID = Step3Func.GetPaidWorkingFunc(NegaraID, SyarikatID, WilayahID, LadangID, UserID, DateTimeFunc.GetDateTime(), Month, Year, getservicesdetail.fld_SevicesActivity, getservicesdetail.fld_ServicesName, getservicesdetail.fld_ClientID, Pkjmstlist.fld_Nopkj.Trim(), out WorkingPayment, out DiffAreaPayment, tbl_Kerja);
                                        WriteLog("Get Daily Work Payment. (Data - No Pkj : " + Pkjmstlist.fld_Nopkj.Trim() + ", Total Payment : RM " + WorkingPayment + ")", false, ServiceName, ServiceProcessID);
                                        AveragePayment = Step3Func.GetAveragePaidFunc(NegaraID, SyarikatID, WilayahID, LadangID, UserID, DateTimeFunc.GetDateTime(), Month, Year, getservicesdetail.fld_SevicesActivity, getservicesdetail.fld_ServicesName, getservicesdetail.fld_ClientID, Pkjmstlist.fld_Nopkj.Trim(), MonthSalaryID, tblOptionConfigsWeb, tbl_Kerjahdr);
                                        WriteLog("Get Average Payment. (Data - No Pkj : " + Pkjmstlist.fld_Nopkj.Trim() + ", Total Payment : RM " + AveragePayment + ")", false, ServiceName, ServiceProcessID);
                                        DailyBonusPayment = Step3Func.GetPaidDailyBonusFunc(NegaraID, SyarikatID, WilayahID, LadangID, UserID, DateTimeFunc.GetDateTime(), Month, Year, getservicesdetail.fld_SevicesActivity, getservicesdetail.fld_ServicesName, getservicesdetail.fld_ClientID, Pkjmstlist.fld_Nopkj.Trim(), MonthSalaryID, KerjaBonusList);
                                        WriteLog("Get Daily Bonus Payment. (Data - No Pkj : " + Pkjmstlist.fld_Nopkj.Trim() + ", Total Payment : RM " + DailyBonusPayment + ")", false, ServiceName, ServiceProcessID);
                                        OTPayment = Step3Func.GetPaidOTFunc(NegaraID, SyarikatID, WilayahID, LadangID, UserID, DateTimeFunc.GetDateTime(), Month, Year, getservicesdetail.fld_SevicesActivity, getservicesdetail.fld_ServicesName, getservicesdetail.fld_ClientID, Pkjmstlist.fld_Nopkj.Trim(), MonthSalaryID, KerjaOTList);
                                        WriteLog("Get OT Payment. (Data - No Pkj : " + Pkjmstlist.fld_Nopkj.Trim() + ", Total Payment : RM " + OTPayment + ")", false, ServiceName, ServiceProcessID);
                                        var workerIncentifs = new List<tbl_Insentif>();
                                        OthrInsPayment = Step3Func.GetPaidInsentifFunc(NegaraID, SyarikatID, WilayahID, LadangID, UserID, DateTimeFunc.GetDateTime(), Month, Year, getservicesdetail.fld_SevicesActivity, getservicesdetail.fld_ServicesName, getservicesdetail.fld_ClientID, Pkjmstlist.fld_Nopkj.Trim(), MonthSalaryID, out workerIncentifs, tbl_JenisInsentif, tbl_Insentif);
                                        WriteLog("Get Other Insentif Payment. (Data - No Pkj : " + Pkjmstlist.fld_Nopkj.Trim() + ", Total Payment : RM " + OthrInsPayment + ")", false, ServiceName, ServiceProcessID);
                                        DeductInsPayment = Step3Func.GetDeductInsentifFunc(NegaraID, SyarikatID, WilayahID, LadangID, UserID, DateTimeFunc.GetDateTime(), Month, Year, getservicesdetail.fld_SevicesActivity, getservicesdetail.fld_ServicesName, getservicesdetail.fld_ClientID, Pkjmstlist.fld_Nopkj.Trim(), MonthSalaryID, tbl_JenisInsentif, tbl_Insentif);
                                        WriteLog("Get Insentif Deduction. (Data - No Pkj : " + Pkjmstlist.fld_Nopkj.Trim() + ", Total Deduction : RM " + DeductInsPayment + ")", false, ServiceName, ServiceProcessID);
                                        AIPSPayment = Step3Func.GetAIPSFunc(NegaraID, SyarikatID, WilayahID, LadangID, UserID, DateTimeFunc.GetDateTime(), Month, Year, getservicesdetail.fld_SevicesActivity, getservicesdetail.fld_ServicesName, getservicesdetail.fld_ClientID, Pkjmstlist.fld_Nopkj.Trim(), MonthSalaryID, tbl_Produktiviti);
                                        WriteLog("Get AIPS Insentif. (Data - No Pkj : " + Pkjmstlist.fld_Nopkj.Trim() + ", Total Payment : RM " + AIPSPayment + ")", false, ServiceName, ServiceProcessID);

                                        var vw_Kerja_Bonus = Step2Func.vw_Kerja_Bonus(NegaraID, SyarikatID, WilayahID, LadangID, Month, Year, Pkjmstlist.fld_Nopkj);

                                        if (WorkerPaidLeaveLists.Count > 0)
                                        {
                                            //modified by kamalia 1/6/2021
                                            LeavePayment = Step3Func.GetPaidLeaveFunc(NegaraID, SyarikatID, WilayahID, LadangID, UserID, DateTimeFunc.GetDateTime(), Month, Year, getservicesdetail.fld_SevicesActivity, getservicesdetail.fld_ServicesName, getservicesdetail.fld_ClientID, Pkjmstlist.fld_Nopkj.Trim(), MonthSalaryID, WorkerPaidLeaveLists, StartWorkDate, false, CutiKategoriList, Pkjmstlist, tbl_GajiMinimaLdg, tbl_GajiBulanan_Lepas, tbl_Kerjahdr, tblOptionConfigsWeb, tbl_CutiPeruntukan, compCode, workerIncentifs, incentifsType, vw_KerjaInfoDetails, vw_Kerja_Bonus, tbl_Kerjahdr12Month);
                                            WriteLog("Get Leave Payment (Have Count Leave). (Data - No Pkj : " + Pkjmstlist.fld_Nopkj.Trim() + ", Total Payment : RM " + LeavePayment + ")", false, ServiceName, ServiceProcessID);
                                        }
                                        else
                                        {
                                            //modified by kamalia 1/6/2021
                                            LeavePayment = Step3Func.GetPaidLeaveFunc(NegaraID, SyarikatID, WilayahID, LadangID, UserID, DateTimeFunc.GetDateTime(), Month, Year, getservicesdetail.fld_SevicesActivity, getservicesdetail.fld_ServicesName, getservicesdetail.fld_ClientID, Pkjmstlist.fld_Nopkj.Trim(), MonthSalaryID, WorkerPaidLeaveLists, StartWorkDate, true, CutiKategoriList, Pkjmstlist, tbl_GajiMinimaLdg, tbl_GajiBulanan_Lepas, tbl_Kerjahdr, tblOptionConfigsWeb, tbl_CutiPeruntukan, compCode, workerIncentifs, incentifsType, vw_KerjaInfoDetails, vw_Kerja_Bonus, tbl_Kerjahdr12Month);
                                            WriteLog("Get Leave Payment (No Count Leave). (Data - No Pkj : " + Pkjmstlist.fld_Nopkj.Trim() + ", Total Payment : RM " + LeavePayment + ")", false, ServiceName, ServiceProcessID);
                                        }

                                        if (Pkjmstlist.fld_StatusKwspSocso == "1")
                                        {
                                            Step3Func.GetKWSPFunc(NegaraID, SyarikatID, WilayahID, LadangID, UserID, DateTimeFunc.GetDateTime(), Month, Year, getservicesdetail.fld_SevicesActivity, getservicesdetail.fld_ServicesName, getservicesdetail.fld_ClientID, Pkjmstlist.fld_Nopkj.Trim(), MonthSalaryID, Pkjmstlist.fld_KodKWSP, out KWSPMjkn, out KWSPPkj, false, tbl_JenisInsentif, tbl_Insentif, tbl_Kwsp);
                                            WriteLog("Get KWSP. (Data - No Pkj : " + Pkjmstlist.fld_Nopkj.Trim() + ", Employer : RM " + KWSPMjkn + ", Employee : RM " + KWSPPkj + ")", false, ServiceName, ServiceProcessID);
                                        }
                                        else
                                        {
                                            Step3Func.GetKWSPFunc(NegaraID, SyarikatID, WilayahID, LadangID, UserID, DateTimeFunc.GetDateTime(), Month, Year, getservicesdetail.fld_SevicesActivity, getservicesdetail.fld_ServicesName, getservicesdetail.fld_ClientID, Pkjmstlist.fld_Nopkj.Trim(), MonthSalaryID, Pkjmstlist.fld_KodKWSP, out KWSPMjkn, out KWSPPkj, true, tbl_JenisInsentif, tbl_Insentif, tbl_Kwsp);
                                            WriteLog("No KWSP. (Data - No Pkj : " + Pkjmstlist.fld_Nopkj.Trim() + ")", false, ServiceName, ServiceProcessID);
                                        }

                                        if (Pkjmstlist.fld_StatusKwspSocso == "1")
                                        {
                                            Step3Func.GetSocsoFunc(NegaraID, SyarikatID, WilayahID, LadangID, UserID, DateTimeFunc.GetDateTime(), Month, Year, getservicesdetail.fld_SevicesActivity, getservicesdetail.fld_ServicesName, getservicesdetail.fld_ClientID, Pkjmstlist.fld_Nopkj.Trim(), MonthSalaryID, Pkjmstlist.fld_KodSocso, out SocsoMjkn, out SocsoPkj, false, tbl_JenisInsentif, tbl_Insentif, tbl_Socso);
                                            WriteLog("Get Socso. (Data - No Pkj : " + Pkjmstlist.fld_Nopkj.Trim() + ", Employer : RM " + SocsoMjkn + ", Employee : RM " + SocsoPkj + ")", false, ServiceName, ServiceProcessID);
                                        }
                                        else
                                        {
                                            Step3Func.GetSocsoFunc(NegaraID, SyarikatID, WilayahID, LadangID, UserID, DateTimeFunc.GetDateTime(), Month, Year, getservicesdetail.fld_SevicesActivity, getservicesdetail.fld_ServicesName, getservicesdetail.fld_ClientID, Pkjmstlist.fld_Nopkj.Trim(), MonthSalaryID, Pkjmstlist.fld_KodSocso, out SocsoMjkn, out SocsoPkj, true, tbl_JenisInsentif, tbl_Insentif, tbl_Socso);
                                            WriteLog("No Socso. (Data - No Pkj : " + Pkjmstlist.fld_Nopkj.Trim() + ")", false, ServiceName, ServiceProcessID);
                                        }

                                        //added by faeza 12.11.2021
                                        Step3Func.UpdatePaymentMode(NegaraID, SyarikatID, WilayahID, LadangID, UserID, DateTimeFunc.GetDateTime(), Month, Year, getservicesdetail.fld_SevicesActivity, getservicesdetail.fld_ServicesName, getservicesdetail.fld_ClientID, Pkjmstlist.fld_Nopkj.Trim(), Pkjmstlist.fld_PaymentMode, MonthSalaryID);
                                        WriteLog("Update Payment Mode. (Data - No Pkj : " + Pkjmstlist.fld_Nopkj.Trim() + ", Payment Mode : " + Pkjmstlist.fld_PaymentMode + ")", false, ServiceName, ServiceProcessID);


                                        Step3Func.GetOtherContributionsFunc(NegaraID, SyarikatID, WilayahID, LadangID, UserID, DateTimeFunc.GetDateTime(), Month, Year, getservicesdetail.fld_SevicesActivity, getservicesdetail.fld_ServicesName, getservicesdetail.fld_ClientID, Pkjmstlist.fld_Nopkj.Trim(), MonthSalaryID, out TotalOthrContMjkCont, out TotalOthrContPkjCont, tbl_JenisInsentif, tbl_Insentif, tbl_CarumanTambahan, tbl_SubCarumanTambahan, tbl_JadualCarumanTambahan);
                                        WriteLog("Get Other Contribution. (Data - No Pkj : " + Pkjmstlist.fld_Nopkj.Trim() + ", Employer : RM " + TotalOthrContMjkCont + ", Employee : RM " + TotalOthrContPkjCont + ")", false, ServiceName, ServiceProcessID);
                                        Step3Func.GetOverallSalaryFunc(NegaraID, SyarikatID, WilayahID, LadangID, UserID, DateTimeFunc.GetDateTime(), Month, Year, getservicesdetail.fld_SevicesActivity, getservicesdetail.fld_ServicesName, getservicesdetail.fld_ClientID, Pkjmstlist.fld_Nopkj.Trim(), MonthSalaryID, out OverallSalary, out Salary);
                                        WriteLog("Get Overall Salary. (Data - No Pkj : " + Pkjmstlist.fld_Nopkj.Trim() + ", Total Payment : RM " + OverallSalary + ")", false, ServiceName, ServiceProcessID);
                                        WriteLog("Get Salary. (Data - No Pkj : " + Pkjmstlist.fld_Nopkj.Trim() + ", Total Payment : RM " + Salary + ")", false, ServiceName, ServiceProcessID);
                                    }
                                }
                            }
                            else
                            {
                                WriteLog("Get Worker Start Working Date. (Data - No Pkj : " + Pkjmstlist.fld_Nopkj.Trim() + ", Start Working Date : Date Not Found)", false, ServiceName, ServiceProcessID);
                            }
                        }

                        WriteLog("Total Loop Data Count. (Data - Total Data : " + DataCount2 + ")", false, ServiceName, ServiceProcessID);
                        Step1Func.UpdateServicesProcessPercFunc(SevicesProcess, 80, 1, db, TotalDataCount2, DataCount2);
                        if (compCode == "8800")
                        {
                            if (Pkjmstlists != null)
                            {
                                var tbl_CustomerVendorGLMap = db.tbl_CustomerVendorGLMap.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_TypeCode == "GL" && x.fld_compcode == compCode).ToList();
                                DataCount = 1;
                                DataCount2 = DataCount2 + DataCount;
                                //TotalDataCount = 14; //commented by faeza 08.10.2021
                                TotalDataCount = 17; //modified by kamy 16/4/2022
                                WriteLog("Start to create Transaction Listing", false, ServiceName, ServiceProcessID);

                                ScTranRemoveCount = RemoveDataFunc.RemoveData_tbl_Sctran(NegaraID, SyarikatID, WilayahID, LadangID, UserID, DateTimeFunc.GetDateTime(), Month, Year, getservicesdetail.fld_SevicesActivity, getservicesdetail.fld_ServicesName, getservicesdetail.fld_ClientID);
                                //Percentage = Math.Round((LoopCountData++ / CountData) * 100, 2); //commented by faeza 08.10.2021
                                Percentage = ((DataCount / TotalDataCount) * 19.5m) + 80; //added by faeza 08.10.2021
                                WriteLog("Total Loop Data Count. (Data - Total Data : " + DataCount2 + ")", false, ServiceName, ServiceProcessID);
                                Step1Func.UpdateServicesProcessPercFunc(SevicesProcess, Percentage, 1, db, TotalDataCount2, DataCount2);
                                WriteLog("Removed Transaction Listing Data. (Data - Total Data Removed : " + ScTranRemoveCount + ")", false, ServiceName, ServiceProcessID);

                                WorkSCTransList = Step6Func.GetWorkActvtyPktFunc(NegaraID, SyarikatID, WilayahID, LadangID, UserID, DateTimeFunc.GetDateTime(), Month, Year, getservicesdetail.fld_SevicesActivity, getservicesdetail.fld_ServicesName, getservicesdetail.fld_ClientID, Pkjmstlists, compCode, tbl_UpahAktiviti);
                                DataCount = DataCount + 1;
                                DataCount2 = DataCount2 + 1;
                                //Percentage = Math.Round((LoopCountData++ / CountData) * 100, 2); //commented by faeza 08.10.2021
                                Percentage = ((DataCount / TotalDataCount) * 19.5m) + 80; //added by faeza 08.10.2021
                                WriteLog("Total Loop Data Count. (Data - Total Data : " + DataCount2 + ")", false, ServiceName, ServiceProcessID);
                                Step1Func.UpdateServicesProcessPercFunc(SevicesProcess, Percentage, 1, db, TotalDataCount2, DataCount2);
                                WriteLog("Get Work Data By Activity & Peringkat. (Data - Total Data : " + WorkSCTransList.Count + ")", false, ServiceName, ServiceProcessID);

                                //added by kamalia 16/2/22
                                AdminSCTransList = Step6Func.GetWorkAdminPktFunc(NegaraID, SyarikatID, WilayahID, LadangID, UserID, DateTimeFunc.GetDateTime(), Month, Year, getservicesdetail.fld_SevicesActivity, getservicesdetail.fld_ServicesName, getservicesdetail.fld_ClientID, Pkjmstlists, compCode, CutiKategoriList, tbl_Kerjahdr, tbl_Kerja); ;
                                DataCount = DataCount + 1;
                                DataCount2 = DataCount2 + 1;
                                Percentage = ((DataCount / TotalDataCount) * 19.5m) + 80;
                                WriteLog("Total Loop Data Count. (Data - Total Data : " + DataCount2 + ")", false, ServiceName, ServiceProcessID);
                                Step1Func.UpdateServicesProcessPercFunc(SevicesProcess, Percentage, 1, db, TotalDataCount2, DataCount2);
                                WriteLog("Get Work Data By Activity & Peringkat. (Data - Total Data : " + AdminSCTransList.Count + ")", false, ServiceName, ServiceProcessID);

                                Step6Func.GetAmountWorkActivityFunc(NegaraID, SyarikatID, WilayahID, LadangID, UserID, DateTimeFunc.GetDateTime(), Month, Year, getservicesdetail.fld_SevicesActivity, getservicesdetail.fld_ServicesName, getservicesdetail.fld_ClientID, WorkSCTransList, out Log, Pkjmstlists);
                                DataCount = DataCount + 1;
                                DataCount2 = DataCount2 + 1;
                                //Percentage = Math.Round((LoopCountData++ / CountData) * 100, 2); //commented by faeza 08.10.2021
                                Percentage = ((DataCount / TotalDataCount) * 19.5m) + 80; //added by faeza 08.10.2021
                                WriteLog("Total Loop Data Count. (Data - Total Data : " + DataCount2 + ")", false, ServiceName, ServiceProcessID);
                                Step1Func.UpdateServicesProcessPercFunc(SevicesProcess, Percentage, 1, db, TotalDataCount2, DataCount2);
                                WriteLog2(Log, ServiceName, ServiceProcessID);

                                Step6Func.GetAmountDailyBonusFunc(NegaraID, SyarikatID, WilayahID, LadangID, UserID, DateTimeFunc.GetDateTime(), Month, Year, getservicesdetail.fld_SevicesActivity, getservicesdetail.fld_ServicesName, getservicesdetail.fld_ClientID, WorkSCTransList, out Log, Pkjmstlists);
                                DataCount = DataCount + 1;
                                DataCount2 = DataCount2 + 1;
                                //Percentage = Math.Round((LoopCountData++ / CountData) * 100, 2); //commented by faeza 08.10.2021
                                Percentage = ((DataCount / TotalDataCount) * 19.5m) + 80; //added by faeza 08.10.2021
                                WriteLog("Total Loop Data Count. (Data - Total Data : " + DataCount2 + ")", false, ServiceName, ServiceProcessID);
                                Step1Func.UpdateServicesProcessPercFunc(SevicesProcess, Percentage, 1, db, TotalDataCount2, DataCount2);
                                WriteLog2(Log, ServiceName, ServiceProcessID);

                                Step6Func.GetAmountOTFunc(NegaraID, SyarikatID, WilayahID, LadangID, UserID, DateTimeFunc.GetDateTime(), Month, Year, getservicesdetail.fld_SevicesActivity, getservicesdetail.fld_ServicesName, getservicesdetail.fld_ClientID, WorkSCTransList, out Log, Pkjmstlists, compCode, tbl_UpahAktiviti);
                                DataCount = DataCount + 1;
                                DataCount2 = DataCount2 + 1;
                                //Percentage = Math.Round((LoopCountData++ / CountData) * 100, 2); //commented by faeza 08.10.2021
                                Percentage = ((DataCount / TotalDataCount) * 19.5m) + 80; //added by faeza 08.10.2021
                                WriteLog("Total Loop Data Count. (Data - Total Data : " + DataCount2 + ")", false, ServiceName, ServiceProcessID);
                                Step1Func.UpdateServicesProcessPercFunc(SevicesProcess, Percentage, 1, db, TotalDataCount2, DataCount2);
                                WriteLog2(Log, ServiceName, ServiceProcessID);

                                Step6Func.GetAmountLeaveFunc(NegaraID, SyarikatID, WilayahID, LadangID, UserID, DateTimeFunc.GetDateTime(), Month, Year, getservicesdetail.fld_SevicesActivity, getservicesdetail.fld_ServicesName, getservicesdetail.fld_ClientID, AdminSCTransList, out Log, Pkjmstlists, tbl_CustomerVendorGLMap);//modified by kamalia 15/2/22
                                DataCount = DataCount + 1;
                                DataCount2 = DataCount2 + 1;
                                //Percentage = Math.Round((LoopCountData++ / CountData) * 100, 2); //commented by faeza 08.10.2021
                                Percentage = ((DataCount / TotalDataCount) * 19.5m) + 80; //added by faeza 08.10.2021
                                WriteLog("Total Loop Data Count. (Data - Total Data : " + DataCount2 + ")", false, ServiceName, ServiceProcessID);
                                Step1Func.UpdateServicesProcessPercFunc(SevicesProcess, Percentage, 1, db, TotalDataCount2, DataCount2);
                                WriteLog2(Log, ServiceName, ServiceProcessID);

                                Step6Func.GetAmountAddedInsentifFunc(NegaraID, SyarikatID, WilayahID, LadangID, UserID, DateTimeFunc.GetDateTime(), Month, Year, getservicesdetail.fld_SevicesActivity, getservicesdetail.fld_ServicesName, getservicesdetail.fld_ClientID, AdminSCTransList, out Log, Pkjmstlists, tbl_CustomerVendorGLMap);//modified by kamalia 15/2/22
                                DataCount = DataCount + 1;
                                DataCount2 = DataCount2 + 1;
                                //Percentage = Math.Round((LoopCountData++ / CountData) * 100, 2); //commented by faeza 08.10.2021
                                Percentage = ((DataCount / TotalDataCount) * 19.5m) + 80; //added by faeza 08.10.2021
                                WriteLog("Total Loop Data Count. (Data - Total Data : " + DataCount2 + ")", false, ServiceName, ServiceProcessID);
                                Step1Func.UpdateServicesProcessPercFunc(SevicesProcess, Percentage, 1, db, TotalDataCount2, DataCount2);
                                WriteLog2(Log, ServiceName, ServiceProcessID);

                                Step6Func.GetAmountDeductedInsentifFunc(NegaraID, SyarikatID, WilayahID, LadangID, UserID, DateTimeFunc.GetDateTime(), Month, Year, getservicesdetail.fld_SevicesActivity, getservicesdetail.fld_ServicesName, getservicesdetail.fld_ClientID, AdminSCTransList, out Log, Pkjmstlists, compCode, tbl_CustomerVendorGLMap);//modified by kamalia 15/2/22
                                DataCount = DataCount + 1;
                                DataCount2 = DataCount2 + 1;
                                //Percentage = Math.Round((LoopCountData++ / CountData) * 100, 2); //commented by faeza 08.10.2021
                                Percentage = ((DataCount / TotalDataCount) * 19.5m) + 80; //added by faeza 08.10.2021
                                WriteLog("Total Loop Data Count. (Data - Total Data : " + DataCount2 + ")", false, ServiceName, ServiceProcessID);
                                Step1Func.UpdateServicesProcessPercFunc(SevicesProcess, Percentage, 1, db, TotalDataCount2, DataCount2);
                                WriteLog2(Log, ServiceName, ServiceProcessID);

                                Step6Func.GetAmountKWSPFunc(NegaraID, SyarikatID, WilayahID, LadangID, UserID, DateTimeFunc.GetDateTime(), Month, Year, getservicesdetail.fld_SevicesActivity, getservicesdetail.fld_ServicesName, getservicesdetail.fld_ClientID, AdminSCTransList, out Log, Pkjmstlists, tbl_CustomerVendorGLMap);//modified by kamalia 15/2/22
                                DataCount = DataCount + 1;
                                DataCount2 = DataCount2 + 1;
                                //Percentage = Math.Round((LoopCountData++ / CountData) * 100, 2); //commented by faeza 08.10.2021
                                Percentage = ((DataCount / TotalDataCount) * 19.5m) + 80; //added by faeza 08.10.2021
                                WriteLog("Total Loop Data Count. (Data - Total Data : " + DataCount2 + ")", false, ServiceName, ServiceProcessID);
                                Step1Func.UpdateServicesProcessPercFunc(SevicesProcess, Percentage, 1, db, TotalDataCount2, DataCount2);
                                WriteLog2(Log, ServiceName, ServiceProcessID);

                                Step6Func.GetAmountSocsoFunc(NegaraID, SyarikatID, WilayahID, LadangID, UserID, DateTimeFunc.GetDateTime(), Month, Year, getservicesdetail.fld_SevicesActivity, getservicesdetail.fld_ServicesName, getservicesdetail.fld_ClientID, AdminSCTransList, out Log, Pkjmstlists, tbl_CustomerVendorGLMap);//modified by kamalia 15/2/22
                                DataCount = DataCount + 1;
                                DataCount2 = DataCount2 + 1;
                                //Percentage = Math.Round((LoopCountData++ / CountData) * 100, 2); //commented by faeza 08.10.2021
                                Percentage = ((DataCount / TotalDataCount) * 19.5m) + 80; //added by faeza 08.10.2021
                                WriteLog("Total Loop Data Count. (Data - Total Data : " + DataCount2 + ")", false, ServiceName, ServiceProcessID);
                                Step1Func.UpdateServicesProcessPercFunc(SevicesProcess, Percentage, 1, db, TotalDataCount2, DataCount2);
                                WriteLog2(Log, ServiceName, ServiceProcessID);

                                Step6Func.GetAmountOtherContributionsFunc(NegaraID, SyarikatID, WilayahID, LadangID, UserID, DateTimeFunc.GetDateTime(), Month, Year, getservicesdetail.fld_SevicesActivity, getservicesdetail.fld_ServicesName, getservicesdetail.fld_ClientID, AdminSCTransList, out Log, Pkjmstlists, tbl_CustomerVendorGLMap);//modified by kamalia 15/2/22
                                DataCount = DataCount + 1;
                                DataCount2 = DataCount2 + 1;
                                //Percentage = Math.Round((LoopCountData++ / CountData) * 100, 2); //commented by faeza 08.10.2021
                                Percentage = ((DataCount / TotalDataCount) * 19.5m) + 80; //added by faeza 08.10.2021
                                WriteLog("Total Loop Data Count. (Data - Total Data : " + DataCount2 + ")", false, ServiceName, ServiceProcessID);
                                Step1Func.UpdateServicesProcessPercFunc(SevicesProcess, Percentage, 1, db, TotalDataCount2, DataCount2);
                                WriteLog2(Log, ServiceName, ServiceProcessID);

                                //Step6Func.GetAmountAIPSFunc(NegaraID, SyarikatID, WilayahID, LadangID, UserID, DateTimeFunc.GetDateTime(), Month, Year, getservicesdetail.fld_SevicesActivity, getservicesdetail.fld_ServicesName, getservicesdetail.fld_ClientID, AdminSCTransList, out Log, Pkjmstlists);//modified by kamalia 15/2/22
                                DataCount = DataCount + 1;
                                DataCount2 = DataCount2 + 1;
                                //Percentage = Math.Round((LoopCountData++ / CountData) * 100, 2); //commented by faeza 08.10.2021
                                Percentage = ((DataCount / TotalDataCount) * 19.5m) + 80; //added by faeza 08.10.2021
                                WriteLog("Total Loop Data Count. (Data - Total Data : " + DataCount2 + ")", false, ServiceName, ServiceProcessID);
                                Step1Func.UpdateServicesProcessPercFunc(SevicesProcess, Percentage, 1, db, TotalDataCount2, DataCount2);
                                WriteLog2(Log, ServiceName, ServiceProcessID);

                                Step6Func.GetAmountWorkerSalaryFunc(NegaraID, SyarikatID, WilayahID, LadangID, UserID, DateTimeFunc.GetDateTime(), Month, Year, getservicesdetail.fld_SevicesActivity, getservicesdetail.fld_ServicesName, getservicesdetail.fld_ClientID, out Log, Pkjmstlists, compCode);
                                DataCount = DataCount + 1;
                                DataCount2 = DataCount2 + 1;
                                //Percentage = Math.Round((LoopCountData++ / CountData) * 100, 2); //commented by faeza 08.10.2021
                                Percentage = ((DataCount / TotalDataCount) * 19.5m) + 80; //added by faeza 08.10.2021
                                WriteLog("Total Loop Data Count. (Data - Total Data : " + DataCount2 + ")", false, ServiceName, ServiceProcessID);
                                Step1Func.UpdateServicesProcessPercFunc(SevicesProcess, Percentage, 1, db, TotalDataCount2, DataCount2);
                                WriteLog2(Log, ServiceName, ServiceProcessID);

                                Step6Func.GetDebitCreditBalanceFunc(NegaraID, SyarikatID, WilayahID, LadangID, UserID, DateTimeFunc.GetDateTime(), Month, Year, getservicesdetail.fld_SevicesActivity, getservicesdetail.fld_ServicesName, getservicesdetail.fld_ClientID, tblOptionConfigsWeb, out Log);
                                DataCount = DataCount + 1;
                                DataCount2 = DataCount2 + 1;
                                //Percentage = Math.Round((LoopCountData++ / CountData) * 100, 2); //commented by faeza 08.10.2021
                                Percentage = ((DataCount / TotalDataCount) * 19.5m) + 80; //added by faeza 08.10.2021
                                WriteLog("Total Loop Data Count. (Data - Total Data : " + DataCount2 + ")", false, ServiceName, ServiceProcessID);
                                Step1Func.UpdateServicesProcessPercFunc(SevicesProcess, Percentage, 1, db, TotalDataCount2, DataCount2);
                                WriteLog2(Log, ServiceName, ServiceProcessID);

                                //Added by kamalia 28/8/2021
                                Step7Func.AddTo_tbl_SAPPostRef(NegaraID, SyarikatID, WilayahID, LadangID, UserID, DateTimeFunc.GetDateTime(), Month, Year, getservicesdetail.fld_SevicesActivity, getservicesdetail.fld_ServicesName, getservicesdetail.fld_ClientID, out Log, 1, "SAP Data Creat.", Pkjmstlists, compCode, incentifsType.Where(x => x.fld_JenisInsentif == "T").ToList());
                                DataCount = DataCount + 1;
                                DataCount2 = DataCount2 + 1;
                                //Percentage = Math.Round((LoopCountData++ / CountData) * 100, 2); //commented by faeza 08.10.2021
                                Percentage = ((DataCount / TotalDataCount) * 19.5m) + 80; //added by faeza 08.10.2021
                                WriteLog("Total Loop Data Count. (Data - Total Data : " + DataCount2 + ")", false, ServiceName, ServiceProcessID);
                                Step1Func.UpdateServicesProcessPercFunc(SevicesProcess, Percentage, 1, db, TotalDataCount2, DataCount2);
                                WriteLog2(Log, ServiceName, ServiceProcessID);

                                Step6Func.Add_tbl_AuditTrail(NegaraID, SyarikatID, WilayahID, LadangID, Year);
                            }
                            else
                            {
                                WriteLog("No worker found)", false, ServiceName, ServiceProcessID);
                            }
                        }
                        else
                        {
                            if (Pkjmstlists != null)
                            {
                                DataCount = 1;
                                DataCount2 = DataCount2 + DataCount;
                                //TotalDataCount = 14; //commented by faeza 08.10.2021
                                TotalDataCount = 17; //modified by kamy 16/4/2022
                                WriteLog("Start to create Transaction Listing", false, ServiceName, ServiceProcessID);

                                ScTranRemoveCount = RemoveDataFunc.RemoveData_tbl_Sctran(NegaraID, SyarikatID, WilayahID, LadangID, UserID, DateTimeFunc.GetDateTime(), Month, Year, getservicesdetail.fld_SevicesActivity, getservicesdetail.fld_ServicesName, getservicesdetail.fld_ClientID);
                                //Percentage = Math.Round((LoopCountData++ / CountData) * 100, 2); //commented by faeza 08.10.2021
                                Percentage = ((DataCount / TotalDataCount) * 19.5m) + 80; //added by faeza 08.10.2021
                                WriteLog("Total Loop Data Count. (Data - Total Data : " + DataCount2 + ")", false, ServiceName, ServiceProcessID);
                                Step1Func.UpdateServicesProcessPercFunc(SevicesProcess, Percentage, 1, db, TotalDataCount2, DataCount2);
                                WriteLog("Removed Transaction Listing Data. (Data - Total Data Removed : " + ScTranRemoveCount + ")", false, ServiceName, ServiceProcessID);

                                WorkSCTransList = Step4Func.GetWorkActvtyPktFunc(NegaraID, SyarikatID, WilayahID, LadangID, UserID, DateTimeFunc.GetDateTime(), Month, Year, getservicesdetail.fld_SevicesActivity, getservicesdetail.fld_ServicesName, getservicesdetail.fld_ClientID, Pkjmstlists, compCode, tbl_UpahAktiviti);
                                DataCount = DataCount + 1;
                                DataCount2 = DataCount2 + 1;
                                //Percentage = Math.Round((LoopCountData++ / CountData) * 100, 2); //commented by faeza 08.10.2021
                                Percentage = ((DataCount / TotalDataCount) * 19.5m) + 80; //added by faeza 08.10.2021
                                WriteLog("Total Loop Data Count. (Data - Total Data : " + DataCount2 + ")", false, ServiceName, ServiceProcessID);
                                Step1Func.UpdateServicesProcessPercFunc(SevicesProcess, Percentage, 1, db, TotalDataCount2, DataCount2);
                                WriteLog("Get Work Data By Activity & Peringkat. (Data - Total Data : " + WorkSCTransList.Count + ")", false, ServiceName, ServiceProcessID);

                                //added by kamalia 16/2/22
                                AdminSCTransList = Step4Func.GetWorkAdminPktFunc(NegaraID, SyarikatID, WilayahID, LadangID, UserID, DateTimeFunc.GetDateTime(), Month, Year, getservicesdetail.fld_SevicesActivity, getservicesdetail.fld_ServicesName, getservicesdetail.fld_ClientID, Pkjmstlists, compCode, tbl_Kerja);
                                DataCount = DataCount + 1;
                                DataCount2 = DataCount2 + 1;
                                Percentage = ((DataCount / TotalDataCount) * 19.5m) + 80;
                                WriteLog("Total Loop Data Count. (Data - Total Data : " + DataCount2 + ")", false, ServiceName, ServiceProcessID);
                                Step1Func.UpdateServicesProcessPercFunc(SevicesProcess, Percentage, 1, db, TotalDataCount2, DataCount2);
                                WriteLog("Get Work Data By Activity & Peringkat. (Data - Total Data : " + AdminSCTransList.Count + ")", false, ServiceName, ServiceProcessID);

                                Step4Func.GetAmountWorkActivityFunc(NegaraID, SyarikatID, WilayahID, LadangID, UserID, DateTimeFunc.GetDateTime(), Month, Year, getservicesdetail.fld_SevicesActivity, getservicesdetail.fld_ServicesName, getservicesdetail.fld_ClientID, WorkSCTransList, out Log, Pkjmstlists);
                                DataCount = DataCount + 1;
                                DataCount2 = DataCount2 + 1;
                                //Percentage = Math.Round((LoopCountData++ / CountData) * 100, 2); //commented by faeza 08.10.2021
                                Percentage = ((DataCount / TotalDataCount) * 19.5m) + 80; //added by faeza 08.10.2021
                                WriteLog("Total Loop Data Count. (Data - Total Data : " + DataCount2 + ")", false, ServiceName, ServiceProcessID);
                                Step1Func.UpdateServicesProcessPercFunc(SevicesProcess, Percentage, 1, db, TotalDataCount2, DataCount2);
                                WriteLog2(Log, ServiceName, ServiceProcessID);

                                Step4Func.GetAmountDailyBonusFunc(NegaraID, SyarikatID, WilayahID, LadangID, UserID, DateTimeFunc.GetDateTime(), Month, Year, getservicesdetail.fld_SevicesActivity, getservicesdetail.fld_ServicesName, getservicesdetail.fld_ClientID, WorkSCTransList, out Log, Pkjmstlists);
                                DataCount = DataCount + 1;
                                DataCount2 = DataCount2 + 1;
                                //Percentage = Math.Round((LoopCountData++ / CountData) * 100, 2); //commented by faeza 08.10.2021
                                Percentage = ((DataCount / TotalDataCount) * 19.5m) + 80; //added by faeza 08.10.2021
                                WriteLog("Total Loop Data Count. (Data - Total Data : " + DataCount2 + ")", false, ServiceName, ServiceProcessID);
                                Step1Func.UpdateServicesProcessPercFunc(SevicesProcess, Percentage, 1, db, TotalDataCount2, DataCount2);
                                WriteLog2(Log, ServiceName, ServiceProcessID);

                                Step4Func.GetAmountOTFunc(NegaraID, SyarikatID, WilayahID, LadangID, UserID, DateTimeFunc.GetDateTime(), Month, Year, getservicesdetail.fld_SevicesActivity, getservicesdetail.fld_ServicesName, getservicesdetail.fld_ClientID, WorkSCTransList, out Log, Pkjmstlists, compCode, tbl_UpahAktiviti);
                                DataCount = DataCount + 1;
                                DataCount2 = DataCount2 + 1;
                                //Percentage = Math.Round((LoopCountData++ / CountData) * 100, 2); //commented by faeza 08.10.2021
                                Percentage = ((DataCount / TotalDataCount) * 19.5m) + 80; //added by faeza 08.10.2021
                                WriteLog("Total Loop Data Count. (Data - Total Data : " + DataCount2 + ")", false, ServiceName, ServiceProcessID);
                                Step1Func.UpdateServicesProcessPercFunc(SevicesProcess, Percentage, 1, db, TotalDataCount2, DataCount2);
                                WriteLog2(Log, ServiceName, ServiceProcessID);

                                Step4Func.GetAmountLeaveFunc(NegaraID, SyarikatID, WilayahID, LadangID, UserID, DateTimeFunc.GetDateTime(), Month, Year, getservicesdetail.fld_SevicesActivity, getservicesdetail.fld_ServicesName, getservicesdetail.fld_ClientID, AdminSCTransList, out Log, Pkjmstlists);//modified by kamalia 15/2/22
                                DataCount = DataCount + 1;
                                DataCount2 = DataCount2 + 1;
                                //Percentage = Math.Round((LoopCountData++ / CountData) * 100, 2); //commented by faeza 08.10.2021
                                Percentage = ((DataCount / TotalDataCount) * 19.5m) + 80; //added by faeza 08.10.2021
                                WriteLog("Total Loop Data Count. (Data - Total Data : " + DataCount2 + ")", false, ServiceName, ServiceProcessID);
                                Step1Func.UpdateServicesProcessPercFunc(SevicesProcess, Percentage, 1, db, TotalDataCount2, DataCount2);
                                WriteLog2(Log, ServiceName, ServiceProcessID);

                                Step4Func.GetAmountAddedInsentifFunc(NegaraID, SyarikatID, WilayahID, LadangID, UserID, DateTimeFunc.GetDateTime(), Month, Year, getservicesdetail.fld_SevicesActivity, getservicesdetail.fld_ServicesName, getservicesdetail.fld_ClientID, AdminSCTransList, out Log, Pkjmstlists);//modified by kamalia 15/2/22
                                DataCount = DataCount + 1;
                                DataCount2 = DataCount2 + 1;
                                //Percentage = Math.Round((LoopCountData++ / CountData) * 100, 2); //commented by faeza 08.10.2021
                                Percentage = ((DataCount / TotalDataCount) * 19.5m) + 80; //added by faeza 08.10.2021
                                WriteLog("Total Loop Data Count. (Data - Total Data : " + DataCount2 + ")", false, ServiceName, ServiceProcessID);
                                Step1Func.UpdateServicesProcessPercFunc(SevicesProcess, Percentage, 1, db, TotalDataCount2, DataCount2);
                                WriteLog2(Log, ServiceName, ServiceProcessID);

                                Step4Func.GetAmountDeductedInsentifFunc(NegaraID, SyarikatID, WilayahID, LadangID, UserID, DateTimeFunc.GetDateTime(), Month, Year, getservicesdetail.fld_SevicesActivity, getservicesdetail.fld_ServicesName, getservicesdetail.fld_ClientID, AdminSCTransList, out Log, Pkjmstlists, compCode);//modified by kamalia 15/2/22
                                DataCount = DataCount + 1;
                                DataCount2 = DataCount2 + 1;
                                //Percentage = Math.Round((LoopCountData++ / CountData) * 100, 2); //commented by faeza 08.10.2021
                                Percentage = ((DataCount / TotalDataCount) * 19.5m) + 80; //added by faeza 08.10.2021
                                WriteLog("Total Loop Data Count. (Data - Total Data : " + DataCount2 + ")", false, ServiceName, ServiceProcessID);
                                Step1Func.UpdateServicesProcessPercFunc(SevicesProcess, Percentage, 1, db, TotalDataCount2, DataCount2);
                                WriteLog2(Log, ServiceName, ServiceProcessID);

                                Step4Func.GetAmountKWSPFunc(NegaraID, SyarikatID, WilayahID, LadangID, UserID, DateTimeFunc.GetDateTime(), Month, Year, getservicesdetail.fld_SevicesActivity, getservicesdetail.fld_ServicesName, getservicesdetail.fld_ClientID, AdminSCTransList, out Log, Pkjmstlists);//modified by kamalia 15/2/22
                                DataCount = DataCount + 1;
                                DataCount2 = DataCount2 + 1;
                                //Percentage = Math.Round((LoopCountData++ / CountData) * 100, 2); //commented by faeza 08.10.2021
                                Percentage = ((DataCount / TotalDataCount) * 19.5m) + 80; //added by faeza 08.10.2021
                                WriteLog("Total Loop Data Count. (Data - Total Data : " + DataCount2 + ")", false, ServiceName, ServiceProcessID);
                                Step1Func.UpdateServicesProcessPercFunc(SevicesProcess, Percentage, 1, db, TotalDataCount2, DataCount2);
                                WriteLog2(Log, ServiceName, ServiceProcessID);

                                Step4Func.GetAmountSocsoFunc(NegaraID, SyarikatID, WilayahID, LadangID, UserID, DateTimeFunc.GetDateTime(), Month, Year, getservicesdetail.fld_SevicesActivity, getservicesdetail.fld_ServicesName, getservicesdetail.fld_ClientID, AdminSCTransList, out Log, Pkjmstlists);//modified by kamalia 15/2/22
                                DataCount = DataCount + 1;
                                DataCount2 = DataCount2 + 1;
                                //Percentage = Math.Round((LoopCountData++ / CountData) * 100, 2); //commented by faeza 08.10.2021
                                Percentage = ((DataCount / TotalDataCount) * 19.5m) + 80; //added by faeza 08.10.2021
                                WriteLog("Total Loop Data Count. (Data - Total Data : " + DataCount2 + ")", false, ServiceName, ServiceProcessID);
                                Step1Func.UpdateServicesProcessPercFunc(SevicesProcess, Percentage, 1, db, TotalDataCount2, DataCount2);
                                WriteLog2(Log, ServiceName, ServiceProcessID);

                                Step4Func.GetAmountOtherContributionsFunc(NegaraID, SyarikatID, WilayahID, LadangID, UserID, DateTimeFunc.GetDateTime(), Month, Year, getservicesdetail.fld_SevicesActivity, getservicesdetail.fld_ServicesName, getservicesdetail.fld_ClientID, AdminSCTransList, out Log, Pkjmstlists);//modified by kamalia 15/2/22
                                DataCount = DataCount + 1;
                                DataCount2 = DataCount2 + 1;
                                //Percentage = Math.Round((LoopCountData++ / CountData) * 100, 2); //commented by faeza 08.10.2021
                                Percentage = ((DataCount / TotalDataCount) * 19.5m) + 80; //added by faeza 08.10.2021
                                WriteLog("Total Loop Data Count. (Data - Total Data : " + DataCount2 + ")", false, ServiceName, ServiceProcessID);
                                Step1Func.UpdateServicesProcessPercFunc(SevicesProcess, Percentage, 1, db, TotalDataCount2, DataCount2);
                                WriteLog2(Log, ServiceName, ServiceProcessID);

                                Step4Func.GetAmountAIPSFunc(NegaraID, SyarikatID, WilayahID, LadangID, UserID, DateTimeFunc.GetDateTime(), Month, Year, getservicesdetail.fld_SevicesActivity, getservicesdetail.fld_ServicesName, getservicesdetail.fld_ClientID, AdminSCTransList, out Log, Pkjmstlists);//modified by kamalia 15/2/22
                                DataCount = DataCount + 1;
                                DataCount2 = DataCount2 + 1;
                                //Percentage = Math.Round((LoopCountData++ / CountData) * 100, 2); //commented by faeza 08.10.2021
                                Percentage = ((DataCount / TotalDataCount) * 19.5m) + 80; //added by faeza 08.10.2021
                                WriteLog("Total Loop Data Count. (Data - Total Data : " + DataCount2 + ")", false, ServiceName, ServiceProcessID);
                                Step1Func.UpdateServicesProcessPercFunc(SevicesProcess, Percentage, 1, db, TotalDataCount2, DataCount2);
                                WriteLog2(Log, ServiceName, ServiceProcessID);

                                Step4Func.GetAmountWorkerSalaryFunc(NegaraID, SyarikatID, WilayahID, LadangID, UserID, DateTimeFunc.GetDateTime(), Month, Year, getservicesdetail.fld_SevicesActivity, getservicesdetail.fld_ServicesName, getservicesdetail.fld_ClientID, out Log, Pkjmstlists, compCode);
                                DataCount = DataCount + 1;
                                DataCount2 = DataCount2 + 1;
                                //Percentage = Math.Round((LoopCountData++ / CountData) * 100, 2); //commented by faeza 08.10.2021
                                Percentage = ((DataCount / TotalDataCount) * 19.5m) + 80; //added by faeza 08.10.2021
                                WriteLog("Total Loop Data Count. (Data - Total Data : " + DataCount2 + ")", false, ServiceName, ServiceProcessID);
                                Step1Func.UpdateServicesProcessPercFunc(SevicesProcess, Percentage, 1, db, TotalDataCount2, DataCount2);
                                WriteLog2(Log, ServiceName, ServiceProcessID);

                                Step4Func.GetDebitCreditBalanceFunc(NegaraID, SyarikatID, WilayahID, LadangID, UserID, DateTimeFunc.GetDateTime(), Month, Year, getservicesdetail.fld_SevicesActivity, getservicesdetail.fld_ServicesName, getservicesdetail.fld_ClientID, out Log);
                                DataCount = DataCount + 1;
                                DataCount2 = DataCount2 + 1;
                                //Percentage = Math.Round((LoopCountData++ / CountData) * 100, 2); //commented by faeza 08.10.2021
                                Percentage = ((DataCount / TotalDataCount) * 19.5m) + 80; //added by faeza 08.10.2021
                                WriteLog("Total Loop Data Count. (Data - Total Data : " + DataCount2 + ")", false, ServiceName, ServiceProcessID);
                                Step1Func.UpdateServicesProcessPercFunc(SevicesProcess, Percentage, 1, db, TotalDataCount2, DataCount2);
                                WriteLog2(Log, ServiceName, ServiceProcessID);

                                //Added by kamalia 28/8/2021
                                Step5Func.AddTo_tbl_SAPPostRef(NegaraID, SyarikatID, WilayahID, LadangID, UserID, DateTimeFunc.GetDateTime(), Month, Year, getservicesdetail.fld_SevicesActivity, getservicesdetail.fld_ServicesName, getservicesdetail.fld_ClientID, out Log, 1, "SAP Data Creat.", Pkjmstlists, compCode);
                                DataCount = DataCount + 1;
                                DataCount2 = DataCount2 + 1;
                                //Percentage = Math.Round((LoopCountData++ / CountData) * 100, 2); //commented by faeza 08.10.2021
                                Percentage = ((DataCount / TotalDataCount) * 19.5m) + 80; //added by faeza 08.10.2021
                                WriteLog("Total Loop Data Count. (Data - Total Data : " + DataCount2 + ")", false, ServiceName, ServiceProcessID);
                                Step1Func.UpdateServicesProcessPercFunc(SevicesProcess, Percentage, 1, db, TotalDataCount2, DataCount2);
                                WriteLog2(Log, ServiceName, ServiceProcessID);

                                Step4Func.Add_tbl_AuditTrail(NegaraID, SyarikatID, WilayahID, LadangID, Year);
                            }
                            else
                            {
                                WriteLog("No worker found)", false, ServiceName, ServiceProcessID);
                            }
                        }
                    }
                    #endregion Normal Salary Gen

                    #region Special Incentive
                    else if (SevicesProcess.fld_ProcessName == "LadangSalaryGenBonus")
                    {
                        GetConnectFunc conn = new GetConnectFunc();
                        string host, catalog, user, pass = "";
                        conn.GetConnection(out host, out catalog, out user, out pass, WilayahID, SyarikatID, NegaraID);

                        GenSalaryModelEstate db2 = GenSalaryModelEstate.ConnectToSqlServer(host, catalog, user, pass);
                        var tbl_Pkjmast = db2.tbl_Pkjmast.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID).ToList();
                        var PkjmstlistsAll = Step1Func.GetAllWorkerFunc(NegaraID, SyarikatID, WilayahID, LadangID, UserID, DateTimeFunc.GetDateTime(), Month, Year, getservicesdetail.fld_SevicesActivity, getservicesdetail.fld_ServicesName, getservicesdetail.fld_ClientID, tbl_Pkjmast);
                        var tbl_Insentif = db2.tbl_Insentif.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Month == Month && x.fld_Year == Year && x.fld_Deleted == false).ToList();
                        var tbl_JenisInsentifSpecial = db.tbl_JenisInsentif.Where(x => x.fld_JenisInsentif == "P" && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_Deleted == false && x.fld_InclSecondPayslip == true && x.fld_ProcessDT >= DateTime.Today).ToList();
                        var tbl_Insentif2 = db2.tbl_Insentif.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Month == Month && x.fld_Year == Year && x.fld_Deleted == false).ToList();
                        var tbl_Kwsp = db.tbl_Kwsp.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID).ToList();
                        var tbl_Socso = db.tbl_Socso.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID).ToList();
                        var PkjmstlistsSpecialInsentif = Step1Func.GetWorkerSpecialInsentifFunc(NegaraID, SyarikatID, WilayahID, LadangID, UserID, DateTimeFunc.GetDateTime(), Month, Year, getservicesdetail.fld_SevicesActivity, getservicesdetail.fld_ServicesName, getservicesdetail.fld_ClientID, tbl_JenisInsentifSpecial, tbl_Insentif2);
                        int SpecialInsentifRemoveCount = 0;
                        List<tbl_SpecialInsentif> SpecialInsentifList = new List<tbl_SpecialInsentif>();
                        tbl_SpecialInsentif SpecialInsentif = new tbl_SpecialInsentif();

                        SpecialInsentifRemoveCount = RemoveDataFunc.RemoveData_tbl_SpecialInsentif(NegaraID, SyarikatID, WilayahID, LadangID, UserID, DateTimeFunc.GetDateTime(), Month, Year, getservicesdetail.fld_SevicesActivity, getservicesdetail.fld_ServicesName, getservicesdetail.fld_ClientID, PkjmstlistsAll, tbl_JenisInsentifSpecial);
                        WriteLog("Removed Incentive from tbl_SpecialInsentif. (Data - Total Data Removed : " + KerjaOTRemoveCount + ")", false, ServiceName, ServiceProcessID);


                        if (PkjmstlistsSpecialInsentif != null)
                        {
                            foreach (var Pkjmstlist in PkjmstlistsSpecialInsentif)
                            {
                                SpecialInsentifList = new List<tbl_SpecialInsentif>();
                                Step2Func.GetSpecialInsentifFunc(NegaraID, SyarikatID, WilayahID, LadangID, UserID, DateTimeFunc.GetDateTime(), Month, Year, getservicesdetail.fld_SevicesActivity, getservicesdetail.fld_ServicesName, getservicesdetail.fld_ClientID, Pkjmstlist.fld_Nopkj.Trim(), out SpecialInsentif, tbl_JenisInsentifSpecial, tbl_Insentif);

                                if (SpecialInsentif != null)
                                {
                                    WriteLog("Get SpecialInsentif Payment. (Data - No Pkj : " + Pkjmstlist.fld_Nopkj.Trim() + ", Payment : RM " + SpecialInsentif.fld_NilaiInsentif + ")", false, ServiceName, ServiceProcessID);
                                    SpecialInsentifList.Add(SpecialInsentif);
                                }
                                else
                                {
                                    WriteLog("No SpecialInsentif.", false, ServiceName, ServiceProcessID);
                                }

                                Step2Func.AddTo_tbl_SpecialInsentif(NegaraID, SyarikatID, WilayahID, LadangID, SpecialInsentifList);
                                WriteLog("Insert To tbl_SpecialInsentif. (Data - No Pkj : " + Pkjmstlist.fld_Nopkj.Trim() + ", Total Data : " + SpecialInsentifList.Count + ")", false, ServiceName, ServiceProcessID);
                            }

                            var workerSpecialInsentifs = Step2Func.GetSpecialInsentif(NegaraID, SyarikatID, WilayahID, LadangID, Month, Year);
                            var workerSpecialWorkerNo = PkjmstlistsSpecialInsentif.Select(s => s.fld_Nopkj).ToArray();
                            Step3Func.GetPkjMastsData(PkjmstlistsAll.Where(x => workerSpecialWorkerNo.Contains(x.fld_Nopkj)).ToList());
                            foreach (var item in PkjmstlistsSpecialInsentif)
                            {
                                var Pkjmstlist = PkjmstlistsAll.Where(x => x.fld_Nopkj == item.fld_Nopkj).FirstOrDefault();
                                var workerSpecialInsentif = workerSpecialInsentifs.Where(x => x.fld_Nopkj == Pkjmstlist.fld_Nopkj && x.fld_Month == Month && x.fld_KodInsentif == item.fld_KodInsentif).FirstOrDefault();
                                if (Pkjmstlist.fld_StatusKwspSocso == "1" && workerSpecialInsentif != null)
                                {
                                    if (tbl_JenisInsentifSpecial.Any(x => x.fld_AdaCaruman == true))
                                    {
                                        var CustMod_KWSP = Step3Func.GetKWSPForBonusFunc(NegaraID, SyarikatID, WilayahID, LadangID, UserID, DateTimeFunc.GetDateTime(), Month, Year, getservicesdetail.fld_SevicesActivity, getservicesdetail.fld_ServicesName, getservicesdetail.fld_ClientID, Pkjmstlist.fld_Nopkj.Trim(), Pkjmstlist.fld_KodKWSP, false, tbl_Kwsp, workerSpecialInsentif);
                                        KWSPMjkn = CustMod_KWSP.KWSPMjk;
                                        KWSPPkj = CustMod_KWSP.KWSPPkj;
                                        WriteLog("Get KWSP. (Data - No Pkj : " + Pkjmstlist.fld_Nopkj.Trim() + ", Employer : RM " + KWSPMjkn + ", Employee : RM " + KWSPPkj + ")", false, ServiceName, ServiceProcessID);

                                        var CustMod_Socso = Step3Func.GetSocsoForBonusFunc(NegaraID, SyarikatID, WilayahID, LadangID, UserID, DateTimeFunc.GetDateTime(), Month, Year, getservicesdetail.fld_SevicesActivity, getservicesdetail.fld_ServicesName, getservicesdetail.fld_ClientID, Pkjmstlist.fld_Nopkj.Trim(), Pkjmstlist.fld_KodSocso, false, tbl_Socso, workerSpecialInsentif);
                                        SocsoMjkn = CustMod_Socso.SocsoMjk;
                                        SocsoPkj = CustMod_Socso.SocsoPkj;
                                        WriteLog("Get Socso. (Data - No Pkj : " + Pkjmstlist.fld_Nopkj.Trim() + ", Employer : RM " + SocsoMjkn + ", Employee : RM " + SocsoPkj + ")", false, ServiceName, ServiceProcessID);
                                    }

                                }
                                if (workerSpecialInsentif != null)
                                {
                                    Step3Func.Update_SpecialInsentif(NegaraID, SyarikatID, WilayahID, LadangID, UserID, DateTimeFunc.GetDateTime(), Month, Year, getservicesdetail.fld_SevicesActivity, getservicesdetail.fld_ServicesName, getservicesdetail.fld_ClientID, Pkjmstlist.fld_Nopkj.Trim(), workerSpecialInsentifs.Where(x => x.fld_Nopkj == Pkjmstlist.fld_Nopkj).ToList(), item.fld_KodInsentif, KWSPPkj.Value, SocsoPkj.Value, workerSpecialInsentif);
                                }
                            }

                            //foreach (var item in PkjmstlistsSpecialInsentif)
                            //{
                            //    var Pkjmstlist = PkjmstlistsAll.Where(x => x.fld_Nopkj == item.fld_Nopkj).FirstOrDefault();
                            //    var workerSpecialInsentif = workerSpecialInsentifs.Where(x => x.fld_Nopkj == Pkjmstlist.fld_Nopkj && x.fld_Month == Month && x.fld_KodInsentif == item.fld_KodInsentif).FirstOrDefault();
                            //    KWSPPkj = 0;
                            //    SocsoPkj = 0;
                            //    if (Pkjmstlist.fld_StatusKwspSocso == "1" && workerSpecialInsentif != null)
                            //    {
                            //        var CustMod_KWSP = Step3Func.GetKWSPForBonusFunc(NegaraID, SyarikatID, WilayahID, LadangID, UserID, DateTimeFunc.GetDateTime(), Month, Year, getservicesdetail.fld_SevicesActivity, getservicesdetail.fld_ServicesName, getservicesdetail.fld_ClientID, Pkjmstlist.fld_Nopkj.Trim(), Pkjmstlist.fld_KodKWSP, false, tbl_Kwsp, workerSpecialInsentif);
                            //        KWSPMjkn = CustMod_KWSP.KWSPMjk;
                            //        KWSPPkj = CustMod_KWSP.KWSPPkj;
                            //        WriteLog("Get KWSP. (Data - No Pkj : " + Pkjmstlist.fld_Nopkj.Trim() + ", Employer : RM " + KWSPMjkn + ", Employee : RM " + KWSPPkj + ")", false, ServiceName, ServiceProcessID);

                            //        var CustMod_Socso = Step3Func.GetSocsoForBonusFunc(NegaraID, SyarikatID, WilayahID, LadangID, UserID, DateTimeFunc.GetDateTime(), Month, Year, getservicesdetail.fld_SevicesActivity, getservicesdetail.fld_ServicesName, getservicesdetail.fld_ClientID, Pkjmstlist.fld_Nopkj.Trim(), Pkjmstlist.fld_KodSocso, false, tbl_Socso, workerSpecialInsentif);
                            //        SocsoMjkn = CustMod_Socso.SocsoMjk;
                            //        SocsoPkj = CustMod_Socso.SocsoPkj;
                            //        WriteLog("Get Socso. (Data - No Pkj : " + Pkjmstlist.fld_Nopkj.Trim() + ", Employer : RM " + SocsoMjkn + ", Employee : RM " + SocsoPkj + ")", false, ServiceName, ServiceProcessID);
                            //    }

                            //    if (workerSpecialInsentif != null)
                            //    {
                            //        //var taxWorkerInfo = tbl_TaxWorkerInfo.Where(x => x.fld_NopkjPermanent == Pkjmstlist.fld_NopkjPermanent).FirstOrDefault();
                            //        Step3Func.Update_SpecialInsentif(NegaraID, SyarikatID, WilayahID, LadangID, UserID, DateTimeFunc.GetDateTime(), Month, Year, getservicesdetail.fld_SevicesActivity, getservicesdetail.fld_ServicesName, getservicesdetail.fld_ClientID, Pkjmstlist.fld_Nopkj.Trim(), workerSpecialInsentifs.Where(x => x.fld_Nopkj == Pkjmstlist.fld_Nopkj).ToList(), item.fld_KodInsentif, KWSPPkj.Value, SocsoPkj.Value, workerSpecialInsentif);
                            //    }
                            //}
                        }
                    }
                    #endregion Special Incentive

                    Percentage = 100; //added by faeza 08.10.2021
                }
                //Ashahri - 27/02/2022
                string hdrmsg = "Generate Completed!";
                string msg = "Overall data process: " + DataCount2 + "/" + TotalDataCount2;
                string status = "success";
                SendStatusToWeb(LadangID.Value, hdrmsg, msg, status);
                //Ashahri - 27/02/2022
            }
            catch (DbEntityValidationException ex)
            {
                var err = "";
                foreach (var validationErrors in ex.EntityValidationErrors)
                {
                    foreach (var validationError in validationErrors.ValidationErrors)
                    {
                        err += $"Property: {validationError.PropertyName} Error: {validationError.ErrorMessage}\n";

                    }
                }
                LogFunc.WriteErrorLog(err, ex.StackTrace, ex.Source, ex.TargetSite.ToString(), ServiceName, ServiceProcessID);
                string hdrmsg = "Generate Not Completed!";
                string msg = "Error on generate:<b/>";
                msg += "Error on - " + logMessage + "<b/>";
                msg += "Error Trace: " + err;
                string status = "warning";
                SendStatusToWeb(LadangID.Value, hdrmsg, msg, status);
            }
            catch (Exception ex)
            {
                LogFunc.WriteErrorLog(ex.Message, ex.StackTrace, ex.Source, ex.TargetSite.ToString(), ServiceName, ServiceProcessID);
                //Ashahri - 27/02/2022
                string hdrmsg = "Generate Not Completed!";
                string msg = "Error on generate:<b/>";
                msg += "Error on - " + logMessage + "<b/>";
                msg += "Error Trace: " + ex.Message;
                string status = "warning";
                SendStatusToWeb(LadangID.Value, hdrmsg, msg, status);
                //Ashahri - 27/02/2022
            }
            finally
            {
                if (SevicesProcess != null)
                {
                    Step1Func.UpdateServicesProcessPercFunc(SevicesProcess, Percentage, 0, db, TotalDataCount2, DataCount2);
                }
                WriteLog("End Process.", false, ServiceName, ServiceProcessID);
                OnStop();
            }
        }

        //Ashahri - 27/02/2022
        public void SendStatusToWeb(int estateID, string hdrmsg, string msg, string status)
        {
            var url = ConfigReader2("commonconfig", "appurl");
            var connection = new HubConnection(url);
            var myHub = connection.CreateHubProxy("GenerateSalaryHub");
            connection.Start().Wait();
            myHub.Invoke("GetGenEnd", estateID, hdrmsg, msg, status).Wait();
            connection.Dispose();
        }
        //Ashahri - 27/02/2022

        private string ConfigReader2(string Name, string Data)
        {
            string getresult = "";
            INIReaderFunc parser = new INIReaderFunc(AppDomain.CurrentDomain.BaseDirectory + "CommonConfigs.ini");

            getresult = parser.GetSetting(Name, Data);

            return getresult;
        }

        public void WriteLog(string message, bool startprocess, string ServicesName, long ServiceProcessID)
        {
            string msg = "";
            logMessage = message; //Ashahri - 27/02/2022
            if (startprocess)
            {
                msg += DateTimeFunc.GetDateTime() + " - " + message;
            }
            else
            {
                msg += DateTimeFunc.GetDateTime() + " - " + message;
            }
            LogFunc.WriteProcessLog(msg, ServicesName, ServiceProcessID);
        }

        public void WriteLog2(string message, string ServicesName, long ServiceProcessID)
        {
            string msg = "";
            if (message != "")
            {
                msg += message;
                LogFunc.WriteProcessLog(msg, ServicesName, ServiceProcessID);
            }
        }

        private string ConfigReader(string Name, string Data)
        {
            string getresult = "";
            INIReaderFunc parser = new INIReaderFunc(AppDomain.CurrentDomain.BaseDirectory + "Configs.ini");

            getresult = parser.GetSetting(Name, Data);

            return getresult;
        }

        public void OnDebug()
        {
            OnStart(null);
        }

        protected override void OnStart(string[] args)
        {
            DoProcess();
        }

        protected override void OnStop()
        {
            Stop();
        }
    }
}
