package com.nullspace.utils;

import java.nio.charset.Charset;

import com.nullspace.packet.NSMessage;
import com.nullspace.packet.NSMessageHead;

import io.netty.buffer.ByteBuf;
import io.netty.buffer.PooledByteBufAllocator;

public class NSByteBufHelper 
{

	public static void writeString(ByteBuf src, String value)
	{
		try
		{
			if (value == null)
			{
				value = "";
			}
			byte[] bytes = value.getBytes(Charset.forName("UTF-8"));
			src.writeInt(bytes.length);
			src.writeBytes(bytes);
		}
		catch (Exception e)
		{
			e.printStackTrace();
		}
	}
	
	public static String readString(ByteBuf src) throws Exception
	{
		try
		{
			if (CanRead(src, 4))
			{
				int len = src.readInt();
				if (CanRead(src, len))
				{
					byte[] dst = new byte[len];
					src.readBytes(dst);
					return new String(dst, Charset.forName("UTF-8"));
				}
			}
		} 
		catch (Exception e) 
		{
			e.printStackTrace();
		}
		return null;
	}
	
	public static void writeBytes(ByteBuf src, byte[] bytes)
	{
		src.writeBytes(bytes);
	}
	
	public static byte[] readBytes(ByteBuf src, int len)
	{
		if (CanRead(src, len))
		{
			byte[] res = new byte[len];
			src.readBytes(res);
			return res;
		}
		return null;
	}
	
	public static void writeBoolean(ByteBuf src, boolean bl)
	{
		int is = bl == true ? 1 : 0;
		src.writeByte(is);
	}
	
	public static boolean readBoolean(ByteBuf src)
	{
		if (CanRead(src, 1))
		{
			return (src.readByte() == 1 ? true : false);
		}
		return false;
	}
	
	public static boolean CanRead(ByteBuf src, int readLength)
	{
		int readable = src.readableBytes();
		return readable >= readLength && readable != 0;
	}
	
	public static boolean ReadHead(ByteBuf src, NSMessageHead head)
	{
		if (CanRead(src, NSMessageHead.Size()))
		{
			head.mType = src.readInt();
			head.mLength = src.readInt();
			head.mResult = src.readInt();
			head.mSession = src.readInt();
			
			head.mFrom = src.readLong();
			head.mTo = src.readLong();
			head.mMask = src.readLong();
			head.mAddition = src.readLong();
			return true;
		}
		return false;
	}
	
	public static ByteBuf GetByteBuf(NSMessage msg)
	{
		ByteBuf buf = WriteHead(msg.mHead);
		buf.writeBytes(msg.mContent);
		return buf;
	}
	
	public static ByteBuf WriteHead(NSMessageHead head)
	{
		ByteBuf buf = PooledByteBufAllocator.DEFAULT.directBuffer(NSMessageHead.Size());	
		buf.writeInt(head.mType);
		buf.writeInt(head.mLength);
		buf.writeInt(head.mResult);
		buf.writeInt(head.mSession);
		buf.writeLong(head.mFrom);
		buf.writeLong(head.mTo);
		buf.writeLong(head.mMask);
		buf.writeLong(head.mAddition);
		return buf;
	}
}
