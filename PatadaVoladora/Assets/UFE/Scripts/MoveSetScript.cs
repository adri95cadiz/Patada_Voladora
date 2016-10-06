using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class BasicMoveInfo {
	public AnimationClip animationClip;
	public float animationSpeed = 1;
	public bool autoSpeed;
	public AudioClip soundEffect;
	public bool continuousSound;
	public ParticleInfo particleEffect = new ParticleInfo();
	
	[HideInInspector]
	public string name;
	[HideInInspector]
	public bool editorToggle;
}


[System.Serializable]
public class ParticleInfo {
	public bool editorToggle;
	public GameObject prefab;
	public float duration = 1;
	public Vector3 position;
}

[System.Serializable]
public class BasicMoves {
	public BasicMoveInfo intro = new BasicMoveInfo();
	public BasicMoveInfo outro = new BasicMoveInfo();
	public BasicMoveInfo idle = new BasicMoveInfo();
	public BasicMoveInfo moveForward = new BasicMoveInfo();
	public BasicMoveInfo moveBack = new BasicMoveInfo();
	public BasicMoveInfo jumping = new BasicMoveInfo();
	public BasicMoveInfo falling = new BasicMoveInfo();
	public BasicMoveInfo landing = new BasicMoveInfo();
	public BasicMoveInfo crouching = new BasicMoveInfo();
	public BasicMoveInfo blockingLowPose = new BasicMoveInfo();
	public BasicMoveInfo blockingLowHit = new BasicMoveInfo();
	public BasicMoveInfo blockingHighPose = new BasicMoveInfo();
	public BasicMoveInfo blockingHighHit = new BasicMoveInfo();
	public BasicMoveInfo blockingAirPose = new BasicMoveInfo();
	public BasicMoveInfo blockingAirHit = new BasicMoveInfo();
	public BasicMoveInfo parryCrouching = new BasicMoveInfo();
	public BasicMoveInfo parryHigh = new BasicMoveInfo();
	public BasicMoveInfo parryAir = new BasicMoveInfo();
	public BasicMoveInfo getHitCrouching = new BasicMoveInfo();
	public BasicMoveInfo getHitHigh = new BasicMoveInfo();
	public BasicMoveInfo getHitAir = new BasicMoveInfo();
	public BasicMoveInfo fallDown = new BasicMoveInfo();
	public BasicMoveInfo getUp = new BasicMoveInfo();
	public BasicMoveInfo bounce = new BasicMoveInfo();
	public BasicMoveInfo fallingFromBounce = new BasicMoveInfo();
}

public class MoveSetScript : MonoBehaviour {
	[HideInInspector]
	public BasicMoves basicMoves;
	[HideInInspector]
	public MoveInfo[] attackMoves;
	[HideInInspector]
	public MoveInfo[] moves;
	[HideInInspector]
	public float[] animSpeedStorage;
	[HideInInspector]
	public int totalAirMoves;
	
	private ControlsScript controlsScript;
	private List<ButtonPress> lastButtonPresses = new List<ButtonPress>();
	private float lastTimePress;
	private bool charged;

	private MoveInfo intro;
	private MoveInfo outro;
	
	void Awake(){
		controlsScript = transform.parent.gameObject.GetComponent<ControlsScript>();
		controlsScript.myInfo.currentCombatStance = CombatStances.Stance10;
		changeMoveStances(CombatStances.Stance1);
	}
	
	public void changeMoveStances(CombatStances newStance){
		if (controlsScript.myInfo.currentCombatStance == newStance) return;
		foreach(MoveSetData moveSetData in controlsScript.myInfo.moves){
			if (moveSetData.combatStance == newStance){
				basicMoves = moveSetData.basicMoves;
				attackMoves = moveSetData.attackMoves;
				//moves = moveSetData.attackMoves;
				
				GameObject opponent = GameObject.Find(gameObject.name);
				if (!opponent.Equals(gameObject) && opponent.name == gameObject.name){ // Mirror Match Move cloning
					List<MoveInfo> moveList = new List<MoveInfo>();
					bool alreadyCloned = false;
					foreach(MoveInfo move in attackMoves){
						if (move.name.IndexOf("(Clone)") != -1) {
							alreadyCloned = true;
							break;
						}
						moveList.Add(Instantiate(move) as MoveInfo);
					}
					if (alreadyCloned){
						moves = attackMoves;
					}else{
						moves = moveList.ToArray();
					}
				}else{
					moves = attackMoves;
				}
				
				fillMoves();

				if (moveSetData.cinematicIntro != null) {
					intro = Instantiate(moveSetData.cinematicIntro) as MoveInfo;
					attachAnimation(intro.animationClip, intro.name, intro.animationSpeed, intro.wrapMode);
				}
				if (moveSetData.cinematicOutro != null) {
					outro = Instantiate(moveSetData.cinematicOutro) as MoveInfo;
					attachAnimation(outro.animationClip, outro.name, outro.animationSpeed, outro.wrapMode);
				}

				controlsScript.myInfo.currentCombatStance = newStance;
				
				System.Array.Sort(moves, delegate(MoveInfo move1, MoveInfo move2) {
					return move1.buttonSequence.Length.CompareTo(move2.buttonSequence.Length);
				});
				System.Array.Reverse(moves);
				
				return;
			}
		}
	}
	
	private void fillMoves(){
		DestroyImmediate(gameObject.GetComponent(typeof(Animation)));
		gameObject.AddComponent(typeof(Animation));

        GetComponent<Animation>().wrapMode = WrapMode.Once;
		foreach(MoveInfo move in moves) {
			if (move.animationClip != null) {
				attachAnimation(move.animationClip, move.name, move.animationSpeed, move.wrapMode);
				//animation.AddClip(move.animationClip, move.name);
				//animation[move.name].wrapMode = move.wrapMode;
				//animation[move.name].speed = move.animationSpeed;
			}
		}
		setBasicMoveAnimation(basicMoves.idle, "idle", WrapMode.Loop);
		setBasicMoveAnimation(basicMoves.moveForward, "moveForward", WrapMode.Loop);
		setBasicMoveAnimation(basicMoves.moveBack, "moveBack", WrapMode.Loop);
		setBasicMoveAnimation(basicMoves.jumping, "jumping", WrapMode.ClampForever);
		setBasicMoveAnimation(basicMoves.falling, "falling", WrapMode.ClampForever);
		setBasicMoveAnimation(basicMoves.landing, "landing", WrapMode.Once);
		setBasicMoveAnimation(basicMoves.crouching, "crouching", WrapMode.Loop);
		setBasicMoveAnimation(basicMoves.blockingLowPose, "blockingLowPose", WrapMode.ClampForever);
		setBasicMoveAnimation(basicMoves.blockingHighPose, "blockingHighPose", WrapMode.ClampForever);
		setBasicMoveAnimation(basicMoves.blockingAirPose, "blockingAirPose", WrapMode.ClampForever);
		setBasicMoveAnimation(basicMoves.blockingLowHit, "blockingLowHit", WrapMode.Once);
		setBasicMoveAnimation(basicMoves.blockingHighHit, "blockingHighHit", WrapMode.Once);
		setBasicMoveAnimation(basicMoves.blockingAirHit, "blockingAirHit", WrapMode.Once);
		setBasicMoveAnimation(basicMoves.parryHigh, "parryHigh", WrapMode.Once);
		setBasicMoveAnimation(basicMoves.parryCrouching, "parryLow", WrapMode.Once);
		setBasicMoveAnimation(basicMoves.parryAir, "parryAir", WrapMode.Once);
		setBasicMoveAnimation(basicMoves.getHitCrouching, "getHitLow", WrapMode.Once);
		setBasicMoveAnimation(basicMoves.getHitHigh, "getHitHigh", WrapMode.Once);
		setBasicMoveAnimation(basicMoves.getHitAir, "getHitAir", WrapMode.ClampForever);
		setBasicMoveAnimation(basicMoves.fallDown, "fallDown", WrapMode.ClampForever);
		setBasicMoveAnimation(basicMoves.bounce, "bounce", WrapMode.ClampForever);
		setBasicMoveAnimation(basicMoves.fallingFromBounce, "fallingFromBounce", WrapMode.ClampForever);
		setBasicMoveAnimation(basicMoves.getUp, "getUp", WrapMode.Once);
		if (GetComponent<Animation>()["getUp"] != null) GetComponent<Animation>()["getUp"].speed = GetComponent<Animation>()["getUp"].length/UFE.config.knockDownOptions.getUpTime;
		
		animSpeedStorage = new float[GetComponent<Animation>().GetClipCount() + 2];
	}
	
	private void setBasicMoveAnimation(BasicMoveInfo basicMove, string animName, WrapMode wrapMode){
		if (basicMove.animationClip == null) return;
		basicMove.name = animName;
		//if (animation[animName] != null) animation.RemoveClip(animName);
		attachAnimation(basicMove.animationClip, animName, basicMove.animationSpeed, wrapMode);
	}

	private void attachAnimation(AnimationClip clip, string animName, float speed, WrapMode wrapMode){
        GetComponent<Animation>().AddClip(clip, animName);
        GetComponent<Animation>()[animName].speed = speed;
        GetComponent<Animation>()[animName].wrapMode = wrapMode;
	}
	
	public bool compareBlockButtons(ButtonPress button){
		if (button == ButtonPress.Button1 && UFE.config.blockOptions.blockType == BlockType.HoldButton1) return true;
		if (button == ButtonPress.Button2 && UFE.config.blockOptions.blockType == BlockType.HoldButton2) return true;
		if (button == ButtonPress.Button3 && UFE.config.blockOptions.blockType == BlockType.HoldButton3) return true;
		if (button == ButtonPress.Button4 && UFE.config.blockOptions.blockType == BlockType.HoldButton4) return true;
		if (button == ButtonPress.Button5 && UFE.config.blockOptions.blockType == BlockType.HoldButton5) return true;
		if (button == ButtonPress.Button6 && UFE.config.blockOptions.blockType == BlockType.HoldButton6) return true;
		if (button == ButtonPress.Button7 && UFE.config.blockOptions.blockType == BlockType.HoldButton7) return true;
		if (button == ButtonPress.Button8 && UFE.config.blockOptions.blockType == BlockType.HoldButton8) return true;
		if (button == ButtonPress.Button9 && UFE.config.blockOptions.blockType == BlockType.HoldButton9) return true;
		if (button == ButtonPress.Button10 && UFE.config.blockOptions.blockType == BlockType.HoldButton10) return true;
		if (button == ButtonPress.Button11 && UFE.config.blockOptions.blockType == BlockType.HoldButton11) return true;
		if (button == ButtonPress.Button12 && UFE.config.blockOptions.blockType == BlockType.HoldButton12) return true;
		//if (GlobalScript.prefs.blockOptions.blockType == BlockType.AutoBlock) return true;
		return false;
	}
	
	public bool compareParryButtons(ButtonPress button){
		if (button == ButtonPress.Button1 && UFE.config.blockOptions.parryType == ParryType.TapButton1) return true;
		if (button == ButtonPress.Button2 && UFE.config.blockOptions.parryType == ParryType.TapButton2) return true;
		if (button == ButtonPress.Button3 && UFE.config.blockOptions.parryType == ParryType.TapButton3) return true;
		if (button == ButtonPress.Button4 && UFE.config.blockOptions.parryType == ParryType.TapButton4) return true;
		if (button == ButtonPress.Button5 && UFE.config.blockOptions.parryType == ParryType.TapButton5) return true;
		if (button == ButtonPress.Button6 && UFE.config.blockOptions.parryType == ParryType.TapButton6) return true;
		if (button == ButtonPress.Button7 && UFE.config.blockOptions.parryType == ParryType.TapButton7) return true;
		if (button == ButtonPress.Button8 && UFE.config.blockOptions.parryType == ParryType.TapButton8) return true;
		if (button == ButtonPress.Button9 && UFE.config.blockOptions.parryType == ParryType.TapButton9) return true;
		if (button == ButtonPress.Button10 && UFE.config.blockOptions.parryType == ParryType.TapButton10) return true;
		if (button == ButtonPress.Button11 && UFE.config.blockOptions.parryType == ParryType.TapButton11) return true;
		if (button == ButtonPress.Button12 && UFE.config.blockOptions.parryType == ParryType.TapButton12) return true;
		return false;
	}
	
	public void playBasicMove(BasicMoveInfo basicMove){
		if (GetComponent<Animation>().IsPlaying(basicMove.name) || controlsScript.isBlocking) return;
        GetComponent<Animation>().CrossFade(basicMove.name, controlsScript.myInfo.interpolationSpeed);

		_playBasicMove(basicMove);
	}
	
	public void playBasicMove(BasicMoveInfo basicMove, float interpolationSpeed){
		if (GetComponent<Animation>().IsPlaying(basicMove.name) || controlsScript.isBlocking) return;
        GetComponent<Animation>().CrossFade(basicMove.name, interpolationSpeed);

		_playBasicMove(basicMove);

	}

	private void _playBasicMove(BasicMoveInfo basicMove){
		if (basicMove.soundEffect != null && UFE.config.soundfx) Camera.main.GetComponent<AudioSource>().PlayOneShot(basicMove.soundEffect);
		
		if (basicMove.particleEffect.prefab != null) {
			GameObject pTemp = (GameObject) Instantiate(basicMove.particleEffect.prefab);
			pTemp.transform.parent = transform;
			pTemp.transform.localPosition = basicMove.particleEffect.position;
			Destroy(pTemp, basicMove.particleEffect.duration);
		}
	}
	
	private bool HasEnoughGauge(float gaugeNeeded){
		if (controlsScript.myInfo.currentGaugePoints < gaugeNeeded) return false;
		return true;
	}
	
	private void RemoveGauge(float gaugeLoss){
		controlsScript.myInfo.currentGaugePoints -= gaugeLoss;
		if (controlsScript.myInfo.currentGaugePoints < 0) controlsScript.myInfo.currentGaugePoints = 0;
	}

	public bool searchMove(string moveName, MoveInfo[] moves){
		if (moveName.IndexOf("(Clone)") != -1)
			moveName = moveName.Substring(0, moveName.Length - 7);

		foreach(MoveInfo move in moves)
			if (moveName == move.name) return true;

		return false;
	}

	public MoveInfo getIntro(){
		return intro;
	}
	
	public MoveInfo getOutro(){
		return outro;
	}

	public MoveInfo getMove(string moveName){
		foreach(MoveInfo i in moves) {
			if (i.name == moveName) {
				MoveInfo newMove = Instantiate(i) as MoveInfo;
				newMove.name = newMove.name.Substring(0, newMove.name.Length - 7);
				return newMove;
			}
		}
		return null;
	}
	
	public MoveInfo getMove(ButtonPress[] buttonPress, float charge, MoveInfo currentMove, bool inputUp){
		if (buttonPress.Length > 0 && Time.time - lastTimePress <= controlsScript.myInfo.executionTiming) {
			foreach(MoveInfo move in moves) {
				MoveInfo newMove = TestMoveExecution(move, currentMove, buttonPress, inputUp, true);
				if (newMove != null) return newMove;
			}
			//Debug.Log("Adding "+ buttonPress[0] +" to queue...");
		}else if (buttonPress.Length > 0) {
			charged = false;
			lastButtonPresses.Clear();
			
			if (charge >= controlsScript.myInfo.chargeTiming) charged = true;
		}
		
		lastTimePress = Time.time;
		lastButtonPresses.Add(buttonPress[0]);
		foreach(MoveInfo move in moves) {
			MoveInfo newMove = TestMoveExecution(move, currentMove, buttonPress, inputUp, false);
			if (newMove != null) return newMove;
		}
		
		return null;
	}

	private MoveInfo TestMoveExecution(MoveInfo move, MoveInfo currentMove, ButtonPress[] buttonPress, bool inputUp, bool fromSequence) {
		if (move.onRelease && !inputUp) return null;
		if (!HasEnoughGauge(move.gaugeUsage)) return null;
		if (move.previousMoves.Length > 0 && currentMove == null) return null;
		if (move.previousMoves.Length > 0 && !searchMove(currentMove.name, move.previousMoves)) return null;
		if (Array.IndexOf(move.possibleStates, controlsScript.currentState) == -1) return null;
		
		Array.Sort(buttonPress);
		Array.Sort(move.buttonExecution);

		if (fromSequence){
			if (move.buttonSequence.Length == 0) return null;
			if (move.chargeMove && !charged) return null;
			
			/*string allbp = "";
			foreach(ButtonPress bp in lastButtonPresses) allbp += bp.ToString();
			string allbp2 = "";
			foreach(ButtonPress bp in move.buttonSequence) allbp2 += bp.ToString();
			Debug.Log(allbp +"="+ allbp2 +"? "+ ArraysEqual<ButtonPress>(buttonPress, move.buttonExecution));*/

			if (lastButtonPresses.Count >= move.buttonSequence.Length){
				ButtonPress[] compareSequence = lastButtonPresses.GetRange(
					lastButtonPresses.Count - move.buttonSequence.Length, move.buttonSequence.Length).ToArray();
				
				if (!ArraysEqual<ButtonPress>(compareSequence, move.buttonSequence)) return null;
			}else{
				return null;
			}
			
			//Debug.Log("Sequence pass! Testing Execution:"+ ArraysEqual<ButtonPress>(buttonPress, move.buttonExecution));
		}else{
			if (move.buttonSequence.Length > 0) return null;
		}

		if (!ArraysEqual<ButtonPress>(buttonPress, move.buttonExecution)) return null;

		if (controlsScript.storedMove != null && 
		    move.moveName != currentMove.moveName &&
		    move.moveName == controlsScript.storedMove.moveName) {
			MoveInfo newMove = Instantiate(move) as MoveInfo;
			newMove.name = move.name;
			return newMove;
		}

		if (controlsScript.currentState == PossibleStates.StraightJump ||
		    controlsScript.currentState == PossibleStates.ForwardJump ||
		    controlsScript.currentState == PossibleStates.BackJump){
			if (totalAirMoves >= controlsScript.myInfo.possibleAirMoves) return null;
			totalAirMoves ++;
		}

		if (currentMove == null || searchMove(move.name, currentMove.frameLink.linkableMoves)){
			changeMoveStances(move.changeCombatStance);
			MoveInfo newMove = Instantiate(move) as MoveInfo;
			newMove.name = move.name;
			UFE.FireMove(newMove, controlsScript.myInfo);
			RemoveGauge(move.gaugeUsage);
			//Debug.Log("move "+ newMove.name +" executed.");
			return newMove;
		}

		return null;
	}
	
	private bool ArraysEqual<T>(T[] a1, T[] a2) {
    	if (ReferenceEquals(a1,a2)) return true;
  		if (a1 == null || a2 == null) return false;
		if (a1.Length != a2.Length) return false;
	    EqualityComparer<T> comparer = EqualityComparer<T>.Default;
		for (int i = 0; i < a1.Length; i++){
        	if (!comparer.Equals(a1[i], a2[i])) return false;
    	}
    	return true;
	}
}
