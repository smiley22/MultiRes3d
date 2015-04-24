using System;
using System.Diagnostics;

namespace MultiRes3d {
	/// <summary>
	/// Berechnet die Frames pro Sekunde.
	/// </summary>
	public class FrameCounter {
		long ticksNow;
		long ticksLastCall;
		double elapsedTime;
		int numFrames;

		/// <summary>
		/// Die Dauer eines einzelnen "Ticks".
		/// </summary>
		public static readonly double secondsPerTick = 1.0 / Stopwatch.Frequency;

		/// <summary>
		/// Event, der einmal pro Sekunde ausgelöst wird, um die aktuellen FPS mitzuteilen.
		/// </summary>
		public event EventHandler<int> FPSCalculated;

		/// <summary>
		/// Die Zeitspanne, die seit dem letzten Aufruf von <c>Count</c> vergangen ist.
		/// </summary>
		public double DeltaTime {
			get;
			private set;
		}

		/// <summary>
		/// Die Gesamtlaufzeit, in Sekunden.
		/// </summary>
		public double TotalTime {
			get;
			private set;
		}

		/// <summary>
		/// Berechnet die Frames pro Sekunde.
		/// </summary>
		public void Count() {
			ticksLastCall = ticksNow;
			ticksNow = Stopwatch.GetTimestamp();
			// Zeitspanne berechnen, die seit letztem Aufruf verstrichen ist.
			DeltaTime = (ticksNow - ticksLastCall) * secondsPerTick;
			TotalTime += DeltaTime;
			numFrames++;
			elapsedTime += DeltaTime;
			if (elapsedTime >= 1.0) {
				var fpsEvent = FPSCalculated;
				if (fpsEvent != null)
					fpsEvent(this, numFrames);
				elapsedTime = 0;
				numFrames = 0;
			}
		}
	}
}
