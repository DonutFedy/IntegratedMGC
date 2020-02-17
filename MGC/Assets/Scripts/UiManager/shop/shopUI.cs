using PACKET;
using PROTOCOL;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class shopUI : UI
{
    public enum INDEX_OF_SHOP_UI
    {

    }



    public override void initUI(UI manager, int uiType)
    {
        base.initUI(manager, uiType);
        // 기본 세팅

    }
    public override void update(C_BasePacket eventData)
    {
        isExistSubUI(eventData);
    }

    public override void releaseUI()
    {
        // 중단
    }

    protected override void setUI()
    {
        // 초기화
        m_bWaiting = false;
    }

    protected override void startWaiting(responseListener listener)
    {
        base.startWaiting(listener);
        // ~~
    }
    protected override void stopWaiting()
    {
        base.stopWaiting();
    }

    protected override void processInputKey(inputKeyManager.S_KeyInput.KeyType type)
    {

    }

    public void exitUI()
    {
        m_uiManager.closeUI(1);
    }
}