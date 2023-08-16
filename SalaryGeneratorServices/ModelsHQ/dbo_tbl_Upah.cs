namespace SalaryGeneratorServices.ModelsHQ
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("[dbo.tbl_Upah]")]
    public partial class dbo_tbl_Upah
    {
        [Key]
        public int fld_ID { get; set; }

        [StringLength(50)]
        public string fld_Kawasan { get; set; }

        [StringLength(50)]
        public string fld_JnsKerja { get; set; }

        [StringLength(50)]
        public string fld_Perinician { get; set; }

        [StringLength(50)]
        public string fld_Unit { get; set; }

        public decimal? fld_Harga { get; set; }

        [StringLength(50)]
        public string fld_Produktiviti { get; set; }

        [StringLength(50)]
        public string fld_KodAktvt { get; set; }

        [StringLength(50)]
        public string fld_Checkroll { get; set; }

        public int? fld_NegaraID { get; set; }

        public int? fld_SyarikatID { get; set; }

        public bool? fld_Deleted { get; set; }
    }
}
