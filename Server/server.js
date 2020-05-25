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

const MAX_PLAYER = 3;

// 방 정보
var Room = function(){
    this.roomName;
    this.playerNames=[]
    this.playerCounts = 0;
}

// RoomList 정의 및 method 생성
var RoomList = function(){
    this.rooms = []
    this.pos = 0;
    this.listSize = 0;
}
RoomList.prototype.createRoom = function(roomName, playerName){
    var room = new Room();
    room.roomName = roomName;
    room.playerNames[0] = playerName;
    room.playerCounts = 1;
    this.rooms.push(room);
    this.listSize++;
}
RoomList.prototype.addPlayer = function(roomName, playerName){
    var roomIndex = this.findRoom(roomName);

    if(roomIndex >  -1){
        if(this.rooms[roomIndex].playerCounts == MAX_PLAYER){
            return 0;
        }
        this.rooms[roomIndex].playerNames[this.rooms[roomIndex].playerCounts] = playerName;
        this.rooms[roomIndex].playerCounts++;
        return true;
    }
    else{
        return -1;
    }
}
RoomList.prototype.removePlayer = function(roomName, playerName){
    roomIndex = this.findRoom(roomName);

    if(roomIndex > -1){
        var playerPos = 0;
        for(; playerPos < this.rooms[roomIndex].playerCounts; playerPos++){
            if(this.rooms[roomIndex].playerNames[playerPos] == playerName){
                break;
            }
            if(playerPos == this.rooms[roomIndex].playerCounts -1){ // Not found!
                return -1;
            }
        }
        this.rooms[roomIndex].playerNames.splice(playerPos,1);
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
RoomList.prototype.getNames = function(roomName){
    var roomIndex = this.findRoom(roomName);

    if(roomIndex > -1){
        return this.rooms[roomIndex].playerNames;
    }
    else
        return null;
}
RoomList.prototype.findRoom = function(roomName){
    var i = 0;
    for(;i<this.listSize;i++){
        if(this.rooms[i].roomName == roomName){
            return i;
        }
    }
    return -1;
}
RoomList.prototype.removeRoom = function(roomName){
    var removePos = this.findRoom(roomName);

    if(removePos > -1){
        this.rooms.splice(removePos,1);
        this.listSize--;
        return true;
    }
    return -1;
}
RoomList.prototype.stringifyRoomList = function(){
    var RoomNames = [];
    var Headcounts = [];
    for(var i = 0; i < this.listSize; i++){
        RoomNames.push(this.rooms[i].roomName);
        Headcounts.push(this.rooms[i].playerCounts);
    }
    var sendingData = new Object();
    sendingData.RoomNames = RoomNames;
    sendingData.Headcounts = Headcounts;
    return JSON.stringify(sendingData);
}
RoomList.prototype.stringifyRoomInfo = function(roomName){
    var sendingData = new Object();
    sendingData.RoomName = roomName;
    sendingData.PlayerNames = this.getNames(roomName);
    return JSON.stringify(sendingData);
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

        ROOM_LIST.createRoom(data.RoomName, data.PlayerName);
        socket.join(data.RoomName);
        console.log('received  datas = ' + data);

        var datas = ROOM_LIST.stringifyRoomInfo(data.RoomName);
        console.log(datas);
        socket.emit('room_info', datas);
    });

    socket.on('enter_room', function(data){
        //for debugging
        console.log('in enter_room');

        var flag;
        flag = ROOM_LIST.addPlayer(data.RoomName, data.PlayerName)
        if(flag > 0){  // flag == true
            socket.join(data.RoomName);

            var datas = ROOM_LIST.stringifyRoomInfo(data.RoomName);
            console.log(datas);
            socket.emit('room_info', datas);
        }
        else { // flag == 0 방이 가득 참 or flag ==  -1 해당 방 이름이 존재하지 않음
            var datas = ROOM_LIST.stringifyRoomList();
            socket.emit('fail_enter_room');
            socket.emit('room_list', datas);
        }
    });

    socket.on('exit_room', function(data){
        //for debugging
        console.log('in exit_room');

        if(ROOM_LIST.removePlayer(data.RoomName, data.PlayerName) > -1){
            console.log(data.PlayerName + ' player exit ' + data.RoomName + ' room!');
            socket.leave(data.RoomName);

            // 방을 나간 플레이어에게만
            var aroomList = ROOM_LIST.stringifyRoomList();
            socket.emit('room_list', ROOM_LIST);

            // 플레이어가 나간 방의 다른 플레이어들에게
            var roomInfo = ROOM_LIST.stringifyRoomInfo(data.RoomName);
            io.sockets.in(data.RoomName).emit('room_info', roomInfo);
        }
    });

    socket.on('room_info', function(data) {
        var datas = ROOM_LIST.stringifyRoomInfo(data.RoomName);
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
