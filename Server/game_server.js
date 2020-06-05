var port = Number(process.argv.slice(2,3));
var totalPlayer = Number(process.argv.slice(3));

var express = require('express');
var app = express();

var server = require('http').createServer(app);
var io = require('socket.io')(server);

var TIME_STAMP = 0;

var isFirst = true;

// 상대위치 이동정도 및 절대위치를 보관
var PLAYERS_POSITION = new Object();
    var positions = [];
    for(var j = 0; j < totalPlayer; j++){
        var position = new Object();
        position.x = j * 5;
        position.y = 3.5;
        position.z = j * 5;

        positions.push(position);
    }
PLAYERS_POSITION.Positions = positions;

// 변수 초기화
var INDEX_TO_SOCKET_ID = [];
var SCORE_BOARD = [];
for(var i = 0; i < totalPlayer; i++){
    INDEX_TO_SOCKET_ID.push('');
    SCORE_BOARD.push(0);
}

var loadedPlayerIndex = [];

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
//sendingPosition.PlayerPositions = PLAYERS_POSITION.Positions;

io.on('connection', function(socket) {

    console.log("Connect in child");

    socket.on('player_index', function(data) {
        var playerIndex = data;
        INDEX_TO_SOCKET_ID[playerIndex] = socket.id;
        console.log('player_index ' + data);
        socket.emit('player_index', data);
    });

    socket.on('complete_loading', function(data) {
        var playerIndex = data;
        if(loadedPlayerIndex.indexOf(playerIndex) == -1){
            loadedPlayerIndex.push(playerIndex);
        }
        if(loadedPlayerIndex.length == totalPlayer){
            io.emit('kick_off', '');
        }
    });

    socket.on('absolute_position', function(data){
        if(isFirst){
            TIME_STAMP = Date.now();
            isFirst = false;

            for(var i = 0; i < totalPlayer; i++){
                PLAYERS_POSITION.Positions[i].x = j * 5;
                PLAYERS_POSITION.Positions[i].y = 3.5;
                PLAYERS_POSITION.Positions[i].z = j * 5;
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

        PLAYERS_POSITION.Positions[data.PlayerIndex].x = data.PlayerPosition.x;
        PLAYERS_POSITION.Positions[data.PlayerIndex].y = data.PlayerPosition.y;
        PLAYERS_POSITION.Positions[data.PlayerIndex].z = data.PlayerPosition.z;

        var timeDiff = Date.now() - TIME_STAMP;

        // 20ms마다 절대 좌표 + 공
        if(timeDiff > 40){
            sendingPosition.BallPositions = ballsPositions;
            sendingPosition.PlayerPositions = PLAYERS_POSITION.Positions;
            var datas = JSON.stringify(sendingPosition);
            //console.log('절대' + datas);

            io.emit('absolute_position', datas);
            TIME_STAMP = Date.now();
        }
    });

    socket.on('score', function(data){
        // data = (Scorer,  Conceder, IsFeverBall);
        var scorer = data.Scorer;
        var conceder = data.Conceder;
        var flag = data.IsFeverBall;

        // 골 점수 처리하는 부분
        if(flag == 1){
            SCORE_BOARD[scorer] += 3;
            SCORE_BOARD[conceder] -= 2;
            if(SCORE_BOARD[conceder] < 0){
                SCORE_BOARD[conceder] = 0;
            }
        }
        else{
            SCORE_BOARD[scorer] += 2;
            SCORE_BOARD[conceder] -= 1;
            if(SCORE_BOARD[conceder] < 0){
                SCORE_BOARD[conceder] = 0;
            }
        }

        var sendingData = new Object();
        sendingData.ScoreBoard = SCORE_BOARD;
        var datas = JSON.stringify(sendingData);
        io.emit('score', datas);
    });

    socket.on('end_game', function(data){

    })

    socket.on('disconnect', function(data){
        var i;
        for(i = 0; i < totalPlayer; i++){
            if(INDEX_TO_SOCKET_ID[i] == socket.id){
                break;
            }
        }
        console.log(i+"player is disconnected in "+ port+' '+socket.id);
        io.emit('disconnection', JSON.stringify(i));
    });

    socket.on('disconnection', function(data) {
        console.log('in disconnection '+ socket.id);
        socket.disconnect(true);
    })

});

server.listen(port, function(){
    console.log('Game server listening on port=' + port+ ' totalCount='+totalPlayer);
})
