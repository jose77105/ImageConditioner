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
using System.Windows.Forms;
using System.IO;

namespace ImageConditioner
{
	class MainForm : System.Windows.Forms.Form
	{
		static public bool Run(ImageConditioner imageConditioner)
		{
			Application.EnableVisualStyles();
			MainForm mainForm = new MainForm(imageConditioner);
			mainForm.RunModal();
			return true;
		}

		private MainForm(ImageConditioner imageConditioner)
			: base()
		{
			this.imageConditioner = imageConditioner;
			int row = 0;
			TableLayoutPanel layout = new TableLayoutPanel();
			this.SuspendLayout();
			layout.SuspendLayout();
			// Layout
			layout.Anchor =
				AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
			layout.Location = new System.Drawing.Point(10, 10);
			// Use 'TableLayoutPanelCellBorderStyle.Single' to debug the grid
			//	layout.CellBorderStyle = TableLayoutPanelCellBorderStyle.Single;
			layout.ColumnCount = 3;
			layout.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
			layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
			layout.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
			layout.RowCount = 9;
			layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
			layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
			layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
			layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
			layout.RowStyles.Add(new RowStyle(SizeType.Percent, 30F));
			layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
			layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
			layout.RowStyles.Add(new RowStyle(SizeType.Percent, 70F));
			layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
			// Profile
			Label profileLabel = new Label();
			profileLabel.AutoSize = true;
			profileLabel.Anchor = AnchorStyles.Left;
			profileLabel.Text = "&Profile:";
			layout.Controls.Add(profileLabel, 0, row);
			profileComboBox = new ComboBox();
			profileComboBox.Anchor = AnchorStyles.Left | AnchorStyles.Right;
			profileComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
			profileComboBox.SelectedIndexChanged +=
				new EventHandler(profileComboBox_SelectedIndexChanged);
			layout.SetColumnSpan(profileComboBox, 2);
			layout.Controls.Add(profileComboBox, 1, row);
			row++;
			// Profile extra data
			Label profileExtraDataLabel = new Label();
			profileExtraDataLabel.AutoSize = true;
			profileExtraDataLabel.Anchor = AnchorStyles.Left;
			profileExtraDataLabel.Text = "E&xtra data:";
			layout.Controls.Add(profileExtraDataLabel, 0, row);
			profileExtraDataTextBox = new TextBox();
			profileExtraDataTextBox.Enabled = false;
			profileExtraDataTextBox.Anchor = AnchorStyles.Left | AnchorStyles.Right;
			layout.SetColumnSpan(profileExtraDataTextBox, 2);
			layout.Controls.Add(profileExtraDataTextBox, 1, row);
			row++;
			// Source Directory
			Button addDirectoryButton = new Button();
			addDirectoryButton.Anchor = AnchorStyles.Left | AnchorStyles.Right;
			addDirectoryButton.Text = "Add F&older...";
			addDirectoryButton.Click += new EventHandler(addDirectoryButton_Click);
			layout.Controls.Add(addDirectoryButton, 2, row);
			Button addFileButton = new Button();
			addFileButton.Anchor = AnchorStyles.Left | AnchorStyles.Right;
			addFileButton.Text = "Add F&ile...";
			addFileButton.Click += new EventHandler(addFileButton_Click);
			layout.Controls.Add(addFileButton, 2, row + 1);
			Button removeFilesButton = new Button();
			removeFilesButton.AutoSize = true;
			removeFilesButton.Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top;
			removeFilesButton.Text = "Re&move Files";
			removeFilesButton.Click += new EventHandler(removeFilesButton_Click);
			layout.Controls.Add(removeFilesButton, 2, row + 2);
			imagesListBox = new ListBox();
			imagesListBox.SelectionMode = SelectionMode.MultiExtended;
			imagesListBox.HorizontalScrollbar = true;
			layout.SetColumnSpan(imagesListBox, 2);
			layout.SetRowSpan(imagesListBox, 3);
			imagesListBox.Anchor =
				AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top | AnchorStyles.Bottom;
			imagesListBox.AllowDrop = true;
			imagesListBox.DoubleClick += new EventHandler(imagesListBox_DoubleClick);
			imagesListBox.KeyDown += new KeyEventHandler(imagesListBox_KeyDown);
			imagesListBox.DragEnter += new DragEventHandler(imagesListBox_DragEnter);
			imagesListBox.DragDrop += new DragEventHandler(imagesListBox_DragDrop);
			layout.Controls.Add(imagesListBox, 0, row);
			row += 3;
			// Target Directory
			Label targetDirectoryLabel = new Label();
			targetDirectoryLabel.Text = "&Target Directory:";
			targetDirectoryLabel.AutoSize = true;
			targetDirectoryLabel.Anchor = AnchorStyles.Left;
			layout.Controls.Add(targetDirectoryLabel, 0, row);
			targetDirectoryTextBox = new TextBox();
			targetDirectoryTextBox.AllowDrop = true;
			targetDirectoryTextBox.DragEnter += new DragEventHandler(
				targetDirectoryTextBox_DragEnter);
			targetDirectoryTextBox.DragDrop += new DragEventHandler(
				targetDirectoryTextBox_DragDrop);
			targetDirectoryTextBox.Anchor = AnchorStyles.Left | AnchorStyles.Right;
			layout.Controls.Add(targetDirectoryTextBox, 1, row);
			Button targetDirectoryButton = new Button();
			targetDirectoryButton.Text = "&...";
			targetDirectoryButton.Anchor = AnchorStyles.Left | AnchorStyles.Right;
			targetDirectoryButton.Click += new EventHandler(targetDirectoryButton_Click);
			layout.Controls.Add(targetDirectoryButton, 2, row);
			row++;
			// Results
			Label resultsLabel = new Label();
			layout.SetColumnSpan(resultsLabel, 3);
			resultsLabel.AutoSize = true;
			resultsLabel.Anchor = AnchorStyles.Left;
			resultsLabel.Text = "Resu&lts:";
			layout.Controls.Add(resultsLabel, 0, row);
			row++;
			resultsListView = new ListView();
			layout.SetColumnSpan(resultsListView, 3);
			resultsListView.Anchor =
				AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top | AnchorStyles.Bottom;
			resultsListView.View = View.Details;
			resultsListView.FullRowSelect = true;
			resultsListView.AllowColumnReorder = true;
			resultsListView.MultiSelect = false;
			// -1 means autosize to content, -2 means autosize to ColumnHeader title
			resultsListView.Columns.Add("Filename", 100, HorizontalAlignment.Left);
			resultsListView.Columns.Add("Source Dimensions", -2, HorizontalAlignment.Center);
			resultsListView.Columns.Add("Source Size", -2, HorizontalAlignment.Right);
			resultsListView.Columns.Add("Target Dimensions", -2, HorizontalAlignment.Center);
			resultsListView.Columns.Add("Target Size", -2, HorizontalAlignment.Right);
			resultsListView.DoubleClick += new EventHandler(resultsListBox_DoubleClick);
			layout.Controls.Add(resultsListView, 0, row);
			row++;
			// Buttons
			Button aboutButton = new Button();
			aboutButton.Text = "&About...";
			aboutButton.Anchor = AnchorStyles.Left;
			aboutButton.Click += new EventHandler(aboutButton_Click);
			layout.Controls.Add(aboutButton, 0, row);
			Button runButton = new Button();
			runButton.Text = "&Run";
			runButton.Anchor = AnchorStyles.Left | AnchorStyles.Right;
			runButton.Click += new EventHandler(runButton_Click);
			layout.Controls.Add(runButton, 2, row);
			// MainForm
			this.ClientSize = new System.Drawing.Size(layout.Size.Width + 2 * layout.Location.X,
				layout.Size.Height + 2 * layout.Location.Y);
			this.Text = "ImageConditioner";
			this.Controls.Add(layout);
			layout.ResumeLayout(false);
			layout.PerformLayout();
			this.ResumeLayout(false);
			// Set default size for the main form
			this.Size = new System.Drawing.Size(500, 400);
			this.MinimumSize = new System.Drawing.Size(250, 300);
		}

		void RunModal()
		{
			CommandLine cmdLine = imageConditioner.CommandLine;
			if (cmdLine.WarningMessage != null)
				MessageBox.Show(cmdLine.WarningMessage, "Command line warnings",
					MessageBoxButtons.OK, MessageBoxIcon.Warning);
			if (cmdLine.ShowHelp)
				MessageBox.Show(CommandLine.HELP_TEXT, "Help", MessageBoxButtons.OK,
					MessageBoxIcon.Information);
			try
			{
				object selectedItem = null;
				Profile defaultProfile = imageConditioner.GetDefaultProfile();
				foreach (Profile profile in imageConditioner.ProfileList)
				{
					profileComboBox.Items.Add(profile);
					if (profile == defaultProfile)
						selectedItem = profile;
				}
				if (selectedItem != null)
					profileComboBox.SelectedItem = selectedItem;
				if (cmdLine.SourceDir != null)
					ImageConditioner.AddImagesFromDir(cmdLine.SourceDir, imagesListBox.Items);
				if (cmdLine.TargetDir != null)
					targetDirectoryTextBox.Text = cmdLine.TargetDir;
			}
			catch (Exception e)
			{
				MessageBox.Show(e.Message, "Error at initialization", MessageBoxButtons.OK,
					MessageBoxIcon.Exclamation);
			}
			ShowDialog();
		}

		void profileComboBox_SelectedIndexChanged(object sender, EventArgs e)
		{
			string extraData = ImageConditioner.GetDefaultExtraData(
				(Profile)profileComboBox.SelectedItem);
			if (extraData == null)
			{
				profileExtraDataTextBox.Enabled = false;
				profileExtraDataTextBox.Text = "";
			}
			else
			{
				profileExtraDataTextBox.Enabled = true;
				profileExtraDataTextBox.Text = extraData;
			}
		}

		void addDirectoryButton_Click(object sender, EventArgs e)
		{
			FolderBrowserDialog dialog = new FolderBrowserDialog();
			if (dialog.ShowDialog() == DialogResult.OK)
				AddImagesInDirectory(dialog.SelectedPath);
		}

		void addFileButton_Click(object sender, EventArgs e)
		{
			OpenFileDialog dialog = new OpenFileDialog();
			dialog.Filter = "Image files|*.jpg;*.bmp;*.png|All files (*.*)|*.*";
			dialog.FilterIndex = 1;
			dialog.RestoreDirectory = true;
			if (dialog.ShowDialog() == DialogResult.OK)
				AddImage(dialog.FileName);
		}

		void removeFilesButton_Click(object sender, EventArgs e)
		{
			RemoveSelectedFiles();
		}

		void imagesListBox_DoubleClick(object sender, EventArgs e)
		{
			System.Diagnostics.Process.Start(imagesListBox.SelectedItem.ToString());
		}

		void imagesListBox_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.KeyCode == Keys.Delete)
				RemoveSelectedFiles();
		}

		void imagesListBox_DragEnter(object sender, DragEventArgs e)
		{
			if (e.Data.GetDataPresent(DataFormats.FileDrop))
				e.Effect = DragDropEffects.Copy;
		}

		void imagesListBox_DragDrop(object sender, DragEventArgs e)
		{
			string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
			foreach (string filename in files)
			{
				FileAttributes attributes = File.GetAttributes(filename);
				if ((attributes & FileAttributes.Directory) != 0)
					AddImagesInDirectory(filename);
				else
					AddImage(filename);
			}
		}

		void targetDirectoryTextBox_DragEnter(object sender, DragEventArgs e)
		{
			if (e.Data.GetDataPresent(DataFormats.FileDrop))
				e.Effect = DragDropEffects.Copy;
		}

		void targetDirectoryTextBox_DragDrop(object sender, DragEventArgs e)
		{
			string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
			if (files.Length == 0)
				return;
			string filename = files[0];
			FileAttributes attributes = File.GetAttributes(filename);
			if ((attributes & FileAttributes.Directory) != 0)
				targetDirectoryTextBox.Text = filename;
			else
				targetDirectoryTextBox.Text = Path.GetDirectoryName(filename);
		}

		void targetDirectoryButton_Click(object sender, EventArgs e)
		{
			FolderBrowserDialog dialog = new FolderBrowserDialog();
			if (dialog.ShowDialog() == DialogResult.OK)
				targetDirectoryTextBox.Text = dialog.SelectedPath;
		}

		void resultsListBox_DoubleClick(object sender, EventArgs e)
		{
			ConditioningResult result = (ConditioningResult)resultsListView.SelectedItems[0].Tag;
			if (result.operation == ConditioningResult.Operation.Ok)
				System.Diagnostics.Process.Start(result.targetImage.Path);
		}

		void aboutButton_Click(object sender, EventArgs e)
		{
			AboutBox.Open();
		}

		void runButton_Click(object sender, EventArgs e)
		{
			resultsListView.Items.Clear();
			if (profileComboBox.SelectedIndex == -1)
			{
				MessageBox.Show("Select a conversion 'Profile' first", "Missing data",
					MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
				return;
			}
			if (targetDirectoryTextBox.Text.Length == 0)
			{
				MessageBox.Show("Specify a target directory first", "Missing data",
					MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
				return;
			}
			if (!Directory.Exists(targetDirectoryTextBox.Text))
			{
				DialogResult dialogResult = MessageBox.Show(String.Format(
					"The target directory doesn't exist:\n  {0}\nDo you want it to be created?",
						targetDirectoryTextBox.Text),
					"Confirmation", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
				if (dialogResult != DialogResult.Yes)
					return;
				Directory.CreateDirectory(targetDirectoryTextBox.Text);
			}
			try
			{
				Profile profile = (Profile)profileComboBox.SelectedItem;
				imageConditioner.StartConditioning(profile, profileExtraDataTextBox.Text);
				foreach (string filename in imagesListBox.Items)
				{
					ConditioningResult result = imageConditioner.ConditionImage(
						filename, targetDirectoryTextBox.Text);
					ListViewItem item = new ListViewItem();
					item.Tag = result;
					if (result.operation == ConditioningResult.Operation.Ok)
					{
						item.Text = Path.GetFileName(result.targetImage.Path);
						item.SubItems.Add(String.Format("{0}x{1}",
							result.sourceImage.Dimensions.Width,
							result.sourceImage.Dimensions.Height));
						item.SubItems.Add(
							String.Format("{0} KB", result.sourceImage.FileSize >> 10));
						item.SubItems.Add(String.Format("{0}x{1}",
							result.targetImage.Dimensions.Width,
							result.targetImage.Dimensions.Height));
						item.SubItems.Add(
							String.Format("{0} KB", result.targetImage.FileSize >> 10));
					}
					else
					{
						item.Text = String.Format(
							"*Skipped {0}", Path.GetFileName(result.sourceImage.Path));
					}
					resultsListView.Items.Add(item);
					// Explicit refresh required to update the 'resultsListView'
					resultsListView.Refresh();
				}
				System.Diagnostics.Process.Start(targetDirectoryTextBox.Text);
			}
			catch (System.Reflection.TargetInvocationException ex)
			{
				// Ignore the 'TargetInvocationException' wrapper
				MessageBox.Show(ex.InnerException.Message, "Error", MessageBoxButtons.OK,
					MessageBoxIcon.Error);
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		void AddImagesInDirectory(string path)
		{
			ImageConditioner.AddImagesFromDir(path, imagesListBox.Items);
			if (targetDirectoryTextBox.Text.Length == 0)
				targetDirectoryTextBox.Text =
					ImageConditioner.GetDefaultTargetDirFromSourceDir(path);
		}

		void AddImage(string path)
		{
			ImageConditioner.AddImage(path, imagesListBox.Items);
			if (targetDirectoryTextBox.Text.Length == 0)
				targetDirectoryTextBox.Text =
					ImageConditioner.GetDefaultTargetDirFromSourceDir(Path.GetDirectoryName(path));
		}

		void RemoveSelectedFiles()
		{
			ListBox.SelectedObjectCollection selectedItems = imagesListBox.SelectedItems;
			if ((selectedItems.Count == 0) && (imagesListBox.Items.Count > 0))
			{
				DialogResult dialogResult = MessageBox.Show(
					"Do you want to clear the entire list of source images?",
					"Confirmation", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
				if (dialogResult == DialogResult.Yes)
					imagesListBox.Items.Clear();
			}
			else
			{
				for (int i = selectedItems.Count - 1; i >= 0; i--)
					imagesListBox.Items.Remove(selectedItems[i]);
			}
		}

		ImageConditioner imageConditioner;
		ComboBox profileComboBox;
		TextBox profileExtraDataTextBox;
		TextBox targetDirectoryTextBox;
		ListBox imagesListBox;
		ListView resultsListView;
	}
}
