using SalaryGeneratorServices.ModelsEstate;
using SalaryGeneratorServices.ModelsHQ;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace SalaryGeneratorServices.FuncClass
{
    public class Step5Func
    {
        private DateTimeFunc DateTimeFunc = new DateTimeFunc();

        public void AddTo_tbl_SAPPostRef(int? NegaraID, int? SyarikatID, int? WilayahID, int? LadangID, int? UserID, DateTime DTProcess, int? Month, int? Year, string processname, string servicesname, int? ClientID, out string Log, short Purpose, string PurMsg, List<tbl_Pkjmast> Pkjmstlists, string compCode)
        {
            GenSalaryModelHQ db = new GenSalaryModelHQ();
            GetConnectFunc conn = new GetConnectFunc();
            RemoveDataFunc RemoveDataFunc = new RemoveDataFunc();
            DateTime DateNow = DateTimeFunc.GetDateTime();
            string host, catalog, user, pass = "";
            conn.GetConnection(out host, out catalog, out user, out pass, WilayahID, SyarikatID, NegaraID);
            GenSalaryModelEstate db2 = GenSalaryModelEstate.ConnectToSqlServer(host, catalog, user, pass);
            Log = "";
            string message = "";
            int i = 1; // Modified by kamalia 18/11/21
            string GLGajiKawalan = "";
            decimal? GajiKawalan = 0;
            string GLKeteranganGajiKawalan = "";

            bool Contribution = false;
            bool DeductionStatus = false;
            decimal? BalAmountDeduct = 0;

            Guid SAPPostID = Guid.NewGuid();
            Guid SAPPostID2 = Guid.NewGuid();

            Guid SAPPostID3 = Guid.NewGuid();
            tbl_SAPPostRef tbl_SAPPostRef = new tbl_SAPPostRef();
            tbl_SAPPostRef tbl_SAPPostRef2 = new tbl_SAPPostRef();

            string CompanyCode = "";
            DateTime LastDayMonthPosting = GetLastDayOfMonth(Month, Year);


            var GetGLSctran = db2.tbl_Sctran.Where(x => x.fld_Month == Month && x.fld_Year == Year && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID).ToList();
            if (GetGLSctran.Count > 0)
            {
                //  var NSWL = db.vw_NSWL.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID).FirstOrDefault(); //commented by kamalia 21/10/21
                var Compcode = db.tbl_Ladang.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WlyhID == WilayahID && x.fld_ID == LadangID).FirstOrDefault(); // added by kamalia 21 / 10 / 21
                var SapDocType = db.tblOptionConfigsWebs.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fldOptConfFlag1 == "purposeCode" && x.fldDeleted == false).ToList();
                var CheckStatusProceed = db2.tbl_SAPPostRef.Where(x => x.fld_Month == Month && x.fld_Year == Year && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID).ToList();
                var CheckStatusProceed1 = CheckStatusProceed.Where(x => x.fld_Purpose == 1).FirstOrDefault();
                if (CheckStatusProceed1 != null)
                {
                    SAPPostID = CheckStatusProceed1.fld_ID;

                    if (CheckStatusProceed1.fld_StatusProceed == false)
                    {
                        RemoveDataFunc.RemoveData_tbl_SAPPostingDetail(db2, CheckStatusProceed1.fld_ID);

                        CheckStatusProceed1.fld_DocDate = LastDayMonthPosting;
                        CheckStatusProceed1.fld_PostingDate = LastDayMonthPosting;
                        CheckStatusProceed1.fld_ModifiedDT = DateNow;
                        CheckStatusProceed1.fld_ModifiedBy = UserID;
                        db2.Entry(CheckStatusProceed1).State = EntityState.Modified;
                        db2.SaveChanges();
                        Add_To_tbl_SAPPostRef1(db, db2, Month, Year, NegaraID, SyarikatID, WilayahID, LadangID, SAPPostID, out GajiKawalan, out GLGajiKawalan, out GLKeteranganGajiKawalan, out Contribution, Pkjmstlists, compCode);

                    }
                }
                else
                {
                    CompanyCode = Compcode.fld_CostCentre; //commented by kamalia 21/10/21
                    var SAPDocType1 = SapDocType.Where(x => x.fldOptConfValue == "1").FirstOrDefault();
                    tbl_SAPPostRef.fld_ID = Guid.NewGuid();
                    tbl_SAPPostRef.fld_Month = Month;
                    tbl_SAPPostRef.fld_Year = Year;
                    tbl_SAPPostRef.fld_CompCode = CompanyCode;
                    tbl_SAPPostRef.fld_DocType = SAPDocType1.fldOptConfFlag2;
                    tbl_SAPPostRef.fld_DocDate = LastDayMonthPosting;
                    tbl_SAPPostRef.fld_PostingDate = LastDayMonthPosting;

                    tbl_SAPPostRef.fld_NegaraID = NegaraID;
                    tbl_SAPPostRef.fld_SyarikatID = SyarikatID;
                    tbl_SAPPostRef.fld_WilayahID = WilayahID;
                    tbl_SAPPostRef.fld_LadangID = LadangID;
                    tbl_SAPPostRef.fld_StatusProceed = false;
                    tbl_SAPPostRef.fld_CreatedDT = DateNow;
                    tbl_SAPPostRef.fld_CreatedBy = UserID;
                    tbl_SAPPostRef.fld_Purpose = Purpose;

                    db2.tbl_SAPPostRef.Add(tbl_SAPPostRef);
                    db2.SaveChanges();

                    SAPPostID = tbl_SAPPostRef.fld_ID;

                    Add_To_tbl_SAPPostRef1(db, db2, Month, Year, NegaraID, SyarikatID, WilayahID, LadangID, SAPPostID, out GajiKawalan, out GLGajiKawalan, out GLKeteranganGajiKawalan, out Contribution, Pkjmstlists, compCode);

                }

                message = PurMsg + " (Created SAP Data Step 1)";
                Log += i == 1 ? DateTimeFunc.GetDateTime() + " - " + message : "\r\n" + DateTimeFunc.GetDateTime() + " - " + message;

                //if (Contribution)
                //{
                if (string.IsNullOrEmpty(GLKeteranganGajiKawalan))
                {
                    GetBackRequiredData(db, db2, Month, Year, NegaraID, SyarikatID, WilayahID, LadangID, out GajiKawalan, out GLGajiKawalan, out GLKeteranganGajiKawalan, compCode);
                }

                var CheckStatusProceed2 = CheckStatusProceed.Where(x => x.fld_Purpose == 2).FirstOrDefault();

                if (CheckStatusProceed2 != null)
                {
                    SAPPostID2 = CheckStatusProceed2.fld_ID;
                    if (CheckStatusProceed2.fld_StatusProceed == false)
                    {
                        RemoveDataFunc.RemoveData_tbl_SAPPostingDetail(db2, CheckStatusProceed2.fld_ID);

                        CheckStatusProceed2.fld_ModifiedDT = DateNow;
                        CheckStatusProceed2.fld_ModifiedBy = UserID;
                        CheckStatusProceed2.fld_DocDate = LastDayMonthPosting;
                        CheckStatusProceed2.fld_PostingDate = LastDayMonthPosting;
                        db2.Entry(CheckStatusProceed2).State = EntityState.Modified;
                        db2.SaveChanges();

                        Add_To_tbl_SAPPostRef2(db, db2, Month, Year, NegaraID, SyarikatID, WilayahID, LadangID, SAPPostID2, GajiKawalan, GLGajiKawalan, GLKeteranganGajiKawalan, out DeductionStatus, out BalAmountDeduct, compCode);
                    }
                }
                else
                {
                    CompanyCode = Compcode.fld_CostCentre; //Added by kamalia 21/10/21
                    var SAPDocType2 = SapDocType.Where(x => x.fldOptConfValue == "2").FirstOrDefault();
                    tbl_SAPPostRef2.fld_Month = Month;
                    tbl_SAPPostRef2.fld_Year = Year;
                    tbl_SAPPostRef2.fld_CompCode = CompanyCode;
                    tbl_SAPPostRef2.fld_DocDate = LastDayMonthPosting;
                    tbl_SAPPostRef2.fld_PostingDate = LastDayMonthPosting;
                    tbl_SAPPostRef2.fld_DocType = SAPDocType2.fldOptConfFlag2;
                    tbl_SAPPostRef2.fld_Purpose = short.Parse(SAPDocType2.fldOptConfValue);
                    tbl_SAPPostRef2.fld_CreatedDT = DateNow;
                    tbl_SAPPostRef2.fld_CreatedBy = UserID;
                    tbl_SAPPostRef2.fld_NegaraID = NegaraID;
                    tbl_SAPPostRef2.fld_SyarikatID = SyarikatID;
                    tbl_SAPPostRef2.fld_WilayahID = WilayahID;
                    tbl_SAPPostRef2.fld_LadangID = LadangID;
                    tbl_SAPPostRef2.fld_StatusProceed = false;
                    db2.tbl_SAPPostRef.Add(tbl_SAPPostRef2);
                    db2.SaveChanges();

                    SAPPostID2 = tbl_SAPPostRef2.fld_ID;

                    Add_To_tbl_SAPPostRef2(db, db2, Month, Year, NegaraID, SyarikatID, WilayahID, LadangID, SAPPostID2, GajiKawalan, GLGajiKawalan, GLKeteranganGajiKawalan, out DeductionStatus, out BalAmountDeduct, compCode);
                }
                message = PurMsg + " (Created SAP Data Step 2)";
                Log += i == 1 ? DateTimeFunc.GetDateTime() + " - " + message : "\r\n" + DateTimeFunc.GetDateTime() + " - " + message;
                //}
            }
            else
            {
                message = PurMsg + " (No Data SAP Created)";
                Log += i == 1 ? DateTimeFunc.GetDateTime() + " - " + message : "\r\n" + DateTimeFunc.GetDateTime() + " - " + message;
            }
        }
        //modified by kamalia 2/12/21
        public void Add_To_tbl_SAPPostRef1(GenSalaryModelHQ db, GenSalaryModelEstate db2, int? Month, int? Year, int? NegaraID, int? SyarikatID, int? WilayahID, int? LadangID, Guid SAPPostID1, out decimal? GajiKawalan, out string GLGajiKawalan, out string GLKeteranganGajiKawalan, out bool Contribution, List<tbl_Pkjmast> Pkjmstlists, string compCode)
        {
            List<tbl_SAPPostDataDetails> tbl_SAPPostDataDetails = new List<tbl_SAPPostDataDetails>();
            int i = 1;
            decimal? Amount = 0;
            string DescActvt = "";
            GajiKawalan = 0;
            GLGajiKawalan = "";
            GLKeteranganGajiKawalan = "";
            Contribution = false;

            var ScTrans = db2.tbl_Sctran.Where(x => x.fld_Month == Month && x.fld_Year == Year && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID).Select(s => new { s.fld_GL, s.fld_IO, s.fld_Amt, s.fld_KodAktvt, s.fld_Keterangan, s.fld_JnisAktvt, s.fld_SAPType }).ToList();
            var GetWorkActvt = ScTrans.Where(x => x.fld_KodAktvt.Substring(0, 1) == "0" || x.fld_KodAktvt.Substring(0, 1) == "1").Select(s => new { s.fld_GL, s.fld_IO, s.fld_Amt, s.fld_KodAktvt, s.fld_SAPType }).ToList();
            var GetWorkActvtDistincts = GetWorkActvt.Select(s => new { s.fld_GL, s.fld_IO, s.fld_KodAktvt, s.fld_SAPType }).Distinct().ToList();
            var GetEstateCOde = db.tbl_Ladang.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WlyhID == WilayahID && x.fld_ID == LadangID).Select(s => s.fld_LdgCode).FirstOrDefault();

            foreach (var GetWorkActvtDistinctsTKT in GetWorkActvtDistincts)
            {
                DescActvt = "3" + GetEstateCOde + "- GAJI BURUH TKI BERHASIL" + "(" + GetEstateCOde + ") " + Month + "/" + Year;
                //modified by kamalia 8/12/2021
                Amount = GetWorkActvt.Where(x => x.fld_GL == GetWorkActvtDistinctsTKT.fld_GL && x.fld_IO == GetWorkActvtDistinctsTKT.fld_IO && x.fld_KodAktvt == GetWorkActvtDistinctsTKT.fld_KodAktvt && x.fld_SAPType == GetWorkActvtDistinctsTKT.fld_SAPType).Sum(s => s.fld_Amt);//end
                if (Amount != 0)
                {
                    tbl_SAPPostDataDetails.Add(new tbl_SAPPostDataDetails() { fld_Amount = Amount, fld_Currency = "RM", fld_Desc = DescActvt, fld_GL = GetWorkActvtDistinctsTKT.fld_GL, fld_IO = GetWorkActvtDistinctsTKT.fld_IO, fld_ItemNo = i, fld_Purpose = "1", fld_SAPActivityCode = GetWorkActvtDistinctsTKT.fld_KodAktvt, fld_SAPPostRefID = SAPPostID1, fld_flag = 0, fld_SAPType = GetWorkActvtDistinctsTKT.fld_SAPType });
                    i++;
                }
            }

            var GetNotWorkActs = db.tbl_CustomerVendorGLMap.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_TypeCode == "GL" && x.fld_compcode == compCode).ToList();

            //Kira Byar Cuti flag = 2
            var GetNotWorkAct3s = GetNotWorkActs.Where(x => x.fld_Flag == "2" && x.fld_TypeCode == "GL").Select(s => s.fld_KodAktiviti).Distinct().ToList();
            var CheckNotWorkAct3 = ScTrans.Where(x => GetNotWorkAct3s.Contains(x.fld_KodAktvt)).Select(s => new { s.fld_GL, s.fld_IO, s.fld_KodAktvt, s.fld_SAPType }).Distinct().ToList();
            if (CheckNotWorkAct3.Count > 0)
            {
                foreach (var CheckNotWorkAct3Data in CheckNotWorkAct3)
                {
                    //modified by kamalia 28/8/2021
                    DescActvt = ScTrans.Where(x => x.fld_GL == CheckNotWorkAct3Data.fld_GL && x.fld_IO == CheckNotWorkAct3Data.fld_IO && x.fld_KodAktvt == CheckNotWorkAct3Data.fld_KodAktvt && x.fld_SAPType == CheckNotWorkAct3Data.fld_SAPType).Select(s => s.fld_Keterangan).FirstOrDefault() + " (" + GetEstateCOde + ") " + Month + "/" + Year;
                    Amount = ScTrans.Where(x => x.fld_GL == CheckNotWorkAct3Data.fld_GL && x.fld_IO == CheckNotWorkAct3Data.fld_IO && x.fld_KodAktvt == CheckNotWorkAct3Data.fld_KodAktvt && x.fld_SAPType == CheckNotWorkAct3Data.fld_SAPType).Sum(s => s.fld_Amt);
                    if (Amount != 0)
                    {
                        tbl_SAPPostDataDetails.Add(new tbl_SAPPostDataDetails() { fld_Amount = Amount, fld_Currency = "RM", fld_Desc = DescActvt.ToUpper(), fld_GL = CheckNotWorkAct3Data.fld_GL, fld_IO = CheckNotWorkAct3Data.fld_IO, fld_ItemNo = i, fld_Purpose = "1", fld_SAPActivityCode = CheckNotWorkAct3Data.fld_KodAktvt, fld_SAPPostRefID = SAPPostID1, fld_flag = 0, fld_SAPType = CheckNotWorkAct3Data.fld_SAPType });
                        i++;
                        Contribution = true;
                    }
                }
            }

            //Kira Caruman Majikan flag = 1
            var GetNotWorkAct2s = GetNotWorkActs.Where(x => x.fld_Flag == "1" && x.fld_TypeCode == "GL").Select(s => s.fld_KodAktiviti).ToList();
            //var CheckNotWorkAct2 = ScTrans.Where(x => GetNotWorkAct2s.Contains(x.fld_KodAktvt)).Select(s => new { s.fld_GL, s.fld_IO, s.fld_SAPType }).Distinct().ToList();
            var CheckNotWorkAct2 = ScTrans.Where(x => GetNotWorkAct2s.Contains(x.fld_KodAktvt)).Select(s => new { s.fld_GL, s.fld_IO, s.fld_KodAktvt, s.fld_SAPType }).Distinct().ToList();
            if (CheckNotWorkAct2.Count > 0)
            {
                foreach (var CheckNotWorkAct2Data in CheckNotWorkAct2)
                {
                    //DescActvt = ScTrans.Where(x => x.fld_GL == CheckNotWorkAct2Data.fld_GL && x.fld_IO == CheckNotWorkAct2Data.fld_IO && x.fld_SAPType == CheckNotWorkAct2Data.fld_SAPType).Select(s => s.fld_Keterangan).FirstOrDefault() + " (" + GetEstateCOde + ") " + Month + "/" + Year;
                    //Amount = ScTrans.Where(x => x.fld_GL == CheckNotWorkAct2Data.fld_GL && x.fld_IO == CheckNotWorkAct2Data.fld_IO && x.fld_SAPType == CheckNotWorkAct2Data.fld_SAPType).Sum(s => s.fld_Amt);
                    //var codeActiviti = ScTrans.Where(x => x.fld_GL == CheckNotWorkAct2Data.fld_GL && x.fld_IO == CheckNotWorkAct2Data.fld_IO && x.fld_SAPType == CheckNotWorkAct2Data.fld_SAPType).Select(s => s.fld_KodAktvt).FirstOrDefault();
                    DescActvt = ScTrans.Where(x => x.fld_GL == CheckNotWorkAct2Data.fld_GL && x.fld_IO == CheckNotWorkAct2Data.fld_IO && x.fld_KodAktvt == CheckNotWorkAct2Data.fld_KodAktvt && x.fld_SAPType == CheckNotWorkAct2Data.fld_SAPType).Select(s => s.fld_Keterangan).FirstOrDefault() + " (" + GetEstateCOde + ") " + Month + "/" + Year;
                    Amount = ScTrans.Where(x => x.fld_GL == CheckNotWorkAct2Data.fld_GL && x.fld_IO == CheckNotWorkAct2Data.fld_IO && x.fld_KodAktvt == CheckNotWorkAct2Data.fld_KodAktvt && x.fld_SAPType == CheckNotWorkAct2Data.fld_SAPType).Sum(s => s.fld_Amt);
                    if (Amount != 0)
                    {
                        tbl_SAPPostDataDetails.Add(new tbl_SAPPostDataDetails() { fld_Amount = Amount, fld_Currency = "RM", fld_Desc = DescActvt.ToUpper(), fld_GL = CheckNotWorkAct2Data.fld_GL, fld_IO = CheckNotWorkAct2Data.fld_IO, fld_ItemNo = i, fld_Purpose = "1", fld_SAPActivityCode = CheckNotWorkAct2Data.fld_KodAktvt, fld_SAPPostRefID = SAPPostID1, fld_flag = 0, fld_SAPType = CheckNotWorkAct2Data.fld_SAPType });
                        i++;
                        Contribution = true;
                    }
                }
            }

            //kira aips flag = 6
            var GetNotWorkAct7s = GetNotWorkActs.Where(x => x.fld_Flag == "6" && (x.fld_TypeCode == "GL")).Select(s => s.fld_KodAktiviti).ToList();
            var CheckNotWorkAct7 = ScTrans.Where(x => GetNotWorkAct7s.Contains(x.fld_KodAktvt)).ToList();
            if (CheckNotWorkAct7.Count > 0)
            {
                foreach (var GetNotWorkAct7 in CheckNotWorkAct7)
                {
                    DescActvt = ScTrans.Where(x => x.fld_KodAktvt == GetNotWorkAct7.fld_KodAktvt && x.fld_GL == GetNotWorkAct7.fld_GL && x.fld_SAPType == GetNotWorkAct7.fld_SAPType).Select(s => s.fld_Keterangan).FirstOrDefault() + " (" + GetEstateCOde + ") " + Month + "/" + Year;
                    Amount = ScTrans.Where(x => x.fld_KodAktvt == GetNotWorkAct7.fld_KodAktvt && x.fld_GL == GetNotWorkAct7.fld_GL && x.fld_IO == GetNotWorkAct7.fld_IO && x.fld_SAPType == GetNotWorkAct7.fld_SAPType).Sum(s => s.fld_Amt);
                    if (Amount != 0)
                    {
                        tbl_SAPPostDataDetails.Add(new tbl_SAPPostDataDetails() { fld_Amount = Amount, fld_Currency = "RM", fld_Desc = DescActvt.ToUpper(), fld_GL = GetNotWorkAct7.fld_GL, fld_IO = GetNotWorkAct7.fld_IO, fld_ItemNo = i, fld_Purpose = "1", fld_SAPActivityCode = "3900", fld_SAPPostRefID = SAPPostID1, fld_flag = 0, fld_SAPType = GetNotWorkAct7.fld_SAPType });
                        i++;

                    }
                }
            }
            //Kira Tolakkan flag=5
            var GetNotWorkAct6s = GetNotWorkActs.Where(x => x.fld_Flag == "5" && (x.fld_TypeCode == "GL")).Select(s => s.fld_KodAktiviti).ToList();
            var CheckNotWorkAct6 = ScTrans.Where(x => GetNotWorkAct6s.Contains(x.fld_KodAktvt)).Select(s => new { s.fld_KodAktvt, s.fld_GL, s.fld_IO, s.fld_SAPType }).Distinct().ToList();
            if (CheckNotWorkAct6.Count > 0)
            {
                foreach (var CheckNotWorkAct6Data in CheckNotWorkAct6)
                {
                    //Modified by kamalia 14/9/2021
                    DescActvt = ScTrans.Where(x => x.fld_GL == CheckNotWorkAct6Data.fld_GL && x.fld_IO == CheckNotWorkAct6Data.fld_IO && x.fld_KodAktvt == CheckNotWorkAct6Data.fld_KodAktvt && x.fld_SAPType == CheckNotWorkAct6Data.fld_SAPType).Select(s => s.fld_Keterangan).FirstOrDefault() + " (" + GetEstateCOde + ") " + Month + "/" + Year;
                    Amount = ScTrans.Where(x => x.fld_GL == CheckNotWorkAct6Data.fld_GL && x.fld_KodAktvt == CheckNotWorkAct6Data.fld_KodAktvt && x.fld_SAPType == CheckNotWorkAct6Data.fld_SAPType).Sum(s => s.fld_Amt);
                    if (Amount != 0)
                    {
                        tbl_SAPPostDataDetails.Add(new tbl_SAPPostDataDetails() { fld_Amount = -Amount, fld_Currency = "RM", fld_Desc = DescActvt.ToUpper(), fld_GL = CheckNotWorkAct6Data.fld_GL, fld_ItemNo = i, fld_Purpose = "1", fld_SAPActivityCode = CheckNotWorkAct6Data.fld_KodAktvt, fld_SAPPostRefID = SAPPostID1, fld_flag = 0, fld_SAPType = CheckNotWorkAct6Data.fld_SAPType }); //modified by kamalia 17/3/2022
                        i++;
                        Contribution = true;

                    }
                }
            }

            //kira gaji flag =4
            var GetNotWorkAct5s = GetNotWorkActs.Where(x => x.fld_Flag == "4" && (x.fld_TypeCode == "GL")).Select(s => s.fld_KodAktiviti).ToList();
            var CheckNotWorkAct5 = ScTrans.Where(x => GetNotWorkAct5s.Contains(x.fld_KodAktvt)).ToList();
            if (CheckNotWorkAct5.Count > 0)
            {
                foreach (var GetNotWorkAct5 in CheckNotWorkAct5)
                {
                    DescActvt = GetNotWorkAct5.fld_Keterangan + " (" + GetEstateCOde + ") " + Month + "/" + Year;
                    Amount = GetNotWorkAct5.fld_Amt;
                    if (Amount != 0)
                    {
                        var GLNo = ScTrans.Where(x => x.fld_KodAktvt == GetNotWorkAct5.fld_KodAktvt && x.fld_GL == GetNotWorkAct5.fld_GL).Select(s => s.fld_GL).FirstOrDefault();
                        tbl_SAPPostDataDetails.Add(new tbl_SAPPostDataDetails() { fld_Amount = -Amount, fld_Currency = "RM", fld_Desc = DescActvt.ToUpper(), fld_GL = GLNo, fld_ItemNo = i, fld_Purpose = "1", fld_SAPActivityCode = "4000", fld_SAPPostRefID = SAPPostID1, fld_flag = 0 });
                        i++;
                        //add by kamalia 4/5/2022
                        Contribution = true;
                    }
                }
            }
            //Kira Caruman Majikan Pekerja (Kawalan Gaji) flag =3
            var GetNotWorkAct4s = GetNotWorkActs.Where(x => x.fld_Flag == "3" && x.fld_TypeCode == "GL").Select(s => s.fld_KodAktiviti).Distinct().ToList();
            var CheckNotWorkAct4 = ScTrans.Where(x => GetNotWorkAct4s.Contains(x.fld_KodAktvt)).ToList();
            if (CheckNotWorkAct4.Count > 0)
            {
                //var CCNo = db.tbl_CustomerVendorGLMap.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WIlayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Flag == "1" && x.fld_TypeCode == "CCGLFINANCE").Select(s => s.fld_SAPCode).FirstOrDefault();
                DescActvt = "CLEARING" + " (" + GetEstateCOde + ") " + Month + "/" + Year;
                Amount = ScTrans.Where(x => GetNotWorkAct4s.Contains(x.fld_KodAktvt)).Sum(s => s.fld_Amt);
                if (Amount != 0)
                {
                    //modified by kamalia 24/2/22
                    var GLNo = db.tbl_CustomerVendorGLMap.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_Flag == "4" && x.fld_TypeCode == "GL" && x.fld_compcode == compCode).Select(s => s.fld_SAPCode).FirstOrDefault();
                    tbl_SAPPostDataDetails.Add(new tbl_SAPPostDataDetails() { fld_Amount = -Amount, fld_Currency = "RM", fld_Desc = DescActvt.ToUpper(), fld_GL = GLNo, fld_ItemNo = i, fld_Purpose = "1", fld_SAPActivityCode = null, fld_SAPPostRefID = SAPPostID1, fld_flag = 0 });
                    i++;

                    GajiKawalan = Amount;
                    GLGajiKawalan = GLNo;
                    GLKeteranganGajiKawalan = DescActvt;
                }
            }

            if (tbl_SAPPostDataDetails.Count > 0)
            {
                db2.tbl_SAPPostDataDetails.AddRange(tbl_SAPPostDataDetails);
                db2.SaveChanges();
            }
        }

        public void GetBackRequiredData(GenSalaryModelHQ db, GenSalaryModelEstate db2, int? Month, int? Year, int? NegaraID, int? SyarikatID, int? WilayahID, int? LadangID, out decimal? GajiKawalan, out string GLGajiKawalan, out string GLKeteranganGajiKawalan, string compCode)
        {
            decimal? Amount = 0;
            string DescActvt = "";
            GajiKawalan = 0;
            GLGajiKawalan = "";
            GLKeteranganGajiKawalan = "";

            var GetEstateCOde = db.tbl_Ladang.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WlyhID == WilayahID && x.fld_ID == LadangID).Select(s => s.fld_LdgCode).FirstOrDefault();
            var GetNotWorkActs = db.tbl_CustomerVendorGLMap.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_TypeCode == "GL" && x.fld_compcode == compCode).ToList();
            var ScTrans = db2.tbl_Sctran.Where(x => x.fld_Month == Month && x.fld_Year == Year && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID).Select(s => new { s.fld_GL, s.fld_IO, s.fld_Amt, s.fld_KodAktvt, s.fld_Keterangan, s.fld_JnisAktvt, s.fld_SAPType }).ToList();
            var GetWorkActvt = ScTrans.Where(x => x.fld_KodAktvt.Substring(0, 1) == "0" || x.fld_KodAktvt.Substring(0, 1) == "1").Select(s => new { s.fld_GL, s.fld_IO, s.fld_Amt, s.fld_KodAktvt, s.fld_SAPType }).ToList();

            var GetNotWorkAct4s = GetNotWorkActs.Where(x => x.fld_Flag == "3" && x.fld_TypeCode == "GL").Select(s => s.fld_KodAktiviti).Distinct().ToList();
            var CheckNotWorkAct4 = ScTrans.Where(x => GetNotWorkAct4s.Contains(x.fld_KodAktvt)).ToList();
            if (CheckNotWorkAct4.Count > 0)
            {
                DescActvt = "CLEARING" + " (" + GetEstateCOde + ") " + Month + "/" + Year;
                Amount = ScTrans.Where(x => GetNotWorkAct4s.Contains(x.fld_KodAktvt)).Sum(s => s.fld_Amt);
                if (Amount != 0)
                {
                    var GLNo = GetNotWorkActs.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_Flag == "4" && x.fld_TypeCode == "GL").Select(s => s.fld_SAPCode).FirstOrDefault();
                    GajiKawalan = Amount;
                    GLGajiKawalan = GLNo;
                    GLKeteranganGajiKawalan = DescActvt;
                }
            }
        }

        //modified whole function by kamalia 15/2/22
        public void Add_To_tbl_SAPPostRef2(GenSalaryModelHQ db, GenSalaryModelEstate db2, int? Month, int? Year, int? NegaraID, int? SyarikatID, int? WilayahID, int? LadangID, Guid SAPPostID2, decimal? GajiKawalan, string GLGajiKawalan, string GLKeteranganGajiKawalan, out bool DeductionStatus, out decimal? BalAmountDeduct, string compCode)
        {
            List<tbl_SAPPostDataDetails> tbl_SAPPostDataDetails = new List<tbl_SAPPostDataDetails>();
            int i = 1; // Modified by kamalia 18/11/21
            decimal? Amount = 0;
            decimal? Amount3 = 0;
            decimal? Amount4 = 0;
            decimal? Amount5 = 0;
            BalAmountDeduct = 0;
            string DescActvt = "";
            DeductionStatus = false;
            var GetEstateCOde = db.tbl_Ladang.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WlyhID == WilayahID && x.fld_ID == LadangID).Select(s => s.fld_LdgCode).FirstOrDefault();
            var NamaLadang = db.tbl_Ladang.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WlyhID == WilayahID && x.fld_ID == LadangID).Select(s => s.fld_LdgName).FirstOrDefault();
            var flag = db2.tbl_SAPPostDataDetails.Where(x => x.fld_SAPPostRefID == SAPPostID2).Select(s => s.fld_flag).FirstOrDefault();

            //clearing batch 1 -socso sip sbkp
            var GetNotWorkActs = db.tbl_CustomerVendorGLMap.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_TypeCode == "GL" && x.fld_compcode == compCode).ToList();
            var GetNotWorkCodeAct1s = GetNotWorkActs.Where(x => x.fld_Flag == "3" && x.fld_KodAktiviti != "3802").Select(s => s.fld_KodAktiviti).Distinct().ToArray();
            var ScTrans = db2.tbl_Sctran.Where(x => x.fld_Month == Month && x.fld_Year == Year && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID).Select(s => new { s.fld_GL, s.fld_IO, s.fld_Amt, s.fld_KodAktvt, s.fld_Keterangan, s.fld_Kategori }).ToList();
            var GetWorkActvt = ScTrans.Where(x => x.fld_KodAktvt.Substring(0, 1) == "0").Select(s => new { s.fld_GL, s.fld_IO, s.fld_Amt, s.fld_KodAktvt }).ToList();
            var GLClearing = db.tbl_CustomerVendorGLMap.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_Flag == "3" && x.fld_TypeCode == "GL" && x.fld_compcode == compCode).Select(s => new { s.fld_SAPCode, s.fld_KodAktiviti }).FirstOrDefault();
            var vendorList = db.tbl_VDSAP.Where(x => x.fld_CompanyCode == compCode && x.fld_Deleted == false).ToList();
            DescActvt = GLKeteranganGajiKawalan;
            Amount = ScTrans.Where(x => GetNotWorkCodeAct1s.Contains(x.fld_KodAktvt)).Sum(s => s.fld_Amt);
            if (Amount != 0)
            {
                if (flag == null)
                {
                    flag = 1;
                }
                var GLNo = GLClearing.fld_SAPCode;
                tbl_SAPPostDataDetails.Add(new tbl_SAPPostDataDetails() { fld_Amount = Amount, fld_Currency = "RM", fld_Desc = DescActvt, fld_GL = GLNo, fld_ItemNo = i, fld_Purpose = "2", fld_SAPActivityCode = GLClearing.fld_KodAktiviti, fld_SAPPostRefID = SAPPostID2, fld_flag = flag });
                i++;
            }

            foreach (var GetNotWorkCodeAct1 in GetNotWorkCodeAct1s)
            {
                //modified by kamalia 21/2/22
                Amount = ScTrans.Where(x => x.fld_KodAktvt == GetNotWorkCodeAct1).Sum(s => s.fld_Amt);
                Amount = Amount == null ? 0 : Amount;
                if (Amount != 0)
                {
                    var vendorNo1 = GetNotWorkActs.Where(x => x.fld_Flag == "3" && x.fld_KodAktiviti != "3802").Select(s => s.fld_VendorNo).FirstOrDefault();
                    var getDesc = ScTrans.Where(x => x.fld_KodAktvt == GetNotWorkCodeAct1).Select(s => s.fld_Keterangan).FirstOrDefault().ToUpper();
                    DescActvt = getDesc + " (" + GetEstateCOde + ") " + Month + "/" + Year;

                    var GLNo1 = GetNotWorkActs.Where(x => x.fld_KodAktiviti == GetNotWorkCodeAct1).Select(s => s.fld_SAPCode).FirstOrDefault();
                    tbl_SAPPostDataDetails.Add(new tbl_SAPPostDataDetails() { fld_Amount = -Amount, fld_Currency = "RM", fld_Desc = DescActvt, fld_GL = null, fld_ItemNo = i, fld_Purpose = "2", fld_SAPActivityCode = GetNotWorkCodeAct1, fld_SAPPostRefID = SAPPostID2, fld_flag = flag, fld_VendorCode = vendorNo1 });
                    i++;
                }
            }

            //clearing batch 2 -kwsp
            var GetNotWorkActs2 = db.tbl_CustomerVendorGLMap.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_TypeCode == "GL" && x.fld_compcode == compCode).ToList();
            var GetNotWorkCodeAct1s2 = GetNotWorkActs.Where(x => x.fld_Flag == "3" && x.fld_KodAktiviti == "3802").Select(s => s.fld_KodAktiviti).Distinct().ToArray();
            var ScTrans2 = db2.tbl_Sctran.Where(x => x.fld_Month == Month && x.fld_Year == Year && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID).Select(s => new { s.fld_GL, s.fld_IO, s.fld_Amt, s.fld_KodAktvt, s.fld_Keterangan }).ToList();
            var GetWorkActvt2 = ScTrans.Where(x => x.fld_KodAktvt.Substring(0, 1) == "0").Select(s => new { s.fld_GL, s.fld_IO, s.fld_Amt, s.fld_KodAktvt }).ToList();
            var GLClearing2 = db.tbl_CustomerVendorGLMap.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_Flag == "3" && x.fld_TypeCode == "GL" && x.fld_compcode == compCode).Select(s => new { s.fld_SAPCode, s.fld_KodAktiviti }).FirstOrDefault();
            DescActvt = "CLEARING KWSP (PEKERJA DAN MAJIKAN)" + " (" + GetEstateCOde + ") " + Month + "/" + Year;
            Amount = ScTrans.Where(x => GetNotWorkCodeAct1s2.Contains(x.fld_KodAktvt)).Sum(s => s.fld_Amt);
            if (Amount != 0)
            {
                if (flag == null)
                {
                    flag = 1;
                }
                else
                {
                    flag = flag + 1;
                }
                var GLNo = GLClearing.fld_SAPCode;
                tbl_SAPPostDataDetails.Add(new tbl_SAPPostDataDetails() { fld_Amount = Amount, fld_Currency = "RM", fld_Desc = DescActvt, fld_GL = GLNo, fld_ItemNo = i, fld_Purpose = "2", fld_SAPActivityCode = GLClearing.fld_KodAktiviti, fld_SAPPostRefID = SAPPostID2, fld_flag = flag });
                i++;
            }

            foreach (var GetNotWorkCodeAct1 in GetNotWorkCodeAct1s2)
            {
                //modified by kamalia 21/2/22   
                Amount = ScTrans.Where(x => x.fld_KodAktvt == GetNotWorkCodeAct1).Sum(s => s.fld_Amt);
                Amount = Amount == null ? 0 : Amount;
                if (Amount != 0)
                {
                    var vendorNo2 = GetNotWorkActs2.Where(x => x.fld_Flag == "3" && x.fld_KodAktiviti == "3802").Select(s => s.fld_VendorNo).FirstOrDefault();
                    var getDesc = ScTrans.Where(x => x.fld_KodAktvt == GetNotWorkCodeAct1).Select(s => s.fld_Keterangan).FirstOrDefault().ToUpper();
                    DescActvt = getDesc + " (" + GetEstateCOde + ") " + Month + "/" + Year;
                    var GLNo1 = GetNotWorkActs.Where(x => x.fld_KodAktiviti == GetNotWorkCodeAct1).Select(s => s.fld_SAPCode).FirstOrDefault();
                    tbl_SAPPostDataDetails.Add(new tbl_SAPPostDataDetails() { fld_Amount = -Amount, fld_Currency = "RM", fld_Desc = DescActvt, fld_GL = null, fld_ItemNo = i, fld_Purpose = "2", fld_SAPActivityCode = GetNotWorkCodeAct1, fld_SAPPostRefID = SAPPostID2, fld_flag = flag, fld_VendorCode = vendorNo2 });
                    i++;
                }
            }

            //clearing batch 3 - Merchantrade
            var GetNotWorkActs3 = db.tbl_CustomerVendorGLMap.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_TypeCode == "GL" && x.fld_compcode == compCode).ToList();
            var GetNotWorkCodeActs3 = GetNotWorkActs.Where(x => x.fld_Flag == "4").Select(s => s.fld_KodAktiviti).FirstOrDefault();
            var GetWorkActvt3 = ScTrans.Where(x => x.fld_Kategori == 13).FirstOrDefault();

            if (GetWorkActvt3 != null)
            {
                DescActvt = GetWorkActvt3.fld_Keterangan + " (" + GetEstateCOde + ") " + Month + "/" + Year;
                Amount3 = GetWorkActvt3.fld_Amt;
            }

            if (Amount3 > 0)
            {
                if (flag == null)
                {
                    flag = 1;
                }
                else
                {
                    flag = flag + 1;
                }
                var GLNo = GLClearing.fld_SAPCode;
                tbl_SAPPostDataDetails.Add(new tbl_SAPPostDataDetails() { fld_Amount = Amount3, fld_Currency = "RM", fld_Desc = DescActvt.ToUpper(), fld_GL = GLNo, fld_ItemNo = i, fld_Purpose = "2", fld_SAPActivityCode = GLClearing.fld_KodAktiviti, fld_SAPPostRefID = SAPPostID2, fld_flag = flag });
                i++;

                var vendor = vendorList.Where(x => x.fld_VendorInd == "MTA").FirstOrDefault();
                DescActvt = "MERCHANTRADE ASIA " + " (" + GetEstateCOde + ") " + Month + "/" + Year;
                Amount = db.tblSokPermhnWang.Where(x => x.fld_Month == Month && x.fld_Year == Year && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID).Sum(s => s.fld_JumlahEwallet);
                if (Amount != 0)
                {
                    var GLNo1 = GetNotWorkActs3.Where(x => x.fld_KodAktiviti == GetNotWorkCodeActs3).Select(s => s.fld_SAPCode).FirstOrDefault();
                    tbl_SAPPostDataDetails.Add(new tbl_SAPPostDataDetails() { fld_Amount = -Amount3, fld_Currency = "RM", fld_Desc = DescActvt, fld_GL = null, fld_ItemNo = i, fld_Purpose = "2", fld_SAPActivityCode = GLNo1, fld_SAPPostRefID = SAPPostID2, fld_flag = flag, fld_VendorCode = vendor.fld_VendorNo });
                    i++;
                }
            }

            //clearing batch 5 - Maybank M2E
            Amount5 = 0;
            var GetWorkActvt7 = ScTrans.Where(x => x.fld_Kategori == 16).FirstOrDefault();
            if (GetWorkActvt7 != null)
            {
                Amount5 = GetWorkActvt7.fld_Amt;
            }


            if (Amount5 > 0)
            {
                if (flag == null)
                {
                    flag = 1;
                }
                else
                {
                    flag = flag + 1;
                }
                DescActvt = GetWorkActvt7.fld_Keterangan + " (" + GetEstateCOde + ") " + Month + "/" + Year;
                var GLNo = GLClearing.fld_SAPCode;
                tbl_SAPPostDataDetails.Add(new tbl_SAPPostDataDetails() { fld_Amount = Amount5, fld_Currency = "RM", fld_Desc = DescActvt.ToUpper(), fld_GL = GLNo, fld_ItemNo = i, fld_Purpose = "2", fld_SAPActivityCode = GLClearing.fld_KodAktiviti, fld_SAPPostRefID = SAPPostID2, fld_flag = flag });
                i++;
            }

            decimal? Amount6 = 0;
            GetWorkActvt7 = ScTrans.Where(x => x.fld_Kategori == 17).FirstOrDefault();
            if (GetWorkActvt7 != null)
            {
                Amount6 = GetWorkActvt7.fld_Amt;
            }


            if (Amount6 > 0)
            {
                if (flag == null)
                {
                    flag = 1;
                }
                else
                {
                    flag = flag + 1;
                }
                DescActvt = GetWorkActvt7.fld_Keterangan + " (" + GetEstateCOde + ") " + Month + "/" + Year;
                var GLNo = GLClearing.fld_SAPCode;
                tbl_SAPPostDataDetails.Add(new tbl_SAPPostDataDetails() { fld_Amount = Amount6, fld_Currency = "RM", fld_Desc = DescActvt.ToUpper(), fld_GL = GLNo, fld_ItemNo = i, fld_Purpose = "2", fld_SAPActivityCode = GLClearing.fld_KodAktiviti, fld_SAPPostRefID = SAPPostID2, fld_flag = flag });
                i++;
            }

            var totalAmount56 = Amount5 + Amount6;

            if (totalAmount56 > 0)
            {
                var vendor = vendorList.Where(x => x.fld_VendorInd == "M2E").FirstOrDefault();
                var totalAmount = totalAmount56;
                tbl_SAPPostDataDetails.Add(new tbl_SAPPostDataDetails() { fld_Amount = -totalAmount, fld_Currency = "RM", fld_Desc = vendor.fld_Desc.ToUpper() + " (" + GetEstateCOde + ") " + Month + "/" + Year, fld_GL = null, fld_ItemNo = i, fld_Purpose = "2", fld_SAPActivityCode = "", fld_SAPPostRefID = SAPPostID2, fld_flag = flag, fld_VendorCode = vendor.fld_VendorNo });
                i++;
            }

            Amount5 = 0;

            //clearing batch 4 - Rancangan
            var GetWorkActvt4 = ScTrans.Where(x => x.fld_Kategori == 11 || x.fld_Kategori == 12).ToList();
            if (GetWorkActvt4.Count() > 0)
            {
                Amount4 = GetWorkActvt4.Sum(s => s.fld_Amt.Value);
            }

            //modified by kamy 31/2/2022
            var GetNotWorkActs4 = db.tbl_CustomerVendorGLMap.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_TypeCode == "GL" && x.fld_compcode == compCode).ToList();
            var GetNotWorkCodeAct4s = GetNotWorkActs.Where(x => (x.fld_Flag == "4" || x.fld_Flag == "5")).Select(s => s.fld_KodAktiviti).Distinct().ToArray();
            var GetActivityCode = ScTrans.Where(x => GetNotWorkCodeAct4s.Contains(x.fld_KodAktvt)).OrderByDescending(o => o.fld_KodAktvt).Select(s => s.fld_KodAktvt).Distinct();

            var GetActivitySctrans = GetWorkActvt4;
            if (flag == null)
            {
                flag = 1;
            }
            else
            {
                flag = flag + 1;
            }

            if (Amount4 > 0)
            {
                //modified by kamalia 17/3/22
                foreach (var GetNotWorkCodeAct4 in GetActivitySctrans)
                {
                    Amount = GetNotWorkCodeAct4.fld_Amt;
                    Amount = Amount == null ? 0 : Amount;
                    var GLNo = GLClearing.fld_SAPCode;
                    if (Amount != 0)
                    {
                        var getDesc = ScTrans.Where(x => x.fld_KodAktvt == GetNotWorkCodeAct4.fld_KodAktvt && x.fld_Kategori == GetNotWorkCodeAct4.fld_Kategori).Select(s => s.fld_Keterangan).FirstOrDefault().ToUpper();
                        DescActvt = getDesc + " (" + GetEstateCOde + ") " + Month + "/" + Year;

                        var GLNo1 = GetNotWorkActs.Where(x => x.fld_KodAktiviti == GetNotWorkCodeAct4.fld_KodAktvt).Select(s => s.fld_SAPCode).FirstOrDefault();
                        tbl_SAPPostDataDetails.Add(new tbl_SAPPostDataDetails() { fld_Amount = Amount, fld_Currency = "RM", fld_Desc = DescActvt.ToUpper(), fld_GL = GLNo, fld_ItemNo = i, fld_Purpose = "2", fld_SAPActivityCode = GetNotWorkCodeAct4.fld_KodAktvt, fld_SAPPostRefID = SAPPostID2, fld_flag = flag });
                        i++;
                    }
                }
            }

            //clearing batch 5 - Maybank CDMAS
            var GetWorkActvt5 = ScTrans.Where(x => x.fld_Kategori == 14).FirstOrDefault();
            if (GetWorkActvt5 != null)
            {
                Amount5 = GetWorkActvt5.fld_Amt;
                Amount4 += Amount5;
            }


            if (Amount5 > 0)
            {
                DescActvt = GetWorkActvt5.fld_Keterangan + " (" + GetEstateCOde + ") " + Month + "/" + Year;
                var GLNo = GLClearing.fld_SAPCode;
                tbl_SAPPostDataDetails.Add(new tbl_SAPPostDataDetails() { fld_Amount = Amount5, fld_Currency = "RM", fld_Desc = DescActvt.ToUpper(), fld_GL = GLNo, fld_ItemNo = i, fld_Purpose = "2", fld_SAPActivityCode = GLClearing.fld_KodAktiviti, fld_SAPPostRefID = SAPPostID2, fld_flag = flag });
                i++;
            }

            //clearing batch 5 - Maybank M2U
            Amount5 = 0;
            var GetWorkActvt6 = ScTrans.Where(x => x.fld_Kategori == 15).FirstOrDefault();
            if (GetWorkActvt6 != null)
            {
                Amount5 = GetWorkActvt6.fld_Amt;
                Amount4 += Amount5;
            }


            if (Amount5 > 0)
            {
                DescActvt = GetWorkActvt6.fld_Keterangan + " (" + GetEstateCOde + ") " + Month + "/" + Year;
                var GLNo = GLClearing.fld_SAPCode;
                tbl_SAPPostDataDetails.Add(new tbl_SAPPostDataDetails() { fld_Amount = Amount5, fld_Currency = "RM", fld_Desc = DescActvt.ToUpper(), fld_GL = GLNo, fld_ItemNo = i, fld_Purpose = "2", fld_SAPActivityCode = GLClearing.fld_KodAktiviti, fld_SAPPostRefID = SAPPostID2, fld_flag = flag });
                i++;
            }

            DescActvt = "FELDA " + NamaLadang + " (" + GetEstateCOde + ") " + Month + "/" + Year;
            //var AmountPotongan = ScTrans.Where(x => GetActivityCode.Contains(x.fld_KodAktvt) && x.fld_KodAktvt != "4000").Sum(s => s.fld_Amt);
            Amount = Amount4;// + AmountPotongan;
            if (Amount != 0)
            {
                var vendorNo4 = db.tbl_Ladang.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WlyhID == WilayahID && x.fld_ID == LadangID).Select(s => s.fld_CustCPD).FirstOrDefault();
                var GLNo1 = GetNotWorkActs3.Where(x => x.fld_KodAktiviti == GetNotWorkCodeActs3).Select(s => s.fld_SAPCode).FirstOrDefault();
                tbl_SAPPostDataDetails.Add(new tbl_SAPPostDataDetails() { fld_Amount = -Amount, fld_Currency = "RM", fld_Desc = DescActvt.ToUpper(), fld_GL = null, fld_ItemNo = i, fld_Purpose = "2", fld_SAPActivityCode = "-", fld_SAPPostRefID = SAPPostID2, fld_flag = flag, fld_VendorCode = vendorNo4 });
                i++;
            }


            if (GajiKawalan - Amount > 0)
            {
                BalAmountDeduct = GajiKawalan - Amount;
                DeductionStatus = true;
            }
            db2.tbl_SAPPostDataDetails.AddRange(tbl_SAPPostDataDetails);
            db2.SaveChanges();
        }

        //modified whole function by kamalia 15/2/22
        public void Add_To_tbl_SAPPostRef2Check(GenSalaryModelHQ db, GenSalaryModelEstate db2, int? Month, int? Year, int? NegaraID, int? SyarikatID, int? WilayahID, int? LadangID, Guid SAPPostID2, decimal? GajiKawalan, string GLGajiKawalan, string GLKeteranganGajiKawalan, out bool DeductionStatus, out decimal? BalAmountDeduct, string compCode)
        {
            List<tbl_SAPPostDataDetails> tbl_SAPPostDataDetails = new List<tbl_SAPPostDataDetails>();
            int i = 1; // Modified by kamalia 18/11/21
            //int i = 0;  // commented by kamalia 18/11/21
            decimal? Amount = 0;
            decimal? Amount3 = 0;
            decimal? Amount4 = 0;
            decimal? Amount5 = 0;
            BalAmountDeduct = 0;
            string DescActvt = "";
            DeductionStatus = false;
            var GetEstateCOde = db.tbl_Ladang.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WlyhID == WilayahID && x.fld_ID == LadangID).Select(s => s.fld_LdgCode).FirstOrDefault();
            var NamaLadang = db.tbl_Ladang.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WlyhID == WilayahID && x.fld_ID == LadangID).Select(s => s.fld_LdgName).FirstOrDefault();
            var flag = db2.tbl_SAPPostDataDetails.Where(x => x.fld_SAPPostRefID == SAPPostID2).Select(s => s.fld_flag).FirstOrDefault();

            //clearing batch 1 -socso sip sbkp
            var GetNotWorkActs = db.tbl_CustomerVendorGLMap.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_TypeCode == "GL" && x.fld_compcode == compCode).ToList();
            var GetNotWorkCodeAct1s = GetNotWorkActs.Where(x => x.fld_Flag == "3" && x.fld_KodAktiviti != "3802").Select(s => s.fld_KodAktiviti).Distinct().ToArray();
            var ScTrans = db2.tbl_Sctran.Where(x => x.fld_Month == Month && x.fld_Year == Year && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID).Select(s => new { s.fld_GL, s.fld_IO, s.fld_Amt, s.fld_KodAktvt, s.fld_Keterangan, s.fld_Kategori }).ToList();
            var GetWorkActvt = ScTrans.Where(x => x.fld_KodAktvt.Substring(0, 1) == "0").Select(s => new { s.fld_GL, s.fld_IO, s.fld_Amt, s.fld_KodAktvt }).ToList();
            var GLClearing = db.tbl_CustomerVendorGLMap.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_Flag == "3" && x.fld_TypeCode == "GL" && x.fld_compcode == compCode).Select(s => new { s.fld_SAPCode, s.fld_KodAktiviti }).FirstOrDefault();
            DescActvt = GLKeteranganGajiKawalan;
            Amount = ScTrans.Where(x => GetNotWorkCodeAct1s.Contains(x.fld_KodAktvt)).Sum(s => s.fld_Amt);
            if (Amount != 0)
            {
                if (flag == null)
                {
                    flag = 1;
                }
                var GLNo = GLClearing.fld_SAPCode;
                tbl_SAPPostDataDetails.Add(new tbl_SAPPostDataDetails() { fld_Amount = Amount, fld_Currency = "RM", fld_Desc = DescActvt, fld_GL = GLNo, fld_ItemNo = i, fld_Purpose = "2", fld_SAPActivityCode = GLClearing.fld_KodAktiviti, fld_SAPPostRefID = SAPPostID2, fld_flag = flag });
                i++;
            }

            foreach (var GetNotWorkCodeAct1 in GetNotWorkCodeAct1s)
            {
                //modified by kamalia 21/2/22
                Amount = ScTrans.Where(x => x.fld_KodAktvt == GetNotWorkCodeAct1).Sum(s => s.fld_Amt);
                Amount = Amount == null ? 0 : Amount;
                if (Amount != 0)
                {
                    var vendorNo1 = GetNotWorkActs.Where(x => x.fld_Flag == "3" && x.fld_KodAktiviti != "3802").Select(s => s.fld_VendorNo).FirstOrDefault();
                    var getDesc = ScTrans.Where(x => x.fld_KodAktvt == GetNotWorkCodeAct1).Select(s => s.fld_Keterangan).FirstOrDefault().ToUpper();
                    DescActvt = getDesc + " (" + GetEstateCOde + ") " + Month + "/" + Year;

                    var GLNo1 = GetNotWorkActs.Where(x => x.fld_KodAktiviti == GetNotWorkCodeAct1).Select(s => s.fld_SAPCode).FirstOrDefault();
                    tbl_SAPPostDataDetails.Add(new tbl_SAPPostDataDetails() { fld_Amount = -Amount, fld_Currency = "RM", fld_Desc = DescActvt.ToUpper(), fld_GL = null, fld_ItemNo = i, fld_Purpose = "2", fld_SAPActivityCode = GetNotWorkCodeAct1, fld_SAPPostRefID = SAPPostID2, fld_flag = flag, fld_VendorCode = vendorNo1 });
                    i++;
                }
            }

            //clearing batch 2 -kwsp
            var GetNotWorkActs2 = db.tbl_CustomerVendorGLMap.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_TypeCode == "GL" && x.fld_compcode == compCode).ToList();
            var GetNotWorkCodeAct1s2 = GetNotWorkActs.Where(x => x.fld_Flag == "3" && x.fld_KodAktiviti == "3802").Select(s => s.fld_KodAktiviti).Distinct().ToArray();
            var ScTrans2 = db2.tbl_Sctran.Where(x => x.fld_Month == Month && x.fld_Year == Year && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID).Select(s => new { s.fld_GL, s.fld_IO, s.fld_Amt, s.fld_KodAktvt, s.fld_Keterangan }).ToList();
            var GetWorkActvt2 = ScTrans.Where(x => x.fld_KodAktvt.Substring(0, 1) == "0").Select(s => new { s.fld_GL, s.fld_IO, s.fld_Amt, s.fld_KodAktvt }).ToList();
            var GLClearing2 = db.tbl_CustomerVendorGLMap.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_Flag == "3" && x.fld_TypeCode == "GL" && x.fld_compcode == compCode).Select(s => new { s.fld_SAPCode, s.fld_KodAktiviti }).FirstOrDefault();
            DescActvt = "CLEARING KWSP (PEKERJA DAN MAJIKAN)" + " (" + GetEstateCOde + ") " + Month + "/" + Year;
            Amount = ScTrans.Where(x => GetNotWorkCodeAct1s2.Contains(x.fld_KodAktvt)).Sum(s => s.fld_Amt);
            if (Amount != 0)
            {
                if (flag == null)
                {
                    flag = 1;
                }
                else
                {
                    flag = flag + 1;
                }
                var GLNo = GLClearing.fld_SAPCode;
                tbl_SAPPostDataDetails.Add(new tbl_SAPPostDataDetails() { fld_Amount = Amount, fld_Currency = "RM", fld_Desc = DescActvt, fld_GL = GLNo, fld_ItemNo = i, fld_Purpose = "2", fld_SAPActivityCode = GLClearing.fld_KodAktiviti, fld_SAPPostRefID = SAPPostID2, fld_flag = flag });
                i++;
            }

            foreach (var GetNotWorkCodeAct1 in GetNotWorkCodeAct1s2)
            {
                //modified by kamalia 21/2/22   
                Amount = ScTrans.Where(x => x.fld_KodAktvt == GetNotWorkCodeAct1).Sum(s => s.fld_Amt);
                Amount = Amount == null ? 0 : Amount;
                if (Amount != 0)
                {
                    var vendorNo2 = GetNotWorkActs2.Where(x => x.fld_Flag == "3" && x.fld_KodAktiviti == "3802").Select(s => s.fld_VendorNo).FirstOrDefault();
                    var getDesc = ScTrans.Where(x => x.fld_KodAktvt == GetNotWorkCodeAct1).Select(s => s.fld_Keterangan).FirstOrDefault().ToUpper();
                    DescActvt = getDesc + " (" + GetEstateCOde + ") " + Month + "/" + Year;
                    var GLNo1 = GetNotWorkActs.Where(x => x.fld_KodAktiviti == GetNotWorkCodeAct1).Select(s => s.fld_SAPCode).FirstOrDefault();
                    tbl_SAPPostDataDetails.Add(new tbl_SAPPostDataDetails() { fld_Amount = -Amount, fld_Currency = "RM", fld_Desc = DescActvt, fld_GL = null, fld_ItemNo = i, fld_Purpose = "2", fld_SAPActivityCode = GetNotWorkCodeAct1, fld_SAPPostRefID = SAPPostID2, fld_flag = flag, fld_VendorCode = vendorNo2 });
                    i++;
                }
            }

            //clearing batch 3 - Merchantrade
            var GetNotWorkActs3 = db.tbl_CustomerVendorGLMap.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_TypeCode == "GL" && x.fld_compcode == compCode).ToList();
            var GetNotWorkCodeActs3 = GetNotWorkActs.Where(x => x.fld_Flag == "4").Select(s => s.fld_KodAktiviti).FirstOrDefault();
            var GetWorkActvt3 = ScTrans.Where(x => x.fld_Kategori == 13).FirstOrDefault();
            if (GetWorkActvt3 != null)
            {
                DescActvt = GetWorkActvt3.fld_Keterangan + " (" + GetEstateCOde + ") " + Month + "/" + Year;
                Amount3 = GetWorkActvt3.fld_Amt;

                if (Amount3 > 0)
                {
                    if (flag == null)
                    {
                        flag = 1;
                    }
                    else
                    {
                        flag = flag + 1;
                    }
                    var GLNo = GLClearing.fld_SAPCode;
                    tbl_SAPPostDataDetails.Add(new tbl_SAPPostDataDetails() { fld_Amount = Amount3, fld_Currency = "RM", fld_Desc = DescActvt.ToUpper(), fld_GL = GLNo, fld_ItemNo = i, fld_Purpose = "2", fld_SAPActivityCode = GLClearing.fld_KodAktiviti, fld_SAPPostRefID = SAPPostID2, fld_flag = flag });
                    i++;

                    var vendorNo3 = db.tbl_VDSAP.Where(x => x.fld_Desc == "merchantrade").Select(s => s.fld_VendorNo).FirstOrDefault();
                    DescActvt = "MERCHANTRADE ASIA" + " (" + GetEstateCOde + ") " + Month + "/" + Year;
                    Amount = db.tblSokPermhnWang.Where(x => x.fld_Month == Month && x.fld_Year == Year && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID).Sum(s => s.fld_JumlahEwallet);
                    if (Amount != 0)
                    {
                        var GLNo1 = GetNotWorkActs3.Where(x => x.fld_KodAktiviti == GetNotWorkCodeActs3).Select(s => s.fld_SAPCode).FirstOrDefault();
                        tbl_SAPPostDataDetails.Add(new tbl_SAPPostDataDetails() { fld_Amount = -Amount3, fld_Currency = "RM", fld_Desc = DescActvt, fld_GL = null, fld_ItemNo = i, fld_Purpose = "2", fld_SAPActivityCode = GLNo1, fld_SAPPostRefID = SAPPostID2, fld_flag = flag, fld_VendorCode = vendorNo3 });
                        i++;
                    }
                }
            }

            //clearing batch 4 - Rancangan
            var GetWorkActvt4 = ScTrans.Where(x => x.fld_Kategori == 11 || x.fld_Kategori == 12).ToList();

            Amount4 = GetWorkActvt4.Sum(s => s.fld_Amt.Value);

            //modified by kamy 31/2/2022
            var GetNotWorkActs4 = db.tbl_CustomerVendorGLMap.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_TypeCode == "GL" && x.fld_compcode == compCode).ToList();
            var GetNotWorkCodeAct4s = GetNotWorkActs.Where(x => (x.fld_Flag == "4" || x.fld_Flag == "5")).Select(s => s.fld_KodAktiviti).Distinct().ToArray();
            var GetActivityCode = ScTrans.Where(x => GetNotWorkCodeAct4s.Contains(x.fld_KodAktvt)).OrderByDescending(o => o.fld_KodAktvt).Select(s => s.fld_KodAktvt).Distinct();

            if (Amount4 > 0)
            {
                var GetActivitySctrans = GetWorkActvt4;
                if (flag == null)
                {
                    flag = 1;
                }
                else
                {
                    flag = flag + 1;
                }
                //modified by kamalia 17/3/22
                foreach (var GetNotWorkCodeAct4 in GetActivitySctrans)
                {
                    Amount = GetNotWorkCodeAct4.fld_Amt;
                    Amount = Amount == null ? 0 : Amount;
                    var GLNo = GLClearing.fld_SAPCode;
                    if (Amount != 0)
                    {
                        var getDesc = ScTrans.Where(x => x.fld_KodAktvt == GetNotWorkCodeAct4.fld_KodAktvt && x.fld_Kategori == GetNotWorkCodeAct4.fld_Kategori).Select(s => s.fld_Keterangan).FirstOrDefault().ToUpper();
                        DescActvt = getDesc + " (" + GetEstateCOde + ") " + Month + "/" + Year;

                        var GLNo1 = GetNotWorkActs.Where(x => x.fld_KodAktiviti == GetNotWorkCodeAct4.fld_KodAktvt).Select(s => s.fld_SAPCode).FirstOrDefault();
                        tbl_SAPPostDataDetails.Add(new tbl_SAPPostDataDetails() { fld_Amount = Amount, fld_Currency = "RM", fld_Desc = DescActvt.ToUpper(), fld_GL = GLNo, fld_ItemNo = i, fld_Purpose = "2", fld_SAPActivityCode = GetNotWorkCodeAct4.fld_KodAktvt, fld_SAPPostRefID = SAPPostID2, fld_flag = flag });
                        i++;
                    }
                }

                DescActvt = "FELDA " + NamaLadang + " (" + GetEstateCOde + ") " + Month + "/" + Year;
                var AmountPotongan = ScTrans.Where(x => GetActivityCode.Contains(x.fld_KodAktvt) && x.fld_KodAktvt != "4000").Sum(s => s.fld_Amt);
                Amount = Amount4 + AmountPotongan;
                if (Amount != 0)
                {
                    var vendorNo4 = db.tbl_Ladang.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WlyhID == WilayahID && x.fld_ID == LadangID).Select(s => s.fld_CustCPD).FirstOrDefault();
                    var GLNo1 = GetNotWorkActs3.Where(x => x.fld_KodAktiviti == GetNotWorkCodeActs3).Select(s => s.fld_SAPCode).FirstOrDefault();
                    tbl_SAPPostDataDetails.Add(new tbl_SAPPostDataDetails() { fld_Amount = -Amount, fld_Currency = "RM", fld_Desc = DescActvt, fld_GL = null, fld_ItemNo = i, fld_Purpose = "2", fld_SAPActivityCode = "-", fld_SAPPostRefID = SAPPostID2, fld_flag = flag, fld_VendorCode = vendorNo4 });
                    i++;
                }
            }

            //clearing batch 5 - Maybank
            var amountMaybank = db2.vw_PaySheetPekerja
                 .Where(x => x.fld_Month == Month && x.fld_Year == Year &&
                             x.fld_NegaraID == NegaraID &&
                             x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID &&
                             x.fld_LadangID == LadangID && x.fld_PaymentMode == "4").ToList();

            if (amountMaybank.Count() > 0)
            {
                Amount5 = amountMaybank.Sum(s => s.fld_GajiBersih.Value);
            }

            if (Amount5 != 0)
            {
                if (flag == null)
                {
                    flag = 1;
                }
                else
                {
                    flag = flag + 1;
                }
                DescActvt = "GAJI BERSIH PEKERJA BURUH" + " (" + GetEstateCOde + ") " + Month + "/" + Year;
                var GLNo = GLClearing.fld_SAPCode;
                tbl_SAPPostDataDetails.Add(new tbl_SAPPostDataDetails() { fld_Amount = Amount5, fld_Currency = "RM", fld_Desc = DescActvt, fld_GL = GLNo, fld_ItemNo = i, fld_Purpose = "2", fld_SAPActivityCode = GLClearing.fld_KodAktiviti, fld_SAPPostRefID = SAPPostID2, fld_flag = flag });
                i++;

                DescActvt = "MAYBANK" + " (" + GetEstateCOde + ") " + Month + "/" + Year;
                Amount = db.tblSokPermhnWang.Where(x => x.fld_Month == Month && x.fld_Year == Year && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID).Sum(s => s.fld_JumlahCdmas);
                if (Amount != 0)
                {
                    var vendorNo5 = db.tbl_VDSAP.Where(x => x.fld_Desc == "maybank").Select(s => s.fld_VendorNo).FirstOrDefault();
                    var GLNo1 = GetNotWorkActs3.Where(x => x.fld_KodAktiviti == GetNotWorkCodeActs3).Select(s => s.fld_SAPCode).FirstOrDefault();
                    tbl_SAPPostDataDetails.Add(new tbl_SAPPostDataDetails() { fld_Amount = -Amount5, fld_Currency = "RM", fld_Desc = DescActvt, fld_GL = null, fld_ItemNo = i, fld_Purpose = "2", fld_SAPActivityCode = GLNo1, fld_SAPPostRefID = SAPPostID2, fld_flag = flag, fld_VendorCode = vendorNo5 });
                    i++;
                }
            }
            if (GajiKawalan - Amount > 0)
            {
                BalAmountDeduct = GajiKawalan - Amount;
                DeductionStatus = true;
            }
            db2.tbl_SAPPostDataDetails.AddRange(tbl_SAPPostDataDetails);
            db2.SaveChanges();
        }


        public static DateTime GetLastDayOfMonth(int? Month, int? Year)
        {
            return new DateTime(Year.Value, Month.Value, DateTime.DaysInMonth(Year.Value, Month.Value));
        }


    }
}
