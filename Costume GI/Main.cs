using Avalonia;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Costume_GI
{
    public class Main : GuvsewrPackage
    {
        public Main(Package manifest, string basePath, Assembly assembly) : base(manifest, basePath, assembly)
        {
        }
        public static GIExecutor executor;
        public static GI gI;

        public override void Start()
        {
            executor = new GIExecutor("cost");
            gI = new GI(executor, this);

            gI.Start = gi =>
            {
                AvaloniaRunner.Start();
            };

            gi.Register(gI);
        }
    }
}
