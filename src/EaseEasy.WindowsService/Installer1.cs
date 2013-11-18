using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration.Install;
using System.Linq;
using System.ServiceProcess;

namespace EaseEasy.WindowsService {
    [RunInstaller(true)]
    public partial class Installer1 : Installer {
        private ServiceProcessInstaller serviceProcessInstaller1;
        private ServiceInstaller serviceInstaller1;

        public Installer1() {
            InitializeComponent();
            this.serviceProcessInstaller1 = new ServiceProcessInstaller();
            this.serviceInstaller1 = new ServiceInstaller();
            // 
            // serviceProcessInstaller1
            // 
            this.serviceProcessInstaller1.Account = ServiceAccount.LocalSystem;
            this.serviceProcessInstaller1.Password = null;
            this.serviceProcessInstaller1.Username = null;
            // 
            // serviceInstaller1
            // 
            this.serviceInstaller1.ServiceName = ServiceInfo.GetServiceName();
            this.serviceInstaller1.DisplayName = ServiceInfo.GetServiceDisplayName();
            this.serviceInstaller1.Description = ServiceInfo.GetServiceDescription();
            this.serviceInstaller1.StartType = ServiceStartMode.Automatic;
            // 
            // ProjectInstaller
            // 
            this.Installers.AddRange(new Installer[] {
            this.serviceProcessInstaller1,
            this.serviceInstaller1});
        }
    }
}
