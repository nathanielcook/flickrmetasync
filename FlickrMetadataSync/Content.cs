using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Specialized;
using FlickrNet;
using System.IO;

namespace FlickrMetadataSync
{
    public abstract class Content
    {
        public string filename;

        public string flickrCaption;
        public DateTime? flickrDateTaken;
        public DateTime? flickrDatePosted;
        public double? flickrGpsLatitude;
        public double? flickrGpsLongitude;
        public string flickrTitle;
        public string flickrID;
        public StringCollection flickrTags;

        public bool flickrLoaded;
        public bool flickrMergedInUI;

        public void loadFlickrInfo(PhotoInfo photoInfo)
        {
            if (Path.GetFileNameWithoutExtension(filename).Equals(photoInfo.Title))
            {
                if (photoInfo.Description.Length > 0)
                    flickrCaption = photoInfo.Description;

                flickrDateTaken = photoInfo.Dates.TakenDate;
                flickrDatePosted = photoInfo.Dates.PostedDate;
                if (photoInfo.Location != null)
                {
                    flickrGpsLatitude = photoInfo.Location.Latitude;
                    flickrGpsLongitude = photoInfo.Location.Longitude;
                }
                flickrTags = new StringCollection();

                for (int i = 0; i < photoInfo.Tags.TagCollection.Length; i++)
                {
                    flickrTags.Add(photoInfo.Tags.TagCollection[i].Raw);
                }

                flickrTitle = photoInfo.Title;
                flickrLoaded = true;
            }
            else
            {
                throw new Exception("title not equal");
            }
        }
    }
}
