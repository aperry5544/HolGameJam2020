using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    [SerializeField]
    private Text splashText = null;

    [SerializeField]
    private Image splashImage = null;

    [SerializeField]
    private Text titleText = null;

    [SerializeField]
    private Image titleImage = null;

    [SerializeField]
    private float welcomeDuration = 2.0f;

    [SerializeField]
    private float fadeDuration = 2.0f;

    private float currentWelcome = 2.0f;
    private float currentFade = 2.0f;
    private MenuState currentState = MenuState.Welcome;

    private void Start()
    {
        UpdateState(MenuState.Welcome);
    }

    private enum MenuState
    {
        Welcome,
        Fade,
        Title,
    }

    void Update()
    {
        switch (currentState)
        {
            case MenuState.Welcome:
                WelcomeUpdate();
                break;
            case MenuState.Fade:
                FadeUpdate();
                break;
            case MenuState.Title:
                TitleUpdate();
                break;
            default:
                break;
        }
    }

    private void UpdateState(MenuState newState)
    {
        switch (newState)
        {
            case MenuState.Welcome:
                currentWelcome = welcomeDuration;
                currentFade = fadeDuration;
                splashImage.enabled = true;
                splashText.enabled = true;
                titleText.enabled = false;
                titleImage.enabled = false;
                break;
            case MenuState.Fade:
                break;
            case MenuState.Title:
                splashText.enabled = false;
                splashImage.enabled = false;
                titleText.enabled = true;
                titleImage.enabled = true;
                break;
            default:
                break;
        }

        currentState = newState;
    }

    private void WelcomeUpdate()
    {
        if (currentWelcome > 0)
        {
            currentWelcome -= Time.deltaTime;
        }
        else
        {
            UpdateState(MenuState.Fade);
        }
    }

    private void FadeUpdate()
    {
        if (currentFade > 0)
        {
            currentFade -= Time.deltaTime;

            float percentageComplete = (1 - Mathf.Clamp01(currentFade / (fadeDuration))) * 1.25f;

            splashText.color = Color.Lerp(Color.black, Color.white, percentageComplete);
        }
        else
        {
            UpdateState(MenuState.Title);
        }
    }

    private void TitleUpdate()
    {
        //if (Input.GetKeyDown())
    }

}
