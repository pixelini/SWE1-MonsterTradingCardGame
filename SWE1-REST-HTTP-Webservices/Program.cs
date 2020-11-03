using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Net.WebSockets;
using System.Text;

namespace SWE1_REST_HTTP_Webservices
{
    class Program
    {

        static void Main(string[] args)
        {


            // Create a new Listening Socket
            TcpListener listeningSocket = new TcpListener(IPAddress.Loopback, 6542); // loopback ist 127.0.0.1

            // Start listening
            listeningSocket.Start();
            TcpClient connectionSocket = listeningSocket.AcceptTcpClient();
            Console.WriteLine("Connected!");

            NetworkStream dataStream = connectionSocket.GetStream();

            // Buffer for reading
            byte[] buffer = new byte[2000];
            string input = null;
                
              



            var sw = new StreamWriter(dataStream);
            sw.Write("Halloooo!");

            // intput = Encoding.ASCII.GetString(buffer, 0, length); // byte array zu string zurückverwandeln



            var sr = new StreamReader(dataStream);
            while (!sr.EndOfStream)
            {
                var line = sr.ReadLine();
                Console.WriteLine(line);
            }










            /*            try
                        {
                            var serverSock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                            serverSock.Bind(new IPEndPoint(IPAddress.Loopback, 10101)); // loopback ist 127.0.0.1
                            serverSock.Listen(5);

                            var clientSock = serverSock.Accept();
                            clientSock.Send(
                                Encoding.ASCII
                                    .GetBytes(
                                        "Hello World")); // nicht notwendig auf byte order achten --> strings werden als byte array geschickt


                            // receiving data
                            byte[] bufferAsByteArray = new byte[2000]; // size?
                            int length = clientSock.Receive(bufferAsByteArray);
                            string input =
                                Encoding.ASCII.GetString(bufferAsByteArray, 0, length); // byte array zu string zurückverwandeln

                            Console.WriteLine(serverSock);

                        }
                        catch (SocketException ex)
                        {
                            Console.WriteLine("A socket error occurred!");
                            Console.WriteLine(ex.Message);
                            return;
                        }
                        catch (ArgumentNullException ex)
                        {
                            Console.WriteLine("Binding to socket failed! Make sure that you enter a valid IP and port.");
                            Console.WriteLine(ex.Message);
                            return;
                        }
                        catch (ArgumentOutOfRangeException ex)
                        {
                            Console.WriteLine("Binding to socket failed! Make sure that you enter a valid IP and port.");
                            Console.WriteLine(ex.Message);
                            return;
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine("An error occurred!");
                            Console.WriteLine(ex.Message);
                            return;
                        }



                    }

                    public static void Read()
                    {
                        var s = new TcpClient(); // eine ebene höher (nicht mehr auf byte ebene)
                        s.Connect("technikum-wien.at", 80);
                        var stream = s.GetStream();
                        var sr = new StreamReader(stream);
                        while (!sr.EndOfStream)
                        {
                            var line = sr.ReadLine();
                            Console.WriteLine(line);
                        }
                    */
        }
    }

}
