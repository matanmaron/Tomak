using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(Rigidbody2D))]
public class Enemy : MonoBehaviour
{
    public int WalkDistanceX;
    public float WalkSpeed;
    public bool mirror;
    public float SleepTime = 1;
    public GameObject zObject = null;

    private float rot, _startScale;
    private Rigidbody2D rig;
    private Vector3 StartPoint;
    private Vector3 EndPoint;
    Vector3 dir = Vector3.zero;
    private bool _isSleeping = false;

    void Start()
    {
        if (zObject != null)
        {
            zObject.SetActive(false);
        }
        StartPoint = transform.position;
        EndPoint = new Vector3(transform.position.x + WalkDistanceX, transform.position.y, transform.position.z);
        rig = gameObject.GetComponent<Rigidbody2D>();
        _startScale = transform.localScale.x;
        dir.x = 1;
    }

    private void Update()
    {
        if (Mathf.Abs(transform.position.x - StartPoint.x) < 0.5f)
        {
            dir.x = 1;
        }
        else if (Mathf.Abs(transform.position.x - EndPoint.x) < 0.5f)
        {
            dir.x = -1;
        }
    }


    void FixedUpdate()
    {
        if (_isSleeping)
        {
            return;
        }
        dir.Normalize();

        if (dir.x > 0)
            mirror = false;
        if (dir.x < 0)
            mirror = true;

        if (!mirror)
        {
            rot = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            transform.localScale = new Vector3(_startScale, _startScale, 1);
            //_Blade.transform.rotation = Quaternion.AngleAxis(rot, Vector3.forward);
        }
        if (mirror)
        {
            rot = Mathf.Atan2(-dir.y, -dir.x) * Mathf.Rad2Deg;
            transform.localScale = new Vector3(-_startScale, _startScale, 1);
            //_Blade.transform.rotation = Quaternion.AngleAxis(rot, Vector3.forward);
        }
        if (rig!= null)
        {
            rig.velocity = new Vector2(dir.x * WalkSpeed * Time.deltaTime, rig.velocity.y);
        }
    }

    public bool IsMirror()
    {
        return mirror;
    }

    public void Sleep()
    {
        if (_isSleeping)
        {
            return;
        }
        StartCoroutine(MakeMeSleep());
    }

    IEnumerator MakeMeSleep()
    {
        GetComponent<Rigidbody2D>().simulated = false;
        zObject.SetActive(true);
        _isSleeping = true;
        yield return new WaitForSeconds(SleepTime);
        zObject.SetActive(false);
        _isSleeping = false;
        GetComponent<Rigidbody2D>().simulated = true;
    }
}