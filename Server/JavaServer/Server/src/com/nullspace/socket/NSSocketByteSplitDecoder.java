package com.nullspace.socket;

import java.util.List;

import com.nullspace.packet.NSMessage;
import com.nullspace.packet.NSMessageHead;
import com.nullspace.utils.NSByteBufHelper;

import io.netty.buffer.ByteBuf;
import io.netty.channel.ChannelHandlerContext;
import io.netty.handler.codec.ByteToMessageDecoder;

public class NSSocketByteSplitDecoder  extends ByteToMessageDecoder
{

	@Override
	protected void decode(ChannelHandlerContext ctx, ByteBuf in, List<Object> out) throws Exception 
	{
		ByteBuf remain = ctx.channel().attr(NSSocketChannelKey.SESSIONBUF).get();
		remain.writeBytes(in);
		NSMessageHead head = ctx.channel().attr(NSSocketChannelKey.HEAD).get();
		while (true)
		{
			if (!ctx.channel().attr(NSSocketChannelKey.IsSharedHeadFilled).get())
			{
				boolean success = NSByteBufHelper.ReadHead(remain, head);
				if (!success)
				{
					break;
				}
				ctx.channel().attr(NSSocketChannelKey.IsSharedHeadFilled).set(true);
			}
			if (ctx.channel().attr(NSSocketChannelKey.IsSharedHeadFilled).get())
			{
				if (head.mLength == 0)
				{
					out.add(new NSMessage(head.Clone(), null));
					ctx.channel().attr(NSSocketChannelKey.IsSharedHeadFilled).set(false);
				}
				else if (NSByteBufHelper.CanRead(remain, head.mLength))
				{
					byte[] content = NSByteBufHelper.readBytes(remain, head.mLength);
					out.add(new NSMessage(head.Clone(), content));
					ctx.channel().attr(NSSocketChannelKey.IsSharedHeadFilled).set(false);
				}
				else
				{
					break;
				}
			}
		}
		remain.discardReadBytes();
	}

}
