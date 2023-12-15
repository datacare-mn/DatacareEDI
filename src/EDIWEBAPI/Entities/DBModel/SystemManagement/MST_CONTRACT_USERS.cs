using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace EDIWEBAPI.Entities.DBModel.SystemManagement
{
    public class MST_CONTRACT_USERS
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Required]
        public int ID { get; set; }

        public int CONTRACTID { get; set; }

        public int USERID { get; set; }

        public DateTime INSYMD { get; set; }

        public int INSEMP { get; set; }

        public DateTime UPDYMD { get; set; }

        public int UPDEMP { get; set; }
        [ForeignKey("CONTRACTID")]
        public virtual MST_CONTRACT MST_CONTRACT { get; set; }
        [ForeignKey("USERID")]
        public virtual SYSTEM_USERS SYSTEM_USERS { get; set; }
    }
}
