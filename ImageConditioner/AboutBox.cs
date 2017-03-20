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

using System.Windows.Forms;

namespace ImageConditioner
{
	class AboutBox : System.Windows.Forms.Form
	{
		static public void Open()
		{
			AboutBox aboutBox = new AboutBox();
			aboutBox.ShowDialog();
		}

		private AboutBox()
			: base()
		{
			System.Reflection.Assembly assembly = System.Reflection.Assembly.GetExecutingAssembly();
			TableLayoutPanel layout = new TableLayoutPanel();
			this.SuspendLayout();
			layout.Anchor =
				AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
			layout.Location = new System.Drawing.Point(5, 5);
			// Product Name
			Label productName = new Label();
			productName.AutoSize = true;
			productName.Anchor = AnchorStyles.Left | AnchorStyles.Right;
			productName.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			productName.Text = assembly.GetName().Name;
			productName.Font = new System.Drawing.Font("Arial", 16, System.Drawing.FontStyle.Bold);
			layout.Controls.Add(productName);
			// Version
			Label version = new Label();
			version.AutoSize = true;
			version.Anchor = AnchorStyles.Left | AnchorStyles.Right;
			version.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			version.Text = assembly.GetName().Version.ToString();
			layout.Controls.Add(version);
			// Summary
			Label summary = new Label();
			summary.AutoSize = true;
			summary.Anchor = AnchorStyles.Left | AnchorStyles.Right;
			summary.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			summary.Text = ((System.Reflection.AssemblyTitleAttribute)
				System.Attribute.GetCustomAttribute(assembly,
					typeof(System.Reflection.AssemblyTitleAttribute), false)).Title + "\n ";
			layout.Controls.Add(summary);
			// License
			Label license = new Label();
			license.AutoSize = true;
			license.Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top;
			license.BorderStyle = BorderStyle.FixedSingle;
			license.Text = Program.LICENSE_TEXT;
			license.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			layout.Controls.Add(license);
			// About dialog settings
			this.ClientSize = new System.Drawing.Size(layout.Size.Width + 2 * layout.Location.X,
				layout.Height + 2 * layout.Location.Y);
			this.Text = "About";
			this.FormBorderStyle = FormBorderStyle.FixedSingle;
			this.ShowInTaskbar = false;
			this.StartPosition = FormStartPosition.CenterParent;
			this.ShowIcon = false;
			this.MinimizeBox = false;
			this.MaximizeBox = false;
			this.Controls.Add(layout);
			this.ResumeLayout(false);
			this.PerformLayout();
			// Auto-adjust to the minimum required size (after the layout is rendered)
			this.ClientSize = new System.Drawing.Size(license.Right + 2 * layout.Location.X,
				license.Bottom + 2 * layout.Location.Y);
		}
	}
}