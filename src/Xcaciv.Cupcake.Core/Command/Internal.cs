using System;
using System.Collections.Generic;
using System.IO.Abstractions;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Xcaciv.Command.Interface;

namespace Xcaciv.Cupcake.Core.Command
{
    internal class Internal
    {
        public static CommandDescription Factory()
        {
            return new CommandDescription()
            {
                BaseCommand = "echo",
                PackageDescription = new PackageDescription()
                {
                    Name = "echo",
                    FullPath = string.Empty,
                    Version = new Version(0, 0)
                } 
            };
        }

    }
}
