package com.nullspace.socket;


import org.slf4j.Logger;
import org.slf4j.LoggerFactory;

import com.nullspace.server.NSServer;

import io.netty.bootstrap.ServerBootstrap;
import io.netty.buffer.PooledByteBufAllocator;
import io.netty.channel.ChannelOption;
import io.netty.channel.EventLoopGroup;
import io.netty.channel.nio.NioEventLoopGroup;
import io.netty.channel.socket.nio.NioServerSocketChannel;

public class NSSocketServer extends NSServer
{
	private static Logger mLogger = LoggerFactory.getLogger(NSSocketServer.class);
	@Override
	public void Run()
	{
		// 主线程池
		EventLoopGroup bossGroup = new NioEventLoopGroup(1);
		// 工作线程池
		EventLoopGroup workerGroup = new NioEventLoopGroup(8);
		try
		{
            ServerBootstrap b = new ServerBootstrap();
            // 添加服务的配置信息
            b.group(bossGroup, workerGroup)
             .channel(NioServerSocketChannel.class)
             .option(ChannelOption.SO_BACKLOG, 1024)
             .option(ChannelOption.SO_REUSEADDR, true)
             .option(ChannelOption.ALLOCATOR, PooledByteBufAllocator.DEFAULT)
             .childOption(ChannelOption.ALLOCATOR, PooledByteBufAllocator.DEFAULT)
             .childOption(ChannelOption.TCP_NODELAY, true) 
             .childOption(ChannelOption.SO_KEEPALIVE, true)
             //.handler(new LoggingHandler(LogLevel.INFO))
             .childHandler(new NSSocketChannelInitializer()); 
            // Start the server.
            mServerChannel = b.bind(mIP, mPort).sync().channel();
            // Wait until the server socket is closed.
            mServerChannel.closeFuture().sync();
        }
		catch (Exception e)
		{
			mLogger.debug("server: " + e.getMessage());
		}
		finally 
		{
            // Shut down all event loops to terminate all threads.
            bossGroup.shutdownGracefully();
            workerGroup.shutdownGracefully();
        }
	}
	
	public NSSocketServer(String ip, int port)
	{
		super(ip, port);
	}
	
	@Override
	public String toString()
	{
		StringBuilder build = new StringBuilder();
		build.append("Socket TCP Server started ...\n").append(" IP: ").append(mIP).append(" at PORT: ").append(mPort);
		return build.toString();
	}
	
}
