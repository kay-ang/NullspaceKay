package com.nullspace.dao;

import java.sql.Connection;
import java.sql.PreparedStatement;
import java.sql.ResultSet;
import java.sql.SQLException;
import java.util.ArrayList;
import java.util.List;

import com.nullspace.dao.NSConnParameterList.MJConnParameter;
import com.nullspace.dao.bean.NSSqlViewObject;
import com.sun.rowset.CachedRowSetImpl;

public class NSConnOperator
{
	final static String PACKAGE = "com.nullspace.dao.bean.";
	static String GetFullClassName(String name)
	{
		return PACKAGE + name;
	}
	public static NSConnOperator instance = new NSConnOperator();
	private NSConnOperator(){}
	
	public class ConnResult
	{
		public int mCountAffacted;
		public boolean mFlag;
		public ConnResult()
		{
			mCountAffacted = 0;
			mFlag = true;
		}
	}
	
	public enum ConnOperatorType
	{
		T_NONE(0),
		T_INSERT(1),
		T_DELETE(2),
		T_UPDATE(3),
		T_QUERY(4);

	    private final int value;

	    ConnOperatorType(int value) {
	        this.value = value;
	    }

	    public int getValue() {
	        return value;
	    }
	}
	
	public class ConnResultList
	{
		public boolean mFlag;
		public int mAffectCount;
		private ArrayList<CachedRowSetImpl> mResultSetList;
		public ConnResultList()
		{
			mResultSetList = new ArrayList<>();
		}
		
		public int Size()
		{
			return mResultSetList.size();
		}
		
		public ResultSet Get(int index)
		{
			return mResultSetList.get(index);
		}
		
		public void AddResult(CachedRowSetImpl set)
		{
			mResultSetList.add(set);
		}
	}
	
	/// update insert delete, without parameters
	public ConnResult Execute(Connection conn, String sql, ConnOperatorType type)
	{
		return Execute(conn, sql, type, null, null);
	}
	
	/// update insert delete, within parameters
	public ConnResult Execute(Connection conn, String sql, ConnOperatorType type, NSConnParameterList parameters)
	{
		return Execute(conn, sql, type, parameters, null);
	}
	
	// query,without parameters
	public ConnResult Execute(Connection conn, String sql, ConnOperatorType type, ConnResultList result)
	{
		return Execute(conn, sql, type, null, result);
	}
	
	public ConnResultList Execute(int sqlId, NSConnParameterList parameters)
	{
		NSSqlViewObject sqlView = NSDaoCacheManager.Get(sqlId);
		if (sqlView == null)
		{
			return null;
		}
		if (sqlView.mOperatorType == NSConnOperator.ConnOperatorType.T_NONE)
		{
			return null;
		}
		ConnResultList resultList = new ConnResultList();
		Connection connection;
		try 
		{
			connection = NSDaoPoolManager.instance.AquireConnection();
			ConnResult result = Execute(connection, sqlView.mSqlStatement, sqlView.mOperatorType, parameters, resultList);
			resultList.mAffectCount = result.mCountAffacted;
			resultList.mFlag = result.mFlag;
		}
		catch (SQLException e) 
		{
			e.printStackTrace();
		}
		return resultList;
	}
	
	@SuppressWarnings("unchecked")
	public List<NSConnResultSet> ParseFrom(int sqlId, ConnResultList resultList)
	{
		if (resultList == null || resultList.Size() < 1)
		{
			return null;
		}
		NSSqlViewObject sqlView = NSDaoCacheManager.Get(sqlId);
		if (sqlView == null)
		{
			return null;
		}
		String[] objectClass = sqlView.mObjectIds.split(";");
		if (objectClass.length != resultList.Size())
		{
			return null;
		}
		List<NSConnResultSet> connResultSets = new ArrayList<>();
		try
		{
			for (int index = 0; index < objectClass.length; ++index)
			{
				String classname = objectClass[index].trim();
				if (classname.equals(""))
				{
					continue;
				}
				classname = GetFullClassName(classname);
				Class<?> clazz = Class.forName(classname);
				// com.nullspace.dao.MJConnRecordObject
				if (clazz.getSuperclass().getName().equalsIgnoreCase("com.nullspace.dao.MJConnRecordObject"))
				{
					NSConnResultSet resultSet = NSConnOperator.instance.CreateTableObject(resultList.Get(index), (Class<? extends NSConnRecordObject>)clazz);
					connResultSets.add(resultSet);
				}
			}
		}
		catch(Exception e)
		{
			e.printStackTrace();
		}

		return connResultSets;
	}
	
	// update insert delete query
	public ConnResult Execute(Connection conn, String sql, ConnOperatorType type, NSConnParameterList parameters, ConnResultList result)
	{
		ConnResult ret = instance.new ConnResult();
		PreparedStatement statement;
		try
		{
			if (conn != null && !conn.isClosed())
			{
				conn.setAutoCommit(false);
				statement = conn.prepareStatement(sql);
				if (parameters != null)
				{
					ParameterSet(statement, type, parameters, result);
				}
				switch (type)
				{
				case T_INSERT:
				case T_UPDATE:
				case T_DELETE:
					ret.mCountAffacted = statement.executeUpdate();
					break;
				case T_QUERY:
					boolean flag = statement.execute();
					if (flag)
					{
						ResultOut(statement, result);
					}
					break;
				default:
					System.out.println("type undefined: " + type);
					break;
				}
				conn.commit();
			}
		}
		catch (Exception e)
		{
			e.printStackTrace();
			ret.mFlag = false;
		}
		return ret;
	}
	
	public NSConnResultSet CreateTableObject(ResultSet resultSet, Class<? extends NSConnRecordObject> clazz)
	{
		NSConnResultSet set = new NSConnResultSet(clazz);
		set.HandleResult(resultSet);
		return set;
	}

    private void ParameterSet(PreparedStatement prep_stat, ConnOperatorType type, NSConnParameterList parameters, ConnResultList result) throws SQLException
	{
		if (prep_stat != null)
		{
			int size = parameters.Size();
			for (int i = 0; i < size; ++i)
			{
				ParameterSet(prep_stat, parameters.GetAt(i));
			}
		}
	}

    private void ParameterSet(PreparedStatement prep_stat, MJConnParameter parameters) throws SQLException
	{
		switch (parameters.mType)
		{
		case P_INT:
			prep_stat.setInt(parameters.mSortId, parameters.toInt());
			break;
		case P_STRING:
			prep_stat.setString(parameters.mSortId, parameters.toString());
			break;
		case P_REAL:
			prep_stat.setFloat(parameters.mSortId, parameters.toReal());
			break;
		case P_TIME:
			prep_stat.setTimestamp(parameters.mSortId, parameters.toTimestamp());
			break;
		case P_BOOLEAN:
			prep_stat.setBoolean(parameters.mSortId, parameters.toBoolean());
			break;
		default:
			break;
		}
	}

    private void ResultOut(PreparedStatement statement, ConnResultList result) throws SQLException
	{
		ResultSet set = statement.getResultSet();
		CachedRowSetImpl temp = new CachedRowSetImpl();
		temp.populate(set);
		result.AddResult(temp);		
		boolean isMore = false;
		while((isMore = statement.getMoreResults()== true)||statement.getUpdateCount() != -1)
		{
			if (isMore)
			{
				set = statement.getResultSet();
				temp = new CachedRowSetImpl();
				temp.populate(set);
				result.AddResult(temp);
			}
		}
	}
}
