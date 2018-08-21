using PcapDotNet.Packets;
using PcapDotNet.Packets.IpV4;
using PcapDotNet.Packets.Transport;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SeeSharp
{
    class UdpPacketInterpreter : PacketInterpreter
    {
       

        public UdpPacketInterpreter(Packet ToInterpret)
        {
            StoredPacket = ToInterpret;
            IpData = StoredPacket.Ethernet.IpV4;

            if (IpData.Udp.IsValid)
                UdpData = IpData.Udp;
            else
                UdpData = null;
        }

        public IpV4Datagram IpData { get => ipData; set => ipData = value; }
        public UdpDatagram UdpData { get => udpData; set => udpData = value; }

        public override string ToString()
        {
            string description = IpPacketInterpreter.ProvideBasicIpDescription(StoredPacket);

            if (UdpData != null)
            {
                description += String.Format("({0},{2})->({1},{3})",
                                              IpData.Source.ToString(),
                                              IpData.Destination.ToString(),
                                              UdpData.SourcePort,
                                              UdpData.DestinationPort);
            }

            else
                description += "Invalid UDP data formatting";

            return description;

        }

        public override string[] ToStringArray()
        {
            throw new NotImplementedException();
        }

        private IpV4Datagram ipData;
        private UdpDatagram udpData;
    }
}
