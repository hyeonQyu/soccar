import java.io.BufferedReader;
import java.io.ByteArrayInputStream;
import java.io.ByteArrayOutputStream;
import java.io.IOException;
import java.io.InputStream;
import java.io.InputStreamReader;
import java.io.ObjectInput;
import java.io.ObjectInputStream;
import java.io.ObjectOutput;
import java.io.ObjectOutputStream;
import java.io.OutputStream;
import java.io.PrintWriter;
import java.net.Socket;
import java.nio.ByteBuffer;
import java.nio.ByteOrder;
import java.util.Enumeration;
import java.util.Hashtable;

import org.springframework.util.SerializationUtils;

public class ServerThread extends Thread {
	private Socket socket;
	private OutputStream sender;
	private InputStream receiver;
	private int clientIndex;
	private int maybeDisconnected = 0; // 클라이언트의 연결해제 상태를 예측하기 위함.
	private int roomIndex = 0; // Hashtable roomList의 clientTable index.
	private Packet packet;

	ServerThread(Socket s, int index) {
		this.socket = s;
		this.clientIndex = index;
	}

	private void packetRecieve(InputStream stream, Socket socket) throws IOException {
		// 사데이터 사이즈를 받는다.
		if (!socket.isClosed()) {
			System.out.println("Waiting for Receiving Data");
			byte[] byteHeader = new byte[8];
			Packet.Header header = new Packet.Header(-1);
			// 해더패킷을 받고 분석함.
			stream.read(byteHeader, 0, 8);
			header = (Packet.Header) SerializationUtils.deserialize(byteHeader);
			//header = (Packet.Header) byteToObject(byteHeader);

			byte[] byteBody = new byte[header.Length];
			System.out.println("Received Body Data Length = " + header.Length);
			// 데이터를 받는다.
			stream.read(byteBody, 0, header.Length);
			Object body = SerializationUtils.deserialize(byteBody);
			//Object body = byteToObject(byteBody);

			switch (header.MsgType) {
			case Packet.TypeStartButtonSend:
				Packet.StartButtonSend startButtonSend = (Packet.StartButtonSend) body;
				if (startButtonSend.CheckNum == Server.GameStartPacket) {
					// Send 할 패킷구성
					Packet.StartButtonAck startButtonAck = new Packet.StartButtonAck();
					startButtonAck.CheckNum = Server.GameStartPacket;
					packetSend(Packet.TypeStartButtonAck, startButtonAck);
				}
				break;
			case Packet.TypeRequestPlayerIndexSend:
				Packet.RequestPlayerIndexSend requestPlayerIndexSend = (Packet.RequestPlayerIndexSend) body;
				if (requestPlayerIndexSend.CheckNum == Server.RequestPlayerIndex) {
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
			case Packet.TypePlayerMotionSend:
				Enumeration<Integer> en = Server.clientTable.keys();// en에 커서가 있다
				while (en.hasMoreElements()) { // clientTable을 돌며 모든 클라이언트에게 위치값 전송
					int key = en.nextElement();
					sender = Server.clientTable.get(key).getOutputStream();
					packetSend(Packet.TypePlayerMotionAck, body);
				}
				sender = socket.getOutputStream();
				break;

			}

		}

	}

	private void packetSend(int msgType, Object obj) {
		Packet.Header header = new Packet.Header(msgType);

		// 패킷 해더와 몸체 바이트화
		byte[] byteBody = SerializationUtils.serialize(obj);//objectToByte(obj);
		header.Length = byteBody.length;
		byte[] byteHeader = SerializationUtils.serialize(header);//objectToByte(header);

		// 전송할 패킷 구성(byteHeader + byteBody)
		byte[] packet = new byte[byteHeader.length + byteBody.length];
		System.arraycopy(byteHeader, 0, packet, 0, byteHeader.length);
		System.arraycopy(byteBody, 0, packet, byteHeader.length, byteBody.length);

		try {
			sender.write(packet);
		} catch (IOException e) {
			// TODO Auto-generated catch block
			e.printStackTrace();
		}
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
					packetRecieve(receiver, socket);
//
//					if (data.length != 0) {
//						System.out.println("data 수신");
					// System.out.println("Received Data Length = " + data.length);
//						if (data.length == 4) {
//							int num = byteToInt(data);
//							System.out.println("Receive Num = " + num);
//							if (num == 2015) { // GameStart 버튼을 눌렀을 때.
//								sender.write(data);
//							} else if (num == 8282) { // Client index를 요청했을 경우
//								int state;
//								state = findRoom();
//								if (state == 1) {
//									System.out.println("GameStart now successfully");
//								} else if (state == -1) {
//									System.out.println("Something is wrong");
//								} else {
//									System.out.println("Waiting other player in : " + roomIndex);
//								}
//							}
//						} else { // 플레이어가 움직였을 때.
//							Enumeration<Integer> en = Server.clientTable.keys();// en에 커서가 있다
//							while (en.hasMoreElements()) { // clientTable을 돌며 모든 클라이언트에게 위치값 전송
//								int key = en.nextElement();
//								sender = Server.clientTable.get(key).getOutputStream();
//								sender.write(data);
//							}
//							sender = socket.getOutputStream();
//						}

//					} else {
//						// 클라이언트에서 Close 혹은 Disconnect 신호가 안들어와서 getReceive 에서 size가 0인 패킷을 10번 이상 받으면
//						// 연결해제로 간주
//						if (maybeDisconnected > 10) {
//							break;
//						}
//						maybeDisconnected++;
//					}

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

	public void closeAll() throws IOException {
		if (socket != null)
			socket.close();
	}

	public byte[] intToByte(int num, ByteOrder byteOrder) {
		ByteBuffer buff = ByteBuffer.allocate(Integer.SIZE / 8);
		buff.order(byteOrder);

		buff.putInt(num);

		return buff.array();

	}

	public int byteToInt(byte[] buf) {
		ByteBuffer buffer = ByteBuffer.wrap(buf);
		buffer.order(ByteOrder.LITTLE_ENDIAN); // if you want little-endian
		return buffer.getShort();
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
						requestPlayerIndexAck.PlayerIndex = playerIndex++;
						packetSend(Packet.TypeRequestPlayerIndexAck, requestPlayerIndexAck);
					}
					sender = socket.getOutputStream();
				} catch (IOException e1) {
					// TODO Auto-generated catch block
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

	public static byte[] objectToByte(Object object) {
		ByteArrayOutputStream bos = new ByteArrayOutputStream();
		ObjectOutput out;
		try {
			out = new ObjectOutputStream(bos);
			out.writeObject(object);
		} catch (IOException e) {
			// TODO Auto-generated catch block
			e.printStackTrace();
		}
		return bos.toByteArray();
	}

	public static Object byteToObject(byte[] byteData) {
		ByteArrayInputStream bis = new ByteArrayInputStream(byteData);
		ObjectInput in;
		try {
			in = new ObjectInputStream(bis);
			Object object = in.readObject();
			return object;
		} catch (IOException e) {
			// TODO Auto-generated catch block
			e.printStackTrace();
		} catch (ClassNotFoundException e) {
			// TODO Auto-generated catch block
			e.printStackTrace();
		}
		return null;
	}

}