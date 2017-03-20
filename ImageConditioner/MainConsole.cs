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
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Runtime.InteropServices;

namespace ImageConditioner
{
	class MainConsole
	{
		#region Console management API on a Windows-based app
		[DllImport("kernel32.dll", SetLastError = true)]
		static extern bool AllocConsole();
		[DllImport("kernel32.dll", SetLastError = true)]
		static extern bool FreeConsole();
		#endregion

		static public bool Run(ImageConditioner imageConditioner)
		{
			bool resultOk = true;
			// Open always a dedicated console with 'AllocConsole'
			// NOTE: The 'AttachConsole' API could be used to reuse the same console (if already
			//	within a 'cmd.exe' session) but I've faced some problems with it
			AllocConsole();
			Console.WriteLine(Program.LICENSE_TEXT);
			Profile profile = imageConditioner.GetDefaultProfile();
			// Check Settings
			CommandLine cmdLine = imageConditioner.CommandLine;
			if (cmdLine.WarningMessage != null)
				Console.WriteLine(cmdLine.WarningMessage);
			if (cmdLine.ShowHelp)
			{
				Console.WriteLine(CommandLine.HELP_TEXT);
				resultOk = false;
			}
			if ((profile == null) || (cmdLine.SourceDir == null) || (cmdLine.TargetDir == null))
			{
				Console.WriteLine("Error: Some required data is missing");
				Console.WriteLine(
					"Check that profile, source directory and target directory are specified");
				Console.WriteLine("");
				Console.WriteLine(CommandLine.HELP_TEXT);
				resultOk = false;
			}
			if (resultOk)
			{
				// Execute conditioning
				try
				{
					DoConditioning(imageConditioner, profile);
					Console.WriteLine("Conditioning finished");
				}
				catch (Exception e)
				{
					Console.WriteLine("Error: {0}", e.Message);
					resultOk = false;
				}
			}
			// Ask for a key before closing the console in order outputs can be read
			Console.WriteLine("Press a key to exit...");
			Console.ReadKey();
			FreeConsole();
			return resultOk;
		}

		static void DoConditioning(ImageConditioner imageConditioner, Profile profile)
		{
			List<string> srcPathList = new List<string>();
			CommandLine cmdLine = imageConditioner.CommandLine;
			ImageConditioner.AddImagesFromDir(cmdLine.SourceDir, srcPathList);
			if (srcPathList.Count == 0)
				throw new Exception("No image files found");
			Console.WriteLine("Initiating batch image conditoning");
			Console.WriteLine();
			Console.WriteLine("Profile: {0}", profile.Caption);
			Console.WriteLine("Source: {0}", cmdLine.SourceDir);
			Console.WriteLine("Target: {0}", cmdLine.TargetDir);
			Console.WriteLine();
			Console.WriteLine("Images:");
			foreach (string filename in srcPathList)
				Console.WriteLine(Path.GetFileName(filename));
			Console.WriteLine();
			if (!Directory.Exists(cmdLine.TargetDir))
				System.Console.WriteLine("The directory '{0}' doesn't exist. It will be created",
					cmdLine.TargetDir);
			Console.Write("Do you want to continue (y/N)?");
			ConsoleKeyInfo consoleKeyInfo = Console.ReadKey();
			Console.WriteLine();
			if (consoleKeyInfo.Key == ConsoleKey.Y)
			{
				if (!Directory.Exists(cmdLine.TargetDir))
					Directory.CreateDirectory(cmdLine.TargetDir);
				imageConditioner.StartConditioning(profile, null);
				foreach (string srcPath in srcPathList)
					Console.WriteLine(imageConditioner.ConditionImage(srcPath, cmdLine.TargetDir));
			}
		}
	}
}