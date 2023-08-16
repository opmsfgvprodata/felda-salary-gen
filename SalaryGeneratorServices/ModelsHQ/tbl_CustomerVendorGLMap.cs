﻿namespace SalaryGeneratorServices.ModelsHQ
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class tbl_CustomerVendorGLMap
    {
        [Key]
        public Guid fld_ID { get; set; }

        [StringLength(10)]
        public string fld_KodAktiviti { get; set; }

        [StringLength(10)]
        public string fld_Flag { get; set; }

        [StringLength(12)]
        public string fld_TypeCode { get; set; }

        [StringLength(12)]
        public string fld_SAPCode { get; set; }

        public int? fld_NegaraID { get; set; }

        public int? fld_SyarikatID { get; set; }

        public int? fld_WIlayahID { get; set; }

        public int? fld_LadangID { get; set; }

        public bool? fld_Deleted { get; set; }
        //add by kamalia 17/12/21
        [StringLength(20)]
        public string fld_VendorNo { get; set; }

        //add by kamalia 15/2/22
        [StringLength(1)]
        public string fld_JnsLot { get; set; }

        [StringLength(5)]
        public string fld_Paysheet { get; set; }
        [StringLength(1)]
        public string fld_StatusTnmn { get; set; }

        [StringLength(5)]
        public string fld_compcode { get; set; }

        [StringLength(10)]
        public string fld_SAPType { get; set; }
        //end
    }
}
