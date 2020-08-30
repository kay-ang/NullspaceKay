package com.nullspace.file;

import java.io.BufferedInputStream;
import java.io.FileInputStream;
import java.io.FileNotFoundException;
import java.io.IOException;
import java.io.InputStream;
import java.io.UnsupportedEncodingException;
import java.util.Properties;
import java.util.concurrent.locks.ReadWriteLock;
import java.util.concurrent.locks.ReentrantReadWriteLock;

/**
 * Properties文件路径
 * 读取配置信息
 * @author kay.yang
 *
 */
public class NSPathConfig
{
    public static Properties mProperties = null;
    private static ReadWriteLock mLock = new ReentrantReadWriteLock(true);
    private static String mPath;

    public static boolean initConfig(String path)
    {
        if (path == null || path.equals(""))
        {
            return false;
        }

        mPath = path;
        if (mProperties == null)
        {
            return loadProperties(path);
        }
        return true;
    }

    public static boolean refreshProperties()
    {
        if (mPath == null || mPath.isEmpty())
            return false;

        try
        {
            Properties temp = new Properties();
            InputStream inputStream = new BufferedInputStream(
                    new FileInputStream(mPath));
            temp.load(inputStream);

            mLock.writeLock().lock();
            mProperties = temp;
        }
        catch (FileNotFoundException e)
        {
            return false;
        }
        catch (IOException e)
        {
            return false;
        }
        finally
        {
            mLock.writeLock().unlock();
        }
        return true;
    }

    private static boolean loadProperties(String path)
    {
        try
        {
            mLock.writeLock().lock();
            mProperties = new Properties();
            InputStream inputStream = new BufferedInputStream(
                    new FileInputStream(path));
            mProperties.load(inputStream);
            return true;
        }
        catch (FileNotFoundException e)
        {
            return false;
        }
        catch (IOException e)
        {
            return false;
        }
        finally
        {
            mLock.writeLock().unlock();
        }
    }

    public static String getValue(String key)
    {
        if (mProperties == null)
        {
            return null;
        }
        String value = mProperties.getProperty(key);
        if (null == value)
            return null;
        try
        {
            return new String(value.getBytes(), "utf-8");
        }
        catch (UnsupportedEncodingException e)
        {
            return null;
        }
    }

    public static Object setValue(String key, String value)
    {
        return mProperties.put(key, value);
    }

    public static boolean checkValue(String key)
    {
        String ips = NSPathConfig.getValue("AdminIP");
        if (ips != null && key != null && !key.isEmpty())
        {
            for (String s : ips.split("\\|"))
            {
                if (s.equals(key))
                    return true;
            }
        }
        return false;
    }
}
