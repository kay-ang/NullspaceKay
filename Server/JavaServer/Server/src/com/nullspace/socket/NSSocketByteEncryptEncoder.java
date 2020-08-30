package com.nullspace.socket;

import java.util.List;

import com.nullspace.packet.NSMessage;

import io.netty.channel.ChannelHandlerContext;

import io.netty.handler.codec.MessageToMessageEncoder;

public class NSSocketByteEncryptEncoder extends MessageToMessageEncoder<NSMessage>{

	@Override
	protected void encode(ChannelHandlerContext ctx, NSMessage msg, List<Object> out) throws Exception 
	{
		msg.Encrypt();
		out.add(msg);
	}

}
