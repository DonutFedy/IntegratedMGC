using PACKET;
using PROTOCOL;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class UI : MonoBehaviour
{
    [SerializeField]
    protected Stack<int> m_uiIndexStack;
    [SerializeField]
    protected List<UI> m_uiList = new List<UI>();

    protected int m_uiType;
    protected bool m_bWaiting;
    protected bool m_bisFocuse;
    [SerializeField]
    //protected PROTOCOL.OUTGAME_TYPE m_type;
    protected UI m_uiManager;

    protected delegate void responseListener(C_BasePacket eventData);
    protected responseListener m_listener;

    public waitingResponse m_waitingUI;

    public virtual void initUI(UI manager, int uiType)
    {
        m_uiIndexStack = new Stack<int>();
        m_uiManager = manager;
        m_uiType = uiType;
        // 보유 list에 있는 UI들 init
        for (int i = 0; i < m_uiList.Count; ++i)
        {
            m_uiList[i].initUI(this, i);
        }
    }
    public virtual void releaseUI()
    {
        m_uiIndexStack.Clear();
        close();
        // 보유 ui list release
        foreach (UI var in m_uiList)
        {
            var.releaseUI();
        }
    }

    protected abstract void setUI();
    protected virtual void startWaiting(responseListener listener)
    {
        m_listener += listener;
        m_bWaiting = true;
        if (m_waitingUI)
        {
            m_waitingUI.m_clearWaitingFUNC = isWaiting;
            m_waitingUI.m_overTimeFunc = null;
            m_waitingUI.m_overTimeFunc = stopWaiting;
            m_waitingUI.gameObject.SetActive(true);
        }
    }
    protected virtual void stopWaiting()
    {
        m_listener = null;
        m_bWaiting = false;
        if (m_waitingUI)
            m_waitingUI.gameObject.SetActive(false);
    }

    bool isWaiting()
    {
        return m_bWaiting;
    }

    public void stopWaitingUI()
    {
        stopWaiting();
    }
    public virtual void update(C_BasePacket eventData)
    {
        if (m_bWaiting)
        {
            // 만약 이벤트 데이터 타입이 response라면 
        }

    }

    /// <summary>
    /// 리스트의 UI를 열게된다.
    /// </summary>
    /// <param name="targetUiType"></param>
    public void openUI(int targetUiType)
    {

        m_bisFocuse = false;

        // cur ui close
        if (m_uiIndexStack.Count > 0)
        {
            // check exist
            if (m_uiIndexStack.Peek() == targetUiType)
                return;
            m_uiList[m_uiIndexStack.Peek()].close();
        }
        // push targetUI
        m_uiIndexStack.Push(targetUiType);
        m_uiList[m_uiIndexStack.Peek()].open();
    }

    /// <summary>
    /// 현재 열려진 UI를 닫는다.
    /// </summary>
    /// <param name="nCounter"></param>
    public void closeUI(int nCounter = 1)
    {
        if (m_uiIndexStack.Count <= 0)
            return;
        m_uiList[(int)m_uiIndexStack.Peek()].close();
        m_uiIndexStack.Pop();
        if (m_uiIndexStack.Count <= 0)
            m_bisFocuse = true;
        --nCounter;
        if (nCounter > 0)
            m_uiManager.closeUI(nCounter);
        if (m_uiIndexStack.Count > 0)
            m_uiList[(int)m_uiIndexStack.Peek()].open();
    }
    public void open()
    {
        this.gameObject.SetActive(true);
        setUI();
    }
    public void close()
    {
        stopWaiting();
        releaseUI();
        // 보유 ui list release
        foreach (UI var in m_uiList)
        {
            var.close();
        }
        m_uiIndexStack.Clear();
        this.gameObject.SetActive(false);
    }



    public void inputKey(inputKeyManager.S_KeyInput.KeyType type)
    {
        processInputKey(type);
    }

    protected bool isExistSubUI(C_BasePacket eventData)
    {
        if (m_uiIndexStack.Count <= 0)
        {
            return false;
        }

        m_uiList[m_uiIndexStack.Peek()].update(eventData);

        return true;
    }
    protected bool isExistSubUI(inputKeyManager.S_KeyInput.KeyType type)
    {
        if (m_uiIndexStack.Count <= 0)
        {
            return false;
        }
        m_uiList[m_uiIndexStack.Peek()].inputKey(type);
        return true;
    }

    /// <summary>
    /// 자식 UI가 닫힐때 세팅
    /// </summary>
    public virtual void setOnCloseSubUI()
    {

    }
    protected abstract void processInputKey(inputKeyManager.S_KeyInput.KeyType type);

    public UI getCurrentUI()
    {
        if(m_uiIndexStack.Count>0)
        {
            return m_uiList[(int)m_uiIndexStack.Peek()].getCurrentUI();
        }
        return this;
    }
}
