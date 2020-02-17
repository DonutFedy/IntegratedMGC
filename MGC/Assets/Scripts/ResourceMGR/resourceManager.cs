using System.Collections;
using System.Collections.Generic;
using PACKET;
using PROTOCOL;
using UnityEngine;

public class resourceManager : management
{
    [SerializeField]
    List<Sprite>            m_characterSprite = new List<Sprite>();
    [SerializeField]
    Sprite                  m_EmptyCharacterSprite;
    [SerializeField]
    List<Sprite>            m_TWENTY_QUESTION_item_sprite;
    [SerializeField]
    List<Sprite>            m_RELAY_NOVEL_item_sprite;
    [SerializeField]
    List<Sprite>            m_TABOO_WORD_GAME_item_sprite;
    [SerializeField]
    List<Sprite>            m_CATCH_MIND_item_sprite;
    [SerializeField]
    List<Sprite>            m_UIspriteList;

    public enum UIspriteINDEX
    {
        GAMEROOM_MASTER_SP = 0,
        GAMEROOM_READY_SP = 1,
        GUILD_MASTER = 2,
        GUILD_NORMAL = 3,
    }


    public override void init()
    {
    }

    public override void release()
    {
        m_characterSprite.Clear();
    }

    protected override void processEvent(C_BasePacket curEvent)
    {
    }

    public Sprite getCharacterSprite(int nIndex)
    {
        if (nIndex == -1)
            return m_EmptyCharacterSprite;
        return m_characterSprite[nIndex];
    }
    public Sprite getShopSprite(int nIndex, GAME_MODE type)
    {
        switch (type)
        {
            case GAME_MODE.TWENTY_QUESTION:
                if (nIndex >= m_TWENTY_QUESTION_item_sprite.Count)
                {
                    string errorMSG = "sprite Index overflow";
                    GameManager.m_Instance.writeErrorLog(errorMSG);
                    throw new System.OverflowException();
                }
                return m_TWENTY_QUESTION_item_sprite[nIndex];
            case GAME_MODE.RELAY_NOVEL:
                if (nIndex >= m_RELAY_NOVEL_item_sprite.Count)
                {
                    string errorMSG = "sprite Index overflow";
                    GameManager.m_Instance.writeErrorLog(errorMSG);
                    throw new System.OverflowException();
                }
                return m_RELAY_NOVEL_item_sprite[nIndex];
            case GAME_MODE.TABOO_WORD_GAME:
                if (nIndex >= m_TABOO_WORD_GAME_item_sprite.Count)
                {
                    string errorMSG = "sprite Index overflow";
                    GameManager.m_Instance.writeErrorLog(errorMSG);
                    throw new System.OverflowException();
                }
                return m_TABOO_WORD_GAME_item_sprite[nIndex];
            case GAME_MODE.CATCH_MIND:
                if(nIndex>= m_CATCH_MIND_item_sprite.Count)
                {
                    string errorMSG = "sprite Index overflow";
                    GameManager.m_Instance.writeErrorLog(errorMSG);
                    throw new System.OverflowException();
                }
                return m_CATCH_MIND_item_sprite[nIndex];
        }
        return null;
    }
    public Sprite getUIsprite(int nIndex)
    {
        return m_UIspriteList[nIndex];
    }
}
