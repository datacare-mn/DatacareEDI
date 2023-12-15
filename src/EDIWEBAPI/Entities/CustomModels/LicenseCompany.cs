using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EDIWEBAPI.Entities.CustomModels
{
    public class LicenseCompany
    {
        public int ID { get; set; }
        public string REGNO { get; set; }
        public decimal ctrcnt { get; set; }
        public int CONTRACTSCORE { get; set; }
        public decimal invamt { get; set; }
        public int SALESCORE { get; set; }
        public int skucnt { get; set; }
        public int SKUSCORE { get; set; }
        public int TOTALSCORE { get; set; }
        public decimal Price { get; set; }
        public decimal MaxUserPrice { get; set; }
        public string SaleDesc { get; set; }
        public string payctrcd { get; set; }
        public string paycycle { get; set; }
        public string payjumcd { get; set; }
        public bool NoPayment { get; set; }

        public void FillLicense(APIModel.LicenseVariables license)
        {
            this.ctrcnt = license.ctrcnt;
            this.invamt = license.invamt ?? 0;
            this.skucnt = license.skucnt ?? 0;
            this.payctrcd = license.payctrcd;
            this.paycycle = license.paycycle;
            this.payjumcd = license.payjumcd;
        }
    }
}
