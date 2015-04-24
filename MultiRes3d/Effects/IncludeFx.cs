using SlimDX.D3DCompiler;
using System.IO;

namespace MultiRes3d {
	/// <summary>
	/// Notwendig, um Include-Anweisungen in Effektdateien auflösen zu können.
	/// </summary>
    public class IncludeFx : Include {
        static string includeDirectory = @"Effects/";

		/// <summary>
		/// Wird aufgerufen, um geöffnete Dateien freigebenen bzw. schließen zu können.
		/// </summary>
		/// <param name="stream"></param>
        public void Close(Stream stream) {
            stream.Dispose();
        }

		/// <summary>
		/// Wird aufgerufen, um eine Include-Anweisung innerhalb einer Effekt-Datei
		/// während des Kompilierens aufzulösen.
		/// </summary>
        public void Open(IncludeType type, string fileName, Stream parentStream, out Stream stream) {
            stream = new FileStream(includeDirectory + fileName, FileMode.Open);
        }
    }

}
