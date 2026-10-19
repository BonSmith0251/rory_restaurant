// Ported from ScoreManager.as (decompiled).
using System;
using System.Collections.Generic;
using Starlite;

public class ScoreManager
{
    public static Global _global = Global.getInstance();
    public static TextField scoreText;
    public static TextField scoreDisplayText;
    public static bool updatingScore;
    public static string score;
    public static string totalScore;

    public ScoreManager()
    {
    }

    public static void initScore(int arg1, int arg2)
    {
        totalScore = "a";
        score = "a";
        scoreDisplayText = new TextField(150, 50, "$ 0", "jenna_sue", 35);
        scoreDisplayText.x = arg1 + 30;
        scoreDisplayText.y = arg2;
        scoreDisplayText.vAlign = "center";
        scoreDisplayText.touchable = false;
    }

    public static void updateScore(int arg1)
    {
        totalScore = _global._maxCrypto.ITS(_global._maxCrypto.STI(totalScore) + arg1);
        updatingScore = true;
    }

    public static void updateDisplayedScore()
    {
        int shown = _global._maxCrypto.STI(score);
        int target = _global._maxCrypto.STI(totalScore);
        if (shown < target)
        {
            // AS3 Number arithmetic, then coerced to int by ITS(int)
            score = _global._maxCrypto.ITS((int)(shown + 5 + 5.0 * (target - shown) / 100));
            scoreDisplayText.text = "$ " + _global.addCommas(_global._maxCrypto.STI(score));
        }
        else
        {
            updatingScore = false;
        }
        if (_global._maxCrypto.STI(score) > target)
        {
            score = totalScore;
            scoreDisplayText.text = "$ " + _global.addCommas(_global._maxCrypto.STI(score));
        }
    }

    public static void Kill()
    {
        scoreDisplayText.removeFromParent(true);
        scoreDisplayText = null;
    }
}
