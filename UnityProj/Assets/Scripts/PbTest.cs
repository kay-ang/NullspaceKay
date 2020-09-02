using Google.Protobuf;

public class PbTest
{
    void Test()
    {
        csTest test = new csTest();
        ByteString bs = test.Value;
        string url = test.TypeUrl;
        byte[] send = test.ToByteArray();
        csTest clone = csTest.Parser.ParseFrom(send);
    }
}