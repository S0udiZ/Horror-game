using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;

public class PlinkoCreator : MonoBehaviour
{
    public Ball ball;
    public GameObject peg;
    public GameObject board;
    public GameObject screen;
	public GameObject wall;
    public bool useScreen;
	public bool useWalls;
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
	public float boardMarginUp;
	public float boardMarginDown;
	public float wallMargin;
    public float ballMass;
	public float ballGravityFactor;
	public float ballSize;
    public float pegSize;
    public float extraDepthMargin;
	public float endLength;
	public uint seed;
	public float maxPegRotate;
	public float maxPegDisplaceX;
	public float maxPegDisplaceY;
	public bool DEBUG_DESTROY;
	public bool DEBUG_CREATE;

	Ball ballObj;
    List<GameObject> pegs;
    GameObject boardObj;
	GameObject screenObj;
	GameObject wallLeft;
	GameObject wallRight;

	// Start is called once before the first execution of Update after the MonoBehaviour is created
	void Start()
    {
        CreateBoard(false);
	}

    public void CreateBoard(bool destroyOld) {
		if (destroyOld) {
			DestroyBoard();
		}

		Unity.Mathematics.Random rng = new(seed);

		ballObj = Instantiate(ball.gameObject, transform).GetComponent<Ball>();
		ballObj.transform.position += ballVertOffset*transform.up + (ballSize - boardDistance)/2*transform.forward;
		ballObj.transform.localScale = Vector3.one * ballSize;
		ballObj.GetComponent<Rigidbody>().mass = ballMass;
		ballObj.gravityFactor = ballGravityFactor;
		ballObj.MaxForce *= ballSize;

		float lowestY = pegRowInitialCenter.y + pegRowNextOffset.y * (pegRows - 1) - pegSize / 2 - endLength;

		ballObj.endY = lowestY*transform.up.y - ballSize / 2 - transform.position.y;

		pegs = new List<GameObject>();
		Vector3 pegScale = new Vector3(pegSize, boardDistance + extraDepthMargin, pegSize);
		Quaternion sidewaysRotation = transform.rotation * Quaternion.AngleAxis(90, new Vector3(1, 0));
		float furthestRight = ballSize/2;
		float furthestLeft = -furthestRight;
		for (int i = 0; i < pegRows; i++) {
			float rowX = pegRowInitialCenter.x + pegRowNextOffset.x*i;
			Vector3 rowPos = transform.position + (rowX * transform.right + (pegRowInitialCenter.y + pegRowNextOffset.y * i) * transform.up);

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
				float pegX = rowX + pegRowInternalOffset.x*relativeToCenter;
				if (ii == 0 && pegX - pegSize/2 < furthestLeft) {
					furthestLeft = pegX - pegSize/2;
				} else if (ii == rowCount - 1 && pegX + pegSize/2 > furthestRight) {
					furthestRight = pegX + pegSize/2;
				}
				Vector3 pegPos = rowPos + relativeToCenter * (pegRowInternalOffset.x * transform.right + pegRowInternalOffset.y * transform.up);

				GameObject pegObj = Instantiate(peg, transform);
				Vector2 randDisplace;
				if (maxPegDisplaceX > 0 || maxPegDisplaceY > 0) {
					randDisplace = new Vector2(rng.NextFloat(-maxPegDisplaceX, maxPegDisplaceX), rng.NextFloat(-maxPegDisplaceY, maxPegDisplaceY));
				} else {
					randDisplace = Vector2.zero;
				}
				pegObj.transform.position = pegPos + randDisplace.x*transform.right + randDisplace.y*transform.up;
				Quaternion randRotation;
				if (maxPegRotate > 0) {
					randRotation = Quaternion.Euler(rng.NextFloat(maxPegRotate), rng.NextFloat(360), 0);
				} else {
					randRotation = Quaternion.identity;
				}
				pegObj.transform.rotation = sidewaysRotation*randRotation;
				pegObj.transform.localScale = pegScale;
				pegs.Add(pegObj);
			}
		}

		Vector2 boardCenter = new Vector2((furthestLeft+furthestRight)/2, (lowestY - boardMarginDown + ballVertOffset + boardMarginUp)/2);
		Vector2 boardSize = new Vector2(furthestRight - furthestLeft + wallMargin*2, ballVertOffset - lowestY + ballSize + boardMarginUp + boardMarginDown);

		boardObj = Instantiate(board, transform);
		boardObj.transform.position += (transform.rotation * (Vector3)boardCenter) - transform.forward * (boardDistance / 2);
		boardObj.transform.localScale = new Vector3(boardSize.x/10, .1f, boardSize.y/10);
		boardObj.transform.rotation = sidewaysRotation;

		if (useScreen) {
			screenObj = Instantiate(screen, transform);
			screenObj.transform.position += (transform.rotation * (Vector3)boardCenter) + transform.forward * (boardDistance / 2);
			screenObj.transform.localScale = new Vector3(boardSize.x/10, .1f, boardSize.y/10);
			screenObj.transform.rotation = transform.rotation * Quaternion.AngleAxis(-90, new Vector3(1, 0));
		}

		if (useWalls) {
			wallLeft = Instantiate(wall, transform);
			wallLeft.transform.position += (furthestLeft - wallMargin)*transform.right + boardCenter.y*transform.up;
			wallLeft.transform.localScale = new Vector3(boardSize.y/10, .1f, (boardDistance + extraDepthMargin)/10);
			wallLeft.transform.rotation = transform.rotation*Quaternion.AngleAxis(-90, new Vector3(0, 0, 1));

			wallRight = Instantiate(wall, transform);
			wallRight.transform.position += (furthestRight + wallMargin) * transform.right + boardCenter.y * transform.up;
			wallRight.transform.localScale = new Vector3(boardSize.y/10, .1f, (boardDistance + extraDepthMargin)/10);
			wallRight.transform.rotation = transform.rotation * Quaternion.AngleAxis(90, new Vector3(0, 0, 1));
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
		if (wallLeft != null) {
			Destroy(wallLeft);
		}
		if (wallRight != null) {
			Destroy(wallRight);
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
