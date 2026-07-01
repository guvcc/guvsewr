using Avalonia;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Costume_GI
{
    public class AvaloniaRunner
    {
        private static Thread? _thread;
        public static void Start()
        {
            if (_thread != null)
                return;

            _thread = new Thread(() =>
            {
                Program.BuildAvaloniaApp()
                    .StartWithClassicDesktopLifetime(Array.Empty<string>());
            });

            _thread.SetApartmentState(ApartmentState.STA);
            _thread.IsBackground = false;
            _thread.Start();
        }
    }
}}