using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour
{
    public enum DestroyType
    {
        Destroy = 0,
        Hide = 1,
    }
    public int health = 100;
    public DestroyType destroyType;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void HealthChange(int change)
    {
        health += change;
        if(health <= 0)
        {
            if(destroyType == DestroyType.Destroy)
            {
               AudioSource audioSource = gameObject.GetComponent<AudioSource>();
                if (audioSource != null && audioSource.clip.name == "С������") {
                    audioSource.Play();
                }
                GameObject.Destroy(gameObject);
            }
            else
            {
                CharacterControl control = GetComponent<CharacterControl>();
                if(control != null)
                {
                    int playerId = control.player == DataCenter.PlayerEnum.Player1 ? 0 : 1;
                    DataCenter.instance.playerDead[playerId] = true;
                }
                gameObject.SetActive(false);
            }
        }
    }
    public int GetHealth()
    {
        return health;
    }
}
