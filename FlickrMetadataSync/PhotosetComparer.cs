using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using FlickrNet;

namespace FlickrMetadataSync
{
    class PhotosetComparer : IComparer
    {
        #region IComparer Members

        int IComparer.Compare(object x, object y)
        {
            if (!(x is Photoset) || !(y is Photoset))
                throw new Exception("Objects to compare must be Flickrnet Photosets");

            Photoset photoset1 = ((Photoset)x);
            Photoset photoset2 = ((Photoset)y);

            int result;
            bool photoset1_startsWithDate = (photoset1.Title.Length >= 8) && int.TryParse(photoset1.Title.Substring(0, 8), out result);
            bool photoset2_startsWithDate = (photoset2.Title.Length >= 8) && int.TryParse(photoset2.Title.Substring(0, 8), out result);

            if (photoset1_startsWithDate && !photoset2_startsWithDate)
                return -1;
            else if (!photoset1_startsWithDate && photoset2_startsWithDate)
                return 1;
            else if (photoset1_startsWithDate && photoset2_startsWithDate)
                return (-1 * photoset1.Title.CompareTo(photoset2.Title));
            else
                return photoset1.Title.CompareTo(photoset2.Title);
        }

        #endregion
    }
}
