using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EVE_Bot
{
    public static class Commons
    {
        public static double ReadDouble(string strCell)
        {
            double dRnt = 0;
            strCell = strCell.Replace(",", "");
            if (!double.TryParse(strCell, out dRnt))
            {
                MessageBox.Show("数值转换错误:" + strCell);
            }
            return dRnt;
        }

        public static int ReadInt(string strCell)
        {
            int dRnt = 0;
            strCell = strCell.Replace(",", "");
            if (!int.TryParse(strCell, out dRnt))
            {
                MessageBox.Show("数值转换错误:" + strCell);
            }
            return dRnt;
        }
        public static string FormatISK(string strISK)
        {
            if (strISK.Length > 10)
            {
                //亿换算
                strISK = (Commons.ReadDouble(strISK) / 100000000).ToString();
                if (strISK.Contains('.') && strISK.Length < 6)
                {
                    strISK = strISK.PadRight(6, '0');
                }
                else
                {
                    strISK = strISK.PadLeft(6, ' ');
                }
                strISK += "亿";
            }
            else if (strISK.Length > 5)
            {
                //万换算
                strISK = (Commons.ReadDouble(strISK) / 10000).ToString();
                if (strISK.Contains('.') && strISK.Length < 6)
                {
                    strISK = strISK.PadRight(6, '0');
                }
                else
                {
                    strISK = strISK.PadLeft(6, ' ');
                }
                strISK += "万";
            }
            else
            {
                strISK = strISK.PadLeft(8, ' ');
            }

            return strISK;
        }
    }
}
