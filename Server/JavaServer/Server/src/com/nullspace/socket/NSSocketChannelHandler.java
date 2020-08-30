package com.nullspace.socket;

import java.net.InetSocketAddress;

import org.slf4j.Logger;
import org.slf4j.LoggerFactory;

import com.nullspace.command.NSCommandManager;
import com.nullspace.logic.NSSessionManager;
import com.nullspace.packet.NSMessage;
import com.nullspace.packet.NSMessageHead;

import io.netty.buffer.ByteBuf;
import io.netty.buffer.PooledByteBufAllocator;
import io.netty.channel.ChannelHandlerContext;
import io.netty.channel.SimpleChannelInboundHandler;

public class NSSocketChannelHandler extends SimpleChannelInboundHandler<NSMessage>
{
	private static Logger mLogger = LoggerFactory.getLogger(NSSocketChannelHandler.class);
	@Override
	protected void channelRead0(ChannelHandlerContext ctx, NSMessage msg) throws Exception 
	{
		NSSocketSession session = ctx.channel().attr(NSSocketChannelKey.SESSION).get();
		NSCommandManager.instance.ExecuteMessage(session, msg);
	}
	
    @Override
    public void channelActive(ChannelHandlerContext ctx) throws Exception
    {
    	NSSocketSession session = new NSSocketSession();
    	NSSessionManager.manager.AddSession(session);
    	
    	ByteBuf buf = PooledByteBufAllocator.DEFAULT.directBuffer(NSMessageHead.Size());
    	NSMessageHead head = new NSMessageHead();
        session.Channel(ctx.channel());
        
        ctx.channel().attr(NSSocketChannelKey.SESSION).set(session);
        ctx.channel().attr(NSSocketChannelKey.SESSIONBUF).set(buf);
        ctx.channel().attr(NSSocketChannelKey.HEAD).set(head);
        ctx.channel().attr(NSSocketChannelKey.IsSharedHeadFilled).set(false);
        
        InetSocketAddress saddr =  (InetSocketAddress)ctx.channel().remoteAddress();
		session.IP(saddr.getHostString());
		session.port(saddr.getPort());
		System.out.println("connected: " + session.IP() + " " + session.port());
		mLogger.info("connected: " + session.IP() + " " + session.port());
    }


    @Override
    public void channelInactive(ChannelHandlerContext ctx) throws Exception
    {
    	NSSocketSession session = ctx.channel().attr(NSSocketChannelKey.SESSION).get();
    	ctx.channel().attr(NSSocketChannelKey.SESSIONBUF).get().release();
    	NSSessionManager.manager.Remove(session.id());
    	System.out.println("disconnected: " + session.IP() + " " + session.port());
    	mLogger.info("disconnected: " + session.IP() + " " + session.port());
    	ctx.close(); 
    }
    
    @Override
    public void exceptionCaught(ChannelHandlerContext ctx, Throwable cause)
            throws Exception {
    	System.out.println("exception: " + cause.getMessage());
    	ctx.close();
    }
}
