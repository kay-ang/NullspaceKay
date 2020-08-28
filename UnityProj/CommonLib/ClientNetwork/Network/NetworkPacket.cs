
using System;
using System.IO;

namespace Nullspace
{
    public class NetworkPacket : NetworkMessage
    {
        public static byte[] HeartPacketBytes = null;
        public static int HeadSize = 0;
        static NetworkPacket()
        {
            NetworkPacket packet = ObjectPools.Instance.Acquire<NetworkPacket>();
            packet.CommandID = NetworkCommandDefine.HeartCodec;
            HeartPacketBytes = packet.ToBytes();
            ObjectPools.Instance.Release(packet);

            int size = 0;
            size += sizeof(int);
            size += sizeof(int);
            size += sizeof(int);
            size += sizeof(int);
            size += sizeof(long);
            size += sizeof(long);
            size += sizeof(long);
            size += sizeof(long);
            HeadSize = size;
        }

        public int CommandID = 0;
        public int Length = 0;
        public int Result = 0;
        public int Session = 0;
        public long From = 0;
        public long To = 0;
        public long Mask = 0;
        public long Addition = 0;

        public byte[] BodyContent;

        public virtual void ToHead(byte[] bytes)
        {
            Reset();
            mByteStream.Write(bytes, 0, bytes.Length);
            mByteStream.Seek(0, SeekOrigin.Begin);
            ReadPacket();
        }

        public virtual byte[] ToBytes()
        {
            Reset();
            WritePacket();
            mByteStream.Seek(0, System.IO.SeekOrigin.Begin);
            return ReadBytes((int)mByteStream.Length);
        }

        protected int BodyLength { get { return BodyContent != null ? BodyContent.Length : 0; } }

        protected void ReadPacket()
        {
            CommandID = ReadInt32();
            Length = ReadInt32();
            Result = ReadInt32();
            Session = ReadInt32();

            From = ReadInt64();
            To = ReadInt64();
            Mask = ReadInt64();
            Addition = ReadInt64();

            CommandID = System.Net.IPAddress.NetworkToHostOrder(CommandID);
            Length = System.Net.IPAddress.NetworkToHostOrder(Length);
            Result = System.Net.IPAddress.NetworkToHostOrder(Result);
            From = System.Net.IPAddress.NetworkToHostOrder(From);
            To = System.Net.IPAddress.NetworkToHostOrder(To);
            Session = System.Net.IPAddress.NetworkToHostOrder(Session);
            Addition = System.Net.IPAddress.NetworkToHostOrder(Addition);
            Mask = System.Net.IPAddress.NetworkToHostOrder(Mask);
        }

        protected void WritePacket()
        {
            WriteInt32(System.Net.IPAddress.HostToNetworkOrder(CommandID));
            WriteInt32(System.Net.IPAddress.HostToNetworkOrder(BodyLength));
            WriteInt32(System.Net.IPAddress.HostToNetworkOrder(Result));
            WriteInt32(System.Net.IPAddress.HostToNetworkOrder(Session));

            WriteInt64(System.Net.IPAddress.HostToNetworkOrder(From));
            WriteInt64(System.Net.IPAddress.HostToNetworkOrder(To));
            WriteInt64(System.Net.IPAddress.HostToNetworkOrder(Mask));
            WriteInt64(System.Net.IPAddress.HostToNetworkOrder(Addition));

            WriteBytes(BodyContent);
        }

    }
}


