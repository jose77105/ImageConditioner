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

using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;

namespace ImageConditioner
{
	struct ConditioningInputs
	{
		public Bitmap SourceImage;
		public string ExtraData;
	};

	struct ConditioningOutputs
	{
		public Bitmap TargetImage;
		public ImageFormat TargetImageFormat;
	}

	struct ColorPair
	{
		public Color SourceColor;
		public Color TargetColor;
		public ColorPair(Color source, Color target)
		{
			SourceColor = source;
			TargetColor = target;
		}
	};

	struct DeltraCrop
	{
		public uint Left;
		public uint Top;
		public uint Right;
		public uint Bottom;
		public DeltraCrop(uint left, uint top, uint right, uint bottom)
		{
			this.Left = left;
			this.Top = top;
			this.Right = right;
			this.Bottom = bottom;
		}
	};

	static class ImageTools
	{
		/// <summary>
		/// Interpolation mode used when resizing an image
		/// </summary>
		public static InterpolationMode InterpolationMode = InterpolationMode.Default;

		/// <summary>
		/// Stretches/shrinks an image to absolute dimensions
		/// </summary>
		/// <param name="sourceImage">The source image</param>
		/// <param name="targetWidth">The absolute width of the new image</param>
		/// <param name="targetHeight">The absolute width of the new image</param>
		/// <returns>A new image with the size given</returns>
		/// <remarks>
		/// One of the dimensions ('targetWidth' or 'targetHeight') can be 0 to indicate automatic
		/// calculation of the other dimension keeping the aspect ratio
		/// </remarks>
		public static Bitmap Resize(Bitmap sourceImage, int targetWidth, int targetHeight)
		{
			System.Diagnostics.Debug.Assert((targetWidth != 0) || (targetHeight != 0));
			if (targetWidth == 0)
				targetWidth = (int)Math.Round((double)sourceImage.Width 
					* (double)targetHeight/sourceImage.Height);
			else if (targetHeight == 0)
				targetHeight = (int)Math.Round((double)sourceImage.Height
					* (double)targetWidth / sourceImage.Width);
			return DoResize(sourceImage, targetWidth, targetHeight);
		}

		/// <summary>
		/// Stretches/shrinks an image proportionally
		/// </summary>
		/// <param name="sourceImage">The source image</param>
		/// <param name="widthFactor">The proportion in width (.5 means 50%)</param>
		/// <param name="heightFactor">The proportion in height (.5 means 50%)</param>
		/// <returns>A new resized image</returns>
		public static Bitmap ResizeFactor(Bitmap sourceImage, double widthFactor,
			double heightFactor)
		{
			int	targetWidth = (int)Math.Round(sourceImage.Width * widthFactor);
			int targetHeight = (int)Math.Round(sourceImage.Height * heightFactor);
			return DoResize(sourceImage, targetWidth, targetHeight);
		}

		/// <summary>
		/// Creates an image by remapping a list of colors
		/// </summary>
		/// <param name="sourceImage">The source image</param>
		/// <param name="colorsMap">
		/// An array of color-pairs defining the original color and the new one
		/// </param>
		/// <returns>A new image with the remapped colors</returns>
		public static Bitmap RemapColors(Bitmap sourceImage, ColorPair[] colorsMap)
		{
			return RemapColorsAndCrop(sourceImage, colorsMap,
				new Rectangle(new Point(0, 0), sourceImage.Size));
		}

		/// <summary>
		/// Creates an image by copying an area of the original image and applying color remapping
		/// </summary>
		/// <param name="sourceImage">The source image</param>
		/// <param name="colorsMap">
		/// An array of color-pairs defining the original color and the new one
		/// </param>
		/// <param name="cropRectangle">The area of the original image to be copied</param>
		/// <returns>A new image with 'cropRectangle' dimensions and the remapped colors</returns>
		public static Bitmap RemapColorsAndCrop(Bitmap sourceImage, ColorPair[] colorsMap,
			Rectangle cropRectangle)
		{
			// Note: to remap colors we could also use the 'ImageAttributes.SetRemapTable'
			//	in the function 'Graphics.DrawImage'
			//	To be analyzed if it would give better results
			Bitmap targetImage = new Bitmap(cropRectangle.Size.Width, cropRectangle.Height);
			BitmapData sourceData = sourceImage.LockBits(
				new Rectangle(0, 0, sourceImage.Width, sourceImage.Height),
				ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
			BitmapData targetData = targetImage.LockBits(
				new Rectangle(0, 0, targetImage.Width, targetImage.Height),
				ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);
			// Code based on snippet at:
			//	http://stackoverflow.com/questions/9865437/how-do-you-edit-a-png-programmatically
			int sourceStride = sourceData.Stride;
			System.IntPtr sourceScan0 = sourceData.Scan0;
			int targetStride = targetData.Stride;
			System.IntPtr targetScan0 = targetData.Scan0;
			unsafe
			{
				byte* sourcePointer = (byte*)sourceScan0;
				byte* targetPointer = (byte*)targetScan0;
				int sourceOffset = sourceStride - sourceImage.Width * 4;
				int targetOffset = targetStride - targetImage.Width * 4;
				sourcePointer += cropRectangle.Y * sourceImage.Width * 4;
				sourcePointer += cropRectangle.Left * 4;
				for (int y = 0; y < targetImage.Height; ++y)
				{
					for (int x = 0; x < targetImage.Width; ++x)
					{
						*(int*) targetPointer = *(int*)sourcePointer;
						int sourceColor = Color.FromArgb(
							sourcePointer[2], sourcePointer[1], sourcePointer[0]).ToArgb();
						foreach (ColorPair colorMap in colorsMap)
						{
							if (sourceColor == colorMap.SourceColor.ToArgb())
							{
								Color targetColor = colorMap.TargetColor;
								targetPointer[0] = targetColor.B;
								targetPointer[1] = targetColor.G;
								targetPointer[2] = targetColor.R;
								targetPointer[3] = targetColor.A;
								break;
							}
						}
						sourcePointer += 4;
						targetPointer += 4;
					}
					sourcePointer += (sourceImage.Width - targetImage.Width) * 4;
					sourcePointer += sourceOffset;
					targetPointer += targetOffset;
				}
			}
			sourceImage.UnlockBits(sourceData);
			targetImage.UnlockBits(targetData);
			return targetImage;
		}

		/// <summary>
		/// Gets the rectangle defining the portion of the source image after having cropped
		/// the borders equal to the cropColor
		/// </summary>
		/// <param name="sourceImage">The source image</param>
		/// <param name="cropColor">The color that will be cropped</param>
		/// <param name="cropMargin">The minimum margin of cropColor that must be kept</param>
		/// <returns>The rectangle with the cropped image</returns>
		public static Rectangle GetCroppedRectangle(Bitmap sourceImage, Color cropColor,
			int cropMargin)
		{
			int left, right, top, bottom;
			int x, y;
			// Find Top margin
			for (y = 0; y < sourceImage.Height; y++)
			{
				for (x = sourceImage.Width - 1;
					(x >= 0) && (sourceImage.GetPixel(x, y) == cropColor); x--)
					;
				if (x >= 0)
					break;
			}
			if (y == sourceImage.Height)
				throw new Exception("Empty image");
			top = y;
			// Find Bottom margin
			for (y = sourceImage.Height - 1; y >= 0; y--)
			{
				for (x = sourceImage.Width - 1;
					(x >= 0) && (sourceImage.GetPixel(x, y) == cropColor); x--)
					;
				if (x >= 0)
					break;
			}
			System.Diagnostics.Debug.Assert(y >= 0);
			bottom = y;
			// Find Left margin (use y-cropped values for faster checking)
			for (x = 0; x < sourceImage.Width; x++)
			{
				for (y = top; (y <= bottom) && (sourceImage.GetPixel(x, y) == cropColor); y++)
					;
				if (y <= bottom)
					break;
			}
			System.Diagnostics.Debug.Assert(x < sourceImage.Width);
			left = x;
			// Find Right margin
			for (x = sourceImage.Width - 1; x >= 0; x--)
			{
				for (y = top; (y <= bottom) && (sourceImage.GetPixel(x, y) == cropColor); y++)
					;
				if (y <= bottom)
					break;
			}
			System.Diagnostics.Debug.Assert(x >= 0);
			right = x;
			// Keep the minimum margin.
			left -= cropMargin;
			if (left < 0)
				left = 0;
			top -= cropMargin;
			if (top < 0)
				top = 0;
			right += cropMargin;
			if (right > sourceImage.Width - 1)
				right = sourceImage.Width - 1;
			bottom += cropMargin;
			if (bottom > sourceImage.Height - 1)
				bottom = sourceImage.Height - 1;
			return new Rectangle(left, top, right - left + 1, bottom - top + 1);
		}

		private static Bitmap DoResize(Bitmap sourceImage, int targetWidth, int targetHeight)
		{
			// For resizing we could use the default implementation provided by the Bitmap object
			//	return new Bitmap(sourceImage, targetWidth, targetHeight);
			// The 'DrawImage' way is used instead for more options and flexibility on rendering
			Bitmap targetImage = new Bitmap(targetWidth, targetHeight);
			using (Graphics g = Graphics.FromImage(targetImage))
			{
				g.InterpolationMode = InterpolationMode; 
				// Other possible interesting options for 'DrawImage' may be 'g.SmoothingMode' and
				// 'g.CompositingQuality'
				g.DrawImage(sourceImage, 0, 0, targetWidth, targetHeight);
			}
			return targetImage;
		}
	}
}