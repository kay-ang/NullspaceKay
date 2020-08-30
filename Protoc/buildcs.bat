@echo off
protoc.exe --csharp_out=../UnityProj/CommonLib/ProtobufMessage/MessageDefine *.proto
protoc.exe --java_out=../Server/JavaServer/MessageDefine *.proto
echo successfullly
pause