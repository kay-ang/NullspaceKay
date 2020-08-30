package com.nullspace.packet;

import com.nullspace.utils.NSByteBufHelper;

import io.netty.buffer.ByteBuf;

public class NSMessage 
{
	public NSMessageHead mHead;
	public byte[] mContent;
	
	public NSMessage()
	{
		mHead = new NSMessageHead();
		mContent = new byte[NSMessageHead.Size()];
	}
	
	public NSMessage(NSMessageHead head, byte[] content)
	{
		mHead = head;
		mContent = content;
	}
	
	public ByteBuf GetByteBuf()
	{
		ByteBuf buf = NSByteBufHelper.WriteHead(mHead);		
		if (mContent != null && mContent.length > 0)
		{
			buf.writeBytes(mContent);
		}
		return buf;
	}
	
	public void Encrypt()
	{
		// to do
	}
	
}
