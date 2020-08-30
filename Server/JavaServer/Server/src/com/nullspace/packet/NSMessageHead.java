package com.nullspace.packet;

public class NSMessageHead 
{
    public int mType = 0;
    // 报文长度，不包含消息头，目前最大允许1K
    // 单包大于1k的上行请求消息，将会被丢弃并且会关闭SOCKET
    public int mLength = 0;
    // 返回码
    public int mResult = 0;
    // 会话id，供服务之间使用，客户端无需关心
    public int mSession = 0;
    // 源地址，接入服务会将此项值与SOCKET绑定，是应答或者通知消息下发路由的一句
    // 具体的游戏需要使用相同的填充规则，比如同一田聪USERID
    public long mFrom = 0;

    // 目的地址，功能与from字段类似
    public long mTo = 0;

    // 业务掩码，服务器根据此字段对消息做路由转发
    // 比如：如果填充房间id，那么消息会转发到相同的服务节点
    // 如果填0，则做负载均衡分发
    public long mMask = 0;
    // 附加的字段，服务端会鸳鸯返回
    // 客户端可填充序列号，用于确认一条特定的消息的应答
    public long mAddition = 0;
    
	public NSMessageHead Clone()
	{
		NSMessageHead head = new NSMessageHead();
		head.mType = mType;
		head.mLength = mLength;
		head.mResult = mResult;
		head.mSession = mSession;
		
		head.mFrom = mFrom;
		head.mTo = mTo;
		head.mMask = mMask;
		head.mAddition = mAddition;
		return head;
	}
	
	public static int Size()
	{
		return 48; // 4 + 4 + 4 + 4 + 8 + 8 + 8 + 8;
	}

}
