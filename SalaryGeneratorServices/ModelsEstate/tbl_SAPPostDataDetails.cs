namespace SalaryGeneratorServices.ModelsEstate
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class tbl_SAPPostDataDetails
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid fld_ID { get; set; }

        public int? fld_ItemNo { get; set; }

        [StringLength(12)]
        public string fld_GL { get; set; }

        [StringLength(25)]
        public string fld_IO { get; set; }

        [StringLength(12)]
        public string fld_SAPActivityCode { get; set; }

        public decimal? fld_Amount { get; set; }

        [StringLength(100)]
        public string fld_Desc { get; set; }

        [StringLength(10)]
        public string fld_Currency { get; set; }


        public Guid? fld_SAPPostRefID { get; set; }

        [StringLength(10)]
        public string fld_Purpose { get; set; }
        //add by kamalia 17/12/21
        [StringLength(20)]
        public string fld_VendorCode { get; set; }

        //added by kamalia 15/2/22
        [StringLength(100)]
        public string fld_DocNoSAP { get; set; }

        public int? fld_flag { get; set; }

        [StringLength(10)]
        public string fld_SAPType { get; set; }
    }
}
