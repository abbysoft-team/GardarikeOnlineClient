cd Assets/Scripts/proto
rm -rf *
wget https://github.com/abbysoft-team/ProjectXServer/blob/master/rpc/protocol/server.proto
protoc --proto_path=/ --csharp_out=/ server.proto