using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace EDIWEBAPI.Utils
{
    public static class CurrencyAmountSpeller
    {
        private static string numName { get; set; }
        private static decimal amount { get; set; }

        public static string SpellMoney(string number, bool showdecimal = false)
        {
            int minus = 0;
            if (Convert.ToDecimal(number) > 0)
            {
                minus = 1;
            }
            else
            {
                number = number.Replace("-", "");
            }
            if (!number.Contains(".")) number += ".00";
            var decimalMoney = number.Substring(number.IndexOf('.') + 1);
            var wholeMoney = number.Substring(0, number.IndexOf('.'));

            numName = "";
            amount = Convert.ToDecimal(wholeMoney);
            if (wholeMoney == "0") numName = "0 ";
            else RekursivConvert("" + amount);
            var moneyName = numName;
            numName = "";
            amount = Convert.ToDecimal(decimalMoney);
            if (amount == 0) numName = "тэг";
            else RekursivConvert("" + amount);
            if (showdecimal)
            {
                return  minus == 1 ? string.Format("{0} {2} {1} {3}", moneyName, numName, "төгрөг", "мөнгө").ToLower() : string.Format("хасах {0} {2} {1} {3}", moneyName, numName, "төгрөг", "мөнгө").ToLower();
            }
            return  minus == 1 ? string.Format("{0} {1}", moneyName,  "төгрөг").ToLower() : string.Format("хасах {0} {1}", moneyName, "төгрөг").ToLower();
        }


        private static string NumNames(string num, bool type)
        {
            if (num == "1")
                if (type) return "нэг";
                else return "арван";
            else if (num == "2")
                if (type) return "хоёр";
                else return "хорин";
            else if (num == "3")
                if (type) return "гурван";
                else return "гучин";
            else if (num == "4")
                if (type) return "дөрвөн";
                else return "дөчин";
            else if (num == "5")
                if (type) return "таван";
                else return "тавин";
            else if (num == "6")
                if (type) return "зургаан";
                else return "жаран";
            else if (num == "7")
                if (type) return "долоон";
                else return "далан";
            else if (num == "8")
                if (type) return "найман";
                else return "наян";
            else if (num == "9")
                if (type) return "есөн";
                else return "ерэн";
            return "";
        }

        private static void RekursivConvert(string num)
        {
            if (num.Length > 0)
                if (num.Length == 1)
                {
                    if (num != "0")
                        if (num.Equals("1")
                            && ((amount.ToString(CultureInfo.InvariantCulture).Length != 1
                                 && amount.ToString(CultureInfo.InvariantCulture).Length < 4)
                                || (amount.ToString(CultureInfo.InvariantCulture).Length >= 5
                                    && Convert.ToDecimal(
                                            amount.ToString(CultureInfo.InvariantCulture).Substring(
                                                    amount.ToString(CultureInfo.InvariantCulture).Length - 3, 2)) != 0))) numName += " нэгэн";
                        else numName += " " + NumNames(num, true);
                }
                else if (num.Length == 2)
                {
                    if (num.Substring(0, 1) != "0") numName += " " + NumNames(num.Substring(0, 1), false);
                    RekursivConvert(num.Substring(1));
                }
                else if (num.Length == 3)
                {
                    if (num.Substring(0, 1) != "0") numName += string.Format(" {0} зуун", NumNames(num.Substring(0, 1), true));
                    RekursivConvert(num.Substring(1));
                }
                else if (num.Length == 4)
                {
                    if (num.Substring(0, 1) != "0")
                        if (num.Substring(0, 1) == "1" && numName != "") numName += " нэгэн";
                        else numName += " " + NumNames(num.Substring(0, 1), true);
                    if (Convert.ToInt32(num.Substring(1)) == 0)
                    {
                        numName += " мянган";
                        return;
                    }
                    else numName += " мянга";
                    if (amount.ToString(CultureInfo.InvariantCulture).Length >= 9
                        && Convert.ToDecimal(
                                amount.ToString(CultureInfo.InvariantCulture).Substring(
                                        amount.ToString(CultureInfo.InvariantCulture).Length - 6, 3)) == 0) numName = numName.Replace(" мянга", "");
                    RekursivConvert(num.Substring(1));
                }
                else if (num.Length == 5)
                {
                    if (num.Substring(0, 1) != "0") numName += " " + NumNames(num.Substring(0, 1), false);
                    RekursivConvert(num.Substring(1));
                }
                else if (num.Length == 6)
                {
                    if (num.Substring(0, 1) != "0") numName += string.Format(" {0} зуун", NumNames(num.Substring(0, 1), true));
                    RekursivConvert(num.Substring(1));
                }
                else if (num.Length == 7)
                {
                    if (num.Substring(0, 1) != "0")
                        if (num.Substring(0, 1) == "1" && numName != "") numName += " нэгэн";
                        else numName += " " + NumNames(num.Substring(0, 1), true);
                    numName += " сая";
                    if (amount.ToString(CultureInfo.InvariantCulture).Length >= 9
                        && Convert.ToDecimal(
                                amount.ToString(CultureInfo.InvariantCulture).Substring(
                                        amount.ToString(CultureInfo.InvariantCulture).Length - 9, 3)) == 0) numName = numName.Replace(" сая", "");
                    if (Convert.ToInt32(num.Substring(1)) == 0) return;
                    RekursivConvert(num.Substring(1));
                }
                else if (num.Length == 8)
                {
                    if (num.Substring(0, 1) != "0") numName += " " + NumNames(num.Substring(0, 1), false);
                    RekursivConvert(num.Substring(1));
                }
                else if (num.Length == 9)
                {
                    if (num.Substring(0, 1) != "0") numName += string.Format(" {0} зуун", NumNames(num.Substring(0, 1), true));
                    RekursivConvert(num.Substring(1));
                }
                else if (num.Length == 10)
                {
                    if (num.Substring(0, 1) != "0")
                        if (num.Substring(0, 1) == "1" && numName != "") numName += " нэгэн";
                        else numName += " " + NumNames(num.Substring(0, 1), true);
                    numName += " тэрбум";
                    if (amount.ToString(CultureInfo.InvariantCulture).Length > 12
                        && Convert.ToDecimal(
                                amount.ToString(CultureInfo.InvariantCulture).Substring(
                                        amount.ToString(CultureInfo.InvariantCulture).Length - 12, 3)) == 0) numName = numName.Replace(" тэрбум", "");
                    if (Convert.ToInt32(num.Substring(1)) == 0) return;
                    RekursivConvert(num.Substring(1));
                }
                else if (num.Length == 11)
                {
                    if (num.Substring(0, 1) != "0") numName += " " + NumNames(num.Substring(0, 1), false);
                    RekursivConvert(num.Substring(1));
                }
                else if (num.Length == 12)
                {
                    if (num.Substring(0, 1) != "0") numName += string.Format(" {0} зуун", NumNames(num.Substring(0, 1), true));
                    RekursivConvert(num.Substring(1));
                }
                else if (num.Length == 13)
                {
                    numName += NumNames(num.Substring(0, 1), true) + " триллион";
                    if (Convert.ToInt32(num.Substring(1)) == 0) return;
                    RekursivConvert(num.Substring(1));
                }
        }

    }
}
