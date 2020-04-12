const GAME_START = 'start';
const GAME_SERVER1 = '9091'
const GAME_SERVER2 = '9092'

var express = require('express');
var app = express();
var compression = require('compression');

var server = require('http').createServer(app);
var io = require('socket.io')(server);

app.use(compression());
app.use(express.static(__dirname + '/client/'));

var server_num = 0;

app.get('/', function(req, res) {

});

io.on('connection', function(socket) {

    console.log("Connect " + socket.id);

    socket.on('start_button', function(data) {
        console.log('start_button ' + data);
        /* �������� timestamp�� ���ϴ� �ڵ�
        console.log('time ' + Date.now()); */

        // Ŭ���̾�Ʈ�� ���� ���� ��ư�� �������� ���� ���� �޽��� ����
        if(data == GAME_START) {
          if(server_num == 1){
            socket.emit('start_button', GAME_SERVER1);
            server_num = 0;
          }
          else {
            socket.emit('start_button', GAME_SERVER2);
            server_num = 1;
          }
        }
    });

    socket.on('request_player_index', function(data) {
        console.log('request_player_index ' + data);

        if(data == REQUEST_PLAYER_INDEX) {
            // Ŭ���̾�Ʈ���� ���� ã���ְ� �˸��� �÷��̾� �ε����� ����
            var playerIndexString = playerIndex.toString();
            socket.emit('request_player_index', playerIndexString);
            playerIndex++;
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
