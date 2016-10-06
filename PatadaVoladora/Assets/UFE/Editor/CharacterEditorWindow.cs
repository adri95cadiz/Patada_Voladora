using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;


public class CharacterEditorWindow : EditorWindow {
	public static CharacterEditorWindow characterEditorWindow;
	public static CharacterInfo sentCharacterInfo;
	private CharacterInfo characterInfo;

	private Vector2 scrollPos;
	private GameObject character;
	private bool characterPreviewToggle;

	private int bloodTypeChoice;
	private string[] bloodTypeChoices = new string[]{"O-","O+","A-","A+","B-","B+","AB-","AB+"};
	
	private bool hitBoxesOption;
	private bool transformToggle;
	private bool hitBoxesToggle;

	private bool physicsOption;
	private bool moveSetOption;
	private bool characterWarning;
	private string errorMsg;


	private string titleStyle;
	private string removeButtonStyle;
	private string addButtonStyle;
	private string rootGroupStyle;
	private string subGroupStyle;
	private string arrayElementStyle;
	private string subArrayElementStyle;
	private string toggleStyle;
	private string foldStyle;
	private string enumStyle;
	private GUIStyle labelStyle;

	[MenuItem("Window/U.F.E./Character Editor")]
	public static void Init(){
		characterEditorWindow = EditorWindow.GetWindow<CharacterEditorWindow>(false, "Character", true);
		characterEditorWindow.Show();
		characterEditorWindow.Populate();
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
		ClosePreview();
	}
	
	void OnDestroy(){
		ClosePreview();
	}
	
	void OnLostFocus(){
		ClosePreview();
	}

	public void PreviewCharacter(){
		characterPreviewToggle = true;
		EditorCamera.SetPosition(Vector3.up * 3.5f);
		EditorCamera.SetRotation(Quaternion.identity);
		EditorCamera.SetOrthographic(true);
		EditorCamera.SetSize(8);
		if (character == null){
			character = (GameObject) PrefabUtility.InstantiatePrefab(characterInfo.characterPrefab);
			character.transform.position = new Vector3(-2,0,0);
		}
	}
	
	public void ClosePreview(){
		characterPreviewToggle = false;
		if (character != null){
			DestroyImmediate(character);
			character = null;
		}
	}

	void helpButton(string page){
		if (GUILayout.Button("?", GUILayout.Width(18), GUILayout.Height(18))) 
			Application.OpenURL("http://www.ufe3d.com/doku.php/"+ page);
	}

	void Update(){
		if (EditorApplication.isPlayingOrWillChangePlaymode && character != null) {
			ClosePreview();
		}
	}

	void Populate(){
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
		
		labelStyle = new GUIStyle();
		labelStyle.alignment = TextAnchor.MiddleCenter;
		labelStyle.fontStyle = FontStyle.Bold;
		labelStyle.normal.textColor = Color.white;
		
		
		if (sentCharacterInfo != null){
			EditorGUIUtility.PingObject( sentCharacterInfo );
			Selection.activeObject = sentCharacterInfo;
			sentCharacterInfo = null;
		}

		Object[] selection = Selection.GetFiltered(typeof(CharacterInfo), SelectionMode.Assets);
		if (selection.Length > 0){
			if (selection[0] == null) return;
			//characterInfoSO = new SerializedObject(selection[0]);
			characterInfo = (CharacterInfo) selection[0];
		}
	}
	
	public void OnGUI(){
		if (characterInfo == null){
			GUILayout.BeginHorizontal("GroupBox");
			GUILayout.Label("Select a character file or create a new character.","CN EntryInfo");
			GUILayout.EndHorizontal();
			EditorGUILayout.Space();
			if (GUILayout.Button("Create new character"))
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
				EditorGUILayout.LabelField("", (characterInfo.characterName == ""? "New Character":characterInfo.characterName) , fontStyle, GUILayout.Height(32));
				helpButton("character:start");
			}EditorGUILayout.EndHorizontal();
		}EditorGUILayout.EndVertical();
		
		scrollPos = EditorGUILayout.BeginScrollView(scrollPos);{
			EditorGUILayout.BeginVertical(rootGroupStyle);{
				EditorGUILayout.BeginHorizontal();{
					//Rect rect = GUILayoutUtility.GetRect(120, 120);
					//characterInfo.profilePicture = (Texture2D) EditorGUI.ObjectField(rect, characterInfo.profilePicture, typeof(Texture2D), false);
					characterInfo.profilePictureSmall = (Texture2D) EditorGUILayout.ObjectField(characterInfo.profilePictureSmall, typeof(Texture2D), false, GUILayout.Width(100), GUILayout.Height(122));
					EditorGUILayout.BeginVertical();{
						EditorGUIUtility.labelWidth = 90;
						characterInfo.characterName = EditorGUILayout.TextField("Name:", characterInfo.characterName);
						characterInfo.age = EditorGUILayout.IntField("Age:", characterInfo.age);
						bloodTypeChoice = EditorGUILayout.Popup("Blood Type:", bloodTypeChoice, bloodTypeChoices);
						characterInfo.bloodType = bloodTypeChoices[bloodTypeChoice];
						characterInfo.gender = (Gender) EditorGUILayout.EnumPopup("Gender:", characterInfo.gender);
						characterInfo.height = EditorGUILayout.FloatField("Height:", characterInfo.height);
						characterInfo.lifePoints = EditorGUILayout.IntField("Life Points:", characterInfo.lifePoints);
						characterInfo.maxGaugePoints = EditorGUILayout.IntField("Max Gauge:", characterInfo.maxGaugePoints);
					} EditorGUILayout.EndVertical();

				}EditorGUILayout.EndHorizontal();
				EditorGUIUtility.labelWidth = 180;
				EditorGUILayout.LabelField("Portrail Big:");
				characterInfo.profilePictureBig = (Texture2D) EditorGUILayout.ObjectField(characterInfo.profilePictureBig, typeof(Texture2D), false);
				EditorGUILayout.Space();

				characterInfo.alternativeColor = EditorGUILayout.ColorField("Alternative Color:", characterInfo.alternativeColor);
				characterInfo.deathSound = (AudioClip) EditorGUILayout.ObjectField("Death Sound:", characterInfo.deathSound, typeof(UnityEngine.AudioClip), false);
				
				EditorGUIUtility.labelWidth = 150;
				EditorGUILayout.Space();
				GUILayout.Label("Description:");
				Rect rect = GUILayoutUtility.GetRect(50, 70);
				EditorStyles.textField.wordWrap = true;
				characterInfo.characterDescription = EditorGUI.TextArea(rect, characterInfo.characterDescription);
				//characterInfo.characterDescription = EditorGUILayout.TextArea(characterInfo.characterDescription, GUILayout.Height(50), GUILayout.Width(Screen.width - 64));
				//EditorGUIUtility.labelWidth = 180;

				EditorGUILayout.Space();
			}EditorGUILayout.EndVertical();


			// Hit Boxes
			EditorGUILayout.BeginVertical(rootGroupStyle);{
				EditorGUILayout.BeginHorizontal();{
					hitBoxesOption = EditorGUILayout.Foldout(hitBoxesOption, "Hit Box Setup", foldStyle);
					helpButton("character:hitbox");
				}EditorGUILayout.EndHorizontal();

				if (hitBoxesOption){
					EditorGUILayout.BeginVertical(subGroupStyle);{
						EditorGUI.indentLevel += 1;
					
						characterInfo.characterPrefab = (GameObject) EditorGUILayout.ObjectField("Character Prefab:", characterInfo.characterPrefab, typeof(UnityEngine.GameObject), true);
						if (characterInfo.characterPrefab != null){
							if (PrefabUtility.GetPrefabType(characterInfo.characterPrefab) != PrefabType.Prefab){
								characterWarning = true;
								errorMsg = "This character is not a prefab.";
								characterInfo.characterPrefab = null;
								ClosePreview();
							}else if (characterInfo.characterPrefab.GetComponent<HitBoxesScript>() == null){
								characterWarning = true;
								errorMsg = "This character doesn't have hitboxes!\n Please add the HitboxScript and try again.";
								characterInfo.characterPrefab = null;
								ClosePreview();
							}else if (character != null && EditorApplication.isPlayingOrWillChangePlaymode) {
								characterWarning = true;
								errorMsg = "You can't change this field while in play mode.";
								ClosePreview();
							}else {
								characterWarning = false;
								if (character != null && characterInfo.characterPrefab.name != character.name) ClosePreview();
							}
						}

						if (characterWarning){
							GUILayout.BeginHorizontal("GroupBox");
							GUILayout.Label(errorMsg, "CN EntryWarn");
							GUILayout.EndHorizontal();
						}

						if (characterInfo.characterPrefab != null){
							if (!characterPreviewToggle){
								if (StyledButton("Open Character")) {
									EditorWindow.FocusWindowIfItsOpen<SceneView>();
									PreviewCharacter();
								}
							}else{
								if (StyledButton("Close Character")) ClosePreview();
							}
							
							if (character != null){
								EditorGUILayout.BeginVertical(subGroupStyle);{	
									EditorGUILayout.Space();
									transformToggle = EditorGUILayout.Foldout(transformToggle, "Transform", EditorStyles.foldout);
									if (transformToggle){
										EditorGUILayout.BeginVertical(subGroupStyle);{
											EditorGUI.indentLevel += 1;
											character.transform.position = EditorGUILayout.Vector3Field("Position", character.transform.position);
											character.transform.rotation = Quaternion.Euler(EditorGUILayout.Vector3Field("Rotation", character.transform.rotation.eulerAngles));
											character.transform.localScale = EditorGUILayout.Vector3Field("Scale", character.transform.localScale);
											EditorGUI.indentLevel -= 1;
										}EditorGUILayout.EndVertical();
									}
									
									EditorGUILayout.Space();
									hitBoxesToggle = EditorGUILayout.Foldout(hitBoxesToggle, "Hit Boxes", EditorStyles.foldout);
									if (hitBoxesToggle){
										EditorGUILayout.BeginVertical(subGroupStyle);{
											HitBoxesScript hitBoxesScript = character.GetComponent<HitBoxesScript>();
											for (int i = 0; i < hitBoxesScript.hitBoxes.Length; i ++){
												EditorGUILayout.Space();
												EditorGUILayout.BeginVertical(subArrayElementStyle);{
													EditorGUILayout.Space();
													EditorGUILayout.BeginHorizontal();{
														hitBoxesScript.hitBoxes[i].bodyPart = (BodyPart) EditorGUILayout.EnumPopup("Body Part:", hitBoxesScript.hitBoxes[i].bodyPart, enumStyle);
														if (GUILayout.Button("", removeButtonStyle)){
															hitBoxesScript.hitBoxes = RemoveElement<HitBox>(hitBoxesScript.hitBoxes, hitBoxesScript.hitBoxes[i]);
															return;
														}
													}EditorGUILayout.EndHorizontal();
													hitBoxesScript.hitBoxes[i].collisionType = (CollisionType) EditorGUILayout.EnumPopup("Collision Type:", hitBoxesScript.hitBoxes[i].collisionType, enumStyle);
													hitBoxesScript.hitBoxes[i].type = (HitBoxType) EditorGUILayout.EnumPopup("Hit Box Type:", hitBoxesScript.hitBoxes[i].type, enumStyle);
													hitBoxesScript.hitBoxes[i].offSet = EditorGUILayout.Vector2Field("Off Set:", hitBoxesScript.hitBoxes[i].offSet);
													hitBoxesScript.hitBoxes[i].radius = EditorGUILayout.Slider("Radius:", hitBoxesScript.hitBoxes[i].radius, .1f, 5);
													hitBoxesScript.hitBoxes[i].position = (Transform) EditorGUILayout.ObjectField("Link:", hitBoxesScript.hitBoxes[i].position, typeof(UnityEngine.Transform), true);
													EditorGUILayout.Space();
												}EditorGUILayout.EndVertical();
											}
											if (StyledButton("New Hit Box"))
												hitBoxesScript.hitBoxes = AddElement<HitBox>(hitBoxesScript.hitBoxes, new HitBox());
											
										}EditorGUILayout.EndVertical();
									}
									
									EditorGUILayout.Space();
									EditorGUILayout.BeginHorizontal();{
										if (StyledButton("Reset Scene View")){
											EditorCamera.SetPosition(Vector3.up * 3.5f);
											EditorCamera.SetRotation(Quaternion.identity);
											EditorCamera.SetOrthographic(true);
											EditorCamera.SetSize(5);
										}
										if (StyledButton("Apply Changes")){
											PrefabUtility.ReplacePrefab(character, PrefabUtility.GetPrefabParent(character), ReplacePrefabOptions.ConnectToPrefab);
										}
									}EditorGUILayout.EndHorizontal();
									EditorGUILayout.Space();
								}EditorGUILayout.EndVertical();
							}
						}

						EditorGUI.indentLevel -= 1;
					}EditorGUILayout.EndVertical();
				}else{
					ClosePreview();
				}
			}EditorGUILayout.EndVertical();

			// Physics
			EditorGUILayout.BeginVertical(rootGroupStyle);{
				EditorGUILayout.BeginHorizontal();{
					physicsOption = EditorGUILayout.Foldout(physicsOption, "Physics", foldStyle);
					helpButton("character:physics");
				}EditorGUILayout.EndHorizontal();

				if (physicsOption){
					EditorGUILayout.BeginVertical(subGroupStyle);{
						EditorGUILayout.Space();
						EditorGUI.indentLevel += 1;
						characterInfo.physics.moveForwardSpeed = EditorGUILayout.FloatField("Move Forward Speed:", characterInfo.physics.moveForwardSpeed);
						characterInfo.physics.moveBackSpeed = EditorGUILayout.FloatField("Move Back Speed:", characterInfo.physics.moveBackSpeed);
						characterInfo.physics.highMovingFriction = EditorGUILayout.Toggle("High Moving Friction", characterInfo.physics.highMovingFriction);
						characterInfo.physics.friction = EditorGUILayout.FloatField("Friction:", characterInfo.physics.friction);
						characterInfo.physics.jumpForce = EditorGUILayout.FloatField("Jump Force:", characterInfo.physics.jumpForce);
						characterInfo.physics.jumpDistance = EditorGUILayout.FloatField("Jump Distance:", characterInfo.physics.jumpDistance);
						characterInfo.physics.multiJumps = EditorGUILayout.IntField("Air Jumps:", characterInfo.physics.multiJumps);
						characterInfo.physics.weight = EditorGUILayout.FloatField("Character's Weight:", characterInfo.physics.weight);
						characterInfo.physics.cumulativeForce = EditorGUILayout.Toggle("Cumulative Force", characterInfo.physics.cumulativeForce);
						EditorGUILayout.Space();
						EditorGUI.indentLevel -= 1;
					}EditorGUILayout.EndVertical();
				}
			}EditorGUILayout.EndVertical();


			// Move Sets
			EditorGUILayout.BeginVertical(rootGroupStyle);{
				EditorGUILayout.BeginHorizontal();{
					moveSetOption = EditorGUILayout.Foldout(moveSetOption, "Move Sets ("+ characterInfo.moves.Length +")", foldStyle);
					helpButton("character:movesets");
				}EditorGUILayout.EndHorizontal();

				if (moveSetOption){
					EditorGUILayout.BeginVertical(subGroupStyle);{
						EditorGUILayout.Space();
						EditorGUI.indentLevel += 1;
						// content
						characterInfo.executionTiming = EditorGUILayout.FloatField("Execution Timing:", characterInfo.executionTiming);
						characterInfo.chargeTiming = EditorGUILayout.FloatField("Charge Timing:", characterInfo.chargeTiming);
						characterInfo.possibleAirMoves = EditorGUILayout.IntField("Possible Air Moves:", characterInfo.possibleAirMoves);
						characterInfo.interpolationSpeed = EditorGUILayout.FloatField("Interpolation Speed:", characterInfo.interpolationSpeed);

						EditorGUI.indentLevel += 1;
						for (int i = 0; i < characterInfo.moves.Length; i ++){
							EditorGUILayout.Space();
							EditorGUILayout.BeginVertical(arrayElementStyle);{
								EditorGUILayout.Space();
								EditorGUILayout.BeginHorizontal();{
									characterInfo.moves[i].combatStance = (CombatStances)EditorGUILayout.EnumPopup("Combat Stance:", characterInfo.moves[i].combatStance, enumStyle);
									if (GUILayout.Button("", removeButtonStyle)){
										characterInfo.moves = RemoveElement<MoveSetData>(characterInfo.moves, characterInfo.moves[i]);
										return;
									}
								}EditorGUILayout.EndHorizontal();

								
								characterInfo.moves[i].cinematicIntro = (MoveInfo)EditorGUILayout.ObjectField("Cinematic Intro:", characterInfo.moves[i].cinematicIntro, typeof(MoveInfo), false);
								characterInfo.moves[i].cinematicOutro = (MoveInfo)EditorGUILayout.ObjectField("Cinematic Outro:", characterInfo.moves[i].cinematicOutro, typeof(MoveInfo), false);

								EditorGUILayout.Space();
								characterInfo.moves[i].basicMovesToggle = EditorGUILayout.Foldout(characterInfo.moves[i].basicMovesToggle, "Basic Moves", foldStyle);
								if (characterInfo.moves[i].basicMovesToggle){
									EditorGUILayout.BeginVertical(subGroupStyle);{
										EditorGUI.indentLevel += 1;
										EditorGUILayout.Space();

										basicMoveBlock("Idle", characterInfo.moves[i].basicMoves.idle, false, true);
										basicMoveBlock("Move Forward", characterInfo.moves[i].basicMoves.moveForward, false, true);
										basicMoveBlock("Move Back", characterInfo.moves[i].basicMoves.moveBack, false, true);
										basicMoveBlock("Jumping", characterInfo.moves[i].basicMoves.jumping, true, true);
										basicMoveBlock("Falling", characterInfo.moves[i].basicMoves.falling, true, true);
										basicMoveBlock("Landing", characterInfo.moves[i].basicMoves.landing, true, true);
										basicMoveBlock("Crouching", characterInfo.moves[i].basicMoves.crouching, false, true);
										basicMoveBlock("Blocking Low Pose", characterInfo.moves[i].basicMoves.blockingLowPose, false, true);
										basicMoveBlock("Blocking Low Hit", characterInfo.moves[i].basicMoves.blockingLowHit, true, true);
										basicMoveBlock("Blocking High Pose", characterInfo.moves[i].basicMoves.blockingHighPose, false, true);
										basicMoveBlock("Blocking High Hit", characterInfo.moves[i].basicMoves.blockingHighHit, true, true);
										basicMoveBlock("Blocking Air Pose", characterInfo.moves[i].basicMoves.blockingAirPose, false, true);
										basicMoveBlock("Blocking Air Hit", characterInfo.moves[i].basicMoves.blockingAirHit, true, true);
										basicMoveBlock("Parry Crouching", characterInfo.moves[i].basicMoves.parryCrouching, true, true);
										basicMoveBlock("Parry High", characterInfo.moves[i].basicMoves.parryHigh, true, true);
										basicMoveBlock("Parry Air", characterInfo.moves[i].basicMoves.parryAir, true, true);
										basicMoveBlock("Get Hit Crouching", characterInfo.moves[i].basicMoves.getHitCrouching, true, true);
										basicMoveBlock("Get Hit High", characterInfo.moves[i].basicMoves.getHitHigh, true, true);
										basicMoveBlock("Get Hit Air", characterInfo.moves[i].basicMoves.getHitAir, true, true);
										basicMoveBlock("Fall Down", characterInfo.moves[i].basicMoves.fallDown, true, true);
										basicMoveBlock("Get Up", characterInfo.moves[i].basicMoves.getUp, true, true);
										basicMoveBlock("Bounce", characterInfo.moves[i].basicMoves.bounce, true, true);
										basicMoveBlock("Falling From Bounce", characterInfo.moves[i].basicMoves.fallingFromBounce, true, true);

										EditorGUI.indentLevel -= 1;
										EditorGUILayout.Space();
										
									}EditorGUILayout.EndVertical();
								}

								characterInfo.moves[i].attackMovesToggle = EditorGUILayout.Foldout(characterInfo.moves[i].attackMovesToggle, "Attack & Special Moves ("+ characterInfo.moves[i].attackMoves.Length +")", foldStyle);
								if (characterInfo.moves[i].attackMovesToggle){
									EditorGUILayout.BeginVertical(subGroupStyle);{
										EditorGUILayout.Space();
										EditorGUI.indentLevel += 1;
										
										for (int y = 0; y < characterInfo.moves[i].attackMoves.Length; y ++){
											EditorGUILayout.Space();
											EditorGUILayout.BeginVertical(subArrayElementStyle);{
												EditorGUILayout.Space();
												EditorGUILayout.BeginHorizontal();{
													characterInfo.moves[i].attackMoves[y] = (MoveInfo)EditorGUILayout.ObjectField("Move File:", characterInfo.moves[i].attackMoves[y], typeof(MoveInfo), false);
													if (GUILayout.Button("", removeButtonStyle)){
														characterInfo.moves[i].attackMoves = RemoveElement<MoveInfo>(characterInfo.moves[i].attackMoves, characterInfo.moves[i].attackMoves[y]);
														return;
													}
												}EditorGUILayout.EndHorizontal();
												
												if (GUILayout.Button("Open in the Move Editor")) {
													MoveEditorWindow.sentMoveInfo = characterInfo.moves[i].attackMoves[y];
													MoveEditorWindow.Init();
												}
											}EditorGUILayout.EndVertical();
										}
										EditorGUILayout.Space();
										if (StyledButton("New Move"))
											characterInfo.moves[i].attackMoves = AddElement<MoveInfo>(characterInfo.moves[i].attackMoves, null);

										EditorGUILayout.Space();
										EditorGUI.indentLevel -= 1;
									}EditorGUILayout.EndVertical();
								}
								EditorGUILayout.Space();
							}EditorGUILayout.EndVertical();
						}
						EditorGUILayout.Space();
						if (StyledButton("New Move Set"))
							characterInfo.moves = AddElement<MoveSetData>(characterInfo.moves, new MoveSetData());
						
						EditorGUILayout.Space();
						EditorGUI.indentLevel -= 1;

					}EditorGUILayout.EndVertical();
				}
				
			}EditorGUILayout.EndVertical();

		}EditorGUILayout.EndScrollView();


		if (GUI.changed) {
			Undo.RecordObject(characterInfo, "Character Editor Modify");
			EditorUtility.SetDirty(characterInfo);
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

	public void basicMoveBlock(string label, BasicMoveInfo basicMove, bool speedToggle, bool hasSound){
		basicMove.editorToggle = EditorGUILayout.Foldout(basicMove.editorToggle, label, foldStyle);
		if (basicMove.editorToggle){
			EditorGUILayout.BeginVertical(subArrayElementStyle);{
				EditorGUILayout.Space();
				EditorGUI.indentLevel += 1;
				EditorGUIUtility.labelWidth = 180;
				basicMove.animationClip = (AnimationClip) EditorGUILayout.ObjectField("Animation Clip:", basicMove.animationClip, typeof(UnityEngine.AnimationClip), false);

				if (speedToggle){
					EditorGUI.BeginDisabledGroup(true);
					EditorGUILayout.TextField("Animation Speed:", "(Automatic)");
					EditorGUI.EndDisabledGroup();
				}else{
					basicMove.animationSpeed = EditorGUILayout.FloatField("Animation Speed:", basicMove.animationSpeed);
				}

				if (hasSound){
					basicMove.soundEffect = (AudioClip) EditorGUILayout.ObjectField("Sound Effect:", basicMove.soundEffect, typeof(UnityEngine.AudioClip), true);
					basicMove.continuousSound = EditorGUILayout.Toggle("Continuous Sound", basicMove.continuousSound, toggleStyle);
				}

				basicMove.particleEffect.editorToggle = EditorGUILayout.Foldout(basicMove.particleEffect.editorToggle, "Particle Effect", foldStyle);
				if (basicMove.particleEffect.editorToggle){
					EditorGUILayout.BeginVertical(subGroupStyle);{
						EditorGUILayout.Space();
						
						basicMove.particleEffect.prefab = (GameObject) EditorGUILayout.ObjectField("Particle Prefab:", basicMove.particleEffect.prefab, typeof(UnityEngine.GameObject), true);
						basicMove.particleEffect.duration = EditorGUILayout.FloatField("Duration (seconds):", basicMove.particleEffect.duration);
						basicMove.particleEffect.position = EditorGUILayout.Vector3Field("Position (relative):", basicMove.particleEffect.position);

						EditorGUILayout.Space();
					}EditorGUILayout.EndVertical();
				}
				
				EditorGUIUtility.labelWidth = 150;
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