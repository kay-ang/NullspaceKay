package com.nullspace.dao;

import java.sql.Connection;
import java.util.ArrayList;
import java.util.HashMap;
import java.util.Map;

import com.nullspace.dao.NSConnOperator.ConnResult;
import com.nullspace.dao.NSConnOperator.ConnResultList;
import com.nullspace.dao.bean.NSSqlViewObject;

/*
 * 
 * 1 t_object 表
 * 		int     objId 唯一索引
 * 		string  className
 * 
 * 2 t_object_filed 表
 * 		int     id 
 * 		int     objId  foreign key  t_object
 * 		int 	sortId  表字段的index
 * 		int		type    字段类型
 * 
 * 3 t_sql （所需参数，客户端传递）
 * 		int   sqlId  primary key
 * 		string  objIds(foreign, 以','分割)
 * 		string sql_statement
 * 
 * 4 t_sql_param (暂时不需要配置表，使用 pb 文件配置即可)
 * 		int id
 * 		int sqlId(foreign key)
 * 		int paramType
 * 		int paramIndex
 * 		string value
 * */

public class NSDaoCacheManager 
{
	static Map<String, NSConnParameterList> mParamCache = new HashMap<>();
	static Map<Integer, NSSqlViewObject> mSqlCache = new HashMap<>();
	
	public static void RegisterParamLst(Class<? extends NSConnRecordObject> clazz, NSConnParameterList lst)
	{
		mParamCache.put(clazz.toString(), lst);
	}
	
	public static NSConnParameterList GetParamLst(Class<? extends NSConnRecordObject> clazz)
	{
		return mParamCache.get(clazz.toString());
	}
	
	public static void Initialize()
	{
		try
		{
			ConnResultList result = NSConnOperator.instance.new ConnResultList();
			Connection connection = NSDaoPoolManager.instance.AquireConnection();
			ConnResult flag = NSConnOperator.instance.Execute(connection, "select sql_id, object_ids, sql_statement, sql_type from t_sql_view;", NSConnOperator.ConnOperatorType.T_QUERY, null, result);
			if (flag.mFlag)
			{
				NSConnResultSet resultSet = NSConnOperator.instance.CreateTableObject(result.Get(0), NSSqlViewObject.class);
				ArrayList<NSConnRecordObject> objects = resultSet.GetObjects();
				for (NSConnRecordObject obj : objects)
				{
					System.out.println(obj);	
					NSSqlViewObject sqlObject = (NSSqlViewObject)obj;
					mSqlCache.put(sqlObject.mSqlId, sqlObject);
				}
			}
		} 
		catch (Exception e) 
		{
			e.printStackTrace();
		}
	}
	
	public static NSSqlViewObject Get(int sqlId)
	{
		return mSqlCache.get(sqlId);
	}
}
