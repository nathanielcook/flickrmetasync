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
            this.pictureBox = new System.Windows.Forms.PictureBox();
            this.pictureList = new System.Windows.Forms.ListView();
            this.mnuSelected = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.addTagToSelectedMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.removeTagFromSelectedToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.lstTags = new System.Windows.Forms.ListView();
            this.lblGeotag = new System.Windows.Forms.Label();
            this.txtPictureCaption = new System.Windows.Forms.TextBox();
            this.tagReader = new System.ComponentModel.BackgroundWorker();
            this.tagReadingProgressBar = new System.Windows.Forms.ProgressBar();
            this.txtTag = new System.Windows.Forms.TextBox();
            this.btnAddTagToWholeSet = new System.Windows.Forms.Button();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.setList = new System.Windows.Forms.TreeView();
            this.lstAllTags = new System.Windows.Forms.ListView();
            this.mnuAllTags = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.changeThisTagMenuItem = new System.Windows.Forms.ToolStripMenuItem();
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
            this.makePublicToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.lblVisibility = new System.Windows.Forms.Label();
            this.makePrivateToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuSets = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.renameThisSetToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox)).BeginInit();
            this.mnuSelected.SuspendLayout();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.mnuAllTags.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.axWMP)).BeginInit();
            this.mnuSets.SuspendLayout();
            this.SuspendLayout();
            // 
            // timer1
            // 
            this.timer1.Enabled = true;
            this.timer1.Interval = 500;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // pictureBox
            // 
            this.pictureBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.pictureBox.Location = new System.Drawing.Point(456, 157);
            this.pictureBox.Name = "pictureBox";
            this.pictureBox.Size = new System.Drawing.Size(430, 156);
            this.pictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox.TabIndex = 5;
            this.pictureBox.TabStop = false;
            // 
            // pictureList
            // 
            this.pictureList.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.pictureList.ContextMenuStrip = this.mnuSelected;
            this.pictureList.Location = new System.Drawing.Point(292, 0);
            this.pictureList.Name = "pictureList";
            this.pictureList.Size = new System.Drawing.Size(413, 128);
            this.pictureList.TabIndex = 0;
            this.pictureList.UseCompatibleStateImageBehavior = false;
            // 
            // mnuSelected
            // 
            this.mnuSelected.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.addTagToSelectedMenuItem,
            this.removeTagFromSelectedToolStripMenuItem,
            this.makePublicToolStripMenuItem,
            this.makePrivateToolStripMenuItem});
            this.mnuSelected.Name = "mnuSelected";
            this.mnuSelected.Size = new System.Drawing.Size(196, 92);
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
            // lstTags
            // 
            this.lstTags.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)));
            this.lstTags.ForeColor = System.Drawing.Color.DarkGray;
            this.lstTags.Items.AddRange(new System.Windows.Forms.ListViewItem[] {
            listViewItem3});
            this.lstTags.Location = new System.Drawing.Point(292, 134);
            this.lstTags.Name = "lstTags";
            this.lstTags.Size = new System.Drawing.Size(158, 121);
            this.lstTags.TabIndex = 2;
            this.lstTags.UseCompatibleStateImageBehavior = false;
            this.lstTags.View = System.Windows.Forms.View.List;
            // 
            // lblGeotag
            // 
            this.lblGeotag.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblGeotag.AutoSize = true;
            this.lblGeotag.Location = new System.Drawing.Point(453, 355);
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
            this.txtPictureCaption.Location = new System.Drawing.Point(457, 317);
            this.txtPictureCaption.Name = "txtPictureCaption";
            this.txtPictureCaption.Size = new System.Drawing.Size(429, 13);
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
            this.tagReadingProgressBar.Location = new System.Drawing.Point(292, 345);
            this.tagReadingProgressBar.Name = "tagReadingProgressBar";
            this.tagReadingProgressBar.Size = new System.Drawing.Size(158, 23);
            this.tagReadingProgressBar.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
            this.tagReadingProgressBar.TabIndex = 10;
            this.tagReadingProgressBar.Visible = false;
            // 
            // txtTag
            // 
            this.txtTag.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.txtTag.Location = new System.Drawing.Point(292, 261);
            this.txtTag.Name = "txtTag";
            this.txtTag.Size = new System.Drawing.Size(158, 20);
            this.txtTag.TabIndex = 3;
            // 
            // btnAddTagToWholeSet
            // 
            this.btnAddTagToWholeSet.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnAddTagToWholeSet.Enabled = false;
            this.btnAddTagToWholeSet.Location = new System.Drawing.Point(292, 290);
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
            this.splitContainer1.Size = new System.Drawing.Size(287, 371);
            this.splitContainer1.SplitterDistance = 204;
            this.splitContainer1.TabIndex = 19;
            // 
            // setList
            // 
            this.setList.ContextMenuStrip = this.mnuSets;
            this.setList.Dock = System.Windows.Forms.DockStyle.Fill;
            this.setList.Location = new System.Drawing.Point(0, 0);
            this.setList.Name = "setList";
            this.setList.Size = new System.Drawing.Size(287, 204);
            this.setList.TabIndex = 0;
            // 
            // lstAllTags
            // 
            this.lstAllTags.ContextMenuStrip = this.mnuAllTags;
            this.lstAllTags.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lstAllTags.Location = new System.Drawing.Point(0, 0);
            this.lstAllTags.Name = "lstAllTags";
            this.lstAllTags.Size = new System.Drawing.Size(287, 163);
            this.lstAllTags.TabIndex = 23;
            this.lstAllTags.UseCompatibleStateImageBehavior = false;
            this.lstAllTags.View = System.Windows.Forms.View.List;
            // 
            // mnuAllTags
            // 
            this.mnuAllTags.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.changeThisTagMenuItem});
            this.mnuAllTags.Name = "mnuTag";
            this.mnuAllTags.Size = new System.Drawing.Size(153, 26);
            // 
            // changeThisTagMenuItem
            // 
            this.changeThisTagMenuItem.Name = "changeThisTagMenuItem";
            this.changeThisTagMenuItem.Size = new System.Drawing.Size(152, 22);
            this.changeThisTagMenuItem.Text = "&Change this Tag";
            // 
            // btnSetDateTaken
            // 
            this.btnSetDateTaken.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSetDateTaken.Location = new System.Drawing.Point(618, 131);
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
            this.lblDateTaken.Location = new System.Drawing.Point(446, 128);
            this.lblDateTaken.Name = "lblDateTaken";
            this.lblDateTaken.Size = new System.Drawing.Size(67, 13);
            this.lblDateTaken.TabIndex = 21;
            this.lblDateTaken.Text = "Date Taken:";
            this.lblDateTaken.Click += new System.EventHandler(this.lblDateTaken_Click);
            // 
            // btnSetDateTakenForWholeSet
            // 
            this.btnSetDateTakenForWholeSet.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSetDateTakenForWholeSet.Location = new System.Drawing.Point(513, 131);
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
            this.calDateTaken.Location = new System.Drawing.Point(708, 0);
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
            this.flickrProgressBar.Location = new System.Drawing.Point(293, 317);
            this.flickrProgressBar.Name = "flickrProgressBar";
            this.flickrProgressBar.Size = new System.Drawing.Size(157, 23);
            this.flickrProgressBar.TabIndex = 23;
            // 
            // lblFlickrDateTaken
            // 
            this.lblFlickrDateTaken.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblFlickrDateTaken.AutoSize = true;
            this.lblFlickrDateTaken.Location = new System.Drawing.Point(448, 144);
            this.lblFlickrDateTaken.Name = "lblFlickrDateTaken";
            this.lblFlickrDateTaken.Size = new System.Drawing.Size(65, 13);
            this.lblFlickrDateTaken.TabIndex = 24;
            this.lblFlickrDateTaken.Text = "12/29/2008";
            // 
            // btnRemoveTagFromWholeSet
            // 
            this.btnRemoveTagFromWholeSet.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnRemoveTagFromWholeSet.Enabled = false;
            this.btnRemoveTagFromWholeSet.Location = new System.Drawing.Point(395, 290);
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
            this.label1.Location = new System.Drawing.Point(334, 291);
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
            this.axWMP.Size = new System.Drawing.Size(430, 156);
            this.axWMP.TabIndex = 6;
            this.axWMP.Visible = false;
            // 
            // makePublicToolStripMenuItem
            // 
            this.makePublicToolStripMenuItem.Name = "makePublicToolStripMenuItem";
            this.makePublicToolStripMenuItem.Size = new System.Drawing.Size(195, 22);
            this.makePublicToolStripMenuItem.Text = "Make Selected Public";
            // 
            // lblVisibility
            // 
            this.lblVisibility.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.lblVisibility.AutoSize = true;
            this.lblVisibility.Location = new System.Drawing.Point(846, 355);
            this.lblVisibility.Name = "lblVisibility";
            this.lblVisibility.Size = new System.Drawing.Size(40, 13);
            this.lblVisibility.TabIndex = 27;
            this.lblVisibility.Text = "Private";
            this.lblVisibility.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // makePrivateToolStripMenuItem
            // 
            this.makePrivateToolStripMenuItem.Name = "makePrivateToolStripMenuItem";
            this.makePrivateToolStripMenuItem.Size = new System.Drawing.Size(195, 22);
            this.makePrivateToolStripMenuItem.Text = "Make Selected Private";
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
            // Main
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(886, 371);
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
            this.Controls.Add(this.pictureList);
            this.Controls.Add(this.lblGeotag);
            this.Controls.Add(this.pictureBox);
            this.Controls.Add(this.txtPictureCaption);
            this.MinimumSize = new System.Drawing.Size(756, 316);
            this.Name = "Main";
            this.Text = "Flickr Metadata Sync";
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox)).EndInit();
            this.mnuSelected.ResumeLayout(false);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.ResumeLayout(false);
            this.mnuAllTags.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.axWMP)).EndInit();
            this.mnuSets.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }           

        #endregion

        private Timer timer1;
        private FolderBrowserDialog folderPicker;
        private PictureBox pictureBox;
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
    }
}

