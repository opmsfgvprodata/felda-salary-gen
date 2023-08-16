using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SalaryGeneratorServices.CustomModels
{
    public partial class CustMod_DistributeAmount
    {
        public int? fld_NegaraID { get; set; }

        public int? fld_SyarikatID { get; set; }

        public int? fld_WilayahID { get; set; }

        public int? fld_LadangID { get; set; }
        public byte? fld_JnsPkt { get; set; }

        [StringLength(10)]
        public string fld_KodPkt { get; set; }

        [StringLength(2)]
        public string fld_JnisAktvt { get; set; }

        [StringLength(10)]
        public string fld_SocsoType{ get; set; }

        [StringLength(5)]
        public string fld_KodGL { get; set; }

        public string fld_SAPGL { get; set; }

        public string fld_SAPIO { get; set; }

        public string fld_Keterangan { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? fld_Jumlah { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? fld_JumlahPkj { get; set; }
        [Column(TypeName = "numeric")]
        public decimal? fld_JumlahMjk { get; set; }
        [Column(TypeName = "numeric")]
        public decimal? fld_JumlahMix { get; set; }
    }
}