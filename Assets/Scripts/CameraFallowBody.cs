using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFallowBody : MonoBehaviour {

    private Transform m_Transform;
    private Transform player_Transform;

	void Start () {
        m_Transform = gameObject.transform;
        player_Transform = GameObject.FindGameObjectWithTag(Tags.player).GetComponent<Transform>();
	}
	
	
	void LateUpdate () {
        m_Transform.position = player_Transform.position;
	}
}
