using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class fps_PlayerHealth : MonoBehaviour {

    public bool isDead;
    public float resetAfterDeathTime = 5;
    public AudioClip deathClip;
    public AudioClip damageClip;
    public float maxHp = 100;
    public float hp = 100;
    public float recoverSpeed = 1;

    private float timer = 0;
    private FadeInOut fader;
    //private 相机黑白

    void Start()
    {
        hp = maxHp;
        fader = GameObject.FindGameObjectWithTag(Tags.fader).GetComponent<FadeInOut>();
        BleedBehavior.BloodAmount = 0;
    }

    void Update()
    {
        if (!isDead)
        {
            hp += recoverSpeed * Time.deltaTime;
            if (hp>maxHp)
            {
                hp = maxHp;
            }
        }
        if (hp<0)
        {
            if (!isDead)
            {
                PlayerDead();
            }
            else
            {
                LevelRest();
            }
        }
    }

    public void TakeDamage(float damage)
    {
        if (isDead)
        {
            return;
        }
        AudioSource.PlayClipAtPoint(damageClip, transform.position);
        BleedBehavior.BloodAmount += Mathf.Clamp01(damage / hp);
        hp -= damage;
    }

    public void DisableInput()
    {
		//GameObject.Find("FP_Player/Weapon_Camera").gameObject.SetActive(false);
        gameObject.GetComponent<AudioSource>().enabled = false;
        gameObject.GetComponent<fps_PlayerControl>().enabled = false;
        gameObject.GetComponent<fps_FPInput>().enabled = false;
        //if (GameObject.Find("Canvas") != null)
        //{
        //    GameObject.Find("Canvas").SetActive(false);
        //}
    }

    public void PlayerDead()
    {
        isDead = true;
        DisableInput();
        AudioSource.PlayClipAtPoint(deathClip, transform.position);
    }

    public void LevelRest()
    {
        timer += Time.deltaTime;
        if (timer>=resetAfterDeathTime)
        {
            fader.EndScene();
        }
    }
}
