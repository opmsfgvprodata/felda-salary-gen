﻿namespace SalaryGeneratorServices.ModelsEstate
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class tbl_SAPPostRef
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid fld_ID { get; set; }

        [StringLength(50)]
        public string fld_HeaderText { get; set; }

        public int? fld_Month { get; set; }

        public int? fld_Year { get; set; }

        [StringLength(12)]
        public string fld_CompCode { get; set; }

        [Column(TypeName = "date")]
        public DateTime? fld_DocDate { get; set; }



        [Column(TypeName = "date")]
        public DateTime? fld_PostingDate { get; set; }

        [StringLength(10)]
        public string fld_DocType { get; set; }

        [StringLength(50)]
        public string fld_RefNo { get; set; }

        [StringLength(50)]
        public string fld_NoDocSAP { get; set; }

        public short? fld_Purpose { get; set; }


        public int? fld_NegaraID { get; set; }

        public int? fld_SyarikatID { get; set; }

        public int? fld_WilayahID { get; set; }

        public int? fld_LadangID { get; set; }

        public bool? fld_StatusProceed { get; set; }

        public int? fld_CreatedBy { get; set; }

        public DateTime? fld_CreatedDT { get; set; }

        public int? fld_ModifiedBy { get; set; }

        public DateTime? fld_ModifiedDT { get; set; }
    }
}
