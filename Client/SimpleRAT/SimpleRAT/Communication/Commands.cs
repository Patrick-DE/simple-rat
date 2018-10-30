using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleRAT.Communication
{
    public enum Commands : int
    {
        NONE = 0,
        GatherSystemInfo = 1 << 0,
        GatherProcessInfo = 1 << 1,
        GatherNetworkInfo = 1 << 2,
        GatherFSInfo = 1 << 3,

        GatherAllInfo = GatherSystemInfo | GatherProcessInfo | GatherNetworkInfo | GatherFSInfo,

        TakeScreenshot = 1 << 4,
        SendKeys = 1 << 5,
        MoveMouse = 1 << 6,
        PerformHttpRequest = 1 << 7,
        CmdExec = 1 << 8
    }
}
