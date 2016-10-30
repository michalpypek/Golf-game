using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour
{

    public static GameManager instance = null;
    public InputManager inputManager;
    public BallTestScript ballScript;
    public BoardCreator boardCreator;
    public GameObject winscreen;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            SetupManager();
        }

        else if (instance != this)
        {
            Destroy(gameObject);
        }
        Screen.orientation = ScreenOrientation.Portrait;

        DontDestroyOnLoad(gameObject);
    }

    void SetupManager()
    {
        ballScript = GameObject.Find("Ball").GetComponent<BallTestScript>();
        inputManager.transform.SetParent(transform);
        boardCreator = GameObject.Find("BoardCreator").GetComponent<BoardCreator>();
    }

    public void BallHole()
    {
        winscreen.SetActive(true);
    }

    public void Reset()
    {
        winscreen.SetActive(false);
        ballScript.Reset();
        boardCreator.Reset();
        inputManager.Reset();
    }
}
