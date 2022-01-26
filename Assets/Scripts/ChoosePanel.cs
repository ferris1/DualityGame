using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChoosePanel : MonoBehaviour
{
    public StagePanel stagePanel;

    public GameObject choose;

    public GameObject[] Endings;

    private void OnEnable()
    {
        choose.SetActive(true);
        foreach (var endings in Endings)
        {
            endings.SetActive(false);
        }
    }

    public void OnChooseKillBoss()
    {
        choose.SetActive(false);
        stagePanel.ShowPanel(DataCenter.GameStage.GameStage4, ()=> {
            Endings[0].SetActive(true);
        });
    }

    public void OnChooseKillShadow()
    {
        choose.SetActive(false);
        stagePanel.ShowPanel(DataCenter.GameStage.GameStage4, () =>
        {
            if (DataCenter.instance.takenDrug)
            {
                Endings[2].SetActive(true);
            }
            else
            {
                Endings[1].SetActive(true);
            }
        });
    }
}
