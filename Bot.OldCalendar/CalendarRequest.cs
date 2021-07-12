using Bot.ExtendInterface;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bot.OldCalendar
{
    [Export("CalendarRequest", typeof(IMessageRequest))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class CalendarRequest : IMessageRequest
    {
        string IMessageRequest.DealGroupRequest(JORecvGroupMsg jsonGrpMsg)
        {
            throw new NotImplementedException();
        }

        string IMessageRequest.DealPrivateRequest(JORecvGroupMsg jsonGrpMsg)
        {
            throw new NotImplementedException();
        }

        private string DrawTodayCalendar()
        {
            int width = 850;
            int height = 600;
            int margin = 10;


            Bitmap bitmapobj = new Bitmap(width, height);
            Graphics g = Graphics.FromImage(bitmapobj);

            SolidBrush GOODLeft = new SolidBrush(Color.Yellow);//新建一个画刷

            Font font = new Font("黑体", 12);
            Color KyalPink = Color.FromArgb(255, 205, 235);
            Color KyalPurp = Color.FromArgb(110, 100, 115);
            SolidBrush brush = new SolidBrush(KyalPurp);//新建一个画刷
            Pen p = new Pen(Color.Black, 3);//定义了一个画笔


            g.Clear(KyalPink);
            g.FillRectangle(GOODLeft, 0, 40, 125, 125);

            g.DrawRectangle(p, margin, margin, width - (2 * margin), height - (2 * margin));//


            g.DrawString("碳化钨 \n碳化钨", font, brush, 12, 12);//

            bitmapobj.Save("E:/test.png", ImageFormat.Png);//保存为输出流，否则页面上显示不出来
            g.Dispose();//释放掉该资源

            return "";
        }
    }
}
