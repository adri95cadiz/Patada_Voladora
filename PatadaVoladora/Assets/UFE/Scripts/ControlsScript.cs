using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class InputReferences {
	public InputType inputType;
	public string inputButtonName;
   	public ButtonPress engineRelatedButton;
	
	[HideInInspector]
	public float heldDown;
}

public class ControlsScript : MonoBehaviour {
	[HideInInspector]	public GameObject character;
	[HideInInspector]	public GameObject opponent;
	[HideInInspector]	public CharacterInfo myInfo;
	[HideInInspector]	public CharacterInfo opInfo;
	
	[HideInInspector]	public int mirror;
	[HideInInspector]	public PossibleStates currentState;
	[HideInInspector]	public bool stunned;
	[HideInInspector]	public bool blockStunned;
	[HideInInspector]	public float stunTime;
	[HideInInspector]	public bool isBlocking;
	[HideInInspector]	public GUIText debugger;
	[HideInInspector]	public float potentialParry;
	[HideInInspector]	public bool firstHit;
	[HideInInspector]	public bool introPlayed;
	[HideInInspector]	public int roundsWon;
	[HideInInspector]	public bool isDead;
	[HideInInspector]	public MoveInfo currentMove;
	[HideInInspector]	public MoveInfo storedMove;
	
	private AnimatorStateInfo currentBaseState;
	private int[] animState;
	
	private PhysicsScript myPhysicsScript;
	private MoveSetScript myMoveSetScript;
	private HitBoxesScript myHitBoxesScript;
	
	private PhysicsScript opPhysicsScript;
	private ControlsScript opControlsScript;
	private HitBoxesScript opHitBoxesScript;
	
	private CameraScript cameraScript;
	
	private InputReferences[] inputReferences; // Reference to Unity's InputManager to UFE's keys
	
	private bool ignoreAnimationTransform = true; // Force character to always stay at point (0,0,0) related to parent
	private float standardYRotation;
	
	private Quaternion targetRotation;
	private bool hitDetected;
	private List<ButtonPress> totalButtonPressed;
	private string currentHitAnimation;
	private float hitStunDeceleration = 0;
	private int comboHits;
	private bool potentialBlock;
	private Vector3 pullInLocation;
	private int pullInSpeed;
	private bool shakeCharacter;
	private bool shakeCamera;
	private float shakeDensity;
	private bool animationPaused;
	private AnimationFlow storedAnimationFlow;
	
	private Shader[] normalShaders;
	private Color[] normalColors;
	private bool lit;

	private bool roundMsgCasted;
	private bool outroPlayed;

	private float storedMoveTime;
	
	void Start () {
		if (gameObject.name == "Player1") {
			transform.position = new Vector3(UFE.config.roundOptions.p1XPosition, .09f, 0);
			opponent = GameObject.Find("Player2");
			if (UFE.config.player1Character == null) 
				Debug.LogError("Player 1 character not found! Make sure you have set the characters correctly in the Global Editor");

			myInfo = (CharacterInfo) Instantiate(UFE.config.player1Character);
			UFE.config.player1Character = myInfo;
			inputReferences = UFE.config.player1_Inputs;
			
			debugger = UFE.debugger1;
		}else{
			transform.position = new Vector3(UFE.config.roundOptions.p2XPosition, .09f, 0);
			opponent = GameObject.Find("Player1");
			if (UFE.config.player2Character == null) 
				Debug.LogError("Player 2 character not found! Make sure you have set the characters correctly in the Global Editor");

			myInfo = (CharacterInfo) Instantiate(UFE.config.player2Character);
			UFE.config.player2Character = myInfo;
			inputReferences = UFE.config.player2_Inputs;
			
			debugger = UFE.debugger2;
		}
		
		
		if (myInfo.characterPrefab == null) 
			Debug.LogError("Character prefab for "+ gameObject.name +" not found. Make sure you have selected a prefab character in the Character Editor");
		character = (GameObject) Instantiate(myInfo.characterPrefab);
		character.transform.parent = transform;
		character.AddComponent<MoveSetScript>();

		
		standardYRotation = character.transform.rotation.eulerAngles.y;
		
		myPhysicsScript = GetComponent<PhysicsScript>();
		myMoveSetScript = character.GetComponent<MoveSetScript>();
		myHitBoxesScript = character.GetComponent<HitBoxesScript>();
		cameraScript = transform.parent.GetComponent<CameraScript>();
		
		mirror = 1;
		testCharacterRotation(100);
		if (gameObject.name == "Player2") UFE.FireGameBegins();

		if (UFE.config.roundOptions.allowMovement) {
			UFE.config.lockMovements = false;
		}else{
			UFE.config.lockMovements = true;
		}
	}
	
	private bool isAxisRested(){
		foreach (InputReferences inputRef in inputReferences) {
			if (inputRef.inputType == InputType.Button) continue;
			if (Input.GetAxisRaw(inputRef.inputButtonName) != 0) return false;
		}
		return true;
	}
	
	private void testCharacterRotation(float rotationSpeed){
		if (mirror == -1 && opponent.transform.position.x > transform.position.x) {
			mirror = 1;
			standardYRotation = 360 - standardYRotation;
			character.transform.localScale = new Vector3(-character.transform.localScale.x, character.transform.localScale.y, character.transform.localScale.z);
		}else if (mirror == 1 && opponent.transform.position.x < transform.position.x) {
			mirror = -1;
			standardYRotation = 360 - standardYRotation;
			character.transform.localScale = new Vector3(-character.transform.localScale.x, character.transform.localScale.y, character.transform.localScale.z);
		}
		character.transform.rotation = Quaternion.Slerp(character.transform.rotation, Quaternion.AngleAxis(standardYRotation, Vector3.up), Time.deltaTime * rotationSpeed);
	}
	
	private void fixCharacterRotation(){
		if (currentState == PossibleStates.Down) return;
		if (character.transform.rotation != Quaternion.AngleAxis(standardYRotation, Vector3.up)){
			character.transform.rotation = Quaternion.AngleAxis(standardYRotation, Vector3.up);
		}
	}

	private void StartFight(){
		UFE.FireAlert(UFE.config.selectedLanguage.fight, null);
		UFE.config.lockInputs = false;
		UFE.config.lockMovements = false;
	}

	void Update() {
		if (!myPhysicsScript.isGrounded() || myPhysicsScript.freeze || currentMove != null) fixCharacterRotation();
		//if ((currentMove != null && !currentMove.cancelable) || myPhysicsScript.freeze || isBlocking) return;
		//if (myPhysicsScript.freeze || isBlocking) return;
		if (isBlocking) return;
		if (!myPhysicsScript.freeze && myPhysicsScript.isGrounded() && currentState != PossibleStates.Down) testCharacterRotation(30);
		if (stunned || blockStunned) return;

		if (!myPhysicsScript.freeze && storedMoveTime > 0) storedMoveTime -= Time.deltaTime;
		if (storedMoveTime < 0){
			storedMoveTime = 0;
			storedMove = null;
		}
		if ((currentMove == null || currentMove.cancelable) && storedMove != null && !myPhysicsScript.freeze) {
			if (currentMove != null) KillCurrentMove();
			if (System.Array.IndexOf(storedMove.possibleStates, currentState) != -1) currentMove = storedMove;
			storedMove = null;
			return;
		}

		if (!myPhysicsScript.freeze) potentialBlock = false;
		
		if (!myPhysicsScript.freeze && introPlayed && myPhysicsScript.isGrounded() && isAxisRested() && !character.GetComponent<Animation>().IsPlaying("idle")){
			bool playIdle = true;
			foreach(AnimationState animState in character.GetComponent<Animation>())
            {
				if (animState.name != "idle" && 
					animState.name != "moveForward" && 
					animState.name != "moveBack" && 
					animState.name != "crouching" && 
				    animState.name != "blockingLowPose" && 
					character.GetComponent<Animation>().IsPlaying(animState.name)) {
					playIdle = false;
				}
			}
			if (playIdle) {
				myMoveSetScript.playBasicMove(myMoveSetScript.basicMoves.idle);
				currentState = PossibleStates.Stand;
				//if (GlobalScript.prefs.blockOptions.blockType == BlockType.AutoBlock) potentialBlock = true;
			}
		}

		if (!roundMsgCasted && introPlayed && opControlsScript.introPlayed && gameObject.name == "Player1"){
			UFE.FireRoundBegins(UFE.config.currentRound);
			if (UFE.config.currentRound < UFE.config.roundOptions.totalRounds){
				UFE.FireAlert(SetStringValues(UFE.config.selectedLanguage.round, myInfo), null);
			}else{
				UFE.FireAlert(SetStringValues(UFE.config.selectedLanguage.finalRound, myInfo), null);
			}
			Invoke("StartFight", 2);
			roundMsgCasted = true;
		}

		if (!introPlayed || !opControlsScript.introPlayed) return;
		if (UFE.config.lockInputs && !UFE.config.roundOptions.allowMovement) return;
		if (UFE.config.lockMovements) return;
		foreach (InputReferences inputRef in inputReferences) {
			if ((inputRef.engineRelatedButton == ButtonPress.Down
				|| inputRef.engineRelatedButton == ButtonPress.Up)
				&& Input.GetAxisRaw(inputRef.inputButtonName) == 0 
				&& myPhysicsScript.isGrounded() && !myHitBoxesScript.isHit){
				currentState = PossibleStates.Stand;
			}

			if (inputRef.inputType != InputType.Button && inputRef.heldDown > 0 && Input.GetAxisRaw(inputRef.inputButtonName) == 0) {
				if (inputRef.heldDown >= myInfo.chargeTiming) 
					storedMove = myMoveSetScript.getMove(new ButtonPress[]{inputRef.engineRelatedButton}, inputRef.heldDown, currentMove, true);
				inputRef.heldDown = 0;
				if ((currentMove == null || currentMove.cancelable) && storedMove != null) {
					currentMove = storedMove;
					storedMove = null;
					return;
				}else if (storedMove != null){
					storedMoveTime = UFE.config.storedExecutionDelay;
					return;
				}
			}
			
			if (Input.GetButtonUp(inputRef.inputButtonName)) {
				if (inputRef.heldDown >= myInfo.chargeTiming) 
					storedMove = myMoveSetScript.getMove(new ButtonPress[]{inputRef.engineRelatedButton}, inputRef.heldDown, currentMove, true);
				inputRef.heldDown = 0;
				if ((currentMove == null || currentMove.cancelable) && storedMove != null) {
					currentMove = storedMove;
					storedMove = null;
					return;
				}else if (storedMove != null){
					storedMoveTime = UFE.config.storedExecutionDelay;
					return;
				}
			}
			
			if (inputRef.inputType != InputType.Button && Input.GetAxisRaw(inputRef.inputButtonName) != 0) {
				bool axisPressed = false;
				if (inputRef.inputType == InputType.HorizontalAxis) {
					// Check for potential blocking
					if (inputRef.engineRelatedButton == ButtonPress.Back && UFE.config.blockOptions.blockType == BlockType.HoldBack){
						potentialBlock = true;
					}
					
					if (Input.GetAxisRaw(inputRef.inputButtonName) > 0) {
						inputRef.engineRelatedButton = mirror == -1? ButtonPress.Back : ButtonPress.Foward;
						if (inputRef.heldDown == 0) axisPressed = true;
						inputRef.heldDown += Time.deltaTime;
						if (currentState == PossibleStates.Stand) 
							if (currentMove == null) myPhysicsScript.move(mirror, Input.GetAxisRaw(inputRef.inputButtonName));
					}
					
					if (Input.GetAxisRaw(inputRef.inputButtonName) < 0) {
						inputRef.engineRelatedButton = mirror == -1? ButtonPress.Foward : ButtonPress.Back;
						if (inputRef.heldDown == 0) axisPressed = true;
						inputRef.heldDown += Time.deltaTime;
						if (currentState == PossibleStates.Stand)
							if (currentMove == null) myPhysicsScript.move(mirror * -1, Input.GetAxisRaw(inputRef.inputButtonName));
					}
					// Check for potential parry
					if (((inputRef.engineRelatedButton == ButtonPress.Back && UFE.config.blockOptions.parryType == ParryType.TapBack) ||
						(inputRef.engineRelatedButton == ButtonPress.Foward && UFE.config.blockOptions.parryType == ParryType.TapForward)) &&
						potentialParry == 0 && axisPressed && currentMove == null){
						potentialParry = UFE.config.blockOptions.parryTiming;
					}
				}else{
					if (Input.GetAxisRaw(inputRef.inputButtonName) > 0) {
						inputRef.engineRelatedButton = ButtonPress.Up;
						if (currentMove == null) {
							if (myPhysicsScript.isGrounded()) myPhysicsScript.jump();
							if (inputRef.heldDown == 0) {
								if (!myPhysicsScript.isGrounded() && myInfo.physics.multiJumps > 1)
									myPhysicsScript.jump();
								axisPressed = true;	
							}
						}
						inputRef.heldDown += Time.deltaTime;
					}else if (Input.GetAxisRaw(inputRef.inputButtonName) < 0) {
						inputRef.engineRelatedButton = ButtonPress.Down;
						if (inputRef.heldDown == 0) axisPressed = true;
						inputRef.heldDown += Time.deltaTime;
						
						if (myPhysicsScript.isGrounded()) {
							currentState = PossibleStates.Crouch;
							if (currentMove == null) myMoveSetScript.playBasicMove(myMoveSetScript.basicMoves.crouching);
						}
					}
				}
				if (axisPressed){
					storedMove = myMoveSetScript.getMove(new ButtonPress[]{inputRef.engineRelatedButton}, 0, currentMove, false);
					if ((currentMove == null || currentMove.cancelable) && storedMove != null) {
						currentMove = storedMove;
						storedMove = null;
						return;
					}else if (storedMove != null){
						storedMoveTime = UFE.config.storedExecutionDelay;
						return;
					}
				}
			}
			
			if (inputRef.inputType == InputType.Button && !UFE.config.lockInputs){
				if (Input.GetButton(inputRef.inputButtonName)) {
					if (myMoveSetScript.compareBlockButtons(inputRef.engineRelatedButton)) {
						potentialBlock = true;
						CheckBlocking(true);
					}
					
					if (inputRef.heldDown == 0 && potentialParry == 0 && currentMove == null && 
						myMoveSetScript.compareParryButtons(inputRef.engineRelatedButton)) {
						potentialParry = UFE.config.blockOptions.parryTiming;
					}
					
					inputRef.heldDown += Time.deltaTime;
					if (inputRef.heldDown <= UFE.config.plinkingDelay) {
						foreach (InputReferences inputRef2 in inputReferences) {
							if (inputRef2.inputButtonName != inputRef.inputButtonName && Input.GetButtonDown(inputRef2.inputButtonName)) {
								storedMove = myMoveSetScript.getMove(
								new ButtonPress[]{inputRef.engineRelatedButton, inputRef2.engineRelatedButton}, 0, currentMove, false);
								if (storedMove != null) {
									currentMove = storedMove;
									storedMove = null;
									return;
								}
							}
						}
					}
				}
				
				
				if (Input.GetButtonDown(inputRef.inputButtonName)) {
					storedMove = myMoveSetScript.getMove(new ButtonPress[]{inputRef.engineRelatedButton}, 0, currentMove, false);
					if ((currentMove == null || currentMove.cancelable) && storedMove != null) {
						currentMove = storedMove;
						storedMove = null;
						return;
					}else if (storedMove != null){
						storedMoveTime = UFE.config.storedExecutionDelay;
						return;
					}
				}
				
				if (Input.GetButtonUp(inputRef.inputButtonName)) {
					storedMove = myMoveSetScript.getMove(new ButtonPress[]{inputRef.engineRelatedButton}, 0, currentMove, true);
					if ((currentMove == null || currentMove.cancelable) && storedMove != null) {
						currentMove = storedMove;
						storedMove = null;
					}else if (storedMove != null){
						storedMoveTime = UFE.config.storedExecutionDelay;
						return;
					}else if (storedMove != null){
						storedMoveTime = UFE.config.storedExecutionDelay;
						return;
					}
					if (myMoveSetScript.compareBlockButtons(inputRef.engineRelatedButton)) {
						potentialBlock = false;
						CheckBlocking(false);
					}
				}
			}
		}
	}
	
	void FixedUpdate () {
		if (opHitBoxesScript == null) {
			if (opControlsScript == null) opControlsScript = opponent.GetComponent<ControlsScript>();
			opPhysicsScript = opponent.GetComponent<PhysicsScript>();
			opHitBoxesScript = opponent.GetComponentInChildren<HitBoxesScript>();
			opInfo = opControlsScript.myInfo;
			if (gameObject.name == "Player2" && character.name == opControlsScript.character.name){  // Alternative Costume
				//Renderer charRender = character.GetComponentInChildren<Renderer>();
				//charRender.material.color = myInfo.alternativeColor;

				Renderer[] charRenders = character.GetComponentsInChildren<Renderer>();
				foreach(Renderer charRender in charRenders){
					//charRender.material.shader = Shader.Find("VertexLit");
					charRender.material.color = myInfo.alternativeColor;
					//charRender.material.SetColor("_Emission", myInfo.alternativeColor);
				}
			}
			
			Renderer[] charRenderers = character.GetComponentsInChildren<Renderer>();
			List<Shader> shaderList = new List<Shader>();
			List<Color> colorList = new List<Color>();
			foreach(Renderer char_rend in charRenderers){
				shaderList.Add(char_rend.material.shader);
				colorList.Add(char_rend.material.color);
			}
			normalShaders = shaderList.ToArray();
			normalColors = colorList.ToArray();
		}


		if (currentMove != null) potentialParry = 0;

		if (gameObject.name == "Player1" && !introPlayed && currentMove == null){
			currentMove = myMoveSetScript.getIntro();
			if (currentMove == null) {
				introPlayed = true;
			}else{
				currentMove.currentFrame = 0;
			}
		}else if (gameObject.name == "Player2" && !introPlayed && opControlsScript.introPlayed && currentMove == null){
			currentMove = myMoveSetScript.getIntro();
			if (currentMove == null) {
				introPlayed = true;
			}else{
				currentMove.currentFrame = 0;
			}
		}

		
		if (ignoreAnimationTransform) character.transform.localPosition = new Vector3(0, 0, 0);
		
		if (Vector3.Distance(transform.position, opponent.transform.position) < 10) {
			float totalHits = myHitBoxesScript.testCollision(opHitBoxesScript.hitBoxes);
			if (totalHits > 0) {
				if (transform.position.x < opponent.transform.position.x) {
					transform.Translate(new Vector3(-.05f * totalHits, 0, 0));
				} else {
					transform.Translate(new Vector3(.05f * totalHits, 0, 0));
				}
			}
		}
		
		if (pullInSpeed > 0){
			transform.position = Vector3.Lerp(transform.position, pullInLocation, Time.deltaTime * pullInSpeed);
			if (Vector3.Distance(pullInLocation, transform.position) <= .1f) pullInSpeed = 0;
			
			if (transform.position.y < 0) transform.Translate(new Vector3(0, -transform.position.y, 0));
			if (transform.position.z != 0) transform.Translate(new Vector3(0, 0, -transform.position.z));
		}


		if (currentMove != null) {
			
			/*debugger.text = "";
			if (storedMove != null) debugger.text += storedMove.name + "\n";
			debugger.text += currentMove.name +": "+ character.animation.IsPlaying(currentMove.name) + "\n";
			debugger.text += "frames:"+ currentMove.currentFrame + "/" + currentMove.totalFrames + "\n";
			debugger.text += "animationPaused:"+ animationPaused + "\n";
			if (character.animation.IsPlaying(currentMove.name)){
				debugger.text += "normalizedTime: "+ character.animation[currentMove.name].normalizedTime + "\n";
				debugger.text += "time: "+ character.animation[currentMove.name].time + "\n";
			}*/

			if (currentMove.currentFrame == 0) {
				if (character.GetComponent<Animation>()[currentMove.name] == null) Debug.LogError("Animation for move '"+ currentMove.moveName +"' not found!");
				character.GetComponent<Animation>()[currentMove.name].time = 0;
				character.GetComponent<Animation>().CrossFade(currentMove.name, currentMove.interpolationSpeed);
				character.GetComponent<Animation>()[currentMove.name].speed = currentMove.animationSpeed;
			}

			// ANIMATION FRAME DATA
			if (!animationPaused) currentMove.currentFrame ++;
			if (currentMove.currentFrame == 1) AddGauge(currentMove.gaugeGainOnMiss);

			if (UFE.config.animationFlow == AnimationFlow.MorePrecision){
				character.GetComponent<Animation>()[currentMove.name].speed = 0;
				AnimationState animState = character.GetComponent<Animation>()[currentMove.name];
				animState.time = GetAnimationTime(currentMove.currentFrame);
				//animState.time = ((float)currentMove.currentFrame / (float)GlobalScript.prefs.framesPerSeconds) / (1/currentMove.animationSpeed);
			}           
			
			foreach (MoveParticleEffect particleEffect in currentMove.particleEffects){
				if (!particleEffect.casted && currentMove.currentFrame >=  particleEffect.castingFrame){
					if (particleEffect.particleEffect.prefab == null) 
						Debug.LogError("Particle effect for move "+ currentMove.moveName +" not found. Make sure you have set the prefab for this particle correctly in the Move Editor");
					particleEffect.casted = true;
					GameObject pTemp = (GameObject) Instantiate(particleEffect.particleEffect.prefab);
					pTemp.transform.parent = transform;
					pTemp.transform.localPosition = particleEffect.particleEffect.position;
					Destroy(pTemp, particleEffect.particleEffect.duration);
				}
			}
			
			foreach (AppliedForce addedForce in currentMove.appliedForces){
				if (!addedForce.casted && currentMove.currentFrame >= addedForce.castingFrame){
					myPhysicsScript.resetForces(addedForce.resetPreviousHorizontal, addedForce.resetPreviousVertical);
					myPhysicsScript.addForce(addedForce.force, mirror);
					addedForce.casted = true;
				}
			}
			foreach (SoundEffect soundEffect in currentMove.soundEffects){
				if (!soundEffect.casted && currentMove.currentFrame >= soundEffect.castingFrame){
					if (UFE.config.soundfx) Camera.main.GetComponent<AudioSource>().PlayOneShot(soundEffect.sound);
					soundEffect.casted = true;
				}
			}

			foreach (CameraMovement cameraMovement in currentMove.cameraMovements){
				if (currentMove.currentFrame >= cameraMovement.castingFrame){
					cameraMovement.time += Time.deltaTime;
					if (!cameraMovement.casted){
						myPhysicsScript.freeze = cameraMovement.freezeGame;
						opPhysicsScript.freeze = cameraMovement.freezeGame;
						LockCam(cameraMovement.freezeAnimation);
						cameraMovement.casted = true;
						Vector3 targetPosition = character.transform.TransformPoint(cameraMovement.position);
						Vector3 targetRotation = cameraMovement.rotation;
						targetRotation.y *= mirror;
						targetRotation.z *= mirror;
						cameraScript.moveCameraToLocation(targetPosition,
						                                  targetRotation,
														  cameraMovement.fieldOfView,
														  cameraMovement.camSpeed);
					}
				}
				if (cameraMovement.casted && UFE.freeCamera && cameraMovement.time >= cameraMovement.duration){
					ReleaseCam();
				}
			}
			
			if (currentMove.invincibleBodyParts.Length > 0) {
				foreach (InvincibleBodyParts invBodyPart in currentMove.invincibleBodyParts){
					if (currentMove.currentFrame >= invBodyPart.activeFramesBegin &&
						currentMove.currentFrame < invBodyPart.activeFramesEnds) {
						if (invBodyPart.completelyInvincible){
							myHitBoxesScript.hideHitBoxes();
						}else{
							myHitBoxesScript.hideHitBoxes(invBodyPart.hitBoxes);
						}
					}
					if (currentMove.currentFrame >= invBodyPart.activeFramesEnds) {
						if (invBodyPart.completelyInvincible){
							myHitBoxesScript.showHitBoxes();
						}else{
							myHitBoxesScript.showHitBoxes(invBodyPart.hitBoxes);
						}
					}
				}
			}
			
			if (currentMove.blockableArea.bodyPart != BodyPart.none){
				if (currentMove.currentFrame >= currentMove.blockableArea.activeFramesBegin &&
					currentMove.currentFrame < currentMove.blockableArea.activeFramesEnds) {
					myHitBoxesScript.blockableArea = currentMove.blockableArea;
					Vector3 collisionVector_block = opHitBoxesScript.testCollision(myHitBoxesScript.blockableArea);
					if (collisionVector_block != Vector3.zero) opControlsScript.CheckBlocking(true);
				}else if (currentMove.currentFrame >= currentMove.blockableArea.activeFramesEnds){
					opControlsScript.CheckBlocking(false);
				}
			}
			
			foreach (Hit hit in currentMove.hits){
				if (comboHits >= UFE.config.comboOptions.maxCombo) continue;
				if ((hit.hasHit && currentMove.frameLink.onlyOnHit) || !currentMove.frameLink.onlyOnHit){
					if (currentMove.currentFrame >= currentMove.frameLink.activeFramesBegins) currentMove.cancelable = true;
					if (currentMove.currentFrame >= currentMove.frameLink.activeFramesEnds) currentMove.cancelable = false;
				}
				if (hit.hasHit) continue;

				if (currentMove.currentFrame >= hit.activeFramesBegin &&
					currentMove.currentFrame < hit.activeFramesEnds) {
					if (hit.hurtBoxes.Length > 0){
						myHitBoxesScript.activeHurtBoxes = hit.hurtBoxes;
						
						Vector3 collisionVector_hit = opHitBoxesScript.testCollision(myHitBoxesScript.activeHurtBoxes);
						if (collisionVector_hit != Vector3.zero) { // HURTBOX TEST
							if (!opControlsScript.stunned && opControlsScript.currentMove == null && opControlsScript.isBlocking && opControlsScript.TestBlockStances(hit.hitType)){
								opControlsScript.GetHitBlocking(hit, currentMove.totalFrames - currentMove.currentFrame, collisionVector_hit);
								AddGauge(currentMove.gaugeGainOnBlock);
								opControlsScript.AddGauge(currentMove.opGaugeGainOnBlock);
							}else if (opControlsScript.potentialParry > 0 && opControlsScript.currentMove == null && opControlsScript.TestParryStances(hit.hitType)){
								opControlsScript.GetHitParry(hit, collisionVector_hit);
								opControlsScript.AddGauge(currentMove.opGaugeGainOnParry);
							}else{
								opControlsScript.GetHit(hit, currentMove.totalFrames - currentMove.currentFrame, collisionVector_hit);
								AddGauge(currentMove.gaugeGainOnHit);
								
								if (hit.pullSelfIn.enemyBodyPart != BodyPart.none && hit.pullSelfIn.characterBodyPart != BodyPart.none){
									Vector3 newPos = opHitBoxesScript.getPosition(hit.pullSelfIn.enemyBodyPart);
									if (newPos != Vector3.zero){
										pullInLocation = transform.position + (newPos - hit.pullSelfIn.position.position);
										pullInSpeed = hit.pullSelfIn.speed;
									}
								}
							}
							myPhysicsScript.resetForces(hit.resetPreviousHorizontal, hit.resetPreviousVertical);
							myPhysicsScript.addForce(hit.appliedForce, mirror);
							
							if ((opponent.transform.position.x >= UFE.config.selectedStage.rightBoundary - 2 ||
								opponent.transform.position.x <= UFE.config.selectedStage.leftBoundary + 2) &&
								myPhysicsScript.isGrounded()){
								
								myPhysicsScript.addForce(
									new Vector2(hit.pushForce.x + (opPhysicsScript.airTime * opInfo.physics.friction), 0), 
									mirror * -1);
							}
							
							HitPause();
							Invoke("HitUnpause",GetFreezingTime(hit.hitStrengh));
							if (!hit.continuousHit) hit.hasHit = true;
						};
					}
				}
			}
			
			if(currentMove.currentFrame >= currentMove.totalFrames) {
				if (currentMove == myMoveSetScript.getIntro()) introPlayed = true;
				KillCurrentMove();
			}
		}

		if (myHitBoxesScript.isHit){
			if (myPhysicsScript.freeze) {
				if (shakeDensity > 0) shakeDensity -= Time.deltaTime; 
				if (shakeDensity < 0) shakeDensity = 0;
				if (shakeCharacter) shake();
				if (shakeCamera) shakeCam();
			}else{
				shakeCamera = false;
				shakeCharacter = false;
				shakeDensity = 0;
			}
		}
		
		if (potentialParry > 0){
			potentialParry -= Time.deltaTime;
			if (potentialParry <= 0) potentialParry = 0;
		}
		
		if ((stunned || blockStunned) && stunTime > 0 && !myPhysicsScript.freeze){
			character.GetComponent<Animation>()[currentHitAnimation].speed -= hitStunDeceleration * Time.deltaTime;
			if (UFE.config.comboOptions.neverAirRecover && !myPhysicsScript.isGrounded()){
				stunTime = 1;
			}else{
				stunTime -= Time.deltaTime;
			}
			/*if (myPhysicsScript.debugger != null){
				myPhysicsScript.debugger.text = "";
				myPhysicsScript.debugger.text += "<color=#003300>animation speed: "+ character.animation[currentHitAnimation].speed +"</color>\n";
				myPhysicsScript.debugger.text += "<color=#003300>stunTime: "+ stunTime +"</color>\n";
			}*/
			if (!isDead && stunTime <= UFE.config.knockDownOptions.getUpTime){
				if (currentState == PossibleStates.Down && myPhysicsScript.isGrounded()){
					currentState = PossibleStates.Stand;
					character.GetComponent<Animation>().Play("getUp");
				}
			}
			if (stunTime <= 0) ReleaseStun();
		}
		if (pullInSpeed == 0) myPhysicsScript.applyForces(currentMove);
	}
	
	// Imediately cancels any move being executed
	public void KillCurrentMove(){
		if (currentMove == null) return;
		/*currentMove.currentFrame = 0;
		currentMove.cancelable = false;
		currentMove.kill = false;
		foreach (Projectile projectile in currentMove.projectiles) projectile.casted = false;
		foreach (AppliedForce addedForce in currentMove.appliedForces) addedForce.casted = false;
		foreach (SoundEffect soundEffect in currentMove.soundEffects) soundEffect.casted = false;
		foreach (CameraMovement cameraMovement in currentMove.cameraMovements) cameraMovement.casted = false;
		foreach (MoveParticleEffect moveParticleEffect in currentMove.particleEffects) moveParticleEffect.casted = false;
		foreach (Hit hit in currentMove.hits) hit.hasHit = false;*/
		currentMove.currentFrame = 0;
		myHitBoxesScript.activeHurtBoxes = null;
		myHitBoxesScript.blockableArea = null;
		myHitBoxesScript.showHitBoxes();
		opControlsScript.CheckBlocking(false);
		character.GetComponent<Animation>()[currentMove.name].speed = currentMove.animationSpeed;

		currentMove = null;
		ReleaseCam();

	}

	private void LockCam(bool freezeAnimation){
		myHitBoxesScript.hideHitBoxes();
		opControlsScript.PausePlayAnimation(true);
		if (freezeAnimation) PausePlayAnimation(true);
	}

	private void ReleaseCam(){
		if (outroPlayed && UFE.config.roundOptions.freezeCamAfterOutro) return;
		myHitBoxesScript.showHitBoxes();
		opControlsScript.PausePlayAnimation(false);
		PausePlayAnimation(false);
		UFE.freeCamera = false;
		myPhysicsScript.freeze = false;
		opPhysicsScript.freeze = false;
	}

	public bool TestBlockStances(HitType hitType){
		if (UFE.config.blockOptions.blockType == BlockType.None) return false;
		if (hitType == HitType.Unblockable) return false;
		if (hitType == HitType.Overhead && currentState == PossibleStates.Crouch) return false;
		if ((hitType == HitType.Knockdown || hitType == HitType.Low) && currentState != PossibleStates.Crouch) return false;
		if ((hitType == HitType.HighLow || hitType == HitType.Launcher) && myPhysicsScript.isGrounded()) return true;
		if (UFE.config.blockOptions.allowAirBlock && !myPhysicsScript.isGrounded()) return true;
		return true;
	}
	
	public bool TestParryStances(HitType hitType){
		if (UFE.config.blockOptions.parryType == ParryType.None) return false;
		if (hitType == HitType.Unblockable) return false;
		if (hitType == HitType.Overhead && currentState == PossibleStates.Crouch) return false;
		if ((hitType == HitType.Knockdown || hitType == HitType.Low) && currentState != PossibleStates.Crouch) return false;
		if ((hitType == HitType.HighLow || hitType == HitType.Launcher) && myPhysicsScript.isGrounded()) return true;
		if (UFE.config.blockOptions.allowAirParry && !myPhysicsScript.isGrounded()) return true;
		return true;
	}
	
	public void CheckBlocking(bool flag){
		if (flag){
			if (potentialBlock && !isBlocking){
				if (currentState == PossibleStates.Crouch){
					myMoveSetScript.playBasicMove(myMoveSetScript.basicMoves.blockingLowPose);
					isBlocking = true;
				}else if (currentState == PossibleStates.Stand){
					myMoveSetScript.playBasicMove(myMoveSetScript.basicMoves.blockingHighPose);
					isBlocking = true;
				}else if (!myPhysicsScript.isGrounded() && UFE.config.blockOptions.allowAirBlock){
					myMoveSetScript.playBasicMove(myMoveSetScript.basicMoves.blockingAirPose);
					isBlocking = true;
				}
			}
		}else if (!blockStunned){
			isBlocking = false;
		}
	}
	
	private void HighlightOn(GameObject target, bool flag){
		Renderer[] charRenders = target.GetComponentsInChildren<Renderer>();
		if (flag && !lit){
			lit = true;
			foreach(Renderer charRender in charRenders){
				charRender.material.shader = Shader.Find("VertexLit");
				charRender.material.color = UFE.config.blockOptions.parryColor;
			}
		}else if (lit){
			lit = false;
			for(int i = 0; i < charRenders.Length; i ++){
				charRenders[i].material.shader = normalShaders[i];
				charRenders[i].material.color = normalColors[i];
			}
		}
	}
	
	private void HighlightOff(){
		HighlightOn(character, false);
	}
	
	public void GetHitParry(Hit hit, Vector3 location){
		UFE.FireAlert(SetStringValues(UFE.config.selectedLanguage.parry, myInfo), myInfo);

		float stunTime = .4f;
		
		
		if (UFE.config.blockOptions.parrySound != null) 
			if (UFE.config.soundfx) Camera.main.GetComponent<AudioSource>().PlayOneShot(UFE.config.blockOptions.parrySound);
		
		
		// Create hit parry effect
		if (location != Vector3.zero && UFE.config.blockOptions.parryPrefab != null){
			GameObject pTemp = (GameObject) Instantiate(UFE.config.blockOptions.parryPrefab);
			pTemp.transform.position = location;
			pTemp.transform.localScale = new Vector3(mirror, 1, 1);
			Destroy(pTemp, UFE.config.blockOptions.parryKillTime);
		}
		
		// Get correct animation according to stance
		if (currentState == PossibleStates.Crouch){
			 currentHitAnimation = "parryLow";
		}else if (currentState == PossibleStates.Stand){
			currentHitAnimation = "parryHigh";
		}else if (!myPhysicsScript.isGrounded()){
			currentHitAnimation = "parryAir";
		}
		
		character.GetComponent<Animation>().Stop(currentHitAnimation);
		character.GetComponent<Animation>()[currentHitAnimation].speed = (character.GetComponent<Animation>()[currentHitAnimation].length/stunTime) * 1.5f;
		character.GetComponent<Animation>().Play(currentHitAnimation);
		
		// Highlight effect when parry
		if (UFE.config.blockOptions.highlightWhenParry){
			HighlightOn(gameObject, true);
			Invoke("HighlightOff",.2f);
		}
		
		// Freeze screen depending on how strong the hit was
		HitPause();
		Invoke("HitUnpause",GetFreezingTime(hit.hitStrengh));
		
		// Reset hit to allow for another hit while the character is still stunned
		myHitBoxesScript.Invoke("resetHit",GetFreezingTime(hit.hitStrengh) * 1.2f);
		
		// Add force to the move
		myPhysicsScript.resetForces(hit.resetPreviousHorizontalPush, hit.resetPreviousVerticalPush);
		myPhysicsScript.addForce(new Vector3(hit.pushForce.x, 0, 0), opControlsScript.mirror);
		
		potentialParry = 0;
	}

	public void GetHitBlocking(Hit hit, int remainingFrames, Vector3 location){
		// Lose life
		if (hit.damageOnBlock >= myInfo.currentLifePoints){
			GetHit(hit, remainingFrames, location);
			return;
		}else{
			DamageMe(hit.damageOnBlock);
		}

		blockStunned = true;

		int stunFrames = 0;
		if (hit.hitStunType == HitStunType.FrameAdvantage) {
			stunFrames = hit.frameAdvantageOnBlock + remainingFrames;
		}else{
			stunFrames = hit.hitStunOnBlock;
		}

		if (stunFrames < 1) stunFrames = 1;
		stunTime = (float)stunFrames/(float)UFE.config.fps;
		
		if (UFE.config.blockOptions.blockSound != null) 
			if (UFE.config.soundfx) Camera.main.GetComponent<AudioSource>().PlayOneShot(UFE.config.blockOptions.blockSound);
		
		// Create hit blocking effect
		if (location != Vector3.zero && UFE.config.blockOptions.blockPrefab != null){
			GameObject pTemp = (GameObject) Instantiate(UFE.config.blockOptions.blockPrefab);
			pTemp.transform.position = location;
			pTemp.transform.localScale = new Vector3(mirror, 1, 1);
			Destroy(pTemp, UFE.config.blockOptions.blockKillTime);
		}
		
		if (currentState == PossibleStates.Crouch){
			 currentHitAnimation = "blockingLowHit";
		}else if (currentState == PossibleStates.Stand){
			currentHitAnimation = "blockingHighHit";
		}else if (!myPhysicsScript.isGrounded()){
			currentHitAnimation = "blockingAirHit";
		}
		
		character.GetComponent<Animation>().Stop(currentHitAnimation);
		character.GetComponent<Animation>()[currentHitAnimation].speed = (character.GetComponent<Animation>()[currentHitAnimation].length/stunTime) * 1.5f;
		character.GetComponent<Animation>().Play(currentHitAnimation);
		
		// Freeze screen depending on how strong the hit was
		HitPause();
		Invoke("HitUnpause",GetFreezingTime(hit.hitStrengh));
		
		// Reset hit to allow for another hit while the character is still stunned
		myHitBoxesScript.Invoke("resetHit",GetFreezingTime(hit.hitStrengh) * 1.2f);
		
		// Add force to the move
		myPhysicsScript.resetForces(hit.resetPreviousHorizontalPush, hit.resetPreviousVerticalPush);
		myPhysicsScript.addForce(new Vector3(hit.pushForce.x, 0, 0), opControlsScript.mirror);
	}
	
	public void GetHit(Hit hit, int remainingFrames, Vector3 location){
		// Get what animation should be played depending on the character's state
		if (myPhysicsScript.isGrounded()) {
			if (currentState == PossibleStates.Crouch){
				currentHitAnimation = "getHitLow";
			}else if (hit.hitType == HitType.Launcher){
				currentHitAnimation = "getHitAir";
			}else{
				currentHitAnimation = "getHitHigh";
			}
		}else{
			currentHitAnimation = "getHitAir";
		}
		
		// Set position in case of pull enemy in
		if (hit.pullEnemyIn.enemyBodyPart != BodyPart.none && hit.pullEnemyIn.characterBodyPart != BodyPart.none){
			Vector3 newPos = myHitBoxesScript.getPosition(hit.pullEnemyIn.enemyBodyPart);
			if (newPos != Vector3.zero){
				pullInLocation = transform.position + (hit.pullEnemyIn.position.position - newPos);
				pullInSpeed = hit.pullEnemyIn.speed;
			}
		}
		
		// Differenciate hit types
		GameObject hitEffect = null;
		float effectKillTime = 0;
		if (hit.hitStrengh == HitStrengh.Weak) hitEffect = GetHitData(UFE.config.hitOptions.weakHit, ref effectKillTime);
		if (hit.hitStrengh == HitStrengh.Medium) hitEffect = GetHitData(UFE.config.hitOptions.mediumHit, ref effectKillTime);
		if (hit.hitStrengh == HitStrengh.Heavy) hitEffect = GetHitData(UFE.config.hitOptions.heavyHit, ref effectKillTime);
		if (hit.hitStrengh == HitStrengh.Crumple) hitEffect = GetHitData(UFE.config.hitOptions.crumpleHit, ref effectKillTime);
		if (hit.hitStrengh == HitStrengh.Custom1) hitEffect = GetHitData(UFE.config.hitOptions.customHit1, ref effectKillTime);
		if (hit.hitStrengh == HitStrengh.Custom2) hitEffect = GetHitData(UFE.config.hitOptions.customHit2, ref effectKillTime);
		if (hit.hitStrengh == HitStrengh.Custom3) hitEffect = GetHitData(UFE.config.hitOptions.customHit3, ref effectKillTime);

		// Cancel current move if any
		if (!hit.armorBreaker && currentMove != null && currentMove.armor > 0){
			currentMove.armor --;
		}else{
			storedMove = null;
			KillCurrentMove();
		}
		
		// Create hit effect
		if (location != Vector3.zero && hitEffect != null){
			GameObject pTemp = (GameObject) Instantiate(hitEffect, location, Quaternion.identity);
			Destroy(pTemp, effectKillTime);
		}


		// Cast First Hit if true
		if (!firstHit && !opControlsScript.firstHit){
			opControlsScript.firstHit = true;
			UFE.FireAlert(SetStringValues(UFE.config.selectedLanguage.firstHit, opInfo), opInfo);
		}
		UFE.FireHit(myHitBoxesScript.getStrokeHitBox(), opControlsScript.currentMove, opInfo);


		// Convert Percentage
		if (hit.damageType == DamageType.Percentage) hit.damageOnHit = myInfo.lifePoints * (hit.damageOnHit/100);

		// Damage deterioration
		float damage = 0;
		if (!hit.damageScaling || UFE.config.comboOptions.damageDeterioration == Sizes.None){
			damage = hit.damageOnHit;
		}else if (UFE.config.comboOptions.damageDeterioration == Sizes.Small){
			damage = hit.damageOnHit - (hit.damageOnHit * (float)comboHits * .1f);
		}else if (UFE.config.comboOptions.damageDeterioration == Sizes.Medium){
			damage = hit.damageOnHit - (hit.damageOnHit * (float)comboHits * .2f);
		}else if (UFE.config.comboOptions.damageDeterioration == Sizes.High){
			damage = hit.damageOnHit - (hit.damageOnHit * (float)comboHits * .4f);
		}
		if (damage < UFE.config.comboOptions.minDamage) damage = UFE.config.comboOptions.minDamage;
		
		// Lose life
		isDead = DamageMe(damage);
		
		
		// Stun
		// Hit stun deterioration (the longer the combo gets, the harder it is to combo)
		stunned = true;
		
		int stunFrames = 0;
		if (hit.hitStunType == HitStunType.FrameAdvantage) {
			stunFrames = hit.frameAdvantageOnHit + remainingFrames;
		}else{
			stunFrames = hit.hitStunOnHit;
		}

		if (stunFrames < 1) stunFrames = 1;
		if (stunFrames < UFE.config.comboOptions.minHitStun) stunTime = UFE.config.comboOptions.minHitStun;
		stunTime = (float)stunFrames/(float)UFE.config.fps;
		if (!hit.resetPreviousHitStun){
			if (UFE.config.comboOptions.hitStunDeterioration == Sizes.Small){
				stunTime -= (float)comboHits * .01f;
			}else if (UFE.config.comboOptions.hitStunDeterioration == Sizes.Medium){
				stunTime -= (float)comboHits * .02f;
			}else if (UFE.config.comboOptions.hitStunDeterioration == Sizes.High){
				stunTime -= (float)comboHits * .04f;
			}
		}
		comboHits ++;
		if (isDead) stunTime = 999;

		// Set deceleration of hit stun animation so it can look more natural
		hitStunDeceleration = character.GetComponent<Animation>()[currentHitAnimation].length/Mathf.Pow(stunTime,2);
		
		// Stop any previous hit stun and play animation at hit animation speed
		character.GetComponent<Animation>().Stop(currentHitAnimation);
		character.GetComponent<Animation>()[currentHitAnimation].speed = (character.GetComponent<Animation>()[currentHitAnimation].length/stunTime) * 1.5f;
		character.GetComponent<Animation>().Play(currentHitAnimation);
		
		// Freeze screen depending on how strong the hit was
		HitPause();
		Invoke("HitUnpause",GetFreezingTime(hit.hitStrengh));
		
		// Reset hit to allow for another hit while the character is still stunned
		myHitBoxesScript.Invoke("resetHit",GetFreezingTime(hit.hitStrengh) * 1.2f);
		
		// Add force to the move		
		// Air juggle deterioration (the longer the combo, the harder it is to push the opponent higher)
		float verticalPush = hit.pushForce.y;
		if (verticalPush > 0 || isDead  || 
		    hit.hitType == HitType.HardKnockdown ||
		    hit.hitType == HitType.Knockdown){
			if (UFE.config.comboOptions.airJuggleDeterioration == Sizes.None){
				verticalPush = hit.pushForce.y;
			}else if (UFE.config.comboOptions.airJuggleDeterioration == Sizes.Small){
				verticalPush = hit.pushForce.y - (hit.pushForce.y * (float)comboHits * .1f);
			}else if (UFE.config.comboOptions.airJuggleDeterioration == Sizes.Medium){
				verticalPush = hit.pushForce.y - (hit.pushForce.y * (float)comboHits * .2f);
			}else if (UFE.config.comboOptions.airJuggleDeterioration == Sizes.High){
				verticalPush = hit.pushForce.y - (hit.pushForce.y * (float)comboHits * .4f);
			}
			if (verticalPush < UFE.config.comboOptions.minPushForce) verticalPush = UFE.config.comboOptions.minPushForce;
		}

		if (hit.hitType == HitType.Knockdown || hit.hitType == HitType.HardKnockdown) myPhysicsScript.resetForces(true, true);

		myPhysicsScript.resetForces(hit.resetPreviousHorizontalPush, hit.resetPreviousVerticalPush);
		myPhysicsScript.addForce(new Vector2(hit.pushForce.x, verticalPush), opControlsScript.mirror);
	}

	private GameObject GetHitData(HitTypeOptions hitTypeOptions, ref float killTime){
		shakeCamera = hitTypeOptions.shakeCameraOnHit;
		shakeCharacter = hitTypeOptions.shakeCharacterOnHit;
		shakeDensity = hitTypeOptions.shakeDensity;
		
		if (hitTypeOptions.hitSound != null) 
			if (UFE.config.soundfx) Camera.main.GetComponent<AudioSource>().PlayOneShot(hitTypeOptions.hitSound);
		
		killTime = hitTypeOptions.killTime;
		return hitTypeOptions.hitParticle;

	}

	// Pause animations and physics to create a sense of impact
	void HitPause(){
		Camera.main.transform.position += Vector3.forward/2;
		myPhysicsScript.freeze = true;
		PausePlayAnimation(true);
	}
	
	// Unpauses the pause
	void HitUnpause(){
		PausePlayAnimation(false);
		myPhysicsScript.freeze = false;
	}
	
	private bool DamageMe(float damage){
		if (UFE.config.trainingMode) return false;
		if (myInfo.currentLifePoints <= 0 || opInfo.currentLifePoints <= 0) return true;
		myInfo.currentLifePoints -= damage;
		UFE.SetLifePoints(myInfo.currentLifePoints, myInfo);
		if (myInfo.currentLifePoints <= 0 && UFE.config.roundOptions.slowMotionKO){
			opControlsScript.roundsWon ++;
			if (UFE.config.soundfx) Camera.main.GetComponent<AudioSource>().PlayOneShot(myInfo.deathSound);
			
			//storedAnimationFlow = UFE.config.animationFlow;
			//UFE.config.animationFlow = AnimationFlow.Smoother;

			Time.timeScale = Time.timeScale * .2f;
			Invoke("ReturnTimeScale", .4f); // Low timer to account for the slowmotion

			return true;
		}else if (myInfo.currentLifePoints <= 0){
			opControlsScript.roundsWon ++;
			Invoke("EndRound", 3);
			return true;
		}
		return false;
	}
	
	private void AddGauge(float gaugeGain){
		myInfo.currentGaugePoints += gaugeGain;
		if (myInfo.currentGaugePoints > myInfo.maxGaugePoints) myInfo.currentGaugePoints = myInfo.maxGaugePoints;
	}

	private void ReturnTimeScale(){
		Time.timeScale = UFE.config.gameSpeed;
		Invoke("EndRound", 1);

		//UFE.config.animationFlow = storedAnimationFlow;
	}
	
	private void EndRound(){
		UFE.config.lockMovements = true;
		UFE.config.lockInputs = true;

		if (!opPhysicsScript.isGrounded() || !myPhysicsScript.isGrounded()){
			Invoke("EndRound", 1);
			return;
		}

		UFE.FireRoundEnds(opInfo, myInfo);

		if (opControlsScript.roundsWon > Mathf.Ceil(UFE.config.roundOptions.totalRounds/2)){
			opControlsScript.currentMove = myMoveSetScript.getOutro();
			opControlsScript.currentMove.currentFrame = 0;
			opControlsScript.outroPlayed = true;
			Invoke("EndGame", UFE.config.roundOptions.delayBeforeEndGame);
			UFE.FireAlert(SetStringValues(UFE.config.selectedLanguage.victory, opInfo), null);
			
			if (UFE.config.roundOptions.victoryMusic != null) Camera.main.GetComponent<AudioSource>().clip = UFE.config.roundOptions.victoryMusic;
			if (UFE.config.music) Camera.main.GetComponent<AudioSource>().Play();
		}else{
			if (UFE.config.roundOptions.resetPositions) {
				CameraFade.StartAlphaFade(Color.black, false, 1, 2, () => { StartNewRound(); });
			}else{
				Invoke("StartNewRound", 2);
			}
		}
	}
	
	private void EndGame(){
		UFE.FireGameEnds(opInfo, myInfo);
		//PausePlayAnimation(true);
		opControlsScript.PausePlayAnimation(true);
		//myPhysicsScript.freeze = true;
		//opPhysicsScript.freeze = true;
		cameraScript.killCamMove = true;
	}

	public void ResetData(bool resetLife){
		if (UFE.config.roundOptions.resetPositions){
			if (gameObject.name == "Player1"){
				transform.position = new Vector2(UFE.config.roundOptions.p1XPosition, .09f);
			}else{
				transform.position = new Vector2(UFE.config.roundOptions.p2XPosition, .09f);
			}
			myMoveSetScript.playBasicMove(myMoveSetScript.basicMoves.idle, 0);
		}else if (currentState == PossibleStates.Down && myPhysicsScript.isGrounded()){
			character.GetComponent<Animation>().Play("getUp");
		}

		if (resetLife || UFE.config.roundOptions.resetLifePoints){
			myInfo.currentLifePoints = myInfo.lifePoints;
		}

		stunned = false;
		blockStunned = false;
		stunTime = 0;
		isDead = false;

		currentState = PossibleStates.Stand;
	}

	private void StartNewRound(){
		UFE.config.currentRound ++;

		ResetData(true);
		opControlsScript.ResetData(false);
		if (UFE.config.roundOptions.resetPositions) {
			CameraFade.StartAlphaFade(Color.black, true, 2);
			cameraScript.ResetCam();
		}

		UFE.config.lockInputs = true;
		if (gameObject.name == "Player1") {
			roundMsgCasted = false;
		}else{
			opControlsScript.roundMsgCasted = false;
		}
		if (UFE.config.roundOptions.allowMovement) {
			UFE.config.lockMovements = false;
		}else{
			UFE.config.lockMovements = true;
		}
	}

	private string SetStringValues(string msg, CharacterInfo playerInfo){
		msg = msg.Replace("%combo%", comboHits.ToString());
		msg = msg.Replace("%round%", UFE.config.currentRound.ToString());
		msg = msg.Replace("%character%", playerInfo.characterName.ToString());
		return msg;
	}

	// Release character to be playable again
	private void ReleaseStun(){
		if (!stunned && !blockStunned) return;
		if (!isBlocking && comboHits > 1) {
			UFE.FireAlert(SetStringValues(UFE.config.selectedLanguage.combo, opInfo), opInfo);
		}
		stunned = false;
		blockStunned = false;
		stunTime = 0;
		comboHits = 0;
		isBlocking = false;
		myHitBoxesScript.showHitBoxes();
	}
	
	// Method to pause animations and return them to their prior speed accordly
	private void PausePlayAnimation(bool pause){
		if (pause){
			int i = 0;
			foreach(AnimationState animState in character.GetComponent<Animation>()) {
				if (animState.speed != 0.005f) myMoveSetScript.animSpeedStorage[i] = animState.speed;
				animState.speed = 0.005f;
				i ++;
			}
			animationPaused = true;
		}else if (animationPaused) {
			int i = 0;
			foreach(AnimationState animState in character.GetComponent<Animation>()) {
				if (animState.speed == 0.005f) animState.speed = myMoveSetScript.animSpeedStorage[i];
				i ++;
			}
			animationPaused = false;
		}
	}

	public float GetAnimationTime(int animFrame){
		if (currentMove == null) return 0;
		if (currentMove.animationSpeed < 0){
			return (((float)animFrame/(float)UFE.config.fps) * currentMove.animationSpeed) + currentMove.animationClip.length;
		}else{
			return ((float)animFrame/(float)UFE.config.fps) * currentMove.animationSpeed;
		}
	}
	
	// Get amount of freezing time depending on the strenght of the move
	public float GetFreezingTime(HitStrengh hitStrengh){
		if (hitStrengh == HitStrengh.Weak){
			return UFE.config.hitOptions.weakHit.freezingTime;
		} else if (hitStrengh == HitStrengh.Medium){
			return UFE.config.hitOptions.mediumHit.freezingTime;
		}else if (hitStrengh == HitStrengh.Heavy){
			return UFE.config.hitOptions.heavyHit.freezingTime;
		}else if (hitStrengh == HitStrengh.Crumple){
			return UFE.config.hitOptions.crumpleHit.freezingTime;
		}
		return 0;
	}
	
	// Shake character while being hit and in freezing mode
	
	void shakeCam(){
		float rnd = Random.Range(-.1f * shakeDensity,.2f * shakeDensity);
		Camera.main.transform.position += new Vector3(rnd, rnd, rnd);
	}

	void shake(){
		float rnd = Random.Range(-.1f * shakeDensity,.2f * shakeDensity);
		character.transform.localPosition = new Vector3(rnd, 0, 0);
	}
}
