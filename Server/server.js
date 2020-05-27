const GAME_START = 'start';
const REQUEST_PLAYER_INDEX = 'req';

var express = require('express');
var app = express();
var compression = require('compression');

var server = require('http').createServer(app);
var io = require('socket.io')(server);

const cp = require('child_process');
const path = require('path');

app.use(compression());
app.use(express.static(__dirname + '/client/'));

app.get('/', function(req, res) {

});

const MAX_PLAYER = 2;
var ROOM_KEY = 0;

// 방 정보
var Room = function(){
    this.roomKey;
    this.roomName;
    this.playerKeys=[];
    this.playerNames=[];
    this.playerCounts = 0;
}

// RoomList 정의 및 method 생성
var RoomList = function(){
    this.rooms = [];
    this.pos = 0;
    this.listSize = 0;
}
RoomList.prototype.createRoom = function(roomKey, roomName, playerKey, playerName){
    var room = new Room();
    room.roomKey = roomKey;
    room.roomName = roomName;
    room.playerKeys[0] = playerKey;
    room.playerNames[0] = playerName;
    room.playerCounts = 1;
    this.rooms.push(room);
    this.listSize++;
}
RoomList.prototype.addPlayer = function(roomKey, playerKey, playerName){
    var roomIndex = this.findRoom(roomKey);

    if(roomIndex >  -1){
        if(this.rooms[roomIndex].playerCounts == MAX_PLAYER){
            return 0;
        }
        this.rooms[roomIndex].playerKeys[this.rooms[roomIndex].playerCounts] = playerKey;
        this.rooms[roomIndex].playerNames[this.rooms[roomIndex].playerCounts] = playerName;
        this.rooms[roomIndex].playerCounts++;
        return true;
    }
    else{
        return -1;
    }
}
RoomList.prototype.removePlayer = function(roomKey, playerKey){
    roomIndex = this.findRoom(roomKey);

    if(roomIndex > -1){
        var playerPos = 0;
        for(; playerPos < this.rooms[roomIndex].playerCounts; playerPos++){
            if(this.rooms[roomIndex].playerKeys[playerPos] == playerKey){
                break;
            }
            if(playerPos == this.rooms[roomIndex].playerCounts -1){ // Not found!
                return -1;
            }
        }
        this.rooms[roomIndex].playerNames.splice(playerPos,1);
        this.rooms[roomIndex].playerKeys.splice(playerPos,1);
        this.rooms[roomIndex].playerCounts--;
        if(this.rooms[roomIndex].playerCounts == 0){
            this.rooms.splice(roomIndex,1);
            this.listSize--;
        }
        return true;
    }
    else{
        return -1;
    }
}
RoomList.prototype.getNames = function(roomKey){
    var roomIndex = this.findRoom(roomKey);

    if(roomIndex > -1){
        return this.rooms[roomIndex].playerNames;
    }
    else
        return null;
}
RoomList.prototype.findRoom = function(roomKey){
    var i = 0;
    for(;i<this.listSize;i++){
        if(this.rooms[i].roomKey == roomKey){
            return i;
        }
    }
    return -1;
}
RoomList.prototype.removeRoom = function(roomKey){
    var removePos = this.findRoom(roomKey);

    if(removePos > -1){
        this.rooms.splice(removePos,1);
        this.listSize--;
        return true;
    }
    return -1;
}
RoomList.prototype.stringifyRoomList = function(){
    var RoomKeys = [];
    var RoomNames = [];
    var Headcounts = [];
    for(var i = 0; i < this.listSize; i++){
        RoomKeys.push(this.rooms[i].roomKey);
        RoomNames.push(this.rooms[i].roomName);
        Headcounts.push(this.rooms[i].playerCounts);
    }
    var sendingData = new Object();
    sendingData.RoomKeys = RoomKeys;
    sendingData.RoomNames = RoomNames;
    sendingData.Headcounts = Headcounts;
    return JSON.stringify(sendingData);
}
RoomList.prototype.stringifyRoomInfo = function(roomKey){
    var roomIndex = this.findRoom(roomKey);

    if(roomIndex > -1){
        var sendingData = new Object();
        sendingData.RoomKey = roomKey;
        sendingData.RoomName = this.rooms[roomIndex].roomName;
        sendingData.PlayerKeys = this.rooms[roomIndex].playerKeys;
        sendingData.PlayerNames = this.rooms[roomIndex].playerNames;
        return JSON.stringify(sendingData);
    }
}
RoomList.prototype.length = function(){
    return this.listSize;
}
RoomList.prototype.clear = function(){
    this.rooms = [];
    this.listSize = 0;
    this.pos = 0;
}

var ROOM_LIST = new RoomList();

port = ['9091'];
//var child = cp.fork("game_server.js", port);

io.on('connection', function(socket) {

    console.log("Connect");

    socket.on('room_list', function(data) {
        // for debugging
        console.log('in room_list');

        var datas = ROOM_LIST.stringifyRoomList();
        console.log(datas);
        socket.emit('room_list', datas);
    });


    socket.on('create_room', function(data) {
        //for debugging
        console.log('in create_room');

        ROOM_LIST.createRoom(ROOM_KEY, data.RoomName, socket.id, data.PlayerName);
        socket.join(ROOM_KEY);

        var datas = ROOM_LIST.stringifyRoomInfo(ROOM_KEY);
        console.log(datas);
        socket.emit('room_info', datas);
        ROOM_KEY = ROOM_KEY + 1;
    });

    socket.on('enter_room', function(data){
        //for debugging
        console.log('in enter_room');

        var flag;
        flag = ROOM_LIST.addPlayer(data.RoomKey, socket.id, data.PlayerName)
        if(flag > 0){  // flag == true
            socket.join(data.RoomKey);

            var datas = ROOM_LIST.stringifyRoomInfo(data.RoomKey);
            console.log(datas);
            io.sockets.in(data.RoomKey).emit('room_info', datas); // broadcast to all clients in room(data.RoomKey)
        }
        else { // flag == 0 방이 가득 참 or flag ==  -1 해당 방이 존재하지 않음
            var datas = ROOM_LIST.stringifyRoomList();
            socket.emit('fail_enter_room', "");
            socket.emit('room_list', datas);
        }
    });

    socket.on('chat', function(data){
        var sendingData = new Object();
        sendingData.PlayerKey = socket.id;
        sendingData.Message = data.Message;
        var datas = JSON.stringify(sendingData);

        io.sockets.in(data.RoomKey).emit('chat', datas);
    });

    socket.on('exit_room', function(data){
        //for debugging
        console.log('in exit_room');

        if(ROOM_LIST.removePlayer(data.RoomKey, socket.id) > -1){
            console.log(data.PlayerName + ' player exit ' + data.RoomKey + ' room!');
            socket.leave(data.RoomKey);

            // 방을 나간 플레이어에게만
            var roomList = ROOM_LIST.stringifyRoomList();
            socket.emit('room_list', roomList);

            // 플레이어가 나간 방의 다른 플레이어들에게
            var roomInfo = ROOM_LIST.stringifyRoomInfo(data.RoomKey);
            io.sockets.in(data.RoomKey).emit('room_info', roomInfo);
        }
    });

    socket.on('room_info', function(data) {
        var datas = ROOM_LIST.stringifyRoomInfo(data.RoomKey);
        console.log(datas);
        socket.emit('room_info', datas);
    });


});

server.listen(9090, function() {
    console.log('Socket IO server listening on port 9090');
});




    //// ���ӵ� ���� Ŭ���̾�Ʈ���� �޽����� �����Ѵ�
    //io.emit('event_name', msg);

    //// �޽����� ������ Ŭ���̾�Ʈ���Ը� �޽����� �����Ѵ�
    //socket.emit('event_name', msg);

    //// �޽����� ������ Ŭ���̾�Ʈ�� ������ ���� Ŭ���̾�Ʈ���� �޽����� �����Ѵ�
    //socket.broadcast.emit('event_name', msg);

    //// Ư�� Ŭ���̾�Ʈ���Ը� �޽����� �����Ѵ�
    //io.to(id).emit('event_name', data);
