/*
 *		ImageConditioner - A tool for custom image conditioning in batches
 *		Copyright (C) 2017 Jose Maria Ortega
 *
 *	This program is free software: you can redistribute it and/or modify
 *	it under the terms of the GNU General Public License as published by
 *	the Free Software Foundation, either version 3 of the License, or
 *	(at your option) any later version.
 *
 *	This program is distributed in the hope that it will be useful,
 *	but WITHOUT ANY WARRANTY; without even the implied warranty of
 *	MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 *	GNU General Public License for more details.
 *
 *	You should have received a copy of the GNU General Public License
 *	along with this program.  If not, see <http://www.gnu.org/licenses/>.
 *
 */

using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;

namespace ImageConditioner
{
	static partial class Profiles
	{
		static ColorPair[] HantekGraph6022_ColorMap = {
				// Background
				new ColorPair(Color.Black, Color.White),
				// Border background
				new ColorPair(Color.FromArgb(0x32, 0x32, 0x32), Color.White),
				// Graphic rectangle
				new ColorPair(Color.FromArgb(0x71, 0x6F, 0x64), Color.Black),
				new ColorPair(Color.FromArgb(0xF1, 0xEF, 0xE2), Color.Black),
				new ColorPair(Color.FromArgb(0xAC, 0xA8, 0x99), Color.Black),
				// Graphic rectangle axis
				new ColorPair(Color.FromArgb(0xC8, 0xC8, 0xC8), Color.FromArgb(0x60, 0x60, 0x60)),
				// Grid points
				new ColorPair(Color.White, Color.Black),
				// CH1
				new ColorPair(Color.FromArgb(0xFF, 0xFF, 0x0), Color.DarkBlue),
				// CH2
				new ColorPair(Color.FromArgb(0x0, 0xFF, 0x0), Color.DarkRed),
				// Cursor
				new ColorPair(Color.FromArgb(0x0, 0xC8, 0xFF), Color.DarkMagenta),
				// Time scale
				new ColorPair(Color.FromArgb(0xFF, 0x80, 0x0), Color.Black),
			};

		[ProfileAttribute("Hantek 6022 capture: Recolor + Crop to graph with annotations")]
		static public Bitmap Hantek6022_Capture_RecolorAndCropToAnnotations(Bitmap sourceImage,
			out ImageFormat targetImageFormat)
		{
			// Dimensions of OscCapture cropping: Left+0, Top+43, Right-17, Bottom-4
			DeltraCrop deltaCrop = new DeltraCrop(0, 43, 17, 4);
			targetImageFormat = ImageFormat.Png;
			return ImageTools.RemapColorsAndCrop(sourceImage, HantekGraph6022_ColorMap,
				new Rectangle(
					(int)deltaCrop.Left, (int)deltaCrop.Top,
					(int)(sourceImage.Width - deltaCrop.Left - deltaCrop.Right),
					(int)(sourceImage.Height - deltaCrop.Top - deltaCrop.Bottom)));
		}

		[ProfileAttribute("Hantek 6022 capture: Recolor + Crop to graph")]
		static public Bitmap Hantek6022_Capture_RecolorAndCropToGraph(Bitmap sourceImage,
			out ImageFormat targetImageFormat)
		{
			// Dimensions of OscCapture cropping: Left+0, Top+43, Right-17, Bottom-23
			DeltraCrop deltaCrop = new DeltraCrop(0, 43, 17, 23);
			targetImageFormat = ImageFormat.Png;
			return ImageTools.RemapColorsAndCrop(sourceImage, HantekGraph6022_ColorMap,
				new Rectangle(
					(int)deltaCrop.Left, (int)deltaCrop.Top,
					(int)(sourceImage.Width - deltaCrop.Left - deltaCrop.Right),
					(int)(sourceImage.Height - deltaCrop.Top - deltaCrop.Bottom)));
		}

		static ColorPair[] HantekGraphDso2090_ColorMap = {
				// Background
				new ColorPair(Color.Black, Color.White),
				new ColorPair(Color.FromArgb(0x00, 0x4E, 0x98), Color.White),
				// Text
				new ColorPair(Color.White, Color.Black),
				new ColorPair(Color.FromArgb(0xAC, 0xA8, 0x99), Color.Black),
				// Grid
				new ColorPair(Color.FromArgb(0x64, 0x64, 0x64), Color.FromArgb(0x60, 0x60, 0x60)),
				// CH1
				new ColorPair(Color.FromArgb(0x0, 0xFF, 0x0), Color.DarkBlue),
			};

		[ProfileAttribute("Hantek DSO-2090 screenshot: Recolor + Crop to annotations")]
		static public Bitmap HantekDso2090_Capture_RecolorAndCropToAnnotations(Bitmap sourceImage,
			out ImageFormat targetImageFormat)
		{
			DeltraCrop deltaCrop = new DeltraCrop(6, 50, 28, 26);
			targetImageFormat = ImageFormat.Png;
			return ImageTools.RemapColorsAndCrop(sourceImage, HantekGraphDso2090_ColorMap,
				new Rectangle(
					(int)deltaCrop.Left, (int)deltaCrop.Top,
					(int)(sourceImage.Width - deltaCrop.Left - deltaCrop.Right),
					(int)(sourceImage.Height - deltaCrop.Top - deltaCrop.Bottom)));
		}

		[ProfileAttribute("Hantek DSO-2090 screenshot: Recolor + Crop to graph")]
		static public Bitmap HantekDso2090_Capture_RecolorAndCropToGraph(Bitmap sourceImage,
			out ImageFormat targetImageFormat)
		{
			DeltraCrop deltaCrop = new DeltraCrop(6, 85, 28, 56);
			targetImageFormat = ImageFormat.Png;
			return ImageTools.RemapColorsAndCrop(sourceImage, HantekGraphDso2090_ColorMap,
				new Rectangle(
					(int)deltaCrop.Left, (int)deltaCrop.Top,
					(int)(sourceImage.Width - deltaCrop.Left - deltaCrop.Right),
					(int)(sourceImage.Height - deltaCrop.Top - deltaCrop.Bottom)));
		}

		[ProfileAttribute("Putty screenshot: Recolor")]
		static public Bitmap Putty_Screenshot_Recolor(Bitmap sourceImage)
		{
			ColorPair[] colorMap = {
				// Background
				new ColorPair(Color.Black, Color.White),
				// Foreground
				new ColorPair(Color.FromArgb(0xBB, 0xBB, 0xBB), Color.Black),
			};
			return ImageTools.RemapColors(sourceImage, colorMap);
		}
	}
}
