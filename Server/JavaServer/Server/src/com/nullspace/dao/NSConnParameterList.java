package com.nullspace.dao;

import java.sql.Timestamp;
import java.util.ArrayList;
import java.util.Collections;

public class NSConnParameterList
{
	public enum MJConnParameterType
	{
		P_INT(0),
		P_STRING(1),
		P_REAL(2),
		P_TIME(3),
		P_BOOLEAN(4);	
	    private final int value;
	    
	    MJConnParameterType(int value) 
	    {
	        this.value = value;
	    }
	    public int getValue() 
	    {
	        return value;
	    }
	}
	
	public class MJConnParameter implements Comparable<MJConnParameter>
	{
		public MJConnParameterType mType;
		public Integer mSortId;
		public String mValue;
		
		public int toInt()
		{
			return Integer.valueOf(mValue);
		}
		public MJConnParameter()
		{
			
		}
		
		public MJConnParameter(MJConnParameterType type, int id, String value)
		{
			mType = type;
			mSortId = id;
			mValue = value;
		}
		
		public String toString()
		{
			return mValue;
		}
		
		public float toReal()
		{
			return Float.valueOf(mValue);
		}
		
		public boolean toBoolean()
		{
			return Boolean.valueOf(mValue);
		}
		
		public Timestamp toTimestamp()
		{
			return Timestamp.valueOf(mValue);
		}
		
		public NSConnParameterList toList()
		{
			NSConnParameterList list = new NSConnParameterList();
			list.AddParam(this);
			return list;
		}

		@Override
		public int compareTo(MJConnParameter o) {
			// TODO Auto-generated method stub
			return mSortId.compareTo(o.mSortId);
		}	
	}
	
	ArrayList<MJConnParameter> mParamList;
	
	public NSConnParameterList()
	{
		mParamList = new ArrayList<>();
	}
	
	public void AddParam(MJConnParameterType type, int id, String value)
	{
		AddParam(new MJConnParameter(type, id, value));
	}
	
	public void AddParam(MJConnParameter param)
	{
		mParamList.add(param);
	}
	
	public void Sort()
	{
		Collections.sort(mParamList);
	}
	
	public int Size()
	{
		return mParamList.size();
	}
	
	public MJConnParameter GetAt(int index)
	{
		return mParamList.get(index);
	}
}


