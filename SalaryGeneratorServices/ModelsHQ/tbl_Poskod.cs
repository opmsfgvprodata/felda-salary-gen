namespace SalaryGeneratorServices.ModelsHQ
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class tbl_Poskod
    {
        [Key]
        public int fld_ID { get; set; }

        [StringLength(10)]
        public string fld_Postcode { get; set; }

        [StringLength(50)]
        public string fld_DistrictArea { get; set; }

        [StringLength(50)]
        public string fld_State { get; set; }

        [StringLength(50)]
        public string fld_Region { get; set; }

        public bool? fld_deleted { get; set; }
    }
}
