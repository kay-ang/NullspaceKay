package com.nullspace.cmdline;

import java.io.BufferedReader;
import java.io.IOException;
import java.io.InputStreamReader;
import java.util.HashMap;
import java.util.Map.Entry;

import org.apache.log4j.LogManager;
import org.apache.log4j.Logger;


/**
 * 命令行命令的缓存，收到命令，在此处理
 * @author kay.yang
 *
 */
public class NSCmdlineServices
{
	private static Logger mLogger = LogManager.getLogger(NSCmdlineServices.class);
	private HashMap<String, Class<? extends NSCmdlineCommand>> mCommands = new HashMap<>();
	private static NSCmdlineServices instance = new NSCmdlineServices();

	private NSCmdlineServices()
	{
		initCommands();
	}

	public static NSCmdlineServices instance()
	{
		return instance;
	}

	private void initCommands()
	{
		mCommands.put(NSCmdlineType.TOTLE, NSTotalCommandLine.class);
	}

	public void print() throws InstantiationException, IllegalAccessException
	{
		for (Entry<String, Class<? extends NSCmdlineCommand>> entry : mCommands.entrySet())
		{
			String str = entry.getKey();
			Class<? extends NSCmdlineCommand> cmdClazz = entry.getValue();
			NSCmdlineCommand cmd = cmdClazz.newInstance();
			System.out.println("命令行  输入 参数 ：" + str + "  --  " + cmd.description());
		}
	}
	
	public void startCmdline()
	{
		BufferedReader br = new BufferedReader(new InputStreamReader(System.in));

		String cmd;
		
		while (true)
		{
			try
			{
				System.out.print("> ");
				cmd = br.readLine();
				cmd = cmd == null ? cmd : cmd.trim();
			
				if (cmd == null || cmd.equals(""))
				{
					continue;
				}

				String[] temp = cmd.split("-");
				String type = temp[0].trim();
				type = type.toUpperCase();
				if (type.isEmpty() || type.contains(" "))
				{
					mLogger.info("命令类型只支持单字命令");
					continue;
				}
				executeCmd(type, temp);
			}
			catch (IOException e)
			{
				e.printStackTrace();
			}
		}

	}

	private void executeCmd(String type, String[] temp)
	{
		try
		{
			Class<? extends NSCmdlineCommand> clazz = mCommands.get(type);
			if (clazz == null)
			{
				mLogger.info("命令: " + type + " 不存在");
				return;
			}
			NSCmdlineCommand command = clazz.newInstance();
			for (int i = 1; i < temp.length; i++)
			{
				command.addParameter(temp[i].trim());
			}
			command.execute();
		}
		catch (Exception e)
		{
			e.printStackTrace();
		}
	}
}
