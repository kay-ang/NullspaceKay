package com.nullspace.server;

import java.util.ArrayList;
import java.util.List;

import com.nullspace.http.NSHttpServer;
import com.nullspace.socket.NSSocketServer;
import com.nullspace.utils.NSThreePair;
import com.nullspace.websocket.NSWebsocketServer;

public class NSServerManager
{
	public static NSServerManager instance = new NSServerManager();
	
	private List<NSThreePair<Integer, String, Integer>> mAddresses;
	private List<NSServer> mServerList;
	private NSServerManager()
	{
		mAddresses = new ArrayList<NSThreePair<Integer, String, Integer>>();
		mServerList = new ArrayList<>();
	}
	
	public void startServer(String ip_port) throws Exception
	{
		String[] strs = ip_port.replaceAll(" ", "").split(",");
		for (String str : strs) 
		{
			String[] ipport = str.split(":");
			if (ipport.length != 3)
			{
				continue;
			}
			AddServerAddr(Integer.valueOf(ipport[0]), ipport[1], Integer.valueOf(ipport[2]));
		}
		Run();
	}
	
	private void AddServerAddr(Integer id, String ip, int port)
	{
		mAddresses.add(new NSThreePair<Integer, String, Integer>(id, ip, port));
	}
	
	private void Run() throws Exception
	{
		NSServer server;
		for (NSThreePair<Integer, String, Integer> pair : mAddresses) 
		{
			server = StartServer(pair);
			server.Start();
			mServerList.add(server);
		}
	}
	
	public void Stop()
	{
		for (NSServer server : mServerList)
		{
			server.Stop();
		}
	}
	
	private NSServer StartServer(NSThreePair<Integer, String, Integer> pair) throws Exception
	{
		if (pair.first() == 0)
		{
			return new NSSocketServer(pair.second(), pair.three());
		}
		else if (pair.first() == 1)
		{
			return new NSHttpServer(pair.second(), pair.three());
		}
		else if (pair.first() == 2)
		{
			return new NSWebsocketServer(pair.second(), pair.three());
		}
		else
		{
			throw new Exception("No Support Server Type: " + pair.first() );
		
		}
	}
	
}
