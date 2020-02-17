using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

class EventBuffer
{
    private static EventBuffer m_instance;

    public static EventBuffer m_Instance
    {
        get
        {
            if (m_instance == null)
                m_instance = new EventBuffer();

            return m_instance;
        }
    }

    private Queue<byte[]> eventQueue;
    private Queue<string> errorMsgQueue;

    private EventBuffer()
    {
        eventQueue = new Queue<byte[]>();
        errorMsgQueue = new Queue<string>();
    }

    public void PushData(byte[] _baData)
    {
        // 클라이언트 소켓에서 받은 데이터를 큐에 푸쉬
        eventQueue.Enqueue(_baData);
    }

    public void PushErrorMsg(string _strErrorMsg)
    {
        // 클라이언트 소켓에서 받은 에러 메세지를 큐에 푸쉬
        errorMsgQueue.Enqueue(_strErrorMsg);
    }

    public byte[] GetData()
    {
        // 인게임 매니저에서 큐에 있는 데이터를 가져갈 때, 큐가 비어있다면 size 1인 배열을 보내 알려준다.
        byte[] empty = new byte[1];
        empty[0] = 0;

        if (eventQueue.Count > 0)
            return eventQueue.Dequeue();
        else
            return empty;
    }

    public string GetErrorMsg()
    {
        string strEmpty = "";

        if (errorMsgQueue.Count > 0)
            return errorMsgQueue.Dequeue();
        else
            return strEmpty;
    }
}