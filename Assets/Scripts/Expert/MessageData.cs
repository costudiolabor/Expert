public enum TypeMessage {
    Call,
    AnswerCall,
    PrivateMessage
}

public class MessageData {
    public string id;
    public TypeMessage typeMessage;
    public string roomId;
    public string textMessage;
}
