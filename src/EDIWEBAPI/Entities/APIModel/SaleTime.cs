using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EDIWEBAPI.Entities.APIModel
{
    public class SaleTime
    {
        public string barcode { get; set; } //skucd

        public string prodname { get; set; } //skunm

        public string seq { get; set; }
        public string tp { get; set; }
        public decimal? hh09 { get; set; }
        public decimal? hh10 { get; set; }
        public decimal? hh11 { get; set; }
        public decimal? hh12 { get; set; }
        public decimal? hh13 { get; set; }
        public decimal? hh14 { get; set; }
        public decimal? hh15 { get; set; }
        public decimal? hh16 { get; set; }
        public decimal? hh17 { get; set; }
        public decimal? hh18 { get; set; }
        public decimal? hh19 { get; set; }
        public decimal? hh20 { get; set; }
        public decimal? hh21 { get; set; }
        public decimal? hh22 { get; set; }



        public decimal? total { get; set; }

        public void Append(SaleTime source)
        {
            hh09 = hh09 + source.hh09;
            hh10 = hh10 + source.hh10;
            hh11 = hh11 + source.hh11;
            hh12 = hh12 + source.hh12;
            hh13 = hh13 + source.hh13;
            hh14 = hh14 + source.hh14;
            hh15 = hh15 + source.hh15;
            hh16 = hh16 + source.hh16;
            hh17 = hh17 + source.hh17;
            hh18 = hh18 + source.hh18;
            hh19 = hh19 + source.hh19;
            hh20 = hh20 + source.hh20;
            hh21 = hh21 + source.hh21;
            hh22 = hh22 + source.hh22;

            Calculate();
        }

        public void Calculate()
        {
            total = hh09 + hh10 + hh11 + hh12 + hh13 + hh14 + hh15 + hh16 + hh17 + hh18 + hh19 + hh20 + hh21 + hh22;
        }
    }
}
