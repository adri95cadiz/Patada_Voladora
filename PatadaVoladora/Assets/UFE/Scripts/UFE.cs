using UnityEngine;
using System.Collections;

public class UFE : MonoBehaviour {
	public GlobalInfo UFE_Config;

	public delegate void MeterHandler(float newFloat, CharacterInfo player);
	public static event MeterHandler OnLifePointsChange;

	public delegate void IntHandler(int newInt);
	public static event IntHandler OnRoundBegins;

	public delegate void StringHandler(string newString, CharacterInfo player);
	public static event StringHandler OnNewAlert;
	
	public delegate void HitHandler(HitBox strokeHitBox, MoveInfo move, CharacterInfo player);
	public static event HitHandler OnHit;

	public delegate void MoveHandler(MoveInfo move, CharacterInfo player);
	public static event MoveHandler OnMove;
	
	public delegate void GameBeginHandler(CharacterInfo player1, CharacterInfo player2, StageOptions stage);
	public static event GameBeginHandler OnGameBegin;

	
	public delegate void GameEndsHandler(CharacterInfo winner, CharacterInfo loser);
	public static event GameEndsHandler OnGameEnds;
	public static event GameEndsHandler OnRoundEnds;

	public static GlobalInfo config;
	public static bool freeCamera;
	public static bool normalizedCam = true;
	public static GUIText debugger1;
	public static GUIText debugger2;

	private static GameObject currentScreen;
	
	//private int uiLayer;
	//private int uiMask;

	public static void StartIntro(float fadeTime){
		CameraFade.StartAlphaFade(Color.black, false, fadeTime/2, 0, () => { _StartIntro(fadeTime/2); });
	}
	public static GameObject GetIntro(){
		return config.introScreen;
	}
	private static void _StartIntro(float fadeTime){
		if (config.introMusic != null) Camera.main.GetComponent<AudioSource>().clip = config.introMusic;
		if (config.music && !Camera.main.GetComponent<AudioSource>().isPlaying) Camera.main.GetComponent<AudioSource>().Play();

		CameraFade.StartAlphaFade(Color.black, true, fadeTime);
		Destroy(currentScreen);
		if (config.introScreen == null) 
			Debug.LogError("Intro screen not found! Make sure you have set the prefab correctly in the Global Editor");
		currentScreen = (GameObject) Instantiate(config.introScreen);
	}


	public static void StartCharacterSelect(float fadeTime){
		CameraFade.StartAlphaFade(Color.black, false, fadeTime/2, 0, () => { _StartCharacterSelect(fadeTime/2); });
	}
	private static void _StartCharacterSelect(float fadeTime){
		if (config.characterSelectMusic != null) Camera.main.GetComponent<AudioSource>().clip = config.characterSelectMusic;
		if (config.music && !Camera.main.GetComponent<AudioSource>().isPlaying) Camera.main.GetComponent<AudioSource>().Play();

		CameraFade.StartAlphaFade(Color.black, true, fadeTime);
		Destroy(currentScreen);
		if (config.characterSelectScreen == null) 
			Debug.LogError("Character select screen not found! Make sure you have set the prefab correctly in the Global Editor");
		currentScreen = (GameObject) Instantiate(config.characterSelectScreen);
	}
	public static GameObject GetCharacterSelect(){
		return config.characterSelectScreen;
	}


	public static void StartStageSelect(float fadeTime){
		CameraFade.StartAlphaFade(Color.black, false, fadeTime/2, 0, () => { _StartStageSelect(fadeTime/2); });
	}
	private static void _StartStageSelect(float fadeTime){
		if (config.stageSelectMusic != null) Camera.main.GetComponent<AudioSource>().clip = config.stageSelectMusic;
		if (config.music && !Camera.main.GetComponent<AudioSource>().isPlaying) Camera.main.GetComponent<AudioSource>().Play();

		CameraFade.StartAlphaFade(Color.black, true, fadeTime);
		Destroy(currentScreen);
		if (config.stageSelectScreen == null) 
			Debug.LogError("Stage select screen not found! Make sure you have set the prefab correctly in the Global Editor");
		currentScreen = (GameObject) Instantiate(config.stageSelectScreen);
	}
	public static GameObject GetStageSelect(){
		return config.stageSelectScreen;
	}

	
	public static void StartCreditsScreen(float fadeTime){
		CameraFade.StartAlphaFade(Color.black, false, fadeTime/2, 0, () => { _StartCreditsScreen(fadeTime/2); });
	}
	private static void _StartCreditsScreen(float fadeTime){
		if (config.creditsMusic != null) Camera.main.GetComponent<AudioSource>().clip = config.creditsMusic;
		if (config.music && !Camera.main.GetComponent<AudioSource>().isPlaying) Camera.main.GetComponent<AudioSource>().Play();

		CameraFade.StartAlphaFade(Color.black, true, fadeTime);
		Destroy(currentScreen);
		if (config.creditsScreen == null) 
			Debug.LogError("Credits screen not found! Make sure you have set the prefab correctly in the Global Editor");
		currentScreen = (GameObject) Instantiate(config.creditsScreen);
	}
	public static GameObject GetStageCreditsScreen(){
		return config.creditsScreen;
	}

	
	public static void StartGame(float fadeTime){
		CameraFade.StartAlphaFade(Color.black, false, fadeTime/2, 0, () => { _StartGame(fadeTime/2); });
	}
	public static void _StartGame(float fadeTime){
		CameraFade.StartAlphaFade(Color.black, true, fadeTime);
		Destroy(currentScreen);
		
		currentScreen = new GameObject("Game");
		//DestroyImmediate(currentScreen.GetComponent("CameraScript"));
		currentScreen.AddComponent<CameraScript>();
		
		if (config.player1Character == null){
			Debug.LogError("No character selected for player 1.");
			return;
		}
		if (config.player2Character == null){
			Debug.LogError("No character selected for player 2.");
			return;
		}
		if (config.selectedStage == null){
			Debug.LogError("No stage selected.");
			return;
		}
		config.player1Character.currentLifePoints = config.player1Character.lifePoints;
		config.player2Character.currentLifePoints = config.player2Character.lifePoints;
		
		if (config.selectedStage.prefab == null) 
			Debug.LogError("Stage prefab not found! Make sure you have set the prefab correctly in the Global Editor");
		GameObject stageInstance = (GameObject) Instantiate(config.selectedStage.prefab);
		stageInstance.transform.parent = currentScreen.transform;
		
		config.currentRound = 1;
		config.lockInputs = true;

		if (config.selectedStage.music != null) Camera.main.GetComponent<AudioSource>().clip = config.selectedStage.music;
		if (config.music) Camera.main.GetComponent<AudioSource>().Play();
		
		/*GameObject gui = new GameObject("GUI");
		uiLayer = LayerMask.NameToLayer("UI");
		gui.layer = uiLayer;
		gui.AddComponent("UI");
		UI uiScript = gui.GetComponent<UI>();
		uiScript.autoTextureSelectionForHD = false;
		uiScript.allowPod4GenHD = false;
		
		GameObject uiToolkit = new GameObject("UIToolkit");
		uiToolkit.transform.parent = gui.transform;
		uiToolkit.AddComponent("UIToolkit");
		UIToolkit uiToolkitScript = uiToolkit.GetComponent<UIToolkit>();
		uiToolkitScript.texturePackerConfigName = "sprites";
		uiToolkitScript.material = prefs.fontMaterial;*/
		
		//Instantiate(prefs.uiPrefab);
		
		/*GameObject gui = new GameObject("GUI");
		gui.transform.parent = currentScreen.transform;

		GameObject gui1 = new GameObject("GUI1");
		gui1.transform.parent = gui.transform;
		gui1.AddComponent("GUIScript");
		GUIScript guiScript = gui1.GetComponent<GUIScript>();
		guiScript.side = Side.Left;
		guiScript.lifePoints = config.player1.lifePoints;
		guiScript.lifeBarOptions = config.guiOptions.lifeBarOptions1;
		guiScript.characterInfo = config.player1;
		guiScript.currentAnchor = TextAnchor.UpperLeft;
		
		GameObject gui2 = new GameObject("GUI2");
		gui2.transform.parent = gui.transform;
		gui2.AddComponent("GUIScript");
		guiScript = gui2.GetComponent<GUIScript>();
		guiScript.side = Side.Right;
		guiScript.lifePoints = config.player2.lifePoints;
		guiScript.lifeBarOptions = config.guiOptions.lifeBarOptions2;
		guiScript.characterInfo = config.player2;
		guiScript.currentAnchor = TextAnchor.UpperRight;*/


		GameObject p1 = new GameObject("Player1");
		p1.transform.parent = currentScreen.transform;
		p1.AddComponent<ControlsScript>();
		p1.AddComponent<PhysicsScript>();
	
		/*GameObject debuggerGO = new GameObject("Debugger1");
		debuggerGO.AddComponent("GUIText");
		debugger1 = debuggerGO.GetComponent<GUIText>();
		debugger1.pixelOffset = new Vector2(30 * (Screen.width/640), 100 * (Screen.height/360));
		debugger1.text = "Debug mode";
		debugger1.color = Color.black;
		debugger1.richText = true;*/
		
		GameObject p2 = new GameObject("Player2");
		p2.transform.parent = currentScreen.transform;
		p2.AddComponent<ControlsScript>();
		p2.AddComponent<PhysicsScript>();

		/*GameObject debuggerGO2 = new GameObject("Debugger2");
		debuggerGO2.AddComponent("GUIText");
		debugger2 = debuggerGO2.GetComponent<GUIText>();
		debugger2.pixelOffset = new Vector2(600 * (Screen.width/640), 100 * (Screen.height/360));
		debugger2.text = "Debug mode";
		debugger2.color = Color.black;
		debugger2.richText = true;*/
	}

	public static void SetPlayer1(CharacterInfo player1){
		config.player1Character = player1;
	}
	public static CharacterInfo GetPlayer1(){
		return config.player1Character;
	}

	public static void SetPlayer2(CharacterInfo player2){
		config.player2Character = player2;
	}
	public static CharacterInfo GetPlayer2(){
		return config.player2Character;
	}


	
	public static void SetLanguage(string language){
		foreach(LanguageOptions languageOption in config.languages){
			if (language == languageOption.languageName){
				config.selectedLanguage = languageOption;
				return;
			}
		}
	}


	public static void SetStage(string stageName){
		foreach(StageOptions stage in config.stages){
			if (stageName == stage.stageName){
				config.selectedStage = stage;
				return;
			}
		}
	}
	public static StageOptions GetStage(){
		return config.selectedStage;
	}
	
	public static FontOptions GetFont(FontId fontId){
		foreach(FontOptions fontOption in config.fontOptions){
			if (fontOption.fontId == fontId) return fontOption;
		}
		return null;
	}
	public static MoveSetData GetCurrentMoveSet(CharacterInfo character){
		foreach(MoveSetData moveSetData in character.moves)
			if (moveSetData.combatStance == character.currentCombatStance)
				return moveSetData;
		return null;
	}

	public static void SetLifePoints(float newValue, CharacterInfo player){
		if (UFE.OnLifePointsChange != null) OnLifePointsChange(newValue, player);
	}
	
	public static void FireAlert(string alertMessage, CharacterInfo player){
		if (UFE.OnNewAlert != null) OnNewAlert(alertMessage, player);
	}

	public static void FireHit(HitBox strokeHitBox, MoveInfo move, CharacterInfo player){
		if (UFE.OnHit != null) OnHit(strokeHitBox, move, player);
	}
	
	public static void FireMove(MoveInfo move, CharacterInfo player){
		if (UFE.OnMove != null) OnMove(move, player);
	}
	
	public static void FireGameBegins(){
		if (UFE.OnGameBegin != null) OnGameBegin(config.player1Character, config.player2Character, config.selectedStage);
	}
	
	public static void FireGameEnds(CharacterInfo winner, CharacterInfo loser){
		if (UFE.OnGameEnds != null) OnGameEnds(winner, loser);
	}
	
	public static void FireRoundBegins(int currentRound){
		if (UFE.OnRoundBegins != null) OnRoundBegins(currentRound);
	}

	public static void FireRoundEnds(CharacterInfo winner, CharacterInfo loser){
		if (UFE.OnRoundEnds != null) OnRoundEnds(winner, loser);
	}

	public static bool GetMusic(){
		return config.music;
	}

	public static void SetMusic(bool on){
		if (on && !config.music){
			config.music = true;
			Camera.main.GetComponent<AudioSource>().Play();
		}else if (config.music){
			config.music = false;
			Camera.main.GetComponent<AudioSource>().Stop();
		}
	}
	
	public static bool GetSoundFX(){
		return config.soundfx;
	}

	public static void SetSoundFX(bool on){
		if (on && !config.soundfx){
			config.soundfx = true;
		}else if (config.soundfx){
			config.soundfx = false;
		}
	}
	
	public static float GetVolume(){
		return Camera.main.GetComponent<AudioSource>().volume;
	}

	public static void SetVolume(float volume){
		Camera.main.GetComponent<AudioSource>().volume = volume;
	}

	public static string GetInputReference(ButtonPress button, InputReferences[] inputReferences){
		foreach(InputReferences inputReference in inputReferences){
			if (inputReference.engineRelatedButton == button) return inputReference.inputButtonName;
		}
		return null;
	}
	
	public static string GetInputReference(InputType inputType, InputReferences[] inputReferences){
		foreach(InputReferences inputReference in inputReferences){
			if (inputReference.inputType == inputType) return inputReference.inputButtonName;
		}
		return null;
	}

	public static void PauseGame(bool pause){
		if (pause && Time.timeScale == 0) return;

		if (pause){
			Time.timeScale = 0;
		}else{
			Time.timeScale = config.gameSpeed;
		}
	}

	public static bool isPaused(){
		if (Time.timeScale == 0) return true;
		return false;
	}

	void Awake(){
		config = UFE_Config;
		if (config.fps > 0) {
			Time.fixedDeltaTime = (float)1/(float)config.fps;
			Time.timeScale = config.gameSpeed;
			QualitySettings.vSyncCount = 0;
		}
		
		SetLanguage("English");
		
		Camera.main.GetComponent<AudioSource>().loop = true;

		if (config.startGameImmediately){
			config.player1Character = config.p1CharStorage;
			config.player2Character = config.p2CharStorage;
			StartGame(0);
		}else{
			StartIntro(0);
		}
	}

	
	void Update(){
		if (config.fps > 0 && Application.targetFrameRate != config.fps)
			Application.targetFrameRate = config.fps;
	}
}