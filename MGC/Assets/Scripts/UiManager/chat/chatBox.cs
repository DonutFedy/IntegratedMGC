#define DEBUGMODE

using PACKET;
using PROTOCOL;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class chatBox : MonoBehaviour
{
    public enum CHAT_TYPE
    {
        ALL_CHAT = 0,
        WHISPER_CHAT = 1,
        GUILD_CHAT = 2,
    }

    [SerializeField]
    InputField          m_chatIF;
    [SerializeField]
    Text                m_chatText;

    [SerializeField]
    RectTransform       m_chatBoxParentRectTransform_All;
    [SerializeField]
    RectTransform       m_chatBoxParentRectTransform_Whisper;
    [SerializeField]
    RectTransform       m_chatBoxParentRectTransform_Guild;
    [SerializeField]
    List<GameObject>    m_TextList;
    [SerializeField]
    GameObject          m_TextPrefab;
    [SerializeField]
    ScrollRect          m_scrollRect_All;
    [SerializeField]
    ScrollRect          m_scrollRect_Whisper;
    [SerializeField]
    ScrollRect          m_scrollRect_Guild;

    [SerializeField]
    toggleController    m_toggleController;

    [SerializeField]
    GameObject          m_NewIcon_AllTab;
    [SerializeField]
    GameObject          m_NewIcon_WhisperTab;
    [SerializeField]
    GameObject          m_NewIcon_GuildTab;


    [SerializeField]
    float               m_fChatLimitTime;
    float               m_fCurLimitTime;
    Coroutine           m_chatLimit;

    CHAT_TYPE           m_curChatType;

    public float        m_defaultSize_X;
    public float        m_defaultSize_Y;
    public float        m_offset_Y;

    public bool         m_bNowWhisper;

    public void setChatBox()
    {
        m_bNowWhisper = false;
        // set default size 
        clearChatBox();
        changeChatToggle(0);
        m_chatLimit = StartCoroutine(limitChating());
    }

    

    IEnumerator limitChating()
    {
        m_fCurLimitTime = 0;
        while (true)
        {
            if(m_fCurLimitTime >0)
            {
                m_fCurLimitTime -= Time.deltaTime;
            }
            yield return 0;
        }
    }
    private void OnDisable()
    {
        m_chatIF.gameObject.SetActive(false);
        if (m_chatLimit!= null)
        {
            StopCoroutine(m_chatLimit);
            m_chatLimit = null;
        }
    }

    private void OnEnable()
    {
        m_chatIF.gameObject.SetActive(true);
    }

    /// <summary>
    /// input field 값이 변경되면 귓속말 설정중인지 확인한다
    /// </summary>
    public void checkIF()
    {
        //Debug.Log("??");
        m_bNowWhisper = checkWhisper(m_chatIF.text) | checkGuild(m_chatIF.text);
    }

    /// <summary>
    /// 채팅 타입에 따른 분류
    /// </summary>
    /// <param name="chatText"></param>
    /// <param name="target"></param>
    /// <param name="msg"></param>
    /// <returns></returns>
    SocialPacketType checkMsg(string chatText, out string target, out string msg)
    {
        target = "";
        msg = "";
        if (checkWhisper(chatText) == true)
        {
            // check ' ' and targetName
            int nMsgStartIndex = 2;
            if (chatText[2].CompareTo(' ') == 0)
            {
                nMsgStartIndex = chatText.IndexOf(' ', 3);
                if (nMsgStartIndex > 3 && chatText.Length > (nMsgStartIndex + 1))
                {
                    target = chatText.Substring(3, nMsgStartIndex - 3);
                    msg = chatText.Substring(nMsgStartIndex, chatText.Length - nMsgStartIndex);
                    return SocialPacketType.packetTypeSocialChatFriendRequest;
                }
            }
            return SocialPacketType.packetTypeSocialNone;
        }
        else if (checkGuild(chatText) == true)
        {
            if (chatText.Length <= 3)
                return SocialPacketType.packetTypeSocialNone;

            msg = chatText.Substring(3, chatText.Length - 3);
            return SocialPacketType.packetTypeSocialChatGuildRequest;
        }
        msg = chatText;
        return SocialPacketType.packetTypeSocialChatNormalRequest;
    }

    bool checkWhisper(string msg)
    {
        if (msg.Length < 2) return false;
        else if (msg[0].CompareTo('/') == 0 && msg[1].CompareTo('w') == 0)
            return true;
        return false;
    }
    bool checkGuild(string msg)
    {
        if (msg.Length < 2) return false;
        else if (msg[0].CompareTo('/') == 0 && msg[1].CompareTo('g') == 0)
            return true;
        return false;
    }

    public void sendChat()
    {
        // 채팅 시간 제한
        if (m_fCurLimitTime > 0)
            return;
        // 채팅 길이 체크
        if (m_chatIF.text.Length <= 0)
            return;

        string target = null;
        string msg = null;
        SocialPacketType curType = checkMsg(m_chatIF.text, out target, out msg);

        C_BaseSocialPacket curData = null;
        switch (curType)
        {
            case SocialPacketType.packetTypeSocialNone:                 // 잘못된 양식일 때
                focusIF();
                return;
            case SocialPacketType.packetTypeSocialChatNormalRequest:
                C_SocialPacketChatNormalRequest realData = new C_SocialPacketChatNormalRequest();
                realData.m_message = msg;
                curData = realData;
                break;
            case SocialPacketType.packetTypeSocialChatFriendRequest:
                C_SocialPacketChatFriendRequest friendData = new C_SocialPacketChatFriendRequest();
                friendData.m_message = msg;
                friendData.m_destName = target;
                curData = friendData;
                break;
            case SocialPacketType.packetTypeSocialChatGuildRequest:
                break;
        }
        GameManager.m_Instance.makePacket(curData);

        focusIF();
        m_fCurLimitTime = m_fChatLimitTime;
        m_bNowWhisper = false;
    }

    public void focusIF()
    {
        m_chatIF.ActivateInputField();
        switch (m_curChatType)
        {
            case CHAT_TYPE.ALL_CHAT:
                m_chatIF.text = "";
                break;
            case CHAT_TYPE.WHISPER_CHAT:
                m_chatIF.text = "/w ";
                break;
            case CHAT_TYPE.GUILD_CHAT:
                m_chatIF.text = "/g ";
                break;
        }

    }

    void clearChatBox()
    {
        m_NewIcon_AllTab.SetActive(false);
        m_NewIcon_WhisperTab.SetActive(false);
        m_NewIcon_GuildTab.SetActive(false);
        foreach (GameObject var in m_TextList)
        {
            DestroyImmediate(var);
        }
        m_TextList.Clear();
        m_chatBoxParentRectTransform_All.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, m_defaultSize_X);
        m_chatBoxParentRectTransform_All.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, m_defaultSize_Y);
        m_chatBoxParentRectTransform_Whisper.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, m_defaultSize_X);
        m_chatBoxParentRectTransform_Whisper.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, m_defaultSize_Y);
        m_chatBoxParentRectTransform_Guild.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, m_defaultSize_X);
        m_chatBoxParentRectTransform_Guild.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, m_defaultSize_Y);
    }

    public void updateChat(C_BasePacket chat)
    {
        if (chat.m_basicType != BasePacketType.basePacketTypeSocial) return;
        C_BaseSocialPacket data = (C_BaseSocialPacket)chat;
        if (((data.m_socialType == SocialPacketType.packetTypeSocialChatFriendResponse) ||
            (data.m_socialType == SocialPacketType.packetTypeSocialChatGuildResponse) ||
            (data.m_socialType == SocialPacketType.packetTypeSocialChatNormalResponse)) == false)
            return;


        RectTransform curParent = null;
        string curChatText = string.Empty;
        Color color = Color.white;
        ScrollRect curRect = null;
        string srcName = "";
        string clientUserNickname = GameManager.m_Instance.getUserNickName();
        switch (data.m_socialType)
        {
            case SocialPacketType.packetTypeSocialChatNormalResponse:
                C_SocialPacketChatNormalResponse curData = (C_SocialPacketChatNormalResponse)data;
                curParent = m_chatBoxParentRectTransform_All;
                curChatText = curData.m_nickname+ " : " + curData.m_message;
                if(clientUserNickname != curData.m_nickname)
                    srcName = curData.m_nickname;
                curRect = m_scrollRect_All;
                if (m_curChatType != CHAT_TYPE.ALL_CHAT)
                    m_NewIcon_AllTab.SetActive(true);
                break;
            case SocialPacketType.packetTypeSocialChatFriendResponse:
                C_SocialPacketChatFriendResponse curFriendData = (C_SocialPacketChatFriendResponse)data;
                curParent = m_chatBoxParentRectTransform_Whisper;
                if(curFriendData.m_bIsSender)
                {
                    curChatText = curFriendData.m_srcName + "에게 : ";
                    color = Color.red;
                }
                else
                {
                    srcName = curFriendData.m_srcName;
                    curChatText = curFriendData.m_srcName + " : ";
                    if(m_curChatType != CHAT_TYPE.WHISPER_CHAT)     // 채팅 탭이 현재 같은 타입 아니라면 new icon
                        m_NewIcon_WhisperTab.SetActive(true);       // 유저가 쓴 귓속말이 아니면 new icon
                    color = Color.green;
                }
                curChatText += curFriendData.m_message;
                curRect = m_scrollRect_Whisper;
                break;
            case SocialPacketType.packetTypeSocialChatGuildResponse:
                //curParent = m_chatBoxParentRectTransform_Guild;
                //curChatText = chat.m_source + " : " + chat.m_message;
                //curRect = m_scrollRect_Guild;
                //if (m_curChatType != CHAT_TYPE.GUILD_CHAT)
                //    m_NewIcon_GuildTab.SetActive(true);
                break;
        }
        textController curTextController = Instantiate(m_TextPrefab, curParent).GetComponent<textController>();
        curTextController.setText(srcName, curChatText, color, setWhisper);
        m_TextList.Add(curTextController.gameObject);
        curParent.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 10 + m_TextList.Count * m_offset_Y);
        curRect.normalizedPosition = Vector2.zero;
    }

    public void changeChatToggle(int nType)
    {
        m_curChatType = (CHAT_TYPE)nType;
        if (nType == (int)CHAT_TYPE.ALL_CHAT)
        {
            m_scrollRect_All.gameObject.SetActive(true);
            m_scrollRect_Whisper.gameObject.SetActive(false);
            m_scrollRect_Guild.gameObject.SetActive(false);
            m_NewIcon_AllTab.SetActive(false);
        }
        else if(nType == (int)CHAT_TYPE.WHISPER_CHAT)
        {
            m_scrollRect_All.gameObject.SetActive(false);
            m_scrollRect_Whisper.gameObject.SetActive(true);
            m_scrollRect_Guild.gameObject.SetActive(false);
            m_NewIcon_WhisperTab.SetActive(false);
        }
        else
        {
            m_scrollRect_All.gameObject.SetActive(false);
            m_scrollRect_Whisper.gameObject.SetActive(false);
            m_scrollRect_Guild.gameObject.SetActive(true);
            m_NewIcon_GuildTab.SetActive(false);
        }
        m_toggleController.onChange(nType);
        focusIF();
    }


    /// <summary>
    /// 친구 slot, 길드원 slot 클릭시 귓속말 상대 설정
    /// </summary>
    /// <param name="nickname"></param>
    public void setWhisper(string nickname)
    {
        m_chatIF.text = "/w " + nickname+" ";
        m_chatIF.ActivateInputField();
    }
}
