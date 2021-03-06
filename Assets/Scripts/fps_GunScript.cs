﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public delegate void PlayerShoot();

public class fps_GunScript : MonoBehaviour {

    public static event PlayerShoot PlayerShootEvent;
    public float fireRate = 0.1f;
    public float damage = 40;
    public float reloadTime = 1.5f;
    public float flashRate = 0.02f;
    public AudioClip fireAudio;
    public AudioClip reloadAudio;
    public AudioClip damageAudio;
    public AudioClip dryFireAudio;
    public GameObject explosion;
    public int bulletCount = 30;
    public int chargerBulletCount = 60;
    public Text bulletText;

    private string reloadAnim = "Reload";
    private string fireAnim = "Single_Shot";
    private string walkAnim = "Walk";
    private string runAnim = "Run";
    private string jumpAnim = "Jump";
    private string idleAnim = "Idle";

    private Animation anim;
    private float nextFireTime = 0.0f;
    private MeshRenderer flash;
    private int currentBullet;
    private int currentChargerBullet;
    private fps_PlayerControl playerControl;
    private fps_PlayerParameter parameter;
    private LineRenderer line;
    private Ray ray;
    private Vector3 startPoint;
    private Transform camreaTransform;
    void Start()
    {
        parameter = GameObject.FindGameObjectWithTag(Tags.player).GetComponent<fps_PlayerParameter>();
        playerControl = GameObject.FindGameObjectWithTag(Tags.player).GetComponent<fps_PlayerControl>();
        anim = gameObject.GetComponent<Animation>();
        flash = gameObject.transform.Find("muzzle_flash").GetComponent<MeshRenderer>();
        flash.enabled = false;
        currentBullet = bulletCount;
        currentChargerBullet = chargerBulletCount;
        bulletText.text = currentBullet + "/" + currentChargerBullet;
        line = GameObject.FindGameObjectWithTag("Line").GetComponent<LineRenderer>();
        camreaTransform = GameObject.FindGameObjectWithTag(Tags.mainCamera).GetComponent<Transform>();
    }

    void Update()
    {
        if (parameter.inputReload && currentBullet < bulletCount)
            Reload();
        if (parameter.inputFire && !anim.IsPlaying(reloadAnim))
            Fire();
        else if (!anim.IsPlaying(reloadAnim))
            StateAnim(playerControl.State);

        startPoint = line.gameObject.transform.position - new Vector3(0, 1.2f, 0);
        ray = new Ray(startPoint, camreaTransform.forward);
        //ray = Camera.main.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));
        //Debug.DrawLine(startPoint, line.gameObject.transform.forward * 100, Color.red);
        //Debug.DrawLine(startPoint, camreaTransform.forward * 100, Color.red);
        //line.SetPosition(0, startPoint);
        //line.SetPosition(1, camreaTransform.forward * 100);
    }


    private void ReloadAnim()
    {
        anim.Stop(reloadAnim);
        anim[reloadAnim].speed = (anim[reloadAnim].clip.length / reloadTime);
        anim.Rewind(reloadAnim);
        anim.Play(reloadAnim);
    }

    private IEnumerator ReloadFinsh()
    {
        yield return new WaitForSeconds(reloadTime);
        if (currentChargerBullet >= bulletCount - currentBullet)
        {
            currentChargerBullet -= (bulletCount - currentBullet);
            currentBullet = bulletCount;
        }
        else
        {
            currentBullet += currentChargerBullet;
            currentChargerBullet = 0;
        }
        bulletText.text = currentBullet + "/" + currentChargerBullet;
    }

    private void Reload()
    {
        if (!anim.IsPlaying(reloadAnim))
        {
            if (currentChargerBullet > 0)
                StartCoroutine(ReloadFinsh());
            else
            {
                anim.Rewind(fireAnim);
                anim.Play(fireAnim);
                AudioSource.PlayClipAtPoint(dryFireAudio, transform.position);
                return;
            }
            AudioSource.PlayClipAtPoint(reloadAudio, transform.position);
            ReloadAnim();
        }
    }


    private IEnumerator Flash()
    {
        flash.enabled = true;
        yield return new WaitForSeconds(flashRate);
        flash.enabled = false;
    }

    private void Fire()
    {
        if (Time.time > nextFireTime)
        {
            if (currentBullet <=0)
            {
                Reload();
                nextFireTime = Time.time + fireRate;
                return;
            }
            currentBullet--;
            bulletText.text = currentBullet + "/" + currentChargerBullet;
            DamageEnemy();
            if (PlayerShootEvent != null)
            {
                PlayerShootEvent();
            }
            AudioSource.PlayClipAtPoint(fireAudio, transform.position);
            nextFireTime = Time.time + fireRate;
            anim.Rewind(fireAnim);
            anim.Play(fireAnim);
            StartCoroutine(Flash());
        }
    }

    private void DamageEnemy()
    {
        //Ray ray = Camera.main.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));
        //Ray ray = new Ray(transform.position, transform.forward);
        RaycastHit hit;
        if (Physics.Raycast(ray,out hit))
        {
            AudioSource.PlayClipAtPoint(damageAudio, transform.position);
            GameObject go = Instantiate(explosion, hit.point, Quaternion.identity);
            Destroy(go, 3);
            if (hit.transform.tag == Tags.enemy && hit.collider is CapsuleCollider)
            {             
                hit.transform.GetComponent<fps_EnemyHealth>().TakeDamage(damage);
            }
        }
    }

    private void PlayerStateAnim(string animName)
    {
        if (!anim.IsPlaying(animName))
        {
            anim.Rewind(animName);
            anim.Play(animName);
        }
    }

    private void StateAnim(PlayerState state)
    {
        switch (state)
        {
            case PlayerState.Idle:
                PlayerStateAnim(idleAnim);
                break;
            case PlayerState.Walk:
                PlayerStateAnim(walkAnim);
                break;
            case PlayerState.Crouch:
                PlayerStateAnim(walkAnim);
                break;
            case PlayerState.Run:
                PlayerStateAnim(runAnim);
                break;
        }
    }



}
