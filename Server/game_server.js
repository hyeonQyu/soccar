var port = Number(process.argv.slice(2));
var totalPlayer = Number(process.argv.slice(3));
const GAME_START = 'start';
const REQUEST_PLAYER_INDEX = 'req';

var io = require('socket.io').listen(port);


console.log('PortNum='+port+' room starts game, Total_Player='+totalPlayer);

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
        position.x = j * 5;
        position.y = 3.5;
        position.z = j * 5;

        positions.push(position);
    }
    playersPosition.Positions = positions;
    playersPositions.push(playersPosition);
}

// 2개의 공 절대위치를 보관
var ballsPositions = [];
for(var i = 0; i < 2; i++){
    var position = new Object();
    position.x = 18.2 + i * 5;
    position.y = 33.3;
    position.z = -18.7;

    ballsPositions.push(position);
}

var sendingPosition = new Object();
sendingPosition.BallPositions = ballsPositions;
//sendingPosition.PlayerPositions = playersPositions[1].Positions;

io.on('connection', function(socket) {

    console.log("Connect in child");

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

        playersPositions[0].Positions[data.PlayerIndex].x += data.Position.x;
        playersPositions[0].Positions[data.PlayerIndex].y += data.Position.y;
        playersPositions[0].Positions[data.PlayerIndex].z += data.Position.z;
    });

    socket.on('absolute_position', function(data){
        if(isFirst[1]){
            timestamp[1] = Date.now();
            isFirst[1] = false;

            for(var i = 0; i < totalPlayer; i++){
                playersPositions[1].Positions[i].x = j * 5;
                playersPositions[1].Positions[i].y = 3.5;
                playersPositions[1].Positions[i].z = j * 5;
            }
        }

        //  슈퍼클라이언트에게서 공의 절대위치 수신
        if(data.PlayerIndex == 0){
            for(var i = 0; i < 2; i++){
                ballsPositions[i].x = data.BallPositions[i].x;
                ballsPositions[i].y = data.BallPositions[i].y;
                ballsPositions[i].z = data.BallPositions[i].z;
            }
        }

        playersPositions[1].Positions[data.PlayerIndex].x = data.PlayerPosition.x;
        playersPositions[1].Positions[data.PlayerIndex].y = data.PlayerPosition.y;
        playersPositions[1].Positions[data.PlayerIndex].z = data.PlayerPosition.z;

        var timeDiff1 = Date.now() - timestamp[0];
        var timeDiff2 = Date.now() - timestamp[1];

        // 20ms마다 절대 좌표 + 공
        if(timeDiff2 > 20){
            sendingPosition.BallPositions = ballsPositions;
            sendingPosition.PlayerPositions = playersPositions[1].Positions;
            var datas = JSON.stringify(sendingPosition);
            console.log('절대' + datas);

            io.emit('absolute_position', datas);
            for(var i = 0; i < totalPlayer; i++){
                playersPositions[0].Positions[i].x = 0;
                playersPositions[0].Positions[i].y = 0;
                playersPositions[0].Positions[i].z = 0;
            }
            timestamp[1] = Date.now();
        }
        /* Lerp
        // 20ms마다 상대 좌표 + 공 전송
        else if(timeDiff1 > 20){
            sendingPosition.BallPositions = ballsPositions;
            sendingPosition.PlayerPositions = playersPositions[0].Positions;
            var datas = JSON.stringify(sendingPosition);
            //console.log('상대' + datas);

            io.emit('relative_position', datas);
            for(var i = 0; i < totalPlayer; i++){
                playersPositions[0].Positions[i].x = 0;
                playersPositions[0].Positions[i].y = 0;
                playersPositions[0].Positions[i].z = 0;
            }

            timestamp[0] = Date.now();
        }
        */
    });


});
