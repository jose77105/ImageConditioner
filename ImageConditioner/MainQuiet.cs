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
using System.IO;
using System.Collections.Generic;

namespace ImageConditioner
{
	class MainQuiet
	{
		static public bool Run(ImageConditioner imageConditioner)
		{
			CommandLine cmdLine = imageConditioner.CommandLine;
			Profile profile = imageConditioner.GetDefaultProfile();
			if ((profile == null) || (cmdLine.SourceDir == null) || (cmdLine.TargetDir == null))
				return false;
			try
			{
				if (!Directory.Exists(cmdLine.TargetDir))
					Directory.CreateDirectory(cmdLine.TargetDir);
				List<string> sourcePathList = new List<string>();
				ImageConditioner.AddImagesFromDir(cmdLine.SourceDir, sourcePathList);
				imageConditioner.StartConditioning(profile, null);
				foreach (string sourcePath in sourcePathList)
					imageConditioner.ConditionImage(sourcePath, cmdLine.TargetDir);
			}
			catch
			{
				return false;
			}
			return true;
		}
	}
}