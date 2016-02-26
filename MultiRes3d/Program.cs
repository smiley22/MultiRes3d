using SlimDX.Windows;
using System;
using System.Windows.Forms;
using SlimDX.Direct3D11;

namespace MultiRes3d {
	static class Program {
		/// <summary>
		/// Der Einstiegspunkt des Programms.
		/// </summary>
		[STAThread]
		static void Main() {
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);
		    try {
		        var form = new Window();
                // MessagePump verarbeitet alle eingehenden Fensternachrichten und ruft
                // kontinuierlich form.Run auf, wenn es keine Nachrichten zu verarbeiten
                // gibt.
                MessagePump.Run(form, form.Run);
            }
            catch (Direct3D11Exception) {
		        MessageBox.Show(
                    "Please make sure you have installed the DirectX End-User Runtimes (June 2010) " +
                    "available from Microsoft at" + Environment.NewLine + Environment.NewLine +
                    "https://www.microsoft.com/en-us/download/details.aspx?id=8109",
		            "Direct3D 11 Initialization failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
		    }
		}
	}
}
