using PcapDotNet.Packets;
using PcapDotNet.Packets.IpV4;
using System;

namespace SeeSharp
{
    class IpPacketInterpreter : PacketInterpreter
    {
        
        public IpPacketInterpreter(Packet ToInterpret)
        {
            StoredPacket = ToInterpret;

            if (ToInterpret.IpV4.IsValid)
                IpData = ToInterpret.Ethernet.IpV4;
            else
                IpData = null;
        }

        public IpV4Datagram IpData { get => ipData; set => ipData = value; }

        public override string ToString()
        {
            String description = ProvideBasicIpDescription(StoredPacket);

            if (IpData != null)
                description += String.Format("{0}->{1}", IpData.Source.ToString(), IpData.Destination.ToString());
            else
                description += "Invalid IP formatting.";

            return description;
        }

        public static String ProvideBasicIpDescription(Packet packet)
        {
            return (packet.IsValid) ? String.Format("{0}:{1}, TTL:{2}, Length:{3}, ",
                                                              packet.Ethernet.IpV4.Protocol.ToString(),
                                                              packet.Timestamp.ToShortTimeString(),
                                                              packet.Ethernet.IpV4.Ttl,
                                                              packet.Length) : "";

        }

        public override string[] ToStringArray()
        {
            throw new NotImplementedException();
        }

        private IpV4Datagram ipData;
    }
}
