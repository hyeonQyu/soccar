var port = Number(process.argv.slice(2,3));
var totalPlayer = Number(process.argv.slice(3));

var express = require('express');
const { send } = require('process');
var app = express();

var server = require('http').createServer(app);
var io = require('socket.io')(server);

var TRANSFORM_TIME_STAMP = 0;
var RUNNING_TIME_STAMP = 0;

var SUPER_CLIENT_INDEX = 0;

var isFirst = true;
var isEnd = false;
var isFever = false;

var CONNECTED_CLIENT_COUNT = totalPlayer;
var BALL_COUNT = 2;

// 상대위치 이동정도 및 절대위치를 보관
var PLAYERS_TRANFORM = new Object();
    var positions = [];
    var rotations = [];
    var playerSpeeds = [];
    var animHashCodes = [];
    var shootPowers = []
    for(var j = 0; j < totalPlayer; j++){
        var position = new Object();
        var rotation = new Object();
        position.x = 0;
        position.y = 0;
        position.z = 0;

        rotation.x = 0;
        rotation.y = 0;
        rotation.z = 0;

        playerSpeeds.push(0);
        animHashCodes.push(0);
        shootPowers.push(0);

        positions.push(position);
        rotations.push(rotation);
    }
PLAYERS_TRANFORM.positions = positions;
PLAYERS_TRANFORM.playerSpeeds = playerSpeeds;
PLAYERS_TRANFORM.animHashCodes = animHashCodes;
PLAYERS_TRANFORM.rotations = rotations;
PLAYERS_TRANFORM.shootPowers = shootPowers;

// 변수 초기화
var INDEX_TO_SOCKET_ID = [];
var SCORE_BOARD = [];
var GOAL_COUNTS = [];
var PLAYERS_NAME = [];
for(var i = 0; i < totalPlayer; i++){
    INDEX_TO_SOCKET_ID.push('');
    PLAYERS_NAME.push('');
    SCORE_BOARD.push(0);
    GOAL_COUNTS.push(0);
}

var LOADED_PLAYER_INDEX = [];

// 2개의 공 절대위치를 보관
var BALLS_TRANSFORM = new Object();
var ballPositions = [];
var ballRotations = [];
for(var i = 0; i < 2; i++){
    var position = new Object();
    var rotation = new Object();
    position.x = 0;
    position.y = 8;
    position.z = 0;
    rotation.x = 0;
    rotation.y = 0;
    rotation.z = 0;

   ballPositions.push(position);
   ballRotations.push(rotation);
}
BALLS_TRANSFORM.positions = ballPositions;
BALLS_TRANSFORM.rotations = ballRotations;

var sendingTransform = new Object();

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
            RUNNING_TIME_STAMP = Date.now();
        }
    });

    socket.on('transform', function(data){
        if(isFirst){
            TRANSFORM_TIME_STAMP = Date.now();
            isFirst = false;
        }
        var ballCount = data.BallPositions;
        //  슈퍼클라이언트에게서 공의 절대위치 수신
        if(data.PlayerIndex == SUPER_CLIENT_INDEX){
            for(var i = 0; i < Object.keys(ballCount).length; i++){
                BALLS_TRANSFORM.positions[i].x = data.BallPositions[i].x;
                BALLS_TRANSFORM.positions[i].y = data.BallPositions[i].y;
                BALLS_TRANSFORM.positions[i].z = data.BallPositions[i].z;

                BALLS_TRANSFORM.rotations[i].x = data.BallRotations[i].x;
                BALLS_TRANSFORM.rotations[i].y = data.BallRotations[i].y;
                BALLS_TRANSFORM.rotations[i].z = data.BallRotations[i].z;
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
            PLAYERS_TRANFORM.shootPowers[data.PlayerIndex] = data.ShootPower;
            PLAYERS_TRANFORM.animHashCodes[data.PlayerIndex] = data.AnimHashCode
        }

        
        var timeDiff = Date.now() - TRANSFORM_TIME_STAMP;

        if(!isFever && Date.now() - RUNNING_TIME_STAMP > 240000){
            console.log('fever time !! in port' + port);
            isFever = true;
            io.emit('fever_time', " ");
            var position = new Object();
            var rotation = new Object();
            position.x = 0;
            position.y = 8;
            position.z = 0;
            rotation.x = 0;
            rotation.y = 0;
            rotation.z = 0;

            BALLS_TRANSFORM.positions.push(position);
            BALLS_TRANSFORM.rotations.push(rotation);
            BALL_COUNT += 1;
        }

        if(!isEnd && Date.now() - RUNNING_TIME_STAMP > 300000){
            isEnd = true;
            var winnerIndex = 0;
            for(var i = 1; i < totalPlayer; i++){
                if(SCORE_BOARD[i] < SCORE_BOARD[winnerIndex]){
                    continue;
                }
                else if(SCORE_BOARD[i] == SCORE_BOARD[winnerIndex]){
                    if(GOAL_COUNTS[i] > GOAL_COUNTS[winnerIndex]){
                        winnerIndex = i;
                    }
                }
                else{
                    winnerIndex = i;
                }
            }
            console.log('End Game in port '+ port);
            io.emit('end_game', JSON.stringify(winnerIndex));
        }
        // 40ms마다 절대 좌표 + 공
        else if(timeDiff > 25){
            sendingTransform.BallPositions = BALLS_TRANSFORM.positions;
            sendingTransform.BallRotations = BALLS_TRANSFORM.rotations;
            sendingTransform.PlayerPositions = PLAYERS_TRANFORM.positions;
            sendingTransform.PlayerRotations = PLAYERS_TRANFORM.rotations;
            sendingTransform.AnimHashCodes = PLAYERS_TRANFORM.animHashCodes;
            sendingTransform.PlayerSpeeds = PLAYERS_TRANFORM.playerSpeeds;
            sendingTransform.ShootPowers = PLAYERS_TRANFORM.shootPowers;
            var datas = JSON.stringify(sendingTransform);
            //console.log('절대' + datas);

            io.emit('transform', datas);
            TRANSFORM_TIME_STAMP = Date.now();
            for(var i = 0; i < totalPlayer; i++){
                PLAYERS_TRANFORM.animHashCodes[i] = 0;
                PLAYERS_TRANFORM.shootPowers[i] = 0;
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
            GOAL_COUNTS[scorer] += 1;
            SCORE_BOARD[conceder] -= 2;
            if(SCORE_BOARD[conceder] < 0){
                SCORE_BOARD[conceder] = 0;
            }
        }
        else{
            SCORE_BOARD[scorer] += 2;
            GOAL_COUNTS[scorer] += 1;
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

    socket.on('tackle_event', function(data){
        console.log('in tackle_event : '+ data.PlayerIndex +' is tackeld!');
        var sendingData = new Object();
        sendingData.PlayerIndex = data.PlayerIndex;
        sendingData.PlayerPosition = new Object();
        sendingData.PlayerPosition.x = data.PlayerPosition.x;
        sendingData.PlayerPosition.y = data.PlayerPosition.y;
        sendingData.PlayerPosition.z = data.PlayerPosition.z;

        var datas = JSON.stringify(sendingData);
        io.emit('tackle_event', datas);
    });

    socket.on('disconnect', function(data){
        var disconnectedIndex;
        for(disconnectedIndex = 0; disconnectedIndex < totalPlayer; disconnectedIndex++){
            if(INDEX_TO_SOCKET_ID[disconnectedIndex] == socket.id){
                break;
            }
        }
        console.log(disconnectedIndex+"player is disconnected in "+ port+' '+socket.id);
        SCORE_BOARD[disconnectedIndex] = -1;
        CONNECTED_CLIENT_COUNT -= 1;

        if(!isEnd && CONNECTED_CLIENT_COUNT == 1){
            var winnerIndex = 0;
            for(var j = 1; j < totalPlayer; j++){
                if(SCORE_BOARD[j] < SCORE_BOARD[winnerIndex]){
                    continue;
                }
                else if(SCORE_BOARD[j] == SCORE_BOARD[winnerIndex]){
                    if(GOAL_COUNTS[j] > GOAL_COUNTS[winnerIndex]){
                        winnerIndex = j;
                    }
                }
                else{
                    winnerIndex = j;
                }
            }
            console.log('End Game in port '+ port);
            io.emit('end_game', JSON.stringify(winnerIndex));
        }

        else if(CONNECTED_CLIENT_COUNT == 0){
            process.exit(1);
        }
        io.emit('disconnection', JSON.stringify(disconnectedIndex));
    });

    socket.on('disconnection', function(data) {
        console.log('in disconnection '+ socket.id);
        socket.disconnect(true);
    });

});

server.listen(port, function(){
    console.log('Game server listening on port=' + port+ ' totalCount='+totalPlayer);
})
