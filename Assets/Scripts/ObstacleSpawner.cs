using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleSpawner : MonoBehaviour {

	[SerializeField] private float waitTime;
	[SerializeField] private float stepTopWithBottomPipe;
	[SerializeField] private GameObject obstaclePrefab;
	[SerializeField] private Vector2 pipeTopsLimits;
	[SerializeField] private Vector2 standartPipeStepsRange;
    [SerializeField] private Vector2 hardPipeStepsRange;
	[SerializeField] private EnumDifficultyType[] difficultyTypesOrder;
	private EnumOffsetDirection offsetDirection = EnumOffsetDirection.Upward;
	private int difficultyOrderIndex;
	private float tempTime;
	private float prevPipeYPosition = 4;

	void Start(){
		tempTime = waitTime - Time.deltaTime;
	}

	void LateUpdate () {
		if(GameManager.Instance.GameState()){
			tempTime += Time.deltaTime;
			if(tempTime > waitTime){
				// Wait for some time, create an obstacle, then set wait time to 0 and start again
				tempTime = 0;
				GameObject pipeClone = Instantiate(obstaclePrefab, transform.position, transform.rotation);
				SetNewPipeToRelevantPosition(pipeClone);
			}
		}
	}

	private void SetNewPipeToRelevantPosition(GameObject pipeCloneGO)
    {
        ObstacleBehaviour obstacleBehaviour = pipeCloneGO.GetComponent<ObstacleBehaviour>();
		float yStep = GetNextPipetopOffsetStep();
		float pipetopYpos = prevPipeYPosition;

		if (offsetDirection == EnumOffsetDirection.Upward)
        {
			if (pipetopYpos + yStep <= pipeTopsLimits.y) pipetopYpos += yStep;
			else { offsetDirection = EnumOffsetDirection.Downward; pipetopYpos -= yStep; }
		}
		else
        {
			if (pipetopYpos - yStep >= pipeTopsLimits.x) pipetopYpos -= yStep;
			else { offsetDirection = EnumOffsetDirection.Upward; pipetopYpos += yStep; }
		}

		obstacleBehaviour.PipetopTransform.localPosition = new Vector3(0, pipetopYpos, 0);
		obstacleBehaviour.PipeBottomTransform.localPosition = new Vector3(0, pipetopYpos - stepTopWithBottomPipe, 0);
		prevPipeYPosition = pipetopYpos;


		difficultyOrderIndex++;
		if (difficultyOrderIndex >= difficultyTypesOrder.Length) difficultyOrderIndex = 0;
	}

    private float GetNextPipetopOffsetStep()
    {
		return difficultyTypesOrder[difficultyOrderIndex] == EnumDifficultyType.Easy ? Random.Range(standartPipeStepsRange.x, standartPipeStepsRange.y) :
			Random.Range(hardPipeStepsRange.x, hardPipeStepsRange.y);
	}

	void OnTriggerEnter2D(Collider2D col){
		if(col.gameObject.transform.parent != null){
			Destroy(col.gameObject.transform.parent.gameObject);
		}else{
			Destroy(col.gameObject);
		}
	}

}
