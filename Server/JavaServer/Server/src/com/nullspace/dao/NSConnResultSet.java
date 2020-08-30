package com.nullspace.dao;

import java.sql.ResultSet;
import java.sql.SQLException;
import java.util.ArrayList;

import com.nullspace.dao.NSConnParameterList.MJConnParameter;

public class NSConnResultSet
{
	private NSConnParameterList mResultParameterList;
	private ArrayList<NSConnRecordObject> mRecords;
	private Class<? extends NSConnRecordObject> mClazz;
	
	public NSConnResultSet(Class<? extends NSConnRecordObject> clazz)
	{
		mClazz = clazz;
		mResultParameterList = NSDaoCacheManager.GetParamLst(clazz);
		mRecords = new ArrayList<>();
	}
	
	public void HandleResult(ResultSet set)
	{
		try 
		{
			if (mResultParameterList == null)
			{
				mClazz.newInstance();
				mResultParameterList = NSDaoCacheManager.GetParamLst(mClazz);
			}
			while (set.next())
			{
				NSConnRecordObject t = mClazz.newInstance();
				Parse(set, t);
				mRecords.add(t);
			}
		} 
		catch (Exception e) 
		{
			e.printStackTrace();
		}
	}
	
	public ArrayList<NSConnRecordObject> GetObjects()
	{
		return mRecords;
	}
	
	private void Parse(ResultSet resultSet, NSConnRecordObject t) throws SQLException
	{
		int size = mResultParameterList.Size();
		// should have default constructor
		for (int i = 0; i < size; ++i)
		{
			MJConnParameter param = mResultParameterList.GetAt(i);
			param.mValue = resultSet.getString(param.mSortId.intValue());	
		}
		t.InitializeByParameter(mResultParameterList);
	}
}
