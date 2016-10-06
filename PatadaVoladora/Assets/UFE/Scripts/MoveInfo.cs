using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum ButtonPress {
	Foward,
	Back,
	Up,
	Down,
	Button1,
	Button2,
	Button3,
	Button4,
	Button5,
	Button6,
	Button7,
	Button8,
	Button9,
	Button10,
	Button11,
	Button12
}

public enum InputType {
	HorizontalAxis,
	VerticalAxis,
	Button
}

public enum PossibleStates {
	Stand,
	Crouch,
	StraightJump,
	ForwardJump,
	BackJump,
	Down
}
public enum CombatStances {
	Stance1,
	Stance2,
	Stance3,
	Stance4,
	Stance5,
	Stance6,
	Stance7,
	Stance8,
	Stance9,
	Stance10
}
public enum DamageType {
	Percentage,
	Points
}
public enum AttackType {
	Regular,
	Special,
	EX,
	Super
}
public enum ProjectileType {
	Shot,
	Beam
}
public enum HitType {
	HighLow,
	Low,
	Overhead,
	Launcher,
	Knockdown,
	HardKnockdown,
	Unblockable
}
public enum HitStrengh {
	Weak,
	Medium,
	Heavy,
	Crumple,
	Custom1,
	Custom2,
	Custom3
}
public enum HitStunType {
	FrameAdvantage,
	Frames
}

[System.Serializable]
public class Projectile {
	public int castingFrame = 1;
	public GameObject projectilePrefab;
	public GameObject impactPrefab;
	public BodyPart bodyPart;
	public ProjectileType type;
	public Vector3 offSet;
	public int totalHits = 1;
	public bool projectileCollision;
	
	public bool resetPreviousHitStun = true;
	public int hitStunOnHit;
	public int hitStunOnBlock;
	
	public DamageType damageType;
	public float damageOnHit;
	public float damageOnBlock;
	public bool damageScaling;
	
	public int speed = 20;
	public int directionAngle;
	public float duration;
	
	public Vector2 pushForce;
	public float hitRadius;
	public HitStrengh hitStrengh;
	public HitType hitType;

	[HideInInspector] public bool damageOptionsToggle;
	[HideInInspector] public bool hitStunOptionsToggle;

	[HideInInspector]
	public bool casted;
	[HideInInspector]
	public Transform position;
}

[System.Serializable]
public class InvincibleBodyParts {
	public BodyPart[] bodyParts;
	public bool completelyInvincible = true;
	public int activeFramesBegin;
	public int activeFramesEnds;
	[HideInInspector]
	public HitBox[] hitBoxes;
}


[System.Serializable]
public class AppliedForce {
	public int castingFrame;
	public bool resetPreviousVertical;
	public bool resetPreviousHorizontal;
	public Vector2 force;
	[HideInInspector]
	public bool casted;
}

[System.Serializable]
public class Hit {
	public int activeFramesBegin;
	public int activeFramesEnds;
	public bool armorBreaker;
	public bool continuousHit;
	public bool resetPreviousHitStun;
	public HitStrengh hitStrengh;
	public HitStunType hitStunType;
	public int hitStunOnHit;
	public int hitStunOnBlock;
	public int frameAdvantageOnHit;
	public int frameAdvantageOnBlock;
	public bool damageScaling;
	public DamageType damageType;
	public float damageOnHit;
	public float damageOnBlock;
   	public HitType hitType;
	public bool resetPreviousHorizontalPush;
	public bool resetPreviousVerticalPush;
	public Vector2 pushForce;
	public bool resetPreviousHorizontal;
	public bool resetPreviousVertical;
	public Vector2 appliedForce;
	public HurtBox[] hurtBoxes;
	public PullIn pullEnemyIn;
	public PullIn pullSelfIn;
	
	[HideInInspector] public bool hasHit;

	[HideInInspector] public bool damageOptionsToggle;
	[HideInInspector] public bool hitStunOptionsToggle;
	[HideInInspector] public bool forceOptionsToggle;
	[HideInInspector] public bool pullInToggle;
	[HideInInspector] public bool hurtBoxesToggle;
}

[System.Serializable]
public class HurtBox {
	public BodyPart bodyPart;
	public float radius = .5f;
	public Vector2 offSet;
	[HideInInspector]
	public Transform position;
}

[System.Serializable]
public class BlockArea {
	public int activeFramesBegin;
	public int activeFramesEnds;
	public BodyPart bodyPart;
	public float radius;
	public Vector2 offSet;
	[HideInInspector]
	public Transform position;
}

[System.Serializable]
public class PullIn {
	public int speed = 50;
	public BodyPart characterBodyPart;
	public BodyPart enemyBodyPart;
	[HideInInspector]
	public Transform position;
}

[System.Serializable]
public class FrameLink {
	public bool onlyOnHit;
	public int activeFramesBegins;
	public int activeFramesEnds;
	public MoveInfo[] linkableMoves = new MoveInfo[0];
}

[System.Serializable]
public class MoveParticleEffect {
	public int castingFrame;
	public ParticleInfo particleEffect;
	[HideInInspector]
	public bool casted;
}

[System.Serializable]
public class SoundEffect {
	public int castingFrame;
	public AudioClip sound;
	[HideInInspector]
	public bool casted;
}

[System.Serializable]
public class CameraMovement {
	public Vector3 position;
	public Vector3 rotation;
	public int castingFrame;
	public float duration;
	public float fieldOfView;
	public float camSpeed = 2;
	public bool freezeGame;
	public bool freezeAnimation;
	[HideInInspector]
	public bool casted;
	[HideInInspector]
	public float time;
	
	public float startTime;
}

public class MoveInfo: ScriptableObject {
	public GameObject characterPrefab;
	public string moveName;
	public string description;
	public int fps = 60;
	public bool ignoreGravity;
	public bool useGauge;
	public int armor;
	public int gaugeUsage;
	public int gaugeGainOnMiss;
	public int gaugeGainOnHit;
	public int gaugeGainOnBlock;
	public int opGaugeGainOnBlock;
	public int opGaugeGainOnParry;
	public PossibleStates[] possibleStates = new PossibleStates[0];
	public AttackType attackType;
	public bool cancelMoveWheLanding;
	
	public AnimationClip animationClip;
	public WrapMode wrapMode;
	public float interpolationSpeed = 0;
	public float animationSpeed = 1;
	public int totalFrames = 15;
	public int startUpFrames = 5;
	public int activeFrames = 5;
	public int recoveryFrames = 5;
	
	public bool chargeMove;
	public bool onRelease;
	public ButtonPress[] buttonSequence = new ButtonPress[0];
	public ButtonPress[] buttonExecution = new ButtonPress[0];
	
	public MoveInfo[] previousMoves = new MoveInfo[0];
	public FrameLink frameLink;
	
	public MoveParticleEffect[] particleEffects = new MoveParticleEffect[0];
	public AppliedForce[] appliedForces = new AppliedForce[0];
	
	public SoundEffect[] soundEffects = new SoundEffect[0];
	public CameraMovement[] cameraMovements = new CameraMovement[0];
	
	public Hit[] hits = new Hit[0];
	public BlockArea blockableArea;
	public InvincibleBodyParts[] invincibleBodyParts = new InvincibleBodyParts[0];
	
	public Projectile[] projectiles = new Projectile[0];
	public CombatStances changeCombatStance;
	
	
	[HideInInspector] public bool cancelable;
	[HideInInspector] public bool kill;
	[HideInInspector] public int currentFrame;
}