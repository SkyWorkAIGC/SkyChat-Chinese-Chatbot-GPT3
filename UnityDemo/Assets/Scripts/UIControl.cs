using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using Button = UnityEngine.UI.Button;

public class UIControl : MonoBehaviour
{
    private Request _request;
    public ChatPanelManager chatPanelMan;
    
    public Button setting;
    public Button register;

    public Button chatExit, startChat;
    public GameObject settingPanel, chatPanel;
    
    
    public Button settingExit, Save;

    public Text apiKey, apiSecret, AIName, YourName, meaningUrl, propertyUrl;
    public Text toast;
    
    private string key = "key", secret = "secret", AI = "AIName", You = "YourName"
        , meaning = "meaning", property = "property";
    // Start is called before the first frame update

    private void Awake()
    {
        Screen.SetResolution(1080,1920,false);
    }

    void Start()
    {
        _request = GetComponent<Request>();
        ButtonEventRegister();
    }

    void ButtonEventRegister()
    {
        register.onClick.AddListener(() =>
        {
            Application.OpenURL("https://openapi.singularity-ai.com/index.html#/login");
        });
        
        setting.onClick.AddListener(() =>
        {
            settingPanel.SetActive(true);
            
            if (CheckIfHasKey(key))
            {
                apiKey.GetComponentInParent<InputField>().text = PlayerPrefs.GetString(key).ToString();
            }
            if (CheckIfHasKey(secret))
            {
                apiSecret.GetComponentInParent<InputField>().text = PlayerPrefs.GetString(secret).ToString();
            }
            if (CheckIfHasKey(AI))
            {
                AIName.GetComponentInParent<InputField>().text = PlayerPrefs.GetString(AI);
                
            }
            if (CheckIfHasKey(You))
            {
                YourName.GetComponentInParent<InputField>().text = PlayerPrefs.GetString(You); 
            }

            meaningUrl.GetComponentInParent<InputField>().text = CheckIfHasKey(meaning) 
                ? PlayerPrefs.GetString(meaning) 
                : _request.meaningUrl;

            propertyUrl.GetComponentInParent<InputField>().text = CheckIfHasKey(property) 
                ? PlayerPrefs.GetString(property) 
                : _request.propertyDrawUrl;
        });
        
        settingExit.onClick.AddListener(() =>
        {
            settingPanel.SetActive(false);
        });
        
        chatExit.onClick.AddListener(() =>
        {
            chatPanel.SetActive(false);
        });
        
        Save.onClick.AddListener(SaveKey);
        
        startChat.onClick.AddListener(StartChat);
        
    }

    void StartChat()
    {
        
        
        if (CheckIfHasKey(key))
        {
            _request.apiKey = PlayerPrefs.GetString(key);
            print(_request.apiKey);
        }
        else
        {
            showToast("No key", 2);
            return;
        }
        if (CheckIfHasKey(secret))
        {
            _request.apiSecret = PlayerPrefs.GetString(secret);
        }else
        {
            showToast("No secret", 2);
            return;
        }
        if (!CheckIfHasKey(AI))
        {
            showToast("No AI Name", 2);
            return;
        }
        if (!CheckIfHasKey(You))
        {
            showToast("No userName", 2);
            return;
        }

        if (CheckIfHasKey(meaning))
        {
            _request.meaningUrl = PlayerPrefs.GetString(meaning);
        }

        if (CheckIfHasKey(property))
        {
            _request.propertyDrawUrl = PlayerPrefs.GetString(property);
        }
        
        _request.LoadData(PlayerPrefs.GetString(You), PlayerPrefs.GetString(AI));
        chatPanel.SetActive(true);
    }

    bool CheckIfHasKey(string key)
    {
        return PlayerPrefs.HasKey(key);
    }
    
    void SaveKey()
    {
        PlayerPrefs.SetString(key, apiKey.text);
        PlayerPrefs.SetString(secret, apiSecret.text);
        PlayerPrefs.SetString(AI, AIName.text);
        PlayerPrefs.SetString(You, YourName.text);
        PlayerPrefs.SetString(meaning, meaningUrl.text);
        PlayerPrefs.SetString(property, propertyUrl.text);
        
    }
    
   

    public void showToast(string text,
        int duration)
    {
        StartCoroutine(showToastCOR(text, duration));
    }

    private IEnumerator showToastCOR(string text,
        int duration)
    {
        Color orginalColor = toast.color;

        toast.text = text;
        toast.enabled = true;

        //Fade in
        yield return fadeInAndOut(toast, true, 0.5f);

        //Wait for the duration
        float counter = 0;
        while (counter < duration)
        {
            counter += Time.deltaTime;
            yield return null;
        }

        //Fade out
        yield return fadeInAndOut(toast, false, 0.5f);

        toast.enabled = false;
        toast.color = orginalColor;
    }

    IEnumerator fadeInAndOut(Text targetText, bool fadeIn, float duration)
    {
        //Set Values depending on if fadeIn or fadeOut
        float a, b;
        if (fadeIn)
        {
            a = 0f;
            b = 1f;
        }
        else
        {
            a = 1f;
            b = 0f;
        }

        Color currentColor = Color.clear;
        float counter = 0f;

        while (counter < duration)
        {
            counter += Time.deltaTime;
            float alpha = Mathf.Lerp(a, b, counter / duration);

            targetText.color = new Color(currentColor.r, currentColor.g, currentColor.b, alpha);
            yield return null;
        }
    }
}
