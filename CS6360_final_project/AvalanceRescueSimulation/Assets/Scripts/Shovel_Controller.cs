﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class Shovel_Controller : MonoBehaviour
{

    public static Vector3 shovelPos;
    public static Vector3 shovelPosForRay;

    float sPosX;
    float sPosZ;
    //public GameObject player;

    public float pForward;
    public float pHeight = -0.3f;
    public bool pEquip;
    public float sOffset = 0f;
    public Transform blade;

    // Start is called before the first frame update
    void Start()
    {
        pForward = 0.6f;
        Transform player = GameObject.Find("OVRCameraRig").GetComponent<Transform>();
        shovelPos = new Vector3(player.position.x, player.position.y + pHeight, player.position.z) + player.forward * pForward;
        //this.transform.rotation = player.transform.rotation;
        this.transform.rotation = Quaternion.Euler(player.rotation.eulerAngles.x, player.rotation.eulerAngles.y + 90f, player.rotation.eulerAngles.z);
        this.transform.position = shovelPos;
        sOffset = 1f;

    }

    // Update is called once per frame
    void Update()
    {
        Transform player = GameObject.Find("OVRCameraRig").GetComponent<Transform>();
        blade = GameObject.Find("Shovel").GetComponent<Transform>();
        shovelPosForRay = blade.transform.position;

        shovelPos = new Vector3(player.transform.position.x, player.transform.position.y, player.transform.position.z) + player.transform.forward * pForward;
        //this.transform.rotation = player.transform.rotation;
        this.transform.rotation = Quaternion.Euler(player.rotation.eulerAngles.x, player.rotation.eulerAngles.y + 90f, player.rotation.eulerAngles.z);
        this.transform.position = shovelPos;
        if (!XRDevice.isPresent)
        {

            this.transform.position = new Vector3(player.transform.position.x, player.transform.position.y, player.transform.position.z) + player.transform.forward * pForward;
            this.transform.rotation = Quaternion.Euler(player.transform.rotation.eulerAngles.x, player.transform.rotation.eulerAngles.y + 180f, player.transform.rotation.eulerAngles.z);
            //this.transform.Rotate(0f, 180f, 0f);
            //float yAngle = this.transform.rotation.eulerAngles.y - beaconAngle.eulerAngles.y;
            //float tAngle = beaconAngle.eulerAngles.y;
        }
        else
        {
            Transform t = GameObject.Find("RightHandAnchor").GetComponent<Transform>();


            //this.transform.position = t.position;
            this.transform.position = new Vector3(t.transform.position.x, t.transform.position.y, t.transform.position.z) + t.forward * sOffset;
            this.transform.rotation = Quaternion.Euler(-t.transform.rotation.eulerAngles.x, t.transform.rotation.eulerAngles.y + 180f, -t.transform.rotation.eulerAngles.z);


            //this.transform.position = new Vector3(player.transform.position.x, player.transform.position.y, player.transform.position.z) + player.transform.forward * bOffset;
            //this.transform.rotation = Quaternion.Euler(player.transform.rotation.eulerAngles.x, player.transform.rotation.eulerAngles.y + 180f, player.transform.rotation.eulerAngles.z);

        }

        GameObject body = GameObject.Find("Body");
        Vector3 burriedPos = transmitting_script.beaconPos;        //bool is_triggered = false;
        int rad = 1;
        if (Vector3.Distance(body.transform.position, shovelPosForRay) < 0.3)
        {
            Debug.Log("Do something here");
            GameObject.Find("FoundTargetCanvas").GetComponent<Canvas>().enabled = true;
            GameObject.Find("FoundTargetCanvas").GetComponent<finalMenu>().EnterMenu();
        }
    }

    //void OnCollisionEnter(Collision collision)
    //{
    //    //Check for a match with the specified name on any GameObject that collides with your GameObject
    //    //Debug.Log("collided with object ", collision.gameObject.transform.name.ToString());
    //        Debug.Log("Do something here");
    //        GameObject.Find("FoundTargetCanvas").GetComponent<Canvas>().enabled = true;
    //        GameObject.Find("FoundTargetCanvas").GetComponent<finalMenu>().EnterMenu();
    //    if (collision.gameObject.name == "Body")
    //    {
    //        //If the GameObject's name matches the one you suggest, output this message in the console

    //    }
    //}
}
