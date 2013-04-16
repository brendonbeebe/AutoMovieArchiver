using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;

namespace AMA
{
    class Program
    {
        private NotifyIcon notifyIcon1 = new NotifyIcon();
        protected Thread m_thread;
        protected ManualResetEvent m_shutdownEvent;

        [STAThread]
        static void Main(string[] args)
        {
            Program p = new Program();
            p.setup();
        }

        private void setup()
        {
            if (!File.Exists("settings.conf"))
            {
                Form1 form = new Form1();
                form.ShowDialog();
            }

            ThreadStart ts = new ThreadStart(MonitorDrives);
            this.m_shutdownEvent = new ManualResetEvent(false);
            m_thread = new Thread(ts);
            m_thread.Start();
        }
        private void showPopup(string s)
        {
            notifyIcon1.Icon = new Icon("dvd.ico");
            notifyIcon1.Visible = true;
            notifyIcon1.ShowBalloonTip(5000, "Attention!", s, ToolTipIcon.Info);
        }

        protected void MonitorDrives()
        {
            while (true)
            {
                foreach (DriveInfo drive in DriveInfo.GetDrives().Where(d => d.DriveType == DriveType.CDRom))
                {
                    if (drive.IsReady)
                    {
                        showPopup(" " + drive.Name + " DVD inserted!");
                        TitleRetriever t = new TitleRetriever(drive.Name);
                        string DVDtitle = t.getMovieName();
                        showPopup(DVDtitle + "\n--Ripping--");
                        Ripper r = new Ripper();
                        string filename = r.rip(drive.Name, DVDtitle);
                        if (filename != "-1")
                        {
                            showPopup("Ripped!\n--Transcoding--");
                            r.transcode(filename, DVDtitle);
                        }
                        else
                            showPopup("Error Ripping! Exiting!");
                        ///////////////////need to add code to copy file to location in settings.conf////////////////////
                        //if (File.Exists(path))
                        //    File.Move(path, newpath);
                        /////////////////////////////////////////////////////////////////////////////////////////////////
                        showPopup("Done!");
                        string rt = "";
                        mciSendStringA("set CDAudio door open", rt, 127, 0);
                        
                    }
                }

            }
        }

        [DllImport("winmm.dll", EntryPoint = "mciSendStringA", CallingConvention = CallingConvention.Cdecl)]
        public static extern void mciSendStringA(string lpstrCommand,
               string lpstrReturnString, long uReturnLength, long hwndCallback);
    }
}
