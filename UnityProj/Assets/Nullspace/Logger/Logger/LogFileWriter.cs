using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;

namespace Nullspace
{
    public class LogFileWriter
    {
        private List<string> mCacheMessages = new List<string>();
        private List<string> mBackMessages = new List<string>();

        private LoggerConfig mConfig;
        private string mDayH;
        private string mFilePath;
        private StreamWriter mFileStream;
        private Thread mThread = null;
        private bool isStopped = false;

        public LogFileWriter(LoggerConfig config)
        {
            mConfig = config;
            LogToNextFile();
            StartLoging();
        }
        public void Log(string message)
        {
            lock (this)
            {
                mCacheMessages.Add(message);
            }
        }

        public void Stop()
        {
            isStopped = true;
        }

        private void StartLoging()
        {
            if (mThread == null)
            {
                mThread = new Thread(Check);
                mThread.Start();
            }
        }

        private void Check()
        {
            while (true)
            {
                if (mCacheMessages.Count > 0)
                {
                    lock (this)
                    {
                        mBackMessages.AddRange(mCacheMessages);
                        mCacheMessages.Clear();
                    }
                    int count = mBackMessages.Count;
                    for (int i = 0; i < count; ++i)
                    {
                        if (IsSameDayH(mBackMessages[i]))
                        {
                            mFileStream.WriteLine(mBackMessages[i]);
                        }
                        else
                        {
                            LogToNextFile();
                        }
                    }
                    mBackMessages.Clear();
                    mFileStream.Flush();
                }
                if (isStopped)
                {
                    mFileStream.Close();
                    break;
                }
                else
                {
                    Thread.Sleep(mConfig.FlushInterval);
                }
            }
        }

        private bool IsSameDayH(string msg)
        {
            string dayH = StringUtils.StrTok(msg, ":");
            if (dayH != null)
            {
                return dayH == mDayH;
            }
            return false;
        }

        private string GetFilePath()
        {
            return string.Format("{0}/{1}_{2}.{3}", mConfig.Directory, mConfig.FileName, mDayH, mConfig.FileExtention);
        }

        private void LogToNextFile()
        {
            mDayH = DateTimeUtils.FormatTimeH(DateTime.Now);
            mFilePath = GetFilePath();
            if (mFileStream != null)
            {
                mFileStream.Close();
            }
            mFileStream = new StreamWriter(mFilePath, true, Encoding.Unicode);
            mFileStream.AutoFlush = false;
        }
    }

}
