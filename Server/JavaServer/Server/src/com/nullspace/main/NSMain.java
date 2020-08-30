package com.nullspace.main;

import java.util.List;

import org.apache.log4j.PropertyConfigurator;
import com.nullspace.cmdline.NSCmdlineServices;
import com.nullspace.command.NSCommandManager;
import com.nullspace.dao.NSConnOperator;
import com.nullspace.dao.NSConnOperator.ConnResultList;
import com.nullspace.dao.NSConnRecordObject;
import com.nullspace.dao.NSConnResultSet;
import com.nullspace.dao.NSDaoCacheManager;
import com.nullspace.dao.NSDaoPoolManager;
import com.nullspace.file.NSPathConfig;
import com.nullspace.http.NSHttpServer;
import com.nullspace.server.NSServerManager;

public class NSMain 
{
	public static void main(String[] argvs) throws Exception
	{
		String config = "";
		if (argvs.length < 1)
			config = "res/base_config.properties";
		else
			config = argvs[0];
		NSPathConfig.initConfig(config);
		PropertyConfigurator.configure(NSPathConfig.getValue("log"));
		if (NSPathConfig.getValue("openDb") == "1")
		{
			NSDaoPoolManager.instance.Initialize(NSPathConfig.getValue("bonecp_config"));		
			NSDaoCacheManager.Initialize();
		}
		NSHttpServer.SetResourceDir(NSPathConfig.getValue("http_res"));
		NSCommandManager.initCommandManager(NSPathConfig.getValue("command"));
		NSServerManager.instance.startServer(NSPathConfig.getValue("ip_port"));
		NSCmdlineServices.instance().startCmdline();
	}
	
	static void Test()
	{
		ConnResultList set = NSConnOperator.instance.Execute(1, null);
		if (set.mFlag)
		{
			List<NSConnResultSet> temps = NSConnOperator.instance.ParseFrom(1, set);		
			for (NSConnResultSet set2 : temps)
			{
				for (NSConnRecordObject obj : set2.GetObjects())
				{
					System.out.println(obj);	
				}
			}
		}
	}
}
