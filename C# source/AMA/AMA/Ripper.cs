using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.IO;

namespace AMA
{
    class Ripper
    {
        public Ripper()
        { }

        public void transcode(string mkvFile, string dvdTitle)
        {
            Process p = new Process();
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.FileName = "HandBrakeCLI.exe";
            p.StartInfo.Arguments = "-i \"" + mkvFile + "\" -o " + dvdTitle + ".mp4 -f mp4 -e x264";
            if(p.Start())
            {
                Console.Write("Transcoding");
                p.WaitForExit();
                if(p.ExitCode == 0)
                    Console.Write("Successfully transcoded " + dvdTitle);
                else
                    Console.Write("Error: " + p.ExitCode);
            }
            else
                Console.Write("Error transcoding, quitting");
        }


        public string rip(string drive, string dvdTitle)
        {
            string outpath = Path.Combine(Path.GetTempPath(), dvdTitle);
            //////////////debug code//////////////////////
            //remove before release
            //if (true)
            //    return Directory.GetFiles(outpath)[0];
            //////////////////////////////////////////////
            Console.Write("Saving to: " + outpath);

		    if (!Directory.Exists(outpath))
		        Directory.CreateDirectory(outpath);
    
            Process p = new Process();
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.FileName = @"C:\Program Files\MakeMKV\makemkvcon.exe";
            p.StartInfo.Arguments = "-r --minlength=2700 mkv disc:0 all \"" + outpath + "\"";
            Console.Write("\nSaving to: " + outpath);
            Console.Write("\n" + p.StartInfo.Arguments);
            if(p.Start())
            {
                Console.Write("\nRipping");
                p.WaitForExit();
                if(p.ExitCode == 0)
                {
                    Console.Write("\nSuccessfully ripped " + dvdTitle);
                    return Directory.GetFiles(outpath)[0];
                }
                else
                    Console.Write("\nError: " + p.ExitCode);
            }
            else
                Console.Write("\nError ripping, quitting");
            return "-1";
        }
    }
}
