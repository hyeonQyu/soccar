var port = Number(process.argv.slice(2,3));
var totalPlayer = Number(process.argv.slice(3));

var express = require('express');
var app = express();

var server = require('http').createServer(app);
var io = require('socket.io')(server);

var playerIndex = 0;

var timestamp = 0;

var isFirst = true;

// 상대위치 이동정도 및 절대위치를 보관
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
//sendingPosition.PlayerPositions = playersPosition.Positions;

io.on('connection', function(socket) {

    console.log("Connect in child");

    socket.on('player_index', function(data) {
        console.log('player_index ' + data);
        socket.emit('player_index', data);
    });

    socket.on('absolute_position', function(data){
        if(isFirst){
            timestamp = Date.now();
            isFirst = false;

            for(var i = 0; i < totalPlayer; i++){
                playersPosition.Positions[i].x = j * 5;
                playersPosition.Positions[i].y = 3.5;
                playersPosition.Positions[i].z = j * 5;
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

        playersPosition.Positions[data.PlayerIndex].x = data.PlayerPosition.x;
        playersPosition.Positions[data.PlayerIndex].y = data.PlayerPosition.y;
        playersPosition.Positions[data.PlayerIndex].z = data.PlayerPosition.z;

        var timeDiff = Date.now() - timestamp;

        // 20ms마다 절대 좌표 + 공
        if(timeDiff > 40){
            sendingPosition.BallPositions = ballsPositions;
            sendingPosition.PlayerPositions = playersPosition.Positions;
            var datas = JSON.stringify(sendingPosition);
            //console.log('절대' + datas);

            io.emit('absolute_position', datas);
            timestamp = Date.now();
        }
    });

});

server.listen(port, function(){
    console.log('Game server listening on port ' + port);
})
