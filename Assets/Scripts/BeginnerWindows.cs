using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BeginnerWindows : MonoBehaviour
{

    public string[] titleList;
    public string[] contentList;
    public Text titleText;
    public Text contentText;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ShowStageText(DataCenter.GameStage stage)
    {
        var stageId = (int)stage;
        titleText.text = titleList[stageId];
        contentText.text = contentList[stageId];
    }
}
