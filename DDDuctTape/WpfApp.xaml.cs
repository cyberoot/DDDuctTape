﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Microsoft.Practices.Unity;

namespace DDDuctTape
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class WpfApp : Application
    {

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            IUnityContainer container = new UnityContainer();
                        
        }
    }
}
