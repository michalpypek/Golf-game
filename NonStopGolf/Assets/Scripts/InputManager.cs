using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

public class InputManager : MonoBehaviour
{
    public Slider powerSlider;
    public Slider accuracySlider;
    public Slider flagSlider;
    public Slider powerGhostSlider;
    public Text parText;
    public Text shotsText;

    public int offset = 30;
    public float firstAccuracyGainSpeed = 1;  //how fast does the accuracy bar go up, original value
    public float firstPowerGainSpeed = 1;       // how fast does the power bar go up, original value
    public bool inputActive = false;

    // <name, powerSlider max value
    Dictionary<string,float> clubs= new Dictionary<string, float>();

    int par;
    int shotsTaken;

    float powerGainSpeed;
    float accuracyGainSpeed;
    float distance;
    float timer;
    float addPowerValue = 0.1f;
    float addAccuracyValue;

    bool powerRising = false;
    bool accuracyRising = false;
    bool isPlay = false;

    void Awake()
    {
        powerGainSpeed = firstPowerGainSpeed;
        accuracyGainSpeed = firstAccuracyGainSpeed;
        clubs.Add("Normal", 15);
        clubs.Add("Sand", 7.5f);
        clubs.Add("Putter", 3f);
    }

    void Start()
    {
        ResetSliders();
    }

    void ResetSliders()
    {
        flagSlider.value = Mathf.Clamp((Vector2.Distance(GameManager.instance.ballScript.hole.transform.position, GameManager.instance.ballScript.transform.position)), powerSlider.minValue, powerSlider.maxValue);
        powerSlider.value = 0;
        accuracySlider.value = -1;
        powerGhostSlider.gameObject.SetActive(false);
    }

    void FixedUpdate()
    {
        addAccuracyValue = 0.01f * accuracySlider.maxValue;
        addPowerValue = 0.01f * powerSlider.maxValue;

        if (isPlay)
        {
            if (powerRising)
            {
                AddPower();
            }

            if (accuracyRising)
            {
                AddAccuracy();
            }
        }

        UpdateText();
    }

    public void StartPower()
    {
        if (isPlay)
        {
            PickClub();
            ResetSliders();
            inputActive = true;
            powerSlider.value = 0;
            flagSlider.value = Mathf.Clamp((Vector2.Distance(GameManager.instance.ballScript.hole.transform.position, GameManager.instance.ballScript.transform.position)), powerSlider.minValue, powerSlider.maxValue);
            powerRising = true;
            timer = 0;
        }
    }

    void AddPower()
    {
        if(powerSlider.value == powerSlider.maxValue)
        {
            StopPower();
        }

        timer++;

        //if (accuracySlider.value < 2.24f)
        //{
        //    accuracySlider.gameObject.SetActive(true);
        //    accuracySlider.value += addAccuracyValue * Time.deltaTime * accuracyGainSpeed;
        //}

        //else
        //{
            //accuracySlider.gameObject.SetActive(false);
            powerSlider.value += addPowerValue * Time.deltaTime * powerGainSpeed;
            //accuracySlider.value += addAccuracyValue* Time.deltaTime * accuracyGainSpeed;
        //}
    }

    void StopPower()
    {
        powerRising = false;
        powerGhostSlider.gameObject.SetActive(true);
        powerGhostSlider.value = powerSlider.value;
        StartCoroutine(StartAccuracy());
    }

    IEnumerator StartAccuracy()
    {
        yield return new WaitForSeconds(.05f);
        accuracySlider.gameObject.SetActive(true);
        accuracyRising = true;
        timer = 0;
    }

    void AddAccuracy()
    {
        if (accuracySlider.value == accuracySlider.maxValue)
        {
            StopAccuracy();
        }

        if (powerSlider.value == 0 && accuracySlider.value < 0)
        {
            accuracySlider.value = 0;
        }

        if (powerSlider.value > 0)
        {
            powerSlider.value -= addPowerValue * Time.deltaTime * powerGainSpeed;
        }      
     
        else
        {
            powerSlider.value = 0;
            accuracySlider.value += addAccuracyValue * Time.deltaTime * accuracyGainSpeed;
        }
    }

    void StopAccuracy()
    {
        accuracyRising = false;
        GameManager.instance.ballScript.Hit();
        inputActive = false;
        shotsTaken++;
    }

    void PickClub()
    {
        distance = Vector3.Distance(GameManager.instance.ballScript.gameObject.transform.position, GameManager.instance.ballScript.hole.transform.position);
        BoardCreator.TileType onTileType = GameManager.instance.ballScript.onTileType;

        // pick the speed of the bar first
        switch (onTileType)
        {
            case (BoardCreator.TileType.sand):
                powerGainSpeed = firstPowerGainSpeed * 2;
                accuracyGainSpeed = firstAccuracyGainSpeed * 2;
                break;

            case (BoardCreator.TileType.fairway):
                powerGainSpeed = firstPowerGainSpeed;
                accuracyGainSpeed = firstAccuracyGainSpeed;
                break;

            case (BoardCreator.TileType.rough):
                powerGainSpeed = firstPowerGainSpeed * 1.5f;
                accuracyGainSpeed = firstAccuracyGainSpeed * 1.5f;
                break;
        }

        // then choose the correct club (maximum value of the power bar) 

        if (distance >= 4)
        {
            switch (onTileType)
            {
                case (BoardCreator.TileType.sand):
                    powerSlider.maxValue = clubs["Sand"];
                    powerGhostSlider.maxValue = clubs["Sand"];
                    flagSlider.maxValue = clubs["Sand"];
                    break;

                case (BoardCreator.TileType.fairway):
                    powerSlider.maxValue = clubs["Normal"];
                    powerGhostSlider.maxValue = clubs["Normal"];
                    flagSlider.maxValue = clubs["Normal"];
                    break;

                case (BoardCreator.TileType.rough):
                    powerSlider.maxValue = clubs["Normal"];
                    powerGhostSlider.maxValue = clubs["Normal"];
                    flagSlider.maxValue = clubs["Normal"];
                    break;
            }
        }

        else
        {
            powerSlider.maxValue = clubs["Putter"];
            powerGhostSlider.maxValue = clubs["Putter"];
            flagSlider.maxValue = clubs["Putter"];
        }
    }

    void UpdateText()
    {
        parText.text = "Par: " + par;
        shotsText.text = "Shots taken: " + shotsTaken;
    }

    public float GetAccuracy()
    {
        if(powerSlider.value > 0)
        {
            return  Mathf.Clamp(powerSlider.value,0, 1) * -1;
        }
        return accuracySlider.value;
    }

    public float GetPower()
    {
        return powerGhostSlider.value;
    }

    public void Click()
    {
        if (isPlay)
        {
            if (powerRising)
            {
                StopPower();
            }

            if (accuracyRising)
            {
                StopAccuracy();
            }
        }

        else
        {
            isPlay = true;
        }
    }

    public void Reset()
    {
        PickClub();
        isPlay = false;
        ResetSliders();
        shotsTaken = 0;
    }

    public float GetNormalClubRange()
    {
        return clubs["Normal"];
    }
    
    public void SetPar(int x)
    {
        par = x;
    }

}
