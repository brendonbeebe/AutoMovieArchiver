using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Diagnostics;
using System.Net;
using System.IO;
using System.Xml;

namespace AMA
{
    class TitleRetriever
    {
        string drive = "G";
        string dvdTitle = "";
        string dvdYear = "";
        string dvdPlot = "";

        public TitleRetriever(string d)
        {
            drive = d;
        }

        private string getCorrectTitle()
        {
            string title = dvdTitle + "(" + dvdYear + ")";
            return title;
        }

        private string getDVDId()
        {
            StringBuilder outputBuilder;
            ProcessStartInfo processStartInfo;
            Process process;
            outputBuilder = new StringBuilder();

            processStartInfo = new ProcessStartInfo();
            processStartInfo.CreateNoWindow = true;
            processStartInfo.RedirectStandardOutput = true;
            processStartInfo.RedirectStandardInput = true;
            processStartInfo.UseShellExecute = false;
            processStartInfo.Arguments = drive;
            processStartInfo.FileName = "dvdid.exe";

            process = new Process();
            process.StartInfo = processStartInfo;
            // enable raising events because Process does not raise events by default
            process.EnableRaisingEvents = true;
            // attach the event handler for OutputDataReceived before starting the process
            process.OutputDataReceived += new DataReceivedEventHandler
            (
                delegate(object sender, DataReceivedEventArgs e)
                {
                    // append the new data to the data already read-in
                    outputBuilder.Append(e.Data);
                }
            );
            // start the process
            // then begin asynchronously reading the output
            // then wait for the process to exit
            // then cancel asynchronously reading the output
            process.Start();
            process.BeginOutputReadLine();
            process.WaitForExit();
            process.CancelOutputRead();

            // use the output
            string output = outputBuilder.ToString();
            return output.Replace("|", "");

        }

        private void parseXMLResponse(XmlDocument response)
        {
            XmlNode node = response.SelectSingleNode("//dvdXml/dvd/title");
            dvdTitle = node.InnerText.ToString();
            node = response.SelectSingleNode("//dvdXml/dvd/year");
            dvdYear = node.InnerText.ToString();
            node = response.SelectSingleNode("//dvdXml/dvd/plot");
            dvdPlot = node.InnerText.ToString();

            if (dvdTitle == "")
                dvdTitle = "Title Unavailable";
        }

        public string getMovieName()
        {
            string dvdid = getDVDId();
            string username = "nyanyo";
            string password = "mydvdid";
            string apiHttp = @"http://api.dvdxml.com/";

            string request = 
            "<dvdXml>"
        	    +"<authentication>"
        		    +"<user>" + username + "</user>"
        		    +"<password>" + password + "</password>"
        	    +"</authentication>"
        	    +"<requests>"
        		    +"<request type =\"information\">"
        			    +"<dvdId>" + dvdid + "</dvdId>"
        		    +"</request>"
        	    +"</requests>"
            +"</dvdXml>";

            XmlDocument doc = new XmlDocument();
            XmlDocument response = new XmlDocument();
            doc.LoadXml(request);
            response = PostXMLTransaction(apiHttp, doc);
            parseXMLResponse(response);

            string newTitle = getCorrectTitle();
            return newTitle;
        }

        public static XmlDocument PostXMLTransaction(string v_strURL, XmlDocument v_objXMLDoc)
        {
            //Declare XMLResponse document
            XmlDocument XMLResponse = null;

            //Declare an HTTP-specific implementation of the WebRequest class.
            HttpWebRequest objHttpWebRequest;

            //Declare an HTTP-specific implementation of the WebResponse class
            HttpWebResponse objHttpWebResponse = null;

            //Declare a generic view of a sequence of bytes
            Stream objRequestStream = null;
            Stream objResponseStream = null;

            //Declare XMLReader
            XmlTextReader objXMLReader;

            //Creates an HttpWebRequest for the specified URL.
            objHttpWebRequest = (HttpWebRequest)WebRequest.Create(v_strURL);

            try
            {
                //---------- Start HttpRequest

                //Set HttpWebRequest properties
                byte[] bytes;
                bytes = System.Text.Encoding.ASCII.GetBytes(v_objXMLDoc.InnerXml);
                objHttpWebRequest.Method = "POST";
                objHttpWebRequest.ContentLength = bytes.Length;
                objHttpWebRequest.ContentType = "text/xml; encoding='utf-8'";

                //Get Stream object
                objRequestStream = objHttpWebRequest.GetRequestStream();

                //Writes a sequence of bytes to the current stream
                objRequestStream.Write(bytes, 0, bytes.Length);

                //Close stream
                objRequestStream.Close();

                //---------- End HttpRequest

                //Sends the HttpWebRequest, and waits for a response.
                objHttpWebResponse = (HttpWebResponse)objHttpWebRequest.GetResponse();

                //---------- Start HttpResponse
                if (objHttpWebResponse.StatusCode == HttpStatusCode.OK)
                {
                    //Get response stream
                    objResponseStream = objHttpWebResponse.GetResponseStream();

                    //Load response stream into XMLReader
                    objXMLReader = new XmlTextReader(objResponseStream);

                    //Declare XMLDocument
                    XmlDocument xmldoc = new XmlDocument();
                    xmldoc.Load(objXMLReader);

                    //Set XMLResponse object returned from XMLReader
                    XMLResponse = xmldoc;

                    //Close XMLReader
                    objXMLReader.Close();
                }

                //Close HttpWebResponse
                objHttpWebResponse.Close();
            }
            catch (WebException we)
            {
                //TODO: Add custom exception handling
                throw new Exception(we.Message);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            finally
            {
                //Close connections
                objRequestStream.Close();
                objResponseStream.Close();
                objHttpWebResponse.Close();

                //Release objects
                objXMLReader = null;
                objRequestStream = null;
                objResponseStream = null;
                objHttpWebResponse = null;
                objHttpWebRequest = null;
            }

            //Return
            return XMLResponse;
        }
    }
}
