using PACKET;
using PROTOCOL;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class makeGuildUI : UI
{
    public enum INDEX_OF_MAKE_GUILD_UI
    {
    }


    #region Basic UI
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

    #endregion

    #region Server REQ / RES
    public void tryMakeGuild()
    {
        // 현재 잘못된 데이터가 있는지 check

        startWaiting(responseTryMakeGuild);
        // 현재 데이터 동봉해서 서버에 req
    }

    void responseTryMakeGuild(C_BasePacket curEventData)
    {
        stopWaiting();
        // 현재 이벤트에 맞는 타입이 아니면 return

        // 실패 recv 시 실패 팝업
        // 성공 recv 시 성공 팝업
    }
    #endregion

    #region UI OPEN / CLOSE
    /// <summary>
    /// 현재 UI 닫기 BTN
    /// </summary>
    public void exitUI()
    {
        m_uiManager.closeUI(1);
        m_uiManager.setOnCloseSubUI();
    }
    
    #endregion
}