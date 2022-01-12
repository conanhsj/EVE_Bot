using EVE_Bot.JsonEVE;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EVE_Bot
{
    public static class Commons
    {
        public static Random rnd = new Random(100);
        public static Regex regCQCode = new Regex(@"(?:\[CQ:.*\])");

        public static string RemoveCQCode(string strMessage)
        {
            while (regCQCode.IsMatch(strMessage))
            {
                strMessage = regCQCode.Replace(strMessage, "");
            }
            return strMessage;
        }

        public static double ReadDouble(string strCell)
        {
            double dRnt = 0;
            strCell = strCell.Replace(",", "");
            if (!double.TryParse(strCell, out dRnt))
            {

            }
            return dRnt;
        }

        public static int ReadInt(string strCell)
        {
            int dRnt = 0;
            strCell = strCell.Replace(",", "");
            if (!int.TryParse(strCell, out dRnt))
            {

            }
            return dRnt;
        }

        public static string FormatISK(string strISK)
        {
            if (strISK.Length > 12)
            {
                //十亿换算
                strISK = (Commons.ReadDouble(strISK) / 1000000000).ToString("0.00");
                if (strISK.Contains('.') && strISK.Length < 5)
                {
                    strISK = strISK.PadRight(5, '0');
                }
                else
                {
                    strISK = strISK.PadLeft(5, ' ');
                }
                strISK += "十亿";
            }
            else if (strISK.Length > 9)
            {
                //百万换算
                strISK = (Commons.ReadDouble(strISK) / 1000000).ToString("0.00");
                if (strISK.Contains('.') && strISK.Length < 5)
                {
                    strISK = strISK.PadRight(5, '0');
                }
                else
                {
                    strISK = strISK.PadLeft(5, ' ');
                }
                strISK += "百万";
            }
            else if (strISK.Length > 6)
            {
                //千换算
                strISK = (Commons.ReadDouble(strISK) / 1000).ToString("0.00");
                if (strISK.Contains('.') && strISK.Length < 5)
                {
                    strISK = strISK.PadRight(5, '0');
                }
                else
                {
                    strISK = strISK.PadLeft(5, ' ');
                }
                strISK += "千";
            }
            else
            {
                strISK = strISK.PadLeft(7, ' ');
            }

            #region 亿，万
            //if (strISK.Length > 11)
            //{
            //    //亿换算
            //    strISK = (Commons.ReadDouble(strISK) / 100000000).ToString("0.0000").TrimEnd('0');
            //    if (strISK.Contains('.') && strISK.Length < 6)
            //    {
            //        strISK = strISK.PadRight(6, '0');
            //    }
            //    else
            //    {
            //        strISK = strISK.PadLeft(6, ' ');
            //    }
            //    strISK += "亿";
            //}
            //else if (strISK.Length > 7)
            //{
            //    //万换算
            //    strISK = (Commons.ReadDouble(strISK) / 10000).ToString("0.0000").TrimEnd('0');
            //    if (strISK.Contains('.') && strISK.Length < 6)
            //    {
            //        strISK = strISK.PadRight(6, '0');
            //    }
            //    else
            //    {
            //        strISK = strISK.PadLeft(6, ' ');
            //    }
            //    strISK += "万";
            //}
            //else
            //{
            //    strISK = strISK.PadLeft(8, ' ');
            //}
            #endregion

            return strISK;
        }

        public static string DrawKyalImage(string strInfo)
        {
            strInfo = strInfo.Replace(",", "\t").Replace("=", "==").Replace("：", "：\t");

            string strPath = string.Empty;

            int width = 800;
            int height = 600;
            int margin = 10;
            string strBasePath = Application.StartupPath + @"\image\BaseSource\Base.png";
            Bitmap bitKyal = new Bitmap(strBasePath);


            Bitmap bitmapobj = new Bitmap(width, height);
            Graphics g = Graphics.FromImage(bitmapobj);
            Font font = new Font("宋体", 12);
            Color KyalPink = Color.FromArgb(255, 205, 235);
            Color KyalPurp = Color.FromArgb(110, 100, 115);
            SolidBrush brush = new SolidBrush(KyalPurp);//新建一个画刷
            Pen p = new Pen(Color.Black, 3);//定义了一个画笔
            g.Clear(KyalPink);
            g.DrawRectangle(p, margin, margin, width - (2 * margin), height - (2 * margin));//
            g.DrawString(strInfo, font, brush, 20, 20);//
            g.DrawImage(bitKyal, width - bitKyal.Width, height - bitKyal.Height);


            string strPathImage = Application.StartupPath + @"\image\ImageSource\KyalImage.png";
            bitmapobj.Save(strPathImage, ImageFormat.Png);//保存为输出流，否则页面上显示不出来
            g.Dispose();//释放掉该资源


            strPathImage = strPathImage.Replace("[", "&#91;").Replace("]", "&#93;");
            return "[CQ:image,file=" + strPathImage + "]";
        }
    }
}
