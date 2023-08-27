using Microsoft.AspNet.SignalR.Client; //Ashahri - 27/02/2022
using SalaryGeneratorServices.CustomModels;
using SalaryGeneratorServices.FuncClass;
using SalaryGeneratorServices.ModelsEstate;
using SalaryGeneratorServices.ModelsHQ;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
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
            tbl_KerjaOT KerjaOT = new tbl_KerjaOT();
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

                    TotalPkjCount = Pkjmstlists.Count;
                    WriteLog("Total Pkj Count. (Data - Total Data : " + TotalPkjCount + ")", false, ServiceName, ServiceProcessID);
                    DateLists = Step1Func.GetDateListFunc(Month, Year);
                    TotalDateCount = DateLists.Count;
                    WriteLog("Total Date Count. (Data - Total Data : " + TotalDateCount + ")", false, ServiceName, ServiceProcessID);
                    TotalDataCount = TotalPkjCount * TotalDateCount;
                    TotalDataCount2 = TotalDataCount + 17; //modified by kamalia 26/4/2022
                    WriteLog("Total Data Count. (Data - Total Data : " + TotalDataCount2 + ")", false, ServiceName, ServiceProcessID);
                    var vw_KerjaInfoDetails = Step2Func.vw_KerjaInfoDetails(NegaraID,SyarikatID,WilayahID,LadangID, Month, Year);
                    var vw_Kerja_Bonus = Step2Func.vw_Kerja_Bonus(NegaraID, SyarikatID, WilayahID, LadangID, Month, Year);

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
                        StartWorkDate = Step1Func.GetDateStarkWorkingFunc(NegaraID, SyarikatID, WilayahID, LadangID, Pkjmstlist.fld_Nopkj.Trim());
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
                                TakePaidLeave = Step2Func.GetWorkingPaidLeaveFunc(NegaraID, SyarikatID, WilayahID, LadangID, UserID, DateTimeFunc.GetDateTime(), Month, Year, getservicesdetail.fld_SevicesActivity, getservicesdetail.fld_ServicesName, getservicesdetail.fld_ClientID, Pkjmstlist.fld_Nopkj.Trim(), DateList.Date, CutiKategoriList, out PaidPeriod, out WorkingAtt, out KumCode);
                                WriteLog("Get Paid Leave. (Data - No Pkj : " + Pkjmstlist.fld_Nopkj.Trim() + ", Paid Leave : " + TakePaidLeave + ")", false, ServiceName, ServiceProcessID);
                                if (TakePaidLeave)
                                {
                                    WriteLog("Code Paid Leave. (Data - No Pkj : " + Pkjmstlist.fld_Nopkj.Trim() + ", Leave Code : " + WorkingAtt.fld_Kdhdct + ")", false, ServiceName, ServiceProcessID);

                                    WorkerPaidLeaveLists.Add(new CustMod_WorkerPaidLeave() { fld_Nopkj = Pkjmstlist.fld_Nopkj.Trim(), fld_Kdhdct = WorkingAtt.fld_Kdhdct, fld_Tarikh = DateList.Date, fld_PaidPeriod = PaidPeriod, fld_KerjahdrID = WorkingAtt.fld_UniqueID, fld_Kum = KumCode });
                                }
                                else
                                {
                                    AttendStatus = Step2Func.GetAttendStatusFunc(NegaraID, SyarikatID, WilayahID, LadangID, UserID, DateTimeFunc.GetDateTime(), Month, Year, getservicesdetail.fld_SevicesActivity, getservicesdetail.fld_ServicesName, getservicesdetail.fld_ClientID, Pkjmstlist.fld_Nopkj.Trim(), DateList.Date, out AttCode);
                                    if (AttendStatus)
                                    {
                                        Step2Func.GetDailyBonusFunc(NegaraID, SyarikatID, WilayahID, LadangID, UserID, DateTimeFunc.GetDateTime(), Month, Year, getservicesdetail.fld_SevicesActivity, getservicesdetail.fld_ServicesName, getservicesdetail.fld_ClientID, Pkjmstlist.fld_Nopkj.Trim(), DateList.Date, out KerjaBonus, JenisAktiviti, bonusHarian);
                                        if (KerjaBonus != null)
                                        {
                                            WriteLog("Get Daily Bonus. (Data - No Pkj : " + Pkjmstlist.fld_Nopkj.Trim() + ", Bonus Price : RM " + KerjaBonus.fld_Jumlah + ")", false, ServiceName, ServiceProcessID);
                                            KerjaBonusList.Add(KerjaBonus);
                                        }
                                        else
                                        {
                                            WriteLog("No Daily Bonus.", false, ServiceName, ServiceProcessID);
                                        }

                                        Step2Func.GetOTFunc(NegaraID, SyarikatID, WilayahID, LadangID, UserID, DateTimeFunc.GetDateTime(), Month, Year, getservicesdetail.fld_SevicesActivity, getservicesdetail.fld_ServicesName, getservicesdetail.fld_ClientID, Pkjmstlist.fld_Nopkj.Trim(), DateList.Date, out KerjaOT, AttCode, JenisAktiviti);

                                        if (KerjaOT != null)
                                        {
                                            WriteLog("Get Daily OT. (Data - No Pkj : " + Pkjmstlist.fld_Nopkj.Trim() + ", OT Price : RM " + KerjaOT.fld_Jumlah + ")", false, ServiceName, ServiceProcessID);
                                            KerjaOTList.Add(KerjaOT);
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
                                    MonthSalaryID = Step3Func.GetPaidWorkingFunc(NegaraID, SyarikatID, WilayahID, LadangID, UserID, DateTimeFunc.GetDateTime(), Month, Year, getservicesdetail.fld_SevicesActivity, getservicesdetail.fld_ServicesName, getservicesdetail.fld_ClientID, Pkjmstlist.fld_Nopkj.Trim(), out WorkingPayment, out DiffAreaPayment);
                                    WriteLog("Get Daily Work Payment. (Data - No Pkj : " + Pkjmstlist.fld_Nopkj.Trim() + ", Total Payment : RM " + WorkingPayment + ")", false, ServiceName, ServiceProcessID);
                                    AveragePayment = Step3Func.GetAveragePaidFunc(NegaraID, SyarikatID, WilayahID, LadangID, UserID, DateTimeFunc.GetDateTime(), Month, Year, getservicesdetail.fld_SevicesActivity, getservicesdetail.fld_ServicesName, getservicesdetail.fld_ClientID, Pkjmstlist.fld_Nopkj.Trim(), MonthSalaryID);
                                    WriteLog("Get Average Payment. (Data - No Pkj : " + Pkjmstlist.fld_Nopkj.Trim() + ", Total Payment : RM " + AveragePayment + ")", false, ServiceName, ServiceProcessID);
                                    DailyBonusPayment = Step3Func.GetPaidDailyBonusFunc(NegaraID, SyarikatID, WilayahID, LadangID, UserID, DateTimeFunc.GetDateTime(), Month, Year, getservicesdetail.fld_SevicesActivity, getservicesdetail.fld_ServicesName, getservicesdetail.fld_ClientID, Pkjmstlist.fld_Nopkj.Trim(), MonthSalaryID);
                                    WriteLog("Get Daily Bonus Payment. (Data - No Pkj : " + Pkjmstlist.fld_Nopkj.Trim() + ", Total Payment : RM " + DailyBonusPayment + ")", false, ServiceName, ServiceProcessID);
                                    OTPayment = Step3Func.GetPaidOTFunc(NegaraID, SyarikatID, WilayahID, LadangID, UserID, DateTimeFunc.GetDateTime(), Month, Year, getservicesdetail.fld_SevicesActivity, getservicesdetail.fld_ServicesName, getservicesdetail.fld_ClientID, Pkjmstlist.fld_Nopkj.Trim(), MonthSalaryID);
                                    WriteLog("Get OT Payment. (Data - No Pkj : " + Pkjmstlist.fld_Nopkj.Trim() + ", Total Payment : RM " + OTPayment + ")", false, ServiceName, ServiceProcessID);
                                    var workerIncentifs = new List<tbl_Insentif>();
                                    OthrInsPayment = Step3Func.GetPaidInsentifFunc(NegaraID, SyarikatID, WilayahID, LadangID, UserID, DateTimeFunc.GetDateTime(), Month, Year, getservicesdetail.fld_SevicesActivity, getservicesdetail.fld_ServicesName, getservicesdetail.fld_ClientID, Pkjmstlist.fld_Nopkj.Trim(), MonthSalaryID, out workerIncentifs);
                                    WriteLog("Get Other Insentif Payment. (Data - No Pkj : " + Pkjmstlist.fld_Nopkj.Trim() + ", Total Payment : RM " + OthrInsPayment + ")", false, ServiceName, ServiceProcessID);
                                    DeductInsPayment = Step3Func.GetDeductInsentifFunc(NegaraID, SyarikatID, WilayahID, LadangID, UserID, DateTimeFunc.GetDateTime(), Month, Year, getservicesdetail.fld_SevicesActivity, getservicesdetail.fld_ServicesName, getservicesdetail.fld_ClientID, Pkjmstlist.fld_Nopkj.Trim(), MonthSalaryID);
                                    WriteLog("Get Insentif Deduction. (Data - No Pkj : " + Pkjmstlist.fld_Nopkj.Trim() + ", Total Deduction : RM " + DeductInsPayment + ")", false, ServiceName, ServiceProcessID);
                                    AIPSPayment = Step3Func.GetAIPSFunc(NegaraID, SyarikatID, WilayahID, LadangID, UserID, DateTimeFunc.GetDateTime(), Month, Year, getservicesdetail.fld_SevicesActivity, getservicesdetail.fld_ServicesName, getservicesdetail.fld_ClientID, Pkjmstlist.fld_Nopkj.Trim(), MonthSalaryID);
                                    WriteLog("Get AIPS Insentif. (Data - No Pkj : " + Pkjmstlist.fld_Nopkj.Trim() + ", Total Payment : RM " + AIPSPayment + ")", false, ServiceName, ServiceProcessID);
                                    if (WorkerPaidLeaveLists.Count > 0)
                                    {
                                        //modified by kamalia 1/6/2021
                                        LeavePayment = Step3Func.GetPaidLeaveFunc(NegaraID, SyarikatID, WilayahID, LadangID, UserID, DateTimeFunc.GetDateTime(), Month, Year, getservicesdetail.fld_SevicesActivity, getservicesdetail.fld_ServicesName, getservicesdetail.fld_ClientID, Pkjmstlist.fld_Nopkj.Trim(), MonthSalaryID, WorkerPaidLeaveLists, StartWorkDate, false, CutiKategoriList, Pkjmstlist);
                                        WriteLog("Get Leave Payment (Have Count Leave). (Data - No Pkj : " + Pkjmstlist.fld_Nopkj.Trim() + ", Total Payment : RM " + LeavePayment + ")", false, ServiceName, ServiceProcessID);
                                    }
                                    else
                                    {
                                        //modified by kamalia 1/6/2021
                                        LeavePayment = Step3Func.GetPaidLeaveFunc(NegaraID, SyarikatID, WilayahID, LadangID, UserID, DateTimeFunc.GetDateTime(), Month, Year, getservicesdetail.fld_SevicesActivity, getservicesdetail.fld_ServicesName, getservicesdetail.fld_ClientID, Pkjmstlist.fld_Nopkj.Trim(), MonthSalaryID, WorkerPaidLeaveLists, StartWorkDate, true, CutiKategoriList, Pkjmstlist);
                                        WriteLog("Get Leave Payment (No Count Leave). (Data - No Pkj : " + Pkjmstlist.fld_Nopkj.Trim() + ", Total Payment : RM " + LeavePayment + ")", false, ServiceName, ServiceProcessID);
                                    }

                                    if (Pkjmstlist.fld_StatusKwspSocso == "1")
                                    {
                                        Step3Func.GetKWSPFunc(NegaraID, SyarikatID, WilayahID, LadangID, UserID, DateTimeFunc.GetDateTime(), Month, Year, getservicesdetail.fld_SevicesActivity, getservicesdetail.fld_ServicesName, getservicesdetail.fld_ClientID, Pkjmstlist.fld_Nopkj.Trim(), MonthSalaryID, Pkjmstlist.fld_KodKWSP, out KWSPMjkn, out KWSPPkj, false);
                                        WriteLog("Get KWSP. (Data - No Pkj : " + Pkjmstlist.fld_Nopkj.Trim() + ", Employer : RM " + KWSPMjkn + ", Employee : RM " + KWSPPkj + ")", false, ServiceName, ServiceProcessID);
                                    }
                                    else
                                    {
                                        Step3Func.GetKWSPFunc(NegaraID, SyarikatID, WilayahID, LadangID, UserID, DateTimeFunc.GetDateTime(), Month, Year, getservicesdetail.fld_SevicesActivity, getservicesdetail.fld_ServicesName, getservicesdetail.fld_ClientID, Pkjmstlist.fld_Nopkj.Trim(), MonthSalaryID, Pkjmstlist.fld_KodKWSP, out KWSPMjkn, out KWSPPkj, true);
                                        WriteLog("No KWSP. (Data - No Pkj : " + Pkjmstlist.fld_Nopkj.Trim() + ")", false, ServiceName, ServiceProcessID);
                                    }

                                    if (Pkjmstlist.fld_StatusKwspSocso == "1")
                                    {
                                        Step3Func.GetSocsoFunc(NegaraID, SyarikatID, WilayahID, LadangID, UserID, DateTimeFunc.GetDateTime(), Month, Year, getservicesdetail.fld_SevicesActivity, getservicesdetail.fld_ServicesName, getservicesdetail.fld_ClientID, Pkjmstlist.fld_Nopkj.Trim(), MonthSalaryID, Pkjmstlist.fld_KodSocso, out SocsoMjkn, out SocsoPkj, false);
                                        WriteLog("Get Socso. (Data - No Pkj : " + Pkjmstlist.fld_Nopkj.Trim() + ", Employer : RM " + SocsoMjkn + ", Employee : RM " + SocsoPkj + ")", false, ServiceName, ServiceProcessID);
                                    }
                                    else
                                    {
                                        Step3Func.GetSocsoFunc(NegaraID, SyarikatID, WilayahID, LadangID, UserID, DateTimeFunc.GetDateTime(), Month, Year, getservicesdetail.fld_SevicesActivity, getservicesdetail.fld_ServicesName, getservicesdetail.fld_ClientID, Pkjmstlist.fld_Nopkj.Trim(), MonthSalaryID, Pkjmstlist.fld_KodSocso, out SocsoMjkn, out SocsoPkj, true);
                                        WriteLog("No Socso. (Data - No Pkj : " + Pkjmstlist.fld_Nopkj.Trim() + ")", false, ServiceName, ServiceProcessID);
                                    }

                                    //added by faeza 12.11.2021
                                    Step3Func.UpdatePaymentMode(NegaraID, SyarikatID, WilayahID, LadangID, UserID, DateTimeFunc.GetDateTime(), Month, Year, getservicesdetail.fld_SevicesActivity, getservicesdetail.fld_ServicesName, getservicesdetail.fld_ClientID, Pkjmstlist.fld_Nopkj.Trim(), Pkjmstlist.fld_PaymentMode, MonthSalaryID);
                                    WriteLog("Update Payment Mode. (Data - No Pkj : " + Pkjmstlist.fld_Nopkj.Trim() + ", Payment Mode : " + Pkjmstlist.fld_PaymentMode + ")", false, ServiceName, ServiceProcessID);
                                    
                                    var oRP = Step3Func.GetORPFunc(NegaraID, SyarikatID, WilayahID, LadangID, UserID, DateTimeFunc.GetDateTime(), Month, Year, getservicesdetail.fld_SevicesActivity, getservicesdetail.fld_ServicesName, getservicesdetail.fld_ClientID, Pkjmstlist.fld_Nopkj.Trim(), MonthSalaryID, workerIncentifs, incentifsType, vw_KerjaInfoDetails, vw_Kerja_Bonus);
                                    WriteLog("Get ORP. (Data - No Pkj : " + Pkjmstlist.fld_Nopkj.Trim() + ", Total Payment : RM " + oRP + ")", false, ServiceName, ServiceProcessID);

                                    Step3Func.GetOtherContributionsFunc(NegaraID, SyarikatID, WilayahID, LadangID, UserID, DateTimeFunc.GetDateTime(), Month, Year, getservicesdetail.fld_SevicesActivity, getservicesdetail.fld_ServicesName, getservicesdetail.fld_ClientID, Pkjmstlist.fld_Nopkj.Trim(), MonthSalaryID, out TotalOthrContMjkCont, out TotalOthrContPkjCont);
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

                            WorkSCTransList = Step6Func.GetWorkActvtyPktFunc(NegaraID, SyarikatID, WilayahID, LadangID, UserID, DateTimeFunc.GetDateTime(), Month, Year, getservicesdetail.fld_SevicesActivity, getservicesdetail.fld_ServicesName, getservicesdetail.fld_ClientID, Pkjmstlists, compCode);
                            DataCount = DataCount + 1;
                            DataCount2 = DataCount2 + 1;
                            //Percentage = Math.Round((LoopCountData++ / CountData) * 100, 2); //commented by faeza 08.10.2021
                            Percentage = ((DataCount / TotalDataCount) * 19.5m) + 80; //added by faeza 08.10.2021
                            WriteLog("Total Loop Data Count. (Data - Total Data : " + DataCount2 + ")", false, ServiceName, ServiceProcessID);
                            Step1Func.UpdateServicesProcessPercFunc(SevicesProcess, Percentage, 1, db, TotalDataCount2, DataCount2);
                            WriteLog("Get Work Data By Activity & Peringkat. (Data - Total Data : " + WorkSCTransList.Count + ")", false, ServiceName, ServiceProcessID);

                            //added by kamalia 16/2/22
                            AdminSCTransList = Step6Func.GetWorkAdminPktFunc(NegaraID, SyarikatID, WilayahID, LadangID, UserID, DateTimeFunc.GetDateTime(), Month, Year, getservicesdetail.fld_SevicesActivity, getservicesdetail.fld_ServicesName, getservicesdetail.fld_ClientID, Pkjmstlists, compCode);
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

                            Step6Func.GetAmountOTFunc(NegaraID, SyarikatID, WilayahID, LadangID, UserID, DateTimeFunc.GetDateTime(), Month, Year, getservicesdetail.fld_SevicesActivity, getservicesdetail.fld_ServicesName, getservicesdetail.fld_ClientID, WorkSCTransList, out Log, Pkjmstlists, compCode);
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

                            Step6Func.GetDebitCreditBalanceFunc(NegaraID, SyarikatID, WilayahID, LadangID, UserID, DateTimeFunc.GetDateTime(), Month, Year, getservicesdetail.fld_SevicesActivity, getservicesdetail.fld_ServicesName, getservicesdetail.fld_ClientID, out Log);
                            DataCount = DataCount + 1;
                            DataCount2 = DataCount2 + 1;
                            //Percentage = Math.Round((LoopCountData++ / CountData) * 100, 2); //commented by faeza 08.10.2021
                            Percentage = ((DataCount / TotalDataCount) * 19.5m) + 80; //added by faeza 08.10.2021
                            WriteLog("Total Loop Data Count. (Data - Total Data : " + DataCount2 + ")", false, ServiceName, ServiceProcessID);
                            Step1Func.UpdateServicesProcessPercFunc(SevicesProcess, Percentage, 1, db, TotalDataCount2, DataCount2);
                            WriteLog2(Log, ServiceName, ServiceProcessID);

                            //Added by kamalia 28/8/2021
                            Step7Func.AddTo_tbl_SAPPostRef(NegaraID, SyarikatID, WilayahID, LadangID, UserID, DateTimeFunc.GetDateTime(), Month, Year, getservicesdetail.fld_SevicesActivity, getservicesdetail.fld_ServicesName, getservicesdetail.fld_ClientID, out Log, 1, "SAP Data Creat.", Pkjmstlists, compCode);
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

                            WorkSCTransList = Step4Func.GetWorkActvtyPktFunc(NegaraID, SyarikatID, WilayahID, LadangID, UserID, DateTimeFunc.GetDateTime(), Month, Year, getservicesdetail.fld_SevicesActivity, getservicesdetail.fld_ServicesName, getservicesdetail.fld_ClientID, Pkjmstlists, compCode);
                            DataCount = DataCount + 1;
                            DataCount2 = DataCount2 + 1;
                            //Percentage = Math.Round((LoopCountData++ / CountData) * 100, 2); //commented by faeza 08.10.2021
                            Percentage = ((DataCount / TotalDataCount) * 19.5m) + 80; //added by faeza 08.10.2021
                            WriteLog("Total Loop Data Count. (Data - Total Data : " + DataCount2 + ")", false, ServiceName, ServiceProcessID);
                            Step1Func.UpdateServicesProcessPercFunc(SevicesProcess, Percentage, 1, db, TotalDataCount2, DataCount2);
                            WriteLog("Get Work Data By Activity & Peringkat. (Data - Total Data : " + WorkSCTransList.Count + ")", false, ServiceName, ServiceProcessID);

                            //added by kamalia 16/2/22
                            AdminSCTransList = Step4Func.GetWorkAdminPktFunc(NegaraID, SyarikatID, WilayahID, LadangID, UserID, DateTimeFunc.GetDateTime(), Month, Year, getservicesdetail.fld_SevicesActivity, getservicesdetail.fld_ServicesName, getservicesdetail.fld_ClientID, Pkjmstlists, compCode);
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

                            Step4Func.GetAmountOTFunc(NegaraID, SyarikatID, WilayahID, LadangID, UserID, DateTimeFunc.GetDateTime(), Month, Year, getservicesdetail.fld_SevicesActivity, getservicesdetail.fld_ServicesName, getservicesdetail.fld_ClientID, WorkSCTransList, out Log, Pkjmstlists, compCode);
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
                    
                    Percentage = 100; //added by faeza 08.10.2021
                }
                //Ashahri - 27/02/2022
                string hdrmsg = "Generate Completed!";
                string msg = "Overall data process: " + DataCount2 + "/" + TotalDataCount2;
                string status = "success";
                SendStatusToWeb(LadangID.Value, hdrmsg, msg, status);
                //Ashahri - 27/02/2022
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
