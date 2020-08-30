package com.nullspace.command;

import java.lang.reflect.Constructor;
import java.util.HashMap;
import java.util.List;
import java.util.Map;
import java.util.concurrent.ExecutorService;
import java.util.concurrent.Executors;

import org.slf4j.Logger;
import org.slf4j.LoggerFactory;

import com.nullspace.packet.NSMessage;
import com.nullspace.socket.NSSocketSession;
import com.nullspace.utils.NSClassFileUtil;

public class NSCommandManager
{	
	private static Logger mLogger = LoggerFactory.getLogger(NSCommandManager.class);
	public static Map<Integer, Class<?> > mCommandCache = new HashMap<>();
	public static NSCommandManager instance = new NSCommandManager();
	private ExecutorService mExecutor = Executors.newCachedThreadPool();

	private NSCommandManager()
	{

	}
	
	public void ExecuteMessage(NSSocketSession session, NSMessage message)
	{
		Class<?> type = getClass(message.mHead.mType);
		if (type != null)
		{
			try {
				Constructor<?> constructor = type.getConstructor(NSSocketSession.class, NSMessage.class);
				if (constructor != null)
				{
					NSAbstractCommand command = (NSAbstractCommand) constructor.newInstance(session, message);
					mExecutor.submit(command);
				}
			} 
			catch (Exception e) {
				
				e.printStackTrace();
			} 
		}
		else
		{
			mLogger.info("not exists type: " + message.mHead.mType);
		}
	}
	
	public Class<?> getClass(int type)
	{
		if (mCommandCache.containsKey(type))
		{
			return mCommandCache.get(type);
		}
		return null;
	}
	
	public static boolean initCommandManager(String packageName)
	{
	    List<Class<?>> allClasses = NSClassFileUtil.getClasses(packageName);
	    try 
	    {
	    	for (Class<?> clazz : allClasses)
	 	    {
	 	        NSCommandAnnotion cmd = clazz.getAnnotation(NSCommandAnnotion.class);
	 	        if (cmd == null)
	 	        {
	 	            continue;
	 	        }

	 	        if (mCommandCache.containsKey(cmd.code()))
	 	        {
	 	        	mLogger.info(String.format(
	 	                    "Duplicated first command code: [0x%x|%d].",
	 	                    cmd.code(), cmd.code()));

	 	            return false;
	 	        }

	 	       mCommandCache.put(cmd.code(), clazz);
	 	    }

	    	 mLogger.info(String.format("command size: %d", mCommandCache
	 	            .size()));
	 	    return true;
		} 
	    catch (Exception e) 
		{
	    	e.printStackTrace();
	    	mLogger.error("加载command类   : " + e.getMessage());
			return false;
		}
	}
}
