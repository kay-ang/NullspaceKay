package com.nullspace.socket;

import java.util.List;

import com.nullspace.packet.NSMessage;

import io.netty.channel.ChannelHandlerContext;
import io.netty.handler.codec.MessageToMessageDecoder;

public class NSSocketByteDecryptDecoder extends MessageToMessageDecoder<NSMessage>
{
	@Override
	protected void decode(ChannelHandlerContext ctx, NSMessage msg, List<Object> out) throws Exception 
	{
		out.add(msg);
	}
	
}
