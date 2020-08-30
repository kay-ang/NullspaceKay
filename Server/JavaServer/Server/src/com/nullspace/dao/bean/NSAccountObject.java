package com.nullspace.dao.bean;

import com.nullspace.dao.NSConnParameterList;
import com.nullspace.dao.NSConnParameterList.MJConnParameter;
import com.nullspace.dao.NSConnRecordObject;
import com.nullspace.dao.NSDaoCacheManager;

public class NSAccountObject extends NSConnRecordObject
{
	// must register
	static 
	{
		NSConnParameterList pLst = new NSConnParameterList();
		MJConnParameter parameter = pLst.new MJConnParameter();
		parameter.mType = NSConnParameterList.MJConnParameterType.P_INT;
		parameter.mSortId = 1;
		pLst.AddParam(parameter);
		parameter = pLst.new MJConnParameter();
		parameter.mType = NSConnParameterList.MJConnParameterType.P_STRING;
		parameter.mSortId = 2;
		pLst.AddParam(parameter);
		NSDaoCacheManager.RegisterParamLst(NSAccountObject.class, pLst);
	}

	public NSAccountObject()
	{
		
	}
	
	@Override
	protected void ParseColumn(MJConnParameter param) 
	{
		switch (param.mSortId)
		{
		case 1:
			mAge = param.toInt();
			break;
		case 2:
			mName = param.toString();
			break;
		}
	}
	
	@Override
	public String toString() 
	{
		StringBuilder builder = new StringBuilder();
		builder.append("MJDaoTest ( name: ").append(mName).append(" age: ").append(mAge).append(" )");
		return builder.toString();
	}
	
	private int mAge;
	private String mName;
}
