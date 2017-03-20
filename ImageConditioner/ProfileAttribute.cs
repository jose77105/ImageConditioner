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

namespace ImageConditioner
{
	[AttributeUsage(AttributeTargets.Method)]
	class ProfileAttribute : System.Attribute
	{
		#region Mandatory inputs
		/// <summary>
		/// Title that will be used to identify the profile
		/// </summary>
		public readonly string Caption;
		#endregion

		#region Optional named inputs
		/// <summary>
		/// Set to 'true' to don't show the profile in the list of available options
		/// </summary>
		public bool Hidden
		{
			get
			{
				return hidden;
			}
			set
			{

				hidden = value;
			}
		}

		/// <summary>
		/// Additional settings for the profile. If specified the value is used as the default value
		/// Don't specify it (or set to 'null') to indicate that the profile doesn't use 'ExtraData'
		/// </summary>
		public string ExtraData
		{
			get
			{
				return extraData;
			}
			set
			{

				extraData = value;
			}
		}

		/// <summary>
		/// Interpolation mode used when resizing an image
		/// </summary>
		public System.Drawing.Drawing2D.InterpolationMode InterpolationMode
		{
			get
			{
				return interpolationMode;
			}
			set
			{

				interpolationMode = value;
			}
		}
		#endregion

		public ProfileAttribute(string caption)
		{
			this.Caption = caption;
		}

		private bool hidden;
		private string extraData;
		System.Drawing.Drawing2D.InterpolationMode interpolationMode;
	}
}