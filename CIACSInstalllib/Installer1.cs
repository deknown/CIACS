using System;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

namespace InstallLib
{
	[RunInstaller(true)]
	public partial class Installer1 : System.Configuration.Install.Installer
	{
		public Installer1()
		{
			InitializeComponent();
		}
        [System.Security.Permissions.SecurityPermission(System.Security.Permissions.SecurityAction.Demand)]
        public override void Install(IDictionary stateSaver)
        {
            base.Install(stateSaver);
        }

        [System.Security.Permissions.SecurityPermission(System.Security.Permissions.SecurityAction.Demand)]
        public override void Commit(IDictionary savedState)
        {
            base.Commit(savedState);

			if (Environment.GetEnvironmentVariable("CIACS") != Context.Parameters["TARGETDIR"])
			{
				Environment.SetEnvironmentVariable("CIACS", Context.Parameters["TARGETDIR"],
					EnvironmentVariableTarget.Machine);
			}
		}
        [System.Security.Permissions.SecurityPermission(System.Security.Permissions.SecurityAction.Demand)]
        public override void Rollback(IDictionary savedState)
        {
            base.Rollback(savedState);

        }

        [System.Security.Permissions.SecurityPermission(System.Security.Permissions.SecurityAction.Demand)]
        public override void Uninstall(IDictionary savedState)
        {
            base.Uninstall(savedState);

            if (Environment.GetEnvironmentVariable("CIACS") != null)
			{
                Environment.SetEnvironmentVariable("CIACS", null,
                    EnvironmentVariableTarget.Machine);
            }

            MessageBox.Show("Program successfully removed from the computer");
        }
    }
}


