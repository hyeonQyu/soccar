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

var timestamp = [];
timestamp[0] = 0;
timestamp[1] = 0;

var isFirst = [];
isFirst[0] = true;
isFirst[1] = true;

// 상대위치 이동정도 및 절대위치를 보관
var playersPositions = [];
for(var i = 0; i < 2; i++){
    var playersPosition = new Object();
    var positions = [];
    for(var j = 0; j < totalPlayer; j++){
        var position = new Object();
        position.x = 0;
        position.y = 0;
        position.z = 0;

        positions.push(position);
    }
    playersPosition.Positions = positions;
    playersPositions.push(playersPosition);
}

playersPositions[1].Positions = positions;
playersPositions[1].Positions[1].x = -0.09;
playersPositions[1].Positions[1].y = 3;
playersPositions[1].Positions[1].z = -8.6;

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

    socket.on('relative_position', function(data) {
        if(isFirst[0]){
            timestamp[0] = Date.now();
            isFirst[0] = false;
        }

        //console.log(data);
        //console.log('player_motion ' + data.PlayerIndex);

        playersPositions[0].Positions[data.PlayerIndex].x += data.Position.x;
        playersPositions[0].Positions[data.PlayerIndex].y += data.Position.y;
        playersPositions[0].Positions[data.PlayerIndex].z += data.Position.z;

        // 일정 시간이 되면 각 플레이어들에게 서버에서 모은 모든 플레이어들의 위치를 전송
        if(Date.now() - timestamp[0] > 20){
            var datas = JSON.stringify(playersPositions[0]);

            //console.log(datas);
            io.emit('relative_position', datas);
            for(var i = 0; i < totalPlayer; i++){
                playersPositions[0].Positions[i].x = 0;
                playersPositions[0].Positions[i].y = 0;
                playersPositions[0].Positions[i].z = 0;
            }

            timestamp[0] = Date.now();
        }

    });

    socket.on('absolute_position', function(data){
        if(isFirst[1]){
            timestamp[1] = Date.now();
            isFirst[1] = false;
        }

        playersPositions[1].Positions[data.PlayerIndex].x = data.Position.x;
        playersPositions[1].Positions[data.PlayerIndex].y = data.Position.y;
        playersPositions[1].Positions[data.PlayerIndex].z = data.Position.z;

        // 일정 시간이 되면 모든 플레이어의 위치를 서버에서 보관한 위치로 맞춤
        if(Date.now() - timestamp[1] > 500){
            var datas = JSON.stringify(playersPositions[1]);
            io.emit('absolute_position', datas);
            timestamp[1] = Date.now();
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
