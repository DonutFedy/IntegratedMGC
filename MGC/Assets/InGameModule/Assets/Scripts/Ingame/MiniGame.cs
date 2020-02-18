using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MiniGame : MonoBehaviour
{
    protected bool m_bStartPopupTime, m_bReserveExit, m_bStartChoiceAnswerPopupTime, m_bRoundStart, m_bStartAnswerPopupTime, m_bStartExaminerExitPopupTime;
    protected int m_iInputFieldCount, m_iPlayerCnt, m_iIngameLimitTime;
    protected float m_fLimitPopupTime;
    protected string m_strNickname, m_strAnswer;
    protected List<Text> m_listText = new List<Text>();
    protected List<GameObject> m_listPlayerInfo = new List<GameObject>();
    protected GameData.EnumGameType m_eGameType;
    protected GameData.EnumPlayerState m_ePlayerState;

    public GameObject m_goPopup, m_goPlayerExitPopup, m_goRadioGroupAnswer, m_goWaitChoiceAnswerPopup, m_goIngameTimeText;
    public GameObject m_goPosition1, m_goPosition2, m_goPosition3, m_goPosition4, m_goPosition5, m_goPosition6;
    public GameObject m_goCharacterExit1, m_goCharacterExit2, m_goCharacterExit3, m_goCharacterExit4, m_goCharacterExit5, m_goCharacterExit6, m_goReserveExitOrCancel;
    public GameObject m_goReserveExit1, m_goReserveExit2, m_goReserveExit3, m_goReserveExit4, m_goReserveExit5, m_goReserveExit6;
    public GameObject m_goAnswerText, m_goChatCountBox, m_goChatBox;
    public Text m_textPopupTime, m_textChatBoxText, m_textChatCountBox, m_textReserveExitOrCancel, m_textChoiceAnswerPopupTime, m_textWaitChoiceAnswerPopupTime, m_textIngameTime;
    public Text m_textNickname1, m_textNickname2, m_textNickname3, m_textNickname4, m_textNickname5, m_textNickname6;
    public Text m_textRound, m_textAnswer, m_textAnswer1, m_textAnswer2, m_textAnswer3, m_textAnswer4, m_textAnswer5;
    public Image m_imgTempCharacter1, m_imgTempCharacter2, m_imgTempCharacter3, m_imgTempCharacter4, m_imgTempCharacter5, m_imgTempCharacter6;
    public InputField m_ifChatBox;

    protected void StartTime()
    {
        m_fLimitPopupTime = 3;
        m_bStartPopupTime = true;
    }

    protected void EndTime()
    {
        m_goPopup.SetActive(false);
    }

    protected void CountChat()
    {        
        m_iInputFieldCount = m_ifChatBox.characterLimit - m_ifChatBox.text.Length;
        //m_iInputFieldCount = m_ifChatBox.characterLimit - m_textChatBoxText.text.Length;
        m_textChatCountBox.text = "" + m_iInputFieldCount;
    }

    protected void SendAnswer()
    {
        GameData.SendExaminerSelectedAnswerPacket examinerSelectedAnswer = new GameData.SendExaminerSelectedAnswerPacket();

        examinerSelectedAnswer.byteGameType = (byte)m_eGameType;

        if(m_eGameType.Equals(GameData.EnumGameType.TWENTY))
            examinerSelectedAnswer.byteStructType = (byte)GameData.EnumGameTwentyStructType.SEND_EXAMINER_SELECT_ANSWER_PACKET;
        else if(m_eGameType.Equals(GameData.EnumGameType.CATCH))
            examinerSelectedAnswer.byteStructType = (byte)GameData.EnumGameCatchStructType.SEND_EXAMINER_SELECT_ANSWER_PACKET;

        examinerSelectedAnswer.strAnswer = m_strAnswer;

        byte[] packet = Serializer.StructureToByte(examinerSelectedAnswer);

        IngameManager.m_Instance.GetClient().SendPacket(packet);

        Debug.Log("Send Answer Packet");
    }

    public void SetNickname(string _strNickname)
    {
        m_strNickname = _strNickname;
    }

    public void SetPlayerCount(int _iPlayerCnt)
    {
        m_iPlayerCnt = _iPlayerCnt;        
    }

    public void SetGameType(GameData.EnumGameType _eGameType)
    {
        m_eGameType = _eGameType;
    }

    public void SetPlayerState(GameData.EnumPlayerState _ePlayerState)
    {
        m_ePlayerState = _ePlayerState;
    }

    public void SelectAnswer1()
    {
        m_strAnswer = m_textAnswer1.text;
    }

    public void SelectAnswer2()
    {
        m_strAnswer = m_textAnswer2.text;
    }

    public void SelectAnswer3()
    {
        m_strAnswer = m_textAnswer3.text;
    }

    public void SelectAnswer4()
    {
        m_strAnswer = m_textAnswer4.text;
    }

    public void SelectAnswer5()
    {
        m_strAnswer = m_textAnswer5.text;
    }

    public void ExitGame(GameData.EnumGameType _eGameType)
    {
        IngameManager.m_Instance.ExitGame(_eGameType);
    }
}