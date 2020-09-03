using Google.Protobuf;
using System.Reflection;


namespace Nullspace
{
    public class ProtobufParserUtils
    {
        /// <summary>
        /// need cache Parser
        /// </summary>
        public static T ParseFrom<T>(byte[] data) where T : IMessage<T>
        {
            PropertyInfo propInfo = typeof(T).GetProperty("Parser");
            MessageParser<T> parser = (MessageParser<T>)propInfo.GetValue(null);
            return parser.ParseFrom(data);
        }

        public static byte[] WriteTo<T>(T obj) where T : IMessage<T>
        {
            return obj.ToByteArray();
        }
    }
}
