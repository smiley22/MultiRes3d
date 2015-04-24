using SlimDX.Windows;
using System;
using System.Windows.Forms;

namespace MultiRes3d {
	static class Program {
		/// <summary>
		/// Der Einstiegspunkt des Programms.
		/// </summary>
		[STAThread]
		static void Main() {
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);
			var form = new Window();
			MessagePump.Run(form, form.Run);
		}
	}
}
