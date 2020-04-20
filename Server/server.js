const GAME_START = 'start';
const REQUEST_PLAYER_INDEX = 'req';

var express = require('express');
var app = express();
var compression = require('compression');

var server = require('http').createServer(app);
var io = require('socket.io')(server);

app.use(compression());
app.use(express.static(__dirname + '/client/'));

const totalPlayer = 4;
var playerIndex = 0;

var timestamp = 0;
var isFirst = true;

var playersPosition = new Object();
var positions = [];
for(var i = 0; i < totalPlayer; i++){
    var position = new Object();
    position.x = i;
    position.y = 0;
    position.z = 0;

    positions.push(position);
}
playersPosition.Positions = positions;

app.get('/', function(req, res) {

});

io.on('connection', function(socket) {

    console.log("Connect");

    socket.on('start_button', function(data) {
        console.log('start_button ' + data);

        if(data == GAME_START) {
            socket.emit('start_button', GAME_START);
        }
    });

    socket.on('request_player_index', function(data) {
        console.log('request_player_index ' + data);

        if(data == REQUEST_PLAYER_INDEX) {
            // 클라이언트에게 플레이어 인덱스 전송
            var playerIndexString = playerIndex.toString();
            socket.emit('request_player_index', playerIndexString);
            playerIndex++;
        }
    });

    socket.on('player_motion', function(data) {
        if(isFirst){
            timestamp = Date.now();
            isFirst = false;
        }

        //console.log(data);
        //console.log('player_motion ' + data.PlayerIndex);

        playersPosition.Positions[data.PlayerIndex].x = data.Position.x;
        playersPosition.Positions[data.PlayerIndex].y = data.Position.y;
        playersPosition.Positions[data.PlayerIndex].z = data.Position.z;
      //  console.log(playersPosition.Positions[data.PlayerIndex].x + ' ' + playersPosition.Positions[data.PlayerIndex].y + ' ' + playersPosition.Positions[data.PlayerIndex].z);

        // 일정 시간이 되면 각 플레이어들에게 서버에서 모은 모든 플레이어들의 위치를 전송
        if(Date.now() - timestamp > 20){
            var datas = JSON.stringify(playersPosition);

            console.log(datas);
            io.emit('player_motion', datas);
            timestamp = Date.now();
        }

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
