using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleBehaviour : MonoBehaviour {
	
	[SerializeField] private float moveSpeed;
	[SerializeField] private Transform pipetopTransform;
	[SerializeField] private Transform pipeBottomTransform;

	public Transform PipetopTransform { get => pipetopTransform; }
    public Transform PipeBottomTransform { get => pipeBottomTransform; }

    void Update () {
		if(GameManager.Instance.GameState()){
			// Continuosly move the obstacles to the left if the game hasn't ended
			transform.position = new Vector2(transform.position.x - Time.deltaTime * moveSpeed, transform.position.y);
		}
	}
}
