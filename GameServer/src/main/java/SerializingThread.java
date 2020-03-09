import java.io.ByteArrayInputStream;
import java.io.ByteArrayOutputStream;
import java.io.IOException;
import java.io.ObjectInputStream;
import java.io.ObjectOutputStream;

public class SerializingThread extends Thread {
	
	private final int MAX_BYTE_LENGTH = 200;
	// 직렬화, 역직렬화를 위한 바이트 배열
	private byte[] bytes = null;
	// 직렬화, 역직렬화를 위한 객체
	private Object object = null;
	private String work = null;
	
	public SerializingThread(byte[] bytes, Object object, String work) {
		this.bytes = bytes;
		this.object = object;
		this.work = work;
	}

	public void run() {
	
		if(work.equals("serialize")) {		
			if (object == null) {
				return;
			}
			
			ByteArrayOutputStream baos = new ByteArrayOutputStream(MAX_BYTE_LENGTH);
			try {
				ObjectOutputStream oos = new ObjectOutputStream(baos);
				oos.writeObject(object);
				oos.flush();
			}
			catch (IOException ex) {
				throw new IllegalArgumentException("Failed to serialize object of type: " + object.getClass(), ex);
			}
			bytes = baos.toByteArray();
		}
		
		else if(work.equals("deserialize")) {
			if(bytes == null)
				return;
			
			try {
				ObjectOutputStream oos = new ObjectOutputStream(new ByteArrayOutputStream(MAX_BYTE_LENGTH));
				oos.flush();
				
				ObjectInputStream ois = new ObjectInputStream(new ByteArrayInputStream(bytes));
				object = ois.readObject();
			}
			catch (IOException ex) {
				throw new IllegalArgumentException("Failed to deserialize object", ex);
			}
			catch (ClassNotFoundException ex) {
				throw new IllegalStateException("Failed to deserialize object type", ex);
			}
		}
		
	}
	
	public byte[] getBytes() {
		return bytes;
	}
	
	public Object getObject() {
		return object;
	}

}
