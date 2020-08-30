package com.nullspace.cmdline;

import java.util.ArrayList;
import java.util.List;

/**
 * 命令行处理抽象类
 * @author kay.yang
 *
 */
public abstract class NSCmdlineCommand
{
	private String mCmdLineType;
	private String mDescription;
	private List<NSCmdParameters> mParameters;
	
	public NSCmdlineCommand(String type, String des)
	{
		mParameters = new ArrayList<>();
		this.mCmdLineType = type;
		this.mDescription = des;
	}
	
	public final void execute() throws Exception
	{
		System.out.println("命令: " + this.mCmdLineType + " description: " + mDescription + " 运行开始.");
		executeBefore();
		executeRunning();
		executeAfter();
		System.out.println("命令: " + this.mCmdLineType + " description: " + mDescription + " 运行完成.");
	}

	//对参数进行验证等功能，在执行函数之前执行
	protected abstract void executeBefore() throws Exception;
	//对命令进行执行的函数
	protected abstract void executeRunning() throws Exception;
	//命令执行完毕之后的处理
	protected abstract void executeAfter() throws Exception;

	public String cmdLineType()
	{
		return mCmdLineType;
	}

	public String description()
	{
		return mDescription;
	}

	public List<NSCmdParameters> parameters()
	{
		return mParameters;
	}

	public void addParameter(String value)
	{
		this.mParameters.add(new NSCmdParameters(value));
	}
}
