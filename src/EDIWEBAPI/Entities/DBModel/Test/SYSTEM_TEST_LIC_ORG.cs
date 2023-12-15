using EDIWEBAPI.Enums;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace EDIWEBAPI.Entities.DBModel.SystemManagement
{
    public class SYSTEM_TEST_LIC_ORG
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Required]
        public int ID { get; set; }
        public string REGNO { get; set; }
        public string PAYCTRCD { get; set; }
        public string PAYCYCLE { get; set; }
        public string PAYJUMCD { get; set; }
        public int CTRCNT { get; set; }
        public int SKUCNT { get; set; }
        public decimal INVAMT { get; set; }
    }
}
