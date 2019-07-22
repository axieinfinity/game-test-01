using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class BattleControler : MonoBehaviour {
    public static BattleControler instance;

    #region Editor changable variables

    public List<PlayerController> PlayersList;
    public int myPlayerID;

    public GameObject panelWin;
    public Text resultText;
    public GameObject yourTurnText;
    public GameObject StartParticle;
    #endregion

    bool isCanAttack = false;
    int curID = -1;
    KeyCode attackKey = KeyCode.Space;

    // Use this for initialization
    void Start () {
        instance = this;
        //set id and enemy for Players in list, so player can expand
        //setStartValue();
        
        
	}

    public void setStartValue()
    {
        for (int i = 0; i < PlayersList.Count; i++)
        {
            PlayersList[i].id = i;
            if (i == 0)
            {
                PlayersList[i].setEnemy(PlayersList[PlayersList.Count - 1]);
            }
            else
            {
                PlayersList[i].setEnemy(PlayersList[i - 1]);
            }
        }
        curID = myPlayerID;
        StartParticle.SetActive(true);
        StartCoroutine(IEWaitToTurn());
        SoundManager.instance.playStartGame();
    }

    IEnumerator IEWaitToTurn()
    {
        yield return new WaitForSeconds(1f);
        setYourTurn(true);
    }

    void setYourTurn(bool yourturn)
    {
        isCanAttack = yourturn;
        yourTurnText.SetActive(yourturn);
    }
	
	public void switchPlayerTurn(int id)
    {
        int nextID = id + 1;
        if(nextID >= PlayersList.Count)
        {
            nextID = 0;
        }
        curID = nextID;
        //check enemy next turn or not, wait to space key if curID is myPlayerID
        if(curID == myPlayerID)
        {
            setYourTurn(true);
        }
        else
        {
            PlayersList[nextID].moveToEnemy();
        }
        
    }

    public void showWinPlayer(int id)
    {
        panelWin.SetActive(true);
        if (myPlayerID == id)
        {
            resultText.text = "You Win!!!";
            SoundManager.instance.playWin();
        }  
        else
        {
            resultText.text = "You Fail!";
        }
            
        
    }

    public void press_retry()
    {
        SceneManager.LoadScene("Battle");
    }

    void Update()
    {
        if (isCanAttack) 
        {
            if (Input.GetKeyDown(attackKey))
            {
                if (PlayersList.Count > 1 && curID >= 0 && curID < PlayersList.Count)
                {
                    if (curID == myPlayerID)
                    {
                        setYourTurn(false);
                        PlayersList[curID].moveToEnemy();
                    }
                }
            }
        }
    }
}
