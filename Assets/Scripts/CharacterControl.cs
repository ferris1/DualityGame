using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterControl : MonoBehaviour
{
    public float moveSpeed = 10;
    public float rotationSpeed = 10.0f;
    Rigidbody myrig;
    public float bulletInterval = 0.2f;
    public DataCenter.PlayerEnum player;
    public GameObject BulletObj;

    private float bulletRemainTimes = 0.0f;
    public bool isStage0 = false;

    public string horizontalKey;
    public string verticalKey;
    public string rotationKey;
    public string shootHorizontalKey;
    public string shootVerticalKey;
    public string fireKey;
    AudioSource[] m_AudioSouce;
    #region Animator
    public Animator playerAnimator;
    #endregion

    float rotation = 0.0f;

    public void ResetData()
    {
        rotation = 0.0f;
        bulletRemainTimes = 0.0f;
        isStage0 = false;
        GetComponent<Health>().health = DataCenter.instance.PlayerInitHealth;
    }
    // Start is called before the first frame update
    void Start()
    {
        m_AudioSouce = GetComponents<AudioSource>();
        myrig = GetComponent<Rigidbody>();
        GetComponent<Health>().health = DataCenter.instance.PlayerInitHealth;
    }

    void UpdateRotation()
    {
        var rot = Input.GetAxisRaw(rotationKey);
        rotation += rot * rotationSpeed * Time.deltaTime;
        rotation = Mathf.Repeat(rotation, 360.0f);
    }

    // Update is called once per frame
    void Update()
    {
        //myrig.velocity = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical")).normalized * moveSpeed;
        UpdateRotation();
        float input_h = Input.GetAxis(horizontalKey);
        float input_v = Input.GetAxis(verticalKey);
        Vector3 shootAxis = new Vector3(Input.GetAxis(shootHorizontalKey), 0.0f, Input.GetAxis(shootVerticalKey));
        bool shoot = false;
        if(shootAxis.sqrMagnitude > 0)
        {
            shoot = true;
            shootAxis.Normalize();
            //rotation = Mathf.Atan2(shootAxis.y, shootAxis.x);
        }
        bool isTriggered = Input.GetAxisRaw(fireKey) > 0.0f;
        if(Mathf.Abs(input_h) > 0.0f || Mathf.Abs(input_v) > 0.0f)
        {
            DataCenter.instance.TrySetWitchMove(player);
        }
        else
        {
            DataCenter.instance.TryResetWhichMove(player);
        }
        transform.localRotation = Quaternion.Euler(0.0f, Mathf.Atan2(input_h, input_v) * Mathf.Rad2Deg, 0.0f);

        Vector3 vec = new Vector3(input_h, 0, input_v);
        if (DataCenter.instance.GetWitchMove() == player)
        {
            vec = vec.normalized;
            vec = vec * moveSpeed;
            myrig.velocity = vec;
            playerAnimator.SetFloat("Speed", 1.0f);
            if (!m_AudioSouce[1].isPlaying) {
                m_AudioSouce[1].Play();
            }
        }
        else
        {
            myrig.velocity = Vector3.zero;
            playerAnimator.SetFloat("Speed", 0.0f);
            if (m_AudioSouce[1].isPlaying)
            {
                m_AudioSouce[1].Stop();
            }
        }

        if (shoot)
        {
            bulletRemainTimes -= Time.deltaTime;
            if (bulletRemainTimes <= 0.0f)
            {
                bulletRemainTimes = bulletInterval;
                var bulletInstance = GameObject.Instantiate(BulletObj);
                bulletInstance.transform.position = transform.position;
                bulletInstance.transform.position = transform.position;

                var forward = vec.normalized;
                bulletInstance.transform.forward = shootAxis;
                bulletInstance.GetComponent<Bullet>().which = player;
                m_AudioSouce[0].Play();
            }
        }
        float dist = (DataCenter.instance.boss.transform.position - transform.position).magnitude;
        if (dist > DataCenter.instance.ChaseDist)
        {
            isStage0 = true;
        }

        if(Input.GetKeyDown(KeyCode.O))
        {
            DataCenter.instance.TakePosion();
        }
    }
}
