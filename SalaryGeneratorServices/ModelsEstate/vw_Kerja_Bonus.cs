namespace SalaryGeneratorServices.ModelsEstate
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class vw_Kerja_Bonus
    {
        [Key]
        public Guid fld_ID { get; set; }

        [StringLength(10)]
        public string fld_Nopkj { get; set; }

        [StringLength(50)]
        public string fld_Kum { get; set; }

        [Column(TypeName = "date")]
        public DateTime? fld_Tarikh { get; set; }

        public byte? fld_JnsPkt { get; set; }

        [StringLength(10)]
        public string fld_KodPkt { get; set; }

        [StringLength(2)]
        public string fld_JnisAktvt { get; set; }

        [StringLength(4)]
        public string fld_KodAktvt { get; set; }

        public byte? fld_Bonus { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? fld_Kadar_B { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? fld_Jumlah_B { get; set; }

        public int? fld_CreatedBy { get; set; }

        public DateTime? fld_CreatedDT { get; set; }

        public int? fld_NegaraID { get; set; }

        public int? fld_SyarikatID { get; set; }

        public int? fld_WilayahID { get; set; }

        public int? fld_LadangID { get; set; }

        [StringLength(5)]
        public string fld_KodGL { get; set; }

        [StringLength(10)]
        public string fld_PaySheetID { get; set; }

        [StringLength(15)]
        public string fld_GLKod { get; set; }

        [StringLength(20)]
        public string fld_IOKod { get; set; }
    }
}
