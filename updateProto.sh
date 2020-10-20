cd Assets/Scripts/proto
rm -rf *
wget https://raw.githubusercontent.com/abbysoft-team/ProjectXServer/master/rpc/protocol/server.proto
protoc --csharp_out=../ server.proto
