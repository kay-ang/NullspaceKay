package com.nullspace.cmdline;

import java.util.ArrayList;
import java.util.List;

/**
 * 命令行 指令 加 参数
 * @author kay.yang
 *
 */
public class NSCmdParameters
{
	private String mName;
	private List<String> mValues = new ArrayList<>();
	
	//value形式为：v1 v2 v3
	public NSCmdParameters(String name, String value)
	{
		this.mName = name;
		String[] temps = value.split(" ");
		for (String item : temps)
		{
			item = item.trim();
			mValues.add(item);
		}
	}
	
	//value的形为 以空格分隔的一串字符串，第一个字符为 name，后面的为参数
	public NSCmdParameters(String value)
	{
		String[] temps = value.split(" ");
		if (temps.length > 0)
		{
			this.mName = temps[0].trim();
		}
		for (int i = 1; i < temps.length; i++)
		{
			mValues.add(temps[i].trim());
		}
	}

	public String name()
	{
		return mName;
	}

	public List<String> values()
	{
		return mValues;
	}

}
