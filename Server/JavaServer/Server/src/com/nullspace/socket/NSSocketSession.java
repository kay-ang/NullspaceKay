package com.nullspace.socket;

import org.slf4j.Logger;
import org.slf4j.LoggerFactory;

import com.nullspace.packet.NSMessage;
import io.netty.channel.Channel;
import io.netty.channel.ChannelFuture;
import io.netty.channel.ChannelFutureListener;

public class NSSocketSession 
{
	private static Logger logger = LoggerFactory.getLogger("MJSocketSession");
	private Channel mChannel;
	private int mID;
	private String mClientIP;
	private int mPort;
	
	public NSSocketSession()
	{
		
	}
	
	public void Channel(Channel channel)
	{
		mChannel = channel;
	}
	
	public Channel Channel()
	{
		return mChannel;
	}

	public int id() {
		return mID;
	}

	public void id(int id) {
		this.mID = id;
	}
	
	public String IP()
	{
		return mClientIP;
	}
	
	public void IP(String ip)
	{
		mClientIP = ip;
	}
	
	public int port()
	{
		return mPort;
	}
	
	public void port(int port)
	{
		mPort = port;
	}
	
	public void WriteMessage(final NSMessage message, ChannelFutureListener future)
	{
		if (mChannel != null && mChannel.isActive())
		{
			ChannelFuture res = null;
			synchronized (mChannel)
			{
				res = mChannel.writeAndFlush(message);
			}
			if (future != null && res != null)
			{
				res.addListener(future);
			}
		}
	}

	public void Close()
	{
		if (mChannel != null && mChannel.isOpen())
		{
			mChannel.close()/*.addListener(new ChannelFutureListener() 
			{
				@Override
				public void operationComplete(ChannelFuture future) throws Exception
				{
					logger.info("用户  {} 断开连接", mClientIP);
				}
			})*/;
		}
	}
}
