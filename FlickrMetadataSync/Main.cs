using System;
using System.Windows.Forms;
using Microsoft.Win32;
using FlickrNet;
using System.IO;
using System.Diagnostics;
using FlickrMetadataSync;
using System.Drawing;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Collections;
using DocuTrackProSE;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Soap;
using WMPLib;
using FlickrMetadataSync.Properties;
using System.Threading;

namespace FlickrMetadataSync
{
    public partial class Main : Form
    {
        //from the registry settings
        private static string flickrUserName;
        private static string localFlickrDirectory;
        private static string lastAllTagsScanDate;

        private static string appDataFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "FlickrMetadataSync\\");
        private static string allTagsFilePath = Path.Combine(appDataFolder, "tags.xml");

        private static NameValueCollection allTags = new NameValueCollection(100);
        private static NameValueCollection tagReaderTags = new NameValueCollection(100);
        private static StringCollection newlyScannedTags = new StringCollection();
        private static NameValueCollection setsWithIssues = new NameValueCollection();

        private int tagReadingProgress;
        private int numberOfFiles;

        private Flickr flickr = new Flickr(Settings.Default.flickrAppKey, Settings.Default.flickrSharedSecret);

        private Content currentContent;
        private string currentSetId;
        private string currentSetName = String.Empty;
        private string selectedNodeName;
        private Photosets photosets;
        private static StringDictionary picturesDictionary = new StringDictionary();
        private const int MAX_RETRIES = 5;

        private static object lock_AllTagsSerialization = new object();

        private DateTime? previousDateTaken;
        private double? previousGpsLatitude;
        private double? previousGpsLongitude;
        private int previousPictureIndex;
        private StringCollection selectedItems = new StringCollection();

        //drag and drop for the picture box
        private int xDrag;
        private int yDrag;
        private int xPictureBoxOriginal;
        private int yPictureBoxOriginal;
        private Point mouseDownLocation;
        private static object lock_tagReaderPause = new object();
        private bool needToSetFocusToPictureList = false;

        //if you change this, make sure you update the sVideo function.
        private string fileExtensions = "*.jpg;*.jpeg;*.avi;*.mov;*.mp4;*.3gp";

        public Main()
        {
            InitializeComponent();

            this.txtPictureCaption.Enter += new EventHandler(txtPictureCaption_Enter);
            this.txtPictureCaption.Leave += new EventHandler(txtPictureCaption_Leave);
            this.txtPictureCaption.KeyDown += new KeyEventHandler(txtPictureCaption_KeyDown);
            this.txtTag.TextChanged += new EventHandler(txtTag_TextChanged);
            this.txtTag.KeyDown += new KeyEventHandler(txtTag_KeyDown);
            this.pictureList.KeyDown += new KeyEventHandler(pictureList_KeyDown);
            this.pictureList.ItemSelectionChanged += new System.Windows.Forms.ListViewItemSelectionChangedEventHandler(this.pictureList_ItemSelectionChanged);
            this.setList.KeyDown += new KeyEventHandler(setList_KeyDown);
            this.setList.AfterCheck += new TreeViewEventHandler(setList_AfterCheck);
            this.FormClosing += new FormClosingEventHandler(Main_FormClosing);
            this.setList.AfterSelect += new TreeViewEventHandler(setList_AfterSelect);
            this.lstTags.KeyDown += new KeyEventHandler(lstTags_KeyDown);
            this.flickrGopher.DoWork += new DoWorkEventHandler(flickrGopher_DoWork);
            this.btnSetDateTaken.Click += new EventHandler(btnSetDateTaken_Click);
            this.btnSetDateTakenForWholeSet.Click += new EventHandler(btnSetDateTakenForWholeSet_Click);
            this.lblGeotag.Click += new EventHandler(lblGeotag_Click);
            this.lblFlickrDateTaken.Click += new EventHandler(lblFlickrDateTaken_Click);
            this.lstAllTags.KeyDown += new KeyEventHandler(lstAllTags_KeyDown);
            this.changeThisTagMenuItem.Click += new EventHandler(changeThisTagMenuItem_Click);
            this.addTagToSelectedMenuItem.Click += new EventHandler(addTagToSelectedMenuItem_Click);
            this.removeTagFromSelectedToolStripMenuItem.Click += new EventHandler(removeTagFromSelectedMenuItem_Click);
            this.makePublicToolStripMenuItem.Click += new EventHandler(makePublicToolStripMenuItem_Click);
            this.makePrivateToolStripMenuItem.Click += new EventHandler(makePrivateToolStripMenuItem_Click);
            this.renameThisSetToolStripMenuItem.Click += new EventHandler(renameThisSetToolStripMenuItem_Click);
            this.btnAddTagToWholeSet.Click += new EventHandler(btnAddTagToWholeSet_Click);
            this.lstTags.SelectedIndexChanged += new EventHandler(lstTags_SelectedIndexChanged);
            this.refreshAllTagsToolStripMenuItem.Click += new EventHandler(refreshAllTagsToolStripMenuItem_Click);
            this.copyTagsToolStripMenuItem.Click += new EventHandler(copyTagsToolStripMenuItem_Click);
            this.pasteTagsToolStripMenuItem.Click += new EventHandler(pasteTagsToolStripMenuItem_Click);
            this.lnkPicture.LinkClicked += new LinkLabelLinkClickedEventHandler(lnkPicture_LinkClicked);
            this.lnkSet.LinkClicked += new LinkLabelLinkClickedEventHandler(lnkSet_LinkClicked);
            this.pictureBox.DragOver += new DragEventHandler(pictureBox_DragOver);
            this.pictureBox.DragEnter += new DragEventHandler(pictureBox_DragEnter);
            this.pictureBox.DoubleClick += new EventHandler(pictureBox_DoubleClick);
            this.pictureBox.MouseDown += new MouseEventHandler(pictureBox_MouseDown);
            this.pictureBox.MouseMove += new MouseEventHandler(pictureBox_MouseMove);
            this.Resize += new EventHandler(Main_Resize);
            this.calDateTaken.DateChanged += new System.Windows.Forms.DateRangeEventHandler(this.calDateTaken_DateChanged);
            this.dtpDateTaken.ValueChanged += new EventHandler(dtpDateTaken_ValueChanged);
            this.lblVisibility.Click += new EventHandler(lblVisibility_Click);
            this.btnReUpload.Click += new EventHandler(btnReUpload_Click);
            this.setDateTakenToolStripMenuItem.Click += new EventHandler(setDateTakenToolStripMenuItem_Click);
            this.sortSetsOnFlickrToolStripMenuItem.Click += new EventHandler(sortSetsOnFlickrToolStripMenuItem_Click);
            this.copyGeoTagToolStripMenuItem.Click += new EventHandler(copyGeoTagToolStripMenuItem_Click);
            this.pasteGeoTagToolStripMenuItem.Click += new EventHandler(pasteGeoTagToolStripMenuItem_Click);

            //get flickr authorization token
            loadAuthToken();

            //populate location of sets from registry
            getRegistrySettings();

            this.Show();

            //put directories (sets) in tree view
            populateTreeView();

            //load all tags from disk
            loadAllTagsFromDisk();

            flickr.HttpTimeout = 180000; //wait up to three minutes to get photosets

            if (lastAllTagsScanDate == "" || DateTime.Now.Subtract(DateTime.Parse(lastAllTagsScanDate)).Days > 3)
            {
                //if it's been more than 3 days since the last all tags scan
                //begin reading tags in the background
                //leave this as the last line in Main()
                tagReader.RunWorkerAsync();
            }
        }

        void pasteGeoTagToolStripMenuItem_Click(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        void copyGeoTagToolStripMenuItem_Click(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        void setDateTakenToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show(string.Format("Are you sure you want to set the date for all selected items to {0:F}? This will take place immediately on flickr too.", dtpDateTaken.Value), Application.ProductName, MessageBoxButtons.OKCancel) == DialogResult.OK)
            {
                if (MessageBox.Show("Are you sure? This is for all selected items!", Application.ProductName, MessageBoxButtons.OKCancel) == DialogResult.OK)
                {
                    try
                    {
                        Cursor.Current = Cursors.WaitCursor;
                        setDateTakenForAllItems(pictureList.SelectedItems);
                    }
                    finally
                    {
                        mergeLocalInUI(pictureList.SelectedItems[0].Text);
                        Cursor.Current = Cursors.Default;
                        pictureList.Focus();
                    }
                }
            }
        }

        void btnReUpload_Click(object sender, EventArgs e)
        {
            if (pictureList.SelectedItems.Count == 1 && currentContent != null && currentContent.flickrLoaded)
            {
                if (MessageBox.Show(string.Format("Are you sure you want to re-upload the following photo? \r\n\r\n{0}\r\n\r\nThis will replace the existing photo on flickr.", currentContent.filename), Application.ProductName, MessageBoxButtons.OKCancel) == DialogResult.OK)
                {
                    try
                    {
                        Cursor.Current = Cursors.WaitCursor;

                        flickr.ReplacePicture(currentContent.filename, currentContent.flickrID);
                    }
                    finally
                    {
                        Cursor.Current = Cursors.Default;
                        pictureList.Focus();
                    }
                }
            }

        }

        void lblVisibility_Click(object sender, EventArgs e)
        {
            if (pictureList.SelectedItems.Count == 1)
            {
                if (lblVisibility.Text.ToLower().Equals("private"))
                    changeVisibility(1, pictureList.SelectedItems);
                else if (lblVisibility.Text.ToLower().Equals("public"))
                    changeVisibility(0, pictureList.SelectedItems);
            }
        }

        void dtpDateTaken_ValueChanged(object sender, EventArgs e)
        {
            calDateTaken.SetDate(dtpDateTaken.Value);
        }

        void pasteTagsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (pictureList.SelectedItems.Count > 0)
            {
                string clipboardText = Clipboard.GetText(TextDataFormat.Text);
                string[] tags = clipboardText.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
                bool oneOrMoreTagsTooLong = false;

                foreach (string tag in tags)
                {
                    if (tag.Length > 50)
                    {
                        oneOrMoreTagsTooLong = true;
                    }
                }

                if (tags.Length == 0 || oneOrMoreTagsTooLong)
                {
                    MessageBox.Show("There are no tags in the clipboard or the tags are not in the correct format. Each tag must be on a separate line, with no double quotes. Each tag must be no more than 50 characters. Paste unsuccessful.");
                }
                else
                {
                    if (MessageBox.Show("Are you sure you want to add the following tags to the selected photos?" + "\r\n\r\n" + clipboardText, Application.ProductName, MessageBoxButtons.OKCancel) == DialogResult.OK)
                    {
                        Cursor.Current = Cursors.WaitCursor;

                        foreach (string tag in tags)
                            addTagToAllItems(tag, pictureList.SelectedItems);

                        Cursor.Current = Cursors.Default;
                    }
                }
            }
        }

        void copyTagsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Cursor.Current = Cursors.WaitCursor;

            string clipboardText = "";
            StringCollection copiedSoFar = new StringCollection();

            if (pictureList.SelectedItems.Count > 0)
            {
                foreach (ListViewItem item in pictureList.SelectedItems)
                {
                    string fullFilePath = Path.Combine(setList.SelectedNode.Name, item.Text);

                    Content content;
                    bool isPicture = false;
                    if (isVideo(item.Text))
                        content = new Video(fullFilePath);
                    else
                    {
                        content = new Picture(fullFilePath);
                        isPicture = true;
                    }

                    content.flickrID = picturesDictionary[Path.GetFileNameWithoutExtension(content.filename)];

                    if (isPicture)
                    {
                        foreach (string tag in ((Picture)content).tags)
                        {
                            if (!copiedSoFar.Contains(tag))
                            {
                                clipboardText += tag + "\r\n";
                                copiedSoFar.Add(tag);
                            }
                        }
                    }

                    //get flickr tags
                    PhotoInfo photoInfo = flickr.PhotosGetInfo(content.flickrID);
                    foreach (PhotoInfoTag photoInfoTag in photoInfo.Tags.TagCollection)
                    {
                        string tag = photoInfoTag.Raw;
                        if (!copiedSoFar.Contains(tag))
                        {
                            clipboardText += tag + "\r\n";
                            copiedSoFar.Add(tag);
                        }
                    }
                }

                //remove the last \r\n
                if (clipboardText.Length > 0)
                    clipboardText = clipboardText.Substring(0, clipboardText.Length - 2);

                Clipboard.SetText(clipboardText, TextDataFormat.Text);
            }

            Cursor.Current = Cursors.Default;
        }

        void refreshAllTagsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            tagReader.RunWorkerAsync();
        }

        void renameThisSetToolStripMenuItem_Click(object sender, EventArgs e)
        {
            renameSet();
        }

        private void renameSet()
        {
            axWMP.URL = null;
            string thisSetID = currentSetId;
            string oldSetName = setList.SelectedNode.Text;
            string newSetName = InputBox(string.Format("Enter the new name for the set (currently named {0}):", oldSetName), Application.ProductName, oldSetName);

            string oldFolderName = setList.SelectedNode.Name;
            string newFolderName = setList.SelectedNode.Name.Replace(oldSetName, newSetName);

            if (newSetName.Length > 0 && MessageBox.Show(string.Format("Are you sure you want to change \"{0}\" to \"{1}\"?", oldSetName, newSetName), Application.ProductName, MessageBoxButtons.OKCancel) == DialogResult.OK)
            {
                try
                {
                    Cursor.Current = Cursors.WaitCursor;

                    if (thisSetID != null && thisSetID != "")
                    {
                        Photoset thisSet = flickr.PhotosetsGetInfo(thisSetID);

                        if (thisSet.Title.Equals(oldSetName))
                        {
                            flickr.PhotosetsEditMeta(thisSetID, newSetName, thisSet.Description);
                        }
                    }

                    if (oldFolderName.ToLower().Equals(newFolderName.ToLower()))
                    {
                        //since the windows directory structure is case insensitive, we must do this
                        //to allow for someone just wanting to change the casing of a set/folder.
                        string tmpFolderName = setList.SelectedNode.Name + "_tmp";

                        Directory.Move(oldFolderName, tmpFolderName);
                        Directory.Move(tmpFolderName, newFolderName);
                    }
                    else
                    {
                        Directory.Move(oldFolderName, newFolderName);
                    }

                    //rename the sets in photosets variable
                    for (int i = 0; i < photosets.PhotosetCollection.Length; i++)
                    {
                        if (photosets.PhotosetCollection[i].Title.Equals(oldSetName) && photosets.PhotosetCollection[i].PhotosetId.Equals(thisSetID))
                        {
                            photosets.PhotosetCollection[i].Title = newSetName;
                        }
                    }

                    //rename the sets in the alltags and tagreader tags NameValueCollections
                    //pause the tag reader during this operation
                    lock (lock_tagReaderPause)
                    {
                        for (int i = 0; i <= 1; i++)
                        {
                            NameValueCollection tagCollection;

                            if (i == 0)
                                tagCollection = allTags;
                            else
                                tagCollection = tagReaderTags;

                            NameValueCollection changeQueue = new NameValueCollection();

                            foreach (string tag in tagCollection.AllKeys)
                            {
                                foreach (string pictureFileName in tagCollection.GetValues(tag))
                                {
                                    if (getFolderName(pictureFileName).ToLower().Equals(oldSetName.ToLower()))
                                    {
                                        changeQueue.Add(tag, pictureFileName);
                                    }
                                }
                            }

                            foreach (string tag in changeQueue.AllKeys)
                            {
                                foreach (string pictureFileName in changeQueue.GetValues(tag))
                                {
                                    removeOnlyThisKeyValueCombo(tagCollection, tag, pictureFileName);
                                    tagCollection.Add(tag, pictureFileName.Replace(oldSetName, newSetName));
                                }
                            }
                        }
                    }
                }
                finally
                {
                    Cursor.Current = Cursors.Default;
                }

                populateTreeView();
                setList.SelectedNode = setList.Nodes.Find(newFolderName, true)[0];
            }
        }

        private void renamePhoto()
        {
            if (currentContent != null && currentContent.flickrLoaded && pictureList.SelectedItems.Count == 1 && Path.GetFileName(currentContent.filename).Equals(pictureList.SelectedItems[0].Text))
            {
                string oldPhotoName = pictureList.SelectedItems[0].Text;
                string newPhotoName = InputBox(string.Format("Enter the new file name for the photo (currently named {0}).\r\n\r\nNote: Include the file extension!", oldPhotoName), Application.ProductName, oldPhotoName);

                if (newPhotoName.Length > 0 && newPhotoName.IndexOf(".") > -1 && MessageBox.Show(string.Format("Are you sure you want to change \"{0}\" to \"{1}\"?", oldPhotoName, newPhotoName), Application.ProductName, MessageBoxButtons.OKCancel) == DialogResult.OK)
                {
                    try
                    {
                        Cursor.Current = Cursors.WaitCursor;

                        PhotoInfo photoInfo = flickr.PhotosGetInfo(currentContent.flickrID);

                        if (photoInfo.Title.Equals(Path.GetFileNameWithoutExtension(oldPhotoName)))
                        {
                            flickr.PhotosSetMeta(photoInfo.PhotoId, Path.GetFileNameWithoutExtension(newPhotoName), photoInfo.Description);
                        }

                        string oldPhotoFilePath = Path.Combine(setList.SelectedNode.Name, oldPhotoName);
                        string newPhotoFilePath = Path.Combine(setList.SelectedNode.Name, newPhotoName);

                        if (oldPhotoFilePath.ToLower().Equals(newPhotoFilePath.ToLower()))
                        {
                            //since the windows directory structure is case insensitive, we must do this
                            //to allow for someone just wanting to change the casing of a set/folder.
                            string tmpFilePath = Path.Combine(setList.SelectedNode.Name, oldPhotoName + "_tmp");

                            File.Move(oldPhotoFilePath, tmpFilePath);
                            File.Move(tmpFilePath, newPhotoFilePath);
                        }
                        else
                        {
                            File.Move(oldPhotoFilePath, newPhotoFilePath);
                        }

                        //rename the sets in pictureDictionary
                        picturesDictionary.Remove(Path.GetFileNameWithoutExtension(oldPhotoName));
                        picturesDictionary.Add(Path.GetFileNameWithoutExtension(newPhotoName), currentContent.flickrID);

                        //rename the sets in the alltags and tagreader tags NameValueCollections
                        //pause the tag reader during this operation
                        lock (lock_tagReaderPause)
                        {
                            for (int i = 0; i <= 1; i++)
                            {
                                NameValueCollection tagCollection;

                                if (i == 0)
                                    tagCollection = allTags;
                                else
                                    tagCollection = tagReaderTags;

                                NameValueCollection changeQueue = new NameValueCollection();

                                foreach (string tag in tagCollection.AllKeys)
                                {
                                    foreach (string pictureFileName in tagCollection.GetValues(tag))
                                    {
                                        if (pictureFileName.Equals(oldPhotoFilePath))
                                        {
                                            changeQueue.Add(tag, pictureFileName);
                                        }
                                    }
                                }

                                foreach (string tag in changeQueue.AllKeys)
                                {
                                    foreach (string pictureFileName in changeQueue.GetValues(tag))
                                    {
                                        removeOnlyThisKeyValueCombo(tagCollection, tag, pictureFileName);
                                        tagCollection.Add(tag, newPhotoFilePath);
                                    }
                                }
                            }
                        }
                    }
                    finally
                    {
                        Cursor.Current = Cursors.Default;
                    }
                    populatePictureList(setList.SelectedNode.Name);
                    foreach (ListViewItem item in pictureList.Items)
                    {
                        if (item.Text.ToLower().Equals(newPhotoName.ToLower()))
                        {
                            item.Selected = true;
                            item.Focused = true;
                            item.EnsureVisible();
                        }
                        else
                            item.Selected = false;
                    }
                }
            }
        }


        private void removeOnlyThisKeyValueCombo(NameValueCollection nvc, string key, string value)
        {
            List<String> listOfValuesWithThisKey = new List<String>(nvc.GetValues(key));

            //removes all values with this key (tag)
            nvc.Remove(key);

            //if there was more than 1, lets add the others back
            if (listOfValuesWithThisKey.Count > 1)
            {
                //exclude the one we are removing
                listOfValuesWithThisKey.Remove(value);

                foreach (string valueStillHasKey in listOfValuesWithThisKey)
                {
                    nvc.Add(key, valueStillHasKey);
                }
            }
        }

        void makePublicToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (pictureList.SelectedItems.Count > 0)
            {
                if (MessageBox.Show("Are you sure you want to make the selected photos public?", Application.ProductName, MessageBoxButtons.OKCancel) == DialogResult.OK)
                {
                    try
                    {
                        Cursor.Current = Cursors.WaitCursor;

                        changeVisibility(1, pictureList.SelectedItems);
                    }
                    finally
                    {
                        Cursor.Current = Cursors.Default;
                    }
                }
            }
        }

        void makePrivateToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (pictureList.SelectedItems.Count > 0)
            {
                if (MessageBox.Show("Are you sure you want to make the selected photos private?", Application.ProductName, MessageBoxButtons.OKCancel) == DialogResult.OK)
                {
                    try
                    {
                        Cursor.Current = Cursors.WaitCursor;

                        changeVisibility(0, pictureList.SelectedItems);
                    }
                    finally
                    {
                        Cursor.Current = Cursors.Default;
                    }
                }
            }
        }

        void lstTags_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lstTags.SelectedItems.Count > 0)
                btnRemoveTagFromWholeSet.Enabled = true;
            else
                btnRemoveTagFromWholeSet.Enabled = false;
        }

        void changeThisTagMenuItem_Click(object sender, EventArgs e)
        {
            if (lstAllTags.SelectedItems.Count > 0)
            {
                ListViewItem item = lstAllTags.SelectedItems[0];

                string oldTag = item.Text;
                string newTag = InputBox("Enter the new tag", "Change \"" + oldTag + "\" to what?", "");

                if (!newTag.Equals(oldTag) && newTag.Length > 0 && MessageBox.Show(string.Format("Are you sure you want to change \"{0}\" to \"{1}\"?", oldTag, newTag), Application.ProductName, MessageBoxButtons.OKCancel) == DialogResult.OK)
                {
                    try
                    {
                        Cursor.Current = Cursors.WaitCursor;

                        string[] values = allTags.GetValues(oldTag);

                        allTags.Remove(oldTag);
                        if (tagReader.IsBusy)
                        {
                            tagReaderTags.Remove(oldTag);
                        }

                        foreach (string contentFileName in values)
                        {
                            string setName = getFolderName(contentFileName);

                            Content content = null;
                            if (isVideo(contentFileName))
                                content = new Video(contentFileName);
                            else
                                content = new Picture(contentFileName);

                            //load flickr tags BEGIN ----------------------------
                            for (int i = 0; i < photosets.PhotosetCollection.Length; i++)
                            {
                                if (photosets.PhotosetCollection[i].Title.Equals(setName))
                                {
                                    string setID = photosets.PhotosetCollection[i].PhotosetId;

                                    Photo[] photosThisSet = flickr.PhotosetsGetPhotos(setID).PhotoCollection;

                                    for (int k = 0; k < photosThisSet.Length; k++)
                                    {
                                        if (photosThisSet[k].Title.Equals(Path.GetFileNameWithoutExtension(contentFileName), StringComparison.OrdinalIgnoreCase))
                                        {
                                            content.flickrID = photosThisSet[k].PhotoId;
                                            break;
                                        }
                                    }
                                    break;
                                }
                            }

                            if (content.flickrID == null)
                                throw new Exception();
                            else
                            {
                                PhotoInfo photoInfo = flickr.PhotosGetInfo(content.flickrID);
                                content.loadFlickrInfo(photoInfo);
                            }
                            //load flickr tags END ----------------------------

                            addTagIfNotAlreadyThere(newTag, content, allTags);
                            if (tagReader.IsBusy)
                            {
                                addTagIfNotAlreadyThere(newTag, content, tagReaderTags);
                            }

                            //local BEGIN: remove old tag, add new tag-----------------
                            if (content is Picture)
                            {
                                Picture picture = (Picture)content;
                                picture.tags.Remove(oldTag);
                                if (!picture.tags.Contains(newTag))
                                {
                                    picture.tags.Add(newTag);
                                }
                                picture.Save();
                            }
                            //local END: remove old tag, add new tag-------------------

                            //flickr BEGIN: remove old tag, add new tag----------------
                            if (!content.flickrLoaded)
                                throw new Exception();
                            else
                            {
                                content.flickrTags.Remove(oldTag);
                                if (!content.flickrTags.Contains(newTag))
                                {
                                    content.flickrTags.Add(newTag);
                                }
                                setFlickrTags(content);
                            }
                            //flickr END: remove old tag, add new tag------------------

                            //IF IT'S THE CURRENT PICTURE, REFRESH IT
                            if (currentContent.flickrID != null && currentContent.flickrID.Equals(content.flickrID))
                            {
                                mergeLocalInUI(Path.GetFileName(currentContent.filename));
                            }
                        }

                        //update the ListView controls BEGIN -------------
                        lstAllTags.Items.Remove(item);
                        string copyOfNewTag = newTag;
                        if (!containsTag(lstAllTags, ref copyOfNewTag))
                        {
                            lstAllTags.Items.Add(newTag, newTag, 0);
                        }
                        //update the ListView controls END ---------------

                        saveAllTagsToDisk(allTags);
                        populateAllTagsListView();
                    }
                    finally
                    {
                        Cursor.Current = Cursors.Default;
                        txtTag.Text = "";
                    }
                } // are you sure you want to change this tag?
            } //if (lstAllTags.SelectedItems.Count > 0)

            pictureList.Select();
        }

        void lstAllTags_KeyDown(object sender, KeyEventArgs e)
        {
            if (lstAllTags.SelectedItems.Count > 0 && e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;
                txtTag.Text = lstAllTags.SelectedItems[0].Text;
                txtTag.Focus();
                txtTag.SelectionStart = txtTag.Text.Length;
            }
            else if (e.KeyCode == Keys.Escape)
            {
                txtTag.Focus();
                txtTag.SelectionStart = txtTag.Text.Length;
            }
        }

        void lblFlickrDateTaken_Click(object sender, EventArgs e)
        {
            if (currentContent is Picture)
            {
                Picture currentPicture = ((Picture)currentContent);

                if (MessageBox.Show("Update the local picture to use flickr's date taken?", Application.ProductName, MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    currentPicture.dateTaken = currentPicture.flickrDateTaken.Value;
                    currentPicture.Save();
                }
                else if (MessageBox.Show("Update flickr to use the local picture's date taken?", Application.ProductName, MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    currentPicture.flickrDateTaken = currentPicture.dateTaken.Value;
                    flickr.PhotosSetDates(currentPicture.flickrID, currentPicture.flickrDatePosted.Value, currentPicture.flickrDateTaken.Value, DateGranularity.FullDate);
                }

                mergeLocalInUI(pictureList.SelectedItems[0].Text);
            }
        }

        void lblGeotag_Click(object sender, EventArgs e)
        {
            if (currentContent is Picture)
            {
                Picture currentPicture = ((Picture)currentContent);

                if (lblGeotag.Text.IndexOf("CONFLICT") > -1)
                {
                    if (MessageBox.Show("Update the local picture to use flickr's GPS data?", Application.ProductName, MessageBoxButtons.YesNo) == DialogResult.Yes)
                    {
                        currentPicture.gpsLatitude = currentPicture.flickrGpsLatitude;
                        currentPicture.gpsLongitude = currentPicture.flickrGpsLongitude;
                        currentPicture.Save();
                    }
                    else if (MessageBox.Show("Update flickr to use the local picture's GPS data?", Application.ProductName, MessageBoxButtons.YesNo) == DialogResult.Yes)
                    {
                        currentPicture.flickrGpsLatitude = currentPicture.gpsLatitude;
                        currentPicture.flickrGpsLongitude = currentPicture.gpsLongitude;
                        flickr.PhotosGeoSetLocation(currentPicture.flickrID, currentPicture.flickrGpsLatitude.Value, currentPicture.flickrGpsLongitude.Value);
                    }

                    mergeLocalInUI(pictureList.SelectedItems[0].Text);
                }
            } //if (currentContent is Picture)
            else
            {
                if (!currentContent.flickrGpsLatitude.HasValue && !currentContent.flickrGpsLongitude.HasValue)
                {
                    if (MessageBox.Show("Set the flickr GPS data equal to the previously viewed content's GPS data?", Application.ProductName, MessageBoxButtons.YesNo) == DialogResult.Yes)
                    {
                        currentContent.flickrGpsLatitude = previousGpsLatitude;
                        currentContent.flickrGpsLongitude = previousGpsLongitude;
                        flickr.PhotosGeoSetLocation(currentContent.flickrID, currentContent.flickrGpsLatitude.Value, currentContent.flickrGpsLongitude.Value);
                        mergeFlickrInUI();
                    }
                }
            }
        }

        void lstTags_KeyDown(object sender, KeyEventArgs e)
        {
            if (lstTags.SelectedItems.Count > 0 && e.KeyCode == Keys.Delete)
            {
                e.SuppressKeyPress = true;
                ListViewItem item = lstTags.SelectedItems[0];

                if (currentContent is Picture)
                {
                    Picture currentPicture = ((Picture)currentContent);

                    currentPicture.tags.Remove(item.Text);
                    currentPicture.Save();
                }

                // if exists in alltags remove from alltags
                removeTagIfThere(item.Text, currentContent, allTags);
                if (tagReader.IsBusy)
                {
                    // if tagreader is busy and exists in tagreader tags, remove from tagreader tags
                    removeTagIfThere(item.Text, currentContent, tagReaderTags);
                }

                removeTagFromLstAllTagsIfNeeded(item.Text);

                if (currentContent.flickrTags != null)
                {
                    currentContent.flickrTags.Remove(item.Text);
                    setFlickrTags(currentContent);
                }

                lstTags.Items.Remove(item);
                pictureList.Select();
            }
            else if (e.KeyCode == Keys.Left || e.KeyCode == Keys.Right)
            {
                pictureList.Select();
                pictureList_KeyDown(pictureList, e);
                e.SuppressKeyPress = true;
            }
        }

        private void removeTagFromLstAllTagsIfNeeded(string tag)
        {
            //if as a result of removing this tag, the tag doesn't exist in all tags, then there
            //are no pictures with that tag anymore. Remove it from lstAllTags.
            if (allTags.GetValues(tag) == null)
            {
                ListViewItem[] items = lstAllTags.Items.Find(tag, true);
                if (items.Length > 0)
                {
                    lstAllTags.Items.Remove(items[0]);
                }
            }
        }

        void setList_AfterCheck(object sender, TreeViewEventArgs e)
        {
            pictureList.Select();
        }

        private void loadAllTagsFromDisk()
        {
            if (!Directory.Exists(appDataFolder))
            {
                Directory.CreateDirectory(appDataFolder);
            }

            if (File.Exists(allTagsFilePath))
            {
                using (FileStream fileStream = new FileStream(allTagsFilePath, FileMode.OpenOrCreate))
                {
                    SoapFormatter soapFormatter = new SoapFormatter();
                    allTags = soapFormatter.Deserialize(fileStream) as NameValueCollection;
                }
            }

            foreach (string tag in allTags)
            {
                lstAllTags.Items.Add(tag, tag, 0);
            }
        }

        void txtTag_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                if (pictureList.SelectedItems.Count > 1)
                {
                    string tag = InputBox("OK to add this tag to all selected items?", "There is more than one picture/video selected", txtTag.Text);

                    if (tag.Length > 0)
                    {
                        try
                        {
                            Cursor.Current = Cursors.WaitCursor;

                            addTagToAllItems(tag, pictureList.SelectedItems);
                        }
                        finally
                        {
                            Cursor.Current = Cursors.Default;
                            txtTag.Clear();
                            pictureList.Focus();
                        }
                    }
                }
                else
                {
                    //this is so it will do nothing if they are attempting to add a tag to a video and flickr
                    //is not yet loaded.
                    if (currentContent is Picture || (currentContent is Video && currentContent.flickrLoaded))
                    {
                        string tag = txtTag.Text.Trim();

                        if (tag.ToLower().Equals("me"))
                            tag = "myself";

                        //if it's not already there.
                        if (!containsTag(lstTags, ref tag))
                        {
                            //add to lstAllTags if not already there
                            if (!containsTag(lstAllTags, ref tag))
                            {
                                lstAllTags.Items.Add(tag, tag, 0);
                            }

                            //add to alltags if not already there
                            addTagIfNotAlreadyThere(tag, currentContent, allTags);
                            if (tagReader.IsBusy)
                            {
                                addTagIfNotAlreadyThere(tag, currentContent, tagReaderTags);
                            }

                            saveAllTagsToDisk(allTags);
                            populateAllTagsListView();

                            if (currentContent is Picture)
                            {
                                Picture currentPicture = ((Picture)currentContent);

                                //now actually add the tag to the picture
                                currentPicture.tags.Add(tag);
                                currentPicture.Save();

                            } //if (currentContent is Picture)

                            ListViewItem item = new ListViewItem(tag);

                            if (currentContent.flickrTags != null)
                            {
                                currentContent.flickrTags.Add(tag);
                                setFlickrTags(currentContent);

                                item.ForeColor = Color.Blue;
                            }
                            else
                            {
                                //this only applies to a picture
                                item.ForeColor = Color.Red;
                            }

                            lstTags.Items.Add(item);
                            item.EnsureVisible();

                        } //if (!containsTag(lstTags, ref tag))

                        txtTag.Clear();
                        pictureList.Select();

                    } //if (currentcontent is Picture || (currentContent is Video && currentContent.flickrLoaded))
                }
            }
            else if (e.KeyCode == Keys.Space && e.Control == true)
            {
                e.SuppressKeyPress = true;
                if (lstAllTags.Items.Count > 1)
                {
                    lstAllTags.Select();
                    lstAllTags.Items[0].Selected = true;
                }
                else if (lstAllTags.Items.Count == 1)
                {
                    txtTag.Text = lstAllTags.Items[0].Text;
                    txtTag.SelectionStart = txtTag.Text.Length;
                }
            }
            else if (e.KeyCode == Keys.OemQuotes && e.Shift == true)
            {
                //don't allow double quotes in tags
                e.SuppressKeyPress = true;
            }
            else if (e.KeyCode == Keys.Back && txtTag.Text.Length == 1 && txtTag.SelectionStart == 1)
            {
                txtTag.Text = "";
                pictureList.Select();
                e.SuppressKeyPress = true;
            }
        }

        private bool addTagIfNotAlreadyThere(string tag, Content content, NameValueCollection tagCollection)
        {
            string fileName = content.filename;

            bool foundIt = false;

            if (tagCollection.GetValues(tag) != null)
            {
                foreach (string contentFileName in tagCollection.GetValues(tag))
                {
                    if (contentFileName.Equals(fileName, StringComparison.OrdinalIgnoreCase))
                        foundIt = true;
                }
            }

            if (!foundIt)
                tagCollection.Add(tag, fileName);

            return !foundIt; //returns true if it had to ADD the tag to the NameValueCollection.
        }

        private void removeTagIfThere(string tag, Content content, NameValueCollection tagCollection)
        {
            string fileName = content.filename;

            if (tagCollection.GetValues(tag) != null)
            {
                removeOnlyThisKeyValueCombo(tagCollection, tag, fileName);
            }
        }

        private bool containsTag(ListView listView, ref string tag)
        {
            bool containsTag = false;
            foreach (ListViewItem item in listView.Items)
            {
                if (item.Text.ToLower().Equals(tag.ToLower()))
                {
                    tag = item.Text;
                    containsTag = true;
                    break;
                }
            }
            return containsTag;
        }

        void txtTag_TextChanged(object sender, EventArgs e)
        {
            if (txtTag.Text.Length > 0)
                btnAddTagToWholeSet.Enabled = true;
            else
            {
                btnAddTagToWholeSet.Enabled = false;
                pictureList.Focus();
            }

            lstAllTags.Clear();

            string text = txtTag.Text.Trim();
            foreach (string tag in allTags)
            {
                if (tag.ToLower().StartsWith(text.ToLower()))
                    lstAllTags.Items.Add(tag, tag, 0);
            }

        }

        void txtPictureCaption_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;

                if (!isVideo(currentContent.filename))
                {
                    Picture picture = (Picture)currentContent;

                    if (txtPictureCaption.Text.Equals("") && (picture.caption == null || picture.caption.Equals("")))
                    {
                        //do nothing
                    }
                    else if (!txtPictureCaption.Text.Equals(picture.caption))
                    {
                        picture.caption = txtPictureCaption.Text.Trim();
                        picture.Save();
                        flickr.PhotosSetMeta(picture.flickrID, picture.flickrTitle, picture.caption);
                    }
                }

                pictureList.Focus();
            }
            else if (e.KeyCode == Keys.Escape)
            {
                if (txtPictureCaption.Tag != null)
                    txtPictureCaption.Text = txtPictureCaption.Tag.ToString();
                else
                    txtPictureCaption.Text = "";

                e.SuppressKeyPress = true;
                pictureList.Focus();
            }
        }

        void txtPictureCaption_Leave(object sender, EventArgs e)
        {
            txtPictureCaption.BackColor = this.BackColor;
        }

        void txtPictureCaption_Enter(object sender, EventArgs e)
        {
            txtPictureCaption.BackColor = Color.White;
            txtPictureCaption.Tag = txtPictureCaption.Text;
        }

        private void getRegistrySettings()
        {
            RegistryKey key = Registry.CurrentUser.OpenSubKey(Settings.Default.registryPath, true);

            if (key == null)
                key = Registry.CurrentUser.CreateSubKey(Settings.Default.registryPath, RegistryKeyPermissionCheck.ReadWriteSubTree);
            else
            {
                localFlickrDirectory = key.GetValue("localFlickrDirectory", "").ToString();
                flickrUserName = key.GetValue("flickrUserName", "").ToString();
                lastAllTagsScanDate = key.GetValue("lastAllTagsScanDate", "").ToString();
            }

            if (localFlickrDirectory == "")
            {
                folderPicker.Description = "Choose the root folder of your flickr photos/directories.";

                if (folderPicker.ShowDialog() == DialogResult.Cancel)
                    Environment.Exit(0);
                else
                {
                    localFlickrDirectory = folderPicker.SelectedPath;
                    key.SetValue("localFlickrDirectory", localFlickrDirectory);
                }
            }

            if (flickrUserName == "")
            {
                flickrUserName = InputBox("Enter your flickr user name.\r\n\r\n(Enter carefully; You will only be asked once!)", "Flickr user name needed", "");

                if (flickrUserName == "")
                    Environment.Exit(0);
                else
                    key.SetValue("flickrUserName", flickrUserName);
            }
        }

        private void populateTreeView()
        {
            setList.Nodes.Clear();

            appendDirectoriesToTreeNode(null, localFlickrDirectory);

            if (setList.Nodes.Count > 0)
                setList.SelectedNode = setList.Nodes[0];

            pictureList.Select();
        }

        void setList_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Enter)
            {
                e.SuppressKeyPress = true;
                pictureList.Select();
            }
            else if ((e.KeyCode >= Keys.A && e.KeyCode <= Keys.Z) || e.KeyCode == Keys.OemMinus)
            {
                e.SuppressKeyPress = true;
                pictureList.Select();
            }
            else if (e.KeyCode == Keys.F2 && currentSetName.Equals(selectedNodeName) && currentSetId != null)
            {
                renameSet();
            }
        }

        void pictureList_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Left)
            {
                e.SuppressKeyPress = true;
                SendKeys.Send("{UP}");
            }
            else if (e.KeyData == Keys.Right)
            {
                e.SuppressKeyPress = true;
                SendKeys.Send("{DOWN}");
            }
            else if (e.KeyCode == Keys.A && e.Modifiers == Keys.Control)
            {
                txtPictureCaption.Focus();
            }
            else if (e.KeyCode >= Keys.D0 & e.KeyCode <= Keys.D9)
            {
                e.SuppressKeyPress = true;

                switch (e.KeyCode)
                {
                    case Keys.D0:
                        txtTag.Text = "0";
                        break;
                    case Keys.D1:
                        txtTag.Text = "1";
                        break;
                    case Keys.D2:
                        txtTag.Text = "2";
                        break;
                    case Keys.D3:
                        txtTag.Text = "3";
                        break;
                    case Keys.D4:
                        txtTag.Text = "4";
                        break;
                    case Keys.D5:
                        txtTag.Text = "5";
                        break;
                    case Keys.D6:
                        txtTag.Text = "6";
                        break;
                    case Keys.D7:
                        txtTag.Text = "7";
                        break;
                    case Keys.D8:
                        txtTag.Text = "8";
                        break;
                    case Keys.D9:
                        txtTag.Text = "9";
                        break;
                }
                txtTag.Focus();
                txtTag.SelectionStart = txtTag.Text.Length;
            }
            else if (e.KeyCode >= Keys.A && e.KeyCode <= Keys.Z)
            {
                if (e.KeyCode == Keys.C && e.Control)
                {
                    copyTagsToolStripMenuItem_Click(copyTagsToolStripMenuItem, new EventArgs());
                }
                else if (e.KeyCode == Keys.V && e.Control)
                {
                    pasteTagsToolStripMenuItem_Click(copyTagsToolStripMenuItem, new EventArgs());
                }
                else
                {
                    e.SuppressKeyPress = true;
                    txtTag.Focus();

                    SendKeys.Send(e.KeyCode.ToString().ToLower());
                    if ((System.Windows.Forms.Control.IsKeyLocked(Keys.CapsLock) && !e.Shift) || (e.Shift && !Control.IsKeyLocked(Keys.CapsLock)))
                    {
                        txtTag.Text = txtTag.Text.ToUpper();
                    }
                }
            }
            else if (e.KeyCode == Keys.OemMinus)
            {
                e.SuppressKeyPress = true;
                txtTag.Focus();

                if (e.Shift)
                    txtTag.Text += "_";
                else
                    txtTag.Text += "-";

                txtTag.SelectionStart = txtTag.Text.Length;
            }
            else if (e.KeyCode == Keys.Delete)
            {
                deletePhoto();
            }
            else if (e.KeyCode == Keys.F2)
            {
                renamePhoto();
            }
        }

        private void deletePhoto()
        {
            int index = 0;

            if (pictureList.SelectedItems.Count > 1)
            {
                MessageBox.Show("You may only delete one photo at a time.", Application.ProductName);
            }
            else if (photosets == null)
            {
                MessageBox.Show("Please wait until the photosets have been retrieved from flickr.");
            }
            else if (pictureList.SelectedItems.Count == 1)
            {
                if (MessageBox.Show("Are you sure? This will delete the photo on flickr too! (The local will go in the recyle bin. To undo, you will have to restore the file from the recycle bin, reupload it to flickr and add it back to this set.)", "Delete item " + pictureList.SelectedItems[0].Text + " ???", MessageBoxButtons.OKCancel) == DialogResult.OK)
                {
                    if (MessageBox.Show("Are you sure you really want to delete " + pictureList.SelectedItems[0].Text + " ??? This is your last chance to cancel.", Application.ProductName, MessageBoxButtons.OKCancel) == DialogResult.OK)
                    {
                        try
                        {
                            Cursor.Current = Cursors.WaitCursor;

                            //if the whole set is not in flickr, allow the local delete and do 
                            //nothing on the flickr side.
                            if (picturesDictionary.Count > 0)
                            {
                                flickr.PhotosDelete(currentContent.flickrID);
                            }

                            //File.Delete(currentContent.filename);
                            Microsoft.VisualBasic.FileIO.FileSystem.DeleteFile(currentContent.filename, Microsoft.VisualBasic.FileIO.UIOption.OnlyErrorDialogs, Microsoft.VisualBasic.FileIO.RecycleOption.SendToRecycleBin, Microsoft.VisualBasic.FileIO.UICancelOption.DoNothing);

                            //delete the entries for this photo from the allTags and tagReaderTags NVC
                            //pause the tag reader during this operation
                            lock (lock_tagReaderPause)
                            {
                                for (int i = 0; i <= 1; i++)
                                {
                                    NameValueCollection tagCollection;

                                    if (i == 0)
                                        tagCollection = allTags;
                                    else
                                        tagCollection = tagReaderTags;

                                    NameValueCollection changeQueue = new NameValueCollection();

                                    foreach (string tag in tagCollection.AllKeys)
                                    {
                                        foreach (string pictureFileName in tagCollection.GetValues(tag))
                                        {
                                            if (pictureFileName.Equals(currentContent.filename))
                                            {
                                                changeQueue.Add(tag, pictureFileName);
                                            }
                                        }
                                    }

                                    foreach (string tag in changeQueue.AllKeys)
                                    {
                                        foreach (string pictureFileName in changeQueue.GetValues(tag))
                                        {
                                            removeOnlyThisKeyValueCombo(tagCollection, tag, pictureFileName);
                                        }
                                    }
                                }
                            }

                            picturesDictionary.Remove(Path.GetFileNameWithoutExtension(currentContent.filename));

                            index = pictureList.SelectedItems[0].Index;
                            pictureList.Items.Remove(pictureList.SelectedItems[0]);

                            if (index > (pictureList.Items.Count - 1))
                                index = (pictureList.Items.Count - 1);
                        }
                        finally
                        {
                            Cursor.Current = Cursors.Default;

                            if (index >= 0)
                                pictureList.Focus();

                            pictureList.Items[index].Selected = true;
                        }
                    }
                }

            }
        }

        private void appendDirectoriesToTreeNode(TreeNode node, String root)
        {
            DirectoryInfo rootDir = new DirectoryInfo(root);

            foreach (DirectoryInfo subDir in rootDir.GetDirectories())
            {
                if ((subDir.Attributes & FileAttributes.Hidden) != FileAttributes.Hidden)
                {
                    TreeNode subdirNode = new TreeNode(subDir.Name);
                    subdirNode.Name = subDir.FullName;
                    appendDirectoriesToTreeNode(subdirNode, subDir.FullName);

                    if (node == null)
                        setList.Nodes.Add(subdirNode);
                    else
                        node.Nodes.Add(subdirNode);
                }
            }
        }

        private void setList_AfterSelect(object sender, TreeViewEventArgs e)
        {
            selectedNodeName = e.Node.Text;

            populatePictureList(e.Node.Name);

            if (e.Action == TreeViewAction.ByMouse)
            {
                needToSetFocusToPictureList = true;
            }

            btnSetDateTaken.Enabled = false;
            btnSetDateTakenForWholeSet.Enabled = false;
        }

        private void populatePictureList(string folderPath)
        {
            pictureList.Clear();
            pictureList.View = View.List;

            DirectoryInfo subDir = new DirectoryInfo(folderPath);

            foreach (string fileExtension in fileExtensions.Split(';'))
            {
                foreach (FileInfo fileInfo in subDir.GetFiles(fileExtension))
                    pictureList.Items.Add(fileInfo.Name);
            }

            pictureList.Sorting = SortOrder.Ascending;
            pictureList.Sort();

            if (pictureList.Items.Count > 0)
                pictureList.Items[0].Selected = true;
        }

        void pictureList_ItemSelectionChanged(object sender, ListViewItemSelectionChangedEventArgs e)
        {
            unZoomPicture();

            if (!e.IsSelected)
            {
                e.Item.Font = new Font(e.Item.Font, FontStyle.Regular);
                previousPictureIndex = e.Item.Index;
                pictureBox.Tag = "invalid";
                selectedItems.Remove(e.Item.Text);
                lstTags.Clear();
            }
            else if (e.IsSelected)
            {
                if (!selectedItems.Contains(e.Item.Text))
                {
                    selectedItems.Add(e.Item.Text);
                }

                e.Item.Font = new Font(e.Item.Font, FontStyle.Bold);

                if (currentContent != null)
                {
                    int secondsToAdd = 1;

                    if (e.Item.Index == (previousPictureIndex - 1))
                        secondsToAdd = -1;

                    if (currentContent is Picture)
                    {
                        Picture currentPicture = ((Picture)currentContent);

                        if (currentPicture.dateTaken.HasValue)
                            previousDateTaken = currentPicture.dateTaken.Value.AddSeconds(secondsToAdd);

                        if (currentPicture.gpsLatitude.HasValue)
                            previousGpsLatitude = currentPicture.gpsLatitude;

                        if (currentPicture.gpsLongitude.HasValue)
                            previousGpsLongitude = currentPicture.gpsLongitude;
                    }
                    else if (currentContent is Video)
                    {
                        if (currentContent.flickrDateTaken.HasValue)
                            previousDateTaken = currentContent.flickrDateTaken.Value.AddSeconds(secondsToAdd);
                        else
                            previousDateTaken = null;

                        if (currentContent.flickrGpsLatitude.HasValue)
                            previousGpsLatitude = currentContent.flickrGpsLatitude;
                        else
                            previousGpsLatitude = null;

                        if (currentContent.flickrGpsLongitude.HasValue)
                            previousGpsLongitude = currentContent.flickrGpsLongitude;
                        else
                            previousGpsLongitude = null;
                    }
                }

                mergeLocalInUI(e.Item.Text);
            }
        }

        private Boolean isVideo(string fileName)
        {
            if (fileName.ToLower().EndsWith(".avi") || fileName.ToLower().EndsWith(".mov") || fileName.ToLower().EndsWith(".mp4") || fileName.ToLower().EndsWith(".3gp"))
                return true;
            else
                return false;

        }

        private void mergeLocalInUI(string selectedItemText)
        {
            string fullFilePath = Path.Combine(setList.SelectedNode.Name, selectedItemText);

            //reset some UI stuff
            lstTags.Items.Clear();
            btnRemoveTagFromWholeSet.Enabled = false;
            lblFlickrDateTaken.Visible = false;
            txtPictureCaption.Text = string.Empty;
            txtPictureCaption.ForeColor = Color.FromKnownColor(KnownColor.WindowText);
            toolTip1.SetToolTip(txtPictureCaption, "");
            calDateTaken.TitleBackColor = Color.LightGray;
            string geoTag = "Geotag: ";
            pictureBox.Tag = null;
            lblGeotag.Text = geoTag;
            lblGeotag.Font = new Font(lblGeotag.Font, FontStyle.Regular);
            lblVisibility.Text = "";

            lnkPicture.Visible = false;
            lnkPicture.LinkVisited = false;
            lnkPicture.Links.Clear();

            lnkSet.Visible = false;
            lnkSet.LinkVisited = false;
            lnkSet.Links.Clear();

            if (isVideo(selectedItemText))
            {
                currentContent = new Video(fullFilePath);

                //just use the file created date as the date taken.
                calDateTaken.SetDate(File.GetCreationTime(fullFilePath));

                //load video
                axWMP.URL = fullFilePath;

                axWMP.Visible = true;
                pnlPictureBox.Visible = false;
            }
            else
            {
                axWMP.Ctlcontrols.stop();
                axWMP.URL = null;

                currentContent = new Picture(fullFilePath);
                Picture currentPicture = ((Picture)currentContent);

                loadFlickrPhotoId();

                //load picture
                pictureBox.ImageLocation = currentPicture.filename;

                axWMP.Visible = false;
                pnlPictureBox.Visible = true;

                //load caption
                txtPictureCaption.Text = currentPicture.caption;

                //load date taken
                if (currentPicture.dateTaken.HasValue)
                {
                    calDateTaken.TitleBackColor = Color.LightBlue;
                    calDateTaken.SetDate(currentPicture.dateTaken.Value);
                }
                else if (previousDateTaken.HasValue)
                {
                    calDateTaken.SetDate(previousDateTaken.Value);
                }

                //load tags
                foreach (String tag in currentPicture.tags)
                {
                    if (tag.Length > 0)
                        lstTags.Items.Add(tag);
                }

                //load gps data
                if (currentPicture.gpsLatitude.HasValue)
                    geoTag += "lat " + currentPicture.gpsLatitude.ToString() + ", ";
                if (currentPicture.gpsLongitude.HasValue)
                    geoTag += "long " + currentPicture.gpsLongitude.ToString();
                lblGeotag.Text = geoTag;
            }
        }

        private void flickrGopher_DoWork(object sender, DoWorkEventArgs e)
        {
            int j = 0;
            while (photosets == null && j <= MAX_RETRIES)
            {
                try
                {
                    photosets = flickr.PhotosetsGetList(flickr.PeopleFindByUsername(flickrUserName).UserId);
                }
                catch { }
                j++;
            }

            if (photosets != null && !currentSetName.Equals(selectedNodeName))
            {
                picturesDictionary.Clear();
                currentSetName = "";
                currentSetId = "";
                //currentContent = null;

                for (int i = 0; i < photosets.PhotosetCollection.Length; i++)
                {
                    if (photosets.PhotosetCollection[i].Title.Equals(selectedNodeName))
                    {
                        currentSetId = photosets.PhotosetCollection[i].PhotosetId;
                        currentSetName = selectedNodeName;

                        Photo[] photosThisSet = flickr.PhotosetsGetPhotos(currentSetId).PhotoCollection;
                        for (int k = 0; k < photosThisSet.Length; k++)
                        {
                            //the "if" statement is to prevent an error if a picture is in a photoset more than once.
                            //if (!picturesDictionary.ContainsKey(photosThisSet[k].Title))
                            picturesDictionary.Add(photosThisSet[k].Title, photosThisSet[k].PhotoId);
                        }
                        break;
                    }
                }
            }

            if (currentContent != null && !currentContent.flickrLoaded && currentContent.flickrID != null)
            {
                PhotoInfo photoInfo = flickr.PhotosGetInfo(currentContent.flickrID);

                if (photoInfo.PhotoId == currentContent.flickrID)
                {
                    currentContent.loadFlickrInfo(photoInfo);
                }
            }
        }

        private void Main_FormClosing(object sender, FormClosingEventArgs e)
        {
            //saveAllTagsToDisk(); //not needed here. It's maintained on each tag add/delete.
            tagReader.CancelAsync();
        }

        private void loadAuthToken()
        {
            string authToken = "";
            RegistryKey key = Registry.CurrentUser.OpenSubKey(Settings.Default.registryPath, true);

            if (key == null)
                key = Registry.CurrentUser.CreateSubKey(Settings.Default.registryPath, RegistryKeyPermissionCheck.ReadWriteSubTree);
            else
                authToken = key.GetValue("authToken", "").ToString();

            if (authToken != "")
            {
                flickr.AuthToken = authToken;
            }
            else
            {
                string frob = flickr.AuthGetFrob();
                string url = flickr.AuthCalcUrl(frob, AuthLevel.Delete);

                if (MessageBox.Show("Authorization needed.", Application.ProductName, MessageBoxButtons.OKCancel) != DialogResult.OK)
                {
                    Environment.Exit(0);
                }

                System.Diagnostics.Process.Start(url);

                if (MessageBox.Show("Click ok when authorization is complete.", Application.ProductName, MessageBoxButtons.OKCancel) != DialogResult.OK)
                {
                    Environment.Exit(0);
                }

                flickr.AuthToken = flickr.AuthGetToken(frob).Token;
                key.SetValue("authToken", flickr.AuthToken);
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (this.Visible == true)
            {
                if (needToSetFocusToPictureList)
                {
                    pictureList.Focus();
                    needToSetFocusToPictureList = false;
                }

                if (tagReader.IsBusy)
                {
                    if (numberOfFiles > 0 && tagReadingProgressBar.Visible == false)
                    {
                        tagReadingProgressBar.Visible = true;
                        tagReadingProgressBar.Maximum = numberOfFiles;
                    }

                    while (newlyScannedTags.Count > 0)
                    {
                        ListViewItem item = new ListViewItem(newlyScannedTags[0]);
                        item.Name = newlyScannedTags[0];
                        lstAllTags.Items.Add(item);
                        item.EnsureVisible();

                        newlyScannedTags.RemoveAt(0);
                    }

                    if (tagReadingProgress > 0)
                        tagReadingProgressBar.Value = tagReadingProgress;
                }
                else if (tagReadingProgressBar.Value > 95 && tagReadingProgress == 0)
                {
                    tagReaderComplete();
                }

                loadFlickrPhotoId();

                if (flickrGopher.IsBusy)
                {
                    flickrProgressBar.Style = ProgressBarStyle.Marquee;
                }
                else
                {
                    flickrProgressBar.Style = ProgressBarStyle.Blocks;

                    if ((currentSetName != selectedNodeName) || (!currentContent.flickrLoaded && currentContent.flickrID != null))
                    {
                        flickrGopher.RunWorkerAsync();
                    }

                    if (currentSetName.Equals(selectedNodeName) || currentSetName == "")
                    {
                        if (btnSetDateTaken.Enabled == false)
                        {
                            btnSetDateTaken.Enabled = true;
                            btnSetDateTakenForWholeSet.Enabled = true;
                        }
                    }
                }

                if (currentContent != null && !currentContent.flickrMergedInUI && currentContent.flickrLoaded)
                {
                    mergeFlickrInUI();
                }

                if (pictureList.SelectedItems.Count >= 1)
                {
                    if (pictureList.FocusedItem != null && pictureList.FocusedItem.Selected && !pictureList.FocusedItem.Text.ToLower().Equals(Path.GetFileName(currentContent.filename).ToLower()))
                    {
                        mergeLocalInUI(pictureList.FocusedItem.Text);
                    }
                    else if (pictureBox.Tag != null && pictureBox.Tag.ToString().Equals("invalid"))
                    {
                        if (pictureList.FocusedItem.Selected)
                            mergeLocalInUI(pictureList.FocusedItem.Text);
                        else
                            mergeLocalInUI(selectedItems[selectedItems.Count - 1]);
                    }
                }
            }
        }

        private void tagReaderComplete()
        {
            tagReadingProgressBar.Value = 0;
            tagReadingProgressBar.Visible = false;

            populateAllTagsListView();

            for (int i = 0; i < setList.Nodes.Count; i++)
            {
                TreeNode node = setList.Nodes[i];
                node.NodeFont = new Font(setList.Font.FontFamily, setList.Font.Size, FontStyle.Regular);
                node.ToolTipText = "";
            }

            foreach (string setPath in setsWithIssues.AllKeys)
            {
                string issuesThisSet = "";
                foreach (string issue in setsWithIssues.GetValues(setPath))
                {
                    issuesThisSet += issue + "\r\n";
                }
                TreeNode[] nodes = setList.Nodes.Find(setPath, true);
                if (nodes.Length > 0)
                {
                    TreeNode node = nodes[0];
                    node.NodeFont = new Font(setList.Font.FontFamily, setList.Font.Size, FontStyle.Bold);
                    node.ToolTipText = issuesThisSet;
                }
            }
        }

        private void populateAllTagsListView()
        {
            lstAllTags.Clear();
            foreach (string tag in allTags)
            {
                lstAllTags.Items.Add(tag, tag, 0);
            }
        }

        private void mergeFlickrInUI()
        {
            lnkPicture.Links.Add(0, 7, (string.Format("http://www.flickr.com/photos/{0}/{1}/in/set-{2}/", flickrUserName, currentContent.flickrID, currentSetId)));
            lnkPicture.Visible = true;

            lnkSet.Links.Add(0, 3, (string.Format("http://www.flickr.com/photos/{0}/sets/{1}/", flickrUserName, currentSetId)));
            lnkSet.Visible = true;

            if (currentContent.flickrIsPublic == 1)
            {
                lblVisibility.Text = "Public";
                lblVisibility.Font = new Font(lblVisibility.Font, FontStyle.Bold);
            }
            else
            {
                lblVisibility.Text = "Private";
                lblVisibility.Font = new Font(lblVisibility.Font, FontStyle.Regular);
            }

            if (currentContent is Picture)
            {
                Picture currentPicture = ((Picture)currentContent);

                bool localUnsavedChanges = false;

                //caption
                if (currentPicture.caption != null && currentPicture.flickrCaption != null)
                {
                    if (currentPicture.caption != currentPicture.flickrCaption)
                    {
                        txtPictureCaption.ForeColor = Color.Red;
                        toolTip1.SetToolTip(txtPictureCaption, currentPicture.flickrCaption);
                    }
                }
                else if (currentPicture.caption == null && currentPicture.flickrCaption != null)
                {
                    currentPicture.caption = currentPicture.flickrCaption;
                    localUnsavedChanges = true;
                }
                else if (currentPicture.caption != null && currentPicture.flickrCaption == null)
                {
                    currentPicture.flickrCaption = currentPicture.caption;
                    flickr.PhotosSetMeta(currentPicture.flickrID, currentPicture.flickrTitle, currentPicture.flickrCaption);
                }

                //date taken
                if (currentPicture.dateTaken.HasValue && currentPicture.flickrDateTaken.HasValue)
                {
                    if (currentPicture.dateTaken.Value != currentPicture.flickrDateTaken.Value)
                    {
                        calDateTaken.TitleBackColor = Color.Red;
                        lblFlickrDateTaken.Text = "F conflict " + currentPicture.flickrDateTaken.Value.ToString("M/d/yy");
                        lblFlickrDateTaken.Visible = true;
                    }
                    else
                    {
                        calDateTaken.TitleBackColor = Color.FromKnownColor(KnownColor.ActiveCaption);
                    }
                }
                else if (currentPicture.dateTaken.HasValue)
                {
                    currentPicture.flickrDateTaken = currentPicture.dateTaken;
                    flickr.PhotosSetDates(currentPicture.flickrID, currentPicture.flickrDateTaken.Value, DateGranularity.FullDate);
                }
                else if (currentPicture.flickrDateTaken.HasValue)
                {
                    calDateTaken.TitleBackColor = Color.Red;
                    lblFlickrDateTaken.Text = "F only " + currentPicture.flickrDateTaken.Value.ToShortDateString();
                    lblFlickrDateTaken.Visible = true;

                    //can't do this because the flickr date taken value is set to the date uploaded if none existed
                    //in the exif when the picture was originally uploaded.
                    //currentPicture.dateTaken = currentPicture.flickrDateTaken;
                    //localUnsavedChanges = true;
                }

                //gps data
                if ((currentPicture.gpsLatitude.HasValue && currentPicture.flickrGpsLatitude.HasValue) || (currentPicture.gpsLongitude.HasValue && currentPicture.flickrGpsLongitude.HasValue))
                {
                    if ((currentPicture.gpsLatitude.HasValue && currentPicture.flickrGpsLatitude.HasValue && (Math.Abs(Math.Round(currentPicture.gpsLatitude.Value, 1) - Math.Round(currentPicture.flickrGpsLatitude.Value, 1)) > 0.1)) ||
                    (currentPicture.gpsLongitude.HasValue && currentPicture.flickrGpsLongitude.HasValue && (Math.Abs(Math.Round(currentPicture.gpsLongitude.Value, 1) - Math.Round(currentPicture.flickrGpsLongitude.Value, 1)) > 0.1)))
                    {
                        lblGeotag.Text += "  CONFLICT w/flickr " + currentPicture.flickrGpsLatitude.Value + ", " + currentPicture.flickrGpsLongitude.Value;
                    }
                    else
                    {
                        lblGeotag.Font = new Font(lblGeotag.Font, FontStyle.Bold);
                    }
                }
                else if (currentPicture.gpsLatitude.HasValue && currentPicture.gpsLongitude.HasValue)
                {
                    currentPicture.flickrGpsLatitude = currentPicture.gpsLatitude;
                    currentPicture.flickrGpsLongitude = currentPicture.gpsLongitude;
                    flickr.PhotosGeoSetLocation(currentPicture.flickrID, currentPicture.flickrGpsLatitude.Value, currentPicture.flickrGpsLongitude.Value);
                    lblGeotag.Font = new Font(lblGeotag.Font, FontStyle.Bold);
                }
                else if (currentPicture.flickrGpsLatitude.HasValue && currentPicture.flickrGpsLongitude.HasValue)
                {
                    currentPicture.gpsLatitude = currentPicture.flickrGpsLatitude;
                    currentPicture.gpsLongitude = currentPicture.flickrGpsLongitude;
                    localUnsavedChanges = true;
                }

                //tags (merge)
                foreach (string tag in currentPicture.flickrTags)
                {
                    if (!currentPicture.tags.Contains(tag))
                    {
                        //assume that the tag is already compliant with the capitalization
                        string copyOfTag = tag;
                        if (!containsTag(lstAllTags, ref copyOfTag))
                        {
                            if (tag.Equals(copyOfTag))
                                lstAllTags.Items.Add(tag, tag, 0);
                        }
                        if (!tag.Equals(copyOfTag))
                        {
                            //throw new Exception("Flickr capitalization doesn't match local capitalization");
                            currentPicture.flickrMergedInUI = true; //to avoid a new message box every second
                            MessageBox.Show(string.Format("Flickr capitalization for tag \"{0}\" doesn't match local capitalization. Tags not merged.", tag));

                            ListViewItem item = new ListViewItem(tag);
                            item.Font = new Font(item.Font, FontStyle.Italic);
                            item.ForeColor = Color.Red;
                            lstTags.Items.Add(item);
                        }
                        else
                        {
                            //update tagreader tags and alltags
                            addTagIfNotAlreadyThere(tag, currentPicture, allTags);
                            if (tagReader.IsBusy)
                            {
                                addTagIfNotAlreadyThere(tag, currentPicture, tagReaderTags);
                            }

                            currentPicture.tags.Add(tag);
                            localUnsavedChanges = true;

                            ListViewItem item = new ListViewItem(tag);
                            item.ForeColor = Color.Cyan;
                            lstTags.Items.Add(item);
                        }
                    }
                }

                bool saveFlickrTags = false;
                foreach (ListViewItem item in lstTags.Items)
                {
                    if (currentPicture.flickrTags.Contains(item.Text))
                    {
                        if (item.Font.Italic == false)
                            item.ForeColor = Color.Blue;
                        else
                            item.Font = new Font(item.Font, FontStyle.Regular);
                    }
                    else
                    {
                        //if (item.Text != "??????????")
                        //{
                        item.ForeColor = Color.Red;
                        currentPicture.flickrTags.Add(item.Text);
                        saveFlickrTags = true;
                        //}
                        //else
                        //{
                        //    MessageBox.Show("Houston we have a problem.");
                        //}
                    }
                }

                if (saveFlickrTags)
                {
                    setFlickrTags(currentPicture);
                }

                if (localUnsavedChanges)
                {
                    currentPicture.Save();
                    lblGeotag.Font = new Font(lblGeotag.Font, FontStyle.Bold);
                    saveAllTagsToDisk(allTags);
                    populateAllTagsListView();
                }

                if (saveFlickrTags || localUnsavedChanges)
                {
                    if (pictureList.SelectedItems.Count > 0)
                        mergeLocalInUI(pictureList.SelectedItems[0].Text);
                }
                else
                    currentPicture.flickrMergedInUI = true;

            } //if (currentContent is Picture)
            else //Video
            {
                //date taken
                if (currentContent.flickrDateTaken.HasValue)
                {
                    calDateTaken.SetDate(currentContent.flickrDateTaken.Value);
                    calDateTaken.TitleBackColor = Color.FromKnownColor(KnownColor.ActiveCaption);
                }

                //tags
                lstTags.Clear();
                bool needToRefreshAllTags = false;
                foreach (string tag in currentContent.flickrTags)
                {
                    ListViewItem item = new ListViewItem(tag);
                    item.ForeColor = Color.Blue;
                    lstTags.Items.Add(item);

                    //make sure tags "databases" are up to date
                    if (addTagIfNotAlreadyThere(tag, currentContent, allTags))
                    {
                        needToRefreshAllTags = true;
                    }

                    if (tagReader.IsBusy)
                    {
                        addTagIfNotAlreadyThere(tag, currentContent, tagReaderTags);
                    }
                } //foreach (string tag in currentPicture.flickrTags)     

                if (needToRefreshAllTags)
                {
                    saveAllTagsToDisk(allTags);
                    populateAllTagsListView();
                }

                //load gps data
                string geoTag = "Geotag: ";
                if (currentContent.flickrGpsLatitude.HasValue)
                    geoTag += "lat " + currentContent.flickrGpsLatitude.ToString() + ", ";
                if (currentContent.flickrGpsLongitude.HasValue)
                    geoTag += "long " + currentContent.flickrGpsLongitude.ToString();
                lblGeotag.Text = geoTag;
                lblGeotag.Font = new Font(lblGeotag.Font, FontStyle.Bold);

                currentContent.flickrMergedInUI = true;
            }
        }

        private void setFlickrTags(Content picture)
        {
            string[] tmpArray = new string[picture.flickrTags.Count];
            picture.flickrTags.CopyTo(tmpArray, 0);

            for (int i = 0; i < tmpArray.Length; i++)
            {
                if (tmpArray[i].IndexOf(" ") > -1)
                {
                    tmpArray[i] = "\"" + tmpArray[i] + "\"";
                }
            }

            flickr.PhotosSetTags(picture.flickrID, tmpArray);
        }

        private void loadFlickrPhotoId()
        {
            if (currentSetName == selectedNodeName && currentContent != null && currentContent.flickrID == null)
            {
                if (picturesDictionary.ContainsKey(Path.GetFileNameWithoutExtension(currentContent.filename)))
                {
                    currentContent.flickrID = picturesDictionary[Path.GetFileNameWithoutExtension(currentContent.filename)];
                }
            }
        }

        private string getFolderName(string fullPath)
        {
            DirectoryInfo directoryInfo = new DirectoryInfo(Path.GetDirectoryName(fullPath));
            return directoryInfo.Name;
        }

        private void tagReader_DoWork(object sender, DoWorkEventArgs e)
        {
            StringCollection files = new StringCollection();
            foreach (string fileExtension in fileExtensions.Split(';'))
            {
                foreach (string filePath in Directory.GetFiles(localFlickrDirectory, fileExtension, SearchOption.AllDirectories))
                {
                    files.Add(filePath);
                }
            }

            numberOfFiles = files.Count;
            setsWithIssues.Clear();

            foreach (string fileName in files)
            {
                if (tagReader.CancellationPending)
                    return;

                //ignore hidden folders
                DirectoryInfo directoryInfo = new DirectoryInfo(Path.GetDirectoryName(fileName));

                if (File.Exists(fileName) && (directoryInfo.Attributes & FileAttributes.Hidden) != FileAttributes.Hidden)
                {
                    lock (lock_tagReaderPause)
                    {
                        Content content = null;
                        string contentTitle = Path.GetFileName(fileName);
                        string setName = getFolderName(fileName);

                        int failCount = 0;
                        while (photosets == null)
                        {
                            if (failCount >= (60000 * 4)) //allow 4 minutes
                                throw new Exception("Flickr photosets not loaded. Timeout exceeded. Flickr photosets are required to background process flickr tags.");

                            Thread.Sleep(1000);
                            failCount++;
                        }

                        if (isVideo(fileName))
                        {
                            for (int i = 0; i < photosets.PhotosetCollection.Length; i++)
                            {
                                if (photosets.PhotosetCollection[i].Title.Equals(setName))
                                {
                                    Photo[] photosThisSet = flickr.PhotosetsGetPhotos(photosets.PhotosetCollection[i].PhotosetId).PhotoCollection;
                                    for (int k = 0; k < photosThisSet.Length; k++)
                                    {
                                        if (photosThisSet[k].Title.Equals(Path.GetFileNameWithoutExtension(contentTitle)))
                                        {
                                            PhotoInfo photoInfo = flickr.PhotosGetInfo(photosThisSet[k].PhotoId);
                                            if (isVideo(fileName))
                                            {
                                                content = new Video(fileName);
                                            }
                                            else
                                            {
                                                content = new Picture(fileName);
                                            }
                                            content.loadFlickrInfo(photoInfo);
                                            break;
                                        }
                                    }
                                    break;
                                }
                            }
                        }
                        else
                        {
                            content = new Picture(fileName);
                        }

                        if (content == null)
                        {
                            setsWithIssues.Add(Path.GetDirectoryName(fileName), string.Format("{0} apparently doesn't exist in flickr.", contentTitle));
                        }
                        else if (isVideo(fileName))
                        {
                            foreach (String tag in content.flickrTags)
                            {
                                if (tagReaderTags.GetValues(tag) == null)
                                {
                                    newlyScannedTags.Add(tag);
                                }

                                addTagIfNotAlreadyThere(tag, content, tagReaderTags);
                            }
                        }
                        else
                        {
                            Picture picture = (Picture)content;

                            foreach (String tag in picture.tags)
                            {
                                if (tag.Length > 0)
                                {
                                    if (tagReaderTags.GetValues(tag) == null)
                                    {
                                        newlyScannedTags.Add(tag);
                                    }

                                    // if not already in tagreader tags (might have been added on a txtTag_KeyDown by the user)
                                    addTagIfNotAlreadyThere(tag, picture, tagReaderTags);
                                }
                                else
                                {
                                    setsWithIssues.Add(Path.GetDirectoryName(fileName), string.Format("{0} has a zero-length tag.", contentTitle));
                                }
                            }
                            if (!picture.dateTaken.HasValue)
                                setsWithIssues.Add(Path.GetDirectoryName(fileName), string.Format("{0} has no date taken.", contentTitle));
                        }
                    }
                }

                tagReadingProgress += 1;
            }

            tagReadingProgress = 0;

            saveAllTagsToDisk(tagReaderTags);

            //saved lastAllTagsScanDate to the registry
            RegistryKey key = Registry.CurrentUser.OpenSubKey(Settings.Default.registryPath, true);
            if (key == null)
            {
                key = Registry.CurrentUser.CreateSubKey(Settings.Default.registryPath, RegistryKeyPermissionCheck.ReadWriteSubTree);
            }

            lastAllTagsScanDate = DateTime.Now.ToLongDateString();
            key.SetValue("lastAllTagsScanDate", lastAllTagsScanDate);
        }

        private void saveAllTagsToDisk(NameValueCollection tagsToSave)
        {
            lock (lock_AllTagsSerialization)
            {
                //sort the new NameValueCollection--------------------------------------------
                string[] array = new string[tagsToSave.AllKeys.Length];
                tagsToSave.AllKeys.CopyTo(array, 0);
                Array.Sort(array, new CaseInsensitiveComparer());

                NameValueCollection tmpNVC = new NameValueCollection(tagsToSave.Count);
                for (int i = 0; i < array.Length; i++)
                {
                    foreach (string value in tagsToSave.GetValues(array[i]))
                    {
                        tmpNVC.Add(array[i], value);
                    }
                }

                //now reassign allTags to be equal to the new list
                allTags = tmpNVC;
                //end sort--------------------------------------------------------------------

                //write it to a file
                using (FileStream fileStream = new FileStream(Path.Combine(appDataFolder, "tags.xml"), FileMode.Create))
                {
                    SoapFormatter soapFormatter = new SoapFormatter();
                    soapFormatter.Serialize(fileStream, allTags);
                }
            }
        }

        private void btnSetDateTaken_Click(object sender, EventArgs e)
        {
            if ((currentContent is Picture) || (currentContent is Video && currentContent.flickrLoaded))
            {
                if (MessageBox.Show("Are you sure?", Application.ProductName, MessageBoxButtons.OKCancel) == DialogResult.OK)
                {
                    Cursor.Current = Cursors.WaitCursor;
                    try
                    {
                        if (currentContent is Picture)
                        {
                            ((Picture)currentContent).dateTaken = calDateTaken.SelectionStart;
                            ((Picture)currentContent).Save();
                        }

                        if (currentContent.flickrLoaded)
                        {
                            flickr.PhotosSetDates(currentContent.flickrID, calDateTaken.SelectionStart, DateGranularity.FullDate);
                        }
                    }
                    finally
                    {
                        if (pictureList.SelectedItems.Count > 0)
                        {
                            mergeLocalInUI(pictureList.SelectedItems[0].Text);
                        }
                        Cursor.Current = Cursors.Default;
                        pictureList.Focus();
                    }
                }
            }
        }

        private void btnSetDateTakenForWholeSet_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show(string.Format("Are you sure you want to set the date for all items to {0:F}? This will take place immediately on flickr too.", dtpDateTaken.Value), Application.ProductName, MessageBoxButtons.OKCancel) == DialogResult.OK)
            {
                if (MessageBox.Show("Are you sure? This is for the WHOLE SET!", Application.ProductName, MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button2) == DialogResult.OK)
                {
                    try
                    {
                        Cursor.Current = Cursors.WaitCursor;
                        setDateTakenForAllItems(pictureList.Items);
                    }
                    finally
                    {
                        mergeLocalInUI(pictureList.SelectedItems[0].Text);
                        Cursor.Current = Cursors.Default;
                        pictureList.Focus();
                    }
                }
            }
        }

        private void setDateTakenForAllItems(ICollection items)
        {
            foreach (ListViewItem item in items)
            {
                Picture picture = new Picture(Path.Combine(setList.SelectedNode.Name, item.Text));
                picture.dateTaken = calDateTaken.SelectionStart;
                picture.Save();

                picture.flickrID = picturesDictionary[Path.GetFileNameWithoutExtension(picture.filename)];
                if (picture.flickrID != null)
                {
                    flickr.PhotosSetDates(picture.flickrID, calDateTaken.SelectionStart, DateGranularity.FullDate);
                }
            }
        }

        private void btnAddTagToWholeSet_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show(string.Format("Are you sure you want to add the tag \"{0}\" to the whole set?", txtTag.Text), Application.ProductName, MessageBoxButtons.OKCancel) == DialogResult.OK)
            {
                try
                {
                    Cursor.Current = Cursors.WaitCursor;
                    pictureList.Select();

                    string tag = txtTag.Text;
                    addTagToAllItems(tag, pictureList.Items);
                    txtTag.Clear();
                }
                finally
                {
                    Cursor.Current = Cursors.Default;
                }
            }
        }

        void addTagToSelectedMenuItem_Click(object sender, EventArgs e)
        {
            if (pictureList.SelectedItems.Count > 0)
            {
                string tag = InputBox("Enter the tag you want to add:", Application.ProductName, "");

                if (tag.Length > 0)
                {
                    try
                    {
                        Cursor.Current = Cursors.WaitCursor;

                        addTagToAllItems(tag, pictureList.SelectedItems);
                    }
                    finally
                    {
                        Cursor.Current = Cursors.Default;
                    }
                }
            }
        }

        private void addTagToAllItems(string tag, ICollection items)
        {
            if (tag.ToLower().Equals("me"))
                tag = "myself";

            if (!containsTag(lstAllTags, ref tag))//correct the case
            {
                lstAllTags.Items.Add(tag, tag, 0);
            }
            foreach (ListViewItem item in items)
            {
                string fullFilePath = Path.Combine(setList.SelectedNode.Name, item.Text);

                Content content;
                if (isVideo(item.Text))
                    content = new Video(fullFilePath);
                else
                    content = new Picture(fullFilePath);

                //local BEGIN---------------------------------------------------------------------- 
                if (content is Picture)
                {
                    Picture picture = ((Picture)content);

                    if (!picture.tags.Contains(tag))
                    {
                        picture.tags.Add(tag);
                        picture.Save();
                    }
                }
                //local END-------------------------------------------------------------------------

                //flickr BEGIN----------------------------------------------------------------------
                content.flickrID = picturesDictionary[Path.GetFileNameWithoutExtension(content.filename)];

                if (picturesDictionary.Count == 0)
                {
                    //this whole set is not in flickr. Do nothing. Just ignore flickr.
                }
                else if (content.flickrID == null)
                {
                    //the set is in flickr, but this particular picture isn't. Throw an exception.
                    throw new Exception("Set is in flickr, but picture isn't?");
                }
                else
                {
                    PhotoInfo photoInfo = flickr.PhotosGetInfo(content.flickrID);

                    content.flickrTags = new StringCollection();

                    for (int i = 0; i < photoInfo.Tags.TagCollection.Length; i++)
                    {
                        content.flickrTags.Add(photoInfo.Tags.TagCollection[i].Raw);
                    }

                    if (!content.flickrTags.Contains(tag))
                    {
                        content.flickrTags.Add(tag);
                        setFlickrTags(content);
                    }
                }
                //flickr END-------------------------------------------------------------------------

                //make sure tags "databases" are up to date
                addTagIfNotAlreadyThere(tag, content, allTags);
                if (tagReader.IsBusy)
                {
                    addTagIfNotAlreadyThere(tag, content, tagReaderTags);
                }

                saveAllTagsToDisk(allTags);
                populateAllTagsListView();

                string fileName;

                if (pictureList.SelectedItems.Count == 1)
                    fileName = pictureList.SelectedItems[0].Text;
                else if (pictureList.FocusedItem != null)
                    fileName = pictureList.FocusedItem.Text;
                else
                    fileName = pictureList.Items[0].Text;

                mergeLocalInUI(fileName);
                if (currentContent.flickrLoaded)
                {
                    mergeFlickrInUI();
                }
            }
        }

        private void btnRemoveTagFromWholeSet_Click(object sender, EventArgs e)
        {
            string tag = lstTags.SelectedItems[0].Text;

            if (MessageBox.Show(string.Format("Are you sure you want to remove the tag \"{0}\" from every picture in the set?\r\n\r\n(Note: Simply use the delete key to remove a tag from just the selected picture.)", tag), Application.ProductName, MessageBoxButtons.OKCancel) == DialogResult.OK)
            {
                try
                {
                    Cursor.Current = Cursors.WaitCursor;
                    pictureList.Select();
                    removeTagFromAllItems(tag, pictureList.Items);
                }
                finally
                {
                    Cursor.Current = Cursors.Default;
                }
            }
        }

        void removeTagFromSelectedMenuItem_Click(object sender, EventArgs e)
        {
            if (pictureList.SelectedItems.Count > 0)
            {
                string tag = InputBox("Enter the tag you want to remove from the selected photos:", Application.ProductName, "");

                if (tag.Length > 0)
                {
                    try
                    {
                        Cursor.Current = Cursors.WaitCursor;

                        removeTagFromAllItems(tag, pictureList.SelectedItems);
                    }
                    finally
                    {
                        Cursor.Current = Cursors.Default;
                    }
                }
            }
        }

        private void changeVisibility(int isPublic, ICollection items)
        {
            foreach (ListViewItem item in items)
            {
                string fullFilePath = Path.Combine(setList.SelectedNode.Name, item.Text);

                Content content;
                if (isVideo(item.Text))
                    content = new Video(fullFilePath);
                else
                    content = new Picture(fullFilePath);

                content.flickrID = picturesDictionary[Path.GetFileNameWithoutExtension(content.filename)];

                if (picturesDictionary.Count == 0)
                {
                    //this whole set is not in flickr. Do nothing. Just ignore flickr.
                }
                else if (content.flickrID == null)
                {
                    //the set is in flickr, but this particular picture isn't. Throw an exception.
                    throw new Exception("Set is in flickr, but picture isn't?");
                }
                else
                {
                    PhotoInfo photoInfo = flickr.PhotosGetInfo(content.flickrID);

                    flickr.PhotosSetPerms(content.flickrID, isPublic, photoInfo.Visibility.IsFriend, photoInfo.Visibility.IsFamily, photoInfo.Permissions.PermissionComment, photoInfo.Permissions.PermissionAddMeta);
                }
                //flickr END-------------------------------------------------------------------------

                if (pictureList.SelectedItems.Count > 0)
                {
                    mergeLocalInUI(pictureList.SelectedItems[0].Text);

                    if (currentContent.flickrLoaded)
                    {
                        mergeFlickrInUI();
                    }
                }
            }
        }

        private void removeTagFromAllItems(string tag, ICollection items)
        {
            foreach (ListViewItem item in items)
            {
                string fullFilePath = Path.Combine(setList.SelectedNode.Name, item.Text);

                Content content;
                if (isVideo(item.Text))
                    content = new Video(fullFilePath);
                else
                    content = new Picture(fullFilePath);

                //local BEGIN---------------------------------------------------------------------- 
                if (content is Picture)
                {
                    Picture picture = ((Picture)content);

                    if (picture.tags.Contains(tag))
                    {
                        picture.tags.Remove(tag);
                        picture.Save();
                    }
                }

                removeTagIfThere(tag, content, allTags);
                if (tagReader.IsBusy)
                    removeTagIfThere(tag, content, tagReaderTags);

                removeTagFromLstAllTagsIfNeeded(tag);

                //local END-------------------------------------------------------------------------

                //flickr BEGIN----------------------------------------------------------------------
                content.flickrID = picturesDictionary[Path.GetFileNameWithoutExtension(content.filename)];

                if (picturesDictionary.Count == 0)
                {
                    //this whole set is not in flickr. Do nothing. Just ignore flickr.
                }
                else if (content.flickrID == null)
                {
                    //the set is in flickr, but this particular picture isn't. Throw an exception.
                    throw new Exception("Set is in flickr, but picture isn't?");
                }
                else
                {
                    PhotoInfo photoInfo = flickr.PhotosGetInfo(content.flickrID);

                    content.flickrTags = new StringCollection();

                    for (int i = 0; i < photoInfo.Tags.TagCollection.Length; i++)
                    {
                        if (!photoInfo.Tags.TagCollection[i].Raw.ToLower().Equals(tag.ToLower()))
                            content.flickrTags.Add(photoInfo.Tags.TagCollection[i].Raw);
                    }

                    setFlickrTags(content);
                }
                //flickr END-------------------------------------------------------------------------

                mergeLocalInUI(pictureList.SelectedItems[0].Text);
                if (currentContent.flickrLoaded)
                {
                    mergeFlickrInUI();
                }
            }
        }

        public static string InputBox(string prompt, string title, string defaultValue)
        {
            InputBoxDialog inputBox = new InputBoxDialog();
            inputBox.FormPrompt = prompt;
            inputBox.FormCaption = title;
            inputBox.DefaultValue = defaultValue;
            inputBox.ShowDialog();
            string response = inputBox.InputResponse;
            inputBox.Close();
            return response;
        }

        private void lblDateTaken_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Set the flickr date taken equal to the previously viewed content's date taken?", Application.ProductName, MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                currentContent.flickrDateTaken = previousDateTaken;
                flickr.PhotosSetDates(currentContent.flickrID, currentContent.flickrDatePosted.Value, currentContent.flickrDateTaken.Value, DateGranularity.FullDate);
                mergeFlickrInUI();
            }
        }

        private void lnkPicture_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            lnkPicture.LinkVisited = true;
            try
            {
                System.Diagnostics.Process.Start(lnkPicture.Links[0].LinkData.ToString());
            }
            catch { }
            pictureList.Focus();
        }

        private void lnkSet_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            lnkSet.LinkVisited = true;
            try
            {
                System.Diagnostics.Process.Start(lnkSet.Links[0].LinkData.ToString());
            }
            catch { }
            pictureList.Focus();
        }

        private void pictureBox_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                // we know the mouse is down - has it the mouse moved enough that we should
                // consider it a drag?
                System.Drawing.Size dragBoxSize = SystemInformation.DragSize;

                if (pictureBox.SizeMode == PictureBoxSizeMode.Zoom)
                {
                    if (pictureBox.Image.Size.Height > pictureBox.Size.Height || pictureBox.Image.Size.Width > pictureBox.Size.Width)
                    {
                        zoomPicture();
                        xDrag = e.X;
                        yDrag = e.Y;
                    }
                }
                else
                {
                    xDrag = e.X + pictureBox.Location.X;
                    yDrag = e.Y + pictureBox.Location.Y;
                }

                if (pictureBox.SizeMode == PictureBoxSizeMode.CenterImage)
                {
                    xPictureBoxOriginal = pictureBox.Location.X;
                    yPictureBoxOriginal = pictureBox.Location.Y;

                    this.DoDragDrop("zoomer", DragDropEffects.Move);
                }
            }
        }

        private void zoomPicture()
        {
            pictureBox.SizeMode = PictureBoxSizeMode.CenterImage;
            pictureBox.Size = pictureBox.Image.Size;

            //this code centers the picture box
            int x = (pictureBox.Image.Size.Width - pnlPictureBox.Size.Width) / 2;
            int y = (pictureBox.Image.Size.Height - pnlPictureBox.Size.Height) / 2;

            if (x < 0)
                x = 0;
            else
                x *= -1;

            if (y < 0)
                y = 0;
            else
                y *= -1;

            pictureBox.Location = new Point(x, y);
        }

        void pictureBox_MouseDown(object sender, MouseEventArgs e)
        {
            mouseDownLocation = new Point(e.X, e.Y);

            if (pictureBox.AllowDrop == false)
                pictureBox.AllowDrop = true;
        }

        void pictureBox_DoubleClick(object sender, EventArgs e)
        {
            switch (pictureBox.SizeMode)
            {
                case PictureBoxSizeMode.Zoom:
                    if (pictureBox.Image.Size.Height > pictureBox.Size.Height || pictureBox.Image.Size.Width > pictureBox.Size.Width)
                    {
                        zoomPicture();
                    }
                    break;
                case PictureBoxSizeMode.CenterImage:
                    unZoomPicture();
                    break;
            }
        }

        private void unZoomPicture()
        {
            pictureBox.Location = new Point(0, 0);
            pictureBox.Size = pnlPictureBox.Size;
            pictureBox.SizeMode = PictureBoxSizeMode.Zoom;
        }

        void Main_Resize(object sender, EventArgs e)
        {
            if (pictureBox.SizeMode == PictureBoxSizeMode.CenterImage)
            {
                if (pictureBox.Image.Size.Height < pictureBox.Size.Height || pictureBox.Image.Size.Width < pictureBox.Size.Width)
                {
                    unZoomPicture();
                }
            }
        }

        void pictureBox_DragEnter(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.Move;
        }

        void pictureBox_DragOver(object sender, DragEventArgs e)
        {
            if (e.Data.GetData(String.Empty.GetType()).ToString().Equals("zoomer"))
            {
                Point currentMouseLocation = PointToClient(new Point(e.X - pnlPictureBox.Location.X, e.Y - pnlPictureBox.Location.Y));

                //this.Text = pictureBox.Location.ToString() + " mouse: " + currentMouseLocation.ToString() + "x,yDrag: " + string.Format("{{{0},{1}}}", xDrag, yDrag);

                Point proposedLocation = new Point(xPictureBoxOriginal + (currentMouseLocation.X - xDrag), yPictureBoxOriginal + (currentMouseLocation.Y - yDrag));

                if (proposedLocation.X > 0)
                    proposedLocation.X = 0;
                else if (proposedLocation.X < (-1 * (pictureBox.Image.Size.Width - pnlPictureBox.Size.Width)))
                    proposedLocation.X = -1 * (pictureBox.Image.Size.Width - pnlPictureBox.Size.Width);

                if (proposedLocation.Y > 0)
                    proposedLocation.Y = 0;
                else if (proposedLocation.Y < (-1 * (pictureBox.Image.Size.Height - pnlPictureBox.Size.Height)))
                    proposedLocation.Y = -1 * (pictureBox.Image.Size.Height - pnlPictureBox.Size.Height);

                if (pictureBox.Image.Size.Height < pnlPictureBox.Size.Height)
                    proposedLocation.Y = yPictureBoxOriginal;

                if (pictureBox.Image.Size.Width < pnlPictureBox.Size.Width)
                    proposedLocation.X = xPictureBoxOriginal;

                pictureBox.Location = proposedLocation;
            }
        }

        private void calDateTaken_DateChanged(object sender, DateRangeEventArgs e)
        {
            if (dtpDateTaken.Value != e.Start && !e.Start.Equals(new DateTime(dtpDateTaken.Value.Year, dtpDateTaken.Value.Month, dtpDateTaken.Value.Day, 0, 0, 0)))
            {
                dtpDateTaken.Value = e.Start;
            }
        }

        private void sortSetsOnFlickrToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show(string.Format("Are you sure you want to put your flickr sets in REVERSE ALPHABETICAL ORDER?"), Application.ProductName, MessageBoxButtons.OKCancel) == DialogResult.OK)
            {
                try
                {
                    Cursor.Current = Cursors.WaitCursor;

                    Photoset[] photosetsOrdered = new Photoset[photosets.PhotosetCollection.Length];
                    Array.Copy(photosets.PhotosetCollection, photosetsOrdered, photosets.PhotosetCollection.Length);
                    Array.Sort(photosetsOrdered, new PhotosetComparer());

                    string[] photosetIDs = new string[photosetsOrdered.Length];
                    string photosetIDs2 = "";
                    for (int i = 0; i < photosets.PhotosetCollection.Length; i++)
                    {
                        Debug.Print(photosetsOrdered[i].Title);
                        photosetIDs[i] = photosetsOrdered[i].PhotosetId;
                        photosetIDs2 += photosetsOrdered[i].PhotosetId + ",";
                    }
                    photosetIDs2 = photosetIDs2.Substring(0, photosetIDs2.Length - 1);
                    flickr.PhotosetsOrderSets(photosetIDs2);
                }
                finally
                {
                    Cursor.Current = Cursors.Default;
                }
            } // are you sure you want to change this tag?

            pictureList.Select();
        }
    }
}
