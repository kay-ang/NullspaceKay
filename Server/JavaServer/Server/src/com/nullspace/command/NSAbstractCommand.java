package com.nullspace.command;

import java.util.ArrayList;
import java.util.List;
import java.util.concurrent.Callable;
import com.nullspace.packet.NSMessage;
import com.nullspace.socket.NSSocketSession;

public abstract class NSAbstractCommand implements Callable<Boolean> 
{	
	protected NSSocketSession mSession;
	protected NSMessage mMessage;
	protected List<NSMessage> mOut;
	// private long _start;
	public NSAbstractCommand()
	{
		mOut = new ArrayList<NSMessage>();
	}
	
	public NSAbstractCommand(NSSocketSession session, NSMessage message)
	{
		this();
		mSession = session;
		mMessage = message;
		// _start = System.currentTimeMillis();
	}
	
	public NSSocketSession Session()
	{
		return mSession;
	}
	
	public abstract void Execute();

	@Override
	public Boolean call() throws Exception
	{
		 Execute();
		 for (NSMessage msg : mOut) 
		 {
			 mSession.WriteMessage(msg, null);
		 }
		 mOut.clear();
		 // System.out.println("task cost: " + (System.currentTimeMillis() - _start));
		 return true;
	}
}
