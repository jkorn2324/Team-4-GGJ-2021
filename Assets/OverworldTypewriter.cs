﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OverworldTypewriter : MonoBehaviour
{
    public GlobalGameStateManager GameState;
    public Text textBox;
    string[] textList;
    public ScriptData Script;
    int currentTextIndex = -1;
    public float typewriterDelay = 0.05f;
    private IEnumerator currentCoroutine;
    private bool coroutineLock;
    private bool shouldWrite;
    private bool skipRequested = false;
    public CanvasGroup Canvas;


    void Awake()
    {
        GameState = FindObjectOfType<GlobalGameStateManager>();
        textList = new string[] { };
        InitTypewriter(false);
        Script = FindObjectOfType<ScriptData>();
    }

    public void SetText(string key)
    {
        InitTypewriter(true);
        textList = Script.overworldTextMap[key];
    }

    public void AdvanceText()
    {
        currentTextIndex++;
        if (currentTextIndex >= textList.Length)
        {
            InitTypewriter(false);
        }
        else
        {
            // todo: make textbox start sound
            StartCoroutine(WriteText());
        }
    }

    public void InitTypewriter(bool shouldWrite)
    {
        textBox.text = "";
        this.shouldWrite = shouldWrite;
        currentTextIndex = -1;
        if (shouldWrite)
        {
            Canvas.alpha = 1f;
        }
        else
        {
            Canvas.alpha = 0f;
        }
    }

    IEnumerator WriteText()
    {
        coroutineLock = true;
        for (int i = 0; i < (textList[currentTextIndex].Length + 1); i++)
        {
            if (skipRequested)
            {
                textBox.text = textList[currentTextIndex];
                skipRequested = false;
                break;
            }
            textBox.text = textList[currentTextIndex].Substring(0, i);
            // todo: make typing sound
            yield return new WaitForSeconds(typewriterDelay);
        }
        coroutineLock = false;
    }

    public void Update()
    {
        if (GameState.GameMode == GlobalGameStateManager.gameMode.overworld)
        {
            if (shouldWrite)
            {
                if (currentTextIndex == -1)
                {
                    AdvanceText();
                }
                if (!coroutineLock)
                {
                    if (Input.GetButtonDown("Interact")) // player continues
                    {
                        UnityEngine.Debug.Log("Advance Text");
                        AdvanceText();
                    }
                }
                else if (Input.GetButtonDown("Interact")) // player skip
                {
                    UnityEngine.Debug.Log("Text Skip Requested");
                    skipRequested = true;
                }
            }
        }
    }
}