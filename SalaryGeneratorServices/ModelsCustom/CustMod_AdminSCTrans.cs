using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SalaryGeneratorServices.CustomModels
{
    public partial class CustMod_AdminSCTrans
    {
        public byte? fld_JnsPkt { get; set; }
        [StringLength(20)]
        public string fld_Nopkj { get; set; }

        [StringLength(40)]
        public string fld_Nama { get; set; }

        [StringLength(10)]
        public string fld_KodPkt { get; set; }

        [StringLength(2)]
        public string fld_JnisAktvt { get; set; }

        [StringLength(4)]
        public string fld_KodAktvt { get; set; }

        [StringLength(5)]
        public string fld_KodGL { get; set; }

        [StringLength(5)]
        public string fld_PaySheetID { get; set; }

        public string fld_SAPGL { get; set; }

        public string fld_SAPIO { get; set; }

        public string fld_Keterangan { get; set; }

        public int fld_TotalWorking { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? fld_Jumlah { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? fld_JumlahPkj { get; set; }
        [Column(TypeName = "numeric")]
        public decimal? fld_JumlahMjk { get; set; }
        [Column(TypeName = "numeric")]
        public decimal? fld_JumlahMix { get; set; }
        public string fld_SAPType { get; set; }
    }

    public class CustMod_AdminSCTransAmt
    {
        public decimal? fld_Jumlah { get; set; }
        public string fld_KodPkt { get; set; }
        public string fld_SAPIO { get; set; }
        public string fld_KodGL { get; set; }
        public string fld_PaySheetID { get; set; }
        public string fld_JnsAktvt { get; set; }
    }
}