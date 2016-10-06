using UnityEngine;
using System.Collections;

public class PhysicsScript : MonoBehaviour {
	[HideInInspector]
	public bool freeze;
	[HideInInspector]
	public float airTime = 0;
	
	private float moveDirection = 0;
	private float verticalForce = 0;
	private float horizontalForce = 0;
	private float verticalTotalForce = 0;
	private int groundLayer;
	private int groundMask;
	private int currentAirJumps;
	private int bounceTimes;
	private bool isBouncing;
	private float appliedGravity;
	
	private ControlsScript myControlsScript;
	private HitBoxesScript myHitBoxesScript;
	private MoveSetScript myMoveSetScript;
	private GameObject character;

	public void Start(){
		/*Plane groundPlane = (Plane) GameObject.FindObjectOfType(typeof(Plane));
		if (groundPlane == null) Debug.LogError("Plane not found. Please add a plane mesh to your stage prefab!");*/

		groundLayer = LayerMask.NameToLayer("Ground");
  		groundMask = 1 << groundLayer;
		myControlsScript = GetComponent<ControlsScript>();
		character = myControlsScript.character;
		myHitBoxesScript = character.GetComponent<HitBoxesScript>();
		myMoveSetScript = character.GetComponent<MoveSetScript>();
		appliedGravity = myControlsScript.myInfo.physics.weight * UFE.config.gravity;
	}
	
	public void move(int mirror, float direction){
		if (!isGrounded()) return;
		moveDirection = direction;
		if (mirror == 1){
			horizontalForce = myControlsScript.myInfo.physics.moveForwardSpeed * direction;
		}else{
			horizontalForce = myControlsScript.myInfo.physics.moveBackSpeed * direction;
		}
	}
	
	public void jump(){
		if (currentAirJumps >= myControlsScript.myInfo.physics.multiJumps) return;
		currentAirJumps ++;
		horizontalForce = myControlsScript.myInfo.physics.jumpDistance * moveDirection;
		verticalForce = myControlsScript.myInfo.physics.jumpForce;
		setVerticalData(myControlsScript.myInfo.physics.jumpForce);
	}
	
	public void resetForces(bool resetX, bool resetY){
		if (resetX) horizontalForce = 0;
		if (resetY) verticalForce = 0;
	}
	
	public void addForce(Vector2 push, int mirror){
		push.x *= mirror;
		isBouncing = false;
		if (!myControlsScript.myInfo.physics.cumulativeForce){
			horizontalForce = 0;
			verticalForce = 0;
		}
		if (verticalForce < 0 && push.y > 0) verticalForce = 0;
		horizontalForce += push.x;
		verticalForce += push.y;
		setVerticalData(verticalForce);
	}
	
	
	void setVerticalData(float appliedForce){
		float maxHeight = Mathf.Pow(appliedForce,2) / (appliedGravity * 2);
		maxHeight += transform.position.y;
		airTime = Mathf.Sqrt(maxHeight * 2 / appliedGravity);
		verticalTotalForce = appliedGravity * airTime;
	}
	
	public void applyForces(MoveInfo move) {
		//if (myControlsScript.debugger != null) myControlsScript.debugger.text = "isBouncing = " + isBouncing + "\n";
		if (freeze) return;
		
		float appliedFriction = (moveDirection != 0 || myControlsScript.myInfo.physics.highMovingFriction) ? 100 : myControlsScript.myInfo.physics.friction;
		if (!isGrounded()) {
			appliedFriction = 0;
			if (verticalForce == 0) verticalForce = -.1f;
		}
		
		if (horizontalForce != 0) {
			if (horizontalForce > 0) {
				horizontalForce -= appliedFriction * Time.deltaTime;
				horizontalForce = Mathf.Max(0, horizontalForce);
			}else if (horizontalForce < 0) {
				horizontalForce += appliedFriction * Time.deltaTime;
				horizontalForce = Mathf.Min(0, horizontalForce);
			}
			transform.Translate(horizontalForce * Time.deltaTime, 0, 0);
		}
		
		if (move == null || (move != null && !move.ignoreGravity)){
			if ((verticalForce < 0 && !isGrounded()) || verticalForce > 0) {
				verticalForce -= appliedGravity* Time.deltaTime;
				transform.Translate(moveDirection * myControlsScript.myInfo.physics.jumpDistance * Time.deltaTime, verticalForce * Time.deltaTime, 0);
			} else if (verticalForce < 0 && isGrounded()) {
				currentAirJumps = 0;
				verticalForce = 0;
			}
		}
		
		/*if (myControlsScript.debugger != null) {
			myControlsScript.debugger.text = "isBouncing = " + isBouncing + "\n";
			myControlsScript.debugger.text += "controlsScript.stunTime = " + controlsScript.stunTime + "\n";
			myControlsScript.debugger.text += "Animations:\n";
			foreach(AnimationState animState in character.animation){
				if (character.animation.IsPlaying(animState.name)){
					myControlsScript.debugger.text += "<color=#003300>"+ animState.name +"</color>\n";
					myControlsScript.debugger.text += "<color=#003300>"+ animState.speed +"</color>\n";
				}
			}
		}*/
		
		if (isGrounded()){
			if (verticalTotalForce != 0) {
				if (bounceTimes < UFE.config.bounceOptions.maximumBounces && myControlsScript.stunned &&
					UFE.config.bounceOptions.bounceForce != Sizes.None &&
					verticalForce <= -UFE.config.bounceOptions.minimumBounceForce) {
					if (!UFE.config.bounceOptions.bounceHitBoxes) myHitBoxesScript.hideHitBoxes();
					if (UFE.config.bounceOptions.bounceForce == Sizes.Small){ 
						addForce(new Vector2(0,-verticalForce/2.4f), 1);
					}else if (UFE.config.bounceOptions.bounceForce == Sizes.Medium){ 
						addForce(new Vector2(0,-verticalForce/1.8f), 1);
					}else if (UFE.config.bounceOptions.bounceForce == Sizes.High){ 
						addForce(new Vector2(0,-verticalForce/1.2f), 1);
					}
					bounceTimes ++;
					if (!isBouncing){
						myControlsScript.stunTime += airTime + UFE.config.knockDownOptions.knockedOutTime;
						myMoveSetScript.playBasicMove(myMoveSetScript.basicMoves.bounce);
						if (UFE.config.bounceOptions.bouncePrefab != null) {
							GameObject pTemp = (GameObject) Instantiate(UFE.config.bounceOptions.bouncePrefab);
							pTemp.transform.parent = transform;
							pTemp.transform.localPosition = Vector3.zero;
							Destroy(pTemp, 3);
						}
						isBouncing = true;
					}
					return;
				}
				verticalTotalForce = 0;
				airTime = 0;
				myMoveSetScript.totalAirMoves = 0;
				BasicMoveInfo airAnimation = null;
				if (myControlsScript.stunned){
					myControlsScript.stunTime = UFE.config.knockDownOptions.knockedOutTime + UFE.config.knockDownOptions.getUpTime;
					airAnimation = myMoveSetScript.basicMoves.fallDown;
					myControlsScript.currentState = PossibleStates.Down;
					if (!UFE.config.knockDownOptions.knockedOutHitBoxes) myHitBoxesScript.hideHitBoxes();
				}else{
					if ((myControlsScript.currentMove != null && myControlsScript.currentMove.cancelMoveWheLanding) ||
					    myControlsScript.currentMove == null){
						airAnimation = myMoveSetScript.basicMoves.landing;
						myControlsScript.KillCurrentMove();
					}
					myControlsScript.currentState = PossibleStates.Stand;
				}
				isBouncing = false;
				bounceTimes = 0;
				if (airAnimation != null && !character.GetComponent<Animation>().IsPlaying(airAnimation.name)){
					myMoveSetScript.playBasicMove(airAnimation);
				}
			}
			
			if (!myControlsScript.stunned && move == null){
				if (moveDirection < 0 && myControlsScript.mirror == -1 ||
					moveDirection > 0 && myControlsScript.mirror == 1) 
					myMoveSetScript.playBasicMove(myMoveSetScript.basicMoves.moveForward);
				
				if (moveDirection < 0 && myControlsScript.mirror == 1||
					moveDirection > 0 && myControlsScript.mirror == -1) 
					myMoveSetScript.playBasicMove(myMoveSetScript.basicMoves.moveBack);
			}
		}else if (verticalForce > 0 || !isGrounded()){
			if (move != null && myControlsScript.currentState == PossibleStates.Stand)
				myControlsScript.currentState = PossibleStates.StraightJump;
			if (move == null && verticalForce/verticalTotalForce > 0 && verticalForce/verticalTotalForce <= 1) {
				if (isBouncing) return;
				BasicMoveInfo airAnimation = myControlsScript.stunned? 
					myMoveSetScript.basicMoves.getHitAir : myMoveSetScript.basicMoves.jumping;
				
				if (moveDirection == 0) {
					myControlsScript.currentState = PossibleStates.StraightJump;
				}else{
					if (moveDirection < 0 && myControlsScript.mirror == -1 ||
					    moveDirection > 0 && myControlsScript.mirror == 1) 
						myControlsScript.currentState = PossibleStates.ForwardJump;
					
					if (moveDirection < 0 && myControlsScript.mirror == 1||
					    moveDirection > 0 && myControlsScript.mirror == -1)
						myControlsScript.currentState = PossibleStates.BackJump;
				}
				
				if (!character.GetComponent<Animation>().IsPlaying(airAnimation.name)){
					//character.animation[airAnimation].speed = character.animation[airAnimation].length * (appliedGravity/verticalTotalForce);
					character.GetComponent<Animation>()[airAnimation.name].speed = character.GetComponent<Animation>()[airAnimation.name].length/airTime;
					myMoveSetScript.playBasicMove(airAnimation);
					
				}
				
			}else if (move == null && verticalForce/verticalTotalForce <= 0) {
				BasicMoveInfo airAnimation;
				if (isBouncing){
					airAnimation = myMoveSetScript.basicMoves.fallingFromBounce;
				}else {
					airAnimation = myControlsScript.stunned? 
						myMoveSetScript.basicMoves.getHitAir : myMoveSetScript.basicMoves.falling;
				}
				
				if (!character.GetComponent<Animation>().IsPlaying(airAnimation.name)){
					//character.animation[airAnimation].speed = character.animation[airAnimation].length * (appliedGravity/verticalTotalForce);
					//character.animation.CrossFade(airAnimation, GlobalScript.getCurrentMoveSet(myControlsScript.myInfo).interpolationSpeed);
					character.GetComponent<Animation>()[airAnimation.name].speed = character.GetComponent<Animation>()[airAnimation.name].length/airTime;
					myMoveSetScript.playBasicMove(airAnimation);
				}
			}
		}
		if (horizontalForce == 0 && verticalForce == 0) moveDirection = 0;
		
		if (UFE.normalizedCam) {
			Vector3 cameraLeftBounds = Camera.main.ViewportToWorldPoint(new Vector3(0,0,-Camera.main.transform.position.z - 10));
			Vector3 cameraRightBounds = Camera.main.ViewportToWorldPoint(new Vector3(1,0,-Camera.main.transform.position.z - 10));
				
			transform.position = new Vector3(
				Mathf.Clamp(transform.position.x,cameraLeftBounds.x,cameraRightBounds.x),
				transform.position.y,
				transform.position.z);
		}
		
		transform.position = new Vector3(
			Mathf.Clamp(transform.position.x,
				UFE.config.selectedStage.leftBoundary,
				UFE.config.selectedStage.rightBoundary),
			transform.position.y,
			transform.position.z);
	}
	
	public bool isGrounded() {
		if (Physics.RaycastAll(transform.position + Vector3.up + new Vector3(0,1f,0), Vector3.down, 2.1f, groundMask).Length > 0) {
			if (transform.position.y != 0) transform.Translate(new Vector3(0, -transform.position.y, 0));
			return true;
		}
		return false;
	}
}
