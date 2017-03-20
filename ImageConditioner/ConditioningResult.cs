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
using System.IO;

namespace ImageConditioner
{
	struct ConditioningResult
	{
		public enum Operation
		{
			None = 0,
			Ok,
			SkippedBecauseExisting
		};

		public struct ImageInfo
		{
			public string Path;
			public Size Dimensions;
			public long FileSize;
		}

		public Operation operation;
		public ImageInfo sourceImage;
		public ImageInfo targetImage;

		public override string ToString()
		{
			switch (operation)
			{
				case Operation.Ok:
					return string.Format("{0}x{1} {2}KB\t{3}x{4} {5}KB\t{6}",
						sourceImage.Dimensions.Width, sourceImage.Dimensions.Height,
						sourceImage.FileSize >> 10,
						targetImage.Dimensions.Width, targetImage.Dimensions.Height,
						targetImage.FileSize >> 10,
						Path.GetFileName(targetImage.Path));
				case Operation.SkippedBecauseExisting:
					return string.Format("Skipped because existing! {0}",
						Path.GetFileName(targetImage.Path));
				default:
					return "";
			}
		}
	}
}