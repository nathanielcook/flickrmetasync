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
            this.lnkPicture.LinkClicked += new LinkLabelLinkClickedEventHandler(lnkPicture_LinkClicked);
            this.lnkSet.LinkClicked += new LinkLabelLinkClickedEventHandler(lnkSet_LinkClicked);

            //get flickr authorization token
            loadAuthToken();

            //populate location of sets from registry
            getRegistrySettings();

            //put directories (sets) in tree view
            populateTreeView();

            //load all tags from disk
            loadAllTagsFromDisk();

            this.Show();

            flickr.HttpTimeout = 180000; //wait up to three minutes to get photosets

            if (lastAllTagsScanDate == "" || DateTime.Now.Subtract(DateTime.Parse(lastAllTagsScanDate)).Days > 3)
            {
                //if it's been more than 3 days since the last all tags scan
                //begin reading tags in the background
                //leave this as the last line in Main()
                tagReader.RunWorkerAsync();
            }
        }

        void refreshAllTagsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            tagReader.RunWorkerAsync();
        }

        void renameThisSetToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!setList.SelectedNode.FullPath.Equals(setList.TopNode.FullPath))
            {
                string setID = currentSetId;
                string oldSetName = currentSetName;

                string newSetName = InputBox(string.Format("Enter the new name for the set (currently named {0}):", oldSetName), Application.ProductName, "");

                if (newSetName.Length > 0 && MessageBox.Show(string.Format("Are you sure you want to change \"{0}\" to \"{1}\"?", oldSetName, newSetName), Application.ProductName, MessageBoxButtons.OKCancel) == DialogResult.OK)
                {
                    try
                    {
                        Cursor.Current = Cursors.WaitCursor;
                        Directory.Move(Path.Combine(localFlickrDirectory, oldSetName), Path.Combine(localFlickrDirectory, newSetName));
                        flickr.PhotosetsEditMeta(setID, newSetName, flickr.PhotosetsGetInfo(setID).Description);
                        populateTreeView();
                        photosets = null;
                    }
                    finally
                    {
                        Cursor.Current = Cursors.Default;
                    }
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

                if (newTag.Length > 0 && MessageBox.Show(string.Format("Are you sure you want to change \"{0}\" to \"{1}\"?", oldTag, newTag), Application.ProductName, MessageBoxButtons.OKCancel) == DialogResult.OK)
                {

                    //need some code here to make sure newTag <> oldTag

                    try
                    {
                        Cursor.Current = Cursors.WaitCursor;

                        foreach (string pictureFileName in allTags.GetValues(oldTag))
                        {
                            //need some code here to make sure that newTag isn't already part of this picture

                            //add this filename to allTags under the new key (tag)
                            allTags.Add(newTag, pictureFileName);

                            if (tagReader.IsBusy)
                                tagReaderTags.Add(newTag, pictureFileName);

                            //local                 
                            Picture picture = new Picture(pictureFileName);
                            picture.tags.Remove(oldTag);
                            picture.tags.Add(newTag);
                            picture.Save();

                            //flickr BEGIN----------------------------------------------------------------------
                            string setName = pictureFileName.Replace(Path.GetDirectoryName(Path.GetDirectoryName(pictureFileName)), "").Replace(Path.GetFileName(pictureFileName), "").Replace("\\", "");

                            for (int i = 0; i < photosets.PhotosetCollection.Length; i++)
                            {
                                if (photosets.PhotosetCollection[i].Title.Equals(setName))
                                {
                                    string setID = photosets.PhotosetCollection[i].PhotosetId;

                                    Photo[] photosThisSet = flickr.PhotosetsGetPhotos(setID).PhotoCollection;

                                    for (int k = 0; k < photosThisSet.Length; k++)
                                    {
                                        if (photosThisSet[k].Title.Equals(Path.GetFileNameWithoutExtension(pictureFileName), StringComparison.OrdinalIgnoreCase))
                                        {
                                            picture.flickrID = photosThisSet[k].PhotoId;
                                            break;
                                        }
                                    }
                                    break;
                                }
                            }

                            if (picture.flickrID == null)
                                throw new Exception();
                            else
                            {
                                PhotoInfo photoInfo = flickr.PhotosGetInfo(picture.flickrID);

                                picture.flickrTags = new StringCollection();

                                for (int i = 0; i < photoInfo.Tags.TagCollection.Length; i++)
                                {
                                    picture.flickrTags.Add(photoInfo.Tags.TagCollection[i].Raw);
                                }
                                picture.flickrTags.Remove(oldTag);
                                picture.flickrTags.Add(newTag);

                                setFlickrTags(picture);
                            }
                            //IF IT'S THE CURRENT PICTURE, REFRESH IT---------------------------
                            if (currentContent.flickrID != null && currentContent.flickrID.Equals(picture.flickrID))
                            {
                                mergeLocalInUI(Path.GetFileName(currentContent.filename));
                            }
                        }
                        allTags.Remove(oldTag);

                        if (tagReader.IsBusy)
                            tagReaderTags.Remove(oldTag);

                        //REMOVE OLD FROM THE LISTBOX AND ADD NEW TAG TO LISTBOX
                        lstAllTags.Items.Remove(item);

                        if (!containsTag(lstAllTags, ref newTag))
                        {
                            lstAllTags.Items.Add(newTag, newTag, 0);
                        }

                        saveAllTagsToDisk(allTags);
                    }
                    finally
                    {
                        Cursor.Current = Cursors.Default;
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

                    // if exists in alltags remove from alltags
                    removeTagIfThere(item.Text, currentPicture, allTags);
                    if (tagReader.IsBusy)
                    {
                        // if tagreader is busy and exists in tagreader tags, remove from tagreader tags
                        removeTagIfThere(item.Text, currentPicture, tagReaderTags);
                    }

                    removeTagFromLstAllTagsIfNeeded(item.Text);

                    currentPicture.tags.Remove(item.Text);
                    currentPicture.Save();
                }

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
                //this is so it will do nothing if they are attempting to add a tag to a video and flickr
                //is not yet loaded.
                if (currentContent is Picture || (currentContent is Video && currentContent.flickrLoaded))
                {
                    string tag = txtTag.Text.Trim();

                    //if it's not already there.
                    if (!containsTag(lstTags, ref tag))
                    {
                        if (currentContent is Picture)
                        {
                            Picture currentPicture = ((Picture)currentContent);

                            //add to lstAllTags if not already there
                            if (!containsTag(lstAllTags, ref tag))
                            {
                                lstAllTags.Items.Add(tag, tag, 0);
                            }

                            //add to alltags if not already there
                            addTagIfNotAlreadyThere(tag, currentPicture, allTags);

                            // if tagreader tags is busy, and not in tagreader tags, add to tagreader tags
                            if (tagReader.IsBusy)
                            {
                                addTagIfNotAlreadyThere(tag, currentPicture, tagReaderTags);
                            }

                            //now actually add the tag to the picture
                            currentPicture.tags.Add(tag);
                            currentPicture.Save();

                            saveAllTagsToDisk(allTags);

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
                            item.ForeColor = Color.Red;
                        }

                        lstTags.Items.Add(item);
                        item.EnsureVisible();

                    } //if (!containsTag(lstTags, ref tag))

                    txtTag.Clear();
                    pictureList.Select();

                } //if (currentcontent is Picture || (currentContent is Video && currentContent.flickrLoaded))
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

        private void addTagIfNotAlreadyThere(string tag, Picture picture, NameValueCollection tagCollection)
        {
            string fileName = picture.filename;

            bool foundIt = false;

            if (tagCollection.GetValues(tag) != null)
            {
                foreach (string pictureFileName in tagCollection.GetValues(tag))
                {
                    if (pictureFileName.Equals(fileName, StringComparison.OrdinalIgnoreCase))
                        foundIt = true;
                }
            }

            if (!foundIt)
                tagCollection.Add(tag, fileName);
        }

        private void removeTagIfThere(string tag, Picture picture, NameValueCollection tagCollection)
        {
            string fileName = picture.filename;

            if (tagCollection.GetValues(tag) != null)
            {
                List<String> listOfFilesWithThisTag = new List<String>(tagCollection.GetValues(tag));

                //removes all values with this key (tag)
                tagCollection.Remove(tag);

                //if there was more than 1, lets add the others back
                if (listOfFilesWithThisTag.Count > 1)
                {
                    //exclude the one we are removing
                    listOfFilesWithThisTag.Remove(fileName);

                    foreach (string fileStillHasTag in listOfFilesWithThisTag)
                    {
                        tagCollection.Add(tag, fileStillHasTag);
                    }
                }
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
                btnAddTagToWholeSet.Enabled = false;

            lstAllTags.Items.Clear();

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

            TreeNode mainNode = setList.Nodes.Add(localFlickrDirectory.Replace(Path.GetDirectoryName(localFlickrDirectory), "").Replace("\\", "").Trim());
            mainNode.Name = localFlickrDirectory;

            appendDirectoriesToTreeNode(setList.Nodes[0], localFlickrDirectory);
            setList.Select();
            mainNode.Expand();

            try
            {
                mainNode.FirstNode.Expand();
                setList.SelectedNode = mainNode.FirstNode;
            }
            catch { }
        }

        void setList_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Enter)
            {
                e.SuppressKeyPress = true;
                pictureList.Select();
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
            else if (e.KeyCode >= Keys.A && e.KeyCode <= Keys.Z)
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

                    node.Nodes.Add(subdirNode);
                }
            }
        }

        private void setList_AfterSelect(object sender, TreeViewEventArgs e)
        {
            selectedNodeName = e.Node.Text;

            pictureList.Items.Clear();
            pictureList.View = View.List;

            DirectoryInfo subDir = new DirectoryInfo(e.Node.Name);

            foreach (FileInfo fileInfo in subDir.GetFiles("*.jpg"))
                pictureList.Items.Add(fileInfo.Name);

            foreach (FileInfo fileInfo in subDir.GetFiles("*.avi"))
                pictureList.Items.Add(fileInfo.Name);

            foreach (FileInfo fileInfo in subDir.GetFiles("*.mov"))
                pictureList.Items.Add(fileInfo.Name);

            pictureList.Sorting = SortOrder.Ascending;
            pictureList.Sort();

            if (pictureList.Items.Count > 0)
                pictureList.Items[0].Selected = true;

            btnSetDateTaken.Enabled = false;
            btnSetDateTakenForWholeSet.Enabled = false;
        }

        void pictureList_ItemSelectionChanged(object sender, ListViewItemSelectionChangedEventArgs e)
        {
            if (e.IsSelected)
            {
                if (currentContent != null)
                {
                    if (currentContent is Picture)
                    {
                        Picture currentPicture = ((Picture)currentContent);

                        if (currentPicture.dateTaken.HasValue)
                            previousDateTaken = currentPicture.dateTaken;

                        if (currentPicture.gpsLatitude.HasValue)
                            previousGpsLatitude = currentPicture.gpsLatitude;

                        if (currentPicture.gpsLongitude.HasValue)
                            previousGpsLongitude = currentPicture.gpsLongitude;
                    }
                    else if (currentContent is Video)
                    {
                        if (currentContent.flickrDateTaken.HasValue)
                            previousDateTaken = currentContent.flickrDateTaken;
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
            if (fileName.ToLower().EndsWith(".avi") || fileName.ToLower().EndsWith("*.mov"))
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
                pictureBox.Visible = false;
            }
            else
            {
                axWMP.Ctlcontrols.stop();

                currentContent = new Picture(fullFilePath);
                Picture currentPicture = ((Picture)currentContent);

                loadFlickrPhotoId();

                //load picture
                pictureBox.ImageLocation = currentPicture.filename;

                axWMP.Visible = false;
                pictureBox.Visible = true;

                //load caption
                txtPictureCaption.Text = currentPicture.caption;

                //load date taken
                if (currentPicture.dateTaken.HasValue)
                {
                    calDateTaken.TitleBackColor = Color.LightBlue;
                    calDateTaken.SetDate(currentPicture.dateTaken.Value);
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
                string url = flickr.AuthCalcUrl(frob, AuthLevel.Write);

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
                    tagReadingProgressBar.Value = tagReadingProgress;
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

                    if (currentSetName.Equals(selectedNodeName))
                    {
                        if (btnSetDateTaken.Enabled == false)
                        {
                            btnSetDateTaken.Enabled = true;
                            btnSetDateTakenForWholeSet.Enabled = true;
                        }
                    }
                }

                if (!currentContent.flickrMergedInUI && currentContent.flickrLoaded)
                {
                    mergeFlickrInUI();
                }
            }
        }

        private void mergeFlickrInUI()
        {
            lnkPicture.Links.Add(0, 7, (string.Format("http://www.flickr.com/photos/{0}/{1}/in/set-{2}/", flickrUserName, currentContent.flickrID, currentSetId)));
            lnkPicture.Visible = true;

            lnkSet.Links.Add(0, 3, (string.Format("http://www.flickr.com/photos/{0}/sets/{1}/", flickrUserName, currentSetId)));
            lnkSet.Visible = true;

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

                if (currentPicture.flickrIsPublic == 1)
                {
                    lblVisibility.Text = "Public";
                    lblVisibility.Font = new Font(lblVisibility.Font, FontStyle.Bold);
                }
                else
                {
                    lblVisibility.Text = "Private";
                    lblVisibility.Font = new Font(lblVisibility.Font, FontStyle.Regular);
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
                            lstAllTags.Items.Add(tag, tag, 0);
                        }
                        if (tag != copyOfTag)
                        {
                            //throw new Exception("Flickr capitalization doesn't match local capitalization");
                            currentPicture.flickrMergedInUI = true; //to avoid a new message box every second
                            MessageBox.Show(string.Format("Flickr capitalization for tag \"{0}\" doesn't match local capitalization. Tags not merged.", tag));

                            ListViewItem item = new ListViewItem(tag);
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
                        item.ForeColor = Color.Blue;
                    }
                    else
                    {
                        item.ForeColor = Color.Red;
                        currentPicture.flickrTags.Add(item.Text);
                        saveFlickrTags = true;
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
                foreach (string tag in currentContent.flickrTags)
                {
                    ListViewItem item = new ListViewItem(tag);
                    item.ForeColor = Color.Blue;
                    lstTags.Items.Add(item);
                } //foreach (string tag in currentPicture.flickrTags)              

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

        private void tagReader_DoWork(object sender, DoWorkEventArgs e)
        {
            string[] files = Directory.GetFiles(localFlickrDirectory, "*.jpg", SearchOption.AllDirectories);
            numberOfFiles = files.Length;

            foreach (string file in files)
            {
                if (tagReader.CancellationPending)
                    return;

                Picture picture = new Picture(file);
                foreach (String tag in picture.tags)
                {
                    if (tag.Length > 0)
                    {
                        // if not already in tagreader tags (might have been added on a txtTag_KeyDown by the user)
                        addTagIfNotAlreadyThere(tag, picture, tagReaderTags);

                        if (tagReaderTags.GetValues(tag) == null)
                        {
                            newlyScannedTags.Add(tag);
                        }
                    }
                    else
                    {
                        throw new Exception("zero length tag");
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

        private static void saveAllTagsToDisk(NameValueCollection tagsToSave)
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
                        mergeLocalInUI(pictureList.SelectedItems[0].Text);
                        Cursor.Current = Cursors.Default;
                        pictureList.Focus();
                    }
                }
            }
        }

        private void btnSetDateTakenForWholeSet_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Are you sure? This will take place immediately on flickr too.", Application.ProductName, MessageBoxButtons.OKCancel) == DialogResult.OK)
            {
                if (MessageBox.Show("Are you sure? This is for the WHOLE SET!", Application.ProductName, MessageBoxButtons.OKCancel) == DialogResult.OK)
                {
                    try
                    {
                        Cursor.Current = Cursors.WaitCursor;

                        foreach (ListViewItem item in pictureList.Items)
                        {
                            Picture picture = new Picture(Path.Combine(setList.SelectedNode.Name, item.Text));
                            picture.dateTaken = calDateTaken.SelectionStart;
                            picture.Save();

                            picture.flickrID = picturesDictionary[Path.GetFileNameWithoutExtension(picture.filename)];
                            flickr.PhotosSetDates(picture.flickrID, calDateTaken.SelectionStart, DateGranularity.FullDate);
                        }
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

        private void btnAddTagToWholeSet_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show(string.Format("Are you sure you want to add the tag \"{0}\" to the whole set?", txtTag.Text), Application.ProductName, MessageBoxButtons.OKCancel) == DialogResult.OK)
            {
                try
                {
                    Cursor.Current = Cursors.WaitCursor;

                    string tag = txtTag.Text;
                    addTagToAllItems(tag, pictureList.Items);
                    txtTag.Clear();
                }
                finally
                {
                    Cursor.Current = Cursors.Default;
                }
            }
            pictureList.Select();
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
            if (!containsTag(lstAllTags, ref  tag))//correct the case
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

                    picture.tags.Add(tag);
                    picture.Save();

                    addTagIfNotAlreadyThere(tag, picture, allTags);
                    if (tagReader.IsBusy)
                        addTagIfNotAlreadyThere(tag, picture, tagReaderTags);
                }
                //local END-------------------------------------------------------------------------

                saveAllTagsToDisk(allTags);

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
                    content.flickrTags.Add(tag);

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

        private void btnRemoveTagFromWholeSet_Click(object sender, EventArgs e)
        {
            string tag = lstTags.SelectedItems[0].Text;

            if (MessageBox.Show(string.Format("Are you sure you want to remove the tag \"{0}\" from every picture in the set?\r\n\r\n(Note: Simply use the delete key to remove a tag from just the selected picture.)", tag), Application.ProductName, MessageBoxButtons.OKCancel) == DialogResult.OK)
            {
                try
                {
                    Cursor.Current = Cursors.WaitCursor;

                    removeTagFromAllItems(tag, pictureList.Items);
                }
                finally
                {
                    Cursor.Current = Cursors.Default;
                }
            }
            pictureList.Select();
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

                    removeTagIfThere(tag, picture, allTags);
                    if (tagReader.IsBusy)
                        removeTagIfThere(tag, picture, tagReaderTags);

                    removeTagFromLstAllTagsIfNeeded(tag);
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
    }
}
