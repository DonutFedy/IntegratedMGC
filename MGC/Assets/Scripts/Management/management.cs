using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PACKET;

public abstract class management : MonoBehaviour {

    protected bool              m_bInit = false;
    protected Queue<C_BasePacket> m_eventBuffer;

    /// <summary>
    /// 초반 시작 설정
    /// </summary>
    public abstract void init();

    /// <summary>
    /// 종료시 호출
    /// </summary>
    public abstract void release();

    public void updateManager()
    {
        float fCurGameTime = GameManager.m_Instance.getGameTime();
        // 현재 시간 보다 낮거나 같은 이벤트만 처리
        C_BasePacket data = null;

        while (m_eventBuffer.Count > 0)
        {
            data = m_eventBuffer.Peek();
            if (fCurGameTime <= data.m_fEventTime)
                break;
            // 이벤트 처리
            processEvent(data);
            m_eventBuffer.Dequeue();
        }
    }

    protected abstract void processEvent(C_BasePacket curEvent);
    public void enqueueEvent(C_BasePacket packet)
    {
        m_eventBuffer.Enqueue(packet);
    }

}
