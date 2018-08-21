using PcapDotNet.Packets;
using PcapDotNet.Packets.IpV4;
using System;

namespace SeeSharp
{
    abstract class PacketInterpreter
    {
        public Packet StoredPacket { get => storedPacket; set => storedPacket = value; }

        public static PacketInterpreter CreatePacketInterpreter(Packet incomingPacket)
        {

            // ensure validity of packet
            if (incomingPacket.IsValid)
            {
                var ether = incomingPacket.Ethernet;

                // determine what next-level layer the packet was transmitted as
                switch (ether.EtherType)
                {
                    case PcapDotNet.Packets.Ethernet.EthernetType.IpV4:
                        {
                            switch (ether.IpV4.Protocol)
                            {
                                // If TCP or UDP, fit the packet data to the correct datagram format
                                case IpV4Protocol.Tcp:
                                    return new TcpPacketInterpreter(incomingPacket);
                                case IpV4Protocol.Udp:
                                    return new UdpPacketInterpreter(incomingPacket);
                                // further IP protocols would be checked for and implemented here in their own case statement
                                default:
                                    return new IpPacketInterpreter(incomingPacket);                                   
                            }
                        }
                        
                }
            }
            return null;
        }

        public abstract override String ToString();
        public abstract String[] ToStringArray();

        private Packet storedPacket;
    }
}
