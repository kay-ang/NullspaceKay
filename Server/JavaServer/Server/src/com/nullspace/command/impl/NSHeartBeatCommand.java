package com.nullspace.command.impl;

import com.nullspace.command.NSAbstractCommand;
import com.nullspace.command.NSCommandAnnotion;
import com.nullspace.packet.NSMessage;
import com.nullspace.packet.NSMessageType;
import com.nullspace.socket.NSSocketSession;

@NSCommandAnnotion(code = NSMessageType.HEARTBEAT_RES, desc = "心跳包")
public class NSHeartBeatCommand extends NSAbstractCommand
{
	public NSHeartBeatCommand()
	{
		
	}
	
	public NSHeartBeatCommand(NSSocketSession session, NSMessage message)
	{
		super(session, message);
	}
	
	@Override
	public void Execute() 
	{
		mOut.add(mMessage);
	}
	
}
