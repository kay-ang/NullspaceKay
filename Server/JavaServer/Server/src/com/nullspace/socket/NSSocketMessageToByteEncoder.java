package com.nullspace.socket;

import com.nullspace.packet.NSMessage;

import io.netty.buffer.ByteBuf;
import io.netty.channel.ChannelHandlerContext;
import io.netty.handler.codec.MessageToByteEncoder;

public class NSSocketMessageToByteEncoder extends MessageToByteEncoder<NSMessage>
{
	@Override
	protected void encode(ChannelHandlerContext ctx, NSMessage msg, ByteBuf out) throws Exception 
	{
		out = msg.GetByteBuf();
		ctx.writeAndFlush(out);
		out.release();
	}

}
