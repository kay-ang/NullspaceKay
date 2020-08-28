using System;
using System.IO;
using System.Text;

namespace Nullspace
{
    public class NetworkMessage : ObjectBase
    {
        public static byte[] CacheBytes = new byte[1024];
        protected MemoryStream mByteStream;
        public NetworkMessage()
        {
            mByteStream = new MemoryStream();
            Reset();
        }

        protected override void Acquire()
        {
            Reset();
        }

        protected override void Release()
        {
            Reset();
        }

        public override void Destroy()
        {
            Reset();
            mByteStream.Close();
        }

        public void Reset()
        {
            mByteStream.SetLength(0);
            mByteStream.Seek(0, SeekOrigin.Begin);
        }

        public void WriteLength()
        {
            byte[] size = BitConverter.GetBytes(mByteStream.Length);
            mByteStream.Seek(0, SeekOrigin.Begin);
            mByteStream.Write(size, 0, size.Length);
        }

        public short ReadInt16()
        {
            if (CanRead(sizeof(short)))
            {
                mByteStream.Read(CacheBytes, 0, sizeof(short));
                return BitConverter.ToInt16(CacheBytes, 0);
            }
            return short.MaxValue;
        }

        public ushort ReadUInt16(byte[] buffers, int offset, ref int position)
        {
            if (CanRead(sizeof(ushort)))
            {
                mByteStream.Read(CacheBytes, 0, sizeof(short));
                return BitConverter.ToUInt16(CacheBytes, 0);
            }
            return ushort.MaxValue;
        }

        public void WriteUInt16(ushort value)
        {
            byte[] bytes = BitConverter.GetBytes(value);
            mByteStream.Write(bytes, 0, bytes.Length);
        }

        public void WriteInt16(short value)
        {
            byte[] bytes = BitConverter.GetBytes(value);
            mByteStream.Write(bytes, 0, bytes.Length);
        }

        public int ReadInt32()
        {
            if (CanRead(sizeof(int)))
            {
                mByteStream.Read(CacheBytes, 0, sizeof(int));
                return BitConverter.ToInt32(CacheBytes, 0);
            }
            return int.MaxValue;
        }

        public void WriteInt32(int value)
        {
            byte[] bytes = BitConverter.GetBytes(value);
            mByteStream.Write(bytes, 0, bytes.Length);
        }

        public long ReadInt64()
        {
            if (CanRead(sizeof(long)))
            {
                mByteStream.Read(CacheBytes, 0, sizeof(long));
                return BitConverter.ToInt64(CacheBytes, 0);
            }
            return long.MaxValue;
        }

        public void WriteInt64(long value)
        {
            byte[] bytes = BitConverter.GetBytes(value);
            mByteStream.Write(bytes, 0, bytes.Length);
        }

        public void WriteBoolean(bool value)
        {
            byte b = (byte)(value ? 1 : 0);
            mByteStream.WriteByte(b);
        }

        public bool ReadBoolean()
        {
            if (CanRead(sizeof(bool)))
            {
                mByteStream.Read(CacheBytes, 0, sizeof(bool));
                return BitConverter.ToBoolean(CacheBytes, 0);
            }
            return false;
        }

        public float ReadFloat()
        {
            if (CanRead(sizeof(float)))
            {
                mByteStream.Read(CacheBytes, 0, sizeof(float));
                return BitConverter.ToSingle(CacheBytes, 0);
            }
            return float.MaxValue;
        }

        public void WriteFloat(float value)
        {
            byte[] bytes = BitConverter.GetBytes(value);
            mByteStream.Write(bytes, 0, bytes.Length);
        }

        public double ReadDouble()
        {
            if (CanRead(sizeof(double)))
            {
                mByteStream.Read(CacheBytes, 0, sizeof(double));
                return BitConverter.ToDouble(CacheBytes, 0);
            }
            return double.MaxValue;
        }

        public void WriteDouble(double value)
        {
            byte[] bytes = BitConverter.GetBytes(value);
            mByteStream.Write(bytes, 0, bytes.Length);
        }

        public string ReadWString()
        {
            int len = ReadInt32();
            if (len != int.MaxValue)
            {
                byte[] bytes = ReadBytes(len);
                if (bytes != null)
                {
                    return Encoding.Unicode.GetString(bytes);
                }
            }
            return null;
        }

        public string ReadString()
        {
            int len = ReadInt32();
            if (len != int.MaxValue)
            {
                byte[] bytes = ReadBytes(len);
                if (bytes != null)
                {
                    return Encoding.UTF8.GetString(bytes);
                }
            }
            return null;
        }

        public byte[] ReadBytes(int len)
        {
            if (CanRead(len))
            {
                byte[] bytes = new byte[len];
                mByteStream.Read(bytes, 0, len);
                return bytes;
            }
            return null;
        }

        public void WriteBytes(byte[] bytes)
        {
            if (bytes != null)
            {
                mByteStream.Write(bytes, 0, bytes.Length);
            }
        }

        public void WriteString(string value)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(value);
            WriteInt32(bytes.Length);
            WriteBytes(bytes);
        }

        public void WriteWString(string value)
        {
            byte[] bytes = Encoding.Unicode.GetBytes(value);
            WriteInt32(bytes.Length);
            WriteBytes(bytes);
        }

        public bool CanRead(int len)
        {
            if (mByteStream.Position + len <= mByteStream.Length)
            {
                return true;
            }
            return false;
        }

    }
}



