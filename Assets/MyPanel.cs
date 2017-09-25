using UnityEngine;
using System.Collections;

public class MyPanel : MonoBehaviour {

	private Vector3 positionToBe;
	private bool positionNeedsUpdate;

	// Use this for initialization
	void Start () {
		positionNeedsUpdate = false;
	}
	
	// Update is called once per frame
	void Update () {
		if (positionNeedsUpdate) {

			Vector3 diff = (Position - positionToBe);
			float length = diff.magnitude;

			if (length < (Screen.width * 0.005f)) {
				this.GetComponent<RectTransform>().transform.position = positionToBe;
				positionNeedsUpdate = false;
			}
			else {
				this.GetComponent<RectTransform>().transform.position -= diff / 5;
			}

			if (length < (Screen.width / 5.0f)) {
				//if panel is off screen. disable
				if ((Position.x > -Screen.width * 3f && Position.x < 0) ||
					(Position.x > Screen.width && Position.x < Screen.width * 3f)) {
					this.GetComponent<RectTransform>().transform.position = positionToBe;
					positionNeedsUpdate = false;
					this.gameObject.SetActive (false);
				}
			}
		}
	}

	public void ForcePosition(Vector3 position) {
		positionToBe = position;
		this.GetComponent<RectTransform>().transform.position = positionToBe;
		positionNeedsUpdate = false;
	}

	public bool IsMoving {
		get {
			return positionNeedsUpdate;
		}
	}

	public Vector3 Position {

		get 
		{
			return this.GetComponent<RectTransform>().transform.position;
		}
		set 
		{
			positionToBe = value;
			positionNeedsUpdate = true;
		}
	}
}
