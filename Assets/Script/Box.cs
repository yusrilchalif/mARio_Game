using UnityEngine;
using System.Collections;

public class Box : MonoBehaviour {
	AudioSource coinSound;
	public AudioClip coinClip;
	public GameObject coinModel;
	private Shader shader;
	public Renderer rend;
	bool boxHit = false;
	// Use this for initialization
	void Start () {
		rend = GetComponent<Renderer> ();
		coinModel.SetActive (false);
		coinSound = GetComponent<AudioSource> ();
		shader = Shader.Find ("02 - Default");
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnCollisionEnter(Collision info) {
		if (info.gameObject.name == "MarioWhite" && boxHit == false) {
			coinSound.clip = coinClip;
			coinSound.Play ();
			coinModel.SetActive (true);
			Destroy (coinModel, 1.5f);

			Color whiteColor = new Color (1, 1, 1, 1.0f);
			gameObject.GetComponent<Renderer> ().material.color = whiteColor;
			boxHit = true;

            FindObjectOfType<PlayerMovement>().score++;
		}
		if (boxHit == true) {
			
		}
	}
}
