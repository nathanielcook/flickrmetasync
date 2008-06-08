using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FlickrMetadataSync
{
    class Video : Content
    {
        public Video(string filename)
        {
            this.filename = filename;
        }
    }
}
