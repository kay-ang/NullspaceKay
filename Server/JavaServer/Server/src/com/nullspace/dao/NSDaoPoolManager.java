package com.nullspace.dao;

import java.io.FileInputStream;
import java.io.IOException;
import java.sql.Connection;
import java.sql.SQLException;

import com.jolbox.bonecp.BoneCP;
import com.jolbox.bonecp.BoneCPConfig;

public class NSDaoPoolManager
{
	public static NSDaoPoolManager instance = new NSDaoPoolManager();
	boolean isInitialized = false;
	private BoneCP mPool;
	
	private NSDaoPoolManager()
	{
		
	}
	
	public Connection AquireConnection() throws SQLException
	{
		return mPool.getConnection();
	}
	
	public void Initialize(String filename)
	{
		if (!isInitialized)
		{
			FileInputStream fileInputStream = null;
			try
			{
				fileInputStream = new FileInputStream(filename);
				BoneCPConfig config = new BoneCPConfig( fileInputStream, "default-config");
				mPool = new BoneCP(config);
			}
			catch (Exception e)
			{
				e.printStackTrace();
			}
			finally
			{
				if (fileInputStream != null)
				{
					try 
					{
						fileInputStream.close();
					} 
					catch (IOException e) 
					{
						e.printStackTrace();
					}
				}
			}
			isInitialized = true;
		}
	}
}
