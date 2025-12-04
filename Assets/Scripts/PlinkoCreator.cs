using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlinkoCreator : MonoBehaviour
{
    public Ball ball;
    public GameObject peg;
    public GameObject board;
    public GameObject screen;
    public bool useScreen;
    public float ballVertOffset;
	public float ballAreaSize;
    public Vector2 pegRowInitialCenter;
    public float pegRowInitialCount;
	public Vector2 pegRowNextOffset;
	public float pegRowNextAdd;
    public int pegRowMaxCount;
	public bool pegRowMaxAlternate;
	public Vector2 pegRowInternalOffset;
    public int pegRows;
    public float boardDistance;
    public Vector2 boardSize;
	public Vector2 boardCenter;
    public float ballMass;
	public float ballGravityFactor;
	public float ballSize;
    public float pegSize;
    public float pegLengthMargin;
	public float endLength;
	public bool DEBUG_DESTROY;
	public bool DEBUG_CREATE;

	Ball ballObj;
    List<GameObject> pegs;
    GameObject boardObj;
	GameObject screenObj;

	// Start is called once before the first execution of Update after the MonoBehaviour is created
	void Start()
    {
        CreateBoard(false);
	}

    public void CreateBoard(bool destroyOld) {
		if (destroyOld) {
			DestroyBoard();
		}

		ballObj = Instantiate(ball.gameObject, transform).GetComponent<Ball>();
		ballObj.transform.position += transform.up * ballVertOffset;
		ballObj.transform.localScale = Vector3.one * ballSize;
		ballObj.GetComponent<Rigidbody>().mass = ballMass;
		ballObj.gravityFactor = ballGravityFactor;
		ballObj.MaxForce *= ballSize;
		ballObj.endY = ((pegRowInitialCenter.y + pegRowNextOffset.y * (pegRows - 1) - pegSize / 2 - endLength) * transform.up).y - ballSize / 2 - transform.position.y;

		pegs = new List<GameObject>();
		Vector3 pegScale = new Vector3(pegSize, boardDistance + pegLengthMargin, pegSize);
		Quaternion sidewaysRotation = transform.rotation * Quaternion.AngleAxis(90, new Vector3(1, 0));
		for (int i = 0; i < pegRows; i++) {
			Vector3 rowPos = transform.position + ((pegRowInitialCenter.x + pegRowNextOffset.x * i) * transform.right + (pegRowInitialCenter.y + pegRowNextOffset.y * i) * transform.up);

			int rowCount = (int)(pegRowInitialCount + pegRowNextAdd * i);
			if (rowCount > pegRowMaxCount) {
				if (pegRowMaxAlternate && (rowCount & 1) != (pegRowMaxCount & 1)) {
					rowCount = pegRowMaxCount - 1;
				} else {
					rowCount = pegRowMaxCount;
				}
			}

			for (int ii = 0; ii < rowCount; ii++) {
				float relativeToCenter = ii - (rowCount - 1) / 2f;
				Vector3 pegPos = rowPos + relativeToCenter * (pegRowInternalOffset.x * transform.right + pegRowInternalOffset.y * transform.up);

				GameObject pegObj = Instantiate(peg, transform);
				pegObj.transform.position = pegPos;
				pegObj.transform.rotation = sidewaysRotation;
				pegObj.transform.localScale = pegScale;
				pegs.Add(pegObj);
			}
		}

		boardObj = Instantiate(board, transform);
		boardObj.transform.position += transform.rotation * ((Vector3)boardCenter + Vector3.back * boardDistance);
		boardObj.transform.localScale = new Vector3(boardSize.x, 1, boardSize.y);
		boardObj.transform.rotation = sidewaysRotation;

		if (useScreen) {
			screenObj = Instantiate(screen, transform);
			screenObj.transform.position += transform.rotation * ((Vector3)boardCenter + Vector3.forward * boardDistance);
			screenObj.transform.localScale = new Vector3(boardSize.x, 1, boardSize.y);
			screenObj.transform.rotation = sidewaysRotation;
		}
	}

	public void DestroyBoard() {
		if (ballObj != null) {
			Destroy(ballObj.gameObject);
		}
		if (pegs != null) {
			foreach (GameObject peg in pegs) {
				Destroy(peg);
			}
		}
		if (boardObj != null) {
			Destroy(boardObj);
		}
		if (screenObj != null) {
			Destroy(screenObj);
		}
	}

	// Update is called once per frame
	void FixedUpdate()
    {
        if (DEBUG_DESTROY) {
			DEBUG_DESTROY = false;
			DestroyBoard();
		} else if (DEBUG_CREATE) {
			DEBUG_CREATE = false;
			CreateBoard(true);
		}
	}
}
