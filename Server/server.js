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
    this.RoomName;
    this.PlayerNames=[]
    this.num_player = 0;
}

// RoomList 정의 및 method 생성
var RoomList = function(){
    this.Rooms = []
    this.pos = 0;
    this.listSize = 0;
}
RoomList.prototype.create_room = function(room_name, player_name){
    var room = new Room();
    room.RoomName = room_name;
    room.PlayerNames[0] = player_name;
    room.num_player = 1;
    this.Rooms.push(room);
    this.listSize++;
}
RoomList.prototype.add_player = function(room_name, player_name){
    var i = 0;
    for(;i<this.listSize;i++){
        if(this.Rooms[i].RoomName == room_name){
            this.Rooms[i].PlayerNames[this.Rooms[i].num_player] = player_name;
            this.Rooms[i].num_player++;
            return i;
        }
    }
    retrun -1;
}

RoomList.prototype.find_room = function(room_name){
    var i = 0;
    for(;i<this.listSize;i++){
        if(this.Rooms[i].RoomName == room_name){
            return i;
        }
    }
    retrun -1;
}

RoomList.prototype.remove_room = function(room_name){
    var removePos = this.find(room_name);

    if(removePos > -1){
        this.Rooms.splice(removePos,1);
        this.listSize--;
        return true;
    }
    return false;
}
RoomList.prototype.length = function(){
    return this.listSize;
}
RoomList.prototype.clear = function(){
    this.Rooms = [];
    this.listSize = 0;
    this.pos = 0;
}

var roomlist = new RoomList();

port = ['9091'];
var child = cp.fork("game_server.js", port);

io.on('connection', function(socket) {

    console.log("Connect");


    socket.on('start_button', function(data) {
        console.log('start_button ' + data);

        if(data == GAME_START) {
            socket.emit('start_button', GAME_START);
        }
    });


    socket.on('room_list', function(data) {
        var RoomNames = [];
        var Headcounts = [];
        for(var i = 0; i < roomlist.listSize; i++){
            RoomNames.push(roomlist.Rooms[i].RoomName);
            Headcounts.push(roomlist.Rooms[i].num_player);
        }
        var sendingData = new Object();
        sendingData.RoomNames = RoomNames;
        sendingData.Headcounts = Headcounts;

        var datas = JSON.stringify(sendingData);
        socket.emit('room_list', datas);
    });


    socket.on('create_room', function(data) {
        
    });


    socket.on('room_info', function(data) {

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
