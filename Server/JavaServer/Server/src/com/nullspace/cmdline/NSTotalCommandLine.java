package com.nullspace.cmdline;


/**
 * 获得所有的可用  命令，方便查看
 * @author kay.yang
 *
 */
public class NSTotalCommandLine extends NSCmdlineCommand
{

	public NSTotalCommandLine() {
		super(NSCmdlineType.TOTLE, "显示所有 命令行 函数");
	}

	@Override
	protected void executeBefore() throws Exception 
	{
		
	}

	@Override
	protected void executeRunning() throws Exception 
	{
		NSCmdlineServices.instance().print();
	}

	@Override
	protected void executeAfter() throws Exception 
	{
		
	}
	
}
