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
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;

namespace ImageConditioner
{
	static partial class Profiles
	{
		[ProfileAttribute("Resize [Default Quality]", ExtraData = "50%")]
		static public ConditioningOutputs ResizeToAbsoluteDimensions(ConditioningInputs inputs)
		{
			Size targetSize = ExtraDataTools.ParseImageSize(
				inputs.ExtraData, inputs.SourceImage.Size);
			ConditioningOutputs outputs = new ConditioningOutputs();
			outputs.TargetImage = ImageTools.Resize(
				inputs.SourceImage, targetSize.Width, targetSize.Height);
			// Let the caller decide to choose the most appropriate format
			outputs.TargetImageFormat = null;
			return outputs;
		}

		[ProfileAttribute("Resize [HQ]", ExtraData = "W=64",
			InterpolationMode = InterpolationMode.HighQualityBicubic)]
		static public ConditioningOutputs ResizeToAbsoluteDimensionsHQ(ConditioningInputs inputs)
		{
			return ResizeToAbsoluteDimensions(inputs);
		}

		[ProfileAttribute("Auto crop background +1")]
		static public Bitmap CropBackground(Bitmap sourceImage)
		{
			// Crops the borders while equal to the color of the color of the pixel (0,0)
			// which is assumed to be part of the background
			// It keeps 1-pixel border of background if possible
			Rectangle croppedRectangle = ImageTools.GetCroppedRectangle(
				sourceImage, sourceImage.GetPixel(0, 0), 1);
			return sourceImage.Clone(croppedRectangle, PixelFormat.DontCare);
		}

		[ProfileAttribute("Crop dimensions", ExtraData = "10,20,10,20")]
		static public ConditioningOutputs CropDimensions(ConditioningInputs inputs)
		{
			ConditioningOutputs outputs = new ConditioningOutputs();
			Rectangle cropRectangle = ExtraDataTools.ParseDeflatingRectangle(
				inputs.ExtraData, inputs.SourceImage.Size);
			outputs.TargetImage = inputs.SourceImage.Clone(cropRectangle, PixelFormat.DontCare);
			// Let the caller decide to choose the most appropriate format
			outputs.TargetImageFormat = null;
			return outputs;
		}

		[ProfileAttribute("Make background transparent")]
		static public Bitmap MakeBackgroundTransparent(Bitmap sourceImage)
		{
			Color backColor = sourceImage.GetPixel(0, 0);
			// The Bitmap class already offers the possibility to make the background transparent
			//	Bitmap targetBitmap = new Bitmap(sourceImage);
			//	targetBitmap.MakeTransparent(backColor);
			// The 'RemapColors' function gives also the possibility to specify the level of
			// transparency
			ColorPair[] colorMap = {
				// Alpha = 0 for maximum transparency
				// Color = Red just as an indicator in case transparency is not supported
				new ColorPair(backColor, Color.FromArgb(0, 0xFF, 0, 0)),
			};
			return ImageTools.RemapColors(sourceImage, colorMap);
		}

		[ProfileAttribute("Rotate 180º")]
		static public Bitmap Rotate180(Bitmap sourceImage)
		{
			Bitmap targetImage = new Bitmap(sourceImage);
			targetImage.RotateFlip(RotateFlipType.Rotate180FlipNone);
			return targetImage;
		}

		[ProfileAttribute("Add watermark")]
		static public Bitmap AddWatermark(Bitmap sourceImage)
		{
			Bitmap targetImage = new Bitmap(sourceImage);
			StringFormat drawStringFormat = new StringFormat();
			drawStringFormat.Alignment = StringAlignment.Center;
			drawStringFormat.LineAlignment = StringAlignment.Center;
			Point centerPoint = new Point(sourceImage.Width / 2, sourceImage.Height / 2);
			using (Graphics g = Graphics.FromImage(targetImage))
			{
				using (Font font = new Font("Arial", 64.0F, FontStyle.Italic))
				{
					const int alpha = 70;
					using (Brush brush = new SolidBrush(Color.FromArgb(alpha, Color.Red)))
					{
						g.DrawString("DRAFT", font, brush, centerPoint, drawStringFormat);
					}
				}
			}
			return targetImage;
		}
	}
}
