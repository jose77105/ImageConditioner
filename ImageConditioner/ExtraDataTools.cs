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
using System.Globalization;

namespace ImageConditioner
{
	static class ExtraDataTools
	{
		/// <summary>
		/// Parses a string with relative or absolute dimensions clauses to an absolute 'Size'
		/// </summary>
		/// <param name="extraData">The formatted string</param>
		/// <param name="sourceSize">The size to be used if relative dimensions are given</param>
		/// <returns>The absolute target size</returns>
		public static Size ParseImageSize(string extraData, Size sourceSize)
		{
			try
			{
				return DoParseImageSize(extraData, sourceSize);
			}
			catch (System.Exception e)
			{
				throw new Exception(
					"Extra data must have the format 'width x height', 'percentage %', " +
					"'W=width' or 'H=height'\n" +
					"Examples: '800x600', '50%', 'W=800', 'H=600'\n\n" + 
					"Additional info:\n" + e.Message);
			}
		}

		private static Size DoParseImageSize(string extraData, Size sourceSize)
		{
			if (extraData.Length == 0)
				throw new System.Exception("'ExtraData' cannot be an empty string");
			string widthText = "0";
			string heightText = "0";
			string[] extraDataFields;
			if ((extraDataFields = extraData.Split('x')).Length == 2)
			{
				widthText = extraDataFields[0];
				heightText = extraDataFields[1];
			}
			else if ((extraDataFields = extraData.Split('=')).Length == 2)
			{
				string dimension = extraDataFields[0].ToUpper();
				if (dimension.Equals("H"))
					heightText = extraDataFields[1];
				else if (dimension.Equals("W"))
					widthText = extraDataFields[1];
			}
			else
			{
				widthText = extraData;
				heightText = extraData;
			}
			Size targetSize = new Size();
			char[] trimEndArray = new char[] { '%', ' ' };
			targetSize.Width = (widthText.IndexOf('%') >= 0) ?
				(int)Math.Round(double.Parse(widthText.TrimEnd(trimEndArray),
					CultureInfo.InvariantCulture) / 100.0 * sourceSize.Width) :
				int.Parse(widthText);
			targetSize.Height = (heightText.IndexOf('%') >= 0) ?
				(int)Math.Round(double.Parse(heightText.TrimEnd(trimEndArray),
					CultureInfo.InvariantCulture) / 100.0 * sourceSize.Height) :
				int.Parse(heightText);
			if ((targetSize.Width == 0) && (targetSize.Height == 0))
				throw new Exception("No valid values provided for width and height");
			return targetSize;
		}

		/// <summary>
		/// Parses a string specifying deflating widths from a sourceSize to a 'Rectangle'
		/// </summary>
		/// <param name="extraData">The formatted string</param>
		/// <param name="sourceSize">The size of the rectangle to be deflated</param>
		/// <returns>The deflated rectangle</returns>
		public static Rectangle ParseDeflatingRectangle(string extraData, Size sourceSize)
		{
			try
			{
				return DoParseDeflatingRectangle(extraData, sourceSize);
			}
			catch (System.Exception e)
			{
				throw new Exception(
					"Extra data must have the format " +
					"'left_margin, top_margin, right_margin, bottom_margin'\n" +
					"Example: '10,20,10,20'\n\n" + 
					"Additional info:\n" + e.Message);
			}
		}

		private static Rectangle DoParseDeflatingRectangle(string extraData, Size sourceSize)
		{
			string[] extraDataFields;
			if ((extraDataFields = extraData.Split(',')).Length != 4)
				throw new Exception("Missing parameters");
			int left = (int)uint.Parse(extraDataFields[0]);
			if (left < sourceSize.Width)
			{
				int top = (int)uint.Parse(extraDataFields[1]);
				if (top < sourceSize.Height)
				{
					int width = sourceSize.Width - left - (int)uint.Parse(extraDataFields[2]);
					if (width > 0)
					{
						int height = sourceSize.Height - top - (int)uint.Parse(extraDataFields[3]);
						if (height > 0)
							return new Rectangle(left, top, width, height);
					}
				}
			}
			return new Rectangle(0, 0, 0, 0);
		}
	}
}
