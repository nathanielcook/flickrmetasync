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
            System.Windows.Forms.ListViewItem listViewItem1 = new System.Windows.Forms.ListViewItem("testing");
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Main));
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.folderPicker = new System.Windows.Forms.FolderBrowserDialog();
            this.pictureList = new System.Windows.Forms.ListView();
            this.mnuSelected = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.addTagToSelectedMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.removeTagFromSelectedToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.makePublicToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.makePrivateToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.copyTagsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.pasteTagsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.setDateTakenToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.copyGeoTagToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.pasteGeoTagToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
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
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this.lstAllTags = new System.Windows.Forms.ListView();
            this.mnuAllTags = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.changeThisTagMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.advancedToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.refreshAllTagsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.sortSetsOnFlickrToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.btnSetDateTaken = new System.Windows.Forms.Button();
            this.lblDateTaken = new System.Windows.Forms.Label();
            this.btnSetDateTakenForWholeSet = new System.Windows.Forms.Button();
            this.calDateTaken = new System.Windows.Forms.MonthCalendar();
            this.flickrGopher = new System.ComponentModel.BackgroundWorker();
            this.flickrProgressBar = new System.Windows.Forms.ProgressBar();
            this.lblFlickrDateTaken = new System.Windows.Forms.Label();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.lblVisibility = new System.Windows.Forms.Label();
            this.btnRemoveTagFromWholeSet = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.axWMP = new AxWMPLib.AxWindowsMediaPlayer();
            this.lblBrowse = new System.Windows.Forms.Label();
            this.lnkPicture = new System.Windows.Forms.LinkLabel();
            this.lnkSet = new System.Windows.Forms.LinkLabel();
            this.pnlPictureBox = new System.Windows.Forms.Panel();
            this.pictureBox = new System.Windows.Forms.PictureBox();
            this.dtpDateTaken = new System.Windows.Forms.DateTimePicker();
            this.btnReUpload = new System.Windows.Forms.Button();
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
            this.pictureList.Location = new System.Drawing.Point(294, 0);
            this.pictureList.Name = "pictureList";
            this.pictureList.Size = new System.Drawing.Size(356, 128);
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
            this.pasteTagsToolStripMenuItem,
            this.setDateTakenToolStripMenuItem,
            this.copyGeoTagToolStripMenuItem,
            this.pasteGeoTagToolStripMenuItem});
            this.mnuSelected.Name = "mnuSelected";
            this.mnuSelected.Size = new System.Drawing.Size(207, 202);
            // 
            // addTagToSelectedMenuItem
            // 
            this.addTagToSelectedMenuItem.Name = "addTagToSelectedMenuItem";
            this.addTagToSelectedMenuItem.Size = new System.Drawing.Size(206, 22);
            this.addTagToSelectedMenuItem.Text = "Add Tag to Selected";
            // 
            // removeTagFromSelectedToolStripMenuItem
            // 
            this.removeTagFromSelectedToolStripMenuItem.Name = "removeTagFromSelectedToolStripMenuItem";
            this.removeTagFromSelectedToolStripMenuItem.Size = new System.Drawing.Size(206, 22);
            this.removeTagFromSelectedToolStripMenuItem.Text = "Delete Tag from Selected";
            // 
            // makePublicToolStripMenuItem
            // 
            this.makePublicToolStripMenuItem.Name = "makePublicToolStripMenuItem";
            this.makePublicToolStripMenuItem.Size = new System.Drawing.Size(206, 22);
            this.makePublicToolStripMenuItem.Text = "Make Selected Public";
            // 
            // makePrivateToolStripMenuItem
            // 
            this.makePrivateToolStripMenuItem.Name = "makePrivateToolStripMenuItem";
            this.makePrivateToolStripMenuItem.Size = new System.Drawing.Size(206, 22);
            this.makePrivateToolStripMenuItem.Text = "Make Selected Private";
            // 
            // copyTagsToolStripMenuItem
            // 
            this.copyTagsToolStripMenuItem.Name = "copyTagsToolStripMenuItem";
            this.copyTagsToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.C)));
            this.copyTagsToolStripMenuItem.Size = new System.Drawing.Size(206, 22);
            this.copyTagsToolStripMenuItem.Text = "Copy Tags";
            // 
            // pasteTagsToolStripMenuItem
            // 
            this.pasteTagsToolStripMenuItem.Name = "pasteTagsToolStripMenuItem";
            this.pasteTagsToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.V)));
            this.pasteTagsToolStripMenuItem.Size = new System.Drawing.Size(206, 22);
            this.pasteTagsToolStripMenuItem.Text = "Paste Tags";
            // 
            // setDateTakenToolStripMenuItem
            // 
            this.setDateTakenToolStripMenuItem.Name = "setDateTakenToolStripMenuItem";
            this.setDateTakenToolStripMenuItem.Size = new System.Drawing.Size(206, 22);
            this.setDateTakenToolStripMenuItem.Text = "Set Date Taken";
            // 
            // copyGeoTagToolStripMenuItem
            // 
            this.copyGeoTagToolStripMenuItem.Name = "copyGeoTagToolStripMenuItem";
            this.copyGeoTagToolStripMenuItem.Size = new System.Drawing.Size(206, 22);
            this.copyGeoTagToolStripMenuItem.Text = "Copy GeoTag";
            // 
            // pasteGeoTagToolStripMenuItem
            // 
            this.pasteGeoTagToolStripMenuItem.Name = "pasteGeoTagToolStripMenuItem";
            this.pasteGeoTagToolStripMenuItem.Size = new System.Drawing.Size(206, 22);
            this.pasteGeoTagToolStripMenuItem.Text = "Paste GeoTag";
            // 
            // lstTags
            // 
            this.lstTags.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)));
            this.lstTags.ForeColor = System.Drawing.Color.DarkGray;
            this.lstTags.Items.AddRange(new System.Windows.Forms.ListViewItem[] {
            listViewItem1});
            this.lstTags.Location = new System.Drawing.Point(292, 159);
            this.lstTags.Name = "lstTags";
            this.lstTags.Size = new System.Drawing.Size(158, 112);
            this.lstTags.TabIndex = 2;
            this.lstTags.UseCompatibleStateImageBehavior = false;
            this.lstTags.View = System.Windows.Forms.View.List;
            // 
            // lblGeotag
            // 
            this.lblGeotag.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblGeotag.AutoSize = true;
            this.lblGeotag.Location = new System.Drawing.Point(453, 371);
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
            this.txtPictureCaption.Location = new System.Drawing.Point(457, 333);
            this.txtPictureCaption.Name = "txtPictureCaption";
            this.txtPictureCaption.Size = new System.Drawing.Size(428, 13);
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
            this.tagReadingProgressBar.Location = new System.Drawing.Point(292, 361);
            this.tagReadingProgressBar.Name = "tagReadingProgressBar";
            this.tagReadingProgressBar.Size = new System.Drawing.Size(158, 23);
            this.tagReadingProgressBar.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
            this.tagReadingProgressBar.TabIndex = 10;
            this.tagReadingProgressBar.Visible = false;
            // 
            // txtTag
            // 
            this.txtTag.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.txtTag.Location = new System.Drawing.Point(292, 277);
            this.txtTag.Name = "txtTag";
            this.txtTag.Size = new System.Drawing.Size(158, 20);
            this.txtTag.TabIndex = 3;
            // 
            // btnAddTagToWholeSet
            // 
            this.btnAddTagToWholeSet.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnAddTagToWholeSet.Enabled = false;
            this.btnAddTagToWholeSet.Location = new System.Drawing.Point(292, 306);
            this.btnAddTagToWholeSet.Name = "btnAddTagToWholeSet";
            this.btnAddTagToWholeSet.Size = new System.Drawing.Size(37, 23);
            this.btnAddTagToWholeSet.TabIndex = 4;
            this.btnAddTagToWholeSet.TabStop = false;
            this.btnAddTagToWholeSet.Text = "&Add";
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
            this.splitContainer1.Size = new System.Drawing.Size(287, 387);
            this.splitContainer1.SplitterDistance = 212;
            this.splitContainer1.TabIndex = 19;
            // 
            // setList
            // 
            this.setList.ContextMenuStrip = this.mnuSets;
            this.setList.Dock = System.Windows.Forms.DockStyle.Fill;
            this.setList.ImageIndex = 1;
            this.setList.ImageList = this.imageList1;
            this.setList.Location = new System.Drawing.Point(0, 0);
            this.setList.Name = "setList";
            this.setList.SelectedImageIndex = 0;
            this.setList.ShowNodeToolTips = true;
            this.setList.Size = new System.Drawing.Size(287, 212);
            this.setList.TabIndex = 0;
            // 
            // mnuSets
            // 
            this.mnuSets.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.renameThisSetToolStripMenuItem});
            this.mnuSets.Name = "mnuSets";
            this.mnuSets.Size = new System.Drawing.Size(162, 26);
            // 
            // renameThisSetToolStripMenuItem
            // 
            this.renameThisSetToolStripMenuItem.Name = "renameThisSetToolStripMenuItem";
            this.renameThisSetToolStripMenuItem.Size = new System.Drawing.Size(161, 22);
            this.renameThisSetToolStripMenuItem.Text = "Rename This Set";
            // 
            // imageList1
            // 
            this.imageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream")));
            this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList1.Images.SetKeyName(0, "selectedImage.bmp");
            this.imageList1.Images.SetKeyName(1, "unSelectedImage.bmp");
            // 
            // lstAllTags
            // 
            this.lstAllTags.ContextMenuStrip = this.mnuAllTags;
            this.lstAllTags.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lstAllTags.Location = new System.Drawing.Point(0, 0);
            this.lstAllTags.Name = "lstAllTags";
            this.lstAllTags.Size = new System.Drawing.Size(287, 171);
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
            this.mnuAllTags.Size = new System.Drawing.Size(161, 48);
            // 
            // changeThisTagMenuItem
            // 
            this.changeThisTagMenuItem.Name = "changeThisTagMenuItem";
            this.changeThisTagMenuItem.Size = new System.Drawing.Size(160, 22);
            this.changeThisTagMenuItem.Text = "&Change this Tag";
            // 
            // advancedToolStripMenuItem
            // 
            this.advancedToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.refreshAllTagsToolStripMenuItem,
            this.sortSetsOnFlickrToolStripMenuItem});
            this.advancedToolStripMenuItem.Name = "advancedToolStripMenuItem";
            this.advancedToolStripMenuItem.Size = new System.Drawing.Size(160, 22);
            this.advancedToolStripMenuItem.Text = "Advanced";
            // 
            // refreshAllTagsToolStripMenuItem
            // 
            this.refreshAllTagsToolStripMenuItem.Name = "refreshAllTagsToolStripMenuItem";
            this.refreshAllTagsToolStripMenuItem.Size = new System.Drawing.Size(167, 22);
            this.refreshAllTagsToolStripMenuItem.Text = "Refresh All Tags";
            // 
            // sortSetsOnFlickrToolStripMenuItem
            // 
            this.sortSetsOnFlickrToolStripMenuItem.Name = "sortSetsOnFlickrToolStripMenuItem";
            this.sortSetsOnFlickrToolStripMenuItem.Size = new System.Drawing.Size(167, 22);
            this.sortSetsOnFlickrToolStripMenuItem.Text = "Sort Sets on Flickr";
            // 
            // btnSetDateTaken
            // 
            this.btnSetDateTaken.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSetDateTaken.Location = new System.Drawing.Point(467, 131);
            this.btnSetDateTaken.Name = "btnSetDateTaken";
            this.btnSetDateTaken.Size = new System.Drawing.Size(86, 22);
            this.btnSetDateTaken.TabIndex = 22;
            this.btnSetDateTaken.TabStop = false;
            this.btnSetDateTaken.Text = "Set &date taken";
            this.btnSetDateTaken.UseVisualStyleBackColor = true;
            // 
            // lblDateTaken
            // 
            this.lblDateTaken.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblDateTaken.AutoSize = true;
            this.lblDateTaken.Location = new System.Drawing.Point(287, 128);
            this.lblDateTaken.Name = "lblDateTaken";
            this.lblDateTaken.Size = new System.Drawing.Size(67, 13);
            this.lblDateTaken.TabIndex = 21;
            this.lblDateTaken.Text = "Date Taken:";
            this.lblDateTaken.Click += new System.EventHandler(this.lblDateTaken_Click);
            // 
            // btnSetDateTakenForWholeSet
            // 
            this.btnSetDateTakenForWholeSet.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSetDateTakenForWholeSet.Location = new System.Drawing.Point(359, 132);
            this.btnSetDateTakenForWholeSet.Name = "btnSetDateTakenForWholeSet";
            this.btnSetDateTakenForWholeSet.Size = new System.Drawing.Size(102, 21);
            this.btnSetDateTakenForWholeSet.TabIndex = 20;
            this.btnSetDateTakenForWholeSet.TabStop = false;
            this.btnSetDateTakenForWholeSet.Text = "Set for every pic";
            this.btnSetDateTakenForWholeSet.UseVisualStyleBackColor = true;
            // 
            // calDateTaken
            // 
            this.calDateTaken.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.calDateTaken.Location = new System.Drawing.Point(656, -1);
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
            this.flickrProgressBar.Location = new System.Drawing.Point(293, 333);
            this.flickrProgressBar.Name = "flickrProgressBar";
            this.flickrProgressBar.Size = new System.Drawing.Size(157, 23);
            this.flickrProgressBar.TabIndex = 23;
            // 
            // lblFlickrDateTaken
            // 
            this.lblFlickrDateTaken.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblFlickrDateTaken.AutoSize = true;
            this.lblFlickrDateTaken.Location = new System.Drawing.Point(289, 144);
            this.lblFlickrDateTaken.Name = "lblFlickrDateTaken";
            this.lblFlickrDateTaken.Size = new System.Drawing.Size(65, 13);
            this.lblFlickrDateTaken.TabIndex = 24;
            this.lblFlickrDateTaken.Text = "12/29/2008";
            this.lblFlickrDateTaken.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lblVisibility
            // 
            this.lblVisibility.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.lblVisibility.AutoSize = true;
            this.lblVisibility.Location = new System.Drawing.Point(845, 371);
            this.lblVisibility.Name = "lblVisibility";
            this.lblVisibility.Size = new System.Drawing.Size(40, 13);
            this.lblVisibility.TabIndex = 27;
            this.lblVisibility.Text = "Private";
            this.lblVisibility.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.toolTip1.SetToolTip(this.lblVisibility, "Click to change this photo\'s visibility.");
            // 
            // btnRemoveTagFromWholeSet
            // 
            this.btnRemoveTagFromWholeSet.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnRemoveTagFromWholeSet.Enabled = false;
            this.btnRemoveTagFromWholeSet.Location = new System.Drawing.Point(395, 306);
            this.btnRemoveTagFromWholeSet.Name = "btnRemoveTagFromWholeSet";
            this.btnRemoveTagFromWholeSet.Size = new System.Drawing.Size(55, 23);
            this.btnRemoveTagFromWholeSet.TabIndex = 5;
            this.btnRemoveTagFromWholeSet.TabStop = false;
            this.btnRemoveTagFromWholeSet.Text = "&Remove";
            this.btnRemoveTagFromWholeSet.UseVisualStyleBackColor = true;
            this.btnRemoveTagFromWholeSet.Click += new System.EventHandler(this.btnRemoveTagFromWholeSet_Click);
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label1.Location = new System.Drawing.Point(334, 307);
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
            this.axWMP.Location = new System.Drawing.Point(456, 162);
            this.axWMP.Name = "axWMP";
            this.axWMP.OcxState = ((System.Windows.Forms.AxHost.State)(resources.GetObject("axWMP.OcxState")));
            this.axWMP.Size = new System.Drawing.Size(429, 167);
            this.axWMP.TabIndex = 6;
            this.axWMP.Visible = false;
            // 
            // lblBrowse
            // 
            this.lblBrowse.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.lblBrowse.AutoSize = true;
            this.lblBrowse.Location = new System.Drawing.Point(712, 371);
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
            this.lnkPicture.Location = new System.Drawing.Point(759, 371);
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
            this.lnkSet.Location = new System.Drawing.Point(805, 371);
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
            this.pnlPictureBox.Location = new System.Drawing.Point(456, 162);
            this.pnlPictureBox.Name = "pnlPictureBox";
            this.pnlPictureBox.Size = new System.Drawing.Size(429, 167);
            this.pnlPictureBox.TabIndex = 32;
            // 
            // pictureBox
            // 
            this.pictureBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.pictureBox.Location = new System.Drawing.Point(0, 2);
            this.pictureBox.Name = "pictureBox";
            this.pictureBox.Size = new System.Drawing.Size(429, 162);
            this.pictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox.TabIndex = 6;
            this.pictureBox.TabStop = false;
            // 
            // dtpDateTaken
            // 
            this.dtpDateTaken.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.dtpDateTaken.Format = System.Windows.Forms.DateTimePickerFormat.Time;
            this.dtpDateTaken.Location = new System.Drawing.Point(560, 132);
            this.dtpDateTaken.Name = "dtpDateTaken";
            this.dtpDateTaken.ShowUpDown = true;
            this.dtpDateTaken.Size = new System.Drawing.Size(90, 20);
            this.dtpDateTaken.TabIndex = 33;
            this.dtpDateTaken.TabStop = false;
            // 
            // btnReUpload
            // 
            this.btnReUpload.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnReUpload.Location = new System.Drawing.Point(456, 345);
            this.btnReUpload.Name = "btnReUpload";
            this.btnReUpload.Size = new System.Drawing.Size(75, 23);
            this.btnReUpload.TabIndex = 34;
            this.btnReUpload.TabStop = false;
            this.btnReUpload.Text = "Re-Upload";
            this.btnReUpload.UseVisualStyleBackColor = true;
            // 
            // Main
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(885, 387);
            this.Controls.Add(this.lblFlickrDateTaken);
            this.Controls.Add(this.btnReUpload);
            this.Controls.Add(this.pnlPictureBox);
            this.Controls.Add(this.lnkSet);
            this.Controls.Add(this.lnkPicture);
            this.Controls.Add(this.dtpDateTaken);
            this.Controls.Add(this.lblBrowse);
            this.Controls.Add(this.lblVisibility);
            this.Controls.Add(this.axWMP);
            this.Controls.Add(this.flickrProgressBar);
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btnRemoveTagFromWholeSet);
            this.Controls.Add(this.btnAddTagToWholeSet);
            this.Controls.Add(this.txtTag);
            this.Controls.Add(this.tagReadingProgressBar);
            this.Controls.Add(this.btnSetDateTaken);
            this.Controls.Add(this.calDateTaken);
            this.Controls.Add(this.lblDateTaken);
            this.Controls.Add(this.lstTags);
            this.Controls.Add(this.lblGeotag);
            this.Controls.Add(this.txtPictureCaption);
            this.Controls.Add(this.pictureList);
            this.Controls.Add(this.btnSetDateTakenForWholeSet);
            this.MinimumSize = new System.Drawing.Size(901, 421);
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
        private ImageList imageList1;
        private DateTimePicker dtpDateTaken;
        private Button btnReUpload;
        private ToolStripMenuItem setDateTakenToolStripMenuItem;
        private ToolStripMenuItem sortSetsOnFlickrToolStripMenuItem;
        private ToolStripMenuItem copyGeoTagToolStripMenuItem;
        private ToolStripMenuItem pasteGeoTagToolStripMenuItem;
    }
}

