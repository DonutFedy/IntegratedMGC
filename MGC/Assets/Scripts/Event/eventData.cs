


using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Text;
using UnityEngine;


namespace PROTOCOL
{
    public enum GAME_MODE
    {
        [Description("스무고개")]
        TWENTY_QUESTION = 0,
        [Description("릴레이 소설")]
        RELAY_NOVEL = 1,
        [Description("금칙어")]
        TABOO_WORD_GAME = 2,
        [Description("캐치 마인드")]
        CATCH_MIND = 3
    }
    

}