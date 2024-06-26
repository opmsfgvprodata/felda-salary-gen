﻿using SalaryGeneratorServices.ModelsEstate;
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
    public class Step7Func
    {
        private DateTimeFunc DateTimeFunc = new DateTimeFunc();

        public void AddTo_tbl_SAPPostRef(int? NegaraID, int? SyarikatID, int? WilayahID, int? LadangID, int? UserID, DateTime DTProcess, int? Month, int? Year, string processname, string servicesname, int? ClientID, out string Log, short Purpose, string PurMsg, List<tbl_Pkjmast> Pkjmstlists, string compCode, List<tbl_JenisInsentif> incentifsType)
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

            string GLGajiKawalan2 = "";
            decimal? GajiKawalan2 = 0;
            string GLKeteranganGajiKawalan2 = "";

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
                        Add_To_tbl_SAPPostRef1(db, db2, Month, Year, NegaraID, SyarikatID, WilayahID, LadangID, SAPPostID, out GajiKawalan, out GLGajiKawalan, out GLKeteranganGajiKawalan, out GajiKawalan2, out GLGajiKawalan2, out GLKeteranganGajiKawalan2, out Contribution, Pkjmstlists, compCode);

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

                    Add_To_tbl_SAPPostRef1(db, db2, Month, Year, NegaraID, SyarikatID, WilayahID, LadangID, SAPPostID, out GajiKawalan, out GLGajiKawalan, out GLKeteranganGajiKawalan, out GajiKawalan2, out GLGajiKawalan2, out GLKeteranganGajiKawalan2, out Contribution, Pkjmstlists, compCode);

                }

                message = PurMsg + " (Created SAP Data Step 1)";
                Log += i == 1 ? DateTimeFunc.GetDateTime() + " - " + message : "\r\n" + DateTimeFunc.GetDateTime() + " - " + message;

                //if (Contribution)
                //{
                if (string.IsNullOrEmpty(GLKeteranganGajiKawalan))
                {
                    GetBackRequiredData(db, db2, Month, Year, NegaraID, SyarikatID, WilayahID, LadangID, out GajiKawalan, out GLGajiKawalan, out GLKeteranganGajiKawalan, out GajiKawalan2, out GLGajiKawalan2, out GLKeteranganGajiKawalan2, compCode);
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

                        Add_To_tbl_SAPPostRef2(db, db2, Month, Year, NegaraID, SyarikatID, WilayahID, LadangID, SAPPostID2, GajiKawalan, GLGajiKawalan, GLKeteranganGajiKawalan, GajiKawalan2, GLGajiKawalan2, GLKeteranganGajiKawalan2, out DeductionStatus, out BalAmountDeduct, compCode, incentifsType);
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

                    Add_To_tbl_SAPPostRef2(db, db2, Month, Year, NegaraID, SyarikatID, WilayahID, LadangID, SAPPostID2, GajiKawalan, GLGajiKawalan, GLKeteranganGajiKawalan, GajiKawalan2, GLGajiKawalan2, GLKeteranganGajiKawalan2, out DeductionStatus, out BalAmountDeduct, compCode, incentifsType);
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
        public void Add_To_tbl_SAPPostRef1(GenSalaryModelHQ db, GenSalaryModelEstate db2, int? Month, int? Year, int? NegaraID, int? SyarikatID, int? WilayahID, int? LadangID, Guid SAPPostID1, out decimal? GajiKawalan, out string GLGajiKawalan, out string GLKeteranganGajiKawalan, out decimal? GajiKawalan2, out string GLGajiKawalan2, out string GLKeteranganGajiKawalan2, out bool Contribution, List<tbl_Pkjmast> Pkjmstlists, string compCode)
        {
            List<tbl_SAPPostDataDetails> tbl_SAPPostDataDetails = new List<tbl_SAPPostDataDetails>();
            int i = 1;
            decimal? Amount = 0;
            string DescActvt = "";
            GajiKawalan = 0;
            GLGajiKawalan = "";
            GLKeteranganGajiKawalan = "";

            GajiKawalan2 = 0;
            GLGajiKawalan2 = "";
            GLKeteranganGajiKawalan2 = "";
            Contribution = false;

            var ScTrans = db2.tbl_Sctran.Where(x => x.fld_Month == Month && x.fld_Year == Year && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID).Select(s => new { s.fld_GL, s.fld_IO, s.fld_Amt, s.fld_KodAktvt, s.fld_Keterangan, s.fld_JnisAktvt, s.fld_SAPType, s.fld_PaySheetID }).ToList();
            //modified by faeza on 01.12.2023 - add fld_KodAktvt.Substring(0, 1) == "2"
            var GetWorkActvt = ScTrans.Where(x => x.fld_KodAktvt.Substring(0, 1) == "0" || x.fld_KodAktvt.Substring(0, 1) == "1" || x.fld_KodAktvt.Substring(0, 1) == "2").Select(s => new { s.fld_GL, s.fld_IO, s.fld_Amt, s.fld_KodAktvt, s.fld_SAPType, s.fld_PaySheetID }).ToList();
            var GetWorkActvtDistincts = GetWorkActvt.Select(s => new { s.fld_GL, s.fld_IO, s.fld_KodAktvt, s.fld_SAPType, s.fld_PaySheetID }).Distinct().ToList();
            var GetEstateCOde = db.tbl_Ladang.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WlyhID == WilayahID && x.fld_ID == LadangID).Select(s => s.fld_LdgCode).FirstOrDefault();

            foreach (var GetWorkActvtDistinctsTKT in GetWorkActvtDistincts.Where(x => x.fld_PaySheetID == "PT").ToList())
            {
                DescActvt = "3" + GetEstateCOde + "- GAJI BURUH TKT BERHASIL" + "(" + GetEstateCOde + ") " + Month + "/" + Year;
                //modified by kamalia 8/12/2021
                Amount = GetWorkActvt.Where(x => x.fld_GL == GetWorkActvtDistinctsTKT.fld_GL && x.fld_IO == GetWorkActvtDistinctsTKT.fld_IO && x.fld_KodAktvt == GetWorkActvtDistinctsTKT.fld_KodAktvt && x.fld_SAPType == GetWorkActvtDistinctsTKT.fld_SAPType && x.fld_PaySheetID == GetWorkActvtDistinctsTKT.fld_PaySheetID).Sum(s => s.fld_Amt);//end
                if (Amount != 0)
                {
                    tbl_SAPPostDataDetails.Add(new tbl_SAPPostDataDetails() { fld_Amount = Amount, fld_Currency = "RM", fld_Desc = DescActvt, fld_GL = GetWorkActvtDistinctsTKT.fld_GL, fld_IO = GetWorkActvtDistinctsTKT.fld_IO, fld_ItemNo = i, fld_Purpose = "1", fld_SAPActivityCode = GetWorkActvtDistinctsTKT.fld_KodAktvt, fld_SAPPostRefID = SAPPostID1, fld_flag = 0, fld_SAPType = GetWorkActvtDistinctsTKT.fld_SAPType });
                    i++;
                }
            }

            foreach (var GetWorkActvtDistinctsTKT in GetWorkActvtDistincts.Where(x => x.fld_PaySheetID == "PA").ToList())
            {
                DescActvt = "3" + GetEstateCOde + "- GAJI BURUH TKA BERHASIL" + "(" + GetEstateCOde + ") " + Month + "/" + Year;
                //modified by kamalia 8/12/2021
                Amount = GetWorkActvt.Where(x => x.fld_GL == GetWorkActvtDistinctsTKT.fld_GL && x.fld_IO == GetWorkActvtDistinctsTKT.fld_IO && x.fld_KodAktvt == GetWorkActvtDistinctsTKT.fld_KodAktvt && x.fld_SAPType == GetWorkActvtDistinctsTKT.fld_SAPType && x.fld_PaySheetID == GetWorkActvtDistinctsTKT.fld_PaySheetID).Sum(s => s.fld_Amt);//end
                if (Amount != 0)
                {
                    tbl_SAPPostDataDetails.Add(new tbl_SAPPostDataDetails() { fld_Amount = Amount, fld_Currency = "RM", fld_Desc = DescActvt, fld_GL = GetWorkActvtDistinctsTKT.fld_GL, fld_IO = GetWorkActvtDistinctsTKT.fld_IO, fld_ItemNo = i, fld_Purpose = "1", fld_SAPActivityCode = GetWorkActvtDistinctsTKT.fld_KodAktvt, fld_SAPPostRefID = SAPPostID1, fld_flag = 0, fld_SAPType = GetWorkActvtDistinctsTKT.fld_SAPType });
                    i++;
                }
            }

            var GetNotWorkActs = db.tbl_CustomerVendorGLMap.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_TypeCode == "GL" && x.fld_compcode == compCode).ToList();

            //Kira Byar Cuti flag = 2

            var GetNotWorkAct3s = GetNotWorkActs.Where(x => x.fld_Flag == "2" && x.fld_TypeCode == "GL").Select(s => s.fld_KodAktiviti).Distinct().ToList();
            var CheckNotWorkAct3 = ScTrans.Where(x => GetNotWorkAct3s.Contains(x.fld_KodAktvt)).Select(s => new { s.fld_GL, s.fld_IO, s.fld_KodAktvt, s.fld_SAPType, s.fld_PaySheetID }).Distinct().ToList();
            if (CheckNotWorkAct3.Count > 0)
            {
                foreach (var CheckNotWorkAct3Data in CheckNotWorkAct3)
                {
                    var paySheetId = CheckNotWorkAct3Data.fld_PaySheetID == "PT" ? "TKT" : "TKA";

                    //modified by kamalia 28/8/2021
                    DescActvt = ScTrans.Where(x => x.fld_GL == CheckNotWorkAct3Data.fld_GL && x.fld_IO == CheckNotWorkAct3Data.fld_IO && x.fld_KodAktvt == CheckNotWorkAct3Data.fld_KodAktvt && x.fld_SAPType == CheckNotWorkAct3Data.fld_SAPType && x.fld_PaySheetID == CheckNotWorkAct3Data.fld_PaySheetID).Select(s => s.fld_Keterangan).FirstOrDefault() + " " + paySheetId + " (" + GetEstateCOde + ") " + Month + "/" + Year;
                    Amount = ScTrans.Where(x => x.fld_GL == CheckNotWorkAct3Data.fld_GL && x.fld_IO == CheckNotWorkAct3Data.fld_IO && x.fld_KodAktvt == CheckNotWorkAct3Data.fld_KodAktvt && x.fld_SAPType == CheckNotWorkAct3Data.fld_SAPType && x.fld_PaySheetID == CheckNotWorkAct3Data.fld_PaySheetID).Sum(s => s.fld_Amt);
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
            var CheckNotWorkAct2 = ScTrans.Where(x => GetNotWorkAct2s.Contains(x.fld_KodAktvt)).Select(s => new { s.fld_GL, s.fld_IO, s.fld_SAPType, s.fld_PaySheetID }).Distinct().ToList();
            //var CheckNotWorkAct2 = ScTrans.Where(x => GetNotWorkAct2s.Contains(x.fld_KodAktvt)).Select(s => new { s.fld_GL, s.fld_IO, s.fld_KodAktvt, s.fld_SAPType, s.fld_PaySheetID }).Distinct().ToList();
            if (CheckNotWorkAct2.Count > 0)
            {
                foreach (var CheckNotWorkAct2Data in CheckNotWorkAct2)
                {
                    var paySheetId = CheckNotWorkAct2Data.fld_PaySheetID == "PT" ? "TKT" : "TKA";
                    DescActvt = "Caruman KWSP/SOCSO/SIP/SBKP " + paySheetId + " (" + GetEstateCOde + ") " + Month + "/" + Year; //ScTrans.Where(x => x.fld_GL == CheckNotWorkAct2Data.fld_GL && x.fld_IO == CheckNotWorkAct2Data.fld_IO && x.fld_SAPType == CheckNotWorkAct2Data.fld_SAPType && x.fld_PaySheetID == CheckNotWorkAct2Data.fld_PaySheetID).Select(s => s.fld_Keterangan).FirstOrDefault() + " " + paySheetId + " (" + GetEstateCOde + ") " + Month + "/" + Year;
                    Amount = ScTrans.Where(x => x.fld_GL == CheckNotWorkAct2Data.fld_GL && x.fld_IO == CheckNotWorkAct2Data.fld_IO && x.fld_SAPType == CheckNotWorkAct2Data.fld_SAPType && x.fld_PaySheetID == CheckNotWorkAct2Data.fld_PaySheetID && GetNotWorkAct2s.Contains(x.fld_KodAktvt)).Sum(s => s.fld_Amt);
                    //DescActvt = ScTrans.Where(x => x.fld_GL == CheckNotWorkAct2Data.fld_GL && x.fld_IO == CheckNotWorkAct2Data.fld_IO && x.fld_KodAktvt == CheckNotWorkAct2Data.fld_KodAktvt && x.fld_SAPType == CheckNotWorkAct2Data.fld_SAPType && x.fld_PaySheetID == CheckNotWorkAct2Data.fld_PaySheetID).Select(s => s.fld_Keterangan).FirstOrDefault() + " " + paySheetId + " (" + GetEstateCOde + ") " + Month + "/" + Year;
                    //Amount = ScTrans.Where(x => x.fld_GL == CheckNotWorkAct2Data.fld_GL && x.fld_IO == CheckNotWorkAct2Data.fld_IO && x.fld_KodAktvt == CheckNotWorkAct2Data.fld_KodAktvt && x.fld_SAPType == CheckNotWorkAct2Data.fld_SAPType && x.fld_PaySheetID == CheckNotWorkAct2Data.fld_PaySheetID).Sum(s => s.fld_Amt);
                    if (Amount != 0)
                    {
                        tbl_SAPPostDataDetails.Add(new tbl_SAPPostDataDetails() { fld_Amount = Amount, fld_Currency = "RM", fld_Desc = DescActvt.ToUpper(), fld_GL = CheckNotWorkAct2Data.fld_GL, fld_IO = CheckNotWorkAct2Data.fld_IO, fld_ItemNo = i, fld_Purpose = "1", fld_SAPActivityCode = "9999", fld_SAPPostRefID = SAPPostID1, fld_flag = 0, fld_SAPType = CheckNotWorkAct2Data.fld_SAPType });
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
                    var paySheetId = GetNotWorkAct7.fld_PaySheetID == "PT" ? "TKT" : "TKA";
                    DescActvt = ScTrans.Where(x => x.fld_KodAktvt == GetNotWorkAct7.fld_KodAktvt && x.fld_GL == GetNotWorkAct7.fld_GL && x.fld_SAPType == GetNotWorkAct7.fld_SAPType && x.fld_PaySheetID == GetNotWorkAct7.fld_PaySheetID).Select(s => s.fld_Keterangan).FirstOrDefault() + " " + paySheetId + " (" + GetEstateCOde + ") " + Month + "/" + Year;
                    Amount = ScTrans.Where(x => x.fld_KodAktvt == GetNotWorkAct7.fld_KodAktvt && x.fld_GL == GetNotWorkAct7.fld_GL && x.fld_IO == GetNotWorkAct7.fld_IO && x.fld_SAPType == GetNotWorkAct7.fld_SAPType && x.fld_PaySheetID == GetNotWorkAct7.fld_PaySheetID).Sum(s => s.fld_Amt);
                    if (Amount != 0)
                    {
                        tbl_SAPPostDataDetails.Add(new tbl_SAPPostDataDetails() { fld_Amount = Amount, fld_Currency = "RM", fld_Desc = DescActvt.ToUpper(), fld_GL = GetNotWorkAct7.fld_GL, fld_IO = GetNotWorkAct7.fld_IO, fld_ItemNo = i, fld_Purpose = "1", fld_SAPActivityCode = "3900", fld_SAPPostRefID = SAPPostID1, fld_flag = 0, fld_SAPType = GetNotWorkAct7.fld_SAPType });
                        i++;

                    }
                }
            }
            //Kira Tolakkan flag=5
            var GetNotWorkAct6s = GetNotWorkActs.Where(x => x.fld_Flag == "5" && (x.fld_TypeCode == "GL")).Select(s => s.fld_KodAktiviti).ToList();
            var CheckNotWorkAct6 = ScTrans.Where(x => GetNotWorkAct6s.Contains(x.fld_KodAktvt)).Select(s => new { s.fld_KodAktvt, s.fld_GL, s.fld_IO, s.fld_SAPType, s.fld_PaySheetID }).Distinct().ToList();
            if (CheckNotWorkAct6.Count > 0)
            {
                foreach (var CheckNotWorkAct6Data in CheckNotWorkAct6)
                {
                    //Modified by kamalia 14/9/2021
                    var paySheetId = CheckNotWorkAct6Data.fld_PaySheetID == "PT" ? "TKT" : "TKA";
                    DescActvt = ScTrans.Where(x => x.fld_GL == CheckNotWorkAct6Data.fld_GL && x.fld_IO == CheckNotWorkAct6Data.fld_IO && x.fld_KodAktvt == CheckNotWorkAct6Data.fld_KodAktvt && x.fld_SAPType == CheckNotWorkAct6Data.fld_SAPType && x.fld_PaySheetID == CheckNotWorkAct6Data.fld_PaySheetID).Select(s => s.fld_Keterangan).FirstOrDefault() + " " + paySheetId + " (" + GetEstateCOde + ") " + Month + "/" + Year;
                    Amount = ScTrans.Where(x => x.fld_GL == CheckNotWorkAct6Data.fld_GL && x.fld_KodAktvt == CheckNotWorkAct6Data.fld_KodAktvt && x.fld_SAPType == CheckNotWorkAct6Data.fld_SAPType && x.fld_PaySheetID == CheckNotWorkAct6Data.fld_PaySheetID).Sum(s => s.fld_Amt);
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
                    var paySheetId = GetNotWorkAct5.fld_PaySheetID == "PT" ? "TKT" : "TKA";
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
                Amount = ScTrans.Where(x => GetNotWorkAct4s.Contains(x.fld_KodAktvt) && x.fld_PaySheetID == "PT").Sum(s => s.fld_Amt);
                DescActvt = "CLEARING" + " TKT (" + GetEstateCOde + ") " + Month + "/" + Year;
                if (Amount != 0)
                {
                    //modified by kamalia 24/2/22
                    var GLNo = db.tbl_CustomerVendorGLMap.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_Flag == "4" && x.fld_TypeCode == "GL" && x.fld_compcode == compCode && x.fld_Paysheet == "PT").Select(s => s.fld_SAPCode).FirstOrDefault();
                    tbl_SAPPostDataDetails.Add(new tbl_SAPPostDataDetails() { fld_Amount = -Amount, fld_Currency = "RM", fld_Desc = DescActvt.ToUpper(), fld_GL = GLNo, fld_ItemNo = i, fld_Purpose = "1", fld_SAPActivityCode = null, fld_SAPPostRefID = SAPPostID1, fld_flag = 0 });
                    i++;

                    GajiKawalan = Amount;
                    GLGajiKawalan = GLNo;
                    GLKeteranganGajiKawalan = DescActvt;
                }

                Amount = ScTrans.Where(x => GetNotWorkAct4s.Contains(x.fld_KodAktvt) && x.fld_PaySheetID == "PA").Sum(s => s.fld_Amt);
                DescActvt = "CLEARING" + " TKA (" + GetEstateCOde + ") " + Month + "/" + Year;
                if (Amount != 0)
                {
                    //modified by kamalia 24/2/22
                    var GLNo = db.tbl_CustomerVendorGLMap.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_Flag == "4" && x.fld_TypeCode == "GL" && x.fld_compcode == compCode && x.fld_Paysheet == "PT").Select(s => s.fld_SAPCode).FirstOrDefault();
                    tbl_SAPPostDataDetails.Add(new tbl_SAPPostDataDetails() { fld_Amount = -Amount, fld_Currency = "RM", fld_Desc = DescActvt.ToUpper(), fld_GL = GLNo, fld_ItemNo = i, fld_Purpose = "1", fld_SAPActivityCode = null, fld_SAPPostRefID = SAPPostID1, fld_flag = 0 });
                    i++;

                    GajiKawalan2 = Amount;
                    GLGajiKawalan2 = GLNo;
                    GLKeteranganGajiKawalan2 = DescActvt;
                }
            }

            if (tbl_SAPPostDataDetails.Count > 0)
            {
                db2.tbl_SAPPostDataDetails.AddRange(tbl_SAPPostDataDetails);
                db2.SaveChanges();
            }
        }

        public void GetBackRequiredData(GenSalaryModelHQ db, GenSalaryModelEstate db2, int? Month, int? Year, int? NegaraID, int? SyarikatID, int? WilayahID, int? LadangID, out decimal? GajiKawalan, out string GLGajiKawalan, out string GLKeteranganGajiKawalan, out decimal? GajiKawalan2, out string GLGajiKawalan2, out string GLKeteranganGajiKawalan2, string compCode)
        {
            decimal? Amount = 0;
            string DescActvt = "";
            GajiKawalan = 0;
            GLGajiKawalan = "";
            GLKeteranganGajiKawalan = "";
            GajiKawalan2 = 0;
            GLGajiKawalan2 = "";
            GLKeteranganGajiKawalan2 = "";

            var GetEstateCOde = db.tbl_Ladang.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WlyhID == WilayahID && x.fld_ID == LadangID).Select(s => s.fld_LdgCode).FirstOrDefault();
            var GetNotWorkActs = db.tbl_CustomerVendorGLMap.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_TypeCode == "GL" && x.fld_compcode == compCode).ToList();
            var ScTrans = db2.tbl_Sctran.Where(x => x.fld_Month == Month && x.fld_Year == Year && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID).Select(s => new { s.fld_GL, s.fld_IO, s.fld_Amt, s.fld_KodAktvt, s.fld_Keterangan, s.fld_JnisAktvt, s.fld_SAPType, s.fld_PaySheetID }).ToList();
            //modified by faeza on 01.12.2023 - add fld_KodAktvt.Substring(0, 1) == "2"
            var GetWorkActvt = ScTrans.Where(x => x.fld_KodAktvt.Substring(0, 1) == "0" || x.fld_KodAktvt.Substring(0, 1) == "1" || x.fld_KodAktvt.Substring(0, 1) == "2").Select(s => new { s.fld_GL, s.fld_IO, s.fld_Amt, s.fld_KodAktvt, s.fld_SAPType }).ToList();

            var GetNotWorkAct4s = GetNotWorkActs.Where(x => x.fld_Flag == "3" && x.fld_TypeCode == "GL").Select(s => s.fld_KodAktiviti).Distinct().ToList();
            var CheckNotWorkAct4 = ScTrans.Where(x => GetNotWorkAct4s.Contains(x.fld_KodAktvt)).ToList();
            if (CheckNotWorkAct4.Count > 0)
            {
                Amount = ScTrans.Where(x => GetNotWorkAct4s.Contains(x.fld_KodAktvt) && x.fld_PaySheetID == "PT").Sum(s => s.fld_Amt);
                DescActvt = "CLEARING" + " TKT (" + GetEstateCOde + ") " + Month + "/" + Year;
                if (Amount != 0)
                {
                    var GLNo = GetNotWorkActs.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_Flag == "4" && x.fld_TypeCode == "GL" && x.fld_Paysheet == "PT").Select(s => s.fld_SAPCode).FirstOrDefault();
                    GajiKawalan = Amount;
                    GLGajiKawalan = GLNo;
                    GLKeteranganGajiKawalan = DescActvt;
                }

                Amount = ScTrans.Where(x => GetNotWorkAct4s.Contains(x.fld_KodAktvt) && x.fld_PaySheetID == "PA").Sum(s => s.fld_Amt);
                DescActvt = "CLEARING" + " TKA (" + GetEstateCOde + ") " + Month + "/" + Year;
                if (Amount != 0)
                {
                    var GLNo = GetNotWorkActs.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_Flag == "4" && x.fld_TypeCode == "GL" && x.fld_Paysheet == "PA").Select(s => s.fld_SAPCode).FirstOrDefault();
                    GajiKawalan2 = Amount;
                    GLGajiKawalan2 = GLNo;
                    GLKeteranganGajiKawalan2 = DescActvt;
                }
            }
        }

        //modified whole function by kamalia 15/2/22
        public void Add_To_tbl_SAPPostRef2(GenSalaryModelHQ db, GenSalaryModelEstate db2, int? Month, int? Year, int? NegaraID, int? SyarikatID, int? WilayahID, int? LadangID, Guid SAPPostID2, decimal? GajiKawalan, string GLGajiKawalan, string GLKeteranganGajiKawalan, decimal? GajiKawalan2, string GLGajiKawalan2, string GLKeteranganGajiKawalan2, out bool DeductionStatus, out decimal? BalAmountDeduct, string compCode, List<tbl_JenisInsentif> incentifsType)
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

            var GetNotWorkActs = db.tbl_CustomerVendorGLMap.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_TypeCode == "GL" && x.fld_compcode == compCode).ToList();
            var GetNotWorkCodeAct1s = GetNotWorkActs.Where(x => x.fld_Flag == "3" && x.fld_KodAktiviti != "3802").Select(s => s.fld_KodAktiviti).Distinct().ToArray();
            var ScTrans = db2.tbl_Sctran.Where(x => x.fld_Month == Month && x.fld_Year == Year && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID).ToList();

            var GLClearing = db.tbl_CustomerVendorGLMap.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && (x.fld_Flag == "3" || x.fld_Flag == "4") && x.fld_TypeCode == "GL" && x.fld_compcode == compCode).Select(s => new { s.fld_SAPCode, s.fld_KodAktiviti, s.fld_Paysheet }).ToList();
            var vendorList = db.tbl_VDSAP.Where(x => x.fld_CompanyCode == compCode && x.fld_Deleted == false).ToList();

            var descActivity = "";
            var totalAmount = 0M;
            //var gL = "";

            #region clearing batch 1 - M2E

            flag = 1;

            var amountM2EPT = 0M;
            var scTransM2EPT = ScTrans.Where(x => x.fld_Kategori == 16 && x.fld_PaySheetID == "PT").FirstOrDefault();
            if (scTransM2EPT != null)
            {
                amountM2EPT = scTransM2EPT.fld_Amt.Value;
                descActivity = scTransM2EPT.fld_Keterangan + " (" + GetEstateCOde + ") " + Month + "/" + Year;
                var sapClearing = GLClearing.Where(x => x.fld_KodAktiviti == scTransM2EPT.fld_KodAktvt && x.fld_Paysheet == "PT").FirstOrDefault();
                tbl_SAPPostDataDetails.Add(new tbl_SAPPostDataDetails() { fld_Amount = amountM2EPT, fld_Currency = "RM", fld_Desc = descActivity.ToUpper(), fld_GL = sapClearing.fld_SAPCode, fld_ItemNo = i, fld_Purpose = "2", fld_SAPActivityCode = sapClearing.fld_KodAktiviti, fld_SAPPostRefID = SAPPostID2, fld_flag = flag });
            }

            var amountM2EPA = 0M;
            var scTransM2EPA = ScTrans.Where(x => x.fld_Kategori == 16 && x.fld_PaySheetID == "PA").FirstOrDefault();
            if (scTransM2EPA != null)
            {
                i++;
                amountM2EPA = scTransM2EPA.fld_Amt.Value;
                descActivity = scTransM2EPA.fld_Keterangan + " (" + GetEstateCOde + ") " + Month + "/" + Year;
                var sapClearing = GLClearing.Where(x => x.fld_KodAktiviti == scTransM2EPA.fld_KodAktvt && x.fld_Paysheet == "PA").FirstOrDefault();
                tbl_SAPPostDataDetails.Add(new tbl_SAPPostDataDetails() { fld_Amount = amountM2EPA, fld_Currency = "RM", fld_Desc = descActivity.ToUpper(), fld_GL = sapClearing.fld_SAPCode, fld_ItemNo = i, fld_Purpose = "2", fld_SAPActivityCode = sapClearing.fld_KodAktiviti, fld_SAPPostRefID = SAPPostID2, fld_flag = flag });
            }

            if (amountM2EPT + amountM2EPA > 0)
            {
                i++;
                var vendor = vendorList.Where(x => x.fld_VendorInd == "M2E").FirstOrDefault();
                totalAmount = amountM2EPT + amountM2EPA;
                tbl_SAPPostDataDetails.Add(new tbl_SAPPostDataDetails() { fld_Amount = -totalAmount, fld_Currency = "RM", fld_Desc = vendor.fld_Desc.ToUpper() + " (" + GetEstateCOde + ") " + Month + "/" + Year, fld_GL = null, fld_ItemNo = i, fld_Purpose = "2", fld_SAPActivityCode = "", fld_SAPPostRefID = SAPPostID2, fld_flag = flag, fld_VendorCode = vendor.fld_VendorNo });
                flag++;
            }

            #endregion clearing batch 1 - M2E

            #region clearing batch 2 - MERCHANTRADE

            var amountMERCHANTRADEPT = 0M;
            var scTranMERCHANTRADEPT = ScTrans.Where(x => x.fld_Kategori == 13 && x.fld_PaySheetID == "PT").FirstOrDefault();
            if (scTranMERCHANTRADEPT != null)
            {
                i++;
                amountMERCHANTRADEPT = scTranMERCHANTRADEPT.fld_Amt.Value;
                descActivity = scTranMERCHANTRADEPT.fld_Keterangan + " (" + GetEstateCOde + ") " + Month + "/" + Year;
                var sapClearing = GLClearing.Where(x => x.fld_KodAktiviti == scTranMERCHANTRADEPT.fld_KodAktvt && x.fld_Paysheet == "PT").FirstOrDefault();
                tbl_SAPPostDataDetails.Add(new tbl_SAPPostDataDetails() { fld_Amount = amountMERCHANTRADEPT, fld_Currency = "RM", fld_Desc = descActivity.ToUpper(), fld_GL = sapClearing.fld_SAPCode, fld_ItemNo = i, fld_Purpose = "2", fld_SAPActivityCode = sapClearing.fld_KodAktiviti, fld_SAPPostRefID = SAPPostID2, fld_flag = flag });
            }

            var amountMERCHANTRADEPA = 0M;
            var scTranMERCHANTRADEPA = ScTrans.Where(x => x.fld_Kategori == 13 && x.fld_PaySheetID == "PA").FirstOrDefault();
            if (scTranMERCHANTRADEPA != null)
            {
                i++;
                amountMERCHANTRADEPA = scTranMERCHANTRADEPA.fld_Amt.Value;
                descActivity = scTranMERCHANTRADEPA.fld_Keterangan + " (" + GetEstateCOde + ") " + Month + "/" + Year;
                var sapClearing = GLClearing.Where(x => x.fld_KodAktiviti == scTranMERCHANTRADEPA.fld_KodAktvt && x.fld_Paysheet == "PA").FirstOrDefault();
                tbl_SAPPostDataDetails.Add(new tbl_SAPPostDataDetails() { fld_Amount = amountMERCHANTRADEPA, fld_Currency = "RM", fld_Desc = descActivity.ToUpper(), fld_GL = sapClearing.fld_SAPCode, fld_ItemNo = i, fld_Purpose = "2", fld_SAPActivityCode = sapClearing.fld_KodAktiviti, fld_SAPPostRefID = SAPPostID2, fld_flag = flag });
            }

            if (amountMERCHANTRADEPT + amountMERCHANTRADEPA > 0)
            {
                i++;
                var vendor = vendorList.Where(x => x.fld_VendorInd == "MTA").FirstOrDefault();
                totalAmount = amountMERCHANTRADEPT + amountMERCHANTRADEPA;
                tbl_SAPPostDataDetails.Add(new tbl_SAPPostDataDetails() { fld_Amount = -totalAmount, fld_Currency = "RM", fld_Desc = vendor.fld_Desc.ToUpper() + " (" + GetEstateCOde + ") " + Month + "/" + Year, fld_GL = null, fld_ItemNo = i, fld_Purpose = "2", fld_SAPActivityCode = "", fld_SAPPostRefID = SAPPostID2, fld_flag = flag, fld_VendorCode = vendor.fld_VendorNo });
                flag++;
            }

            #endregion clearing batch 2 - MERCHANTRADE

            #region clearing batch 3 - ESTATE

            //Contribution SIP,SBKP,SOCSO,KWSP 
            //3811 - SBKP(Pekerja Dan Majikan)
            //3808 - SIP(Pekerja Dan Majikan)
            //3805 - Socso(Pekerja Dan Majikan)
            //3802 - KWSP(Pekerja Dan Majikan)
            string[] contribution = new string[] { "3811", "3808", "3805", "3802" };

            var scTranContributions = ScTrans.Where(x => contribution.Contains(x.fld_KodAktvt)).Select(s => new { s.fld_Keterangan, s.fld_GL, s.fld_KodAktvt }).Distinct().ToList();

            var amountContribution = 0M;

            foreach (var scTranContribution in scTranContributions)
            {
                i++;
                var amount = ScTrans.Where(x => x.fld_KodAktvt == scTranContribution.fld_KodAktvt && x.fld_GL == scTranContribution.fld_GL).Sum(s => s.fld_Amt);
                descActivity = scTranContribution.fld_Keterangan + " (" + GetEstateCOde + ") " + Month + "/" + Year;
                tbl_SAPPostDataDetails.Add(new tbl_SAPPostDataDetails() { fld_Amount = amount, fld_Currency = "RM", fld_Desc = descActivity.ToUpper(), fld_GL = scTranContribution.fld_GL, fld_ItemNo = i, fld_Purpose = "2", fld_SAPActivityCode = scTranContribution.fld_KodAktvt, fld_SAPPostRefID = SAPPostID2, fld_flag = flag });
                amountContribution += amount.Value;
            }

            //Deduction Water & Electric
            //3502 - POTONGAN BEKALAN AIR
            //3503 - POTONGAN BEKALAN ELEKTRIK
            //3505 - POTONGAN PENDAHULUAN GAJI
            //3510 - POTONGAN AIPS 
            //3504 - POTONGAN RAWATAN PERUBATAN
            //3506 - POTONGAN RAWATAN PERUBATAN
            //3507 - Potongan Pendahuluan Cuti Tahunan
            //3508 - Potongan Terlebih Sip/sbkp/socso/kwsp
            //3509 - Potongan Terlebih Bayar Gaji


            string[] deduction = incentifsType.Where(x => !contribution.Contains(x.fld_KodAktvt)).Select(s => s.fld_KodAktvt).Distinct().ToArray();

            var scTranDeductions = ScTrans.Where(x => deduction.Contains(x.fld_KodAktvt)).Select(s => new { s.fld_Keterangan, s.fld_GL, s.fld_KodAktvt }).Distinct().ToList();

            var amountDeduction = 0M;

            foreach (var scTranDeduction in scTranDeductions)
            {
                i++;
                //var amount = ScTrans.Where(x => x.fld_KodAktvt == scTranDeduction.fld_KodAktvt).Sum(s => s.fld_Amt);
                //modified by faeza 03.01.2024
                var amount = ScTrans.Where(x => x.fld_KodAktvt == scTranDeduction.fld_KodAktvt && x.fld_GL == scTranDeduction.fld_GL).Sum(s => s.fld_Amt);
                descActivity = scTranDeduction.fld_Keterangan + " (" + GetEstateCOde + ") " + Month + "/" + Year;
                tbl_SAPPostDataDetails.Add(new tbl_SAPPostDataDetails() { fld_Amount = amount, fld_Currency = "RM", fld_Desc = descActivity.ToUpper(), fld_GL = scTranDeduction.fld_GL, fld_ItemNo = i, fld_Purpose = "2", fld_SAPActivityCode = scTranDeduction.fld_KodAktvt, fld_SAPPostRefID = SAPPostID2, fld_flag = flag });
                amountDeduction += amount.Value;
            }

            //Salary
            //4000   

            string[] salary = new string[] { "4000" };
            byte?[] category = new byte?[] { 11, 12, 14, 15 };

            var scTranSalaries = ScTrans.Where(x => salary.Contains(x.fld_KodAktvt) && category.Contains(x.fld_Kategori)).Select(s => new { s.fld_Keterangan, s.fld_GL, s.fld_KodAktvt, s.fld_Kategori }).Distinct().ToList();

            var amountSalary = 0M;

            foreach (var scTranSalary in scTranSalaries)
            {
                i++;
                var amount = ScTrans.Where(x => x.fld_KodAktvt == scTranSalary.fld_KodAktvt && x.fld_Kategori == scTranSalary.fld_Kategori).Sum(s => s.fld_Amt);
                descActivity = scTranSalary.fld_Keterangan + " (" + GetEstateCOde + ") " + Month + "/" + Year;
                tbl_SAPPostDataDetails.Add(new tbl_SAPPostDataDetails() { fld_Amount = amount, fld_Currency = "RM", fld_Desc = descActivity.ToUpper(), fld_GL = scTranSalary.fld_GL, fld_ItemNo = i, fld_Purpose = "2", fld_SAPActivityCode = scTranSalary.fld_KodAktvt, fld_SAPPostRefID = SAPPostID2, fld_flag = flag });
                amountSalary += amount.Value;
            }

            totalAmount = amountContribution + amountDeduction + amountSalary;
            if (totalAmount > 0)
            {
                i++;
                var vendorNo = db.tbl_Ladang.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WlyhID == WilayahID && x.fld_ID == LadangID).Select(s => s.fld_CustCPD).FirstOrDefault();
                descActivity = "FELDA TKT " + NamaLadang + " (" + GetEstateCOde + ") " + Month + "/" + Year; ;
                tbl_SAPPostDataDetails.Add(new tbl_SAPPostDataDetails() { fld_Amount = -totalAmount, fld_Currency = "RM", fld_Desc = descActivity.ToUpper(), fld_GL = null, fld_ItemNo = i, fld_Purpose = "2", fld_SAPActivityCode = "-", fld_SAPPostRefID = SAPPostID2, fld_flag = flag, fld_VendorCode = vendorNo });
            }

            #endregion clearing batch 2 - ESTATE

            if (GajiKawalan - Amount > 0)
            {
                BalAmountDeduct = GajiKawalan - Amount;
                DeductionStatus = true;
            }
            db2.tbl_SAPPostDataDetails.AddRange(tbl_SAPPostDataDetails);
            db2.SaveChanges();
            //clearing batch 1 -socso sip sbkp
        }

        public static DateTime GetLastDayOfMonth(int? Month, int? Year)
        {
            return new DateTime(Year.Value, Month.Value, DateTime.DaysInMonth(Year.Value, Month.Value));
        }
    }
}
