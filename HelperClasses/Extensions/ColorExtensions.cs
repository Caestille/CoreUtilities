using SkiaSharp;
using System;
using System.Windows.Media;

namespace CoreUtilities.HelperClasses.Extensions
{
	public static class ColorExtensions
	{
		public static SKColor ToSkColor(this Color colour, byte? alpha = null)
		{
			return new SKColor(colour.R, colour.G, colour.B, alpha ?? colour.A);
		}

		public static Color SetAlpha(this Color colour, byte alpha)
		{
			return Color.FromArgb(alpha, colour.R, colour.G, colour.B);
		}

		public static Color Combine(this Color colour, Color colour2, double colour2Weighting)
		{
			double colour1Weighting = 1 - colour2Weighting;
			byte a = (byte)(((double)colour.A * colour1Weighting + (double)colour2.A * colour2Weighting) / (colour1Weighting + colour2Weighting));
			byte r = (byte)(((double)colour.R * colour1Weighting + (double)colour2.R * colour2Weighting) / (colour1Weighting + colour2Weighting));
			byte g = (byte)(((double)colour.G * colour1Weighting + (double)colour2.G * colour2Weighting) / (colour1Weighting + colour2Weighting));
			byte b = (byte)(((double)colour.B * colour1Weighting + (double)colour2.B * colour2Weighting) / (colour1Weighting + colour2Weighting));
			return Color.FromArgb(a, r, g, b);
		}

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
