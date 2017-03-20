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
	class CommandLine
	{
		public const string HELP_TEXT =
			"Command line options:\n" +
			"/P:ProfileIdentifier\n" +
			"/S:Source directory\n" +
			"/T:Target directory\n" +
			"/C for console-based execution\n" +
			"/Q for quiet mode\n" +
			"/? to show this help message";

		// Note: some string-based-fields can be null if not spicifed by a command-line flag
		public readonly string ProfileIdentifier;
		public readonly string SourceDir;
		public readonly string TargetDir;
		public readonly bool ConsoleBased;
		public readonly bool Quiet;
		public readonly bool ShowHelp;
		public readonly string WarningMessage;

		public CommandLine(string[] args)
		{
			// Initialize WarningMessage with an empty string to simplify concatenation of messages
			WarningMessage = "";
			foreach (string arg in args)
			{
				if (arg.StartsWith("/P:", StringComparison.InvariantCultureIgnoreCase))
				{
					ProfileIdentifier = arg.Substring(3);
				}
				else if (arg.StartsWith("/S:", StringComparison.InvariantCultureIgnoreCase))
				{
					SourceDir = arg.Substring(3);
					if (!System.IO.Directory.Exists(SourceDir))
						WarningMessage += String.Format(
							"* The source dir '{0}' doesn't exist",SourceDir);
				}
				else if (arg.StartsWith("/T:", StringComparison.InvariantCultureIgnoreCase))
				{
					TargetDir = arg.Substring(3);
				}
				else if (string.Compare(arg, "/C", true) == 0)
				{
					ConsoleBased = true;
				}
				else if (string.Compare(arg, "/Q", true) == 0)
				{
					Quiet = true;
				}
				else if (string.Compare(arg, "/?", true) == 0)
				{
					ShowHelp = true;
				}
				else
				{
					WarningMessage += String.Format("* The option '{0}' is not supported", arg);
				}
			}
			// If after having parsed the command-line the target directory is not specified set
			// the default path from the source directory
			if ((TargetDir == null) && (SourceDir != null))
				TargetDir = ImageConditioner.GetDefaultTargetDirFromSourceDir(SourceDir);
			// Format warning message
			if (WarningMessage.Length > 0)
				WarningMessage = "Warnings on command line parsing:\n" + WarningMessage;
			else
				WarningMessage = null;
		}
	}
}