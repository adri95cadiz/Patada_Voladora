using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class PhysicsData {
	public float moveForwardSpeed = 4f; // How fast this character can move forward
	public float moveBackSpeed = 3.5f; // How fast this character can move backwards
	public bool highMovingFriction = true; // When releasing the horizontal controls character will stop imediatelly
	public float friction = 30f; // Friction used in case of highMovingFriction false. Also used when player is pushed
	public float jumpForce = 20f; // How high this character will jumps
	public float jumpDistance = 8f; // How far this character will move horizontally while jumping
	public bool cumulativeForce = true; // If this character is being juggled, should new forces add to or replace existing force?
	public int multiJumps = 1; // Can this character double or triple jump? Set how many times the character can jump here
	public float weight;
}

[System.Serializable]
public class MoveSetData {
	public CombatStances combatStance = CombatStances.Stance1; // This move set combat stance
	public MoveInfo cinematicIntro;
	public MoveInfo cinematicOutro;

	public BasicMoves basicMoves = new BasicMoves(); // List of basic moves
	public MoveInfo[] attackMoves = new MoveInfo[0]; // List of attack moves
	
	[HideInInspector] public bool attackMovesToggle;
	[HideInInspector] public bool basicMovesToggle;
}

public class CharacterInfo: ScriptableObject {
	public Texture2D profilePictureSmall;
	public Texture2D profilePictureBig;
	public string characterName;
	public Gender gender;
	public string characterDescription;
	public Color alternativeColor;
	public AudioClip deathSound;
	public float height;
	public int age;
	public string bloodType;
	public int lifePoints = 1000;
	public int maxGaugePoints;
	public GameObject characterPrefab; // The prefab representing the character (must have hitBoxScript attached to it)

	public PhysicsData physics;
	
	public float executionTiming = .3f; // How fast the player needs to press each key during the execution of a special move
	public float chargeTiming = 1.6f; // How long the player needs to hold down a key before charged is set to TRUE in a move
	public int possibleAirMoves = 1; // How many moves this character can perform while in the air
	public float interpolationSpeed = .1f; // The speed of transiction between basic moves
	public MoveSetData[] moves = new MoveSetData[0];

	[HideInInspector] public CombatStances currentCombatStance;
	[HideInInspector] public float currentLifePoints;
	[HideInInspector] public float currentGaugePoints;
}