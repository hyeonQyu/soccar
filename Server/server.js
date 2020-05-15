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

const totalPlayer = 4;
var playerIndex = 0;

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
    var i = 0;
    for(;i<this.listSize;i++){
        if(this.rooms[i].roomName == roomName){
            this.rooms[i].playerNames[this.rooms[i].playerCounts] = playerName;
            this.rooms[i].playerCounts++;
            return i;
        }
    }
    retrun -1;
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
    retrun -1;
}
RoomList.prototype.removeRoom = function(roomName){
    var removePos = this.findRoom(roomName);

    if(removePos > -1){
        this.rooms.splice(removePos,1);
        this.listSize--;
        return true;
    }
    return false;
}
RoomList.prototype.length = function(){
    return this.listSize;
}
RoomList.prototype.clear = function(){
    this.rooms = [];
    this.listSize = 0;
    this.pos = 0;
}

var roomlist = new RoomList();

roomlist.createRoom('firstRoom', 'Lee');

port = ['9091'];
var child = cp.fork("game_server.js", port);

io.on('connection', function(socket) {

    console.log("Connect");

    socket.on('room_list', function(data) {
        // for Debuging
        console.log('in room_list');

        var RoomNames = [];
        var Headcounts = [];
        for(var i = 0; i < roomlist.listSize; i++){
            RoomNames.push(roomlist.rooms[i].roomName);
            Headcounts.push(roomlist.rooms[i].playerCounts);
        }
        var sendingData = new Object();
        sendingData.RoomNames = RoomNames;
        sendingData.Headcounts = Headcounts;

        var datas = JSON.stringify(sendingData);
        console.log(datas);
        socket.emit('room_list', datas);
    });


    socket.on('create_room', function(data) {
        //for debuging
        console.log('in create_room');

        roomlist.createRoom(data.RoomName, data.PlayerName);
        var sendingData = new Object();
        sendingData.RoomName = data.RoomName;
        sendingData.PlayerNames = roomlist.getNames(data.RoomName);

        var datas = JSON.stringify(sendingData);
        console.log(datas);
        socket.emit('room_info', datas);
    });

    socket.on('enter_room', function(data){
        //for debuging
        console.log('in enter_room');

        roomlist.addPlayer(data.RoomName, data.PlayerName);
        var sendingData = new Object();
        sendingData.RoomName = data.RoomName;
        sendingData.PlayerNames = roomlist.getNames(data.RoomName);

        var datas = JSON.stringify(sendingData);
        console.log(datas);
        socket.emit('room_info', datas);
    });


    socket.on('room_info', function(data) {
        var sendingData = new Object();
        sendingData.RoomName = data.RoomName;
        sendingData.PlayerNames = roomlist.getNames(data.RoomName);

        var datas = JSON.stringify(sendingData);
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
