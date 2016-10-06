using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Reflection;
using System.Collections.Generic;

public class MoveEditorWindow : EditorWindow {
	public static MoveEditorWindow moveEditorWindow;
	public static MoveInfo sentMoveInfo;
	
	private MoveInfo moveInfo;
	private int fpsTemp;
	private Vector2 scrollPos;
	
	private GameObject character;
	private float animationSpeedTemp;
	private int totalFramesTemp;
	private float animFrame;
	private float animTime;
	private bool animIsPlaying;
	private bool smoothPreview;

	private float camTime;
	private float camStart;
	public Vector3 initialCamPosition;
	public Quaternion initialCamRotation;
	public float initialFieldOfView;

	
	private bool possibleStancesToggle;
	private bool buttonSequenceToggle;
	private bool buttonExecutionToggle;
	
	private bool previousMovesToggle;
	private bool nextMovesToggle;
	private bool blockableAreaToggle;
	private bool hitStunToggle;
	private bool damageToggle;
	private bool forceToggle;
	private bool selfForceToggle;
	private bool opponentForceToggle;
	private bool pullEnemyInToggle;
	private bool pullSelfInToggle;
	private bool hitsToggle;
	private bool hurtBoxToggle;
	private bool bodyPartsToggle;
	
	
	private bool generalOptions;
	private bool animationOptions;
	private bool inputOptions;
	private bool moveLinksOptions;
	private bool particleEffectsOptions;
	private bool selfAppliedForceOptions;
	private bool soundOptions;
	private bool cameraOptions;
	private bool pullInOptions;
	private bool activeFramesOptions;
	private bool invincibleFramesOptions;
	private bool projectileOptions;

	
	private bool characterWarning;
	private string errorMsg;
	
	private string titleStyle;
	private string removeButtonStyle;
	private string addButtonStyle;
	private string borderBarStyle;
	private string rootGroupStyle;
	private string subGroupStyle;
	private string arrayElementStyle;
	private string subArrayElementStyle;
	private string toggleStyle;
	private string foldStyle;
	private string enumStyle;
	private string fillBarStyle1;
	private string fillBarStyle2;
	private string fillBarStyle3;
	private string fillBarStyle4;
	private GUIStyle labelStyle;

	
	[MenuItem("Window/U.F.E./Move Editor")]
	public static void Init(){
		moveEditorWindow = EditorWindow.GetWindow<MoveEditorWindow>(false, "Move", true);
		moveEditorWindow.Show();
		EditorWindow.FocusWindowIfItsOpen<SceneView>();
		Camera sceneCam = GameObject.FindObjectOfType<Camera>();
		if (sceneCam != null){
			moveEditorWindow.initialFieldOfView = sceneCam.fieldOfView;
			moveEditorWindow.initialCamPosition = sceneCam.transform.position;
			moveEditorWindow.initialCamRotation = sceneCam.transform.rotation;
		}
		moveEditorWindow.Populate();
	}
	
	void OnSelectionChange(){
		Populate();
		Repaint();
		Clear(false, false);
	}
	
	void OnEnable(){
		Populate();
	}
	
	void OnFocus(){
		Populate();
	}

	void OnDisable(){
		Clear(true, true);
	}

	void OnDestroy(){
		Clear(true, true);
	}

	void OnLostFocus(){
		Clear(true, false);
	}

	void Clear(bool destroyChar, bool resetCam){
		if (destroyChar){
			if (character != null) {
				Editor.DestroyImmediate(character);
				character = null;
			}
		}
		if (resetCam){
			Camera sceneCam = GameObject.FindObjectOfType<Camera>();
			if (sceneCam != null){
				sceneCam.fieldOfView = initialFieldOfView;
				sceneCam.transform.position = initialCamPosition;
				sceneCam.transform.rotation = sceneCam.transform.rotation;
			}
		}
	}
	
	void helpButton(string page){
		if (GUILayout.Button("?", GUILayout.Width(18), GUILayout.Height(18))) 
			Application.OpenURL("http://www.ufe3d.com/doku.php/"+ page);
	}

	void Update(){
		if (EditorApplication.isPlayingOrWillChangePlaymode && character != null) {
			Editor.DestroyImmediate(character);
			character = null;
		}
	}

	void Populate(){
		//initialFieldOfView = 16;
		//initialCamPosition = new Vector3(0,8,-34);
		//initialCamRotation = Quaternion.Euler(new Vector3(6,0,0));
		camTime = 0;

		// Style Definitions
		titleStyle = "MeTransOffRight";
		removeButtonStyle = "TL SelectionBarCloseButton";
		addButtonStyle = "CN CountBadge";
		rootGroupStyle = "GroupBox";
		subGroupStyle = "ObjectFieldThumb";
		arrayElementStyle = "flow overlay box";
		subArrayElementStyle = "HelpBox";
		foldStyle = "Foldout";
		enumStyle = "MiniPopup";
		toggleStyle = "BoldToggle";
		borderBarStyle = "ProgressBarBack";
		fillBarStyle1 = "ProgressBarBar";
		fillBarStyle2 = "flow node 2 on";
		fillBarStyle3 = "flow node 4 on";
		fillBarStyle4 = "flow node 6 on";

		labelStyle = new GUIStyle();
		labelStyle.alignment = TextAnchor.MiddleCenter;
		labelStyle.fontStyle = FontStyle.Bold;
		labelStyle.normal.textColor = Color.white;

		if (sentMoveInfo != null){
			EditorGUIUtility.PingObject( sentMoveInfo );
			Selection.activeObject = sentMoveInfo;
			sentMoveInfo = null;
		}

		Object[] selection = Selection.GetFiltered(typeof(MoveInfo), SelectionMode.Assets);
		if (selection.Length > 0){
			if (selection[0] == null) return;
			moveInfo = (MoveInfo) selection[0];
			fpsTemp = moveInfo.fps;
			animationSpeedTemp = moveInfo.animationSpeed;
			totalFramesTemp = moveInfo.totalFrames;
		}
	}
	
	public void OnGUI(){
		/*GUI.skin = (GUISkin) EditorGUIUtility.Load("EditorSkin.guiskin");
		moveInfoSO.Update();
		
        skin = EditorGUILayout.ObjectField(skin, typeof(GUISkin)) as GUISkin;
        if(skin == null) GUI.enabled = false;

        if(GUILayout.Button("Copy Editor Skin")) {
            GUISkin builtinSkin = EditorGUIUtility.GetBuiltinSkin(EditorSkin.Inspector);
            PropertyInfo[] properties = typeof(GUISkin).GetProperties();
            foreach(PropertyInfo property in properties) {
                if(property.PropertyType == typeof(GUIStyle)) {
                    property.SetValue(skin, property.GetValue(builtinSkin, null), null);
                }
            }
        }
		
		EditorStyles.label.normal.textColor = Color.white;
		EditorStyles.foldout.normal.textColor = Color.white;
		GUI.skin.label.normal.textColor = Color.white;
		GUI.skin.label.alignment = TextAnchor.MiddleCenter;*/
		

		if (moveInfo == null){
			GUILayout.BeginHorizontal("GroupBox");
			GUILayout.Label("Select a move file first or create a new move.","CN EntryInfo");
			GUILayout.EndHorizontal();
			EditorGUILayout.Space();
			if (GUILayout.Button("Create new move"))
				ScriptableObjectUtility.CreateAsset<MoveInfo> ();
			return;
		}

		//EditorGUIUtility.labelWidth = 150;
		GUIStyle fontStyle = new GUIStyle();
		fontStyle.font = (Font) EditorGUIUtility.Load("EditorFont.TTF");
		fontStyle.fontSize = 30;
		fontStyle.alignment = TextAnchor.UpperCenter;
		fontStyle.normal.textColor = Color.white;
		fontStyle.hover.textColor = Color.white;
		EditorGUILayout.BeginVertical(titleStyle);{
			EditorGUILayout.BeginHorizontal();{
				EditorGUILayout.LabelField("", (moveInfo.moveName == ""? "New Move":moveInfo.moveName) , fontStyle, GUILayout.Height(32));
				helpButton("move:start");
			}EditorGUILayout.EndHorizontal();
		}EditorGUILayout.EndVertical();

		if (moveInfo.animationClip != null){
			moveInfo.totalFrames = (int)Mathf.Abs(Mathf.Floor((moveInfo.fps * moveInfo.animationClip.length) / moveInfo.animationSpeed));
			totalFramesTemp = (int)Mathf.Abs(Mathf.Floor((moveInfo.fps * moveInfo.animationClip.length) / animationSpeedTemp));
		}

		scrollPos = EditorGUILayout.BeginScrollView(scrollPos);{
			EditorGUILayout.BeginVertical(rootGroupStyle);{
				// Begin General Options
				EditorGUILayout.BeginHorizontal();{
					generalOptions = EditorGUILayout.Foldout(generalOptions, "General", foldStyle);
					helpButton("move:general");
				}EditorGUILayout.EndHorizontal();

				if (generalOptions){
					EditorGUILayout.BeginVertical(subGroupStyle);{
						EditorGUILayout.Space();
						EditorGUI.indentLevel += 1;
						
						EditorGUIUtility.labelWidth = 180;

						moveInfo.moveName = EditorGUILayout.TextField("Move Name:", moveInfo.moveName);
						moveInfo.description = EditorGUILayout.TextField("Move Description:", moveInfo.description);
						EditorGUILayout.BeginHorizontal();{
							string unsaved = fpsTemp != moveInfo.fps ? "*":"";
							fpsTemp = EditorGUILayout.IntSlider("FPS Architecture:"+ unsaved, fpsTemp, 10, 120);
							if (StyledButton("Apply")) moveInfo.fps = fpsTemp;

						}EditorGUILayout.EndHorizontal();
						moveInfo.attackType = (AttackType)EditorGUILayout.EnumPopup("Attack Type: ",moveInfo.attackType, enumStyle);
						moveInfo.armor = Mathf.Max(0,EditorGUILayout.IntField("Armor:", moveInfo.armor));
						moveInfo.changeCombatStance = (CombatStances)EditorGUILayout.EnumPopup("Combat Stance:", moveInfo.changeCombatStance, enumStyle);
						moveInfo.ignoreGravity = EditorGUILayout.Toggle("Ignore Gravity", moveInfo.ignoreGravity, toggleStyle);
						moveInfo.cancelMoveWheLanding = EditorGUILayout.Toggle("Cancel Move Whe Landing", moveInfo.cancelMoveWheLanding, toggleStyle);
						
						EditorGUIUtility.labelWidth = 150;

						moveInfo.useGauge = EditorGUILayout.Foldout(moveInfo.useGauge, "Gauge Options", foldStyle);
						if (moveInfo.useGauge){
							EditorGUILayout.BeginVertical(subGroupStyle);{
								EditorGUILayout.Space();
								EditorGUI.indentLevel += 1;
								moveInfo.gaugeGainOnHit = StyledSlider("Gauge Gain on Hit", moveInfo.gaugeGainOnHit, EditorGUI.indentLevel, 0, 100);
								moveInfo.gaugeGainOnMiss = StyledSlider("Gauge Gain on Miss", moveInfo.gaugeGainOnMiss, EditorGUI.indentLevel, 0, 100);
								moveInfo.gaugeGainOnBlock = StyledSlider("Gauge Gain on Block", moveInfo.gaugeGainOnBlock, EditorGUI.indentLevel, 0, 100);
								moveInfo.opGaugeGainOnBlock = StyledSlider("Op. Gauge Gain on Block", moveInfo.opGaugeGainOnBlock, EditorGUI.indentLevel, 0, 100);
								moveInfo.opGaugeGainOnParry = StyledSlider("Op. Gauge Gain on Parry", moveInfo.opGaugeGainOnParry, EditorGUI.indentLevel, 0, 100);
								moveInfo.gaugeUsage = StyledSlider("Gauge Required", moveInfo.gaugeUsage, EditorGUI.indentLevel, 0, 100);
								EditorGUI.indentLevel -= 1;
								EditorGUILayout.Space();
							}EditorGUILayout.EndVertical();
						}
						
						// Possible States
						possibleStancesToggle = EditorGUILayout.Foldout(possibleStancesToggle, "Possible States ("+ moveInfo.possibleStates.Length +")", EditorStyles.foldout);
						if (possibleStancesToggle){
							EditorGUILayout.BeginVertical(subGroupStyle);{
								EditorGUI.indentLevel += 1;
								for (int i = 0; i < moveInfo.possibleStates.Length; i ++){
									EditorGUILayout.Space();
									EditorGUILayout.BeginVertical(arrayElementStyle);{
										EditorGUILayout.Space();
										EditorGUILayout.BeginHorizontal();{
											moveInfo.possibleStates[i] = (PossibleStates)EditorGUILayout.EnumPopup("State:", moveInfo.possibleStates[i], enumStyle);
											if (GUILayout.Button("", removeButtonStyle)){
												moveInfo.possibleStates = RemoveElement<PossibleStates>(moveInfo.possibleStates, moveInfo.possibleStates[i]);
												return;
											}
										}EditorGUILayout.EndHorizontal();
										EditorGUILayout.Space();
									}EditorGUILayout.EndVertical();
								}
								if (StyledButton("New Possible State"))
									moveInfo.possibleStates = AddElement<PossibleStates>(moveInfo.possibleStates, PossibleStates.Stand);

								EditorGUI.indentLevel -= 1;
							}EditorGUILayout.EndVertical();
						}
						EditorGUI.indentLevel -= 1;

					}EditorGUILayout.EndVertical();
				}
				//GUILayout.Box("", "TimeScrubber", ", new GUILayoutOption[]{GUILayout.ExpandWidth(true), GUILayout.Height(1)});
				// End General Options

			}EditorGUILayout.EndVertical();
			EditorGUILayout.BeginVertical(rootGroupStyle);{
				// Begin Animation Options
				EditorGUILayout.BeginHorizontal();{
					animationOptions = EditorGUILayout.Foldout(animationOptions, "Animation", foldStyle);
					helpButton("move:animation");
				}EditorGUILayout.EndHorizontal();

				if (animationOptions){
					EditorGUILayout.BeginVertical(subGroupStyle);{
						EditorGUILayout.Space();
						EditorGUI.indentLevel += 1;
						moveInfo.animationClip = (AnimationClip) EditorGUILayout.ObjectField("Animation Clip:", moveInfo.animationClip, typeof(UnityEngine.AnimationClip), true);
						if (moveInfo.animationClip != null){
							moveInfo.wrapMode = (WrapMode)EditorGUILayout.EnumPopup("Wrap Mode: ",moveInfo.wrapMode, enumStyle);
							moveInfo.interpolationSpeed = EditorGUILayout.Slider("Interpolation Speed:", moveInfo.interpolationSpeed, 0, 1);

							EditorGUI.indentLevel += 1;
							EditorGUILayout.Space();
							string unsaved = animationSpeedTemp != moveInfo.animationSpeed ? "*":"";
							animationSpeedTemp = StyledSlider("Animation Speed"+ unsaved, animationSpeedTemp, EditorGUI.indentLevel, -5, 5);
							EditorGUILayout.BeginHorizontal();{
								unsaved = totalFramesTemp != moveInfo.totalFrames ? "*":"";
								EditorGUILayout.LabelField("Total frames:"+ unsaved, totalFramesTemp.ToString());
								if (StyledButton("Apply")) moveInfo.animationSpeed = animationSpeedTemp;
							}EditorGUILayout.EndHorizontal();
							EditorGUILayout.Space();
							EditorGUILayout.Space();

							EditorGUILayout.BeginVertical(subGroupStyle);{
								EditorGUILayout.Space();
								GameObject newCharacterPrefab = (GameObject) EditorGUILayout.ObjectField("Character Prefab:", moveInfo.characterPrefab, typeof(UnityEngine.GameObject), true);
								if (newCharacterPrefab != null && moveInfo.characterPrefab != newCharacterPrefab && !EditorApplication.isPlayingOrWillChangePlaymode){
									if (PrefabUtility.GetPrefabType(newCharacterPrefab) != PrefabType.Prefab){
										characterWarning = true;
										errorMsg = "This character is not a prefab.";
									}else if (newCharacterPrefab.GetComponent<HitBoxesScript>() == null){
										characterWarning = true;
										errorMsg = "This character doesn't have hitboxes!\n Please add the HitboxScript and try again.";
									}else{
										characterWarning = false;
										moveInfo.characterPrefab = newCharacterPrefab;
									}
								}else if (moveInfo.characterPrefab != newCharacterPrefab && EditorApplication.isPlayingOrWillChangePlaymode){
									characterWarning = true;
									errorMsg = "You can't change this field while in play mode.";
								}else if (newCharacterPrefab == null) moveInfo.characterPrefab = null;

								if (character == null){
									if (StyledButton("Animation Preview")){
										if (moveInfo.characterPrefab == null) {
											characterWarning = true;
											errorMsg = "Drag a character into 'Character Prefab' first.";
										}else if (EditorApplication.isPlayingOrWillChangePlaymode){
											characterWarning = true;
											errorMsg = "You can't preview animations while in play mode.";
										}else{
											characterWarning = false;
											EditorCamera.SetPosition(Vector3.up * 4);
											EditorCamera.SetRotation(Quaternion.identity);
											EditorCamera.SetOrthographic(true);
											EditorCamera.SetSize(10);

											character = (GameObject) PrefabUtility.InstantiatePrefab(moveInfo.characterPrefab);
											character.transform.position = new Vector3(-2,0,0);
										}
									}

									if (characterWarning){
										GUILayout.BeginHorizontal("GroupBox");
										GUILayout.Label(errorMsg,"CN EntryWarn");
										GUILayout.EndHorizontal();
									}
								}else {
									if (smoothPreview){
										animFrame = StyledSlider("Animation Frames", animFrame, EditorGUI.indentLevel, 0, moveInfo.totalFrames);
									}else{
										animFrame = StyledSlider("Animation Frames", (int)animFrame, EditorGUI.indentLevel, 0, moveInfo.totalFrames);
									}
									
									if (cameraOptions){
										GUILayout.BeginHorizontal("GroupBox");
										GUILayout.Label("You must close 'Camera Preview' first.","CN EntryError");
										GUILayout.EndHorizontal();
									}

									smoothPreview = EditorGUILayout.Toggle("Smooth Preview", smoothPreview, toggleStyle);
									AnimationSampler();
									
									EditorGUILayout.Space();
									
									EditorGUILayout.BeginHorizontal();{
										if (StyledButton("Reset Scene View")){
											EditorCamera.SetPosition(Vector3.up * 4);
											EditorCamera.SetRotation(Quaternion.identity);
											EditorCamera.SetOrthographic(true);
											EditorCamera.SetSize(10);
										}
										if (StyledButton("Close Preview")) Clear (true, true);
									}EditorGUILayout.EndHorizontal();
									
									EditorGUILayout.Space();
								}
							}EditorGUILayout.EndVertical();
							EditorGUI.indentLevel -= 1;
						}
						EditorGUI.indentLevel -= 1;

					}EditorGUILayout.EndVertical();
				}else if (character != null && !cameraOptions){
					Clear (true, true);
				}
				//GUILayout.Box("", "TimeScrubber", new GUILayoutOption[]{GUILayout.ExpandWidth(true), GUILayout.Height(1)});
				// End Animation Options
				
				
			}EditorGUILayout.EndVertical();
			EditorGUILayout.BeginVertical(rootGroupStyle);{
				// Begin Input Options
				EditorGUILayout.BeginHorizontal();{
					inputOptions = EditorGUILayout.Foldout(inputOptions, "Input", EditorStyles.foldout);
					helpButton("move:input");
				}EditorGUILayout.EndHorizontal();

				if (inputOptions){
					EditorGUILayout.BeginVertical(subGroupStyle);{
						EditorGUI.indentLevel += 1;
						moveInfo.chargeMove = EditorGUILayout.Toggle("Charge Move", moveInfo.chargeMove, toggleStyle);
						
						// Button Sequence
						buttonSequenceToggle = EditorGUILayout.Foldout(buttonSequenceToggle, "Button Sequences ("+ moveInfo.buttonSequence.Length +")", EditorStyles.foldout);
						if (buttonSequenceToggle){
							EditorGUILayout.BeginVertical(subGroupStyle);{
								EditorGUI.indentLevel += 1;
								for (int i = 0; i < moveInfo.buttonSequence.Length; i ++){
									EditorGUILayout.Space();
									EditorGUILayout.BeginVertical(arrayElementStyle);{
										EditorGUILayout.Space();
										EditorGUILayout.BeginHorizontal();{
											moveInfo.buttonSequence[i] = (ButtonPress)EditorGUILayout.EnumPopup("Button:", moveInfo.buttonSequence[i], enumStyle);
											if (GUILayout.Button("", removeButtonStyle)){
												moveInfo.buttonSequence = RemoveElement<ButtonPress>(moveInfo.buttonSequence, moveInfo.buttonSequence[i]);
												return;
											}
										}EditorGUILayout.EndHorizontal();
										EditorGUILayout.Space();
									}EditorGUILayout.EndVertical();
								}
								EditorGUILayout.Space();
								if (StyledButton("New Button Sequence"))
									moveInfo.buttonSequence = AddElement<ButtonPress>(moveInfo.buttonSequence, ButtonPress.Foward);
								
								EditorGUI.indentLevel -= 1;
							}EditorGUILayout.EndVertical();
						}
						
						// Button Execution
						buttonExecutionToggle = EditorGUILayout.Foldout(buttonExecutionToggle, "Button Executions ("+ moveInfo.buttonExecution.Length +")", EditorStyles.foldout);
						if (buttonExecutionToggle){
							EditorGUILayout.BeginVertical(subGroupStyle);{
								EditorGUI.indentLevel += 1;
								moveInfo.onRelease = EditorGUILayout.Toggle("On Button Release", moveInfo.onRelease, toggleStyle);
								for (int i = 0; i < moveInfo.buttonExecution.Length; i ++){
									EditorGUILayout.Space();
									EditorGUILayout.BeginVertical(arrayElementStyle);{
										EditorGUILayout.Space();
										EditorGUILayout.BeginHorizontal();{
											moveInfo.buttonExecution[i] = (ButtonPress)EditorGUILayout.EnumPopup("Button:", moveInfo.buttonExecution[i], enumStyle);
											if (GUILayout.Button("", removeButtonStyle)){
												moveInfo.buttonExecution = RemoveElement<ButtonPress>(moveInfo.buttonExecution, moveInfo.buttonExecution[i]);
												return;
											}
										}EditorGUILayout.EndHorizontal();
										EditorGUILayout.Space();
									}EditorGUILayout.EndHorizontal();
								}
								EditorGUILayout.Space();
								if (StyledButton("New Button Execution"))
									moveInfo.buttonExecution = AddElement<ButtonPress>(moveInfo.buttonExecution, ButtonPress.Button1);

								EditorGUI.indentLevel -= 1;
							}EditorGUILayout.EndVertical();
						}
						EditorGUI.indentLevel -= 1;
					}EditorGUILayout.EndVertical();
				}
				//GUILayout.Box("", "TimeScrubber", new GUILayoutOption[]{GUILayout.ExpandWidth(true), GUILayout.Height(1)});
				// End Input Options
				
				
			}EditorGUILayout.EndVertical();
			EditorGUILayout.BeginVertical(rootGroupStyle);{
				// Begin Move Link Options
				EditorGUILayout.BeginHorizontal();{
					moveLinksOptions = EditorGUILayout.Foldout(moveLinksOptions, "Chain Moves", EditorStyles.foldout);
					helpButton("move:chainmoves");
				}EditorGUILayout.EndHorizontal();

				if (moveLinksOptions){
					EditorGUILayout.BeginVertical(subGroupStyle);{
						EditorGUI.indentLevel += 1;
						
						// Previous Moves
						previousMovesToggle = EditorGUILayout.Foldout(previousMovesToggle, "Required Moves ("+ moveInfo.previousMoves.Length +")", EditorStyles.foldout);
						if (previousMovesToggle){
							EditorGUILayout.BeginVertical(subGroupStyle);{
								EditorGUI.indentLevel += 1;
								for (int i = 0; i < moveInfo.previousMoves.Length; i ++){
									EditorGUILayout.Space();
									EditorGUILayout.BeginVertical(arrayElementStyle);{
										EditorGUILayout.Space();
										EditorGUILayout.BeginHorizontal();{
											moveInfo.previousMoves[i] = (MoveInfo) EditorGUILayout.ObjectField("Move:", moveInfo.previousMoves[i], typeof(MoveInfo), false);
											if (GUILayout.Button("", removeButtonStyle)){
												moveInfo.previousMoves = RemoveElement<MoveInfo>(moveInfo.previousMoves, moveInfo.previousMoves[i]);
												return;
											}
										}EditorGUILayout.EndHorizontal();
										EditorGUILayout.Space();
									}EditorGUILayout.EndHorizontal();
								}
								EditorGUILayout.Space();
								if (StyledButton("New Required Move"))
									moveInfo.previousMoves = AddElement<MoveInfo>(moveInfo.previousMoves, null);
								
								EditorGUILayout.Space();
								EditorGUI.indentLevel -= 1;

							}EditorGUILayout.EndVertical();
						}
						
						
						// Begin Frame Links Options
						nextMovesToggle = EditorGUILayout.Foldout(nextMovesToggle, "Move Links ("+ moveInfo.frameLink.linkableMoves.Length +")", EditorStyles.foldout);
						if (nextMovesToggle){
							EditorGUILayout.BeginVertical(subGroupStyle);{
								EditorGUI.indentLevel += 1;
								if (moveInfo.frameLink.linkableMoves.Length > 0){
									moveInfo.frameLink.onlyOnHit = EditorGUILayout.Toggle("Only on hit", moveInfo.frameLink.onlyOnHit, toggleStyle);
									StyledMinMaxSlider("Frame Links", ref moveInfo.frameLink.activeFramesBegins, ref moveInfo.frameLink.activeFramesEnds, 1, moveInfo.totalFrames, EditorGUI.indentLevel);
								
									EditorGUILayout.LabelField("Can link into:");
									for (int i = 0; i < moveInfo.frameLink.linkableMoves.Length; i ++){
										EditorGUILayout.Space();
										EditorGUILayout.BeginVertical(arrayElementStyle);{
											EditorGUILayout.Space();
											EditorGUILayout.BeginHorizontal();{
												moveInfo.frameLink.linkableMoves[i] = (MoveInfo) EditorGUILayout.ObjectField("Move:", moveInfo.frameLink.linkableMoves[i], typeof(MoveInfo), false);
												if (GUILayout.Button("", removeButtonStyle)){
													moveInfo.frameLink.linkableMoves = RemoveElement<MoveInfo>(moveInfo.frameLink.linkableMoves, moveInfo.frameLink.linkableMoves[i]);
													return;
												}
											}EditorGUILayout.EndHorizontal();
											EditorGUILayout.Space();
										}EditorGUILayout.EndVertical();
									}
									EditorGUILayout.Space();
								}
								if (StyledButton("New Move"))
									moveInfo.frameLink.linkableMoves = AddElement<MoveInfo>(moveInfo.frameLink.linkableMoves, null);
								
								EditorGUILayout.Space();
								EditorGUI.indentLevel -= 1;

							}EditorGUILayout.EndVertical();
						}
						EditorGUI.indentLevel -= 1;

					}EditorGUILayout.EndVertical();
				}
				//GUILayout.Box("", "TimeScrubber", new GUILayoutOption[]{GUILayout.ExpandWidth(true), GUILayout.Height(1)});
				// End Move Link Options
				
				
			}EditorGUILayout.EndVertical();
			EditorGUILayout.BeginVertical(rootGroupStyle);{
				// Begin Particle Effects Options
				EditorGUILayout.BeginHorizontal();{
					particleEffectsOptions = EditorGUILayout.Foldout(particleEffectsOptions, "Particle Effects ("+ moveInfo.particleEffects.Length +")", EditorStyles.foldout);
					helpButton("move:particleeffects");
				}EditorGUILayout.EndHorizontal();

				if (particleEffectsOptions){
					EditorGUILayout.BeginVertical(subGroupStyle);{
						EditorGUI.indentLevel += 1;
						List<int> castingValues = new List<int>();
						foreach(MoveParticleEffect particleEffect in moveInfo.particleEffects) 
							castingValues.Add(particleEffect.castingFrame);
						StyledMarker("Casting Timeline", castingValues.ToArray(), moveInfo.totalFrames, EditorGUI.indentLevel);
							
						for (int i = 0; i < moveInfo.particleEffects.Length; i ++){
							EditorGUILayout.Space();
							EditorGUILayout.BeginVertical(arrayElementStyle);{
								EditorGUILayout.Space();
								EditorGUILayout.BeginHorizontal();{
									moveInfo.particleEffects[i].castingFrame = EditorGUILayout.IntSlider("Casting Frame:", moveInfo.particleEffects[i].castingFrame, 0, moveInfo.totalFrames);
									if (GUILayout.Button("", removeButtonStyle)){
										moveInfo.particleEffects = RemoveElement<MoveParticleEffect>(moveInfo.particleEffects, moveInfo.particleEffects[i]);
										return;
									}
								}EditorGUILayout.EndHorizontal();
								if (moveInfo.particleEffects[i].particleEffect == null) moveInfo.particleEffects[i].particleEffect = new ParticleInfo();
								moveInfo.particleEffects[i].particleEffect.prefab = (GameObject) EditorGUILayout.ObjectField("Particle Effect:", moveInfo.particleEffects[i].particleEffect.prefab, typeof(UnityEngine.GameObject), true);
								moveInfo.particleEffects[i].particleEffect.duration = EditorGUILayout.FloatField("Duration (seconds):", moveInfo.particleEffects[i].particleEffect.duration);
								moveInfo.particleEffects[i].particleEffect.position = EditorGUILayout.Vector3Field("Position (relative):", moveInfo.particleEffects[i].particleEffect.position);
								EditorGUILayout.Space();
							}EditorGUILayout.EndVertical();
						}
						if (StyledButton("New Particle Effect"))
							moveInfo.particleEffects = AddElement<MoveParticleEffect>(moveInfo.particleEffects, new MoveParticleEffect());
							
						EditorGUI.indentLevel -= 1;

					}EditorGUILayout.EndVertical();
				}
				//GUILayout.Box("", "TimeScrubber", new GUILayoutOption[]{GUILayout.ExpandWidth(true), GUILayout.Height(1)});
				// End Particle Effects Options
				
				
			}EditorGUILayout.EndVertical();
			EditorGUILayout.BeginVertical(rootGroupStyle);{
				// Begin Sound Options
				EditorGUILayout.BeginHorizontal();{
					soundOptions = EditorGUILayout.Foldout(soundOptions, "Sound Effects ("+ moveInfo.soundEffects.Length +")", EditorStyles.foldout);
					helpButton("move:soundeffects");
				}EditorGUILayout.EndHorizontal();

				if (soundOptions){
					EditorGUILayout.BeginVertical(subGroupStyle);{
						EditorGUI.indentLevel += 1;
						List<int> castingValues = new List<int>();
						foreach(SoundEffect soundEffect in moveInfo.soundEffects) 
							castingValues.Add(soundEffect.castingFrame);
						StyledMarker("Casting Timeline", castingValues.ToArray(), moveInfo.totalFrames, EditorGUI.indentLevel);
							
						for (int i = 0; i < moveInfo.soundEffects.Length; i ++){
							EditorGUILayout.Space();
							EditorGUILayout.BeginVertical(arrayElementStyle);{
								EditorGUILayout.Space();
								EditorGUILayout.BeginHorizontal();{
									moveInfo.soundEffects[i].castingFrame = EditorGUILayout.IntSlider("Casting Frame:", moveInfo.soundEffects[i].castingFrame, 0, moveInfo.totalFrames);
									if (GUILayout.Button("", removeButtonStyle)){
										moveInfo.soundEffects = RemoveElement<SoundEffect>(moveInfo.soundEffects, moveInfo.soundEffects[i]);
										return;
									}
								}EditorGUILayout.EndHorizontal();
								moveInfo.soundEffects[i].sound = (AudioClip) EditorGUILayout.ObjectField("Audio Clip:", moveInfo.soundEffects[i].sound, typeof(UnityEngine.AudioClip), true);
								EditorGUILayout.Space();
							}EditorGUILayout.EndVertical();
						}
						if (StyledButton("New Sound Effect"))
							moveInfo.soundEffects = AddElement<SoundEffect>(moveInfo.soundEffects, new SoundEffect());

						EditorGUI.indentLevel -= 1;
						
					}EditorGUILayout.EndVertical();
				}
				//GUILayout.Box("", "TimeScrubber", new GUILayoutOption[]{GUILayout.ExpandWidth(true), GUILayout.Height(1)});
				// End Sound Options
				
				
			}EditorGUILayout.EndVertical();
			EditorGUILayout.BeginVertical(rootGroupStyle);{
				// Begin Camera Options
				EditorGUILayout.BeginHorizontal();{
					cameraOptions = EditorGUILayout.Foldout(cameraOptions, "Cinematic Options ("+ moveInfo.cameraMovements.Length +")", EditorStyles.foldout);
					helpButton("move:cinematics");
				}EditorGUILayout.EndHorizontal();

				if (cameraOptions){
					EditorGUILayout.BeginVertical(subGroupStyle);{
						EditorGUI.indentLevel += 1;
						List<int> castingValues = new List<int>();
						foreach(CameraMovement cameraMovement in moveInfo.cameraMovements) 
							castingValues.Add(cameraMovement.castingFrame);
						StyledMarker("Casting Timeline", castingValues.ToArray(), moveInfo.totalFrames, EditorGUI.indentLevel);
						
						for (int i = 0; i < moveInfo.cameraMovements.Length; i ++){
							EditorGUILayout.Space();
							EditorGUILayout.BeginVertical(arrayElementStyle);{
								EditorGUILayout.Space();
								EditorGUILayout.BeginHorizontal();{
									moveInfo.cameraMovements[i].castingFrame = EditorGUILayout.IntSlider("Casting Frame:", moveInfo.cameraMovements[i].castingFrame, 0, moveInfo.totalFrames);
									if (GUILayout.Button("", removeButtonStyle)){
										moveInfo.cameraMovements = RemoveElement<CameraMovement>(moveInfo.cameraMovements, moveInfo.cameraMovements[i]);
										return;
									}
								}EditorGUILayout.EndHorizontal();
								moveInfo.cameraMovements[i].duration = Mathf.Clamp(EditorGUILayout.FloatField("Duration (seconds):", moveInfo.cameraMovements[i].duration), .1f, 99999);
								moveInfo.cameraMovements[i].camSpeed = EditorGUILayout.Slider("Movement Speed:", moveInfo.cameraMovements[i].camSpeed, 1, 100);
								moveInfo.cameraMovements[i].freezeAnimation = EditorGUILayout.Toggle("Freeze Animation", moveInfo.cameraMovements[i].freezeAnimation, toggleStyle);
								moveInfo.cameraMovements[i].freezeGame = EditorGUILayout.Toggle("Freeze Game", moveInfo.cameraMovements[i].freezeGame, toggleStyle);
								moveInfo.cameraMovements[i].fieldOfView = EditorGUILayout.Slider("Field of View:", moveInfo.cameraMovements[i].fieldOfView, 1, 179);
								moveInfo.cameraMovements[i].position = EditorGUILayout.Vector3Field("Move to Position:", moveInfo.cameraMovements[i].position);
								moveInfo.cameraMovements[i].rotation = EditorGUILayout.Vector3Field("Rotate:", moveInfo.cameraMovements[i].rotation);
								
								//if (character != null && StyledButton("Apply external camera reposition"))
								//	moveInfo.cameraMovements[i].position = character.transform.TransformPoint(initialCamPosition);
								
								EditorGUILayout.Space();
								if (character == null){
									if (StyledButton("Camera Preview")){
										if (moveInfo.characterPrefab == null) {
											characterWarning = true;
											errorMsg = "Drag a character into 'Character Prefab' under 'Animation' first.";
										}else if (EditorApplication.isPlayingOrWillChangePlaymode){
											characterWarning = true;
											errorMsg = "You can't preview animations while in play mode.";
										}else{
											characterWarning = false;

											System.Reflection.Assembly assembly = typeof(UnityEditor.EditorWindow).Assembly;
											System.Type type = assembly.GetType("UnityEditor.GameView");
											EditorWindow gameview = EditorWindow.GetWindow(type);
											gameview.Focus();
											//EditorCamera.SetPosition(Vector3.up * 4);
											//EditorCamera.SetRotation(Quaternion.identity);
											//EditorCamera.SetOrthographic(true);
											//EditorCamera.SetSize(8);

											character = (GameObject) PrefabUtility.InstantiatePrefab(moveInfo.characterPrefab);
											character.transform.position = new Vector3(-2,0,0);
										}
									}
									
									if (characterWarning){
										GUILayout.BeginHorizontal("GroupBox");
										GUILayout.Label(errorMsg,"CN EntryWarn");
										GUILayout.EndHorizontal();
									}
								}else{
									EditorGUILayout.BeginVertical(subGroupStyle);{
										EditorGUILayout.Space();
										
										Camera sceneCam = GameObject.FindObjectOfType<Camera>();
										EditorGUILayout.BeginVertical(subArrayElementStyle);{
											EditorGUILayout.Space();
											initialFieldOfView = EditorGUILayout.Slider("Initial Field of View:", initialFieldOfView, 1, 179);
											initialCamPosition = EditorGUILayout.Vector3Field("Initial Camera Position:", initialCamPosition);
											initialCamRotation = Quaternion.Euler(EditorGUILayout.Vector3Field("Initial Camera Rotation:", initialCamRotation.eulerAngles));
											EditorGUILayout.Space();
										}EditorGUILayout.EndVertical();
										
										EditorGUI.indentLevel += 1;
										camTime = StyledSlider("Timeline", camTime, EditorGUI.indentLevel, 0, moveInfo.cameraMovements[i].duration);

										Vector3 targetPos = character.transform.TransformPoint(moveInfo.cameraMovements[i].position);
										Quaternion targetRot = Quaternion.Euler(moveInfo.cameraMovements[i].rotation);
										float targetFoV = moveInfo.cameraMovements[i].fieldOfView;

										float camTimeSpeedModifier = camTime * moveInfo.cameraMovements[i].camSpeed;
										float journey = camTimeSpeedModifier/moveInfo.cameraMovements[i].duration;
										if (journey > 1) journey = 1;

										if (sceneCam != null){
											sceneCam.transform.position = Vector3.Lerp(initialCamPosition, targetPos, journey);
											sceneCam.transform.localRotation = Quaternion.Slerp(initialCamRotation, targetRot, journey);
											sceneCam.fieldOfView = Mathf.Lerp(initialFieldOfView, targetFoV, journey);
										}

										if (animFrame == 0) animFrame = moveInfo.cameraMovements[i].castingFrame;
										if (moveInfo.cameraMovements[i].freezeAnimation) {
											animFrame = moveInfo.cameraMovements[i].castingFrame;
										}else{
											animFrame = moveInfo.cameraMovements[i].castingFrame + Mathf.Floor(camTime * moveInfo.fps);
										}
										AnimationSampler();
										
										if (StyledButton("Close Preview")) {
											Clear (true, true);
											EditorWindow.FocusWindowIfItsOpen<SceneView>();
										}

										EditorGUI.indentLevel -= 1;
										EditorGUILayout.Space();

									}EditorGUILayout.EndVertical();
								}
								EditorGUILayout.Space();
							}EditorGUILayout.EndVertical();
						}
						if (StyledButton("New Cinematic"))
							moveInfo.cameraMovements = AddElement<CameraMovement>(moveInfo.cameraMovements, new CameraMovement());
							
						EditorGUI.indentLevel -= 1;
					}EditorGUILayout.EndVertical();
					
				}else if (!animationOptions && character != null){
					Clear (true, true);
				}
				//GUILayout.Box("", "TimeScrubber", new GUILayoutOption[]{GUILayout.ExpandWidth(true), GUILayout.Height(1)});
				// End Camera Options
				
				
			}EditorGUILayout.EndVertical();
			EditorGUILayout.BeginVertical(rootGroupStyle);{
				// Begin Self Applied Force Options
				EditorGUILayout.BeginHorizontal();{
					selfAppliedForceOptions = EditorGUILayout.Foldout(selfAppliedForceOptions, "Self Applied Forces ("+ moveInfo.appliedForces.Length +")", EditorStyles.foldout);
					helpButton("move:selfappliedforce");
				}EditorGUILayout.EndHorizontal();

				if (selfAppliedForceOptions){
					EditorGUILayout.BeginVertical(subGroupStyle);{
						EditorGUI.indentLevel += 1;
						List<int> castingValues = new List<int>();
						foreach(AppliedForce appliedForce in moveInfo.appliedForces) 
							castingValues.Add(appliedForce.castingFrame);
						StyledMarker("Casting Timeline", castingValues.ToArray(), moveInfo.totalFrames, EditorGUI.indentLevel);
							
						for (int i = 0; i < moveInfo.appliedForces.Length; i ++){
							EditorGUILayout.Space();
							EditorGUILayout.BeginVertical(arrayElementStyle);{
								EditorGUILayout.Space();
								EditorGUILayout.BeginHorizontal();{
									moveInfo.appliedForces[i].castingFrame = EditorGUILayout.IntSlider("Casting Frame:", moveInfo.appliedForces[i].castingFrame, 0, moveInfo.totalFrames);
									if (GUILayout.Button("", removeButtonStyle)){
										moveInfo.appliedForces = RemoveElement<AppliedForce>(moveInfo.appliedForces, moveInfo.appliedForces[i]);
										return;
									}
								}EditorGUILayout.EndHorizontal();
								moveInfo.appliedForces[i].resetPreviousHorizontal = EditorGUILayout.Toggle("Reset X Force", moveInfo.appliedForces[i].resetPreviousHorizontal, toggleStyle);
								moveInfo.appliedForces[i].resetPreviousVertical = EditorGUILayout.Toggle("Reset Y Force", moveInfo.appliedForces[i].resetPreviousVertical, toggleStyle);
								moveInfo.appliedForces[i].force = EditorGUILayout.Vector2Field("Force Applied:", moveInfo.appliedForces[i].force);
								EditorGUILayout.Space();
							}EditorGUILayout.EndVertical();
						}
						if (StyledButton("New Applied Force"))
							moveInfo.appliedForces = AddElement<AppliedForce>(moveInfo.appliedForces, new AppliedForce());

						EditorGUI.indentLevel -= 1;
						
					}EditorGUILayout.EndVertical();
				}
				//GUILayout.Box("", "TimeScrubber", new GUILayoutOption[]{GUILayout.ExpandWidth(true), GUILayout.Height(1)});
				// End Self Applied Force Options
				
				
			}EditorGUILayout.EndVertical();
			EditorGUILayout.BeginVertical(rootGroupStyle);{
				// Begin Active Frame Options
				EditorGUILayout.BeginHorizontal();{
					activeFramesOptions = EditorGUILayout.Foldout(activeFramesOptions, "Active Frames", EditorStyles.foldout);
					helpButton("move:activeframes");
				}EditorGUILayout.EndHorizontal();

				if (activeFramesOptions){
					EditorGUILayout.BeginVertical(subGroupStyle);{
						EditorGUI.indentLevel += 1;
						
						// Hits Toggle
						hitsToggle = EditorGUILayout.Foldout(hitsToggle, "Hits ("+ moveInfo.hits.Length +")", EditorStyles.foldout);
						if (hitsToggle){
							EditorGUILayout.BeginVertical(subGroupStyle);{
								EditorGUI.indentLevel += 1;
								List<Vector2> castingValues = new List<Vector2>();
								foreach(Hit hit in moveInfo.hits) 
									castingValues.Add(new Vector2(hit.activeFramesBegin, hit.activeFramesEnds));
								StyledMarker("Frame Data Timeline", castingValues.ToArray(), moveInfo.totalFrames, EditorGUI.indentLevel, true);
								
								for (int i = 0; i < moveInfo.hits.Length; i ++){
									EditorGUILayout.Space();
									EditorGUILayout.BeginVertical(arrayElementStyle);{
										EditorGUILayout.Space();
										EditorGUILayout.BeginHorizontal();{
											StyledMinMaxSlider("Active Frames", ref moveInfo.hits[i].activeFramesBegin, ref moveInfo.hits[i].activeFramesEnds, 1, moveInfo.totalFrames, EditorGUI.indentLevel);
											if (GUILayout.Button("", removeButtonStyle)){
												moveInfo.hits = RemoveElement<Hit>(moveInfo.hits, moveInfo.hits[i]);
												return;
											}
										}EditorGUILayout.EndHorizontal();
										
										EditorGUILayout.Space();
										moveInfo.hits[i].continuousHit = EditorGUILayout.Toggle("Continuous Hit", moveInfo.hits[i].continuousHit, toggleStyle);
										moveInfo.hits[i].armorBreaker = EditorGUILayout.Toggle("Armor Breaker", moveInfo.hits[i].armorBreaker, toggleStyle);
										moveInfo.hits[i].hitStrengh = (HitStrengh) EditorGUILayout.EnumPopup("Hit Strenght:", moveInfo.hits[i].hitStrengh, enumStyle);
										moveInfo.hits[i].hitType = (HitType) EditorGUILayout.EnumPopup("Hit Type:", moveInfo.hits[i].hitType, enumStyle);
										
										// Damage Toggle
										moveInfo.hits[i].damageOptionsToggle = EditorGUILayout.Foldout(moveInfo.hits[i].damageOptionsToggle, "Damage Options", EditorStyles.foldout);
										if (moveInfo.hits[i].damageOptionsToggle){
											EditorGUILayout.BeginVertical(subGroupStyle);{
												EditorGUI.indentLevel += 1;
												moveInfo.hits[i].damageType = (DamageType) EditorGUILayout.EnumPopup("Damage Type:", moveInfo.hits[i].damageType, enumStyle);
												moveInfo.hits[i].damageOnHit = EditorGUILayout.FloatField("Damage on Hit:", moveInfo.hits[i].damageOnHit);
												moveInfo.hits[i].damageOnBlock = EditorGUILayout.FloatField("Damage on Block:", moveInfo.hits[i].damageOnBlock);
												moveInfo.hits[i].damageScaling = EditorGUILayout.Toggle("Damage Scaling", moveInfo.hits[i].damageScaling, toggleStyle);
												EditorGUI.indentLevel -= 1;
											}EditorGUILayout.EndVertical();
										}
										
										// Hit Stun Toggle
										moveInfo.hits[i].hitStunOptionsToggle = EditorGUILayout.Foldout(moveInfo.hits[i].hitStunOptionsToggle, "Hit Stun Options", EditorStyles.foldout);
										if (moveInfo.hits[i].hitStunOptionsToggle){
											EditorGUILayout.BeginVertical(subGroupStyle);{
												EditorGUI.indentLevel += 1;
												moveInfo.hits[i].hitStunType = (HitStunType) EditorGUILayout.EnumPopup("Hit Stun Type:", moveInfo.hits[i].hitStunType, enumStyle);
												moveInfo.hits[i].resetPreviousHitStun = EditorGUILayout.Toggle("Reset Hit Stun", moveInfo.hits[i].resetPreviousHitStun, toggleStyle);
												EditorGUI.BeginDisabledGroup(moveInfo.hits[i].hitType == HitType.HardKnockdown || moveInfo.hits[i].hitType == HitType.Knockdown);{
													if (moveInfo.hits[i].hitStunType == HitStunType.FrameAdvantage){
														EditorGUILayout.LabelField("Frame Advantage on Hit:");
														moveInfo.hits[i].frameAdvantageOnHit = EditorGUILayout.IntSlider("", moveInfo.hits[i].frameAdvantageOnHit, -40, 120);
													}else{
														moveInfo.hits[i].hitStunOnHit = EditorGUILayout.IntField("Hit Stun on Hit:", moveInfo.hits[i].hitStunOnHit);
													}
												}EditorGUI.EndDisabledGroup();
												if (moveInfo.hits[i].hitStunType == HitStunType.FrameAdvantage){
													EditorGUILayout.LabelField("Frame Advantage on Block:");
													moveInfo.hits[i].frameAdvantageOnBlock = EditorGUILayout.IntSlider("", moveInfo.hits[i].frameAdvantageOnBlock, -40, 120);
												}else{
													moveInfo.hits[i].hitStunOnBlock = EditorGUILayout.IntField("Hit Stun on Block:", moveInfo.hits[i].hitStunOnBlock);
												}
												EditorGUI.indentLevel -= 1;
											}EditorGUILayout.EndVertical();
										}
										
										// Force Toggle
										moveInfo.hits[i].forceOptionsToggle = EditorGUILayout.Foldout(moveInfo.hits[i].forceOptionsToggle, "Force Options", EditorStyles.foldout);
										if (moveInfo.hits[i].forceOptionsToggle){
											EditorGUILayout.BeginVertical(subGroupStyle);{
												EditorGUI.indentLevel += 1;
												opponentForceToggle = EditorGUILayout.Foldout(opponentForceToggle, "Opponent", EditorStyles.foldout);
												if (opponentForceToggle){
														EditorGUI.indentLevel += 1;
														EditorGUI.BeginDisabledGroup(moveInfo.hits[i].hitType == HitType.HardKnockdown || moveInfo.hits[i].hitType == HitType.Knockdown);{
															moveInfo.hits[i].resetPreviousHorizontalPush = EditorGUILayout.Toggle("Reset X Forces", moveInfo.hits[i].resetPreviousHorizontalPush, toggleStyle);
															moveInfo.hits[i].resetPreviousVerticalPush = EditorGUILayout.Toggle("Reset Y Forces", moveInfo.hits[i].resetPreviousVerticalPush, toggleStyle);
														}EditorGUI.EndDisabledGroup();
														moveInfo.hits[i].pushForce = EditorGUILayout.Vector2Field("Applied Force", moveInfo.hits[i].pushForce);
														EditorGUI.indentLevel -= 1;
												}
												selfForceToggle = EditorGUILayout.Foldout(selfForceToggle, "Self", EditorStyles.foldout);
												if (selfForceToggle){
														EditorGUI.indentLevel += 1;
														moveInfo.hits[i].resetPreviousHorizontal = EditorGUILayout.Toggle("Reset X Forces", moveInfo.hits[i].resetPreviousHorizontal, toggleStyle);
														moveInfo.hits[i].resetPreviousVertical = EditorGUILayout.Toggle("Reset Y Forces", moveInfo.hits[i].resetPreviousVertical, toggleStyle);
														moveInfo.hits[i].appliedForce = EditorGUILayout.Vector2Field("Applied Force", moveInfo.hits[i].appliedForce);
														EditorGUI.indentLevel -= 1;
												}
												EditorGUI.indentLevel -= 1;
											}EditorGUILayout.EndVertical();
										}
										
										EditorGUIUtility.labelWidth = 180;
										// Pull In Toggle
										moveInfo.hits[i].pullInToggle = EditorGUILayout.Foldout(moveInfo.hits[i].pullInToggle, "Pull In Options", EditorStyles.foldout);
										if (moveInfo.hits[i].pullInToggle){
											EditorGUILayout.BeginVertical(subGroupStyle);{
												EditorGUI.indentLevel += 1;
												pullEnemyInToggle = EditorGUILayout.Foldout(pullEnemyInToggle, "Opponent Towards Self", EditorStyles.foldout);
												if (pullEnemyInToggle){
													EditorGUI.indentLevel += 1;
													moveInfo.hits[i].pullEnemyIn.speed = EditorGUILayout.IntSlider("Speed:", moveInfo.hits[i].pullEnemyIn.speed, 1, 100);
													moveInfo.hits[i].pullEnemyIn.characterBodyPart = (BodyPart) EditorGUILayout.EnumPopup("Body Part (self):", moveInfo.hits[i].pullEnemyIn.characterBodyPart, enumStyle);
													moveInfo.hits[i].pullEnemyIn.enemyBodyPart = (BodyPart) EditorGUILayout.EnumPopup("Body Part (enemy):", moveInfo.hits[i].pullEnemyIn.enemyBodyPart, enumStyle);
													EditorGUI.indentLevel -= 1;
												}
												pullSelfInToggle = EditorGUILayout.Foldout(pullSelfInToggle, "Self Towards Opponent", EditorStyles.foldout);
												if (pullSelfInToggle){
													EditorGUI.indentLevel += 1;
													moveInfo.hits[i].pullSelfIn.speed = EditorGUILayout.IntSlider("Speed:", moveInfo.hits[i].pullSelfIn.speed, 1, 100);
													moveInfo.hits[i].pullSelfIn.characterBodyPart = (BodyPart) EditorGUILayout.EnumPopup("Body Part (self):", moveInfo.hits[i].pullSelfIn.characterBodyPart, enumStyle);
													moveInfo.hits[i].pullSelfIn.enemyBodyPart = (BodyPart) EditorGUILayout.EnumPopup("Body Part (enemy):", moveInfo.hits[i].pullSelfIn.enemyBodyPart, enumStyle);
													EditorGUI.indentLevel -= 1;
												}
												EditorGUI.indentLevel -= 1;
											}EditorGUILayout.EndVertical();
										}
										EditorGUIUtility.labelWidth = 150;
										
										// Hurt Boxes Toggle
										int amount = moveInfo.hits[i].hurtBoxes != null ? moveInfo.hits[i].hurtBoxes.Length : 0;
										moveInfo.hits[i].hurtBoxesToggle = EditorGUILayout.Foldout(moveInfo.hits[i].hurtBoxesToggle, "Hurt Boxes ("+ amount +")", EditorStyles.foldout);
										if (moveInfo.hits[i].hurtBoxesToggle){
											EditorGUILayout.BeginVertical(subGroupStyle);{
												EditorGUI.indentLevel += 1;
												if (amount > 0){
													for (int y = 0; y < moveInfo.hits[i].hurtBoxes.Length; y ++){
														EditorGUILayout.BeginVertical(subArrayElementStyle);{
															EditorGUILayout.BeginHorizontal();{
																moveInfo.hits[i].hurtBoxes[y].bodyPart = (BodyPart)EditorGUILayout.EnumPopup("Body Part:", moveInfo.hits[i].hurtBoxes[y].bodyPart, enumStyle);
																if (GUILayout.Button("", removeButtonStyle)){
																	moveInfo.hits[i].hurtBoxes = RemoveElement<HurtBox>(moveInfo.hits[i].hurtBoxes, moveInfo.hits[i].hurtBoxes[y]);
																	return;
																}
															}EditorGUILayout.EndHorizontal();
															moveInfo.hits[i].hurtBoxes[y].radius = EditorGUILayout.FloatField("Radius:", moveInfo.hits[i].hurtBoxes[y].radius);
															moveInfo.hits[i].hurtBoxes[y].offSet = EditorGUILayout.Vector2Field("Off Set:", moveInfo.hits[i].hurtBoxes[y].offSet);
															EditorGUILayout.Space();
														}EditorGUILayout.EndVertical();
													}
												}
												if (StyledButton("New Hurt Box"))
													moveInfo.hits[i].hurtBoxes = AddElement<HurtBox>(moveInfo.hits[i].hurtBoxes, new HurtBox());
													
												EditorGUI.indentLevel -= 1;
											}EditorGUILayout.EndVertical();
										}
										EditorGUILayout.Space();
									}EditorGUILayout.EndVertical();
								}

								if (StyledButton("New Hit"))
									moveInfo.hits = AddElement<Hit>(moveInfo.hits, new Hit());

								EditorGUI.indentLevel -= 1;
							}EditorGUILayout.EndVertical();
						}
						
						// Blockable Area Toggle
						blockableAreaToggle = EditorGUILayout.Foldout(blockableAreaToggle, "Blockable Area", EditorStyles.foldout);
						if (blockableAreaToggle){
							EditorGUILayout.BeginVertical(subGroupStyle);{
								EditorGUI.indentLevel += 1;
								StyledMinMaxSlider("Active Frames", ref moveInfo.blockableArea.activeFramesBegin, ref moveInfo.blockableArea.activeFramesEnds, 1, moveInfo.totalFrames, EditorGUI.indentLevel);
								moveInfo.blockableArea.bodyPart = (BodyPart) EditorGUILayout.EnumPopup("Body Part:", moveInfo.blockableArea.bodyPart, enumStyle);
								moveInfo.blockableArea.radius = EditorGUILayout.FloatField("Radius:", moveInfo.blockableArea.radius);
								moveInfo.blockableArea.offSet = EditorGUILayout.Vector2Field("Off Set:", moveInfo.blockableArea.offSet);
								EditorGUI.indentLevel -= 1;
							}EditorGUILayout.EndVertical();
						}
						EditorGUI.indentLevel -= 1;
					
					}EditorGUILayout.EndVertical();
				}
				//GUILayout.Box("", "TimeScrubber", new GUILayoutOption[]{GUILayout.ExpandWidth(true), GUILayout.Height(1)});
				// End Active Frames Options
				
				
			}EditorGUILayout.EndVertical();
			EditorGUILayout.BeginVertical(rootGroupStyle);{
				// Begin Invincible Frames Options
				EditorGUILayout.BeginHorizontal();{
					invincibleFramesOptions = EditorGUILayout.Foldout(invincibleFramesOptions, "Invincible Frames ("+ moveInfo.invincibleBodyParts.Length +")", EditorStyles.foldout);
					helpButton("move:invincibleframes");
				}EditorGUILayout.EndHorizontal();

				if (invincibleFramesOptions){
					EditorGUILayout.BeginVertical(subGroupStyle);{
						EditorGUI.indentLevel += 1;
						List<Vector2> castingValues = new List<Vector2>();
						foreach(InvincibleBodyParts invBodyPart in moveInfo.invincibleBodyParts) 
							castingValues.Add(new Vector2(invBodyPart.activeFramesBegin, invBodyPart.activeFramesEnds));
						StyledMarker("Invincible Frames Timeline", castingValues.ToArray(), moveInfo.totalFrames, EditorGUI.indentLevel, false);

						EditorGUI.indentLevel += 1;
						for (int i = 0; i < moveInfo.invincibleBodyParts.Length; i ++){
							EditorGUILayout.Space();
							EditorGUILayout.BeginVertical(arrayElementStyle);{
								EditorGUILayout.Space();
								EditorGUILayout.BeginHorizontal();{
									StyledMinMaxSlider("Invincible Frames", ref moveInfo.invincibleBodyParts[i].activeFramesBegin, ref moveInfo.invincibleBodyParts[i].activeFramesEnds, 1, moveInfo.totalFrames, EditorGUI.indentLevel);
									if (GUILayout.Button("", removeButtonStyle)){
										moveInfo.invincibleBodyParts = RemoveElement<InvincibleBodyParts>(moveInfo.invincibleBodyParts, moveInfo.invincibleBodyParts[i]);
										return;
									}
								}EditorGUILayout.EndHorizontal();
								
								EditorGUILayout.Space();
								
								EditorGUIUtility.labelWidth = 180;
								moveInfo.invincibleBodyParts[i].completelyInvincible = EditorGUILayout.Toggle("Completely Invincible", moveInfo.invincibleBodyParts[i].completelyInvincible, toggleStyle);
								EditorGUIUtility.labelWidth = 150;

								if (!moveInfo.invincibleBodyParts[i].completelyInvincible){
									bodyPartsToggle = EditorGUILayout.Foldout(bodyPartsToggle, "Body Parts ("+ moveInfo.invincibleBodyParts[i].bodyParts.Length +")", EditorStyles.foldout);
									if (bodyPartsToggle){
										EditorGUILayout.Space();
										EditorGUI.indentLevel += 1;
										if (moveInfo.invincibleBodyParts[i].bodyParts != null){
											for (int y = 0; y < moveInfo.invincibleBodyParts[i].bodyParts.Length; y ++){
												EditorGUILayout.Space();
												EditorGUILayout.BeginVertical(subArrayElementStyle);{
													EditorGUILayout.BeginHorizontal();{
														moveInfo.invincibleBodyParts[i].bodyParts[y] = (BodyPart)EditorGUILayout.EnumPopup("Body Part:", moveInfo.invincibleBodyParts[i].bodyParts[y], enumStyle);
														if (GUILayout.Button("", removeButtonStyle)){
															moveInfo.invincibleBodyParts[i].bodyParts = RemoveElement<BodyPart>(moveInfo.invincibleBodyParts[i].bodyParts, moveInfo.invincibleBodyParts[i].bodyParts[y]);
															return;
														}
													}EditorGUILayout.EndHorizontal();
													EditorGUILayout.Space();
												}EditorGUILayout.EndVertical();
											}
										}
										if (StyledButton("New Body Part"))
											moveInfo.invincibleBodyParts[i].bodyParts = AddElement<BodyPart>(moveInfo.invincibleBodyParts[i].bodyParts, BodyPart.none);

										EditorGUI.indentLevel -= 1;

									}
								}
								EditorGUILayout.Space();
							}EditorGUILayout.EndVertical();
						}
						if (StyledButton("New Invincible Frame Group"))
							moveInfo.invincibleBodyParts = AddElement<InvincibleBodyParts>(moveInfo.invincibleBodyParts, new InvincibleBodyParts());

						EditorGUI.indentLevel -= 2;
						
					}EditorGUILayout.EndVertical();
				}
				//GUILayout.Box("", "TimeScrubber", new GUILayoutOption[]{GUILayout.ExpandWidth(true), GUILayout.Height(1)});
				// End Invincible Frames Options

			}EditorGUILayout.EndVertical();
			EditorGUILayout.BeginVertical(rootGroupStyle);{
				// Begin Projectile Options
				EditorGUILayout.BeginHorizontal();{
					projectileOptions = EditorGUILayout.Foldout(projectileOptions, "Projectiles ("+ moveInfo.projectiles.Length +")", EditorStyles.foldout);
					helpButton("move:projectiles");
				}EditorGUILayout.EndHorizontal();

				if (projectileOptions){
					EditorGUILayout.BeginVertical(subGroupStyle);{
						EditorGUI.indentLevel += 1;
						List<int> castingValues = new List<int>();
						foreach(Projectile projectile in moveInfo.projectiles) 
							castingValues.Add(projectile.castingFrame);
						StyledMarker("Casting Timeline", castingValues.ToArray(), moveInfo.totalFrames, EditorGUI.indentLevel);
							
						for (int i = 0; i < moveInfo.projectiles.Length; i ++){
							EditorGUILayout.Space();
							EditorGUILayout.BeginVertical(arrayElementStyle);{
								EditorGUILayout.Space();
								EditorGUILayout.BeginHorizontal();{
									moveInfo.projectiles[i].castingFrame = EditorGUILayout.IntSlider("Casting Frame:", moveInfo.projectiles[i].castingFrame, 0, moveInfo.totalFrames);
									if (GUILayout.Button("", removeButtonStyle)){
										moveInfo.projectiles = RemoveElement<Projectile>(moveInfo.projectiles, moveInfo.projectiles[i]);
										return;
									}
								}EditorGUILayout.EndHorizontal();
								moveInfo.projectiles[i].projectilePrefab = (GameObject) EditorGUILayout.ObjectField("Projectile Prefab:", moveInfo.projectiles[i].projectilePrefab, typeof(UnityEngine.GameObject), true);
								moveInfo.projectiles[i].impactPrefab = (GameObject) EditorGUILayout.ObjectField("Impact Prefab:", moveInfo.projectiles[i].impactPrefab, typeof(UnityEngine.GameObject), true);
								moveInfo.projectiles[i].bodyPart = (BodyPart) EditorGUILayout.EnumPopup("Body Part Origin:", moveInfo.projectiles[i].bodyPart, enumStyle);
								moveInfo.projectiles[i].type = (ProjectileType) EditorGUILayout.EnumPopup("Projectile Type:", moveInfo.projectiles[i].type, enumStyle);
								moveInfo.projectiles[i].speed = EditorGUILayout.IntSlider("Speed:", moveInfo.projectiles[i].speed, 1, 100);
								moveInfo.projectiles[i].directionAngle = EditorGUILayout.IntSlider("Direction (Angle):", moveInfo.projectiles[i].directionAngle, -180, 180);
								moveInfo.projectiles[i].duration = EditorGUILayout.FloatField("Duration (Seconds):", moveInfo.projectiles[i].duration);
								moveInfo.projectiles[i].projectileCollision = EditorGUILayout.Toggle("Projectile Collision:", moveInfo.projectiles[i].projectileCollision);
								moveInfo.projectiles[i].offSet = EditorGUILayout.Vector3Field("Off Set:", moveInfo.projectiles[i].offSet);
								
								moveInfo.projectiles[i].totalHits = EditorGUILayout.IntField("Total Hits:", moveInfo.projectiles[i].totalHits);
								moveInfo.projectiles[i].pushForce = EditorGUILayout.Vector2Field("Push Force:", moveInfo.projectiles[i].pushForce);
								moveInfo.projectiles[i].hitRadius = EditorGUILayout.FloatField("Hit Radius:", moveInfo.projectiles[i].hitRadius);
								moveInfo.projectiles[i].hitStrengh = (HitStrengh)EditorGUILayout.EnumPopup("Hit Strength:", moveInfo.projectiles[i].hitStrengh, enumStyle);
								moveInfo.projectiles[i].hitType = (HitType)EditorGUILayout.EnumPopup("Hit Type:", moveInfo.projectiles[i].hitType, enumStyle);

								// Damage Toggle
								moveInfo.projectiles[i].damageOptionsToggle = EditorGUILayout.Foldout(moveInfo.projectiles[i].damageOptionsToggle, "Damage Options", EditorStyles.foldout);
								if (moveInfo.projectiles[i].damageOptionsToggle){
									EditorGUILayout.BeginVertical(subGroupStyle);{
										EditorGUI.indentLevel += 1;
										moveInfo.projectiles[i].damageType = (DamageType) EditorGUILayout.EnumPopup("Damage Type:", moveInfo.projectiles[i].damageType, enumStyle);
										moveInfo.projectiles[i].damageOnHit = EditorGUILayout.FloatField("Damage on Hit:", moveInfo.projectiles[i].damageOnHit);
										moveInfo.projectiles[i].damageOnBlock = EditorGUILayout.FloatField("Damage on Block:", moveInfo.projectiles[i].damageOnBlock);
										moveInfo.projectiles[i].damageScaling = EditorGUILayout.Toggle("Damage Scaling", moveInfo.projectiles[i].damageScaling, toggleStyle);
										EditorGUI.indentLevel -= 1;
									}EditorGUILayout.EndVertical();
								}
								
								// Hit Stun Toggle
								moveInfo.projectiles[i].hitStunOptionsToggle = EditorGUILayout.Foldout(moveInfo.projectiles[i].hitStunOptionsToggle, "Hit Stun Options", EditorStyles.foldout);
								if (moveInfo.projectiles[i].hitStunOptionsToggle){
									EditorGUILayout.BeginVertical(subGroupStyle);{
										EditorGUI.indentLevel += 1;
										moveInfo.projectiles[i].resetPreviousHitStun = EditorGUILayout.Toggle("Reset Hit Stun", moveInfo.projectiles[i].resetPreviousHitStun, toggleStyle);
										moveInfo.projectiles[i].hitStunOnHit = EditorGUILayout.IntField("Hit Stun on Hit:", moveInfo.projectiles[i].hitStunOnHit);
										moveInfo.projectiles[i].hitStunOnBlock = EditorGUILayout.IntField("Hit Stun on Block:", moveInfo.projectiles[i].hitStunOnBlock);
										EditorGUI.indentLevel -= 1;
									}EditorGUILayout.EndVertical();
								}EditorGUILayout.Space();
							}EditorGUILayout.EndVertical();
						}
						if (StyledButton("New Projectile"))
							moveInfo.projectiles = AddElement<Projectile>(moveInfo.projectiles, new Projectile());
							
						EditorGUI.indentLevel -= 1;
					
					}EditorGUILayout.EndVertical();
				}
				// End Projectile Options
		}EditorGUILayout.EndVertical();

		}EditorGUILayout.EndScrollView();
		
		//DrawDefaultInspector();

		if (GUI.changed) {
			Undo.RecordObject(moveInfo, "Move Editor Modify");
			EditorUtility.SetDirty(moveInfo);
		}
	}
	
	public int StyledSlider (string label, int targetVar, int indentLevel, int minValue, int maxValue) {
		int indentSpacing = 25 * indentLevel;
		EditorGUILayout.Space();
		EditorGUILayout.Space();

		Rect tempRect = GUILayoutUtility.GetRect(1, 10);

		Rect rect = new Rect(indentSpacing,tempRect.y, Screen.width - indentSpacing - 100, 20);
		EditorGUI.ProgressBar(rect, Mathf.Abs((float)targetVar/maxValue), label);
		
		tempRect = GUILayoutUtility.GetRect(1, 20);
		rect.y += 10;
		rect.x = indentLevel * 10;
		rect.width = (Screen.width - (indentLevel * 10) - 100) + 55; // Changed for 4.3;

		return EditorGUI.IntSlider(rect, "", targetVar, minValue, maxValue);
	}
	
	public float StyledSlider (string label, float targetVar, int indentLevel, float minValue, float maxValue) {
		int indentSpacing = 25 * indentLevel;
		EditorGUILayout.Space();
		EditorGUILayout.Space();

		Rect tempRect = GUILayoutUtility.GetRect(1, 10);

		Rect rect = new Rect(indentSpacing,tempRect.y, Screen.width - indentSpacing - 100, 20);
		EditorGUI.ProgressBar(rect, Mathf.Abs((float)targetVar/maxValue), label);
		
		tempRect = GUILayoutUtility.GetRect(1, 20);
		rect.y += 10;
		rect.x = indentLevel * 10;
		rect.width = (Screen.width - (indentLevel * 10) - 100) + 55; // Changed for 4.3;

		return EditorGUI.Slider(rect, "", targetVar, minValue, maxValue);
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
	
	public void StyledMinMaxSlider (string label, ref int minValue, ref int maxValue, int minLimit, int maxLimit, int indentLevel) {
		int indentSpacing = 25 * indentLevel;
		//indentSpacing += 30;
		EditorGUILayout.Space();
		EditorGUILayout.Space();

		if (minValue < 1) minValue = 1;
		if (maxValue < 2) maxValue = 2;
		if (maxValue > maxLimit) maxValue = maxLimit;
		if (minValue == maxValue) minValue --;
		float minValueFloat = (float) minValue;
		float maxValueFloat = (float) maxValue;
		float minLimitFloat = (float) minLimit;
		float maxLimitFloat = (float) maxLimit;
		
		Rect tempRect = GUILayoutUtility.GetRect(1, 10);

		Rect rect = new Rect(indentSpacing,tempRect.y, Screen.width - indentSpacing - 100, 20);
		//Rect rect = new Rect(indentSpacing + 15,tempRect.y, Screen.width - indentSpacing - 70, 20);
		float fillLeftPos = ((rect.width/maxLimitFloat) * minValueFloat) + rect.x;
		float fillRightPos = ((rect.width/maxLimitFloat) * maxValueFloat) + rect.x;
		float fillWidth = fillRightPos - fillLeftPos;
		
		fillWidth += (rect.width/maxLimitFloat);
		fillLeftPos -= (rect.width/maxLimitFloat);
		
		// Border
		GUI.Box(rect, "", borderBarStyle);
		
		// Overlay
		GUI.Box(new Rect(fillLeftPos, rect.y, fillWidth, rect.height), new GUIContent(), fillBarStyle1);
		
		// Text
		//GUIStyle centeredStyle = GUI.skin.GetStyle("Label");
    	//centeredStyle.alignment = TextAnchor.UpperCenter;
		labelStyle.alignment = TextAnchor.UpperCenter;
		GUI.Label(rect, label + " between "+ Mathf.Floor(minValueFloat)+" and "+Mathf.Floor(maxValueFloat), labelStyle);
		labelStyle.alignment = TextAnchor.MiddleCenter;
		
		// Slider
		rect.y += 10;
		rect.x = indentLevel * 10;
		rect.width = (Screen.width - (indentLevel * 10) - 100);

		EditorGUI.MinMaxSlider(rect, ref minValueFloat, ref maxValueFloat, minLimitFloat, maxLimitFloat);
		minValue = (int) minValueFloat;
		maxValue = (int) maxValueFloat;

		tempRect = GUILayoutUtility.GetRect(1, 20);
	}
	
	public void StyledMarker (string label, int[] locations, int maxValue, int indentLevel) {
		if (indentLevel == 1) indentLevel++;
		int indentSpacing = 25 * indentLevel;
		
		EditorGUILayout.Space();
		EditorGUILayout.Space();
		Rect tempRect = GUILayoutUtility.GetRect(1, 20);
		Rect rect = new Rect(indentSpacing,tempRect.y, Screen.width - indentSpacing - 60, 20);
		
		// Border
		GUI.Box(rect, "", borderBarStyle);
		
		// Overlay
		foreach(int i in locations){
			float xPos = ((rect.width/(float)maxValue) * (float)i) + rect.x;
			if (xPos + 5 > rect.width + rect.x) xPos -= 5;
			GUI.Box(new Rect(xPos, rect.y, 5, rect.height), new GUIContent(), fillBarStyle2);
		}
		
		// Text
		GUI.Label(rect, new GUIContent(label), labelStyle);
		
		tempRect = GUILayoutUtility.GetRect(1, 20);
	}
	
	public void StyledMarker (string label, Vector2[] locations, int maxValue, int indentLevel, bool fillBounds) {
		if (indentLevel == 1) indentLevel++;
		int indentSpacing = 25 * indentLevel;
		
		EditorGUILayout.Space();
		EditorGUILayout.Space();
		Rect tempRect = GUILayoutUtility.GetRect(1, 20);
		Rect rect = new Rect(indentSpacing,tempRect.y, Screen.width - indentSpacing - 60, 20);
		
		// Border
		GUI.Box(rect, "", borderBarStyle);

		if (fillBounds && locations.Length > 0){
			float firstLeftPos = ((rect.width/maxValue) * locations[0].x);
			firstLeftPos -= (rect.width/maxValue);
			GUI.Box(new Rect(rect.x, rect.y, firstLeftPos, rect.height), new GUIContent(), fillBarStyle3);
		}

		// Overlay
		float fillLeftPos = 0;
		float fillRightPos = 0;
		foreach(Vector2 i in locations){
			fillLeftPos = ((rect.width/maxValue) * i.x) + rect.x;
			fillRightPos = ((rect.width/maxValue) * i.y) + rect.x;

			float fillWidth = fillRightPos - fillLeftPos;
			fillWidth += (rect.width/maxValue);
			fillLeftPos -= (rect.width/maxValue);
			
			GUI.Box(new Rect(fillLeftPos, rect.y, fillWidth, rect.height), new GUIContent(), fillBarStyle2);
		}

		if (fillBounds && locations.Length > 0){
			float fillWidth = rect.width - fillRightPos + rect.x;
			GUI.Box(new Rect(fillRightPos, rect.y, fillWidth, rect.height), new GUIContent(), fillBarStyle4);
		}

		// Text
		GUI.Label(rect, new GUIContent(label), labelStyle);

		if (fillBounds && locations.Length > 0){
			EditorGUILayout.Space();
			GUILayout.BeginHorizontal(subArrayElementStyle);{
				labelStyle.normal.textColor = Color.yellow;
				moveInfo.startUpFrames = (moveInfo.hits[0].activeFramesBegin - 1);
				GUILayout.Label("Start Up: "+ moveInfo.startUpFrames, labelStyle);
				labelStyle.normal.textColor = Color.cyan;
				moveInfo.activeFrames = (moveInfo.hits[moveInfo.hits.Length - 1].activeFramesEnds - moveInfo.hits[0].activeFramesBegin);
				GUILayout.Label("Active: "+ moveInfo.activeFrames, labelStyle);
				labelStyle.normal.textColor = Color.red;
				moveInfo.recoveryFrames = (moveInfo.totalFrames - moveInfo.hits[moveInfo.hits.Length - 1].activeFramesEnds + 1);
				GUILayout.Label("Recovery: "+ moveInfo.recoveryFrames, labelStyle);
			}GUILayout.EndHorizontal();
		}
		labelStyle.normal.textColor = Color.white;

		//GUI.skin.label.normal.textColor = new Color(.706f, .706f, .706f, 1);
		tempRect = GUILayoutUtility.GetRect(1, 20);
	}

	public void AnimationSampler(){
		if (moveInfo.animationSpeed < 0){
			animTime = ((animFrame/moveInfo.fps) * moveInfo.animationSpeed) + moveInfo.animationClip.length;
		}else{
			animTime = (animFrame/moveInfo.fps) * moveInfo.animationSpeed;
		}
		HitBoxesScript hitBoxesScript = character.GetComponent<HitBoxesScript>();
		foreach (Hit hit in moveInfo.hits){
			if (animFrame >= hit.activeFramesBegin && animFrame < hit.activeFramesEnds) {
				if (hit.hurtBoxes.Length > 0)
					hitBoxesScript.activeHurtBoxes = hit.hurtBoxes;
				break;
			}else{
				hitBoxesScript.activeHurtBoxes = null;
			}
		}
		if (animFrame >= moveInfo.blockableArea.activeFramesBegin && animFrame < moveInfo.blockableArea.activeFramesEnds) {
			hitBoxesScript.blockableArea = moveInfo.blockableArea;
		}else{
			hitBoxesScript.blockableArea = null;
		}
		foreach (InvincibleBodyParts invincibleBodyPart in moveInfo.invincibleBodyParts){
			if (animFrame >= invincibleBodyPart.activeFramesBegin && animFrame < invincibleBodyPart.activeFramesEnds) {
				if (invincibleBodyPart.completelyInvincible){
					hitBoxesScript.hideHitBoxes();
				}else{
					if (invincibleBodyPart.bodyParts != null && invincibleBodyPart.bodyParts.Length > 0)
						hitBoxesScript.hideHitBoxes(hitBoxesScript.getHitBoxes(invincibleBodyPart.bodyParts));
				}
				break;
			}else{
				if (invincibleBodyPart.completelyInvincible){
					hitBoxesScript.showHitBoxes();
				}else{
					if (invincibleBodyPart.bodyParts != null && invincibleBodyPart.bodyParts.Length > 0)
						hitBoxesScript.showHitBoxes(hitBoxesScript.getHitBoxes(invincibleBodyPart.bodyParts));
				}
			}
		}

		if (animFrame == 0 || animFrame == moveInfo.totalFrames) hitBoxesScript.showHitBoxes();

        moveInfo.animationClip.SampleAnimation(character, animTime);
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
