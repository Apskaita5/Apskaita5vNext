using System;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace Apskaita5.Common.SerializationExtensions
{
    public static class SerializationExtensions
    {

        /// <summary>
        /// Gets a string field by it's index in the delimited string.
        /// Returns string.Empty if the line is empty or if there is no field at fieldIndex.
        /// </summary>
        /// <param name="line">a line (string) that contains delimited fields</param>
        /// <param name="fieldIndex">a zero based index of the field to get</param>
        /// <param name="fieldDelimiter">a string that delimits fields in line</param>
        public static string GetDelimitedField(this string line, int fieldIndex, string fieldDelimiter)
        {
            if (null == fieldDelimiter || fieldDelimiter.Length < 1)
                throw new ArgumentNullException(nameof(fieldDelimiter));

            if (line.IsNullOrWhiteSpace()) return string.Empty;

            var result = line.Split(new string[] { fieldDelimiter }, StringSplitOptions.None);

            if (result.Length < (fieldIndex + 1)) return string.Empty;

            return result[fieldIndex];
        }

        /// <summary>
        /// Serializes any object to XML string using <see cref="XmlSerializer">XmlSerializer</see>.
        /// </summary>
        /// <typeparam name="T">a type of the object to serialize</typeparam>
        /// <param name="objectToSerialize">an object to serialize</param>
        /// <param name="encoding">text encoding to use (if null, UTF8 without BOM is used)</param>
        /// <returns>XML string</returns>
        /// <remarks>an object should be XML serializable in order the method to work</remarks>
        /// <exception cref="ArgumentNullException">objectToSerialize is not specified</exception>
        public static string WriteToXml<T>(this T objectToSerialize, Encoding encoding = null)
        {
            if (!objectToSerialize.GetType().IsValueType && ReferenceEquals(objectToSerialize, null))
                throw new ArgumentNullException(nameof(objectToSerialize));

            if (null == encoding) encoding = new UTF8Encoding(false);

            var xmlSerializer = new XmlSerializer(typeof(T));

            var settings = new XmlWriterSettings
            {
                Indent = true,
                IndentChars = " ",
                Encoding = encoding
            };

            using (var ms = new MemoryStream())
            {
                using (var writer = XmlWriter.Create(ms, settings))
                {
                    xmlSerializer.Serialize(writer, objectToSerialize);
                    return encoding.GetString(ms.ToArray());
                }
            }
        }

        /// <summary>
        /// Deserializes any object from XML string using <see cref="XmlSerializer">XmlSerializer</see>.
        /// </summary>
        /// <typeparam name="T">a type of the object to deserialize</typeparam>
        /// <param name="xmlString">an XML string that contains the deserializable object data</param>
        /// <returns>an object of type T</returns>
        /// <remarks>an object should be XML serializable in order the method to work</remarks>
        /// <exception cref="ArgumentNullException">xmlString is not specified</exception>
        public static T CreateFromXml<T>(this string xmlString)
        {
            if (xmlString.IsNullOrWhiteSpace()) throw new ArgumentNullException(nameof(xmlString));

            var xmlSerializer = new XmlSerializer(typeof(T));

            using (var textReader = new StringReader(xmlString))
            {
                return (T)xmlSerializer.Deserialize(textReader);
            }
        }


        ///// <summary>
        ///// Converts byte array to image.
        ///// </summary>
        ///// <param name="source">the byte array to convert</param>
        //public static Image ByteArrayToImage(byte[] source)
        //{

        //    if (source == null || source.Length < 10) return null;

        //    Image result = null;

        //    using (var ms = new MemoryStream(source))
        //    {
        //        try
        //        {
        //            result = System.Drawing.Image.FromStream(ms);
        //        }
        //        catch (Exception){}
        //    }

        //    return result;

        //}

        ///// <summary>
        ///// Converts <see cref="System.Drawing.Image" /> to byte array encoded by 
        ///// <see cref="System.Drawing.Imaging.ImageFormat.Jpeg" />.
        ///// </summary>
        ///// <param name="source">Image to serialize to byte array.</param>
        //public static byte[] ImageToByteArray(Image source)
        //{

        //    if (source == null) return null;

        //    byte[] result = null;

        //    using (var imageToSave = new Bitmap(source.Width, source.Height, source.PixelFormat))
        //    {
        //        using (var gr = Graphics.FromImage(imageToSave))
        //        {
        //            gr.DrawImage(source, new PointF(0, 0));
        //        }

        //        using (var ms = new MemoryStream())
        //        {
        //            try
        //            {
        //                imageToSave.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
        //                result = ms.ToArray();
        //            }
        //            catch (Exception){}
        //        }
        //    }

        //    GC.Collect();

        //    return result;

        //}

    }
}
