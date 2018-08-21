using PcapDotNet.Packets;
using PcapDotNet.Packets.IpV4;
using PcapDotNet.Packets.Transport;
using System;

namespace SeeSharp
{
    class TcpPacketInterpreter : PacketInterpreter
    {    
        public TcpPacketInterpreter(Packet ToInterpret)
        {
            StoredPacket = ToInterpret;
            IpData = StoredPacket.Ethernet.IpV4;

            var temp = StoredPacket.IpV4.Tcp;

            if (temp.IsValid)
                TcpData = temp;
            else
                TcpData = null;
        }

        public IpV4Datagram IpData { get => ipData; set => ipData = value; }
        public TcpDatagram TcpData { get => tcpData; set => tcpData = value; }

        public override string ToString()
        {
            string description = IpPacketInterpreter.ProvideBasicIpDescription(StoredPacket);

            if (TcpData != null)
                description += String.Format("({0},{2})->({1},{3}), Seq#:{4}, Ack#:{5}, {6}{7}{8}{9}{10}{11}",
                                            IpData.Source.ToString(), IpData.Destination.ToString(), TcpData.SourcePort, TcpData.DestinationPort,
                                            TcpData.SequenceNumber, TcpData.AcknowledgmentNumber,
                                            (TcpData.IsUrgent) ? 'U' : '-',
                                            (TcpData.IsAcknowledgment) ? 'A' : '-',
                                            (TcpData.IsPush) ? 'P' : '-',
                                            (TcpData.IsReset) ? 'R' : '-',
                                            (TcpData.IsSynchronize) ? 'S' : '-',
                                            (TcpData.IsFin) ? 'F' : '-');
            else
                description += "Invalid TCP data formatting.";
            return description;
        }

        public override string[] ToStringArray()
        {
            throw new NotImplementedException();
        }

        private IpV4Datagram ipData;
        private TcpDatagram tcpData;
    }
}
