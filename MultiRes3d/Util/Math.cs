
namespace MultiRes3d {
	/// <summary>
	/// Implementiert gängige trigonometrische und andere mathematische Operationen als single
	/// floating-point Methoden zur bequemeren Verwendung, da Direct3D mit single floating-points
	/// arbeitet.
	/// </summary>
	/// <remarks>
	/// Integriert auch einige nützliche Hilfsmethoden und Konstanten aus der XNA MathHelper
	/// Klasse.
	/// </remarks>
	public static class Math {
		#region Fields
		/// <summary>
		/// Represents the ratio of the circumference of a circle to its diameter, specified by
		/// the constant, π.
		/// </summary>
		public const float Pi = 3.14159274f;

		/// <summary>
		/// Defines the value of Pi divided by two as a <see cref="System.Single"/>.
		/// </summary>
		public const float PiOver2 = Pi / 2;

		/// <summary>
		/// Defines the value of  Pi divided by four as a <see cref="System.Single"/>.
		/// </summary>
		public const float PiOver4 = Pi / 4;

		/// <summary>
		/// Defines the value of Pi multiplied by two as a <see cref="System.Single"/>.
		/// </summary>
		public const float TwoPi = 2 * Pi;

		/// <summary>
		/// Defines the value of Pi multiplied by 3 and divided by two as a <see cref="System.Single"/>.
		/// </summary>
		public const float ThreePiOver2 = 3 * Pi / 2;

		/// <summary>
		/// Defines the value of E as a <see cref="System.Single"/>.
		/// </summary>
		public const float E = 2.71828182845904523536f;

		/// <summary>
		/// Defines the base-10 logarithm of E.
		/// </summary>
		public const float Log10E = 0.434294482f;

		/// <summary>
		/// Defines the base-2 logarithm of E.
		/// </summary>
		public const float Log2E = 1.442695041f;
		#endregion

		#region Methods
		/// <summary>
		/// Clamps a number between a minimum and a maximum.
		/// </summary>
		/// <param name="n">
		/// The number to clamp.
		/// </param>
		/// <param name="min">
		/// The minimum allowed value.
		/// </param>
		/// <param name="max">
		/// The maximum allowed value.
		/// </param>
		/// <returns>
		/// min, if n is lower than min; max, if n is higher than max; n otherwise.
		/// </returns>
		public static int Clamp(int n, int min, int max) {
			n = ((n > max) ? max : n);
			n = ((n < min) ? min : n);
			return n;
		}

		/// <summary>
		/// Clamps a number between a minimum and a maximum.
		/// </summary>
		/// <param name="n">
		/// The number to clamp.
		/// </param>
		/// <param name="min">
		/// The minimum allowed value.
		/// </param>
		/// <param name="max">
		/// The maximum allowed value.
		/// </param>
		/// <returns>
		/// min, if n is lower than min; max, if n is higher than max; n otherwise.
		/// </returns>
		public static float Clamp(float n, float min, float max) {
			n = ((n > max) ? max : n);
			n = ((n < min) ? min : n);
			return n;
		}

		/// <summary>
		/// Clamps a number between a minimum and a maximum.
		/// </summary>
		/// <param name="n">
		/// The number to clamp.
		/// </param>
		/// <param name="min">
		/// The minimum allowed value.
		/// </param>
		/// <param name="max">
		/// The maximum allowed value.
		/// </param>
		/// <returns>
		/// min, if n is lower than min; max, if n is higher than max; n otherwise.
		/// </returns>
		public static double Clamp(double n, double min, double max) {
			n = ((n > max) ? max : n);
			n = ((n < min) ? min : n);
			return n;
		}

		/// <summary>
		/// Returns a value indicating whether this instance and a specified System.Single object
		/// represent the same value.
		/// </summary>
		/// <param name="a">
		/// The first value to compare.
		/// </param>
		/// <param name="b">
		/// The second value to compare.
		/// </param>
		/// <param name="epsilon">
		/// The epsilon value to use.
		/// </param>
		/// <returns>
		/// true if the values are considered equal; otherwise, false.
		/// </returns>
		public static bool NearlyEqual(float a, float b, float epsilon = .0000001f) {
			float absA = System.Math.Abs(a);
			float absB = System.Math.Abs(b);
			float diff = System.Math.Abs(a - b);
			if (a == b) { // shortcut, handles infinities
				return true;
			} else if (a == 0 || b == 0 || diff < float.MinValue) {
				// a or b is zero or both are extremely close to it relative error is less
				// meaningful here
				return diff < (epsilon * float.MinValue);
			} else { // use relative error
				return diff / (absA + absB) < epsilon;
			}
		}

		/// <summary>
		/// Returns the sine of the specified angle.
		/// </summary>
		/// <param name="a">
		/// An angle, measured in radians.
		/// </param>
		/// <returns>
		/// The sine of a. If a is equal to System.Single.NaN, System.Single.NegativeInfinity,
		/// or System.Single.PositiveInfinity, this method returns System.Single.NaN.
		/// </returns>
		public static float Sin(double a) {
			return (float)System.Math.Sin(a);
		}

		/// <summary>
		/// Returns the cosine of the specified angle.
		/// </summary>
		/// <param name="d">
		/// An angle, measured in radians.
		/// </param>
		/// <returns>
		/// The cosine of d. If d is equal to System.Single.NaN, System.Single.NegativeInfinity,
		/// or System.Single.PositiveInfinity, this method returns System.Single.NaN.
		/// </returns>
		public static float Cos(double d) {
			return (float)System.Math.Cos(d);
		}

		/// <summary>
		/// Returns the angle whose cosine is the specified number.
		/// </summary>
		/// <param name="d">
		/// A number representing a cosine, where d must be greater than or equal to -1, but less
		/// than or equal to 1.
		/// </param>
		/// <returns>
		/// An angle, θ, measured in radians, such that 0 ≤ θ ≤ π or System.Single.NaN if d &lt; -1
		/// or d &gt; 1 or d equals System.Single.NaN.
		/// </returns>
		public static float Acos(double d) {
			return (float)System.Math.Acos(d);
		}

		/// <summary>
		/// Returns the absolute value of a single-precision floating-point number.
		/// </summary>
		/// <param name="value">
		/// A number that is greater than or equal to System.Single.MinValue, but less than or
		/// equal to System.Single.MaxValue.
		/// </param>
		/// <returns>
		/// A single-precision floating-point number, x, such that 0 ≤ x ≤
		/// System.Single.MaxValue.
		/// </returns>
		public static float Abs(float value) {
			return System.Math.Abs(value);
		}

		/// <summary>
		/// Returns the absolute value of a double-precision floating-point number.
		/// </summary>
		/// <param name="value">
		/// A number that is greater than or equal to System.Double.MinValue, but less than or
		/// equal to System.Double.MaxValue.
		/// </param>
		/// <returns>
		/// A double-precision floating-point number, x, such that 0 ≤ x ≤
		/// System.Double.MaxValue.
		/// </returns>
		public static double Abs(double value) {
			return System.Math.Abs(value);
		}

		/// <summary>
		/// Returns the smaller of two single-precision floating-point numbers.
		/// </summary>
		/// <param name="val1">
		/// The first of two single-precision floating-point numbers to compare.
		/// </param>
		/// <param name="val2">
		/// The second of two single-precision floating-point numbers to compare.
		/// </param>
		/// <returns>
		/// Parameter val1 or val2, whichever is smaller. If val1, val2, or both val1 and val2 are
		/// equal to System.Single.NaN, System.Single.NaN is returned.
		/// </returns>
		public static float Min(float val1, float val2) {
			return System.Math.Min(val1, val2);
		}

		/// <summary>
		/// Returns the larger of two single-precision floating-point numbers.
		/// </summary>
		/// <param name="val1">
		/// The first of two single-precision floating-point numbers to compare.
		/// </param>
		/// <param name="val2">
		/// The second of two single-precision floating-point numbers to compare.
		/// </param>
		/// <returns>
		/// Parameter val1 or val2, whichever is larger. If val1, or val2, or both val1  and val2
		/// are equal to System.Single.NaN, System.Single.NaN is returned.
		/// </returns>
		public static float Max(float val1, float val2) {
			return System.Math.Max(val1, val2);
		}

		/// <summary>
		/// Convert degrees to radians.
		/// </summary>
		/// <param name="degrees">
		/// An angle in degrees.
		/// </param>
		/// <returns>
		/// The angle expressed in radians.
		/// </returns>
		public static float ToRadians(float degrees) {
			const float degToRad = Pi / 180.0f;
			return degrees * degToRad;
		}

		/// <summary>
		/// Convert radians to degrees.
		/// </summary>
		/// <param name="radians">
		/// An angle in radians.</param>
		/// <returns>
		/// The angle expressed in degrees.
		/// </returns>
		public static float ToDegrees(float radians) {
			const float radToDeg = 180.0f / Pi;
			return radians * radToDeg;
		}

		/// <summary>
		/// Linearly interpolates between two values.
		/// </summary>
		/// <param name="value1">
		/// Source value.
		/// </param>
		/// <param name="value2">
		/// Source value.
		/// </param>
		/// <param name="amount">
		/// Value between 0 and 1 indicating the weight of value2.
		/// </param>
		/// <returns>
		/// Interpolated value.
		/// </returns> 
		/// <remarks>
		/// This method performs the linear interpolation based on the following formula.
		/// <c>value1 + (value2 - value1) * amount</c>
		/// Passing amount a value of 0 will cause value1 to be returned, a value of 1 will cause
		/// value2 to be returned.
		/// </remarks>
		public static float Lerp(float value1, float value2, float amount) {
			return value1 + (value2 - value1) * amount;
		}

		/// <summary>
		/// Reduces a given angle to a value between π and -π.
		/// </summary>
		/// <param name="angle">
		/// The angle to reduce, in radians.
		/// </param>
		/// <returns>
		/// The new angle, in radians.
		/// </returns>
		public static float WrapAngle(float angle) {
			angle = (float)System.Math.IEEERemainder((double)angle, 6.2831854820251465);
			if (angle <= -Pi) {
				angle += TwoPi;
			} else {
				if (angle > Pi) {
					angle -= TwoPi;
				}
			}
			return angle;
		}
		#endregion
	}
}
