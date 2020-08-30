package com.nullspace.logic;

import java.util.concurrent.ConcurrentHashMap;
import java.util.concurrent.atomic.AtomicInteger;

import com.nullspace.packet.NSMessage;
import com.nullspace.socket.NSSocketSession;

public class NSSessionManager 
{
	public static NSSessionManager manager = new NSSessionManager();

	private AtomicInteger mIdGenerator = new AtomicInteger(1);
	private ConcurrentHashMap<Integer, NSSocketSession> mSessions = null;
	
	private NSSessionManager()
	{
		mSessions = new ConcurrentHashMap<Integer, NSSocketSession>();
	}
	
	public ConcurrentHashMap<Integer, NSSocketSession> Sessions() 
	{
		return mSessions;
	}
	
	public void AddSession(NSSocketSession session)
	{
		int id = mIdGenerator.getAndIncrement();
		session.id(id);
		mSessions.put(id, session);
	}
	
	public void Remove(int id)
	{
		NSSocketSession session = mSessions.get(id);
		if (session != null)
		{
			session.Close();
			mSessions.remove(id);
		}
	}
	
	public NSSocketSession GetSession(int id)
	{
		if (mSessions.containsKey(id))
		{
			return mSessions.get(id);
		}
		return null;
	}
	
	public void BroadAll(NSMessage msg)
	{
		BroadAllWithout(msg, -1);
	}
	
	public void BroadAllWithout(NSMessage msg, int id)
	{
		for (NSSocketSession sessionid : mSessions.values())
		{
			if (sessionid.id() != id)
			{
				msg.mHead.mSession = sessionid.id();
				sessionid.WriteMessage(msg, null);
			}
		}
	}
}
