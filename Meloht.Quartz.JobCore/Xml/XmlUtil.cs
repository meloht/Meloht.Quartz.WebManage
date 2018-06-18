using log4net;
using Meloht.Quartz.JobCore.Log;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml.Serialization;

namespace Meloht.Quartz.JobCore.Xml
{
    public class XmlUtil
    {
        private static ILog log = LogManager.GetLogger(Log4NetHelper.Instance.RepositoryName, typeof(XmlUtil));

        public static T DeserializeXmlFromString<T>(string xml) where T : class, new()
        {
            try
            {
                using (var stream = new StringReader(xml))
                {
                    XmlSerializer xs = new XmlSerializer(typeof(T));
                    T data = (T)xs.Deserialize(stream);
                    return data;
                }
            }
            catch (Exception ex)
            {
                log.Error(ex);
            }

            return default(T);
        }

        public static string GetXmlString(string fileName)
        {
            try
            {
                using (var stream = File.Open(fileName, FileMode.Open, FileAccess.Read))
                using (StreamReader sr = new StreamReader(stream))
                {
                    return sr.ReadToEnd();
                }
            }
            catch (Exception ex)
            {
                log.Error(ex);
            }
            return string.Empty;
        }

        public static T DeserializeXml<T>(string fileName) where T : class, new()
        {
            try
            {
                using (var stream = File.Open(fileName, FileMode.Open, FileAccess.Read))
                {
                    XmlSerializer xs = new XmlSerializer(typeof(T));
                    T data = (T)xs.Deserialize(stream);
                    return data;
                }
            }
            catch (Exception ex)
            {
                log.Error(ex);
            }
            return default(T);
        }

        public static bool SerializeXml<T>(T model, string fileName) where T : class, new()
        {
            try
            {
                using (Stream stream = new FileStream(fileName, FileMode.Create))
                {
                    XmlSerializer xs = new XmlSerializer(typeof(T));
                    xs.Serialize(stream, model);
                    return true;
                }
            }
            catch (Exception ex)
            {
                log.Error(ex);
            }
            return false;
        }
    }
}
