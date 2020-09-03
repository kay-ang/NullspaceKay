
using System;
using System.IO;
using System.Text;

namespace Nullspace
{
    public partial class NullMemoryStream
    {
        public bool Eof()
        {
            return mTextReadStream.EndOfStream;
        }

        public int Read()
        {
            return mTextReadStream.Read();
        }

        public int Peek()
        {
            return mTextReadStream.Peek();
        }

        public string ReadLine()
        {
            return mTextReadStream.ReadLine();
        }

        public int Read(char[] buffer, int index, int count)
        {
            return mTextReadStream.Read(buffer, index, count);
        }

        public void Write(char[] buffer)
        {
            mTextWriteStream.Write(buffer);
        }

        public void Write(string value)
        {
            mTextWriteStream.Write(value);
        }

        public void Write(char value)
        {
            mTextWriteStream.Write(value);
        }

        public void Write(char[] buffer, int index, int count)
        {
            mTextWriteStream.Write(buffer, index, count);
        }
    }

    public partial class NullMemoryStream
    {
        public static NullMemoryStream ReadFromFile(string path)
        {
            FileStream fileStream = new FileStream(path, FileMode.Open, FileAccess.Read);
            NullMemoryStream stream = new NullMemoryStream(fileStream);
            return stream;
        }

        public static NullMemoryStream ReadFromBytes(byte[] bytes)
        {
            MemoryStream memoryStream = new MemoryStream(bytes);
            NullMemoryStream stream = new NullMemoryStream(memoryStream);
            return stream;
        }

        public static NullMemoryStream ReadAndWrite()
        {
            MemoryStream memoryStream = new MemoryStream();
            NullMemoryStream stream = new NullMemoryStream(memoryStream);
            return stream;
        }

        public static NullMemoryStream WriteToFile(string path)
        {
            FileStream fileStream = new FileStream(path, FileMode.OpenOrCreate, FileAccess.Write);
            NullMemoryStream stream = new NullMemoryStream(fileStream);
            return stream;
        }

        public static NullMemoryStream ReadTextFromFile(string path)
        {
            StreamReader fileStream = new StreamReader(path, Encoding.UTF8);
            return new NullMemoryStream(fileStream);
        }

        public static NullMemoryStream ReadTextFromString(string content)
        {
            MemoryStream stream = new MemoryStream(Encoding.UTF8.GetBytes(content));
            StreamReader strStream = new StreamReader(stream, Encoding.UTF8);
            return new NullMemoryStream(strStream);
        }

        public static NullMemoryStream WriteTextFromFile(string path, bool append)
        {
            StreamWriter fileStream = new StreamWriter(path, append, Encoding.UTF8);
            return new NullMemoryStream(fileStream);
        }

    }

    public partial class NullMemoryStream : IDisposable
    {
        private Stream mStream;
        private StreamReader mTextReadStream;
        private StreamWriter mTextWriteStream;

        private NullMemoryStream(Stream stream)
        {
            mStream = stream;
        }

        private NullMemoryStream(StreamWriter stream)
        {
            mTextWriteStream = stream;
        }

        private NullMemoryStream(StreamReader stream)
        {
            mTextReadStream = stream;
        }

        public void Dispose()
        {
            if (mStream != null)
            {
                mStream.Close();
                mStream = null;
            }
            if (mTextReadStream != null)
            {
                mTextReadStream.Close();
                mTextReadStream = null;
            }
            if (mTextWriteStream != null)
            {
                mTextWriteStream.Close();
                mTextWriteStream = null;
            }
        }

        public bool CanRead(int size)
        {
            return mStream.Position + size <= mStream.Length;
        }

        public int Write(byte[] bytes, int offset, int count)
        {
            mStream.Write(bytes, offset, count);
            return count;
        }
        public int Read(byte[] bytes, int offset, int count)
        {
            mStream.Read(bytes, offset, count);
            return count;
        }
    }

}
