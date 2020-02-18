using System.Collections;
using System.Collections.Generic;
using PACKET;
using UnityEngine;

public class ingameManager : management {
    
    [SerializeField]
    IngameManager               m_ingameMGR;//진우님이 만든 매니저

    public override void init()
    {
        // 진우님이 만든 인게임 매니저 set
        if (m_bInit == true) return;
        m_bInit = true;

        m_eventBuffer = new Queue<C_BasePacket>();
    }

    public override void release()
    {
        // 진우 님이 만든 인게임 매니저 release
    }
    protected override void processEvent(C_BasePacket curEvent)
    {
        // 진우 님이 만든 인 게임 매니저에게 대 분류 타입부분만 떼고 보내기
        // use-> byte[] realData = getData(curEvent);
        Debug.Log(curEvent.m_basicType.ToString());
        if (curEvent.m_basicType != BasePacketType.basePacketTypeGame) return;
        C_InGamePacket data = (C_InGamePacket)curEvent;
        m_ingameMGR.Event(data.m_gameData);
    }
    byte[] getData(C_BasePacket packet)
    {
        return null;
    }
}
