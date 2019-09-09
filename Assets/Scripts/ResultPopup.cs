using UnityEngine;
using UnityEngine.UI;

public class ResultPopup : MonoBehaviour
{
    [SerializeField] private Text _title;

    private string winText = "You win !!!";
    private string loseText = "You lose !!!";


    public void Show(bool isWin)
    {
        gameObject.SetActive(true);
        _title.text = isWin ? winText : loseText;
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }
}
