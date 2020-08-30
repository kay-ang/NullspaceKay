package com.nullspace.socket;

import com.nullspace.packet.NSMessageHead;

import io.netty.buffer.ByteBuf;
import io.netty.util.AttributeKey;

public class NSSocketChannelKey 
{
	public static AttributeKey<NSSocketSession> SESSION = AttributeKey.newInstance(NSSocketSession.class.toString());
	public static AttributeKey<ByteBuf> SESSIONBUF =  AttributeKey.newInstance(ByteBuf.class.toString());
	public static AttributeKey<NSMessageHead> HEAD =  AttributeKey.newInstance(NSMessageHead.class.toString());
	public static AttributeKey<Boolean> IsSharedHeadFilled = AttributeKey.newInstance("IsSharedHeadFilled");

}
