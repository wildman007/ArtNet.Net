using System;
using System.Net;
using ArtNet.Sockets;
using ArtNet.Packets;
using System.Threading.Tasks;
using System.Timers;

namespace Sample
{
    class MainClass
    {
        public const uint SENDER_FPS = 60; // iterations per second
        public const uint SENDER_UNIVERSES = 2; // number of universes to send
        public const uint SENDER_LENGTH = 512; // number of bytes to send
        public static byte[] s_dmxData = new byte[511];
        public static ArtNetSocket s_artnet = new ArtNetSocket();
        public static int counter = 0;
        public static void Main(string[] args)
        {

            s_artnet.Open(IPAddress.Parse("192.168.0.1"), IPAddress.Parse("255.255.255.0"));
            s_artnet.EnableBroadcast = true;
            Console.WriteLine("Broadcast address: " + s_artnet.BroadcastAddress.ToString() + "\nAt FPS=" + SENDER_FPS + "\n");

            Timer timer = new Timer(1000 / SENDER_FPS);
            timer.Elapsed += SendArtNet2;
            timer.Start();
            Console.ReadLine();
        }

        // We send RGB values (1 byte/channel) via Art-Net.
        private static void SendArtNet(Object source, ElapsedEventArgs e)
        {
            ArtNetDmxPacket toSend = new ArtNetDmxPacket();
            Random rnd = new Random();

            toSend.DmxData = new byte[SENDER_LENGTH];
            for (uint universe = 0; universe < SENDER_UNIVERSES; universe++)
            {
                toSend.Universe = (short)universe;
                for (uint i = 0; i < SENDER_LENGTH; i++)
                {
                    if (i % 2 == 0)
                    {
                        var phase = 2 * Math.PI * i / SENDER_LENGTH; // radians
                        uint redValue = (uint)(Math.Cos(DateTime.Now.Millisecond + phase) * 255.0); // add phase based on i index
                        toSend.DmxData[i] = (byte)redValue; //(byte)rnd.Next(0, 255);
                    }
                    else
                    {
                        toSend.DmxData[i] = 0;
                    }
                }
                s_artnet.Send(toSend);
                Console.WriteLine("Sending [" + toSend.DmxData.Length + "] Art-Net values data on universe [" + toSend.Universe + "]");
            }
        }

        // We send RGB values (1 byte/channel) via Art-Net.
        private static void SendArtNet2(Object source, ElapsedEventArgs e)
        {
            ArtNetDmxPacket toSend = new ArtNetDmxPacket();

            toSend.DmxData = new byte[SENDER_LENGTH];
            for (uint universe = 0; universe < SENDER_UNIVERSES; universe++)
            {
                toSend.Universe = (short)universe;
                for (uint i = 0; i < 6; i++)
                {
                        toSend.DmxData[i] = (byte)counter; //(byte)rnd.Next(0, 255);
                }
                s_artnet.Send(toSend);
                counter++;
                if (counter > 255)
                {
                    counter = 0;
                }
            }
        }

    }
}