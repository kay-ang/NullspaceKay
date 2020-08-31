using Google.Protobuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
public class PbTest
{
    void Test()
    {
        csTest test = new csTest();
        ByteString bs = test.Value;
        string url = test.TypeUrl;
        byte[] send = test.ToByteArray();
        csTest clone = csTest.Parser.ParseFrom(send);
        

        Nullspace.NetworkClient.Instance.Send(send);
    }
}