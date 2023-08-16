﻿namespace SalaryGeneratorServices.ModelsHQ
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class tbl_VDSAP
    {
        [Key]
        public string fld_VendorNo { get; set; }
        public string fld_Desc { get; set; }
        public int? fld_NegaraID { get; set; }
        public int? fld_SyarikatID { get; set; }
        public DateTime? fld_DTCreated { get; set; }
        public DateTime? fld_DTModified { get; set; }
        public bool? fld_Deleted { get; set; }
        //farahin tambah - 3/8/2021
        public string fld_CreatedBy { get; set; }
        public string fld_ModifiedBy { get; set; }
        public string fld_CompanyCode { get; set; }
        public string fld_VendorInd { get; set; }
    }
}
