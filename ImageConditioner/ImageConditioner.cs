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
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Reflection;

namespace ImageConditioner
{
	class ImageConditioner
	{
		// PRECONDITION: sourceDir != null
		public static string GetDefaultTargetDirFromSourceDir(string sourceDir)
		{
			// By default set the target dir equal to the source dir + subdir 'New'
			return Path.Combine(sourceDir, "New");
		}

		public static void AddImagesFromDir(string sourceDir, IList list)
		{
			string[] extensionsManaged = { ".png", ".jpg", ".bmp", ".gif", ".wmf", ".tiff" };
			foreach (string filename in Directory.GetFiles(sourceDir))
			{
				string extension = Path.GetExtension(filename).ToLowerInvariant();
				if (Array.IndexOf(extensionsManaged, extension) != -1)
					if (!list.Contains(filename))
						list.Add(filename);
			}
		}

		public static void AddImage(string path, IList list)
		{
			if (!list.Contains(path))
				list.Add(path);
		}

		// NOTE: Returns 'null' is the profile doesn't use extra data
		public static string GetDefaultExtraData(Profile profile)
		{
			ProfileAttribute profileAttribute = GetProfileAttribute(profile);
			if (profileAttribute != null)
				return profileAttribute.ExtraData;
			return null;
		}

		public ImageConditioner(CommandLine commandLine)
		{
			this.commandLine = commandLine;
			Type profilesType = typeof(Profiles);
			MethodInfo[] methods = profilesType.GetMethods();
			foreach (MethodInfo method in methods)
			{
				object[] attributes = method.GetCustomAttributes(typeof(ProfileAttribute), false);
				System.Diagnostics.Debug.Assert(attributes.Length <= 1);
				if (attributes.Length > 0)
				{
					ProfileAttribute profileAttribute = (ProfileAttribute)attributes[0];
					if (!profileAttribute.Hidden)
						profileList.Add(
							new Profile(profileAttribute.Caption, method));
				}
			}
		}

		public CommandLine CommandLine
		{
			get
			{
				return commandLine;
			}
		}

		public List<Profile> ProfileList
		{
			get
			{
				return profileList;
			}
		}

		public Profile GetDefaultProfile()
		{
			if (commandLine.ProfileIdentifier != null)
			{
				return profileList.Find(
					delegate(Profile profile)
					{
						return profile.conditioningMethod.Name == commandLine.ProfileIdentifier;
					}
				);
			}
			return null;
		}

		public void StartConditioning(Profile profile, string profileExtraData)
		{
			activeProfile = profile;
			activeProfileExtraData = profileExtraData;
			ProfileAttribute profileAttribute = GetProfileAttribute(profile);
			if (profileAttribute != null)
				ImageTools.InterpolationMode = profileAttribute.InterpolationMode;
		}

		public ConditioningResult ConditionImage(string sourceImagePath, string targetImageDir)
		{
			string targetImagePath;
			ConditioningResult result = new ConditioningResult();
			result.operation = ConditioningResult.Operation.SkippedBecauseExisting;
			result.sourceImage.Path = sourceImagePath;
			// If 'targetImageDir' == 'sourceImageDir' the converted image will be placed in the
			//	same directory as the original one plus a suffix
			if (Path.GetDirectoryName(sourceImagePath).Equals(targetImageDir,
				StringComparison.InvariantCultureIgnoreCase))
				targetImagePath = Path.ChangeExtension(sourceImagePath, ".new" +
					Path.GetExtension(sourceImagePath));
			else
				targetImagePath = Path.Combine(targetImageDir, Path.GetFileName(sourceImagePath));
			if (!File.Exists(targetImagePath))
			{
				Bitmap sourceImage = new Bitmap(sourceImagePath);
				result.sourceImage.Dimensions = new Size(sourceImage.Width, sourceImage.Height);
				result.sourceImage.FileSize = new FileInfo(sourceImagePath).Length;
				Bitmap conditionedImage;
				ImageFormat targetImageFormat = null;

				ParameterInfo[] parameters = activeProfile.conditioningMethod.GetParameters();
				if ((parameters.Length == 1) && (parameters[0].ParameterType == typeof(Bitmap)))
				{
					conditionedImage = (Bitmap)activeProfile.conditioningMethod.Invoke(
						null, new object[] { sourceImage });
				}
				else if ((parameters.Length == 2) &&
					(parameters[0].ParameterType == typeof(Bitmap)) &&
					(parameters[1].ParameterType == typeof(ImageFormat).MakeByRefType()))
				{
					object[] args = new object[] { sourceImage, targetImageFormat };
					conditionedImage = (Bitmap)activeProfile.conditioningMethod.Invoke(null, args);
					targetImageFormat = (ImageFormat)args[1];
				}
				else if ((parameters.Length == 1) &&
					(parameters[0].ParameterType == typeof(ConditioningInputs)))
				{
					ConditioningInputs inputs = new ConditioningInputs();
					inputs.SourceImage = sourceImage;
					inputs.ExtraData = activeProfileExtraData;
					ConditioningOutputs outputs = (ConditioningOutputs)
						activeProfile.conditioningMethod.Invoke(null, new object[] { inputs });
					conditionedImage = outputs.TargetImage;
					targetImageFormat = outputs.TargetImageFormat;
				}
				else
				{
					throw new Exception(String.Format("Unsupported method declaration for '{0}'",
						activeProfile.conditioningMethod.Name));
				}

				if (targetImageFormat == null)
				{
					// If not 'targetImageFormat' specified use the same as the original file
					// according to extension
					string sourceExtension = Path.GetExtension(sourceImagePath);
					StringComparison stringComparison = StringComparison.InvariantCultureIgnoreCase;
					if (string.Equals(sourceExtension, ".bmp", stringComparison))
						targetImageFormat = ImageFormat.Bmp;
					else if (string.Equals(sourceExtension, ".png", stringComparison))
						targetImageFormat = ImageFormat.Png;
					else if (string.Equals(sourceExtension, ".gif", stringComparison))
						targetImageFormat = ImageFormat.Gif;
					else
						targetImageFormat = ImageFormat.Jpeg;
				}
				else
				{
					string targetExtension = null;
					if (targetImageFormat == ImageFormat.Png)
						targetExtension = "png";
					else if (targetImageFormat == ImageFormat.Jpeg)
						targetExtension = "jpg";
					else if (targetImageFormat == ImageFormat.Bmp)
						targetExtension = "bmp";
					else if (targetImageFormat == ImageFormat.Gif)
						targetExtension = "gif";
					else if (targetImageFormat == ImageFormat.Wmf)
						targetExtension = "wmf";
					else if (targetImageFormat == ImageFormat.Tiff)
						targetExtension = "tiff";
					if (targetExtension != null)
						targetImagePath = Path.ChangeExtension(targetImagePath, targetExtension);
				}
				// It's possible that the target extension has been changed. So retest for
				// existence to don't unexpectedly overwrite an existing image
				if (!File.Exists(targetImagePath))
				{
					conditionedImage.Save(targetImagePath, targetImageFormat);
					result.operation = ConditioningResult.Operation.Ok;
					result.targetImage.Path = targetImagePath;
					result.targetImage.Dimensions = new Size(
						conditionedImage.Width, conditionedImage.Height);
					result.targetImage.FileSize = new FileInfo(targetImagePath).Length;
				}
				// Dispose images as soon as possible in order we release any possible open handle
				// If not, sometimes we cannot rename or move source files until the app is closed
				conditionedImage.Dispose();
				sourceImage.Dispose();
			}
			return result;
		}

		private static ProfileAttribute GetProfileAttribute(Profile profile)
		{
			object[] attributes = profile.conditioningMethod.GetCustomAttributes(
				typeof(ProfileAttribute), false);
			System.Diagnostics.Debug.Assert(attributes.Length <= 1);
			if (attributes.Length > 0)
				return ((ProfileAttribute)attributes[0]);
			return null;
		}

		readonly List<Profile> profileList = new List<Profile>();
		CommandLine commandLine;
		Profile activeProfile = null;
		string activeProfileExtraData = null;
	}
}