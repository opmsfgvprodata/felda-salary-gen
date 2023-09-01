using SalaryGeneratorServices.CustomModels;
using SalaryGeneratorServices.ModelsEstate;
using SalaryGeneratorServices.ModelsHQ;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace SalaryGeneratorServices.FuncClass
{
    class Step4Func
    {
        private DateTimeFunc DateTimeFunc = new DateTimeFunc();
        public List<CustMod_WorkSCTrans> GetWorkActvtyPktFunc(int? NegaraID, int? SyarikatID, int? WilayahID, int? LadangID, int? UserID, DateTime DTProcess, int? Month, int? Year, string processname, string servicesname, int? ClientID, List<tbl_Pkjmast> PkjMastList, string compCode)
        {
            GenSalaryModelHQ db = new GenSalaryModelHQ();
            GetConnectFunc conn = new GetConnectFunc();
            int ID = 1;
            List<CustMod_WorkSCTrans> WorkSCTransList = new List<CustMod_WorkSCTrans>();
            string host, catalog, user, pass = "";
            string keterangan = "";
            conn.GetConnection(out host, out catalog, out user, out pass, WilayahID, SyarikatID, NegaraID);
            GenSalaryModelEstate db2 = GenSalaryModelEstate.ConnectToSqlServer(host, catalog, user, pass);
            var NoPkjList = PkjMastList.Select(s => s.fld_Nopkj).ToArray();

            var vw_KerjaInfoDetails = db2.vw_KerjaDetailScTrans.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Tarikh.Value.Month == Month && x.fld_Tarikh.Value.Year == Year && x.fld_KodAktvt != null).ToList();
            var WorkDistincts = vw_KerjaInfoDetails.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Tarikh.Value.Month == Month && x.fld_Tarikh.Value.Year == Year && x.fld_KodAktvt != null && NoPkjList.Contains(x.fld_Nopkj)).Select(s => new { s.fld_JnisAktvt, s.fld_KodAktvt, s.fld_JnsPkt, s.fld_KodPkt, s.fld_KodGL, s.fld_IOKod, s.fld_GLKod, s.fld_SAPType }).Distinct().ToList();

            foreach (var WorkDistinct in WorkDistincts)
            {
                keterangan = db.tbl_UpahAktiviti.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_KodJenisAktvt == WorkDistinct.fld_JnisAktvt && x.fld_KodAktvt == WorkDistinct.fld_KodAktvt && x.fld_compcode == compCode).Select(s => s.fld_Desc).FirstOrDefault();
                var sapType = string.IsNullOrEmpty(WorkDistinct.fld_SAPType) ? "IO" : WorkDistinct.fld_SAPType;
                WorkSCTransList.Add(new CustMod_WorkSCTrans() { fld_ID = ID, fld_KodGL = WorkDistinct.fld_KodGL, fld_JnisAktvt = WorkDistinct.fld_JnisAktvt, fld_KodAktvt = WorkDistinct.fld_KodAktvt, fld_JnsPkt = WorkDistinct.fld_JnsPkt, fld_KodPkt = WorkDistinct.fld_KodPkt, fld_Keterangan = keterangan, fld_SAPGL = WorkDistinct.fld_GLKod, fld_SAPIO = WorkDistinct.fld_IOKod, fld_SAPType = sapType });
                ID++;
            }

            db.Dispose();
            db2.Dispose();

            return WorkSCTransList;
        }

        public void GetAmountWorkActivityFunc(int? NegaraID, int? SyarikatID, int? WilayahID, int? LadangID, int? UserID, DateTime DTProcess, int? Month, int? Year, string processname, string servicesname, int? ClientID, List<CustMod_WorkSCTrans> WorkSCTransList, out string Log, List<tbl_Pkjmast> PkjMastList)
        {
            GetConnectFunc conn = new GetConnectFunc();
            string host, catalog, user, pass = "";
            conn.GetConnection(out host, out catalog, out user, out pass, WilayahID, SyarikatID, NegaraID);
            GenSalaryModelEstate db2 = GenSalaryModelEstate.ConnectToSqlServer(host, catalog, user, pass);
            Log = "";
            string message = "";
            int i = 1;
            var NoPkjList = PkjMastList.Select(s => s.fld_Nopkj.Trim()).ToArray();
            var vw_KerjaInfoDetails = db2.vw_KerjaDetailScTrans.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Tarikh.Value.Month == Month && x.fld_Tarikh.Value.Year == Year && x.fld_OverallAmount != null && NoPkjList.Contains(x.fld_Nopkj.Trim())).ToList();
            foreach (var WorkSCTrans in WorkSCTransList)
            {
                var Amount = vw_KerjaInfoDetails.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Tarikh.Value.Month == Month && x.fld_Tarikh.Value.Year == Year && x.fld_JnsPkt == WorkSCTrans.fld_JnsPkt && x.fld_KodPkt == WorkSCTrans.fld_KodPkt && x.fld_JnisAktvt == WorkSCTrans.fld_JnisAktvt && x.fld_KodAktvt == WorkSCTrans.fld_KodAktvt && x.fld_GLKod == WorkSCTrans.fld_SAPGL && x.fld_IOKod == WorkSCTrans.fld_SAPIO).Sum(s => s.fld_OverallAmount);
                AddTo_tbl_Sctran(db2, NegaraID, SyarikatID, WilayahID, LadangID, Amount, WorkSCTrans.fld_JnsPkt, WorkSCTrans.fld_KodPkt, WorkSCTrans.fld_JnisAktvt, WorkSCTrans.fld_KodAktvt, WorkSCTrans.fld_KodGL, WorkSCTrans.fld_Keterangan, DTProcess, UserID, Month, Year, "D", 1, WorkSCTrans.fld_SAPGL, WorkSCTrans.fld_SAPIO, WorkSCTrans.fld_SAPType);
                message = "Transaction Listing (Job). (Data - Code Pkt : " + WorkSCTrans.fld_KodPkt + ", Code Activity : " + WorkSCTrans.fld_KodAktvt + ", SAP GL : " + WorkSCTrans.fld_SAPGL + ", SAP IO : " + WorkSCTrans.fld_SAPIO + " Amount : RM " + Amount + ")";
                Log += i == 1 ? DateTimeFunc.GetDateTime() + " - " + message : "\r\n" + DateTimeFunc.GetDateTime() + " - " + message;
                i++;
            }
            db2.Dispose();
        }

        public void GetAmountDailyBonusFunc(int? NegaraID, int? SyarikatID, int? WilayahID, int? LadangID, int? UserID, DateTime DTProcess, int? Month, int? Year, string processname, string servicesname, int? ClientID, List<CustMod_WorkSCTrans> WorkSCTransList, out string Log, List<tbl_Pkjmast> PkjMastList)
        {
            GetConnectFunc conn = new GetConnectFunc();
            string host, catalog, user, pass = "";
            conn.GetConnection(out host, out catalog, out user, out pass, WilayahID, SyarikatID, NegaraID);
            GenSalaryModelEstate db2 = GenSalaryModelEstate.ConnectToSqlServer(host, catalog, user, pass);
            Log = "";
            string message = "";
            int i = 1;
            var NoPkjList = PkjMastList.Select(s => s.fld_Nopkj).ToArray();
            foreach (var WorkSCTrans in WorkSCTransList)
            {
                var Amount = db2.vw_Kerja_Bonus.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Tarikh.Value.Month == Month && x.fld_Tarikh.Value.Year == Year && x.fld_JnsPkt == WorkSCTrans.fld_JnsPkt && x.fld_KodPkt == WorkSCTrans.fld_KodPkt && x.fld_JnisAktvt == WorkSCTrans.fld_JnisAktvt && x.fld_KodAktvt == WorkSCTrans.fld_KodAktvt && x.fld_KodGL == WorkSCTrans.fld_KodGL && NoPkjList.Contains(x.fld_Nopkj)).Sum(s => s.fld_Jumlah_B);
                Amount = Amount == null ? 0 : Amount;
                if (Amount != 0)
                {
                    AddTo_tbl_Sctran(db2, NegaraID, SyarikatID, WilayahID, LadangID, Amount, WorkSCTrans.fld_JnsPkt, WorkSCTrans.fld_KodPkt, WorkSCTrans.fld_JnisAktvt, WorkSCTrans.fld_KodAktvt, WorkSCTrans.fld_KodGL, WorkSCTrans.fld_Keterangan + " (Bonus)", DTProcess, UserID, Month, Year, "D", 2, WorkSCTrans.fld_SAPGL, WorkSCTrans.fld_SAPIO, WorkSCTrans.fld_SAPType);
                    message = "Transaction Listing (Daily Bonus). (Data - Code Pkt : " + WorkSCTrans.fld_KodPkt + ", Code Activity : " + WorkSCTrans.fld_KodAktvt + ", Amount : RM " + Amount + ")";
                    Log += i == 1 ? DateTimeFunc.GetDateTime() + " - " + message : "\r\n" + DateTimeFunc.GetDateTime() + " - " + message;
                    i++;
                }
            }
            db2.Dispose();
        }

        public void GetAmountOTFunc(int? NegaraID, int? SyarikatID, int? WilayahID, int? LadangID, int? UserID, DateTime DTProcess, int? Month, int? Year, string processname, string servicesname, int? ClientID, List<CustMod_WorkSCTrans> WorkSCTransList, out string Log, List<tbl_Pkjmast> PkjMastList, string compCode)
        {
            GetConnectFunc conn = new GetConnectFunc();
            string host, catalog, user, pass = "";
            conn.GetConnection(out host, out catalog, out user, out pass, WilayahID, SyarikatID, NegaraID);
            GenSalaryModelHQ db = new GenSalaryModelHQ();
            GenSalaryModelEstate db2 = GenSalaryModelEstate.ConnectToSqlServer(host, catalog, user, pass);
            Log = "";
            string message = "";
            int i = 1;
            var NoPkjList = PkjMastList.Select(s => s.fld_Nopkj).ToArray();

            /*modified by kamalia 4/7/2021
            var vw_KerjaInfoDetails = db2.vw_KerjaDetailScTrans.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Tarikh.Value.Month == Month && x.fld_Tarikh.Value.Year == Year && x.fld_KodAktvt != null && x.fld_JumlahOT != null).ToList();
            var WorkDistincts = vw_KerjaInfoDetails.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Tarikh.Value.Month == Month && x.fld_Tarikh.Value.Year == Year && x.fld_KodAktvt != null && NoPkjList.Contains(x.fld_Nopkj)).Select(s => new { s.fld_JnisAktvt, s.fld_KodAktvt, s.fld_JnsPkt, s.fld_KodPkt, s.fld_KodGL, s.fld_IOKod, s.fld_GLKod }).Distinct().ToList();
            */

            //reverted back for kemahang 4 kamalia - 25/7/2021
            var WorkDistincts = db2.vw_KerjaDetailScTrans.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Tarikh.Value.Month == Month && x.fld_Tarikh.Value.Year == Year && x.fld_KodAktvt != null && NoPkjList.Contains(x.fld_Nopkj) && x.fld_JumlahOT != null).Select(s => new { s.fld_JnisAktvt, s.fld_KodAktvt, s.fld_JnsPkt, s.fld_KodPkt, s.fld_KodGL, s.fld_IOKod, s.fld_GLKod, s.fld_SAPType }).Distinct().ToList();
            int ID = 1;
            List<CustMod_WorkSCTrans> WorkSCTransList2 = new List<CustMod_WorkSCTrans>();
            string keterangan = "";
            foreach (var WorkDistinct in WorkDistincts)
            {
                keterangan = db.tbl_UpahAktiviti.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_KodJenisAktvt == WorkDistinct.fld_JnisAktvt && x.fld_KodAktvt == WorkDistinct.fld_KodAktvt && x.fld_compcode == compCode).Select(s => s.fld_Desc).FirstOrDefault();
                WorkSCTransList2.Add(new CustMod_WorkSCTrans() { fld_ID = ID, fld_KodGL = WorkDistinct.fld_KodGL, fld_JnisAktvt = WorkDistinct.fld_JnisAktvt, fld_KodAktvt = WorkDistinct.fld_KodAktvt, fld_JnsPkt = WorkDistinct.fld_JnsPkt, fld_KodPkt = WorkDistinct.fld_KodPkt, fld_Keterangan = keterangan, fld_SAPGL = WorkDistinct.fld_GLKod, fld_SAPIO = WorkDistinct.fld_IOKod, fld_SAPType = WorkDistinct.fld_SAPType });
                ID++;
            }

            foreach (var WorkSCTrans in WorkSCTransList2)
            {
                var Amount = db2.vw_Kerja_OT.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Tarikh.Value.Month == Month && x.fld_Tarikh.Value.Year == Year && x.fld_JnsPkt == WorkSCTrans.fld_JnsPkt && x.fld_KodPkt == WorkSCTrans.fld_KodPkt && x.fld_JnisAktvt == WorkSCTrans.fld_JnisAktvt && x.fld_KodAktvt == WorkSCTrans.fld_KodAktvt && x.fld_KodGL == WorkSCTrans.fld_KodGL && NoPkjList.Contains(x.fld_Nopkj)).Sum(s => s.fld_Jumlah);
                Amount = Amount == null ? 0 : Amount;
                if (Amount != 0)
                {
                    AddTo_tbl_Sctran(db2, NegaraID, SyarikatID, WilayahID, LadangID, Amount, WorkSCTrans.fld_JnsPkt, WorkSCTrans.fld_KodPkt, WorkSCTrans.fld_JnisAktvt, WorkSCTrans.fld_KodAktvt, WorkSCTrans.fld_KodGL, WorkSCTrans.fld_Keterangan + " (OT)", DTProcess, UserID, Month, Year, "D", 3, WorkSCTrans.fld_SAPGL, WorkSCTrans.fld_SAPIO, WorkSCTrans.fld_SAPType);
                    message = "Transaction Listing (OT). (Data - Code Pkt : " + WorkSCTrans.fld_KodPkt + ", Code Activity : " + WorkSCTrans.fld_KodAktvt + ", Amount : RM " + Amount + ")";
                    Log += i == 1 ? DateTimeFunc.GetDateTime() + " - " + message : "\r\n" + DateTimeFunc.GetDateTime() + " - " + message;
                    i++;
                }
            }
            db2.Dispose();
        }

        public List<CustMod_AdminSCTrans> GetWorkAdminPktFunc(int? NegaraID, int? SyarikatID, int? WilayahID, int? LadangID, int? UserID, DateTime DTProcess, int? Month, int? Year, string processname, string servicesname, int? ClientID, List<tbl_Pkjmast> PkjMastList, string compCode)
        {
            GenSalaryModelHQ db = new GenSalaryModelHQ();
            GetConnectFunc conn = new GetConnectFunc();
            List<CustMod_AdminSCTrans> AdminSCTransList = new List<CustMod_AdminSCTrans>();
            string host, catalog, user, pass = "";
            string GLKod = "";
            conn.GetConnection(out host, out catalog, out user, out pass, WilayahID, SyarikatID, NegaraID);
            GenSalaryModelEstate db2 = GenSalaryModelEstate.ConnectToSqlServer(host, catalog, user, pass);
            var KerjaInfoDetails = db2.vw_KerjaInfoDetails.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Tarikh.Value.Month == Month && x.fld_Tarikh.Value.Year == Year).ToList();
            var vw_KerjaInfoDetails = KerjaInfoDetails.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Tarikh.Value.Month == Month && x.fld_Tarikh.Value.Year == Year && x.fld_KodAktvt != null).ToList();
            var getMainPkts = db2.tbl_PktUtama.Where(x => x.fld_LadangID == LadangID).ToList();
            var getSubPkts = db2.tbl_SubPkt.Where(x => x.fld_LadangID == LadangID).ToList();
            var getBlokPkts = db2.tbl_Blok.Where(x => x.fld_LadangID == LadangID).ToList();
            var WorkDistincts = vw_KerjaInfoDetails.Select(s => new { s.fld_Nopkj, s.fld_KodPkt, s.fld_IOKod, s.fld_PaySheetID, s.fld_SAPType, s.fld_JnsPkt }).Distinct().ToList();

            foreach (var WorkDistinct in WorkDistincts)
            {
                var getLot = "";
                var PktUtama = new tbl_PktUtama();
                switch (WorkDistinct.fld_JnsPkt)
                {
                    case 1:
                        PktUtama = getMainPkts.Where(x => x.fld_PktUtama == WorkDistinct.fld_KodPkt).FirstOrDefault();
                        getLot = PktUtama.fld_JnsLot;
                        break;
                    case 2:
                        var subPkt = getSubPkts.Where(x => x.fld_Pkt == WorkDistinct.fld_KodPkt).FirstOrDefault();
                        PktUtama = getMainPkts.Where(x => x.fld_PktUtama == subPkt.fld_KodPktUtama).FirstOrDefault();
                        getLot = PktUtama.fld_JnsLot;
                        break;
                    case 3:
                        var blokPkt = getBlokPkts.Where(x => x.fld_KodPkt == WorkDistinct.fld_KodPkt).FirstOrDefault();
                        PktUtama = getMainPkts.Where(x => x.fld_PktUtama == blokPkt.fld_KodPktutama).FirstOrDefault();
                        getLot = PktUtama.fld_JnsLot;
                        break;
                }
                var sapType = string.IsNullOrEmpty(WorkDistinct.fld_SAPType) ? "IO" : WorkDistinct.fld_SAPType;
                if (sapType == "CC")
                {
                    GLKod = db.tbl_CustomerVendorGLMap.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_compcode == compCode && x.fld_SAPType == WorkDistinct.fld_SAPType).Select(s => s.fld_SAPCode).FirstOrDefault();
                }
                else
                {
                    GLKod = db.tbl_CustomerVendorGLMap.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_Paysheet == WorkDistinct.fld_PaySheetID && x.fld_JnsLot == getLot && x.fld_compcode == compCode && x.fld_SAPType == null).Select(s => s.fld_SAPCode).FirstOrDefault();
                }
                var totalWorking = vw_KerjaInfoDetails.Where(x => x.fld_Nopkj == WorkDistinct.fld_Nopkj && x.fld_KodPkt == WorkDistinct.fld_KodPkt && x.fld_IOKod == WorkDistinct.fld_IOKod && x.fld_PaySheetID == WorkDistinct.fld_PaySheetID).Count();
                AdminSCTransList.Add(new CustMod_AdminSCTrans() { fld_KodGL = GLKod, fld_KodPkt = WorkDistinct.fld_KodPkt, fld_SAPIO = WorkDistinct.fld_IOKod, fld_PaySheetID = WorkDistinct.fld_PaySheetID, fld_Nopkj = WorkDistinct.fld_Nopkj, fld_TotalWorking = totalWorking, fld_SAPType = sapType });
            }

            var vw_KerjaInfoDetails2 = KerjaInfoDetails.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Tarikh.Value.Month == Month && x.fld_Tarikh.Value.Year == Year && x.fld_KodAktvt == null && !string.IsNullOrEmpty(x.fld_SAPChargeCode)).ToList();
            var WorkDistincts2 = vw_KerjaInfoDetails2.Select(s => new { s.fld_Nopkj, s.fld_SAPChargeCode }).Distinct().ToList();

            foreach (var WorkDistinct in WorkDistincts2)
            {
                var getLot = "";
                var PktUtama = new tbl_PktUtama();
                PktUtama = getMainPkts.Where(x => x.fld_IOcode == WorkDistinct.fld_SAPChargeCode).FirstOrDefault();
                getLot = PktUtama.fld_JnsLot;
                var fld_SAPType = PktUtama.fld_SAPType;

                var sapType = string.IsNullOrEmpty(fld_SAPType) ? "IO" : fld_SAPType;
                var GetPaySheetID = db.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "statusTanaman" && x.fldOptConfValue == PktUtama.fld_StatusTnmn && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fldDeleted == false).Select(s => s.fldOptConfFlag2).FirstOrDefault();
                if (sapType == "CC")
                {
                    GLKod = db.tbl_CustomerVendorGLMap.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_compcode == compCode && x.fld_SAPType == sapType).Select(s => s.fld_SAPCode).FirstOrDefault();
                }
                else
                {
                    GLKod = db.tbl_CustomerVendorGLMap.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_Paysheet == GetPaySheetID && x.fld_JnsLot == getLot && x.fld_compcode == compCode && x.fld_SAPType == null).Select(s => s.fld_SAPCode).FirstOrDefault();
                }
                var totalWorking = vw_KerjaInfoDetails.Where(x => x.fld_Nopkj == WorkDistinct.fld_Nopkj && x.fld_SAPChargeCode == WorkDistinct.fld_SAPChargeCode).Count();
                AdminSCTransList.Add(new CustMod_AdminSCTrans() { fld_KodGL = GLKod, fld_KodPkt = PktUtama.fld_PktUtama, fld_SAPIO = WorkDistinct.fld_SAPChargeCode, fld_PaySheetID = GetPaySheetID, fld_Nopkj = WorkDistinct.fld_Nopkj, fld_TotalWorking = totalWorking, fld_SAPType = sapType });
            }

            db.Dispose();
            db2.Dispose();

            return AdminSCTransList;
        }

        //modified whole function by kamalia 15/2/22
        public void GetAmountLeaveFunc(int? NegaraID, int? SyarikatID, int? WilayahID, int? LadangID, int? UserID, DateTime DTProcess, int? Month, int? Year, string processname, string servicesname, int? ClientID, List<CustMod_AdminSCTrans> AdminSCTransList, out string Log, List<tbl_Pkjmast> PkjMastList)
        {
            GenSalaryModelHQ db = new GenSalaryModelHQ();
            GetConnectFunc conn = new GetConnectFunc();
            string host, catalog, user, pass = "";
            conn.GetConnection(out host, out catalog, out user, out pass, WilayahID, SyarikatID, NegaraID);
            GenSalaryModelEstate db2 = GenSalaryModelEstate.ConnectToSqlServer(host, catalog, user, pass);
            Log = "";
            string message = "";
            int i = 1;
            int j = 0;
            decimal? Amount = 0;

            var GetLeave = db.tbl_CutiKategori.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_Deleted == false).ToList();
            var NoPkjList = AdminSCTransList.Select(s => s.fld_Nopkj).Distinct().ToArray();

            foreach (var Leave in GetLeave)
            {
                foreach (var pkjamount in NoPkjList)
                {
                    Amount = Leave.fld_WaktuBayaranCuti == 1 ? db2.vw_Kerja_Hdr_Cuti.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Tarikh.Value.Month == Month && x.fld_Tarikh.Value.Year == Year && x.fld_Kdhdct == Leave.fld_KodCuti && x.fld_Nopkj == pkjamount).Sum(s => s.fld_Jumlah)
              :
                  db2.tbl_KerjahdrCutiTahunan.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Year == Year && x.fld_KodCuti == Leave.fld_KodCuti && x.fld_Nopkj == pkjamount).Sum(s => s.fld_JumlahAmt);
                    Amount = Amount == null ? 0 : Amount;
                    if (Amount != 0)
                    {
                        var GetIoPkj = AdminSCTransList.Where(x => pkjamount.Contains(x.fld_Nopkj)).ToList();

                        decimal[] SplitIO = (decimal[])SplitValue2(Convert.ToDecimal(Amount), GetIoPkj.Count);

                        var adminSCTrans = GetAmountAfterDevide(GetIoPkj, Amount);

                        foreach (var GetLeaveIOs in GetIoPkj)
                        {
                            var AvailableIO = AdminSCTransList.Where(x => x.fld_KodGL == GetLeaveIOs.fld_KodGL && x.fld_Nopkj == pkjamount && x.fld_SAPIO == GetLeaveIOs.fld_SAPIO && x.fld_KodPkt == GetLeaveIOs.fld_KodPkt).FirstOrDefault();
                            var amountAfterDiv = adminSCTrans.Where(x => x.fld_KodGL == GetLeaveIOs.fld_KodGL && x.fld_SAPIO == GetLeaveIOs.fld_SAPIO && x.fld_KodPkt == GetLeaveIOs.fld_KodPkt).Select(s => s.fld_Jumlah).FirstOrDefault();
                            if (AvailableIO != null)
                            {
                                AvailableIO.fld_KodGL = GetLeaveIOs.fld_KodGL;
                                AvailableIO.fld_Jumlah = amountAfterDiv;
                            }
                            else
                            {
                                AdminSCTransList.Add(new CustMod_AdminSCTrans() { fld_KodGL = GetLeaveIOs.fld_KodGL, fld_Jumlah = amountAfterDiv });
                            }
                            if (SplitIO.Count() > 1) { j++; }
                        }
                        j = 0;
                    }
                }
                var getGL = AdminSCTransList.Where(x => x.fld_KodGL != null).Select(s => new { s.fld_KodGL, s.fld_SAPIO, s.fld_KodPkt, s.fld_SAPType }).Distinct().ToList();

                foreach (var GetLeaveIOs in getGL)
                {
                    var getAmount = AdminSCTransList.Where(x => x.fld_KodGL == GetLeaveIOs.fld_KodGL && x.fld_SAPIO == GetLeaveIOs.fld_SAPIO && x.fld_KodPkt == GetLeaveIOs.fld_KodPkt).Sum(s => s.fld_Jumlah);
                    if (getAmount != 0 || Amount != 0)
                    {

                        AddTo_tbl_Sctran(db2, NegaraID, SyarikatID, WilayahID, LadangID, getAmount, 0, GetLeaveIOs.fld_KodPkt, Leave.fld_KodAktvt.Substring(0, 2), Leave.fld_KodAktvt, Leave.fld_KodGL, FirstCharToUpper(Leave.fld_KeteranganCuti.ToLower()), DTProcess, UserID, Month, Year, "D", 4, GetLeaveIOs.fld_KodGL, GetLeaveIOs.fld_SAPIO, GetLeaveIOs.fld_SAPType);
                        message = "Transaction Listing (Leave). (Data - Code Leave : " + Leave.fld_KodCuti + ", Code Activity : " + Leave.fld_KodAktvt + ", Amount : RM " + getAmount + ")";
                        Log += i == 1 ? DateTimeFunc.GetDateTime() + " - " + message : "\r\n" + DateTimeFunc.GetDateTime() + " - " + message;
                        i++;
                    }
                }
                AdminSCTransList.Where(c => c.fld_KodGL != null).ToList().ForEach(cc => cc.fld_Jumlah = 0);
            }
            db.Dispose();
            db2.Dispose();
        }
        //modified whole function by kamalia 15/2/22
        public void GetAmountAddedInsentifFunc(int? NegaraID, int? SyarikatID, int? WilayahID, int? LadangID, int? UserID, DateTime DTProcess, int? Month, int? Year, string processname, string servicesname, int? ClientID, List<CustMod_AdminSCTrans> AdminSCTransList, out string Log, List<tbl_Pkjmast> PkjMastList)
        {
            GenSalaryModelHQ db = new GenSalaryModelHQ();
            GetConnectFunc conn = new GetConnectFunc();
            string host, catalog, user, pass = "";
            conn.GetConnection(out host, out catalog, out user, out pass, WilayahID, SyarikatID, NegaraID);
            GenSalaryModelEstate db2 = GenSalaryModelEstate.ConnectToSqlServer(host, catalog, user, pass);
            Log = "";
            string message = "";
            int i = 1;
            int j = 0;
            decimal? Amount = 0;

            var GetInsentif = db.tbl_JenisInsentif.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_Deleted == false && x.fld_JenisInsentif == "P").ToList();
            var NoPkjList = AdminSCTransList.Select(s => s.fld_Nopkj).Distinct().ToArray();

            foreach (var Insentif in GetInsentif)
            {
                foreach (var pkjamount in NoPkjList)
                {
                    Amount = db2.tbl_Insentif.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Month == Month && x.fld_Year == Year && x.fld_KodInsentif == Insentif.fld_KodInsentif && x.fld_Deleted == false && x.fld_Nopkj == pkjamount).Sum(s => s.fld_NilaiInsentif);
                    Amount = Amount == null ? 0 : Amount;
                    if (Amount != 0)
                    {

                        var GetIoPkj = AdminSCTransList.Where(x => pkjamount.Contains(x.fld_Nopkj)).ToList();

                        decimal[] SplitIO = (decimal[])SplitValue2(Convert.ToDecimal(Amount), GetIoPkj.Count);

                        var adminSCTrans = GetAmountAfterDevide(GetIoPkj, Amount);

                        foreach (var GetAddInsentifs in GetIoPkj)
                        {
                            var AvailableIO = AdminSCTransList.Where(x => x.fld_KodGL == GetAddInsentifs.fld_KodGL && x.fld_Nopkj == pkjamount && x.fld_SAPIO == GetAddInsentifs.fld_SAPIO && x.fld_KodPkt == GetAddInsentifs.fld_KodPkt).FirstOrDefault();
                            var amountAfterDiv = adminSCTrans.Where(x => x.fld_KodGL == GetAddInsentifs.fld_KodGL && x.fld_SAPIO == GetAddInsentifs.fld_SAPIO && x.fld_KodPkt == GetAddInsentifs.fld_KodPkt).Select(s => s.fld_Jumlah).FirstOrDefault();
                            if (AvailableIO != null)
                            {
                                AvailableIO.fld_KodGL = GetAddInsentifs.fld_KodGL;
                                AvailableIO.fld_Jumlah = amountAfterDiv;
                            }
                            else
                            {
                                AdminSCTransList.Add(new CustMod_AdminSCTrans() { fld_KodGL = GetAddInsentifs.fld_KodGL, fld_Jumlah = amountAfterDiv });
                            }
                            if (SplitIO.Count() > 1) { j++; }
                        }
                        j = 0;
                    }
                }
                var getGL = AdminSCTransList.Where(x => x.fld_KodGL != null).Select(s => new { s.fld_KodGL, s.fld_SAPIO, s.fld_KodPkt, s.fld_SAPType }).Distinct().ToList();

                foreach (var GetAddInsentifs in getGL)
                {
                    var getAmount = AdminSCTransList.Where(x => x.fld_KodGL == GetAddInsentifs.fld_KodGL && x.fld_SAPIO == GetAddInsentifs.fld_SAPIO && x.fld_KodPkt == GetAddInsentifs.fld_KodPkt).Sum(s => s.fld_Jumlah);
                    if (getAmount != 0 || Amount != 0)
                    {
                        AddTo_tbl_Sctran(db2, NegaraID, SyarikatID, WilayahID, LadangID, getAmount, 0, GetAddInsentifs.fld_KodPkt, Insentif.fld_KodAktvt.Substring(0, 2), Insentif.fld_KodAktvt, Insentif.fld_KodGL, FirstCharToUpper(Insentif.fld_Keterangan.ToLower()), DTProcess, UserID, Month, Year, "D", 5, GetAddInsentifs.fld_KodGL, GetAddInsentifs.fld_SAPIO, GetAddInsentifs.fld_SAPType);
                        message = "Transaction Listing (Insentif Payment). (Data - Code Insentif : " + Insentif.fld_KodInsentif + ", Code Activity : " + Insentif.fld_KodAktvt + ", Amount : RM " + getAmount + ")";

                        Log += i == 1 ? DateTimeFunc.GetDateTime() + " - " + message : "\r\n" + DateTimeFunc.GetDateTime() + " - " + message;
                        i++;
                    }
                }
                AdminSCTransList.Where(c => c.fld_KodGL != null).ToList().ForEach(cc => cc.fld_Jumlah = 0);
            }
            db.Dispose();
            db2.Dispose();
        }
        //modified whole function by kamalia 17/3/22
        public void GetAmountDeductedInsentifFunc(int? NegaraID, int? SyarikatID, int? WilayahID, int? LadangID, int? UserID, DateTime DTProcess, int? Month, int? Year, string processname, string servicesname, int? ClientID, List<CustMod_AdminSCTrans> AdminSCTransList, out string Log, List<tbl_Pkjmast> PkjMastList, string compCode)
        {
            GenSalaryModelHQ db = new GenSalaryModelHQ();
            GetConnectFunc conn = new GetConnectFunc();
            string host, catalog, user, pass = "";
            conn.GetConnection(out host, out catalog, out user, out pass, WilayahID, SyarikatID, NegaraID);
            GenSalaryModelEstate db2 = GenSalaryModelEstate.ConnectToSqlServer(host, catalog, user, pass);
            Log = "";
            string message = "";
            int i = 1;
            //  int j = 0;
            decimal? Amount = 0;

            var GetInsentif = db.tbl_JenisInsentif.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_Deleted == false && x.fld_JenisInsentif == "T").ToList();
            var NoPkjList = PkjMastList.Select(s => s.fld_Nopkj).ToArray();
            foreach (var Insentif in GetInsentif)
            {
                Amount = db2.tbl_Insentif.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Month == Month && x.fld_Year == Year && x.fld_KodInsentif == Insentif.fld_KodInsentif && x.fld_Deleted == false && NoPkjList.Contains(x.fld_Nopkj)).Sum(s => s.fld_NilaiInsentif);
                Amount = Amount == null ? 0 : Amount;


                if (Amount != 0)
                {
                    var GetPotonganGL = db.tbl_CustomerVendorGLMap.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_Flag == "5" && x.fld_TypeCode == "GL" && x.fld_Deleted == false && x.fld_compcode == compCode).Select(s => s.fld_SAPCode).FirstOrDefault();
                    AddTo_tbl_Sctran(db2, NegaraID, SyarikatID, WilayahID, LadangID, Amount, 0, "-", Insentif.fld_KodAktvt.Substring(0, 2), Insentif.fld_KodAktvt, Insentif.fld_KodGL, FirstCharToUpper(Insentif.fld_Keterangan.ToLower()), DTProcess, UserID, Month, Year, "C", 11, GetPotonganGL, "-", "");
                    message = "Transaction Listing (Insentif Deduction). (Data - Code Insentif : " + Insentif.fld_KodInsentif + ", Code Activity : " + Insentif.fld_KodAktvt + ", Amount : RM " + Amount + ")";
                    Log += i == 1 ? DateTimeFunc.GetDateTime() + " - " + message : "\r\n" + DateTimeFunc.GetDateTime() + " - " + message;
                }

            }
            db.Dispose();
            db2.Dispose();
        }
        //modified whole function by kamalia 15/2/22
        public void GetAmountKWSPFunc(int? NegaraID, int? SyarikatID, int? WilayahID, int? LadangID, int? UserID, DateTime DTProcess, int? Month, int? Year, string processname, string servicesname, int? ClientID, List<CustMod_AdminSCTrans> AdminSCTransList, out string Log, List<tbl_Pkjmast> PkjMastList)
        {
            GenSalaryModelHQ db = new GenSalaryModelHQ();
            GetConnectFunc conn = new GetConnectFunc();
            string host, catalog, user, pass = "";
            conn.GetConnection(out host, out catalog, out user, out pass, WilayahID, SyarikatID, NegaraID);
            GenSalaryModelEstate db2 = GenSalaryModelEstate.ConnectToSqlServer(host, catalog, user, pass);
            Log = "";
            string message = "";
            int i = 1;
            int j = 0;
            decimal? Amount = 0;
            decimal? AmountPkjDistribute = 0;
            decimal? AmountMkjDistribute = 0;

            var GetKWSP = db.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "kwsp" && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fldDeleted == false).ToList();
            var NoPkjList = AdminSCTransList.Select(s => s.fld_Nopkj).Distinct().ToArray();
            foreach (var KWSP in GetKWSP)
            {
                foreach (var pkjamount in NoPkjList)
                {
                    Amount = KWSP.fldOptConfFlag3 == "Employee" ?
                    db2.tbl_GajiBulanan.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Month == Month && x.fld_Year == Year && x.fld_Nopkj == pkjamount).Sum(s => s.fld_KWSPPkj)
                 :
                    db2.tbl_GajiBulanan.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Month == Month && x.fld_Year == Year && x.fld_Nopkj == pkjamount).Sum(s => s.fld_KWSPMjk);
                    Amount = Amount == null ? 0 : Amount;
                    if (Amount != 0)
                    {

                        var GetIoPkj = AdminSCTransList.Where(x => pkjamount.Contains(x.fld_Nopkj)).ToList();

                        decimal[] SplitIO = (decimal[])SplitValue2(Convert.ToDecimal(Amount), GetIoPkj.Count);

                        var adminSCTrans = GetAmountAfterDevide(GetIoPkj, Amount);

                        foreach (var GetKWSPIOs in GetIoPkj)
                        {
                            var AvailableIO = AdminSCTransList.Where(x => x.fld_KodGL == GetKWSPIOs.fld_KodGL && x.fld_Nopkj == pkjamount && x.fld_SAPIO == GetKWSPIOs.fld_SAPIO && x.fld_KodPkt == GetKWSPIOs.fld_KodPkt).FirstOrDefault();
                            var amountAfterDiv = adminSCTrans.Where(x => x.fld_KodGL == GetKWSPIOs.fld_KodGL && x.fld_SAPIO == GetKWSPIOs.fld_SAPIO && x.fld_KodPkt == GetKWSPIOs.fld_KodPkt).Select(s => s.fld_Jumlah).FirstOrDefault();
                            if (AvailableIO != null)
                            {
                                AvailableIO.fld_KodGL = GetKWSPIOs.fld_KodGL;
                                AvailableIO.fld_Jumlah = amountAfterDiv;
                            }
                            else
                            {
                                AdminSCTransList.Add(new CustMod_AdminSCTrans() { fld_KodGL = GetKWSPIOs.fld_KodGL, fld_Jumlah = amountAfterDiv });
                            }
                            if (SplitIO.Count() > 1) { j++; }
                        }
                        j = 0;
                    }
                }
                var getGL = AdminSCTransList.Where(x => x.fld_KodGL != null).Select(s => new { s.fld_KodGL, s.fld_SAPIO, s.fld_KodPkt, s.fld_SAPType }).Distinct().ToList();
                foreach (var GetKWSPIOs in getGL)
                {
                    var getAmount = AdminSCTransList.Where(x => x.fld_KodGL == GetKWSPIOs.fld_KodGL && x.fld_SAPIO == GetKWSPIOs.fld_SAPIO && x.fld_KodPkt == GetKWSPIOs.fld_KodPkt).Sum(s => s.fld_Jumlah);
                    if (getAmount != 0)
                    {
                        AddTo_tbl_Sctran(db2, NegaraID, SyarikatID, WilayahID, LadangID, getAmount, 0, GetKWSPIOs.fld_KodPkt, KWSP.fldOptConfValue.Substring(0, 2), KWSP.fldOptConfValue, KWSP.fldOptConfFlag2, KWSP.fldOptConfDesc, DTProcess, UserID, Month, Year, "D", 7, GetKWSPIOs.fld_KodGL, GetKWSPIOs.fld_SAPIO, GetKWSPIOs.fld_SAPType);
                        message = "Transaction Listing (KWSP " + KWSP.fldOptConfFlag3 + "). (Data - Code Activity : " + KWSP.fldOptConfValue + ", Amount : RM " + Amount + ")";

                        Log += i == 1 ? DateTimeFunc.GetDateTime() + " - " + message : "\r\n" + DateTimeFunc.GetDateTime() + " - " + message;
                        i++;
                    }
                }
                AdminSCTransList.Where(c => c.fld_KodGL != null).ToList().ForEach(cc => cc.fld_Jumlah = 0);
            }
            var KWSPMix = db.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "kwspmix" && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fldDeleted == false).FirstOrDefault();

            var DistributeAmount3 = new List<CustMod_DistributeAmount>();
            foreach (var pkjamount in NoPkjList)
            {
                AmountPkjDistribute = db2.tbl_GajiBulanan.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Month == Month && x.fld_Year == Year && x.fld_Nopkj == pkjamount).Sum(s => s.fld_KWSPPkj);
                AmountMkjDistribute = db2.tbl_GajiBulanan.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Month == Month && x.fld_Year == Year && x.fld_Nopkj == pkjamount).Sum(s => s.fld_KWSPMjk);
                AmountMkjDistribute = AmountMkjDistribute == null ? 0 : AmountMkjDistribute;
                AmountPkjDistribute = AmountPkjDistribute == null ? 0 : AmountPkjDistribute;
                Amount = AmountMkjDistribute + AmountPkjDistribute;

                if (Amount != 0)
                {

                    var GetIoPkj = AdminSCTransList.Where(x => pkjamount.Contains(x.fld_Nopkj)).ToList();

                    decimal[] SplitIO = (decimal[])SplitValue2(Convert.ToDecimal(Amount), GetIoPkj.Count);

                    var adminSCTrans = GetAmountAfterDevide(GetIoPkj, Amount);

                    foreach (var GetKWSPMixIOs in GetIoPkj)
                    {
                        var AvailableIO = AdminSCTransList.Where(x => x.fld_KodGL == GetKWSPMixIOs.fld_KodGL && x.fld_Nopkj == pkjamount && x.fld_SAPIO == GetKWSPMixIOs.fld_SAPIO && x.fld_KodPkt == GetKWSPMixIOs.fld_KodPkt).FirstOrDefault();
                        var amountAfterDiv = adminSCTrans.Where(x => x.fld_KodGL == GetKWSPMixIOs.fld_KodGL && x.fld_SAPIO == GetKWSPMixIOs.fld_SAPIO && x.fld_KodPkt == GetKWSPMixIOs.fld_KodPkt).Select(s => s.fld_Jumlah).FirstOrDefault();
                        if (AvailableIO != null)
                        {
                            AvailableIO.fld_KodGL = GetKWSPMixIOs.fld_KodGL;
                            AvailableIO.fld_Jumlah = amountAfterDiv;
                        }
                        else
                        {
                            AdminSCTransList.Add(new CustMod_AdminSCTrans() { fld_KodGL = GetKWSPMixIOs.fld_KodGL, fld_Jumlah = amountAfterDiv });
                        }
                        if (SplitIO.Count() > 1) { j++; }
                    }
                    j = 0;
                }
            }
            var getGLMix = AdminSCTransList.Where(x => x.fld_KodGL != null).Select(s => new { s.fld_KodGL, s.fld_SAPIO, s.fld_KodPkt, s.fld_SAPType }).Distinct().ToList();
            foreach (var GetKWSPMixIOs in getGLMix)
            {
                var getAmount = AdminSCTransList.Where(x => x.fld_KodGL == GetKWSPMixIOs.fld_KodGL && x.fld_SAPIO == GetKWSPMixIOs.fld_SAPIO && x.fld_KodPkt == GetKWSPMixIOs.fld_KodPkt).Sum(s => s.fld_Jumlah);
                if (getAmount != 0)
                {
                    AddTo_tbl_Sctran(db2, NegaraID, SyarikatID, WilayahID, LadangID, getAmount, 0, GetKWSPMixIOs.fld_KodPkt, KWSPMix.fldOptConfValue.Substring(0, 2), KWSPMix.fldOptConfValue, KWSPMix.fldOptConfFlag2, KWSPMix.fldOptConfDesc, DTProcess, UserID, Month, Year, "C", 7, GetKWSPMixIOs.fld_KodGL, GetKWSPMixIOs.fld_SAPIO, GetKWSPMixIOs.fld_SAPType);
                    message = "Transaction Listing (KWSP " + KWSPMix.fldOptConfFlag3 + "). (Data - Code Activity : " + KWSPMix.fldOptConfValue + ", Amount : RM " + getAmount + ")";
                    Log += i == 1 ? DateTimeFunc.GetDateTime() + " - " + message : "\r\n" + DateTimeFunc.GetDateTime() + " - " + message;
                    i++;
                }
            }
            AdminSCTransList.Where(c => c.fld_KodGL != null).ToList().ForEach(cc => cc.fld_Jumlah = 0);

            db.Dispose();
            db2.Dispose();
        }
        //modified whole function by kamalia 15/2/22
        public void GetAmountSocsoFunc(int? NegaraID, int? SyarikatID, int? WilayahID, int? LadangID, int? UserID, DateTime DTProcess, int? Month, int? Year, string processname, string servicesname, int? ClientID, List<CustMod_AdminSCTrans> AdminSCTransList, out string Log, List<tbl_Pkjmast> PkjMastList)
        {
            GenSalaryModelHQ db = new GenSalaryModelHQ();
            GetConnectFunc conn = new GetConnectFunc();
            string host, catalog, user, pass = "";
            conn.GetConnection(out host, out catalog, out user, out pass, WilayahID, SyarikatID, NegaraID);
            GenSalaryModelEstate db2 = GenSalaryModelEstate.ConnectToSqlServer(host, catalog, user, pass);
            Log = "";
            string message = "";
            int i = 1;
            int j = 0;
            decimal? Amount = 0;
            decimal? AmountPkjDistribute = 0;
            decimal? AmountMkjDistribute = 0;

            var GetSocso = db.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "socso" && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fldDeleted == false).ToList();
            var NoPkjList = AdminSCTransList.Select(s => s.fld_Nopkj).Distinct().ToArray();

            foreach (var Socso in GetSocso)
            {
                foreach (var pkjamount in NoPkjList)
                {
                    Amount = Socso.fldOptConfFlag3 == "Employee" ?
                db2.tbl_GajiBulanan.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Month == Month && x.fld_Year == Year && x.fld_Nopkj == pkjamount).Sum(s => s.fld_SocsoPkj)
                :
                db2.tbl_GajiBulanan.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Month == Month && x.fld_Year == Year && x.fld_Nopkj == pkjamount).Sum(s => s.fld_SocsoMjk);
                    Amount = Amount == null ? 0 : Amount;
                    if (Amount != 0)
                    {

                        var GetIoPkj = AdminSCTransList.Where(x => pkjamount.Contains(x.fld_Nopkj)).ToList();

                        decimal[] SplitIO = (decimal[])SplitValue2(Convert.ToDecimal(Amount), GetIoPkj.Count);

                        var adminSCTrans = GetAmountAfterDevide(GetIoPkj, Amount);

                        foreach (var GetSocsoIOs in GetIoPkj)
                        {
                            var AvailableIO = AdminSCTransList.Where(x => x.fld_KodGL == GetSocsoIOs.fld_KodGL && x.fld_Nopkj == pkjamount && x.fld_SAPIO == GetSocsoIOs.fld_SAPIO && x.fld_KodPkt == GetSocsoIOs.fld_KodPkt).FirstOrDefault();
                            var amountAfterDiv = adminSCTrans.Where(x => x.fld_KodGL == GetSocsoIOs.fld_KodGL && x.fld_SAPIO == GetSocsoIOs.fld_SAPIO && x.fld_KodPkt == GetSocsoIOs.fld_KodPkt).Select(s => s.fld_Jumlah).FirstOrDefault();
                            if (AvailableIO != null)
                            {
                                AvailableIO.fld_KodGL = GetSocsoIOs.fld_KodGL;
                                AvailableIO.fld_Jumlah = amountAfterDiv;
                            }
                            else
                            {
                                AdminSCTransList.Add(new CustMod_AdminSCTrans() { fld_KodGL = GetSocsoIOs.fld_KodGL, fld_Jumlah = amountAfterDiv });
                            }
                            if (SplitIO.Count() > 1) { j++; }
                        }
                        j = 0;
                    }

                }
                var getGL = AdminSCTransList.Where(x => x.fld_KodGL != null).Select(s => new { s.fld_KodGL, s.fld_SAPIO, s.fld_KodPkt, s.fld_SAPType }).Distinct().ToList();

                foreach (var GetSocsoIOs in getGL)
                {
                    var getAmount = AdminSCTransList.Where(x => x.fld_KodGL == GetSocsoIOs.fld_KodGL && x.fld_SAPIO == GetSocsoIOs.fld_SAPIO && x.fld_KodPkt == GetSocsoIOs.fld_KodPkt).Sum(s => s.fld_Jumlah);

                    if (getAmount != 0)
                    {
                        AddTo_tbl_Sctran(db2, NegaraID, SyarikatID, WilayahID, LadangID, getAmount, 0, GetSocsoIOs.fld_KodPkt, Socso.fldOptConfValue.Substring(0, 2), Socso.fldOptConfValue, Socso.fldOptConfFlag2, Socso.fldOptConfDesc, DTProcess, UserID, Month, Year, "D", 8, GetSocsoIOs.fld_KodGL, GetSocsoIOs.fld_SAPIO, GetSocsoIOs.fld_SAPType);
                        message = "Transaction Listing (Socso " + Socso.fldOptConfFlag3 + "). (Data - Code Activity : " + Socso.fldOptConfValue + ", Amount : RM " + getAmount + ")";
                        Log += i == 1 ? DateTimeFunc.GetDateTime() + " - " + message : "\r\n" + DateTimeFunc.GetDateTime() + " - " + message;
                        i++;
                    }
                }
                AdminSCTransList.Where(c => c.fld_KodGL != null).ToList().ForEach(cc => cc.fld_Jumlah = 0);
            }

            var SocsoMix = db.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "socsomix" && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fldDeleted == false).FirstOrDefault();

            foreach (var pkjamount in NoPkjList)
            {
                AmountPkjDistribute = db2.tbl_GajiBulanan.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Month == Month && x.fld_Year == Year && x.fld_Nopkj == pkjamount).Sum(s => s.fld_SocsoPkj);
                AmountMkjDistribute = db2.tbl_GajiBulanan.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Month == Month && x.fld_Year == Year && x.fld_Nopkj == pkjamount).Sum(s => s.fld_SocsoMjk);

                AmountMkjDistribute = AmountMkjDistribute == null ? 0 : AmountMkjDistribute;
                AmountPkjDistribute = AmountPkjDistribute == null ? 0 : AmountPkjDistribute;
                Amount = AmountMkjDistribute + AmountPkjDistribute;
                if (Amount != 0)
                {
                    var GetIoPkj = AdminSCTransList.Where(x => pkjamount.Contains(x.fld_Nopkj)).ToList();

                    decimal[] SplitIO = (decimal[])SplitValue2(Convert.ToDecimal(Amount), GetIoPkj.Count);

                    var adminSCTrans = GetAmountAfterDevide(GetIoPkj, Amount);

                    foreach (var GetSocsoMixIOs in GetIoPkj)
                    {
                        var AvailableIO = AdminSCTransList.Where(x => x.fld_KodGL == GetSocsoMixIOs.fld_KodGL && x.fld_Nopkj == pkjamount && x.fld_SAPIO == GetSocsoMixIOs.fld_SAPIO && x.fld_KodPkt == GetSocsoMixIOs.fld_KodPkt).FirstOrDefault();
                        var amountAfterDiv = adminSCTrans.Where(x => x.fld_KodGL == GetSocsoMixIOs.fld_KodGL && x.fld_SAPIO == GetSocsoMixIOs.fld_SAPIO && x.fld_KodPkt == GetSocsoMixIOs.fld_KodPkt).Select(s => s.fld_Jumlah).FirstOrDefault();
                        if (AvailableIO != null)
                        {
                            AvailableIO.fld_KodGL = GetSocsoMixIOs.fld_KodGL;
                            AvailableIO.fld_Jumlah = amountAfterDiv;
                        }
                        else
                        {
                            AdminSCTransList.Add(new CustMod_AdminSCTrans() { fld_KodGL = GetSocsoMixIOs.fld_KodGL, fld_Jumlah = amountAfterDiv });
                        }
                        if (SplitIO.Count() > 1) { j++; }
                    }
                    j = 0;
                }
            }

            var getGL1 = AdminSCTransList.Where(x => x.fld_KodGL != null).Select(s => new { s.fld_KodGL, s.fld_SAPIO, s.fld_KodPkt, s.fld_SAPType }).Distinct().ToList();
            foreach (var GetSocsoMixIOs in getGL1)
            {
                var getAmount = AdminSCTransList.Where(x => x.fld_KodGL == GetSocsoMixIOs.fld_KodGL && x.fld_SAPIO == GetSocsoMixIOs.fld_SAPIO && x.fld_KodPkt == GetSocsoMixIOs.fld_KodPkt).Sum(s => s.fld_Jumlah);
                if (getAmount != 0)
                {

                    AddTo_tbl_Sctran(db2, NegaraID, SyarikatID, WilayahID, LadangID, getAmount, 0, GetSocsoMixIOs.fld_KodPkt, SocsoMix.fldOptConfValue.Substring(0, 2), SocsoMix.fldOptConfValue, SocsoMix.fldOptConfFlag2, SocsoMix.fldOptConfDesc, DTProcess, UserID, Month, Year, "C", 8, GetSocsoMixIOs.fld_KodGL, GetSocsoMixIOs.fld_SAPIO, GetSocsoMixIOs.fld_SAPType);
                    message = "Transaction Listing (Socso " + SocsoMix.fldOptConfFlag3 + "). (Data - Code Activity : " + SocsoMix.fldOptConfValue + ", Amount : RM " + getAmount + ")";
                    Log += i == 1 ? DateTimeFunc.GetDateTime() + " - " + message : "\r\n" + DateTimeFunc.GetDateTime() + " - " + message;
                    i++;
                }
            }
            AdminSCTransList.Where(c => c.fld_KodGL != null).ToList().ForEach(cc => cc.fld_Jumlah = 0);

            db.Dispose();
            db2.Dispose();
        }
        //modified whole function by kamalia 15/2/22
        public void GetAmountOtherContributionsFunc(int? NegaraID, int? SyarikatID, int? WilayahID, int? LadangID, int? UserID, DateTime DTProcess, int? Month, int? Year, string processname, string servicesname, int? ClientID, List<CustMod_AdminSCTrans> AdminSCTransList, out string Log, List<tbl_Pkjmast> PkjMastList)
        {
            Log = "";
            GenSalaryModelHQ db = new GenSalaryModelHQ();
            GetConnectFunc conn = new GetConnectFunc();
            string host, catalog, user, pass = "";
            conn.GetConnection(out host, out catalog, out user, out pass, WilayahID, SyarikatID, NegaraID);
            GenSalaryModelEstate db2 = GenSalaryModelEstate.ConnectToSqlServer(host, catalog, user, pass);
            string KodCaruman = "";
            decimal? AmountP = 0;
            decimal? AmountM = 0;
            decimal? AmountMix = 0;
            string message = "";
            int mjk = 0;
            int pkj = 0;
            int mix = 0;

            var NoPkjList = AdminSCTransList.Select(s => s.fld_Nopkj).Distinct().ToArray();
            var GetOtherContributions = db.tbl_CarumanTambahan.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_Deleted == false).ToList();

            foreach (var GetOtherContribution in GetOtherContributions)
            {
                var DistributeAmount1 = new List<CustMod_DistributeAmount>();
                foreach (var pkjamount in NoPkjList)
                {

                    var GetIoPkj = AdminSCTransList.Where(x => pkjamount.Contains(x.fld_Nopkj)).ToList();
                    var GetpkjContribution = db2.tbl_PkjCarumanTambahan.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Nopkj == pkjamount && x.fld_Deleted == false).Select(s => s.fld_KodCaruman).FirstOrDefault();// kamy edit 8/5/2022
                    var GetGajiID = db2.tbl_GajiBulanan.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Month == Month && x.fld_Year == Year && x.fld_Nopkj == pkjamount).Select(s => s.fld_ID).FirstOrDefault();
                    KodCaruman = GetOtherContribution.fld_KodCaruman;

                    if (KodCaruman == GetpkjContribution)
                    {
                        switch (GetOtherContribution.fld_CarumanOleh)
                        {
                            case 1:
                                AmountP = db2.tbl_ByrCarumanTambahan.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Month == Month && x.fld_Year == Year && x.fld_KodCaruman == KodCaruman && x.fld_GajiID == GetGajiID).Sum(s => s.fld_CarumanPekerja);
                                AmountM = 0;
                                AmountMix = AmountP + AmountM;
                                break;
                            case 2:
                                AmountP = 0;
                                AmountM = db2.tbl_ByrCarumanTambahan.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Month == Month && x.fld_Year == Year && x.fld_KodCaruman == KodCaruman && x.fld_GajiID == GetGajiID).Sum(s => s.fld_CarumanMajikan);
                                AmountMix = AmountP + AmountM;
                                break;
                            case 3:
                                AmountP = db2.tbl_ByrCarumanTambahan.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Month == Month && x.fld_Year == Year && x.fld_KodCaruman == KodCaruman && x.fld_GajiID == GetGajiID).Sum(s => s.fld_CarumanPekerja);
                                AmountM = db2.tbl_ByrCarumanTambahan.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Month == Month && x.fld_Year == Year && x.fld_KodCaruman == KodCaruman && x.fld_GajiID == GetGajiID).Sum(s => s.fld_CarumanMajikan);
                                AmountMix = AmountP + AmountM;
                                break;
                        }
                        AmountP = AmountP == null ? 0 : AmountP;
                        AmountM = AmountM == null ? 0 : AmountM;
                        AmountMix = AmountMix == null ? 0 : AmountMix;

                        decimal[] SplitIOM = (decimal[])SplitValue2(Convert.ToDecimal(AmountM), GetIoPkj.Count);
                        var adminSCTransM = GetAmountAfterDevide(GetIoPkj, AmountM);
                        decimal[] SplitIOP = (decimal[])SplitValue2(Convert.ToDecimal(AmountP), GetIoPkj.Count);
                        var adminSCTransP = GetAmountAfterDevide(GetIoPkj, AmountP);
                        decimal[] SplitIOMix = (decimal[])SplitValue2(Convert.ToDecimal(AmountMix), GetIoPkj.Count);
                        var adminSCTransMix = GetAmountAfterDevide(GetIoPkj, AmountMix);

                        foreach (var GetOtherContributionIOs in GetIoPkj)
                        {
                            var AvailableIO = AdminSCTransList.Where(x => x.fld_KodGL == GetOtherContributionIOs.fld_KodGL && x.fld_Nopkj == pkjamount && x.fld_SAPIO == GetOtherContributionIOs.fld_SAPIO && x.fld_KodPkt == GetOtherContributionIOs.fld_KodPkt).FirstOrDefault();
                            var amountAfterDivM = adminSCTransM.Where(x => x.fld_KodGL == GetOtherContributionIOs.fld_KodGL && x.fld_SAPIO == GetOtherContributionIOs.fld_SAPIO && x.fld_KodPkt == GetOtherContributionIOs.fld_KodPkt).Select(s => s.fld_Jumlah).FirstOrDefault();
                            var amountAfterDivP = adminSCTransP.Where(x => x.fld_KodGL == GetOtherContributionIOs.fld_KodGL && x.fld_SAPIO == GetOtherContributionIOs.fld_SAPIO && x.fld_KodPkt == GetOtherContributionIOs.fld_KodPkt).Select(s => s.fld_Jumlah).FirstOrDefault();
                            var amountAfterDivMix = adminSCTransMix.Where(x => x.fld_KodGL == GetOtherContributionIOs.fld_KodGL && x.fld_SAPIO == GetOtherContributionIOs.fld_SAPIO && x.fld_KodPkt == GetOtherContributionIOs.fld_KodPkt).Select(s => s.fld_Jumlah).FirstOrDefault();
                            if (AvailableIO != null)
                            {
                                AvailableIO.fld_KodGL = GetOtherContributionIOs.fld_KodGL;
                                AvailableIO.fld_JumlahMjk = amountAfterDivM;
                                AvailableIO.fld_JumlahPkj = amountAfterDivP;
                                AvailableIO.fld_JumlahMix = amountAfterDivMix;
                            }
                            else
                            {
                                AdminSCTransList.Add(new CustMod_AdminSCTrans() { fld_KodGL = GetOtherContributionIOs.fld_KodGL, fld_JumlahPkj = amountAfterDivP, fld_JumlahMjk = amountAfterDivM, fld_JumlahMix = amountAfterDivMix });
                            }
                            if (SplitIOM.Count() > 1 || SplitIOP.Count() > 1 || SplitIOMix.Count() > 1)
                            {
                                mjk++;
                                pkj++;
                                mix++;
                            }
                        }
                        mjk = 0;
                        pkj = 0;
                        mix = 0;

                    }
                }

                var getGL = AdminSCTransList.Where(x => x.fld_KodGL != null).Select(s => new { s.fld_KodGL, s.fld_SAPIO, s.fld_KodPkt, s.fld_SAPType }).Distinct().ToList();

                foreach (var GetOtherContributionIOs in getGL)
                {
                    var GetOtherContribute = db.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == KodCaruman.ToLower() && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fldDeleted == false).ToList();
                    var getAmountPkj = AdminSCTransList.Where(x => x.fld_KodGL == GetOtherContributionIOs.fld_KodGL && x.fld_SAPIO == GetOtherContributionIOs.fld_SAPIO && x.fld_KodPkt == GetOtherContributionIOs.fld_KodPkt).Sum(s => s.fld_JumlahPkj);
                    var getAmountMjk = AdminSCTransList.Where(x => x.fld_KodGL == GetOtherContributionIOs.fld_KodGL && x.fld_SAPIO == GetOtherContributionIOs.fld_SAPIO && x.fld_KodPkt == GetOtherContributionIOs.fld_KodPkt).Sum(s => s.fld_JumlahMjk);
                    var getAmountMix = AdminSCTransList.Where(x => x.fld_KodGL == GetOtherContributionIOs.fld_KodGL && x.fld_SAPIO == GetOtherContributionIOs.fld_SAPIO && x.fld_KodPkt == GetOtherContributionIOs.fld_KodPkt).Sum(s => s.fld_JumlahMix);

                    if (getAmountPkj != 0)
                    {
                        var PkjAktvtDetail = GetOtherContribute.Where(x => x.fldOptConfFlag3 == "Employee").FirstOrDefault();
                        AddTo_tbl_Sctran(db2, NegaraID, SyarikatID, WilayahID, LadangID, getAmountPkj, 0, GetOtherContributionIOs.fld_KodPkt, PkjAktvtDetail.fldOptConfValue.Substring(0, 2), PkjAktvtDetail.fldOptConfValue, PkjAktvtDetail.fldOptConfFlag2, PkjAktvtDetail.fldOptConfDesc, DTProcess, UserID, Month, Year, "D", 10, GetOtherContributionIOs.fld_KodGL, GetOtherContributionIOs.fld_SAPIO, GetOtherContributionIOs.fld_SAPType);
                        message = "Transaction Listing (" + GetOtherContribution.fld_NamaCaruman + " " + PkjAktvtDetail.fldOptConfFlag3 + "). (Data - Code Activity : " + PkjAktvtDetail.fldOptConfValue + ", Amount : RM " + getAmountPkj + ")";
                        Log += DateTimeFunc.GetDateTime() + " - " + message;
                    }
                    if (getAmountMjk != 0)
                    {
                        var MjkAktvtDetail = GetOtherContribute.Where(x => x.fldOptConfFlag3 == "Employer").FirstOrDefault();
                        AddTo_tbl_Sctran(db2, NegaraID, SyarikatID, WilayahID, LadangID, getAmountMjk, 0, GetOtherContributionIOs.fld_KodPkt, MjkAktvtDetail.fldOptConfValue.Substring(0, 2), MjkAktvtDetail.fldOptConfValue, MjkAktvtDetail.fldOptConfFlag2, MjkAktvtDetail.fldOptConfDesc, DTProcess, UserID, Month, Year, "D", 10, GetOtherContributionIOs.fld_KodGL, GetOtherContributionIOs.fld_SAPIO, GetOtherContributionIOs.fld_SAPType);
                        message = "Transaction Listing (" + GetOtherContribution.fld_NamaCaruman + " " + MjkAktvtDetail.fldOptConfFlag3 + "). (Data - Code Activity : " + MjkAktvtDetail.fldOptConfValue + ", Amount : RM " + getAmountMjk + ")";
                        Log += "\r\n" + DateTimeFunc.GetDateTime() + " - " + message;
                    }
                    if (getAmountMix != 0)
                    {
                        var PjkMjkAktvtDetail = GetOtherContribute.Where(x => x.fldOptConfFlag3 == "Employee + Employer").FirstOrDefault();
                        AddTo_tbl_Sctran(db2, NegaraID, SyarikatID, WilayahID, LadangID, getAmountMix, 0, GetOtherContributionIOs.fld_KodPkt, PjkMjkAktvtDetail.fldOptConfValue.Substring(0, 2), PjkMjkAktvtDetail.fldOptConfValue, PjkMjkAktvtDetail.fldOptConfFlag2, PjkMjkAktvtDetail.fldOptConfDesc, DTProcess, UserID, Month, Year, "C", 10, GetOtherContributionIOs.fld_KodGL, GetOtherContributionIOs.fld_SAPIO, GetOtherContributionIOs.fld_SAPType);
                        message = "Transaction Listing (" + GetOtherContribution.fld_NamaCaruman + " " + PjkMjkAktvtDetail.fldOptConfFlag3 + "). (Data - Code Activity : " + PjkMjkAktvtDetail.fldOptConfValue + ", Amount : RM " + getAmountMix + ")";
                        Log += "\r\n" + DateTimeFunc.GetDateTime() + " - " + message;
                    }
                }
                AdminSCTransList.Where(c => c.fld_KodGL != null).ToList().ForEach(cc => { cc.fld_JumlahPkj = 0; cc.fld_JumlahMjk = 0; cc.fld_JumlahMix = 0; });
            }
            db.Dispose();
            db2.Dispose();
        }

        //modified whole function by kamalia 15/2/22
        public void GetAmountAIPSFunc(int? NegaraID, int? SyarikatID, int? WilayahID, int? LadangID, int? UserID, DateTime DTProcess, int? Month, int? Year, string processname, string servicesname, int? ClientID, List<CustMod_AdminSCTrans> AdminSCTransList, out string Log, List<tbl_Pkjmast> PkjMastList)
        {
            GenSalaryModelHQ db = new GenSalaryModelHQ();
            GetConnectFunc conn = new GetConnectFunc();
            string host, catalog, user, pass = "";
            conn.GetConnection(out host, out catalog, out user, out pass, WilayahID, SyarikatID, NegaraID);
            GenSalaryModelEstate db2 = GenSalaryModelEstate.ConnectToSqlServer(host, catalog, user, pass);
            Log = "";
            string message = "";
            int i = 1;
            int j = 0;
            decimal? Amount = 0;

            var GetAIPS = db.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "aips" && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fldDeleted == false).ToList();
            var NoPkjList = AdminSCTransList.Select(s => s.fld_Nopkj).Distinct().ToArray();

            foreach (var AIPS in GetAIPS)
            {
                foreach (var pkjamount in NoPkjList)
                {
                    Amount = db2.tbl_GajiBulanan.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Month == Month && x.fld_Year == Year && x.fld_Nopkj == pkjamount).Sum(s => s.fld_AIPS);
                    Amount = Amount == null ? 0 : Amount;
                    if (Amount != 0)
                    {

                        var GetIoPkj = AdminSCTransList.Where(x => pkjamount.Contains(x.fld_Nopkj)).ToList();

                        decimal[] SplitIO = (decimal[])SplitValue2(Convert.ToDecimal(Amount), GetIoPkj.Count);

                        var adminSCTrans = GetAmountAfterDevide(GetIoPkj, Amount);

                        foreach (var GetAIPSIOs in GetIoPkj)
                        {
                            var AvailableIO = AdminSCTransList.Where(x => x.fld_KodGL == GetAIPSIOs.fld_KodGL && x.fld_Nopkj == pkjamount && x.fld_SAPIO == GetAIPSIOs.fld_SAPIO && x.fld_KodPkt == GetAIPSIOs.fld_KodPkt).FirstOrDefault();
                            var amountAfterDiv = adminSCTrans.Where(x => x.fld_KodGL == GetAIPSIOs.fld_KodGL && x.fld_SAPIO == GetAIPSIOs.fld_SAPIO && x.fld_KodPkt == GetAIPSIOs.fld_KodPkt).Select(s => s.fld_Jumlah).FirstOrDefault();
                            if (AvailableIO != null)
                            {
                                AvailableIO.fld_KodGL = GetAIPSIOs.fld_KodGL;
                                AvailableIO.fld_Jumlah = amountAfterDiv;
                            }
                            else
                            {
                                AdminSCTransList.Add(new CustMod_AdminSCTrans() { fld_KodGL = GetAIPSIOs.fld_KodGL, fld_Jumlah = amountAfterDiv });
                            }
                            if (SplitIO.Count() > 1) { j++; }
                        }
                        j = 0;
                    }
                }
                var getGLMix = AdminSCTransList.Where(x => x.fld_KodGL != null).Select(s => new { s.fld_KodGL, s.fld_SAPIO, s.fld_KodPkt, s.fld_SAPType }).Distinct().ToList();
                foreach (var GetAIPSIOs in getGLMix)
                {
                    var getAmount = AdminSCTransList.Where(x => x.fld_KodGL == GetAIPSIOs.fld_KodGL && x.fld_SAPIO == GetAIPSIOs.fld_SAPIO && x.fld_KodPkt == GetAIPSIOs.fld_KodPkt).Sum(s => s.fld_Jumlah);
                    if (getAmount != 0)
                    {
                        AddTo_tbl_Sctran(db2, NegaraID, SyarikatID, WilayahID, LadangID, getAmount, 0, GetAIPSIOs.fld_KodPkt, AIPS.fldOptConfValue.Substring(0, 2), AIPS.fldOptConfValue, AIPS.fldOptConfFlag2, AIPS.fldOptConfDesc, DTProcess, UserID, Month, Year, "D", 9, GetAIPSIOs.fld_KodGL, GetAIPSIOs.fld_SAPIO, GetAIPSIOs.fld_SAPType);
                        message = "Transaction Listing (AIPS). (Data - Code Activity : " + AIPS.fldOptConfValue + ", Amount : RM " + getAmount + ")";

                        Log += i == 1 ? DateTimeFunc.GetDateTime() + " - " + message : "\r\n" + DateTimeFunc.GetDateTime() + " - " + message;
                        i++;
                    }
                }
                AdminSCTransList.Where(c => c.fld_KodGL != null).ToList().ForEach(cc => cc.fld_Jumlah = 0);
            }
            db.Dispose();
            db2.Dispose();
        }
        public void GetAmountWorkerSalaryFunc(int? NegaraID, int? SyarikatID, int? WilayahID, int? LadangID, int? UserID, DateTime DTProcess, int? Month, int? Year, string processname, string servicesname, int? ClientID, out string Log, List<tbl_Pkjmast> PkjMastList, string compCode)
        {
            GenSalaryModelHQ db = new GenSalaryModelHQ();
            GetConnectFunc conn = new GetConnectFunc();
            string host, catalog, user, pass = "";
            conn.GetConnection(out host, out catalog, out user, out pass, WilayahID, SyarikatID, NegaraID);
            GenSalaryModelEstate db2 = GenSalaryModelEstate.ConnectToSqlServer(host, catalog, user, pass);
            Log = "";
            string message = "";
            int i = 1;

            var GetWorkerSalary = db.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "gajipkj" && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fldDeleted == false).FirstOrDefault();
            var NoPkjList = PkjMastList.Select(s => s.fld_Nopkj).ToArray();

            var description = "";
            decimal? Amount = 0;
            decimal? totalAmount = 0;

            var salariesList = db2.vw_PaySheetPekerja.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Month == Month && x.fld_Year == Year && NoPkjList.Contains(x.fld_Nopkj)).ToList();

            #region Tunai - fld_Kategori - 11
            Amount = salariesList.Where(x => x.fld_PaymentMode == "1").Sum(s => s.fld_GajiBersih);
            Amount = Amount == null ? 0 : Amount;
            if (Amount != 0)
            {
                description = GetWorkerSalary.fldOptConfDesc + " Tunai";
                var GetSalaryGL = db.tbl_CustomerVendorGLMap.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_KodAktiviti == GetWorkerSalary.fldOptConfValue && x.fld_Flag == "4" && x.fld_TypeCode == "GL" && x.fld_Deleted == false && x.fld_compcode == compCode).Select(s => s.fld_SAPCode).FirstOrDefault();
                AddTo_tbl_Sctran(db2, NegaraID, SyarikatID, WilayahID, LadangID, Amount, 0, "-", GetWorkerSalary.fldOptConfValue.Substring(0, 2), GetWorkerSalary.fldOptConfValue, GetWorkerSalary.fldOptConfFlag2, description, DTProcess, UserID, Month, Year, "C", 11, GetSalaryGL, "-", ""); //modified  by kamalia 17/3/22
                message = "Transaction Listing (Worker Salary). (Data - Code Activity : " + GetWorkerSalary.fldOptConfValue + ", Amount : RM " + Amount + ")";
                Log += i == 1 ? DateTimeFunc.GetDateTime() + " - " + message : "\r\n" + DateTimeFunc.GetDateTime() + " - " + message;
                totalAmount += Amount;
            }
            #endregion

            #region Cek - fld_Kategori - 12
            Amount = salariesList.Where(x => x.fld_PaymentMode == "2").Sum(s => s.fld_GajiBersih);
            Amount = Amount == null ? 0 : Amount;
            if (Amount != 0)
            {
                description = GetWorkerSalary.fldOptConfDesc + " Cheque";
                var GetSalaryGL = db.tbl_CustomerVendorGLMap.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_KodAktiviti == GetWorkerSalary.fldOptConfValue && x.fld_Flag == "4" && x.fld_TypeCode == "GL" && x.fld_Deleted == false && x.fld_compcode == compCode).Select(s => s.fld_SAPCode).FirstOrDefault();
                AddTo_tbl_Sctran(db2, NegaraID, SyarikatID, WilayahID, LadangID, Amount, 0, "-", GetWorkerSalary.fldOptConfValue.Substring(0, 2), GetWorkerSalary.fldOptConfValue, GetWorkerSalary.fldOptConfFlag2, description, DTProcess, UserID, Month, Year, "C", 12, GetSalaryGL, "-", ""); //modified  by kamalia 17/3/22
                message = "Transaction Listing (Worker Salary). (Data - Code Activity : " + GetWorkerSalary.fldOptConfValue + ", Amount : RM " + Amount + ")";
                Log += i == 1 ? DateTimeFunc.GetDateTime() + " - " + message : "\r\n" + DateTimeFunc.GetDateTime() + " - " + message;
                totalAmount += Amount;
            }
            #endregion

            #region EWallet - fld_Kategori - 13
            Amount = salariesList.Where(x => x.fld_PaymentMode == "3").Sum(s => s.fld_GajiBersih);
            Amount = Amount == null ? 0 : Amount;
            if (Amount != 0)
            {
                description = GetWorkerSalary.fldOptConfDesc + " eWallet";
                var GetSalaryGL = db.tbl_CustomerVendorGLMap.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_KodAktiviti == GetWorkerSalary.fldOptConfValue && x.fld_Flag == "4" && x.fld_TypeCode == "GL" && x.fld_Deleted == false && x.fld_compcode == compCode).Select(s => s.fld_SAPCode).FirstOrDefault();
                AddTo_tbl_Sctran(db2, NegaraID, SyarikatID, WilayahID, LadangID, Amount, 0, "-", GetWorkerSalary.fldOptConfValue.Substring(0, 2), GetWorkerSalary.fldOptConfValue, GetWorkerSalary.fldOptConfFlag2, description, DTProcess, UserID, Month, Year, "C", 13, GetSalaryGL, "-", ""); //modified  by kamalia 17/3/22
                message = "Transaction Listing (Worker Salary). (Data - Code Activity : " + GetWorkerSalary.fldOptConfValue + ", Amount : RM " + Amount + ")";
                Log += i == 1 ? DateTimeFunc.GetDateTime() + " - " + message : "\r\n" + DateTimeFunc.GetDateTime() + " - " + message;
                totalAmount += Amount;
            }
            #endregion

            #region Maybank CDMAS - fld_Kategori - 14
            Amount = salariesList.Where(x => x.fld_PaymentMode == "4").Sum(s => s.fld_GajiBersih);
            Amount = Amount == null ? 0 : Amount;
            if (Amount != 0)
            {
                description = GetWorkerSalary.fldOptConfDesc + " Maybank - CDMAS";
                var GetSalaryGL = db.tbl_CustomerVendorGLMap.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_KodAktiviti == GetWorkerSalary.fldOptConfValue && x.fld_Flag == "4" && x.fld_TypeCode == "GL" && x.fld_Deleted == false && x.fld_compcode == compCode).Select(s => s.fld_SAPCode).FirstOrDefault();
                AddTo_tbl_Sctran(db2, NegaraID, SyarikatID, WilayahID, LadangID, Amount, 0, "-", GetWorkerSalary.fldOptConfValue.Substring(0, 2), GetWorkerSalary.fldOptConfValue, GetWorkerSalary.fldOptConfFlag2, description, DTProcess, UserID, Month, Year, "C", 14, GetSalaryGL, "-", ""); //modified  by kamalia 17/3/22
                message = "Transaction Listing (Worker Salary). (Data - Code Activity : " + GetWorkerSalary.fldOptConfValue + ", Amount : RM " + Amount + ")";
                Log += i == 1 ? DateTimeFunc.GetDateTime() + " - " + message : "\r\n" + DateTimeFunc.GetDateTime() + " - " + message;
                totalAmount += Amount;
            }
            #endregion

            #region Maybank M2E Estate - fld_Kategori - 15
            Amount = salariesList.Where(x => x.fld_PaymentMode == "5").Sum(s => s.fld_GajiBersih);
            Amount = Amount == null ? 0 : Amount;
            if (Amount != 0)
            {
                description = GetWorkerSalary.fldOptConfDesc + " Maybank - M2E Estate";
                var GetSalaryGL = db.tbl_CustomerVendorGLMap.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_KodAktiviti == GetWorkerSalary.fldOptConfValue && x.fld_Flag == "4" && x.fld_TypeCode == "GL" && x.fld_Deleted == false && x.fld_compcode == compCode).Select(s => s.fld_SAPCode).FirstOrDefault();
                AddTo_tbl_Sctran(db2, NegaraID, SyarikatID, WilayahID, LadangID, Amount, 0, "-", GetWorkerSalary.fldOptConfValue.Substring(0, 2), GetWorkerSalary.fldOptConfValue, GetWorkerSalary.fldOptConfFlag2, description, DTProcess, UserID, Month, Year, "C", 15, GetSalaryGL, "-", ""); //modified  by kamalia 17/3/22
                message = "Transaction Listing (Worker Salary). (Data - Code Activity : " + GetWorkerSalary.fldOptConfValue + ", Amount : RM " + Amount + ")";
                Log += i == 1 ? DateTimeFunc.GetDateTime() + " - " + message : "\r\n" + DateTimeFunc.GetDateTime() + " - " + message;
                totalAmount += Amount;
            }
            #endregion

            #region Maybank M2E HQ - fld_Kategori - 16
            Amount = salariesList.Where(x => x.fld_PaymentMode == "6").Sum(s => s.fld_GajiBersih);
            Amount = Amount == null ? 0 : Amount;
            if (Amount != 0)
            {
                description = GetWorkerSalary.fldOptConfDesc + " Maybank - M2E HQ";
                var GetSalaryGL = db.tbl_CustomerVendorGLMap.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_KodAktiviti == GetWorkerSalary.fldOptConfValue && x.fld_Flag == "4" && x.fld_TypeCode == "GL" && x.fld_Deleted == false && x.fld_compcode == compCode).Select(s => s.fld_SAPCode).FirstOrDefault();
                AddTo_tbl_Sctran(db2, NegaraID, SyarikatID, WilayahID, LadangID, Amount, 0, "-", GetWorkerSalary.fldOptConfValue.Substring(0, 2), GetWorkerSalary.fldOptConfValue, GetWorkerSalary.fldOptConfFlag2, description, DTProcess, UserID, Month, Year, "C", 16, GetSalaryGL, "-", ""); //modified  by kamalia 17/3/22
                message = "Transaction Listing (Worker Salary). (Data - Code Activity : " + GetWorkerSalary.fldOptConfValue + ", Amount : RM " + Amount + ")";
                Log += i == 1 ? DateTimeFunc.GetDateTime() + " - " + message : "\r\n" + DateTimeFunc.GetDateTime() + " - " + message;
                totalAmount += Amount;
            }
            #endregion


            if (totalAmount > 0)
            {
                AddTo_tbl_Skb(db2, NegaraID, SyarikatID, WilayahID, LadangID, totalAmount, Month, Year);
            }

            db.Dispose();
            db2.Dispose();
        }


        public void GetDebitCreditBalanceFunc(int? NegaraID, int? SyarikatID, int? WilayahID, int? LadangID, int? UserID, DateTime DTProcess, int? Month, int? Year, string processname, string servicesname, int? ClientID, out string Log)
        {
            GetConnectFunc conn = new GetConnectFunc();
            string host, catalog, user, pass = "";
            conn.GetConnection(out host, out catalog, out user, out pass, WilayahID, SyarikatID, NegaraID);
            GenSalaryModelEstate db2 = GenSalaryModelEstate.ConnectToSqlServer(host, catalog, user, pass);
            Log = "";
            string message = "";
            decimal? Debit = 0;
            decimal? Credit = 0;
            int i = 1;

            var CheckCloseBizTable = db2.tbl_TutupUrusNiaga.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Month == Month && x.fld_Year == Year).FirstOrDefault();

            var TransactionListingList = db2.tbl_Sctran
                    .Where(x => x.fld_KodAktvt != "3803" && x.fld_KodAktvt != "3800" && x.fld_KodAktvt != "3806" && x.fld_Month == Month &&
                                x.fld_Year == Year && x.fld_NegaraID == NegaraID &&
                                x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID &&
                                x.fld_LadangID == LadangID).ToList();

            Debit = TransactionListingList.Where(x => x.fld_KdCaj == "D").Sum(s => s.fld_Amt);
            Credit = TransactionListingList.Where(x => x.fld_KdCaj == "C").Sum(s => s.fld_Amt);

            if (CheckCloseBizTable != null)
            {
                CheckCloseBizTable.fld_StsTtpUrsNiaga = false;
                CheckCloseBizTable.fld_Debit = Debit;
                CheckCloseBizTable.fld_Credit = Credit;
                CheckCloseBizTable.fld_CreatedDT = DTProcess;
                CheckCloseBizTable.fld_CreatedBy = UserID;
                db2.Entry(CheckCloseBizTable).State = EntityState.Modified;
                db2.SaveChanges();
            }
            else
            {
                tbl_TutupUrusNiaga tbl_TutupUrusNiaga = new tbl_TutupUrusNiaga();

                tbl_TutupUrusNiaga.fld_NegaraID = NegaraID;
                tbl_TutupUrusNiaga.fld_SyarikatID = SyarikatID;
                tbl_TutupUrusNiaga.fld_WilayahID = WilayahID;
                tbl_TutupUrusNiaga.fld_LadangID = LadangID;
                tbl_TutupUrusNiaga.fld_StsTtpUrsNiaga = false;
                tbl_TutupUrusNiaga.fld_Year = Year;
                tbl_TutupUrusNiaga.fld_Month = Month;
                tbl_TutupUrusNiaga.fld_CreatedDT = DTProcess;
                tbl_TutupUrusNiaga.fld_CreatedBy = UserID;
                tbl_TutupUrusNiaga.fld_Debit = Debit;
                tbl_TutupUrusNiaga.fld_Credit = Credit;
                db2.tbl_TutupUrusNiaga.Add(tbl_TutupUrusNiaga);
                db2.SaveChanges();
            }

            AddTo_tbl_Skb(db2, NegaraID, SyarikatID, WilayahID, LadangID, Credit, Month, Year);

            message = "Debit Credit Balance. (Data - Debit Amount : RM " + Debit + ", Credit Amount : RM " + Credit + ")";
            Log += i == 1 ? DateTimeFunc.GetDateTime() + " - " + message : "\r\n" + DateTimeFunc.GetDateTime() + " - " + message;

            db2.Dispose();
        }

        public string FirstCharToUpper(string s)
        {
            char[] array = s.ToCharArray();

            if (array.Length >= 1)
            {
                if (char.IsLower(array[0]))
                {
                    array[0] = char.ToUpper(array[0]);
                }
            }

            for (int i = 1; i < array.Length; i++)
            {
                if (array[i - 1] == ' ')
                {
                    if (char.IsLower(array[i]))
                    {
                        array[i] = char.ToUpper(array[i]);
                    }
                }
            }

            return new string(array);
        }

        public void Add_tbl_AuditTrail(int? NegaraID, int? SyarikatID, int? WilayahID, int? LadangID, int? Year)
        {
            GenSalaryModelHQ db = new GenSalaryModelHQ();
            tbl_AuditTrail tbl_AuditTrail = new tbl_AuditTrail();

            var checkAuditTrail = db.tbl_AuditTrail.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Thn == Year).FirstOrDefault();

            if (checkAuditTrail == null)
            {
                tbl_AuditTrail.fld_Bln1 = 0;
                tbl_AuditTrail.fld_Bln2 = 0;
                tbl_AuditTrail.fld_Bln3 = 0;
                tbl_AuditTrail.fld_Bln4 = 0;
                tbl_AuditTrail.fld_Bln5 = 0;
                tbl_AuditTrail.fld_Bln6 = 0;
                tbl_AuditTrail.fld_Bln7 = 0;
                tbl_AuditTrail.fld_Bln8 = 0;
                tbl_AuditTrail.fld_Bln9 = 0;
                tbl_AuditTrail.fld_Bln10 = 0;
                tbl_AuditTrail.fld_Bln11 = 0;
                tbl_AuditTrail.fld_Bln12 = 0;
                tbl_AuditTrail.fld_NegaraID = NegaraID;
                tbl_AuditTrail.fld_SyarikatID = SyarikatID;
                tbl_AuditTrail.fld_WilayahID = WilayahID;
                tbl_AuditTrail.fld_LadangID = LadangID;
                tbl_AuditTrail.fld_Thn = Year;
                tbl_AuditTrail.fld_Deleted = false;

                db.tbl_AuditTrail.Add(tbl_AuditTrail);
                db.SaveChanges();
            }
        }

        public void AddTo_tbl_Skb(GenSalaryModelEstate db2, int? NegaraID, int? SyarikatID, int? WilayahID, int? LadangID, decimal? Amount, int? Month, int? Year)
        {
            string monthstring = Month.ToString();
            tbl_Skb tbl_Skb = new tbl_Skb();
            if (monthstring.Length == 1)
            {
                monthstring = "0" + monthstring;
            }
            var CheckSkb = db2.tbl_Skb.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Tahun == Year && x.fld_Bulan == monthstring).FirstOrDefault();
            if (CheckSkb != null)
            {
                CheckSkb.fld_GajiBersih = Amount;
                db2.Entry(CheckSkb).State = EntityState.Modified;
                db2.SaveChanges();
            }
            else
            {
                tbl_Skb.fld_GajiBersih = Amount;
                tbl_Skb.fld_NegaraID = NegaraID;
                tbl_Skb.fld_SyarikatID = SyarikatID;
                tbl_Skb.fld_WilayahID = WilayahID;
                tbl_Skb.fld_LadangID = LadangID;
                tbl_Skb.fld_Tahun = Year;
                tbl_Skb.fld_Bulan = monthstring;
                tbl_Skb.fld_Deleted = false;
                db2.tbl_Skb.Add(tbl_Skb);
                db2.SaveChanges();
            }
        }

        public void AddTo_tbl_Sctran(GenSalaryModelEstate db2, int? NegaraID, int? SyarikatID, int? WilayahID, int? LadangID, decimal? Amount, byte? JnsPkt, string KodPkt, string JnisAktvt, string KodAktvt, string KodGL, string Keterangan, DateTime DTProcess, int? UserID, int? Month, int? Year, string KdCaj, byte? Kategori, string GLCode, string IOCode, string sapType)
        {
            tbl_Sctran Sctran = new tbl_Sctran();

            Sctran.fld_NegaraID = NegaraID;
            Sctran.fld_SyarikatID = SyarikatID;
            Sctran.fld_WilayahID = WilayahID;
            Sctran.fld_LadangID = LadangID;
            Sctran.fld_Month = Month;
            Sctran.fld_Year = Year;
            Sctran.fld_JnisAktvt = JnisAktvt;
            Sctran.fld_KodAktvt = KodAktvt;
            Sctran.fld_JnsPkt = JnsPkt;
            Sctran.fld_KodPkt = KodPkt;
            Sctran.fld_KodGL = KodGL;
            Sctran.fld_Keterangan = Keterangan;
            Sctran.fld_KdCaj = KdCaj;
            Sctran.fld_CreatedDT = DTProcess;
            Sctran.fld_CreatedBy = UserID;
            Sctran.fld_Amt = Amount;
            Sctran.fld_Kategori = Kategori;
            Sctran.fld_GL = GLCode;
            Sctran.fld_IO = IOCode;
            Sctran.fld_SAPType = sapType;

            db2.tbl_Sctran.Add(Sctran);
            db2.SaveChanges();
        }
        //untuk devide nilai setiap gl dan io pekerja yg lebih 1
        static IEnumerable<decimal> SplitValue2(decimal value, int count)
        {
            if (count <= 0) throw new ArgumentException("count must be greater than zero.", "count");
            var result = new decimal[count];

            decimal runningTotal = 0M;
            for (int i = 0; i < count; i++)
            {
                var remainder = value - runningTotal;
                var share = remainder > 0M ? Math.Max(Math.Round(remainder / ((decimal)(count - i)), 2), .01M) : 0M;
                result[i] = share;
                runningTotal += share;
            }
            if (runningTotal < value) result[count - 1] += value - runningTotal;

            return result;
        }

        public List<CustMod_AdminSCTransAmt> GetAmountAfterDevide(List<CustMod_AdminSCTrans> adminSCTransList, decimal? amount)
        {
            var adminSCTransAm = new List<CustMod_AdminSCTransAmt>();

            var totalWorkingWorker = adminSCTransList.Sum(s => s.fld_TotalWorking);
            var amountA = 0m;
            int i = 1;
            foreach (var adminSCTrans in adminSCTransList)
            {
                var amountB = (Convert.ToDecimal(adminSCTrans.fld_TotalWorking) / Convert.ToDecimal(totalWorkingWorker)) * amount.Value;
                amountB = Math.Round(amountB, 2);
                amountA += amountB;
                if (amountA > amount.Value)
                {
                    amountB = amount.Value - adminSCTransAm.Sum(s => s.fld_Jumlah.Value);
                }

                if (adminSCTransAm.Sum(s => s.fld_Jumlah.Value) + amountB != amount.Value && adminSCTransList.Count() == i)
                {
                    amountB = amount.Value - adminSCTransAm.Sum(s => s.fld_Jumlah.Value);
                }
                adminSCTransAm.Add(new CustMod_AdminSCTransAmt { fld_SAPIO = adminSCTrans.fld_SAPIO, fld_KodGL = adminSCTrans.fld_KodGL, fld_KodPkt = adminSCTrans.fld_KodPkt, fld_Jumlah = amountB });

                i++;
            }

            if (adminSCTransAm.Sum(s => s.fld_Jumlah.Value) != amount.Value)
            {

            }

            return adminSCTransAm;
        }
    }
}
