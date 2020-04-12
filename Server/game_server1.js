const GAME_START = 'start';
const REQUEST_PLAYER_INDEX = 'req';

var express = require('express');
var app = express();

var server = require('http').createServer(app);
var io = require('socket.io')(server);

var playerIndex = 0;

app.get('/', function(req, res) {
  res.send("game server");
});

io.on('connection', function(socket) {

    console.log("Connect " + socket.id);

    socket.on('request_player_index', function(data) {
        console.log('request_player_index ' + data);

        if(data == REQUEST_PLAYER_INDEX) {
            // Ŭ���̾�Ʈ���� ���� ã���ְ� �˸��� �÷��̾� �ε����� ����
            var playerIndexString = playerIndex.toString();
            socket.emit('request_player_index', playerIndexString);
            playerIndex++;
        }
    });

    socket.on('player_motion', function(data) {
        console.log('player_motion ' + data.PlayerIndex);
        var position = data.Position;
        console.log(position.x + ' ' + position.y + ' ' + position.z);

        //// �ش� ���ӹ濡 �ִ� ������ Ŭ���̾�Ʈ�� ������ ���� Ŭ���̾�Ʈ���� ��ġ ���� ����
        //socket.broadcast.emit('player_motion', data);
        // ���� Ŭ���̾�Ʈ���� ��ġ ���� ����
        io.emit('player_motion', data);
    });

});

server.listen(9091, function() {
    console.log('Socket IO server listening on port 9091');
});




    //// ���ӵ� ���� Ŭ���̾�Ʈ���� �޽����� �����Ѵ�
    //io.emit('event_name', msg);

    //// �޽����� ������ Ŭ���̾�Ʈ���Ը� �޽����� �����Ѵ�
    //socket.emit('event_name', msg);

    //// �޽����� ������ Ŭ���̾�Ʈ�� ������ ���� Ŭ���̾�Ʈ���� �޽����� �����Ѵ�
    //socket.broadcast.emit('event_name', msg);

    //// Ư�� Ŭ���̾�Ʈ���Ը� �޽����� �����Ѵ�
    //io.to(id).emit('event_name', data);
