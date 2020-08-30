package com.nullspace.command;

import java.util.concurrent.FutureTask;

public class NSCommandResultTask extends FutureTask<Boolean>
{
	@SuppressWarnings("unused")
	private NSAbstractCommand mCommand;
	private long mStart = 0;
	public NSCommandResultTask(NSAbstractCommand callable) 
	{
		super(callable); 
		mCommand = callable;
		mStart = System.currentTimeMillis();
	}
	
	@Override
	protected void done()
	{
		System.out.println("task time cost: " + (System.currentTimeMillis() - mStart));
        if (this.isCancelled()) 
        {
            System.out.println("cancelled ");
        }
        else
        {
        	
        } 
	}
}
