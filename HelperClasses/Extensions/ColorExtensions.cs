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

		public static string GetHexString(this Color colour)
		{
			return colour.ToString();
		}

		public static Color FromHexString(string hex)
		{
			return (Color)ColorConverter.ConvertFromString(hex);
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

		/// <summary>
		/// Finds the perceived brightness of a given <see cref="Color"/> using a power rule and scaling factor for the
		/// RGB components.
		/// </summary>
		/// <param name="colour">The input <see cref="Color"/>.</param>
		/// <returns>A <see cref="double"/> between 0 and 1 indicating a scaled dark/bright value.</returns>
		public static double PerceivedBrightness(this Color colour)
		{
			var rFactor = 0.299 * Math.Pow(colour.R, 2);
			var gFactor = 0.587 * Math.Pow(colour.G, 2);
			var bFactor = 0.114 * Math.Pow(colour.B, 2);
			return Math.Sqrt(rFactor + gFactor + bFactor) / 255d;
		}

		public static double Brightness(this Color colour)
		{
			return System.Drawing.Color.FromArgb(colour.A, colour.R, colour.G, colour.B).GetBrightness();
		}

		/// <summary>
		/// Indicates if two colours RGB components are similar to each other by a given theshold.
		/// </summary>
		/// <param name="colour1">The first <see cref="Color"/>.</param>
		/// <param name="colour2">The second <see cref="Color"/></param>
		/// <param name="threshold">The threshold by which the sum of differences of the RGB components of both colours
		/// must be within for this to return <see cref="true"/>.</param>
		/// <returns>A <see cref="bool"/> indicating whether the input <see cref="Color"/> are similar in colour.
		/// </returns>
		public static bool ColoursAreClose(this Color colour1, Color colour2, double threshold)
		{
			var rDist = Math.Abs(colour1.R - colour2.R);
			var gDist = Math.Abs(colour1.G - colour2.G);
			var bDist = Math.Abs(colour1.B - colour2.B);

			return rDist + gDist + bDist < threshold;
		}

		/// <summary>
		/// Compares the brightness of two colours. If greater than a threshold, the input colour has its brightness
		/// modified.
		/// </summary>
		/// <param name="colour">Output <see cref="Color"/>.</param>
		/// <param name="topColour"><see cref="Color"/> on top.</param>
		/// <param name="backgroundColour">Background <see cref="Color"/>.</param>
		/// <param name="colourToSet">The <see cref="Color"/> to adjust.</param>
		/// <param name="comparisonAdjustmentFactor">Fudge factor for the threshold comparison.</param>
		/// <param name="threshold">The threshold to compare the perceived brightness of the two <see cref="Color"/>s
		/// with</param>
		/// <param name="changeFactor">The factor by which to change the colour of the <paramref name="colourToSet"/>
		/// by if the threshold condition is met.</param>
		public static void AdjustBrightnessIfNearColour(
			this ref Color colour,
			Color topColour,
			Color backgroundColour,
			Color colourToSet,
			double comparisonAdjustmentFactor,
			double threshold,
			float changeFactor)
		{
			var themeBrightness = topColour.PerceivedBrightness();
			var backgroundBrightness = backgroundColour.PerceivedBrightness();
			var diff = themeBrightness - backgroundBrightness; // >0 means theme is brighter than background
			colour = comparisonAdjustmentFactor * diff > -1 * threshold ? colourToSet.ChangeColourBrightness(changeFactor) : colourToSet;
		}

		/// <summary>
		/// Changes the brightness of a <see cref="Color"/> by a scaling factor.
		/// </summary>
		/// <param name="colour">The <see cref="Color"/> to cahnge the brightness of.</param>
		/// <param name="factor">The factor to scale the brightnes by.</param>
		/// <returns>A <see cref="Color"/> with brightness modified by the scaling factor.</returns>
		public static void ChangeThisColourBrightness(this ref Color colour, float factor)
		{
			float red = (float)colour.R;
			float green = (float)colour.G;
			float blue = (float)colour.B;

			if (factor < 0)
			{
				factor = 1 + factor;
				red *= factor;
				green *= factor;
				blue *= factor;
			}
			else
			{
				red = (255 - red) * factor + red;
				green = (255 - green) * factor + green;
				blue = (255 - blue) * factor + blue;
			}

			colour = Color.FromArgb(colour.A, (byte)red, (byte)green, (byte)blue);
		}

		/// <summary>
		/// Changes the brightness of a <see cref="Color"/> by a scaling factor.
		/// </summary>
		/// <param name="colour">The <see cref="Color"/> to cahnge the brightness of.</param>
		/// <param name="factor">The factor to scale the brightnes by.</param>
		/// <returns>A <see cref="Color"/> with brightness modified by the scaling factor.</returns>
		public static Color ChangeColourBrightness(this Color colour, float factor)
		{
			float red = (float)colour.R;
			float green = (float)colour.G;
			float blue = (float)colour.B;

			if (factor < 0)
			{
				factor = 1 + factor;
				red *= factor;
				green *= factor;
				blue *= factor;
			}
			else
			{
				red = (255 - red) * factor + red;
				green = (255 - green) * factor + green;
				blue = (255 - blue) * factor + blue;
			}

			return Color.FromArgb(colour.A, (byte)red, (byte)green, (byte)blue);
		}
	}
}
