using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO.Ports;
using System.IO;

namespace Idnaf.SerialListen
{
    class Program
    {
        static SerialPort serialPort = new SerialPort();
        static bool isHex = false;
        static MemoryStream memoryStream = new MemoryStream();

        static bool isDisplay = true;
        static void DisplayHelp()
        {
            Console.WriteLine("Idnaf Serial Port Listener - (c) 2008-2018 http://github.com/fandigunawan");
            Console.WriteLine("Usage :");
            Console.WriteLine("-l --list     Display all COM port available. Application will directly exit after this command executed properly.");
            Console.WriteLine("-p --port     Port name");
            Console.WriteLine("-b --baudrate Baudrate (default 115200)");

            Console.WriteLine("--hex         Display hexadecimal number instead");
            Console.WriteLine("--nodisplay   Do not show data directly to console. Application will print '.' when data received");
            Console.WriteLine("--log      file_name       Save data to log file");
            Console.WriteLine("--help       Show this help page");
        }
        static void Main(string[] args)
        {
            string portName = string.Empty;
            int baudRate = 115200;
            string fileLog = string.Empty;


            if (args.Length >= 1)
            {
                for (int i = 0; i < args.Length; i++)
                {
                    // List com port
                    if (args[i].ToUpper() == "--LIST" || args[i].ToUpper() == "-L")
                    {
                        Console.WriteLine("COM Port List :");
                        foreach (string s in SerialPort.GetPortNames())
                        {
                            Console.WriteLine("-> " + s);
                        }
                        return;
                    }
                    if (args[i].ToUpper() == "--PORT" || args[i].ToUpper() == "-P")
                    {
                        portName = args[i + 1];
                        i++;
                        continue;
                    }
                    if (args[i].ToUpper() == "--BAUDRATE" || args[i].ToUpper() == "-B")
                    {
                        baudRate = int.Parse(args[i + 1]);
                        i++;
                        continue;
                    }
                    if (args[i].ToUpper() == "--HEX" || args[i].ToUpper() == "-H")
                    {
                        isHex = true;
                        continue;
                    }
                    if (args[i].ToUpper() == "--LOG")
                    {
                        fileLog = args[i + 1];
                        i++;
                        continue;
                    }
                    if (args[i].ToUpper() == "--NODISPLAY")
                    {
                        isDisplay = false;
                    }
                    
                }
            }
            else
            {
                DisplayHelp();
                return;
            }

            serialPort.PortName = portName;
            serialPort.BaudRate = baudRate;
            Console.WriteLine("Open : " + portName + " baud=" + baudRate + " log=" + fileLog + " display_hex=" + isHex + " is_display=" + isDisplay);
            Console.WriteLine("Press any key to exit!");
            serialPort.DataReceived += SerialPort_DataReceived;
            try
            {
                serialPort.Open();
            }
            catch(Exception e)
            {
                Console.WriteLine("Exception : " + e.ToString()); 
            }

            Console.ReadKey();

            if (serialPort.IsOpen)
            {
                serialPort.Close();
            }
            if (!string.IsNullOrEmpty(fileLog))
            {
                // Reset memory position
                memoryStream.Position = 0;

                Console.WriteLine("Write log to : " + fileLog);
                try
                {
                    File.WriteAllBytes(fileLog, memoryStream.ToArray());
                }
                catch(Exception e)
                {
                    Console.WriteLine("Exception : " + e.ToString());
                }
                
            }



        }
        /// <summary>
        /// Event data received
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void SerialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            byte[] buff = new byte[serialPort.ReadBufferSize];
            int result = serialPort.Read(buff, 0, buff.Length);
            // If display data to console
            if(isDisplay)
            {
                for (int i = 0; i < result; i++)
                {
                    if (isHex)
                        Console.Write(buff[i].ToString("X02"));
                    else
                        Console.Write((char)buff[i]);
                }
            }
            else
            {
                // Otherwise just print "."
                Console.Write(".");
            }
            // Copy data to memory stream
            memoryStream.Write(buff, 0, result);
        }
    }
}
