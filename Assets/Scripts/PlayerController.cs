using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

	[SerializeField] private float thrust, minTiltSmooth, maxTiltSmooth, hoverDistance, hoverSpeed;
	[SerializeField] private float gravityScale;
	[SerializeField] private float maxGravityScale;
	private float defGravityScale;
	private bool start;
	private float timer, tiltSmooth, y;
	private Rigidbody2D playerRigid;
	private Quaternion downRotation, upRotation;
	private SpriteRenderer spriteRenderer;
	private Animator animator;

    private void Awake()
    {
		spriteRenderer = GetComponent<SpriteRenderer>();
		animator = GetComponent<Animator>();
		defGravityScale = gravityScale;
	}

    void Start () {
		tiltSmooth = maxTiltSmooth;
		playerRigid = GetComponent<Rigidbody2D> ();
		downRotation = Quaternion.Euler (0, 0, -90);
		upRotation = Quaternion.Euler (0, 0, 35);
	}

	void Update () {
		if (!start) {
			// Hover the player before starting the game
			timer += Time.deltaTime;
			y = hoverDistance * Mathf.Sin (hoverSpeed * timer);
			transform.localPosition = new Vector3 (0, y, 0);
		} else {
			// Rotate downward while falling
			transform.rotation = Quaternion.Lerp (transform.rotation, downRotation, tiltSmooth * Time.deltaTime);
		}
		// Limit the rotation that can occur to the player
		transform.rotation = new Quaternion (transform.rotation.x, transform.rotation.y, Mathf.Clamp (transform.rotation.z, downRotation.z, upRotation.z), transform.rotation.w);
	}

	void LateUpdate () {
		if (GameManager.Instance.GameState ()) 
		{
			if (Input.GetMouseButtonDown (0)) 
			{
				if(!start)
				{
					// This code checks the first tap. After first tap the tutorial image is removed and game starts
					start = true;
					GameManager.Instance.GetReady ();
					animator.speed = 2;
				}
				StopAllCoroutines();
				tiltSmooth = minTiltSmooth;
				transform.rotation = upRotation;
				playerRigid.MovePosition(playerRigid.position + (Vector2.up * thrust));
				StartCoroutine(SimulateGravity());
				//SoundManager.Instance.PlayTheAudio("Flap");    //TODO Need to fix sound playing problem with error from this line
			}
		}
	}

	private IEnumerator SimulateGravity()
	{
		gravityScale = defGravityScale;
		yield return new WaitForSeconds(0.1f);
        while (true)
        {
			playerRigid.MovePosition(playerRigid.position + (Vector2.down * gravityScale));
			gravityScale = Mathf.Lerp(gravityScale, maxGravityScale, Time.deltaTime);
			yield return null;
		}
	}

	void OnTriggerEnter2D (Collider2D col) {
		if (col.transform.CompareTag ("Score")) {
			Destroy (col.gameObject);
			GameManager.Instance.UpdateScore ();
		} else if (col.transform.CompareTag ("Obstacle")) {
			// Destroy the Obstacles after they reach a certain area on the screen
			foreach (Transform child in col.transform.parent.transform) {
				child.gameObject.GetComponent<BoxCollider2D> ().enabled = false;
			}
			KillPlayer ();
		}
	}

	void OnCollisionEnter2D (Collision2D col) {
		if (col.transform.CompareTag ("Ground")) {
			playerRigid.simulated = false;
			KillPlayer ();
			transform.rotation = downRotation;
		}
	}

	public void KillPlayer () {
		GameManager.Instance.EndGame ();
		playerRigid.velocity = Vector2.zero;
		// Stop the flapping animation
		animator.enabled = false;
		spriteRenderer.material.SetFloat("_EffectAmount", 1f);
	}

}