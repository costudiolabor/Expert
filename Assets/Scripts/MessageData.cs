public enum TypeMessage
{
   Call,
   PersonalMassage
}
public class MessageData
{
   public string name;
   public int id;
   public int remoteId;
   public TypeMessage typeMessage;
}
