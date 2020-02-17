using PACKET;
using PROTOCOL;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class failUI : UI
{
    [SerializeField]
    Text m_msgText;
    public override void initUI(UI manager, int uiType)
    {
        base.initUI(manager, uiType);
        // 기본 세팅

    }
    public override void update(C_BasePacket eventData)
    {
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

    public void setFail(string failMSG = "알수없는 이유")
    {
        m_msgText.text = "실패 이유 :"+ failMSG;
    }

    public void exitUI(int nCount)
    {
        m_uiManager.closeUI(nCount);
    }
    protected override void processInputKey(inputKeyManager.S_KeyInput.KeyType type)
    {
    }

}
