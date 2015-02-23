using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace BLSpec
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static string arg;
        protected override void OnStartup(StartupEventArgs e)
        {
            if (e.Args != null && e.Args.Any())
                arg = e.Args.First();
            base.OnStartup(e);
        }

    }
}
