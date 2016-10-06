using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum AnimationFlow{
	MorePrecision,
	Smoother
}

public enum BlockType{
	None,
	HoldBack,
	AutoBlock,
	HoldButton1,
	HoldButton2,
	HoldButton3,
	HoldButton4,
	HoldButton5,
	HoldButton6,
	HoldButton7,
	HoldButton8,
	HoldButton9,
	HoldButton10,
	HoldButton11,
	HoldButton12
}

public enum BodyPart {
	none,
	head,
	upperTorso,
	lowerTorso,
	leftUpperArm,
	rightUpperArm,
	leftForearm,
	rightForearm,
	leftHand,
	rightHand,
	leftThigh,
	rightThigh,
	leftCalf,
	rightCalf,
	leftFoot,
	rightFoot,
	custom1,
	custom2,
	custom3,
	custom4,
	custom5,
	custom6,
	custom7,
	custom8,
	custom9
}

public enum CollisionType {
	bodyCollider,
	hitCollider,
	noCollider
}

public enum FontId {
	Font1,
	Font2,
	Font3,
	Font4,
	Font5,
	Font6,
	Font7,
	Font8,
	Font9,
	Font10
}

public enum Gender {
	Unknown,
	Male,
	Female
}

public enum HitBoxType {
	high,
	mid,
	low
}

public enum ParryType{
	None,
	TapBack,
	TapForward,
	TapButton1,
	TapButton2,
	TapButton3,
	TapButton4,
	TapButton5,
	TapButton6,
	TapButton7,
	TapButton8,
	TapButton9,
	TapButton10,
	TapButton11,
	TapButton12
}

public enum Player { 
	Player1,
	Player2
}

public enum Side {
	Left,
	Right
}

public enum Sizes{
	None,
	Small,
	Medium,
	High
}

[System.Serializable]
public class CameraOptions {
	public Vector3 initialDistance;
	public Vector3 initialRotation;
	public float initialFieldOfView;
	public float smooth = 20;
	public float minZoom = 38f;
	public float maxZoom = 54f;
}

[System.Serializable]
public class ComboOptions {
	public Sizes hitStunDeterioration;
	public Sizes damageDeterioration;
	public Sizes airJuggleDeterioration;
	public float minHitStun = 1;
	public float minDamage = 5;
	public float minPushForce = 5;
	public bool neverAirRecover = false;
	public int maxCombo = 99;
}

[System.Serializable]
public class BounceOptions {
	public Sizes bounceForce;
	public GameObject bouncePrefab;
	public float minimumBounceForce = 0;
	public float maximumBounces = 0;
	public bool bounceHitBoxes;
}

[System.Serializable]
public class BlockOptions {
	public BlockType blockType;
	public GameObject blockPrefab;
	public float blockKillTime;
	public AudioClip blockSound;
	public bool allowAirBlock;
	public ParryType parryType;
	public float parryTiming;
	public GameObject parryPrefab;
	public float parryKillTime;
	public AudioClip parrySound;
	public Color parryColor;
	public bool allowAirParry;
	public bool highlightWhenParry;
	public Sizes blockPushForce; // NOT DONE
	public ButtonPress[] pushBlockButtons; // NOT DONE
}

[System.Serializable]
public class KnockDownOptions {
	public float knockedOutTime = 2;
	public float getUpTime = .6f;
	public bool knockedOutHitBoxes;
	public ButtonPress[] quickStandButtons;
	public float minQuickStandTime;
	public ButtonPress[] delayedStandButtons = new ButtonPress[0];
	public float maxDelayedStandTime;
}

[System.Serializable]
public class HitTypeOptions {
	public GameObject hitParticle;
	public float killTime;
	public AudioClip hitSound;
	public float freezingTime;
	public bool shakeCharacterOnHit = true;
	public bool shakeCameraOnHit = true;
	public float shakeDensity = .8f;
	public bool editorToggle;
}

[System.Serializable]
public class HitOptions {
	public HitTypeOptions weakHit;
	public HitTypeOptions mediumHit;
	public HitTypeOptions heavyHit;
	public HitTypeOptions crumpleHit;
	public HitTypeOptions customHit1;
	public HitTypeOptions customHit2;
	public HitTypeOptions customHit3;
}

[System.Serializable]
public class StageOptions {
	public string stageName;
	public Texture2D screenshot;
	public GameObject prefab;
	public AudioClip music;
	public float leftBoundary = -38;
	public float rightBoundary = 38;
}

[System.Serializable]
public class GUIOptions {
	public FontId alertFont;
	public FontId characterNameFont;
	public FontId menuFontBig;
	public FontId menuFontSmall;
	public GUIBarOptions lifeBarOptions1;
	public GUIBarOptions gaugeBarOptions1;
	public GUIBarOptions lifeBarOptions2;
	public GUIBarOptions gaugeBarOptions2;
}

[System.Serializable]
public class LanguageOptions {
	public string languageName = "English";
	public string start = "Start";
	public string options = "Options";
	public string credits = "Credits";
	public string selectYourCharacter = "Select Your Character";
	public string selectYourStage = "Select Your Stage";
	public string round = "Round %round%";
	public string finalRound = "Final Round";
	public string fight = "Fight!";
	public string firstHit = "First Hit!";
	public string combo = "%number% hit combo!";
	public string parry = "Parry!";
	public string victory = "%character% wins!";
	public string perfect = "Perfect!";
	public string rematch = "Rematch";
	public string quit = "Quit";
}

[System.Serializable]
public class GUIBarOptions {
	public bool editorToggle;
	public bool previewToggle;
	public bool flip;
	public Texture2D backgroundImage;
	public Color backgroundColor;
	public Texture2D fillImage;
	public Color fillColor;
	public Rect backgroundRect;
	public Rect fillRect;
	public GameObject bgPreview;
	public GameObject fillPreview;

}

[System.Serializable]
public class FontOptions {
	public FontId fontId;
	public GameObject fontPrefab;
}

[System.Serializable]
public class RoundOptions {
	public int totalRounds = 3;
	public float timer = 99;
	public float p1XPosition = -5;
	public float p2XPosition = 5;
	public float delayBeforeEndGame = 4;
	public AudioClip victoryMusic;
	public bool resetLifePoints = true;
	public bool resetPositions = true;
	public bool allowMovement = true;
	public bool slowMotionKO = true;
	public bool cameraZoomKO = true;
	public bool freezeCamAfterOutro = true;
}

public class GlobalInfo: ScriptableObject {
	public LanguageOptions selectedLanguage;
	public CharacterInfo player1Character;
	public CharacterInfo player2Character;
	public CharacterInfo p1CharStorage;
	public CharacterInfo p2CharStorage;
	public StageOptions selectedStage;
	public int currentRound;
	public bool lockInputs;
	public bool lockMovements;
	public bool trainingMode; // NOT DONE
	public bool startGameImmediately;


	public string gameName;
	public GameObject introScreen;
	public AudioClip introMusic;
	public GameObject characterSelectScreen;
	public AudioClip characterSelectMusic;
	public GameObject stageSelectScreen;
	public AudioClip stageSelectMusic;
	public GameObject optionsScreen;
	public GameObject creditsScreen;
	public AudioClip creditsMusic;

	public int fps = 60;
	public float gameSpeed = 1;
	public AnimationFlow animationFlow;
	public float storedExecutionDelay = .3f;
	public float plinkingDelay = .1f;

	public float gravity = .37f;
	public bool detect3D_Hits;
	public FontOptions[] fontOptions = new FontOptions[0];
	public LanguageOptions[] languages = new LanguageOptions[]{new LanguageOptions()};
	public CameraOptions cameraOptions;
	public RoundOptions roundOptions;
	public BounceOptions bounceOptions;
	public ComboOptions comboOptions;
	public BlockOptions blockOptions; // NOT DONE
	public KnockDownOptions knockDownOptions; // NOT DONE
	public HitOptions hitOptions;
	public GUIOptions guiOptions;
	
	public InputReferences[] player1_Inputs = new InputReferences[0]; // Reference to Unity's InputManager to UFE's keys
	public InputReferences[] player2_Inputs = new InputReferences[0]; // Reference to Unity's InputManager to UFE's keys
	
	public StageOptions[] stages = new StageOptions[0];
	public CharacterInfo[] characters = new CharacterInfo[0];


	public bool music = true;
	public bool soundfx = true;
}