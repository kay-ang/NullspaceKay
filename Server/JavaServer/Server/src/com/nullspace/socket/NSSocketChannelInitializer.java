package com.nullspace.socket;

import io.netty.channel.ChannelInitializer;
import io.netty.channel.ChannelPipeline;
import io.netty.channel.socket.SocketChannel;

public class NSSocketChannelInitializer extends ChannelInitializer<SocketChannel>
{

	@Override
	protected void initChannel(SocketChannel ch) throws Exception 
	{
		ChannelPipeline pipeline = ch.pipeline();
		// 断包粘包处理
		pipeline.addLast(new NSSocketByteSplitDecoder());
		// 收包解密处理
		pipeline.addLast(new NSSocketByteDecryptDecoder());
		// Message对象转字节流
		pipeline.addLast(new NSSocketMessageToByteEncoder());
		// 加密
		pipeline.addLast(new NSSocketByteEncryptEncoder());
		// 收包handle处理器
		pipeline.addLast(new NSSocketChannelHandler());
	}

}
