using System.Windows.Forms;
namespace FlickrMetadataSync
{
    partial class Main
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.Windows.Forms.ListViewItem listViewItem3 = new System.Windows.Forms.ListViewItem("testing");
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Main));
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.folderPicker = new System.Windows.Forms.FolderBrowserDialog();
            this.pictureList = new System.Windows.Forms.ListView();
            this.mnuSelected = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.addTagToSelectedMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.removeTagFromSelectedToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.makePublicToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.makePrivateToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.lstTags = new System.Windows.Forms.ListView();
            this.lblGeotag = new System.Windows.Forms.Label();
            this.txtPictureCaption = new System.Windows.Forms.TextBox();
            this.tagReader = new System.ComponentModel.BackgroundWorker();
            this.tagReadingProgressBar = new System.Windows.Forms.ProgressBar();
            this.txtTag = new System.Windows.Forms.TextBox();
            this.btnAddTagToWholeSet = new System.Windows.Forms.Button();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.setList = new System.Windows.Forms.TreeView();
            this.mnuSets = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.renameThisSetToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.lstAllTags = new System.Windows.Forms.ListView();
            this.mnuAllTags = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.changeThisTagMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.advancedToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.refreshAllTagsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.btnSetDateTaken = new System.Windows.Forms.Button();
            this.lblDateTaken = new System.Windows.Forms.Label();
            this.btnSetDateTakenForWholeSet = new System.Windows.Forms.Button();
            this.calDateTaken = new System.Windows.Forms.MonthCalendar();
            this.flickrGopher = new System.ComponentModel.BackgroundWorker();
            this.flickrProgressBar = new System.Windows.Forms.ProgressBar();
            this.lblFlickrDateTaken = new System.Windows.Forms.Label();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.btnRemoveTagFromWholeSet = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.axWMP = new AxWMPLib.AxWindowsMediaPlayer();
            this.lblVisibility = new System.Windows.Forms.Label();
            this.lblBrowse = new System.Windows.Forms.Label();
            this.lnkPicture = new System.Windows.Forms.LinkLabel();
            this.lnkSet = new System.Windows.Forms.LinkLabel();
            this.pnlPictureBox = new System.Windows.Forms.Panel();
            this.pictureBox = new System.Windows.Forms.PictureBox();
            this.copyTagsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.pasteTagsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuSelected.SuspendLayout();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.mnuSets.SuspendLayout();
            this.mnuAllTags.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.axWMP)).BeginInit();
            this.pnlPictureBox.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox)).BeginInit();
            this.SuspendLayout();
            // 
            // timer1
            // 
            this.timer1.Enabled = true;
            this.timer1.Interval = 500;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // pictureList
            // 
            this.pictureList.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.pictureList.ContextMenuStrip = this.mnuSelected;
            this.pictureList.Location = new System.Drawing.Point(292, 0);
            this.pictureList.Name = "pictureList";
            this.pictureList.Size = new System.Drawing.Size(424, 128);
            this.pictureList.TabIndex = 0;
            this.pictureList.UseCompatibleStateImageBehavior = false;
            // 
            // mnuSelected
            // 
            this.mnuSelected.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.addTagToSelectedMenuItem,
            this.removeTagFromSelectedToolStripMenuItem,
            this.makePublicToolStripMenuItem,
            this.makePrivateToolStripMenuItem,
            this.copyTagsToolStripMenuItem,
            this.pasteTagsToolStripMenuItem});
            this.mnuSelected.Name = "mnuSelected";
            this.mnuSelected.Size = new System.Drawing.Size(196, 158);
            // 
            // addTagToSelectedMenuItem
            // 
            this.addTagToSelectedMenuItem.Name = "addTagToSelectedMenuItem";
            this.addTagToSelectedMenuItem.Size = new System.Drawing.Size(195, 22);
            this.addTagToSelectedMenuItem.Text = "Add Tag to Selected";
            // 
            // removeTagFromSelectedToolStripMenuItem
            // 
            this.removeTagFromSelectedToolStripMenuItem.Name = "removeTagFromSelectedToolStripMenuItem";
            this.removeTagFromSelectedToolStripMenuItem.Size = new System.Drawing.Size(195, 22);
            this.removeTagFromSelectedToolStripMenuItem.Text = "Delete Tag from Selected";
            // 
            // makePublicToolStripMenuItem
            // 
            this.makePublicToolStripMenuItem.Name = "makePublicToolStripMenuItem";
            this.makePublicToolStripMenuItem.Size = new System.Drawing.Size(195, 22);
            this.makePublicToolStripMenuItem.Text = "Make Selected Public";
            // 
            // makePrivateToolStripMenuItem
            // 
            this.makePrivateToolStripMenuItem.Name = "makePrivateToolStripMenuItem";
            this.makePrivateToolStripMenuItem.Size = new System.Drawing.Size(195, 22);
            this.makePrivateToolStripMenuItem.Text = "Make Selected Private";
            // 
            // lstTags
            // 
            this.lstTags.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)));
            this.lstTags.ForeColor = System.Drawing.Color.DarkGray;
            this.lstTags.Items.AddRange(new System.Windows.Forms.ListViewItem[] {
            listViewItem3});
            this.lstTags.Location = new System.Drawing.Point(292, 134);
            this.lstTags.Name = "lstTags";
            this.lstTags.Size = new System.Drawing.Size(158, 149);
            this.lstTags.TabIndex = 2;
            this.lstTags.UseCompatibleStateImageBehavior = false;
            this.lstTags.View = System.Windows.Forms.View.List;
            // 
            // lblGeotag
            // 
            this.lblGeotag.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblGeotag.AutoSize = true;
            this.lblGeotag.Location = new System.Drawing.Point(453, 383);
            this.lblGeotag.Name = "lblGeotag";
            this.lblGeotag.Size = new System.Drawing.Size(45, 13);
            this.lblGeotag.TabIndex = 8;
            this.lblGeotag.Text = "Geotag:";
            // 
            // txtPictureCaption
            // 
            this.txtPictureCaption.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.txtPictureCaption.BackColor = System.Drawing.SystemColors.Control;
            this.txtPictureCaption.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txtPictureCaption.Location = new System.Drawing.Point(457, 345);
            this.txtPictureCaption.Name = "txtPictureCaption";
            this.txtPictureCaption.Size = new System.Drawing.Size(440, 13);
            this.txtPictureCaption.TabIndex = 9;
            // 
            // tagReader
            // 
            this.tagReader.WorkerSupportsCancellation = true;
            this.tagReader.DoWork += new System.ComponentModel.DoWorkEventHandler(this.tagReader_DoWork);
            // 
            // tagReadingProgressBar
            // 
            this.tagReadingProgressBar.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.tagReadingProgressBar.Location = new System.Drawing.Point(292, 373);
            this.tagReadingProgressBar.Name = "tagReadingProgressBar";
            this.tagReadingProgressBar.Size = new System.Drawing.Size(158, 23);
            this.tagReadingProgressBar.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
            this.tagReadingProgressBar.TabIndex = 10;
            this.tagReadingProgressBar.Visible = false;
            // 
            // txtTag
            // 
            this.txtTag.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.txtTag.Location = new System.Drawing.Point(292, 289);
            this.txtTag.Name = "txtTag";
            this.txtTag.Size = new System.Drawing.Size(158, 20);
            this.txtTag.TabIndex = 3;
            // 
            // btnAddTagToWholeSet
            // 
            this.btnAddTagToWholeSet.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnAddTagToWholeSet.Enabled = false;
            this.btnAddTagToWholeSet.Location = new System.Drawing.Point(292, 318);
            this.btnAddTagToWholeSet.Name = "btnAddTagToWholeSet";
            this.btnAddTagToWholeSet.Size = new System.Drawing.Size(37, 23);
            this.btnAddTagToWholeSet.TabIndex = 4;
            this.btnAddTagToWholeSet.TabStop = false;
            this.btnAddTagToWholeSet.Text = "Add";
            this.btnAddTagToWholeSet.UseVisualStyleBackColor = true;
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Left;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.setList);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.lstAllTags);
            this.splitContainer1.Size = new System.Drawing.Size(287, 399);
            this.splitContainer1.SplitterDistance = 219;
            this.splitContainer1.TabIndex = 19;
            // 
            // setList
            // 
            this.setList.ContextMenuStrip = this.mnuSets;
            this.setList.Dock = System.Windows.Forms.DockStyle.Fill;
            this.setList.Location = new System.Drawing.Point(0, 0);
            this.setList.Name = "setList";
            this.setList.Size = new System.Drawing.Size(287, 219);
            this.setList.TabIndex = 0;
            // 
            // mnuSets
            // 
            this.mnuSets.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.renameThisSetToolStripMenuItem});
            this.mnuSets.Name = "mnuSets";
            this.mnuSets.Size = new System.Drawing.Size(155, 26);
            // 
            // renameThisSetToolStripMenuItem
            // 
            this.renameThisSetToolStripMenuItem.Name = "renameThisSetToolStripMenuItem";
            this.renameThisSetToolStripMenuItem.Size = new System.Drawing.Size(154, 22);
            this.renameThisSetToolStripMenuItem.Text = "Rename This Set";
            // 
            // lstAllTags
            // 
            this.lstAllTags.ContextMenuStrip = this.mnuAllTags;
            this.lstAllTags.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lstAllTags.Location = new System.Drawing.Point(0, 0);
            this.lstAllTags.Name = "lstAllTags";
            this.lstAllTags.Size = new System.Drawing.Size(287, 176);
            this.lstAllTags.TabIndex = 23;
            this.lstAllTags.UseCompatibleStateImageBehavior = false;
            this.lstAllTags.View = System.Windows.Forms.View.List;
            // 
            // mnuAllTags
            // 
            this.mnuAllTags.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.changeThisTagMenuItem,
            this.advancedToolStripMenuItem});
            this.mnuAllTags.Name = "mnuTag";
            this.mnuAllTags.Size = new System.Drawing.Size(153, 48);
            // 
            // changeThisTagMenuItem
            // 
            this.changeThisTagMenuItem.Name = "changeThisTagMenuItem";
            this.changeThisTagMenuItem.Size = new System.Drawing.Size(152, 22);
            this.changeThisTagMenuItem.Text = "&Change this Tag";
            // 
            // advancedToolStripMenuItem
            // 
            this.advancedToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.refreshAllTagsToolStripMenuItem});
            this.advancedToolStripMenuItem.Name = "advancedToolStripMenuItem";
            this.advancedToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.advancedToolStripMenuItem.Text = "Advanced";
            // 
            // refreshAllTagsToolStripMenuItem
            // 
            this.refreshAllTagsToolStripMenuItem.Name = "refreshAllTagsToolStripMenuItem";
            this.refreshAllTagsToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.refreshAllTagsToolStripMenuItem.Text = "Refresh All Tags";
            // 
            // btnSetDateTaken
            // 
            this.btnSetDateTaken.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSetDateTaken.Location = new System.Drawing.Point(629, 131);
            this.btnSetDateTaken.Name = "btnSetDateTaken";
            this.btnSetDateTaken.Size = new System.Drawing.Size(86, 22);
            this.btnSetDateTaken.TabIndex = 22;
            this.btnSetDateTaken.TabStop = false;
            this.btnSetDateTaken.Text = "Set date taken";
            this.btnSetDateTaken.UseVisualStyleBackColor = true;
            // 
            // lblDateTaken
            // 
            this.lblDateTaken.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblDateTaken.AutoSize = true;
            this.lblDateTaken.Location = new System.Drawing.Point(457, 128);
            this.lblDateTaken.Name = "lblDateTaken";
            this.lblDateTaken.Size = new System.Drawing.Size(67, 13);
            this.lblDateTaken.TabIndex = 21;
            this.lblDateTaken.Text = "Date Taken:";
            this.lblDateTaken.Click += new System.EventHandler(this.lblDateTaken_Click);
            // 
            // btnSetDateTakenForWholeSet
            // 
            this.btnSetDateTakenForWholeSet.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSetDateTakenForWholeSet.Location = new System.Drawing.Point(524, 131);
            this.btnSetDateTakenForWholeSet.Name = "btnSetDateTakenForWholeSet";
            this.btnSetDateTakenForWholeSet.Size = new System.Drawing.Size(102, 22);
            this.btnSetDateTakenForWholeSet.TabIndex = 20;
            this.btnSetDateTakenForWholeSet.TabStop = false;
            this.btnSetDateTakenForWholeSet.Text = "Set for every pic";
            this.btnSetDateTakenForWholeSet.UseVisualStyleBackColor = true;
            // 
            // calDateTaken
            // 
            this.calDateTaken.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.calDateTaken.Location = new System.Drawing.Point(719, 0);
            this.calDateTaken.Name = "calDateTaken";
            this.calDateTaken.TabIndex = 1;
            this.calDateTaken.TabStop = false;
            // 
            // flickrGopher
            // 
            this.flickrGopher.WorkerSupportsCancellation = true;
            // 
            // flickrProgressBar
            // 
            this.flickrProgressBar.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.flickrProgressBar.Location = new System.Drawing.Point(293, 345);
            this.flickrProgressBar.Name = "flickrProgressBar";
            this.flickrProgressBar.Size = new System.Drawing.Size(157, 23);
            this.flickrProgressBar.TabIndex = 23;
            // 
            // lblFlickrDateTaken
            // 
            this.lblFlickrDateTaken.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblFlickrDateTaken.AutoSize = true;
            this.lblFlickrDateTaken.Location = new System.Drawing.Point(459, 144);
            this.lblFlickrDateTaken.Name = "lblFlickrDateTaken";
            this.lblFlickrDateTaken.Size = new System.Drawing.Size(65, 13);
            this.lblFlickrDateTaken.TabIndex = 24;
            this.lblFlickrDateTaken.Text = "12/29/2008";
            // 
            // btnRemoveTagFromWholeSet
            // 
            this.btnRemoveTagFromWholeSet.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnRemoveTagFromWholeSet.Enabled = false;
            this.btnRemoveTagFromWholeSet.Location = new System.Drawing.Point(395, 318);
            this.btnRemoveTagFromWholeSet.Name = "btnRemoveTagFromWholeSet";
            this.btnRemoveTagFromWholeSet.Size = new System.Drawing.Size(55, 23);
            this.btnRemoveTagFromWholeSet.TabIndex = 5;
            this.btnRemoveTagFromWholeSet.TabStop = false;
            this.btnRemoveTagFromWholeSet.Text = "Remove";
            this.btnRemoveTagFromWholeSet.UseVisualStyleBackColor = true;
            this.btnRemoveTagFromWholeSet.Click += new System.EventHandler(this.btnRemoveTagFromWholeSet_Click);
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label1.Location = new System.Drawing.Point(334, 319);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(57, 20);
            this.label1.TabIndex = 26;
            this.label1.Text = "whole set";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // axWMP
            // 
            this.axWMP.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.axWMP.Enabled = true;
            this.axWMP.Location = new System.Drawing.Point(456, 157);
            this.axWMP.Name = "axWMP";
            this.axWMP.OcxState = ((System.Windows.Forms.AxHost.State)(resources.GetObject("axWMP.OcxState")));
            this.axWMP.Size = new System.Drawing.Size(441, 184);
            this.axWMP.TabIndex = 6;
            this.axWMP.Visible = false;
            // 
            // lblVisibility
            // 
            this.lblVisibility.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.lblVisibility.AutoSize = true;
            this.lblVisibility.Location = new System.Drawing.Point(857, 383);
            this.lblVisibility.Name = "lblVisibility";
            this.lblVisibility.Size = new System.Drawing.Size(40, 13);
            this.lblVisibility.TabIndex = 27;
            this.lblVisibility.Text = "Private";
            this.lblVisibility.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lblBrowse
            // 
            this.lblBrowse.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.lblBrowse.AutoSize = true;
            this.lblBrowse.Location = new System.Drawing.Point(724, 383);
            this.lblBrowse.Name = "lblBrowse";
            this.lblBrowse.Size = new System.Drawing.Size(45, 13);
            this.lblBrowse.TabIndex = 29;
            this.lblBrowse.Text = "Browse:";
            // 
            // lnkPicture
            // 
            this.lnkPicture.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.lnkPicture.AutoSize = true;
            this.lnkPicture.LinkBehavior = System.Windows.Forms.LinkBehavior.HoverUnderline;
            this.lnkPicture.Location = new System.Drawing.Point(771, 383);
            this.lnkPicture.Name = "lnkPicture";
            this.lnkPicture.Size = new System.Drawing.Size(40, 13);
            this.lnkPicture.TabIndex = 30;
            this.lnkPicture.TabStop = true;
            this.lnkPicture.Text = "Picture";
            // 
            // lnkSet
            // 
            this.lnkSet.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.lnkSet.AutoSize = true;
            this.lnkSet.LinkBehavior = System.Windows.Forms.LinkBehavior.HoverUnderline;
            this.lnkSet.Location = new System.Drawing.Point(817, 383);
            this.lnkSet.Name = "lnkSet";
            this.lnkSet.Size = new System.Drawing.Size(23, 13);
            this.lnkSet.TabIndex = 31;
            this.lnkSet.TabStop = true;
            this.lnkSet.Text = "Set";
            // 
            // pnlPictureBox
            // 
            this.pnlPictureBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.pnlPictureBox.Controls.Add(this.pictureBox);
            this.pnlPictureBox.Location = new System.Drawing.Point(456, 157);
            this.pnlPictureBox.Name = "pnlPictureBox";
            this.pnlPictureBox.Size = new System.Drawing.Size(441, 184);
            this.pnlPictureBox.TabIndex = 32;
            // 
            // pictureBox
            // 
            this.pictureBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.pictureBox.Location = new System.Drawing.Point(1, 2);
            this.pictureBox.Name = "pictureBox";
            this.pictureBox.Size = new System.Drawing.Size(441, 184);
            this.pictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox.TabIndex = 6;
            this.pictureBox.TabStop = false;
            // 
            // copyTagsToolStripMenuItem
            // 
            this.copyTagsToolStripMenuItem.Name = "copyTagsToolStripMenuItem";
            this.copyTagsToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.C)));
            this.copyTagsToolStripMenuItem.Size = new System.Drawing.Size(195, 22);
            this.copyTagsToolStripMenuItem.Text = "Copy Tags";
            // 
            // pasteTagsToolStripMenuItem
            // 
            this.pasteTagsToolStripMenuItem.Name = "pasteTagsToolStripMenuItem";
            this.pasteTagsToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.V)));
            this.pasteTagsToolStripMenuItem.Size = new System.Drawing.Size(195, 22);
            this.pasteTagsToolStripMenuItem.Text = "Paste Tags";
            // 
            // Main
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(897, 399);
            this.Controls.Add(this.pnlPictureBox);
            this.Controls.Add(this.lnkSet);
            this.Controls.Add(this.lnkPicture);
            this.Controls.Add(this.lblBrowse);
            this.Controls.Add(this.lblVisibility);
            this.Controls.Add(this.axWMP);
            this.Controls.Add(this.flickrProgressBar);
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btnRemoveTagFromWholeSet);
            this.Controls.Add(this.lblFlickrDateTaken);
            this.Controls.Add(this.btnSetDateTaken);
            this.Controls.Add(this.btnAddTagToWholeSet);
            this.Controls.Add(this.lblDateTaken);
            this.Controls.Add(this.txtTag);
            this.Controls.Add(this.btnSetDateTakenForWholeSet);
            this.Controls.Add(this.tagReadingProgressBar);
            this.Controls.Add(this.calDateTaken);
            this.Controls.Add(this.lstTags);
            this.Controls.Add(this.lblGeotag);
            this.Controls.Add(this.txtPictureCaption);
            this.Controls.Add(this.pictureList);
            this.MinimumSize = new System.Drawing.Size(756, 316);
            this.Name = "Main";
            this.Text = "Flickr Metadata Sync";
            this.mnuSelected.ResumeLayout(false);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.ResumeLayout(false);
            this.mnuSets.ResumeLayout(false);
            this.mnuAllTags.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.axWMP)).EndInit();
            this.pnlPictureBox.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }           

        #endregion

        private Timer timer1;
        private FolderBrowserDialog folderPicker;
        private ListView pictureList;
        private ListView lstTags;
        private Label lblGeotag;
        private TextBox txtPictureCaption;
        private System.ComponentModel.BackgroundWorker tagReader;
        private ProgressBar tagReadingProgressBar;
        private TextBox txtTag;
        private Button btnAddTagToWholeSet;
        private SplitContainer splitContainer1;
        private ListView lstAllTags;
        private Button btnSetDateTaken;
        private Label lblDateTaken;
        private Button btnSetDateTakenForWholeSet;
        private MonthCalendar calDateTaken;
        private TreeView setList;
        private System.ComponentModel.BackgroundWorker flickrGopher;
        private ProgressBar flickrProgressBar;
        private Label lblFlickrDateTaken;
        private ToolTip toolTip1;
        private ContextMenuStrip mnuAllTags;
        private ToolStripMenuItem changeThisTagMenuItem;
        private Button btnRemoveTagFromWholeSet;
        private Label label1;
        private ContextMenuStrip mnuSelected;
        private ToolStripMenuItem addTagToSelectedMenuItem;
        private ToolStripMenuItem removeTagFromSelectedToolStripMenuItem;
        private AxWMPLib.AxWindowsMediaPlayer axWMP;
        private ToolStripMenuItem makePublicToolStripMenuItem;
        private Label lblVisibility;
        private ToolStripMenuItem makePrivateToolStripMenuItem;
        private ContextMenuStrip mnuSets;
        private ToolStripMenuItem renameThisSetToolStripMenuItem;
        private ToolStripMenuItem advancedToolStripMenuItem;
        private ToolStripMenuItem refreshAllTagsToolStripMenuItem;
        private Label lblBrowse;
        private LinkLabel lnkPicture;
        private LinkLabel lnkSet;
        private Panel pnlPictureBox;
        private PictureBox pictureBox;
        private ToolStripMenuItem copyTagsToolStripMenuItem;
        private ToolStripMenuItem pasteTagsToolStripMenuItem;
    }
}

