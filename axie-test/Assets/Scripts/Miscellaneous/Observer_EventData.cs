using System;
using System.Collections.Generic;

public class OnKnifeHitData
{
    public int senderID;
    public bool hitResult;

    public OnKnifeHitData(int senderID, bool hitResult)
    {
        this.senderID = senderID;
        this.hitResult = hitResult;
    }
}

public class UITextData
{
    public UITextData(int senderID, int value)
    {
        this.senderID = senderID;
        this.value = value;
    }
    public int senderID;
    public int value;
}

public class EndGameData
{
    public EndGameData(List<int> idList, Action callback, bool isHasWonPlayer)
    {
        this.idList = idList;
        this.callback = callback;
        this.isHasWonPlayer = isHasWonPlayer;
    }
    public List<int> idList;
    public Action callback;
    public bool isHasWonPlayer;
}

public class CallbackData
{
    public int senderID;
    public Action callback;

    public CallbackData(int senderID, Action value)
    {
        this.senderID = senderID;
        this.callback = value;
    }
}

public class UIImageData
{
    public UIImageData(int senderID, bool value)
    {
        this.senderID = senderID;
        this.value = value;
    }
    public int senderID;
    public bool value;
}

public class UIScoreTextData
{
    public int senderID;
    public int scoreValue;
    public int popUpScoreValue;

    public UIScoreTextData(int senderID, int scoreValue, int popUpScoreValue)
    {
        this.senderID = senderID;
        this.scoreValue = scoreValue;
        this.popUpScoreValue = popUpScoreValue;
    }
}
