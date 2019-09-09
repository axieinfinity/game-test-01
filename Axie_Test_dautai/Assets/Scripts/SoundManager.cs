using UnityEngine;
using System.Collections;

public class SoundManager : MonoBehaviour {
	public static SoundManager instance;

	public AudioClip myPlayerAttack;
	public AudioClip enemyPlayerAttack;
	public AudioClip soundWIn;
	public AudioClip startGame;
	
	public AudioSource sourceBattle;
	// Use this for initialization
	void Awake () {
		instance = this;
	}


    public void playMyPlayerAttack()
    {
        if (sourceBattle != null)
        {
            sourceBattle.PlayOneShot(myPlayerAttack);
        }
            
    }

    public void playEnemyPlayerAttack()
	{
        if (sourceBattle != null)
        {
            sourceBattle.PlayOneShot(enemyPlayerAttack);
        }
            
	}

	public void playWin()
	{
        if (sourceBattle != null)
        {
            sourceBattle.PlayOneShot(soundWIn);
        }
            
	}

	public void playStartGame()
	{
        if(sourceBattle != null)
        {
            sourceBattle.PlayOneShot(startGame);
        }
		
	}
}
