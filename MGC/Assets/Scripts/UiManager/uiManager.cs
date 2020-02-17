#define DEBUGMODE

using System.Collections;
using System.Collections.Generic;
using PACKET;
using PROTOCOL;
using UnityEngine;
using UnityEngine.UI;

public class uiManager : management
{
    public enum INDEX_OUTGAME_UI
    {
        LOGIN = 0,
        MAINMENU = 1,
        GAMEROOM = 2,
        SHOP = 3,
        FRIEND_GUILD = 4
    }

    [SerializeField]
    private UI          m_uiController;


#if DEBUGMODE

#endif

    public override void init()
    {
        // 한번만 불림
        if (m_bInit == true) return;
        m_bInit = true;

        m_eventBuffer = new Queue<C_BasePacket>();
        m_uiController.initUI(null,0);
        m_uiController.openUI((int)INDEX_OUTGAME_UI.LOGIN);
    }

    public void onErrorClient(string errorMSG)
    {
        UI curUI = ((controllerUI)m_uiController).getLoginUI();
        ((loginUI)curUI).onErrorClient(errorMSG);
    }
    public override void release()
    {
        m_uiController.releaseUI();
    }
    protected override void processEvent(C_BasePacket curEvent)
    {
        m_uiController.update(curEvent);
    }
    public void inputKey(inputKeyManager.S_KeyInput.KeyType type)
    {
        m_uiController.inputKey(type);
    }

}
