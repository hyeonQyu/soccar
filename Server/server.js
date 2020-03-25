const GAME_START = 'start';
const REQUEST_PLAYER_INDEX = 'req';

var express = require('express');
var app = express();
var compression = require('compression');

var server = require('http').createServer(app);
var io = require('socket.io')(server);

app.use(compression());
app.use(express.static(__dirname + '/client/'));

var playerIndex = 0;

app.get('/', function(req, res) {
    
});

io.on('connection', function(socket) {

    console.log("Connect");

    socket.on('start_button', function(data) {
        console.log('start_button ' + data);
        
        // 클라이언트가 게임 시작 버튼을 눌렀으면 게임 시작 메시지 전송
        if(data == GAME_START) {
            socket.emit('start_button', GAME_START);
        }
    });

    socket.on('request_player_index', function(data) {
        console.log('request_player_index ' + data);

        if(data == REQUEST_PLAYER_INDEX) {    
            // 클라이언트에게 방을 찾아주고 알맞은 플레이어 인덱스를 전송
            var playerIndexString = playerIndex.toString();
            socket.emit('request_player_index', playerIndexString);
            playerIndex++;
        }
    });

    socket.on('player_motion', function(data) {
        console.log('player_motion');
        console.log('player_motion ' + data.X + ' ' + data.Y + ' ' + data.Z);
        
        // 해당 게임방에 있는 모든 클라이언트에게 위치 정보 전송
        io.emit('player_motion', data);
    });

});

server.listen(9090, function() {
    console.log('Socket IO server listening on port 9090');
});




    //// 접속된 모든 클라이언트에게 메시지를 전송한다
    //io.emit('event_name', msg);

    //// 메시지를 전송한 클라이언트에게만 메시지를 전송한다
    //socket.emit('event_name', msg);

    //// 메시지를 전송한 클라이언트를 제외한 모든 클라이언트에게 메시지를 전송한다
    //socket.broadcast.emit('event_name', msg);

    //// 특정 클라이언트에게만 메시지를 전송한다
    //io.to(id).emit('event_name', data);