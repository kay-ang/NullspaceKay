@echo off
protoc.exe --csharp_out=../UnityProj/CommonLib/ProtobufMessage/MessageDefine *.proto
echo successfullly
pause