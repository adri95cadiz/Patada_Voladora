using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;


public class GlobalEditorWindow : EditorWindow {
	public static GlobalEditorWindow globalEditorWindow;
	private GlobalInfo globalInfo;
	private Vector2 scrollPos;


	private bool advancedOptions;
	private bool cameraOptions;
	private bool fontOptions;
	private bool languageOptions;
	private bool guiOptions;
	private bool screenOptions;
	private bool roundOptions;
	private bool bounceOptions;
	private bool comboOptions;
	private bool blockOptions;
	private bool knockDownOptions;
	private bool hitOptions;
	private bool inputsOptions;
	private bool player1InputOptions;
	private bool player2InputOptions;
	private bool stageOptions;
	private bool characterOptions;
	
	private string titleStyle;
	private string removeButtonStyle;
	private string addButtonStyle;
	private string rootGroupStyle;
	private string subGroupStyle;
	private string arrayElementStyle;
	private string foldStyle;
	private string enumStyle;
	private GUIStyle labelStyle;
	private GUIContent helpGUIContent = new GUIContent();

	[MenuItem("Window/U.F.E./Global Editor")]
	public static void Init(){
		globalEditorWindow = EditorWindow.GetWindow<GlobalEditorWindow>(false, "Global", true);
		globalEditorWindow.Show();
		globalEditorWindow.Populate();
	}
	
	void OnSelectionChange(){
		Populate();
		Repaint();
	}
	
	void OnEnable(){
		Populate();
	}
	
	void OnFocus(){
		Populate();
	}
	
	void OnDisable(){
		Clear();
	}
	
	void OnDestroy(){
		Clear();
	}
	
	void OnLostFocus(){
		//Clear();
	}
	
	void Update(){
		if (EditorApplication.isPlayingOrWillChangePlaymode) {
			Clear();
		}
	}

	void Clear(){
		if (globalInfo != null){
			Clear(globalInfo.guiOptions.lifeBarOptions1);
			Clear(globalInfo.guiOptions.lifeBarOptions2);
			Clear(globalInfo.guiOptions.gaugeBarOptions1);
			Clear(globalInfo.guiOptions.gaugeBarOptions2);
		}
	}

	void Clear(GUIBarOptions guiBar){
		guiBar.previewToggle = false;
		if (guiBar.bgPreview != null){
			DestroyImmediate(guiBar.bgPreview);
			guiBar.bgPreview = null;
		}
		if (guiBar.fillPreview != null){
			DestroyImmediate(guiBar.fillPreview);
			guiBar.fillPreview = null;
		}
	}
	
	void helpButton(string page){
		if (GUILayout.Button("?", GUILayout.Width(18), GUILayout.Height(18))) 
			Application.OpenURL("http://www.ufe3d.com/doku.php/"+ page);
	}

	void Populate(){
		// Style Definitions
		titleStyle = "MeTransOffRight";
		removeButtonStyle = "TL SelectionBarCloseButton";
		addButtonStyle = "CN CountBadge";
		rootGroupStyle = "GroupBox";
		subGroupStyle = "ObjectFieldThumb";
		arrayElementStyle = "flow overlay box";
		foldStyle = "Foldout";
		enumStyle = "MiniPopup";
		
		labelStyle = new GUIStyle();
		labelStyle.alignment = TextAnchor.MiddleCenter;
		labelStyle.fontStyle = FontStyle.Bold;
		labelStyle.normal.textColor = Color.white;


		helpGUIContent.text = "";
		helpGUIContent.tooltip = "Open Live Docs";
		//helpGUIContent.image = (Texture2D) EditorGUIUtility.Load("icons/SVN_Local.png");
		
		Object[] selection = Selection.GetFiltered(typeof(GlobalInfo), SelectionMode.Assets);
		if (selection.Length > 0){
			if (selection[0] == null) return;
			globalInfo = (GlobalInfo) selection[0];
		}
	}

	public void OnGUI(){
		if (globalInfo == null){
			GUILayout.BeginHorizontal("GroupBox");
			GUILayout.Label("Select UFE's Global Configuration file or create a new one.","CN EntryInfo");
			GUILayout.EndHorizontal();
			EditorGUILayout.Space();
			if (GUILayout.Button("Create new Global Configuration"))
				ScriptableObjectUtility.CreateAsset<CharacterInfo> ();
			return;
		}


		GUIStyle fontStyle = new GUIStyle();
		fontStyle.font = (Font) EditorGUIUtility.Load("EditorFont.TTF");
		fontStyle.fontSize = 30;
		fontStyle.alignment = TextAnchor.UpperCenter;
		fontStyle.normal.textColor = Color.white;
		fontStyle.hover.textColor = Color.white;
		EditorGUILayout.BeginVertical(titleStyle);{
			EditorGUILayout.BeginHorizontal();{
				EditorGUILayout.LabelField("", (globalInfo.gameName == ""? "Universal Fighting Engine":globalInfo.gameName) , fontStyle, GUILayout.Height(32));
				helpButton("global:start");
			}EditorGUILayout.EndHorizontal();
		}EditorGUILayout.EndVertical();
		
		scrollPos = EditorGUILayout.BeginScrollView(scrollPos);{
			EditorGUILayout.BeginVertical(rootGroupStyle);{
				globalInfo.gameName = EditorGUILayout.TextField("Project Name:", globalInfo.gameName);
				EditorGUILayout.Space();
				
				EditorGUIUtility.labelWidth = 200;
				//globalInfo.introScreen = (GameObject) EditorGUILayout.ObjectField("Intro Screen:", globalInfo.introScreen, typeof(UnityEngine.GameObject), true);
				//globalInfo.characterSelectScreen = (GameObject) EditorGUILayout.ObjectField("Character Selection Screen:", globalInfo.characterSelectScreen, typeof(UnityEngine.GameObject), true);
				//globalInfo.stageSelectScreen = (GameObject) EditorGUILayout.ObjectField("Stage Selection Screen:", globalInfo.stageSelectScreen, typeof(UnityEngine.GameObject), true);
				//globalInfo.creditsScreen = (GameObject) EditorGUILayout.ObjectField("Credits Screen:", globalInfo.creditsScreen, typeof(UnityEngine.GameObject), true);
				EditorGUILayout.Space();
				globalInfo.startGameImmediately = EditorGUILayout.Toggle("Start Game Immediately", globalInfo.startGameImmediately);
				if (globalInfo.startGameImmediately){
					if (globalInfo.stages.Length > 0) globalInfo.selectedStage = globalInfo.stages[0];
					globalInfo.p1CharStorage = (CharacterInfo) EditorGUILayout.ObjectField("Player 1 Character:", globalInfo.p1CharStorage, typeof(CharacterInfo), false);
					globalInfo.p2CharStorage = (CharacterInfo) EditorGUILayout.ObjectField("Player 2 Character:", globalInfo.p2CharStorage, typeof(CharacterInfo), false);
					globalInfo.trainingMode = EditorGUILayout.Toggle("Training Mode", globalInfo.trainingMode);
				}else{
					globalInfo.player1Character = null;
					globalInfo.player2Character = null;
				}


				EditorGUIUtility.labelWidth = 150;
			}EditorGUILayout.EndVertical();

			
			// Language Options
			EditorGUILayout.BeginVertical(rootGroupStyle);{
				EditorGUILayout.BeginHorizontal();{
					languageOptions = EditorGUILayout.Foldout(languageOptions, "Languages ("+ globalInfo.languages.Length +")", foldStyle);
					helpButton("global:languages");
				}EditorGUILayout.EndHorizontal();

				if (languageOptions){
					EditorGUILayout.BeginVertical(subGroupStyle);{
						EditorGUILayout.Space();
						EditorGUI.indentLevel += 1;
						
						for (int i = 0; i < globalInfo.languages.Length; i ++){
							EditorGUILayout.Space();
							EditorGUILayout.BeginVertical(arrayElementStyle);{
								EditorGUILayout.Space();
								EditorGUILayout.BeginHorizontal();{
									globalInfo.languages[i].languageName = EditorGUILayout.TextField("Language:", globalInfo.languages[i].languageName);
									if (GUILayout.Button("", removeButtonStyle)){
										globalInfo.languages = RemoveElement<LanguageOptions>(globalInfo.languages, globalInfo.languages[i]);
										return;
									}
								}EditorGUILayout.EndHorizontal();
								globalInfo.languages[i].start = EditorGUILayout.TextField("Start:", globalInfo.languages[i].start);
								globalInfo.languages[i].options = EditorGUILayout.TextField("Options:", globalInfo.languages[i].options);
								globalInfo.languages[i].credits = EditorGUILayout.TextField("Credits:", globalInfo.languages[i].credits);
								globalInfo.languages[i].selectYourCharacter = EditorGUILayout.TextField("Select Your Character:", globalInfo.languages[i].selectYourCharacter);
								globalInfo.languages[i].selectYourStage = EditorGUILayout.TextField("Select Your Stage:", globalInfo.languages[i].selectYourStage);
								globalInfo.languages[i].round = EditorGUILayout.TextField("Round:", globalInfo.languages[i].round);
								globalInfo.languages[i].finalRound = EditorGUILayout.TextField("Final Round:", globalInfo.languages[i].finalRound);
								globalInfo.languages[i].fight = EditorGUILayout.TextField("Fight:", globalInfo.languages[i].fight);
								globalInfo.languages[i].firstHit = EditorGUILayout.TextField("First Hit:", globalInfo.languages[i].firstHit);
								globalInfo.languages[i].combo = EditorGUILayout.TextField("Combo:", globalInfo.languages[i].combo);
								globalInfo.languages[i].parry = EditorGUILayout.TextField("Parry:", globalInfo.languages[i].parry);
								globalInfo.languages[i].victory = EditorGUILayout.TextField("Victory:", globalInfo.languages[i].victory);
								globalInfo.languages[i].perfect = EditorGUILayout.TextField("Perfect:", globalInfo.languages[i].perfect);
								globalInfo.languages[i].rematch = EditorGUILayout.TextField("Rematch:", globalInfo.languages[i].rematch);
								globalInfo.languages[i].quit = EditorGUILayout.TextField("Quit:", globalInfo.languages[i].quit);
								EditorGUILayout.Space();
							}EditorGUILayout.EndVertical();
							EditorGUILayout.Space();
						}
						
						if (StyledButton("New Language"))
							globalInfo.languages = AddElement<LanguageOptions>(globalInfo.languages, new LanguageOptions());
						
						EditorGUI.indentLevel -= 1;
						EditorGUILayout.Space();
					}EditorGUILayout.EndVertical();
				}
			}EditorGUILayout.EndVertical();


			// Fonts
			EditorGUILayout.BeginVertical(rootGroupStyle);{
				EditorGUILayout.BeginHorizontal();{
					fontOptions = EditorGUILayout.Foldout(fontOptions, "Fonts ("+ globalInfo.fontOptions.Length +")", foldStyle);
					helpButton("global:fonts");
				}EditorGUILayout.EndHorizontal();

				if (fontOptions){
					EditorGUILayout.BeginVertical(subGroupStyle);{
						EditorGUILayout.Space();
						EditorGUI.indentLevel += 1;
						
						for (int i = 0; i < globalInfo.fontOptions.Length; i ++){
							EditorGUILayout.Space();
							EditorGUILayout.BeginVertical(arrayElementStyle);{
								EditorGUILayout.Space();
								EditorGUILayout.BeginHorizontal();{
									globalInfo.fontOptions[i].fontId = (FontId) EditorGUILayout.EnumPopup("Font ID:", globalInfo.fontOptions[i].fontId, enumStyle);
									if (GUILayout.Button("", removeButtonStyle)){
										globalInfo.fontOptions = RemoveElement<FontOptions>(globalInfo.fontOptions, globalInfo.fontOptions[i]);
										return;
									}
								}EditorGUILayout.EndHorizontal();
								globalInfo.fontOptions[i].fontPrefab = (GameObject) EditorGUILayout.ObjectField("Font Prefab:", globalInfo.fontOptions[i].fontPrefab, typeof(UnityEngine.GameObject), true);
								EditorGUILayout.Space();
							}EditorGUILayout.EndVertical();
							EditorGUILayout.Space();
						}
						
						EditorGUI.indentLevel -= 1;
						
						if (StyledButton("New Font"))
							globalInfo.fontOptions = AddElement<FontOptions>(globalInfo.fontOptions, new FontOptions());
						
					}EditorGUILayout.EndVertical();
				}
			}EditorGUILayout.EndVertical();


			// Camera Options
			EditorGUILayout.BeginVertical(rootGroupStyle);{
				EditorGUILayout.BeginHorizontal();{
					cameraOptions = EditorGUILayout.Foldout(cameraOptions, "Camera Options", foldStyle);
					helpButton("global:camera");
				}EditorGUILayout.EndHorizontal();

				if (cameraOptions){
					EditorGUILayout.BeginVertical(subGroupStyle);{
						EditorGUILayout.Space();
						EditorGUI.indentLevel += 1;
						
						globalInfo.cameraOptions.initialFieldOfView = EditorGUILayout.Slider("Field of View:", globalInfo.cameraOptions.initialFieldOfView, 1, 179);
						globalInfo.cameraOptions.initialDistance = EditorGUILayout.Vector3Field("Initial Distance:", globalInfo.cameraOptions.initialDistance);
						globalInfo.cameraOptions.initialDistance.x = 0;
						globalInfo.cameraOptions.initialRotation = EditorGUILayout.Vector3Field("Initial Rotation:", globalInfo.cameraOptions.initialRotation);
						globalInfo.cameraOptions.smooth = EditorGUILayout.FloatField("Smooth Translation:", globalInfo.cameraOptions.smooth);
						globalInfo.cameraOptions.minZoom = EditorGUILayout.FloatField("Minimum Zoom:", globalInfo.cameraOptions.minZoom);
						globalInfo.cameraOptions.maxZoom = EditorGUILayout.FloatField("Maximum Zoom:", globalInfo.cameraOptions.maxZoom);

						EditorGUI.indentLevel -= 1;
						EditorGUILayout.Space();
					}EditorGUILayout.EndVertical();
				}
			}EditorGUILayout.EndVertical();


			// Round Options
			EditorGUILayout.BeginVertical(rootGroupStyle);{
				EditorGUILayout.BeginHorizontal();{
					roundOptions = EditorGUILayout.Foldout(roundOptions, "Round Options", foldStyle);
					helpButton("global:round");
				}EditorGUILayout.EndHorizontal();

				if (roundOptions){
					EditorGUILayout.BeginVertical(subGroupStyle);{
						EditorGUILayout.Space();
						EditorGUI.indentLevel += 1;
						
						EditorGUIUtility.labelWidth = 200;
						globalInfo.roundOptions.totalRounds = EditorGUILayout.IntField("Total Rounds (Best of):", globalInfo.roundOptions.totalRounds);
						globalInfo.roundOptions.p1XPosition = EditorGUILayout.FloatField("Initial Ground Position (P1):", globalInfo.roundOptions.p1XPosition);
						globalInfo.roundOptions.p2XPosition = EditorGUILayout.FloatField("Initial Ground Position (P2):", globalInfo.roundOptions.p2XPosition);
						globalInfo.roundOptions.delayBeforeEndGame = EditorGUILayout.FloatField("Delay before end game:", globalInfo.roundOptions.delayBeforeEndGame);
						globalInfo.roundOptions.victoryMusic = (AudioClip) EditorGUILayout.ObjectField("Victory Music:", globalInfo.roundOptions.victoryMusic, typeof(UnityEngine.AudioClip), false);
						globalInfo.roundOptions.timer = EditorGUILayout.FloatField("Round Timer (seconds):", globalInfo.roundOptions.timer);
						globalInfo.roundOptions.resetLifePoints = EditorGUILayout.Toggle("Reset life points", globalInfo.roundOptions.resetLifePoints);
						globalInfo.roundOptions.resetPositions = EditorGUILayout.Toggle("Reset positions", globalInfo.roundOptions.resetPositions);
						globalInfo.roundOptions.allowMovement = EditorGUILayout.Toggle("Allow movement before battle", globalInfo.roundOptions.allowMovement);
						globalInfo.roundOptions.slowMotionKO = EditorGUILayout.Toggle("Slow motion K.O.", globalInfo.roundOptions.slowMotionKO);
						//globalInfo.roundOptions.cameraZoomKO = EditorGUILayout.Toggle("Camera Zoom K.O.", globalInfo.roundOptions.cameraZoomKO);
						globalInfo.roundOptions.freezeCamAfterOutro = EditorGUILayout.Toggle("Freeze camera after outro", globalInfo.roundOptions.freezeCamAfterOutro);
						EditorGUIUtility.labelWidth = 150;
						
						EditorGUI.indentLevel -= 1;
						EditorGUILayout.Space();
					}EditorGUILayout.EndVertical();
				}
			}EditorGUILayout.EndVertical();

			// Bounce Options
			EditorGUILayout.BeginVertical(rootGroupStyle);{
				EditorGUILayout.BeginHorizontal();{
					bounceOptions = EditorGUILayout.Foldout(bounceOptions, "Bounce Options", foldStyle);
					helpButton("global:bounce");
				}EditorGUILayout.EndHorizontal();

				if (bounceOptions){
					EditorGUILayout.BeginVertical(subGroupStyle);{
						EditorGUILayout.Space();
						EditorGUI.indentLevel += 1;
						
						EditorGUIUtility.labelWidth = 180;
						globalInfo.bounceOptions.bouncePrefab = (GameObject) EditorGUILayout.ObjectField("Bounce Effect:", globalInfo.bounceOptions.bouncePrefab, typeof(UnityEngine.GameObject), true);
						globalInfo.bounceOptions.minimumBounceForce = EditorGUILayout.FloatField("Minimum Bounce Force:", globalInfo.bounceOptions.minimumBounceForce);
						globalInfo.bounceOptions.bounceForce = (Sizes) EditorGUILayout.EnumPopup("Bounce Back Force:", globalInfo.bounceOptions.bounceForce, enumStyle);
						globalInfo.bounceOptions.maximumBounces = EditorGUILayout.FloatField("Maximum Bounces:", globalInfo.bounceOptions.maximumBounces);
						globalInfo.bounceOptions.bounceHitBoxes = EditorGUILayout.Toggle("Bounce Hit Boxes", globalInfo.bounceOptions.bounceHitBoxes);
						EditorGUIUtility.labelWidth = 150;

						EditorGUI.indentLevel -= 1;
						EditorGUILayout.Space();
					}EditorGUILayout.EndVertical();
				}
			}EditorGUILayout.EndVertical();
			
			// Combo Options
			EditorGUILayout.BeginVertical(rootGroupStyle);{
				EditorGUILayout.BeginHorizontal();{
					comboOptions = EditorGUILayout.Foldout(comboOptions, "Combo Options", foldStyle);
					helpButton("global:combo");
				}EditorGUILayout.EndHorizontal();

				if (comboOptions){
					EditorGUILayout.BeginVertical(subGroupStyle);{
						EditorGUILayout.Space();
						EditorGUI.indentLevel += 1;
						
						EditorGUIUtility.labelWidth = 180;
						globalInfo.comboOptions.maxCombo = EditorGUILayout.IntField("Maximum Hits:", globalInfo.comboOptions.maxCombo);

						globalInfo.comboOptions.hitStunDeterioration = (Sizes) EditorGUILayout.EnumPopup("Hit Stun Deterioration:", globalInfo.comboOptions.hitStunDeterioration, enumStyle);
						EditorGUI.BeginDisabledGroup(globalInfo.comboOptions.hitStunDeterioration == Sizes.None);{
							globalInfo.comboOptions.minHitStun = EditorGUILayout.FloatField("Minimum Hit Stun (frames):", globalInfo.comboOptions.minHitStun);
						}EditorGUI.EndDisabledGroup();

						globalInfo.comboOptions.damageDeterioration = (Sizes) EditorGUILayout.EnumPopup("Damage Deterioration:", globalInfo.comboOptions.damageDeterioration, enumStyle);
						EditorGUI.BeginDisabledGroup(globalInfo.comboOptions.damageDeterioration == Sizes.None);{
							globalInfo.comboOptions.minDamage = EditorGUILayout.FloatField("Minimum Damage:", globalInfo.comboOptions.minDamage);
						}EditorGUI.EndDisabledGroup();

						globalInfo.comboOptions.airJuggleDeterioration = (Sizes) EditorGUILayout.EnumPopup("Air-Juggle Deterioration:", globalInfo.comboOptions.airJuggleDeterioration, enumStyle);
						EditorGUI.BeginDisabledGroup(globalInfo.comboOptions.airJuggleDeterioration == Sizes.None);{
							globalInfo.comboOptions.minPushForce = EditorGUILayout.FloatField("Minimum Juggle Force:", globalInfo.comboOptions.minPushForce);
						}EditorGUI.EndDisabledGroup();

						globalInfo.comboOptions.neverAirRecover = EditorGUILayout.Toggle("Never Air-Recover", globalInfo.comboOptions.neverAirRecover);

						EditorGUIUtility.labelWidth = 150;
						
						EditorGUI.indentLevel -= 1;
						EditorGUILayout.Space();
					}EditorGUILayout.EndVertical();
				}
			}EditorGUILayout.EndVertical();

			
			// Block Options
			EditorGUILayout.BeginVertical(rootGroupStyle);{
				EditorGUILayout.BeginHorizontal();{
					blockOptions = EditorGUILayout.Foldout(blockOptions, "Block Options", foldStyle);
					helpButton("global:block");
				}EditorGUILayout.EndHorizontal();

				if (blockOptions){
					EditorGUILayout.BeginVertical(subGroupStyle);{
						EditorGUILayout.Space();
						EditorGUI.indentLevel += 1;
						
						globalInfo.blockOptions.blockType = (BlockType) EditorGUILayout.EnumPopup("Block Input:", globalInfo.blockOptions.blockType, enumStyle);
						EditorGUI.BeginDisabledGroup(globalInfo.blockOptions.blockType == BlockType.None);{
							globalInfo.blockOptions.blockPrefab = (GameObject) EditorGUILayout.ObjectField("Block Effect:", globalInfo.blockOptions.blockPrefab, typeof(UnityEngine.GameObject), true);
							globalInfo.blockOptions.blockKillTime = EditorGUILayout.FloatField("Effect Kill Time:", globalInfo.blockOptions.blockKillTime);
							globalInfo.blockOptions.blockSound = (AudioClip) EditorGUILayout.ObjectField("Block Sound:", globalInfo.blockOptions.blockSound, typeof(UnityEngine.AudioClip), false);
							globalInfo.blockOptions.allowAirBlock = EditorGUILayout.Toggle("Allow Air Block", globalInfo.blockOptions.allowAirBlock);
						}EditorGUI.EndDisabledGroup();

						EditorGUILayout.Space();

						globalInfo.blockOptions.parryType = (ParryType) EditorGUILayout.EnumPopup("Parry Input:", globalInfo.blockOptions.parryType, enumStyle);
						EditorGUI.BeginDisabledGroup(globalInfo.blockOptions.parryType == ParryType.None);{
							globalInfo.blockOptions.parryTiming = EditorGUILayout.FloatField("Parry Timing:", globalInfo.blockOptions.parryTiming);
							globalInfo.blockOptions.parryPrefab = (GameObject) EditorGUILayout.ObjectField("Parry Effect:", globalInfo.blockOptions.parryPrefab, typeof(UnityEngine.GameObject), true);
							globalInfo.blockOptions.parryKillTime = EditorGUILayout.FloatField("Effect Kill Time:", globalInfo.blockOptions.parryKillTime);
							globalInfo.blockOptions.parrySound = (AudioClip) EditorGUILayout.ObjectField("Parry Sound:", globalInfo.blockOptions.parrySound, typeof(UnityEngine.AudioClip), false);
							globalInfo.blockOptions.allowAirParry = EditorGUILayout.Toggle("Allow Air Parry", globalInfo.blockOptions.allowAirParry);
							globalInfo.blockOptions.highlightWhenParry = EditorGUILayout.Toggle("Highlight When Parry", globalInfo.blockOptions.highlightWhenParry);
							globalInfo.blockOptions.parryColor = EditorGUILayout.ColorField("Parry Color:", globalInfo.blockOptions.parryColor);
						}EditorGUI.EndDisabledGroup();

						EditorGUI.indentLevel -= 1;
						EditorGUILayout.Space();
					}EditorGUILayout.EndVertical();
				}
			}EditorGUILayout.EndVertical();


			// Knock Down Options
			EditorGUILayout.BeginVertical(rootGroupStyle);{
				EditorGUILayout.BeginHorizontal();{
					knockDownOptions = EditorGUILayout.Foldout(knockDownOptions, "Knock Down Options", foldStyle);
					helpButton("global:knockdown");
				}EditorGUILayout.EndHorizontal();

				if (knockDownOptions){
					EditorGUILayout.BeginVertical(subGroupStyle);{
						EditorGUILayout.Space();
						EditorGUI.indentLevel += 1;
						
						EditorGUIUtility.labelWidth = 180;
						globalInfo.knockDownOptions.knockedOutTime = EditorGUILayout.FloatField("Knock Out Time:", globalInfo.knockDownOptions.knockedOutTime);
						globalInfo.knockDownOptions.getUpTime = EditorGUILayout.FloatField("Get Up Time:", globalInfo.knockDownOptions.getUpTime);
						globalInfo.knockDownOptions.knockedOutHitBoxes = EditorGUILayout.Toggle("Knocked Out Hit Boxes", globalInfo.knockDownOptions.knockedOutHitBoxes);
						EditorGUIUtility.labelWidth = 150;


						EditorGUI.indentLevel -= 1;
						EditorGUILayout.Space();
					}EditorGUILayout.EndVertical();
				}
			}EditorGUILayout.EndVertical();

			
			// Hit Effects Options
			EditorGUILayout.BeginVertical(rootGroupStyle);{
				EditorGUILayout.BeginHorizontal();{
					hitOptions = EditorGUILayout.Foldout(hitOptions, "Hit Effects Options", foldStyle);
					helpButton("global:hitEffects");
				}EditorGUILayout.EndHorizontal();

				if (hitOptions){
					EditorGUILayout.BeginVertical(subGroupStyle);{
						EditorGUILayout.Space();
						EditorGUI.indentLevel += 1;
						
						EditorGUIUtility.labelWidth = 180;
						HitOptionBlock("Weak Hit Options", globalInfo.hitOptions.weakHit);
						HitOptionBlock("Medium Hit Options", globalInfo.hitOptions.mediumHit);
						HitOptionBlock("Heavy Hit Options", globalInfo.hitOptions.heavyHit);
						HitOptionBlock("Crumple Hit Options", globalInfo.hitOptions.crumpleHit);
						HitOptionBlock("Custom Hit 1 Options", globalInfo.hitOptions.customHit1);
						HitOptionBlock("Custom Hit 2 Options", globalInfo.hitOptions.customHit2);
						HitOptionBlock("Custom Hit 3 Options", globalInfo.hitOptions.customHit3);
						EditorGUIUtility.labelWidth = 150;
						
						EditorGUI.indentLevel -= 1;
						EditorGUILayout.Space();
					}EditorGUILayout.EndVertical();
				}
			}EditorGUILayout.EndVertical();


			// Screen Options
			EditorGUILayout.BeginVertical(rootGroupStyle);{
				EditorGUILayout.BeginHorizontal();{
					screenOptions = EditorGUILayout.Foldout(screenOptions, "Screen Options", foldStyle);
					helpButton("global:screen");
				}EditorGUILayout.EndHorizontal();

				if (screenOptions){
					EditorGUILayout.BeginVertical(subGroupStyle);{
						EditorGUILayout.Space();
						EditorGUI.indentLevel += 1;
						EditorGUIUtility.labelWidth = 180;

						globalInfo.introScreen = (GameObject) EditorGUILayout.ObjectField("Intro Screen:", globalInfo.introScreen, typeof(UnityEngine.GameObject), true);
						globalInfo.introMusic = (AudioClip) EditorGUILayout.ObjectField("Intro Music:", globalInfo.introMusic, typeof(UnityEngine.AudioClip), false);
						globalInfo.characterSelectScreen = (GameObject) EditorGUILayout.ObjectField("Character Selection Screen:", globalInfo.characterSelectScreen, typeof(UnityEngine.GameObject), true);
						globalInfo.characterSelectMusic = (AudioClip) EditorGUILayout.ObjectField("Character Selection Music:", globalInfo.characterSelectMusic, typeof(UnityEngine.AudioClip), false);
						globalInfo.stageSelectScreen = (GameObject) EditorGUILayout.ObjectField("Stage Selection Screen:", globalInfo.stageSelectScreen, typeof(UnityEngine.GameObject), true);
						globalInfo.stageSelectMusic = (AudioClip) EditorGUILayout.ObjectField("Stage Selection Music:", globalInfo.stageSelectMusic, typeof(UnityEngine.AudioClip), false);
						globalInfo.creditsScreen = (GameObject) EditorGUILayout.ObjectField("Credits Screen:", globalInfo.creditsScreen, typeof(UnityEngine.GameObject), true);
						globalInfo.creditsMusic = (AudioClip) EditorGUILayout.ObjectField("Credits Music:", globalInfo.creditsMusic, typeof(UnityEngine.AudioClip), false);
						
						EditorGUIUtility.labelWidth = 150;
						EditorGUI.indentLevel -= 1;

					}EditorGUILayout.EndVertical();
				}
			}EditorGUILayout.EndVertical();


			// GUI Options
			EditorGUILayout.BeginVertical(rootGroupStyle);{
				EditorGUILayout.BeginHorizontal();{
					guiOptions = EditorGUILayout.Foldout(guiOptions, "Game GUI Options", foldStyle);
					helpButton("global:gamegui");
				}EditorGUILayout.EndHorizontal();

				if (guiOptions){
					EditorGUILayout.BeginVertical(subGroupStyle);{
						EditorGUILayout.Space();
						EditorGUI.indentLevel += 1;
						
						EditorGUIUtility.labelWidth = 180;

						globalInfo.guiOptions.alertFont = (FontId) EditorGUILayout.EnumPopup("In-Game Message Font:", globalInfo.guiOptions.alertFont, enumStyle);
						globalInfo.guiOptions.characterNameFont = (FontId) EditorGUILayout.EnumPopup("Character Name Font:", globalInfo.guiOptions.characterNameFont, enumStyle);
						globalInfo.guiOptions.menuFontBig = (FontId) EditorGUILayout.EnumPopup("Menu Font (Big):", globalInfo.guiOptions.menuFontBig, enumStyle);
						globalInfo.guiOptions.menuFontSmall = (FontId) EditorGUILayout.EnumPopup("Menu Font (Small):", globalInfo.guiOptions.menuFontSmall, enumStyle);

						EditorGUIUtility.labelWidth = 150;

						GUIBarBlock("Player 1 Life Bar Options", globalInfo.guiOptions.lifeBarOptions1);
						GUIBarBlock("Player 1 Gauge Bar Options", globalInfo.guiOptions.gaugeBarOptions1);
						GUIBarBlock("Player 2 Life Bar Options", globalInfo.guiOptions.lifeBarOptions2);
						GUIBarBlock("Player 2 Gauge Bar Options", globalInfo.guiOptions.gaugeBarOptions2);

						EditorGUI.indentLevel -= 1;
						EditorGUILayout.Space();
					}EditorGUILayout.EndVertical();
				}
			}EditorGUILayout.EndVertical();


			// Inputs
			EditorGUILayout.BeginVertical(rootGroupStyle);{
				EditorGUILayout.BeginHorizontal();{
					inputsOptions = EditorGUILayout.Foldout(inputsOptions, "Input Options", foldStyle);
					helpButton("global:input");
				}EditorGUILayout.EndHorizontal();

				if (inputsOptions){
					EditorGUILayout.BeginVertical(subGroupStyle);{
						EditorGUILayout.Space();
						EditorGUI.indentLevel += 1;

						player1InputOptions = EditorGUILayout.Foldout(player1InputOptions, "Player 1 Inputs ("+ globalInfo.player1_Inputs.Length +")", foldStyle);
						if (player1InputOptions) globalInfo.player1_Inputs = PlayerInputsBlock(globalInfo.player1_Inputs);
						
						player2InputOptions = EditorGUILayout.Foldout(player2InputOptions, "Player 2 Inputs ("+ globalInfo.player2_Inputs.Length +")", foldStyle);
						if (player2InputOptions) globalInfo.player2_Inputs = PlayerInputsBlock(globalInfo.player2_Inputs);

						EditorGUI.indentLevel -= 1;
						EditorGUILayout.Space();
					}EditorGUILayout.EndVertical();
				}
			}EditorGUILayout.EndVertical();

			
			// Stages
			EditorGUILayout.BeginVertical(rootGroupStyle);{
				EditorGUILayout.BeginHorizontal();{
					stageOptions = EditorGUILayout.Foldout(stageOptions, "Stages ("+ globalInfo.stages.Length +")", foldStyle);
					helpButton("global:stages");
				}EditorGUILayout.EndHorizontal();

				if (stageOptions){
					EditorGUILayout.BeginVertical(subGroupStyle);{
						EditorGUILayout.Space();
						EditorGUI.indentLevel += 1;
						
						for (int i = 0; i < globalInfo.stages.Length; i ++){
							EditorGUILayout.Space();
							EditorGUILayout.BeginVertical(arrayElementStyle);{
								EditorGUILayout.Space();
								EditorGUILayout.BeginHorizontal();{
									globalInfo.stages[i].prefab = (GameObject) EditorGUILayout.ObjectField("Stage Prefab:", globalInfo.stages[i].prefab, typeof(UnityEngine.GameObject), true);
									if (GUILayout.Button("", removeButtonStyle)){
										globalInfo.stages = RemoveElement<StageOptions>(globalInfo.stages, globalInfo.stages[i]);
										return;
									}
								}EditorGUILayout.EndHorizontal();
								globalInfo.stages[i].stageName = EditorGUILayout.TextField("Name:", globalInfo.stages[i].stageName);
								globalInfo.stages[i].music = (AudioClip) EditorGUILayout.ObjectField("Music:", globalInfo.stages[i].music, typeof(UnityEngine.AudioClip), true);
								globalInfo.stages[i].leftBoundary = EditorGUILayout.FloatField("Left Boundary:", globalInfo.stages[i].leftBoundary);
								globalInfo.stages[i].rightBoundary = EditorGUILayout.FloatField("Right Boundary:", globalInfo.stages[i].rightBoundary);
								EditorGUILayout.LabelField("Screenshot:");
								globalInfo.stages[i].screenshot = (Texture2D) EditorGUILayout.ObjectField(globalInfo.stages[i].screenshot, typeof(Texture2D), false);

								EditorGUILayout.Space();
							}EditorGUILayout.EndVertical();
							EditorGUILayout.Space();
						}
						EditorGUI.indentLevel -= 1;
						
						if (StyledButton("New Stage"))
							globalInfo.stages = AddElement<StageOptions>(globalInfo.stages, new StageOptions());
						
					}EditorGUILayout.EndVertical();
				}
			}EditorGUILayout.EndVertical();


			// Characters
			EditorGUILayout.BeginVertical(rootGroupStyle);{
				EditorGUILayout.BeginHorizontal();{
					characterOptions = EditorGUILayout.Foldout(characterOptions, "Characters ("+ globalInfo.characters.Length +")", foldStyle);
					helpButton("global:characters");
				}EditorGUILayout.EndHorizontal();

				if (characterOptions){
					EditorGUILayout.BeginVertical(subGroupStyle);{
						EditorGUILayout.Space();
						EditorGUI.indentLevel += 1;
						
						for (int i = 0; i < globalInfo.characters.Length; i ++){
							EditorGUILayout.Space();
							EditorGUILayout.BeginVertical(arrayElementStyle);{
								EditorGUILayout.Space();
								EditorGUILayout.BeginHorizontal();{
									globalInfo.characters[i] = (CharacterInfo)EditorGUILayout.ObjectField("Character File:", globalInfo.characters[i], typeof(CharacterInfo), false);
									if (GUILayout.Button("", removeButtonStyle)){
										globalInfo.characters = RemoveElement<CharacterInfo>(globalInfo.characters, globalInfo.characters[i]);
										return;
									}
								}EditorGUILayout.EndHorizontal();
								
								if (GUILayout.Button("Open in the Character Editor")) {
									CharacterEditorWindow.sentCharacterInfo = globalInfo.characters[i];
									CharacterEditorWindow.Init();
								}
							}EditorGUILayout.EndVertical();
						}
						EditorGUI.indentLevel -= 1;
						
						EditorGUILayout.Space();
						if (StyledButton("New Character"))
							globalInfo.characters = AddElement<CharacterInfo>(globalInfo.characters, null);
						
					}EditorGUILayout.EndVertical();
				}
			}EditorGUILayout.EndVertical();
			
			// Advanced Options
			EditorGUILayout.BeginVertical(rootGroupStyle);{
				EditorGUILayout.BeginHorizontal();{
					advancedOptions = EditorGUILayout.Foldout(advancedOptions, "Advanced Options", foldStyle);
					helpButton("global:advanced");
				}EditorGUILayout.EndHorizontal();

				if (advancedOptions){
					EditorGUILayout.BeginVertical(subGroupStyle);{
						EditorGUILayout.Space();
						EditorGUI.indentLevel += 1;
						EditorGUIUtility.labelWidth = 180;

						EditorGUILayout.Space();
						globalInfo.animationFlow = (AnimationFlow) EditorGUILayout.EnumPopup("Animation Flow:", globalInfo.animationFlow, enumStyle);
						globalInfo.fps = EditorGUILayout.IntField("Frames Per Second:", globalInfo.fps);
						globalInfo.storedExecutionDelay = EditorGUILayout.FloatField("Execution Delay (seconds):", globalInfo.storedExecutionDelay);
						globalInfo.plinkingDelay = EditorGUILayout.FloatField("Plinking Delay (seconds):", globalInfo.plinkingDelay);
						globalInfo.gameSpeed = EditorGUILayout.Slider("Game Speed:", globalInfo.gameSpeed, .01f, 10);
						globalInfo.gravity = EditorGUILayout.FloatField("Global Gravity:", globalInfo.gravity);
						//globalInfo.detect3D_Hits = EditorGUILayout.Toggle("3D Hit Detection", globalInfo.detect3D_Hits);
						
						EditorGUIUtility.labelWidth = 150;
						EditorGUI.indentLevel -= 1;
						EditorGUILayout.Space();
					}EditorGUILayout.EndVertical();
				}
			}EditorGUILayout.EndVertical();

		}EditorGUILayout.EndScrollView();

		
		if (GUI.changed) {
			Undo.RecordObject(globalInfo, "Global Editor Modify");
			EditorUtility.SetDirty(globalInfo);
		}
	}

	public bool StyledButton (string label) {
		EditorGUILayout.Space();
		GUILayoutUtility.GetRect(1, 20);
		EditorGUILayout.BeginHorizontal();
		GUILayout.FlexibleSpace();
		bool clickResult = GUILayout.Button(label, addButtonStyle);
		GUILayout.FlexibleSpace();
		EditorGUILayout.EndHorizontal();
		EditorGUILayout.Space();
		return clickResult;
	}

	public void HitOptionBlock(string label, HitTypeOptions hit){
		hit.editorToggle = EditorGUILayout.Foldout(hit.editorToggle, label, foldStyle);
		if (hit.editorToggle){
			EditorGUILayout.BeginVertical(subGroupStyle);{
				EditorGUILayout.Space();
				EditorGUI.indentLevel += 1;

				hit.hitParticle = (GameObject) EditorGUILayout.ObjectField("Particle Effect:", hit.hitParticle, typeof(UnityEngine.GameObject), true);
				hit.killTime = EditorGUILayout.FloatField("Effect Kill Time:", hit.killTime);
				hit.hitSound = (AudioClip) EditorGUILayout.ObjectField("Sound Effect:", hit.hitSound, typeof(UnityEngine.AudioClip), true);
				hit.freezingTime = EditorGUILayout.FloatField("Freezing Time:", hit.freezingTime);
				hit.shakeCharacterOnHit = EditorGUILayout.Toggle("Shake Character On Hit", hit.shakeCharacterOnHit);
				hit.shakeCameraOnHit = EditorGUILayout.Toggle("Shake Camera On Hit", hit.shakeCameraOnHit);
				hit.shakeDensity = EditorGUILayout.FloatField("Shake Density:", hit.shakeDensity);

				EditorGUI.indentLevel -= 1;
				EditorGUILayout.Space();
				
			}EditorGUILayout.EndVertical();
		}
	}

	public InputReferences[] PlayerInputsBlock(InputReferences[] inputReferences){
		EditorGUIUtility.labelWidth = 180;
		EditorGUILayout.BeginVertical(subGroupStyle);{
			for (int i = 0; i < inputReferences.Length; i ++){
				EditorGUILayout.Space();
				EditorGUILayout.BeginVertical(arrayElementStyle);{
					EditorGUILayout.Space();
					EditorGUILayout.BeginHorizontal();{
						inputReferences[i].inputType = (InputType) EditorGUILayout.EnumPopup("Input Type:", inputReferences[i].inputType, enumStyle);
						if (GUILayout.Button("", removeButtonStyle)){
							inputReferences = RemoveElement<InputReferences>(inputReferences, inputReferences[i]);
							return inputReferences;
						}
					}EditorGUILayout.EndHorizontal();

					inputReferences[i].inputButtonName = EditorGUILayout.TextField("Input Manager Reference:", inputReferences[i].inputButtonName);
					if (inputReferences[i].inputType == InputType.Button)
						inputReferences[i].engineRelatedButton = (ButtonPress) EditorGUILayout.EnumPopup("UFE Button Reference:", inputReferences[i].engineRelatedButton, enumStyle);

					EditorGUILayout.Space();
				}EditorGUILayout.EndVertical();
			}
			
			if (StyledButton("New Input"))
				inputReferences = AddElement<InputReferences>(inputReferences, new InputReferences());

		}EditorGUILayout.EndVertical();
		EditorGUILayout.Space();
		EditorGUIUtility.labelWidth = 150;

		return inputReferences;
	}

	public void GUIBarBlock(string label, GUIBarOptions guiBar){
		guiBar.editorToggle = EditorGUILayout.Foldout(guiBar.editorToggle, label, foldStyle);
		if (guiBar.editorToggle){
			EditorGUILayout.BeginVertical(subGroupStyle);{
				EditorGUILayout.Space();
				EditorGUI.indentLevel += 1;

				EditorGUILayout.Space();
				EditorGUILayout.LabelField("Background Image:");
				guiBar.backgroundImage = (Texture2D) EditorGUILayout.ObjectField(guiBar.backgroundImage, typeof(Texture2D), false, GUILayout.Width(320), GUILayout.Height(40));
				//guiBar.backgroundColor = EditorGUILayout.ColorField("Color:", guiBar.backgroundColor);
				guiBar.backgroundRect = EditorGUILayout.RectField("Pixel Inset:", guiBar.backgroundRect);
				if (guiBar.backgroundRect.width == 0 && guiBar.backgroundImage != null) guiBar.backgroundRect.width = guiBar.backgroundImage.width;
				if (guiBar.backgroundRect.height == 0 && guiBar.backgroundImage != null) guiBar.backgroundRect.height = guiBar.backgroundImage.height;
				//guiBar.backgroundRect.x = 0;

				EditorGUILayout.Space();
				
				EditorGUILayout.LabelField("Fill Image:");
				guiBar.fillImage = (Texture2D) EditorGUILayout.ObjectField(guiBar.fillImage, typeof(Texture2D), false, GUILayout.Width(320), GUILayout.Height(36));
				//guiBar.fillColor = EditorGUILayout.ColorField("Color:", guiBar.fillColor);
				guiBar.fillRect = EditorGUILayout.RectField("Pixel Inset (relative):", guiBar.fillRect);
				if (guiBar.fillRect.width == 0 && guiBar.fillImage != null) guiBar.fillRect.width = guiBar.fillImage.width;
				if (guiBar.fillRect.height == 0 && guiBar.fillImage != null) guiBar.fillRect.height = guiBar.fillImage.height;
				
				EditorGUILayout.Space();

				if (guiBar.previewToggle){
					if (guiBar.bgPreview == null){
						guiBar.bgPreview = new GameObject(label + " Background");
						guiBar.bgPreview.transform.position = Vector3.zero;
						guiBar.bgPreview.transform.localScale = Vector3.zero;
						guiBar.bgPreview.AddComponent<GUITexture>();


						guiBar.fillPreview = new GameObject(label + " Fill");
						guiBar.fillPreview.transform.position = Vector3.forward;
						guiBar.fillPreview.transform.localScale = Vector3.zero;
						guiBar.fillPreview.AddComponent<GUITexture>();
					}

					guiBar.bgPreview.GetComponent<GUITexture>().pixelInset = new Rect(0, 0, guiBar.backgroundRect.width, guiBar.backgroundRect.height);
					guiBar.bgPreview.GetComponent<GUITexture>().texture = guiBar.backgroundImage;
					guiBar.fillPreview.GetComponent<GUITexture>().pixelInset = guiBar.fillRect;
					guiBar.fillPreview.GetComponent<GUITexture>().texture = guiBar.fillImage;
					if (StyledButton("Close Preview")) {
						Clear();
						guiBar.previewToggle = false;
					}
				}else{
					if (StyledButton("Preview")) guiBar.previewToggle = true;
				}

				EditorGUI.indentLevel -= 1;
				EditorGUILayout.Space();
				
			}EditorGUILayout.EndVertical();
		}
	}


	public T[] RemoveElement<T> (T[] elements, T element) {
		List<T> elementsList = new List<T>(elements);
		elementsList.Remove(element);
		return elementsList.ToArray();
	}
	
	public T[] AddElement<T> (T[] elements, T element) {
		List<T> elementsList = new List<T>(elements);
		elementsList.Add(element);
		return elementsList.ToArray();
	}
}