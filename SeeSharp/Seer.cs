using PcapDotNet.Core;
using PcapDotNet.Core.Extensions;
using PcapDotNet.Packets;
using PcapDotNet.Packets.IpV4;
using PcapDotNet.Packets.Transport;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SeeSharp.Core
{
    class Seer
    {
        /// <summary>
        /// Number of Bytes for the Seer to read by default
        /// </summary>
        private const int DEFAULT_SNAPSHOT = 65365;

        /// <summary>
        /// Milliseconds that the Seer should wait for a packet before timing out
        /// </summary>
        private const int DEFAULT_READ_TIMEOUT = 1000;

        private const string DEFAULT_FILTER = "ip";

        private const string ValidFilterRegex = @"[a-zA-z0-9.-]";

        public Seer()
        {
            DeviceForUse = null;
            Communicator = null;
            SnapshotLength = DEFAULT_SNAPSHOT;
            ReadTimeout = DEFAULT_READ_TIMEOUT;
            CaptureFilter = DEFAULT_FILTER;
        }

        public PacketCommunicator CreateLivePacketCommunicator() =>
            DeviceForUse.Open(SnapshotLength, PacketDeviceOpenAttributes.Promiscuous, ReadTimeout);

        public void CaptureLivePackets()
        {
            using (Communicator = CreateLivePacketCommunicator())
            {
                Communicator.SetFilter(CaptureFilter);
                do
                {
                    PacketCommunicatorReceiveResult result = Communicator.ReceivePacket(out Packet incomingPacket);
                    switch (result)
                    {
                        case PacketCommunicatorReceiveResult.Timeout:
                            continue;
                        case PacketCommunicatorReceiveResult.Ok:
                            PacketInterpreter interpreter = PacketInterpreter.CreatePacketInterpreter(incomingPacket);
                            Console.WriteLine(interpreter.ToString());
                            break;
                        default:
                            Console.WriteLine("Error. Unable to determine reason.");
                            break;

                    }
                } while (true);
            };
        }

        /// <summary>
        /// Selects the Network Interface associated with inputted index  
        /// </summary>
        /// <param name="ndx">
        /// 1-based index corresponding to network interface
        /// </param>
        public LivePacketDevice SelectPacketDevice(int ndx = -1)
        {
            //Console.WriteLine("Index Received by SelectPacketDevice "+ndx);

            // get a list of network interfaces on the machine
            var localAdapters = LivePacketDevice.AllLocalMachine;

            // ensure that the inputted index corresponds to a potential interface position
            while (!(0 < ndx && ndx <= localAdapters.Count))
            {
                // If an invalid index was input,

                // List a description of each available interface
                var descripts = ListLocalAdapters(localAdapters);

                // print out the descriptions
                foreach(string des in descripts)
                {
                    Console.WriteLine(des);
                }

                // Read user's input
                Console.WriteLine("Enter desired interface's number.");
                string index = Console.ReadLine();

                // Attempt to parse the input as an integer
                bool wasInteger = Int32.TryParse(index, out int check);
                if (wasInteger) ndx = check;
            }

            Console.WriteLine(ProvideCaptureDeviceDescription(localAdapters[ndx-1]));

            return localAdapters[ndx-1];
        }

        /// <summary>
        /// Provides an array of strings containing an interface's position and description
        /// </summary>
        /// <param name="localAdapters"></param>
        /// <returns></returns>
        private string[] ListLocalAdapters(IReadOnlyCollection<PacketDevice> localAdapters)
        {           
            // Verify that a populated list was passed to the system
            if (localAdapters.Count > 0)
            {
                // initiate an array long enough for the number of adapters on the system
                string[] descripts = new string[localAdapters.Count];

                // 1-based index for listing purposes
                int index = 0;

                // Add the descriptions to the array
                foreach (LivePacketDevice dev in localAdapters)
                {
                    //r nic = LivePacketDeviceExtensions.GetNetworkInterface(dev);

                    descripts[index] = $"{++index}: {ProvideCaptureDeviceDescription(dev)}";
                }

                return descripts;
            }
            return null;
        }

        /// <summary>
        /// Provides the description from the system for the inputted device
        /// </summary>
        /// <param name="device"></param>
        /// <returns></returns>
        public String ProvideCaptureDeviceDescription(LivePacketDevice device = null)
        {
            // if the device is valid
            if (device != null)           
                return LivePacketDeviceExtensions.GetNetworkInterface((LivePacketDevice)device).Description;

            return "No available Description.";
        }

        public static bool DetermineFilterInputHasValidCharacters(string input)
        {

            bool ValidCharacters = true;
            if (ValidCharacters) {
                Regex reg = new Regex(ValidFilterRegex, RegexOptions.IgnoreCase & RegexOptions.CultureInvariant & RegexOptions.IgnorePatternWhitespace);
                var strings = input.Substring(0, input.Length).Split(' ');
                for (int i = 0; i < strings.Length; i++)
                {
                    string s = strings[i];
                    ValidCharacters &= (reg.IsMatch(s)) ? true : false;
                    //Console.WriteLine($"{s} is valid? {ValidCharacters}");
                }
            }
           // Console.WriteLine(ValidCharacters);
            return ValidCharacters;
        }

        public LivePacketDevice DeviceForUse { get; set; }
        public string Filter { get; set; }
        public PacketCommunicator Communicator { get; set; }
        public int SnapshotLength {get; set;}
        public int ReadTimeout { get; set; }
        public string CaptureFilter { get; set; }
    }
}
