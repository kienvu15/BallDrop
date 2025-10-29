using NUnit.Framework;
using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameUI : MonoBehaviour
{
    public GameObject homeUI, inGameUI, finishUI, gameOverUI;
    public GameObject allbuttons;
    private bool button;

    [Header("PreGame")]
    public Button soundButton;
    public Sprite soundOnS, soundOffs;

    [Header("InGame")]
    public Image levelSlider;
    public Image currentLevelImg;
    public Image nextLevelImg;
    public Text currentLevelText, nextLevelText;

    [Header("Finish")]
    public Text finishLevelText;

    [Header("GameOver")]
    public Text gameOverScoreText;
    public Text gameOverBestText;

    private Material ballMat;
    private Ball ball;

    private void Awake()
    {
        ballMat = FindFirstObjectByType<Ball>().transform.GetChild(0).GetComponent<MeshRenderer>().material;
        ball = FindFirstObjectByType<Ball>();

        levelSlider.transform.parent.GetComponent<Image>().color = ballMat.color + Color.gray;
        levelSlider.color = ballMat.color;
        currentLevelImg.color = ballMat.color;
        nextLevelImg.color = ballMat.color;

        soundButton.onClick.AddListener(() => SoundManager.instance.SoundOnOff());
    }

    void Start()
    {
        currentLevelText.text = FindFirstObjectByType<LevelSpawner>().level.ToString();
        nextLevelText.text = FindFirstObjectByType<LevelSpawner>().level + 1 + "";
    }

    // Update is called once per frame
    void Update()
    {
        if(ball.ballState == Ball.BallState.Prepare)
        {
            if(SoundManager.instance.sound && soundButton.GetComponent<Image>().sprite != soundOnS)
            {
                soundButton.GetComponent<Image>().sprite = soundOnS;
            }
            else if(!SoundManager.instance.sound && soundButton.GetComponent<Image>().sprite != soundOffs)
            {
                soundButton.GetComponent<Image>().sprite = soundOffs;
            }
        }

        if(Input.GetMouseButtonDown(0) && !IgnoreUI() && ball.ballState == Ball.BallState.Prepare)
        {
            homeUI.SetActive(false);
            inGameUI.SetActive(true);
            ball.ballState = Ball.BallState.Playing; 
            finishUI.SetActive(false);
            gameOverUI.SetActive(false);
        }

        if(ball.ballState == Ball.BallState.Finish)
        {
            homeUI.SetActive(false);
            inGameUI.SetActive(false);
            finishUI.SetActive(true);
            gameOverUI.SetActive(false);

            finishLevelText.text = "Level " + FindFirstObjectByType<LevelSpawner>().level;
        }

        if(ball.ballState == Ball.BallState.Died)
        {
            homeUI.SetActive(false);
            inGameUI.SetActive(false);
            finishUI.SetActive(false);
            gameOverUI.SetActive(true);

            gameOverScoreText.text = ScoreManager.instance.score.ToString();
            gameOverBestText.text = PlayerPrefs.GetInt("HighScore", 0).ToString();

            if (Input.GetMouseButtonDown(0))
            {
                ScoreManager.instance.ResetScore();
                SceneManager.LoadScene(0);
            }
        }
    }

    private bool IgnoreUI()
    {
        PointerEventData pointerEvenData = new PointerEventData(EventSystem.current);
        pointerEvenData.position = Input.mousePosition;

        List<RaycastResult> raycastResultList = new List<RaycastResult>();
        EventSystem.current.RaycastAll(pointerEvenData, raycastResultList);
        for(int i = 0; i< raycastResultList.Count; i++)
        {
            if (raycastResultList[i].gameObject.GetComponent<Ignore>() != null)
            {
                raycastResultList.RemoveAt(i);
                i--;
            }
        }
        return raycastResultList.Count > 0;
    }

    public void LevelSliderFill(float fillAmount)
    {
        levelSlider.fillAmount = fillAmount;
    }

    public void Settings()
    {
        button = !button;
        allbuttons.SetActive(button);
    }
}
