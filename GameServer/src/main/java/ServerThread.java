import java.io.IOException;
import java.io.InputStream;
import java.io.OutputStream;
import java.net.Socket;
import java.util.Enumeration;
import java.util.Hashtable;

import com.fasterxml.jackson.core.JsonProcessingException;

public class ServerThread extends Thread {
	private Socket socket;
	private OutputStream sender;
	private InputStream receiver;
	private int clientIndex;
	private int maybeDisconnected = 0; // 클라이언트의 연결해제 상태를 예측하기 위함.
	private int roomIndex = 0; // Hashtable roomList의 clientTable index.

	ServerThread(Socket s, int index) {
		this.socket = s;
		this.clientIndex = index;
	}

	// 오버라이딩일 경우 throw 불가.
	public void run() {
		try {
			service();
		} catch (IOException e) {

		} finally {
			try {
				closeAll();
			} catch (IOException e) {
				// TODO Auto-generated catch block
				e.printStackTrace();
			}
		}
	}

	private void service() throws IOException {

		{
			try {
				sender = socket.getOutputStream();
				receiver = socket.getInputStream();

				System.out.println("Client Index = " + clientIndex);

				while (socket.isConnected() && !socket.isClosed()) {
					receivePacket(socket);
					System.out.println("완료 코드 보내기");
				}

			} catch (Throwable e) {
				e.printStackTrace();
			} finally {
				if (socket != null) {
					System.out.println(clientIndex + " 손님 한명 퇴장함");
					
					Server.clientTable.remove(clientIndex);
					Server.roomList.get(roomIndex).remove(clientIndex);
					
					socket.close();
					sender.close();
					receiver.close();
				}
			}

		}
	}
	
	private void receivePacket(Socket socket) throws IOException, ClassNotFoundException {
		if (!socket.isClosed()) {
			System.out.println("Waiting for Receiving Data");
			
			// 해더 패킷 분석
			byte[] byteHeader = new byte[Packet.LENGTH_HEADER];
			Packet.Header header = new Packet.Header(-1);
			receiver.read(byteHeader, 0, Packet.LENGTH_HEADER);
			String jsonHeader = new String(byteHeader);
			header = (Packet.Header) JsonManager.jsonToObject(jsonHeader, Packet.Header.class);

			// 패킷 내용 분석
			int lengthBody = header.getLength();
			byte[] byteBody = new byte[lengthBody];
			receiver.read(byteBody, 0, lengthBody);
			String jsonBody = new String(byteBody);

			switch (header.getMsgType()) {
			
			case Packet.TYPE_START_BUTTON_SEND:
				Packet.StartButtonSend startButtonSend = 
						(Packet.StartButtonSend) JsonManager.jsonToObject(jsonBody, Packet.StartButtonSend.class);
				
				if (startButtonSend.getCheckNum() == Server.GAME_START_PACKET) {
					// Send 할 패킷구성
					Packet.StartButtonAck startButtonAck = new Packet.StartButtonAck();
					startButtonAck.setCheckNum(Server.GAME_START_PACKET);
					sendPacket(Packet.TYPE_START_BUTTON_ACK, startButtonAck);
				}
				break;
				
			case Packet.TYPE_REQUEST_PLAYER_INDEX_SEND:
				Packet.RequestPlayerIndexSend requestPlayerIndexSend = 
						(Packet.RequestPlayerIndexSend) JsonManager.jsonToObject(jsonBody, Packet.RequestPlayerIndexSend.class);
				
				if (requestPlayerIndexSend.getCheckNum() == Server.REQUEST_PLAYER_INDEX) {
					int state;
					state = findRoom();
					
					if (state == 1) {
						System.out.println("GameStart now successfully");
					} else if (state == -1) {
						System.out.println("Something is wrong");
					} else {
						System.out.println("Waiting other player in : " + roomIndex);
					}
				}
				break;
				
			case Packet.TYPE_PLAYER_MOTION_SEND:
				Packet.PlayerMotionSend playerMotionSend = 
							(Packet.PlayerMotionSend) JsonManager.jsonToObject(jsonBody, Packet.PlayerMotionSend.class);
				Enumeration<Integer> en = Server.clientTable.keys();
				
				// clientTable을 돌며 모든 클라이언트에게 위치값 전송
				while (en.hasMoreElements()) { 
					int key = en.nextElement();
					sender = Server.clientTable.get(key).getOutputStream();
					sendPacket(Packet.TYPE_PLAYER_MOTION_ACK, playerMotionSend);
				}
				
				sender = socket.getOutputStream();
				break;
			}
		}
	}

	private void sendPacket(int msgType, Object body) throws JsonProcessingException {
		Packet.Header header = new Packet.Header(msgType);

		// 패킷 해더와 몸체 바이트화
		byte[] byteBody = JsonManager.objectToByte(body);
		System.out.println(new String(byteBody));
		header.setLength(byteBody.length);
		byte[] byteHeader = JsonManager.objectToByte(header);
		System.out.println(new String(byteHeader));

		// 전송할 패킷 구성(byteHeader + byteBody)
		byte[] bytePacket = new byte[byteHeader.length + byteBody.length];
		System.arraycopy(byteHeader, 0, bytePacket, 0, byteHeader.length);
		System.arraycopy(byteBody, 0, bytePacket, byteHeader.length, byteBody.length);

		try {
			sender.write(bytePacket);
		} catch (IOException e) {
			e.printStackTrace();
		}
	}

	public void closeAll() throws IOException {
		if (socket != null)
			socket.close();
	}


	public int findRoom() {
		int roomKey = 0;
		Enumeration<Integer> e = Server.roomList.keys();
		
		while (e.hasMoreElements()) {
			roomKey = e.nextElement();
		}
		
		if (Server.roomList.get(roomKey).size() < Server.MAX_PLAYER_PER_ROOM) {
			Server.roomList.get(roomKey).put(clientIndex, socket);
			roomIndex = roomKey;
			
			if (Server.roomList.get(roomKey).size() == Server.MAX_PLAYER_PER_ROOM) {
				// Send GameStart Signal all client in the full room
				e = null;
				e = Server.roomList.get(roomKey).keys();
				int playerIndex = 0;
				
				try {
					while (e.hasMoreElements()) {
						int clientKey = e.nextElement();
						sender = Server.roomList.get(roomKey).get(clientKey).getOutputStream();
						Packet.RequestPlayerIndexAck requestPlayerIndexAck = new Packet.RequestPlayerIndexAck();
						requestPlayerIndexAck.setPlayerIndex(playerIndex++);
						sendPacket(Packet.TYPE_REQUEST_PLAYER_INDEX_ACK, requestPlayerIndexAck);
					}
					sender = socket.getOutputStream();
				} catch (IOException e1) {
					e1.printStackTrace();
				}
				
				Server.roomList.put(Server.roomListCounts++, new Hashtable<Integer, Socket>());
				return 1; // Start new game when the room is full
			}
			return 0; // 0 = the room is not full
		} else { // 여기서 밑에 구문을 실행 할지 size == MAX_PLAYER…. 여기 if문 안에서 할지.
			// Server.roomList.put(Server.roomListCounts++, new Hashtable<Integer,
			// Socket>());
		}
		return -1; // Something is wrong
	}

}