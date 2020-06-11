var port = Number(process.argv.slice(2,3));
var totalPlayer = Number(process.argv.slice(3));

var express = require('express');
var app = express();

var server = require('http').createServer(app);
var io = require('socket.io')(server);

var TIME_STAMP = 0;

var SUPER_CLIENT_INDEX = 0;

var isFirst = true;

var CONNECTED_CLIENT_COUNT = totalPlayer;

// 상대위치 이동정도 및 절대위치를 보관
var PLAYERS_TRANFORM = new Object();
    var positions = [];
    var rotations = [];
    var playerSpeeds = [];
    var animHashCodes = [];
    for(var j = 0; j < totalPlayer; j++){
        var position = new Object();
        var rotation = new Object();
        position.x = j * 5;
        position.y = 3.5;
        position.z = j * 5;

        rotation.x = 0;
        rotation.y = 0;
        rotation.z = 0;

        playerSpeeds.push(0);
        animHashCodes.push(0);

        positions.push(position);
        rotations.push(rotation);
    }
PLAYERS_TRANFORM.positions = positions;
PLAYERS_TRANFORM.playerSpeeds = playerSpeeds;
PLAYERS_TRANFORM.animHashCodes = animHashCodes;
PLAYERS_TRANFORM.rotations = rotations;

// 변수 초기화
var INDEX_TO_SOCKET_ID = [];
var SCORE_BOARD = [];
var PLAYERS_NAME = [];
for(var i = 0; i < totalPlayer; i++){
    INDEX_TO_SOCKET_ID.push('');
    PLAYERS_NAME.push('');
    SCORE_BOARD.push(0);
}

var LOADED_PLAYER_INDEX = [];

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
//sendingPosition.PlayerPositions = PLAYERS_TRANFORM.positions;

io.on('connection', function(socket) {

    console.log("Connect in child");

    socket.on('player_index', function(data) {
        var playerIndex = data;
        INDEX_TO_SOCKET_ID[playerIndex] = socket.id;
        console.log('player_index ' + data);
        socket.emit('player_index', data);
    });

    socket.on('complete_loading', function(data) {
        var playerIndex = data.PlayerIndex;
        var playerName = data.PlayerName;
        if(LOADED_PLAYER_INDEX.indexOf(playerIndex) == -1){
            LOADED_PLAYER_INDEX.push(playerIndex);
            PLAYERS_NAME[playerIndex] = playerName;
        }
        if(LOADED_PLAYER_INDEX.length == totalPlayer){
            var sendingData = new Object();
            sendingData.PlayerNames = PLAYERS_NAME;
            var datas = JSON.stringify(sendingData);
            io.emit('kick_off', datas);
        }
    });

    socket.on('transform', function(data){
        if(isFirst){
            TIME_STAMP = Date.now();
            isFirst = false;
        }

        //  슈퍼클라이언트에게서 공의 절대위치 수신
        if(data.PlayerIndex == SUPER_CLIENT_INDEX){
            for(var i = 0; i < 2; i++){
                ballsPositions[i].x = data.BallPositions[i].x;
                ballsPositions[i].y = data.BallPositions[i].y;
                ballsPositions[i].z = data.BallPositions[i].z;
            }
        }

        PLAYERS_TRANFORM.positions[data.PlayerIndex].x = data.PlayerPosition.x;
        PLAYERS_TRANFORM.positions[data.PlayerIndex].y = data.PlayerPosition.y;
        PLAYERS_TRANFORM.positions[data.PlayerIndex].z = data.PlayerPosition.z;
        PLAYERS_TRANFORM.rotations[data.PlayerIndex].x = data.PlayerRotation.x;
        PLAYERS_TRANFORM.rotations[data.PlayerIndex].y = data.PlayerRotation.y;
        PLAYERS_TRANFORM.rotations[data.PlayerIndex].z = data.PlayerRotation.z;

        PLAYERS_TRANFORM.playerSpeeds[data.PlayerIndex] = data.PlayerSpeed;
        if(PLAYERS_TRANFORM.animHashCodes[data.PlayerIndex] == 0){
            PLAYERS_TRANFORM.animHashCodes[data.PlayerIndex] = data.AnimHashCode
        }

        var timeDiff = Date.now() - TIME_STAMP;

        // 40ms마다 절대 좌표 + 공
        if(timeDiff > 40){
            sendingPosition.BallPositions = ballsPositions;
            sendingPosition.PlayerPositions = PLAYERS_TRANFORM.positions;
            sendingPosition.PlayerRotations = PLAYERS_TRANFORM.rotations;
            sendingPosition.AnimHashCodes = PLAYERS_TRANFORM.animHashCodes;
            sendingPosition.PlayerSpeeds = PLAYERS_TRANFORM.playerSpeeds;
            var datas = JSON.stringify(sendingPosition);
            //console.log('절대' + datas);

             io.emit('transform', datas);
            TIME_STAMP = Date.now();
            for(var i = 0; i < totalPlayer; i++){
                PLAYERS_TRANFORM.animHashCodes[i] = 0;
            }
        }

    });

    socket.on('change_super_client', function(data){
        SUPER_CLIENT_INDEX  = data;
    });

    socket.on('score', function(data){
        // data = (Scorer,  Conceder, IsFeverBall);
        var scorer = data.Scorer;
        var conceder = data.Conceder;
        var flag = data.IsFeverBall;

        // 골 점수 처리하는 부분
        if(scorer == conceder){
            SCORE_BOARD[scorer] -= 1;
            if(SCORE_BOARD[scorer] < 0){
                SCORE_BOARD[scorer] = 0;
            }
        }
        else if(flag == 1){
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
        sendingData.Scorer = scorer;
        sendingData.Conceder = conceder;
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
        CONNECTED_CLIENT_COUNT -= 1;
        if(CONNECTED_CLIENT_COUNT == 0){
            process.exit(1);
        }
        io.emit('disconnection', JSON.stringify(i));
    });

    socket.on('disconnection', function(data) {
        console.log('in disconnection '+ socket.id);
        socket.disconnect(true);
    });

});

server.listen(port, function(){
    console.log('Game server listening on port=' + port+ ' totalCount='+totalPlayer);
})
