using SkiaSharp;
using System;
using System.Windows.Media;

namespace CoreUtilities.HelperClasses.Extensions
{
	/// <summary>
	/// Class of static extension methods to the <see cref="Color"/> class.
	/// </summary>
	public static class ColorExtensions
	{
		/// <summary>
		/// Converts a <see cref="Color"/> to a <see cref="SKColor"/>.
		/// </summary>
		/// <param name="colour">The colour to be converted</param>
		/// <param name="alpha">An overriding alpha value if the <see cref="Color"/>s alpha property is not
		/// desirable.</param>
		/// <returns>A <see cref="SKColor"/> matching the given <see cref="Color"/>.</returns>
		public static SKColor ToSkColor(this Color colour, byte? alpha = null)
		{
			return new SKColor(colour.R, colour.G, colour.B, alpha ?? colour.A);
		}

		/// <summary>
		/// Returns the same <see cref="Color"/> with the given alpha value.
		/// </summary>
		/// <param name="colour">The <see cref="Color"/> to modify.</param>
		/// <param name="alpha">The alpha value to set.</param>
		/// <returns>A modified <see cref="Color"/>.</returns>
		public static Color SetAlpha(this Color colour, byte alpha)
		{
			return Color.FromArgb(alpha, colour.R, colour.G, colour.B);
		}

		/// <summary>
		/// Merges two <see cref="Color"/>s with a given weighting towards the second colour. 
		/// </summary>
		/// <param name="colour">The first <see cref="Color"/> to sample.</param>
		/// <param name="colour2">The second <see cref="Color"/> to sample.</param>
		/// <param name="colour2Weighting">The weighting/bias towards the second <see cref="Color"/>. The weighting
		/// towards the first <see cref="Color"/> is the inverse of this.</param>
		/// <returns>A <see cref="Color"/> which the combination of both input <see cref="Color"/>s.</returns>
		public static Color Combine(this Color colour, Color colour2, double colour2Weighting)
		{
			double colour1Weighting = 1 - colour2Weighting;
			byte a = (byte)(((double)colour.A * colour1Weighting + (double)colour2.A * colour2Weighting) / (colour1Weighting + colour2Weighting));
			byte r = (byte)(((double)colour.R * colour1Weighting + (double)colour2.R * colour2Weighting) / (colour1Weighting + colour2Weighting));
			byte g = (byte)(((double)colour.G * colour1Weighting + (double)colour2.G * colour2Weighting) / (colour1Weighting + colour2Weighting));
			byte b = (byte)(((double)colour.B * colour1Weighting + (double)colour2.B * colour2Weighting) / (colour1Weighting + colour2Weighting));
			return Color.FromArgb(a, r, g, b);
		}

		/// <summary>
		/// Gives a random <see cref="Color"/> which can be optionally restricted in dimness.
		/// </summary>
		/// <param name="lowerBounds">The lower bounds to restrict how dim the <see cref="Color"/> can become.</param>
		/// <returns>A <see cref="Color"/>.</returns>
		public static Color RandomColour(byte lowerBounds = 0)
		{
			var random = new Random();
			return Color.FromArgb(255,
				(byte)random.Next(lowerBounds, 255),
				(byte)random.Next(lowerBounds, 255),
				(byte)random.Next(lowerBounds, 255));
		}
	}
}
