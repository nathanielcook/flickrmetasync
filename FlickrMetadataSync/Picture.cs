using System;
using System.Collections.Generic;
using System.Text;
using System.Collections.Specialized;
using System.Windows.Media.Imaging;
using System.IO;
using System.Collections.ObjectModel;
using FlickrNet;

namespace FlickrMetadataSync
{
    public enum EXIF_ORIENTATION
    {
        None = 0,
        Normal = 1,
        Mirror = 2,
        MirrorAndFlip = 3,
        Flip = 4,
        FlipAndClockwise90 = 5,
        Clockwise90 = 6,
        Clockwise180 = 7,
        Clockwise270 = 8

        //From http://www.exif.org/Exif2-2.PDF
        //1 = The 0th row is at the visual top of the image, and the 0th column is the visual left-hand side.
        //2 = The 0th row is at the visual top of the image, and the 0th column is the visual right-hand side.
        //3 = The 0th row is at the visual bottom of the image, and the 0th column is the visual right-hand side.
        //4 = The 0th row is at the visual bottom of the image, and the 0th column is the visual left-hand side.
        //5 = The 0th row is the visual left-hand side of the image, and the 0th column is the visual top.
        //6 = The 0th row is the visual right-hand side of the image, and the 0th column is the visual top.
        //7 = The 0th row is the visual right-hand side of the image, and the 0th column is the visual bottom.
        //8 = The 0th row is the visual left-hand side of the image, and the 0th column is the visual bottom.        
    }

    public class Picture : Content
    {
        public string caption;
        public DateTime? dateTaken;
        public double? gpsLatitude;
        public double? gpsLongitude;
        public StringCollection tags;
        public EXIF_ORIENTATION orientation = EXIF_ORIENTATION.Normal;

        private static object fileAccess = new object();

        public Picture(string filename)
        {
            lock (fileAccess)
            {
                this.filename = filename;

                using (Stream pictureFileStream = new FileStream(filename, FileMode.Open, FileAccess.Read))
                {
                    // Disable caching to prevent excessive memory usage.
                    JpegBitmapDecoder decoder = new JpegBitmapDecoder(pictureFileStream, BitmapCreateOptions.None, BitmapCacheOption.None);
                    BitmapMetadata bitmapMetadata = (BitmapMetadata)decoder.Frames[0].Metadata;

                    if (bitmapMetadata != null)
                    {
                        if (bitmapMetadata.GetQuery(EXIF_ORIENTATION_QUERY) != null)
                        {
                            orientation = (EXIF_ORIENTATION)Enum.Parse(typeof(EXIF_ORIENTATION), bitmapMetadata.GetQuery(EXIF_ORIENTATION_QUERY).ToString());
                        }

                        //pictureMetadata.Title = bitmapMetadata.Title;
                        if (bitmapMetadata.Comment != null && bitmapMetadata.Comment.Length > 0)
                        {
                            caption = bitmapMetadata.Comment;
                        }

                        if (bitmapMetadata.GetQuery(DATE_TAKEN_QUERY) != null && !bitmapMetadata.GetQuery(DATE_TAKEN_QUERY).ToString().StartsWith("0000"))
                        {
                            try
                            {
                                string rawDate = bitmapMetadata.GetQuery(DATE_TAKEN_QUERY).ToString();
                                string[] tokens = rawDate.Split(':', ' ');
                                int year = int.Parse(tokens[0]);
                                int month = int.Parse(tokens[1]);
                                int day = int.Parse(tokens[2]);
                                int hour = int.Parse(tokens[3]);
                                int minute = int.Parse(tokens[4]);
                                int second = int.Parse(tokens[5]);
                                dateTaken = new DateTime(year, month, day, hour, minute, second);
                            }
                            catch
                            {
                                dateTaken = Convert.ToDateTime(bitmapMetadata.DateTaken);
                            }
                        }
                        else
                        {
                            try
                            {
                                if (bitmapMetadata.DateTaken != null)
                                    dateTaken = Convert.ToDateTime(bitmapMetadata.DateTaken);
                            }
                            catch { } //this is to help with some weird picture files that complain with a 
                            //"object must be initialized" error.
                        }

                        tags = new StringCollection();
                        if (bitmapMetadata.Keywords != null)
                        {
                            //tags                             
                            foreach (string tag in bitmapMetadata.Keywords)
                            {
                                //if there is a question mark then there is a text encoding issue.
                                if (!tag.Contains("?"))
                                    tags.Add(tag);
                            }
                        }

                        //the rest is all gps stuff
                        byte[] gpsVersionNumbers = bitmapMetadata.GetQuery(GPS_VERSION_QUERY) as byte[];

                        ulong[] latitudes = bitmapMetadata.GetQuery(LATITUDE_QUERY) as ulong[];
                        if (latitudes != null)
                        {
                            double latitude = ConvertCoordinate(latitudes);

                            // N or S
                            string northOrSouth = (string)bitmapMetadata.GetQuery(NORTH_OR_SOUTH_QUERY);
                            if (northOrSouth == "S")
                            {
                                // South means negative latitude.
                                latitude = -latitude;
                            }
                            this.gpsLatitude = latitude;
                        }

                        ulong[] longitudes = bitmapMetadata.GetQuery(LONGITUDE_QUERY) as ulong[];
                        if (longitudes != null)
                        {
                            double longitude = ConvertCoordinate(longitudes);

                            // E or W
                            string eastOrWest = (string)bitmapMetadata.GetQuery(EAST_OR_WEST_QUERY);
                            if (eastOrWest == "W")
                            {
                                // West means negative longitude.
                                longitude = -longitude;
                            }
                            this.gpsLongitude = longitude;
                        }
                    }
                }
            }
        }

        public void Save()
        {
            lock (fileAccess)
            {
                bool inPlaceSuccessful = false;

                using (Stream savedFile = File.Open(filename, FileMode.Open, FileAccess.ReadWrite))
                {
                    BitmapDecoder output = BitmapDecoder.Create(savedFile, BitmapCreateOptions.None, BitmapCacheOption.Default);
                    InPlaceBitmapMetadataWriter bitmapMetadata = output.Frames[0].CreateInPlaceBitmapMetadataWriter();

                    SetMetadata(bitmapMetadata);

                    if (bitmapMetadata.TrySave())
                    {
                        //if it was able to save it in place, then...
                        //Great! We're done...
                        inPlaceSuccessful = true;
                    }
                }

                //if the in place save wasn't successful, try to save another way.
                if (!inPlaceSuccessful)
                {
                    string outputFileName = ChangeExtension(filename, "output");

                    WriteCopyOfPictureUsingWic(filename, outputFileName);

                    File.Delete(filename);
                    File.Move(outputFileName, filename);
                }
            }
        }

        private string ChangeExtension(string filePath, string newExtension)
        {
            int dotIndex = filePath.LastIndexOf('.');

            if (dotIndex == -1)
            {
                return filePath;
            }
            else
            {
                return filePath.Substring(0, dotIndex + 1) + newExtension;
            }
        }

        private void SetMetadata(BitmapMetadata bitmapMetadata)
        {
            if (caption != null)
            {
                bitmapMetadata.Comment = caption;
            }

            if (dateTaken.HasValue)
            {
                bitmapMetadata.DateTaken = dateTaken.Value.ToString("M/d/yyyy HH:mm:ss");
                bitmapMetadata.SetQuery(DATE_TAKEN_QUERY, dateTaken.Value.ToString("yyyy:MM:dd HH:mm:ss"));
                bitmapMetadata.SetQuery(DIGITIZED_DATE_QUERY, dateTaken.Value.ToString("yyyy:MM:dd HH:mm:ss"));
                bitmapMetadata.SetQuery(ORIGINAL_DATE_QUERY, dateTaken.Value.ToString("yyyy:MM:dd HH:mm:ss"));
            }

            //-----------tags----------------------
            List<String> tagsList = new List<String>();

            foreach (string tag in tags)
            {
                if (tag.Length > 0)
                    tagsList.Add(tag);
            }

            if (tagsList.Count == 0)
                tagsList.Add("");

            //XMP
            bitmapMetadata.Keywords = new ReadOnlyCollection<string>(tagsList);

            //IPTC
            string[] iptcTagsList = tagsList.ToArray();
            bitmapMetadata.SetQuery(IPTC_KEYWORDS_QUERY, iptcTagsList);
            //-----------tags----------------------

            //the rest is gps stuff
            if (gpsLatitude.HasValue && gpsLongitude.HasValue)
            {
                bitmapMetadata.SetQuery(LATITUDE_QUERY, ConvertCoordinate(gpsLatitude.Value));
                bitmapMetadata.SetQuery(LONGITUDE_QUERY, ConvertCoordinate(gpsLongitude.Value));

                byte[] gpsVersionNumbers = new byte[4];
                gpsVersionNumbers[0] = 0;
                gpsVersionNumbers[1] = 0;
                gpsVersionNumbers[2] = 2;
                gpsVersionNumbers[3] = 2;
                bitmapMetadata.SetQuery(GPS_VERSION_QUERY, gpsVersionNumbers);

                string northOrSouth = (gpsLatitude.Value >= 0) ? "N" : "S";
                bitmapMetadata.SetQuery(NORTH_OR_SOUTH_QUERY, northOrSouth);

                string eastOrWest = (gpsLongitude.Value >= 0) ? "E" : "W";
                bitmapMetadata.SetQuery(EAST_OR_WEST_QUERY, eastOrWest);
            }
            else
            {
                if (bitmapMetadata.GetQuery(LATITUDE_QUERY) != null)
                {
                    bitmapMetadata.RemoveQuery(LATITUDE_QUERY);
                    bitmapMetadata.RemoveQuery(LONGITUDE_QUERY);
                    bitmapMetadata.RemoveQuery(NORTH_OR_SOUTH_QUERY);
                    bitmapMetadata.RemoveQuery(EAST_OR_WEST_QUERY);

                    throw new Exception("Did you really mean to delete GPS data?");
                }
            }
        }

        private static ulong[] ConvertCoordinate(double coordinate)
        {
            ulong[] coordinates = new ulong[3];

            // Make sure coordinate is positive.
            coordinate = Math.Abs(coordinate);

            double degrees = Math.Floor(coordinate);

            coordinate -= degrees;

            double minutes = Math.Floor(coordinate * 60.0);

            coordinate -= (minutes / 60.0);

            double seconds = Math.Floor(coordinate * 3600.0);

            coordinates[0] = Convert.ToUInt64(degrees + DEGREES_OFFSET);
            coordinates[1] = Convert.ToUInt64(minutes + MINUTES_OFFSET);
            coordinates[2] = Convert.ToUInt64((seconds * 100.0) + SECONDS_OFFSET);

            return coordinates;
        }

        private void WriteCopyOfPictureUsingWic(string originalFileName, string outputFileName)
        {
            bool tryOneLastMethod = false;

            using (Stream originalFile = new FileStream(originalFileName, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                BitmapCreateOptions createOptions = BitmapCreateOptions.PreservePixelFormat | BitmapCreateOptions.IgnoreColorProfile;
                BitmapDecoder original = BitmapDecoder.Create(originalFile, createOptions, BitmapCacheOption.None);

                JpegBitmapEncoder output = new JpegBitmapEncoder();

                if (original.Frames[0] != null && original.Frames[0].Metadata != null)
                {
                    BitmapMetadata bitmapMetadata = original.Frames[0].Metadata.Clone() as BitmapMetadata;
                    bitmapMetadata.SetQuery("/app1/ifd/PaddingSchema:Padding", METADATA_PADDING_IN_BYTES);
                    bitmapMetadata.SetQuery("/app1/ifd/exif/PaddingSchema:Padding", METADATA_PADDING_IN_BYTES);
                    bitmapMetadata.SetQuery("/xmp/PaddingSchema:Padding", METADATA_PADDING_IN_BYTES);

                    SetMetadata(bitmapMetadata);

                    output.Frames.Add(BitmapFrame.Create(original.Frames[0], original.Frames[0].Thumbnail, bitmapMetadata, original.Frames[0].ColorContexts));
                }

                try
                {
                    using (Stream outputFile = File.Open(outputFileName, FileMode.Create, FileAccess.ReadWrite))
                    {
                        output.Save(outputFile);
                    }
                }
                catch (Exception e) //System.Exception, NotSupportedException, InvalidOperationException e) //ArgumentException
                {
                    if (e is NotSupportedException || e is ArgumentException)
                    {
                        System.Diagnostics.Debug.Print(e.Message);

                        output = new JpegBitmapEncoder();

                        output.Frames.Add(BitmapFrame.Create(original.Frames[0], original.Frames[0].Thumbnail, original.Metadata, original.Frames[0].ColorContexts));

                        using (Stream outputFile = File.Open(outputFileName, FileMode.Create, FileAccess.ReadWrite))
                        {
                            output.Save(outputFile);
                        }

                        tryOneLastMethod = true;
                    }
                    else
                    {
                        throw new Exception("Error saving picture.", e);
                    }
                }
            }

            if (tryOneLastMethod)
            {
                File.Move(outputFileName, outputFileName + "tmp");

                using (Stream recentlyOutputFile = new FileStream(outputFileName + "tmp", FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    BitmapCreateOptions createOptions = BitmapCreateOptions.PreservePixelFormat | BitmapCreateOptions.IgnoreColorProfile;
                    BitmapDecoder original = BitmapDecoder.Create(recentlyOutputFile, createOptions, BitmapCacheOption.None);
                    JpegBitmapEncoder output = new JpegBitmapEncoder();
                    if (original.Frames[0] != null && original.Frames[0].Metadata != null)
                    {
                        BitmapMetadata bitmapMetadata = original.Frames[0].Metadata.Clone() as BitmapMetadata;
                        bitmapMetadata.SetQuery("/app1/ifd/PaddingSchema:Padding", METADATA_PADDING_IN_BYTES);
                        bitmapMetadata.SetQuery("/app1/ifd/exif/PaddingSchema:Padding", METADATA_PADDING_IN_BYTES);
                        bitmapMetadata.SetQuery("/xmp/PaddingSchema:Padding", METADATA_PADDING_IN_BYTES);

                        SetMetadata(bitmapMetadata);

                        output.Frames.Add(BitmapFrame.Create(original.Frames[0], original.Frames[0].Thumbnail, bitmapMetadata, original.Frames[0].ColorContexts));
                    }

                    using (Stream outputFile = File.Open(outputFileName, FileMode.Create, FileAccess.ReadWrite))
                    {
                        output.Save(outputFile);
                    }
                }
                File.Delete(outputFileName + "tmp");
            }
        }

        private double ConvertCoordinate(ulong[] coordinates)
        {
            int degrees;
            int minutes;
            double seconds;

            degrees = (int)splitLongAndDivide(coordinates[0]);
            minutes = (int)splitLongAndDivide(coordinates[1]);
            seconds = splitLongAndDivide(coordinates[2]);

            double coordinate = degrees + (minutes / 60.0) + (seconds / 3600);
            double roundedCoordinate = Math.Floor(coordinate * COORDINATE_ROUNDING_FACTOR) / COORDINATE_ROUNDING_FACTOR;

            return roundedCoordinate;
        }

        private static double splitLongAndDivide(ulong number)
        {
            byte[] bytes = BitConverter.GetBytes(number);
            int int1 = BitConverter.ToInt32(bytes, 0);
            int int2 = BitConverter.ToInt32(bytes, 4);
            return ((double)int1 / (double)int2);
        }

        #region Private Constants
        private const uint METADATA_PADDING_IN_BYTES = 2048;
        private const string DATE_TAKEN_QUERY = "/app1/ifd/{ushort=306}";
        private const string ORIGINAL_DATE_QUERY = "/app1/ifd/exif/{ushort=36867}";
        private const string DIGITIZED_DATE_QUERY = "/app1/ifd/exif/{ushort=36868}";
        private const string LATITUDE_QUERY = "/app1/ifd/gps/subifd:{ulong=2}";
        private const string LONGITUDE_QUERY = "/app1/ifd/gps/subifd:{ulong=4}";
        private const string NORTH_OR_SOUTH_QUERY = "/app1/ifd/gps/subifd:{char=1}";
        private const string EAST_OR_WEST_QUERY = "/app1/ifd/gps/subifd:{char=3}";
        private const string GPS_VERSION_QUERY = "/app1/ifd/gps/";
        private const string IPTC_KEYWORDS_QUERY = "/app13/irb/8bimiptc/iptc/keywords";
        private const string EXIF_ORIENTATION_QUERY = "/app1/ifd/{ushort=274}";
        private const long DEGREES_OFFSET = 0x100000000;
        private const long MINUTES_OFFSET = 0x100000000;
        private const long SECONDS_OFFSET = 0x6400000000;
        private const double COORDINATE_ROUNDING_FACTOR = 1000000.0;
        #endregion
    }
}
