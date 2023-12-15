using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace EDIWEBAPI.Entities.License
{
    public class BankStatement
    {

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Required]
        public decimal STATEMENTID { get; set; }
        [Required]
        public string ACCOUNTNO { get; set; }

        public string ACCOUNTNAME { get; set; }
        [Required]
        public string BANKID { get; set; }
        [Required]
        public DateTime TRANSACDATE { get; set; }

        public string BRANCH { get; set; }

        public string TELLER { get; set; }

        public string TRANCODE { get; set; }

        public string JOURNAL { get; set; }
        [Required]
        public decimal DEBIT { get; set; }
        [Required]
        public decimal CREDIT { get; set; }

        public string TRANSFERACCOUNT { get; set; }

        public string DESCRIPTION { get; set; }
        [Required]
        public DateTime INSERTDATE { get; set; }

        public int? SYSTEMID { get; set; }

        public DateTime? SENDDATE { get; set; }
        [Required]
        public int ISSEND { get; set; }

        public string DESCVALUE { get; set; }
    }
}
