import java.net.InetSocketAddress;
import java.net.ServerSocket;
import java.net.Socket;
import java.util.Hashtable;

public class Server {

	public static Hashtable<Integer, Hashtable<Integer, Socket>> roomList = new Hashtable<Integer, Hashtable<Integer,Socket>>();
	public static Hashtable<Integer, Socket> clientTable = new Hashtable<Integer, Socket>();
	public static int roomListCounts = 0;
	public static final int MAX_PLAYER_PER_ROOM = 2;
	public static final int GAME_START_PACKET = 2015;
    public static final int REQUEST_PLAYER_INDEX = 8282;
	private final int PORT = 6666;
	private final String IP = "127.0.0.1";
	
	public void go() {
		ServerSocket ss = null;
		Socket s = null;
		
		int clientIndex = 0;
		
		roomList.put(roomListCounts++, new Hashtable<Integer, Socket>());

		try {
			ss = new ServerSocket();
			ss.bind(new InetSocketAddress(IP, PORT));
			System.out.println("**서버 실행**");
			// 다수의 클라이언트와 통신하기 위해 loop
			while (true) {
				s = ss.accept(); // 클라이언트 접속시 새로운 소켓이 리턴
				Server.clientTable.put(clientIndex, s);
				ServerThread st = new ServerThread(s, clientIndex);
				st.start();
				System.out.println(clientIndex + "님 입장");
				++clientIndex;
			}
		} catch (Throwable e) {
			e.printStackTrace();
		} finally {
			try {
			if (s != null)
				s.close();
			if (ss != null)
				ss.close();
			}
			catch(Throwable e) {
				e.printStackTrace();
			}
			System.out.println("**서버 종료**");
		}
	}

	public static void main(String[] args) {
		Server server = new Server();
		server.go();

	}

}