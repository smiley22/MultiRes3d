using System;

namespace MultiRes3d {
	/// <summary>
	/// Beinhaltet verschiedenste Hilfs- u. Extensionmethoden.
	/// </summary>
	public static class Util {
		/// <summary>
		/// Löst den Event aus. Stellt sicher, daß der Event nur ausgelöst wird, wenn
		/// er nicht null ist.
		/// </summary>
		/// <typeparam name="T">
		/// Extends System.EventHandler class.
		/// </typeparam>
		/// <param name="event">
		/// Extends System.EventHandler class.
		/// </param>
		/// <param name="sender">
		/// Der Sender.
		/// </param>
		/// <param name="args">
		/// Event Argumente.
		/// </param>
		public static void Raise<T>(this EventHandler<T> @event, object sender, T args)
			where T : EventArgs {
			EventHandler<T> handler = @event;
			if (handler != null)
				handler(sender, args);
		}

		/// <summary>
		/// Wirft eine ArgumentNullException, wenn die Instanz null ist.
		/// </summary>
		/// <param name="data">
		/// Die zu prüfende Instanz.
		/// </param>
		/// <param name="name">
		/// Der Name der Variablen der in der Exception enthalten sein soll, falls
		/// gewünscht.
		/// </param>
		/// <remarks>
		/// Von Jon Skeet aus StackOverflow.
		/// </remarks>
		public static void ThrowIfNull<T>(this T data, string name)
			where T : class {
			if (data == null)
				throw new ArgumentNullException(name);
		}

		/// <summary>
		/// Wirft eine ArgumentNullException, wenn die Instanz null ist.
		/// </summary>
		/// <param name="data">
		/// Die zu prüfende Instanz.
		/// </param>
		/// <remarks>
		/// Von Jon Skeet aus StackOverflow.
		/// </remarks>
		public static void ThrowIfNull<T>(this T data)
			where T : class {
			if (data == null)
				throw new ArgumentNullException();
		}
	}
}
