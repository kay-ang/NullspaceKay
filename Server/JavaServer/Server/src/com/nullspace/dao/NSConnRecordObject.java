package com.nullspace.dao;

import com.nullspace.dao.NSConnParameterList.MJConnParameter;

public abstract class NSConnRecordObject
{	
	public NSConnRecordObject()
	{
		
	}
	
	// implement by derived class
	public void InitializeByParameter(NSConnParameterList recordLst)
	{
		int size = recordLst.Size();
		for (int i = 0; i < size; ++i)
		{
			MJConnParameter parameter = recordLst.GetAt(i);
			ParseColumn(parameter);
		}
	}
	
	protected abstract void ParseColumn(MJConnParameter param);	
	

}
