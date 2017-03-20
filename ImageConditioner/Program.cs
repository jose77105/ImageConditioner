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
using System.Reflection;

namespace ImageConditioner
{
	class Program
	{
		public const string LICENSE_TEXT =
			"Copyright (C) 2017 Jose Maria Ortega <jose77105@gmail.com>\n\n" +
			"This program comes with ABSOLUTELY NO WARRANTY\n" +
			"This is free software, distributed under the GNU GPLv3\n" +
			"For full terms see the file LICENSE or see <http://www.gnu.org/licenses/>\n";

		// [STAThread] attribute required to open CommonDialog objects
		[STAThread]
		static int Main(string[] args)
		{
			bool result;
			CommandLine commandLine = new CommandLine(args);
			ImageConditioner imageConditioner = new ImageConditioner(commandLine);
			if (commandLine.Quiet)
				result = MainQuiet.Run(imageConditioner);
			else if (commandLine.ConsoleBased)
				result = MainConsole.Run(imageConditioner);
			else
				result = MainForm.Run(imageConditioner);
			return result ? 0 : 1;
		}
	}
}