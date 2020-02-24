import java.io.BufferedReader;
import java.io.IOException;
import java.io.InputStream;
import java.io.InputStreamReader;
import java.io.OutputStream;
import java.io.PrintWriter;
import java.net.Socket;
import java.nio.ByteBuffer;
import java.nio.ByteOrder;
import java.util.Enumeration;

public class ServerThread extends Thread {
	private Socket socket;
	private OutputStream sender;
	private InputStream receiver;
	private int clientIndex;
	private int maybeDisconnected = 0; // 클라이언트의 연결해제 상태를 예측하기 위함.

	ServerThread(Socket s, int index) {
		this.socket = s;
		this.clientIndex = index;
	}

	private static byte[] getRecieve(InputStream stream, Socket socket) throws IOException {
		byte[] buffer = new byte[4];
		// 사데이터 사이즈를 받는다.
		if (!socket.isClosed()) {
			System.out.println("Waiting for Receiving Data");
			stream.read(buffer, 0, 4);
			ByteBuffer data = ByteBuffer.wrap(buffer);
			// 리틀 앤디언 타입으로 설정
			data.order(ByteOrder.LITTLE_ENDIAN);
			// int형으로 변환
			int size = data.getInt();
			buffer = new byte[size];
			// 데이터를 받는다.
			stream.read(buffer, 0, size);
			return buffer;
		} else
			return null;
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
					byte[] data = getRecieve(receiver, socket);

					if (data.length != 0) {
						System.out.println("data 수신");

						//System.out.println("Received Data Length = " + data.length);
						if(data.length == 4) {
							int num = ByteToInt(data);
							System.out.println("Receive Num = " + num);
							if(num == 2015) { // GameStart 버튼을 눌렀을 때.
								sender.write(data);
							}
							else if(num == 2016){
								sender.write(IntToByte(clientIndex, ByteOrder.LITTLE_ENDIAN));
							}
						}
						else {
						Enumeration<Integer> en = Server.hashTable.keys();// en에 커서가 있다
						  while(en.hasMoreElements()){
						   int key = en.nextElement();
						   sender = Server.hashTable.get(key).getOutputStream();
						   sender.write(data);
						  }
						  sender = socket.getOutputStream();
						}

					}
					else {
						// 클라이언트에서 Close 혹은 Disconnect 신호가 안들어와서 getReceive 에서 size가 0인 패킷을 10번 이상 받으면 연결해제로 간주
						if(maybeDisconnected > 10) {
							break;
						}
						maybeDisconnected++;
					}

					System.out.println("완료 코드 보내기");
				}

			} catch (Throwable e) {
				e.printStackTrace();
			} finally {
				if (socket != null) {
					System.out.println("손님 한명 퇴장함");
					socket.close();
					sender.close();
					receiver.close();
					Server.hashTable.remove(clientIndex);

				}
			}

		}
	}

	public void closeAll() throws IOException {
		if (socket != null)
			socket.close();
	}
	
	public byte[] IntToByte(int num , ByteOrder byteOrder) {
		ByteBuffer buff = ByteBuffer.allocate(Integer.SIZE/8);
		buff.order(byteOrder);
		 
		buff.putInt(num);

		return buff.array();
		
		
	}
	
	public int ByteToInt(byte[] buf) {
		ByteBuffer buffer = ByteBuffer.wrap(buf);
		buffer.order(ByteOrder.LITTLE_ENDIAN);  // if you want little-endian
		return buffer.getShort();
	}
}
