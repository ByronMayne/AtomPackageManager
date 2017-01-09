using UnityEngine;
using System.Collections;

public class TESTING : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        this.transform.position += Vector3.forward * Time.deltaTime;
	}
}
