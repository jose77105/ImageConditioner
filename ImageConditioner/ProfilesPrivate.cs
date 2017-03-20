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
using System.Drawing.Drawing2D;

namespace ImageConditioner
{
	static partial class Profiles
	{
		[ProfileAttribute("A custom transformation example", Hidden = true,
		   InterpolationMode = InterpolationMode.HighQualityBicubic)]
		static public Bitmap ACustomTransformationExample(Bitmap sourceImage)
		{
			// Just a basic transformation that resizes the source image to 64x64 pixels
			// with 'InterpolationMode.HighQualityBicubic' (i.e. high-quality compression algorithm)
			return ImageTools.Resize(sourceImage, 64, 64);
		}
	}
}