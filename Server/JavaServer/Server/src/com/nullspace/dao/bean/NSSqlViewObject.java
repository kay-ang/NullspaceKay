package com.nullspace.dao.bean;

import com.nullspace.dao.NSConnParameterList.MJConnParameter;
import com.nullspace.dao.NSConnOperator.ConnOperatorType;
import com.nullspace.dao.NSConnParameterList;
import com.nullspace.dao.NSConnRecordObject;
import com.nullspace.dao.NSDaoCacheManager;

public class NSSqlViewObject extends NSConnRecordObject
{
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
		 parameter = pLst.new MJConnParameter();
		 parameter.mType = NSConnParameterList.MJConnParameterType.P_STRING;
		 parameter.mSortId = 3;
		 pLst.AddParam(parameter);
		 parameter = pLst.new MJConnParameter();
		 parameter.mType = NSConnParameterList.MJConnParameterType.P_INT;
		 parameter.mSortId = 4;
		 pLst.AddParam(parameter);
		 NSDaoCacheManager.RegisterParamLst(NSSqlViewObject.class, pLst);
	 }
	
	@Override
	protected void ParseColumn(MJConnParameter param) 
	{
		switch (param.mSortId)
		{
		case 1:
			mSqlId = param.toInt();
			break;
		case 2:
			mObjectIds = param.toString();
			break;
		case 3:
			mSqlStatement = param.toString();
			break;
		case 4:
			mOperatorType = GetType(param.toInt());
			break;
		}
	}
	
	///
	///T_INSERT(1),
	///T_DELETE(2),
	///T_UPDATE(3),
	///T_QUERY(4);
	static ConnOperatorType GetType(int type)
	{
		switch(type)
		{
		case 1:
			return ConnOperatorType.T_INSERT;
		case 2:
			return ConnOperatorType.T_DELETE;
		case 3:
			return ConnOperatorType.T_UPDATE;
		case 4:
			return ConnOperatorType.T_QUERY;
		}
		return ConnOperatorType.T_NONE;
	}

	@Override
	public String toString() {
		StringBuilder builder = new StringBuilder();
		builder.append("MJSqlViewObject ( _sqlId: ").append(mSqlId).append(" _objectIds: ").append(mObjectIds).append(" _sql: ").append(mSqlStatement).append(" _type: ").append(mOperatorType).append(" )");
		return builder.toString();
	}
	
	public int mSqlId;
	public String mObjectIds;
	public String mSqlStatement;
	public ConnOperatorType mOperatorType;
}
