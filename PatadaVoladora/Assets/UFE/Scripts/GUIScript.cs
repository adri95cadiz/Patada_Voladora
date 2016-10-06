using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class GUIScript : MonoBehaviour {
	public GUISkin customSkin;

	//You can copy or make a reference to any of the live variables in the system

	// Player 1 Variables
	private CharacterInfo player1;
	private Vector2 player1NameLocation;
	private Vector2 player1AlertLocation;
	private TextAnchor player1Anchor;
	private float player1TargetLife;
	private float player1TotalLife;
	private GameObject player1AlertGO;
	private GameObject player1NameGO;

	// Player 2 Variables
	private CharacterInfo player2;
	private Vector3 player2NameLocation;
	private Vector3 player2AlertLocation;
	private TextAnchor player2Anchor;
	private float player2TargetLife;
	private float player2TotalLife;
	private GameObject player2AlertGO;
	private GameObject player2NameGO;
	private GameObject infoGO;

	// Main Alert Variables
	private GameObject mainAlertGO;
	private Vector2 mainAlertLocation;
	private bool showEndMenu;
	private bool showControls;
	//private bool showSpecials;

	private bool isRunning; // Checks if the game is running

	void Awake () {
		// Subscribe to the events from UFE
		/* Possible Events:
		 * OnLifePointsChange(float newLifePoints, CharacterInfo player)
		 * OnNewAlert(string alertMessage, CharacterInfo player)
		 * OnHit(MoveInfo move, CharacterInfo hitter)
		 * OnMove(MoveInfo move, CharacterInfo player)
		 * OnRoundEnds(CharacterInfo winner, CharacterInfo loser)
		 * OnRoundBegins(int roundNumber)
		 * OnGameEnds(CharacterInfo winner, CharacterInfo loser)
		 * OnGameBegins(CharacterInfo player1, CharacterInfo player2, StageOptions stage)
		 * 
		 * usage:
		 * UFE.OnMove += YourFunctionHere;
		 * .
		 * .
		 * void YourFunctionHere(T param1, T param2){...}
		 * 
		 * The following code bellow show more usage examples
		 */
		UFE.OnGameBegin += OnGameBegins;
		UFE.OnGameEnds += OnGameEnds;
		UFE.OnRoundBegins += OnRoundBegins;
		UFE.OnRoundEnds += OnRoundEnds;
		UFE.OnLifePointsChange += OnLifePointsChange;
		UFE.OnNewAlert += OnNewAlert;
		UFE.OnHit += OnHit;
		UFE.OnMove += OnMove;
	}

	void OnGameBegins(CharacterInfo player1, CharacterInfo player2, StageOptions stage){
		/* 
		 * You can have access to any variable at runtime using UFE.config
		 * In this case we can either use the parameter received "player1" or access it directly with UFE.config.player1
		 * Check the documentation for details over each variable.
		 */
		this.player1 = player1;
		player1TargetLife = player1.lifePoints;
		player1TotalLife = player1.lifePoints;
		player1NameLocation = new Vector2(.02f, .93f);
		player1AlertLocation = new Vector2(.06f, .76f);
		player1Anchor = TextAnchor.UpperLeft;
		player1NameGO = CreateText(player1.characterName, UFE.config.guiOptions.characterNameFont, 1, player1NameLocation, player1Anchor);
		
		this.player2 = player2;
		player2TargetLife = player2.lifePoints;
		player2TotalLife = player2.lifePoints;
		player2NameLocation = new Vector2(.98f, .93f);
		player2AlertLocation = new Vector2(.94f, .76f);
		player2Anchor = TextAnchor.UpperRight;
		player2NameGO = CreateText(player2.characterName, UFE.config.guiOptions.characterNameFont, 1, player2NameLocation, player2Anchor);

		mainAlertLocation = new Vector2(.5f, .6f);


		infoGO = new GameObject("Info");
		infoGO.AddComponent<GUIText>();
		GUIText info = infoGO.GetComponent<GUIText>();
		info.pixelOffset = new Vector2((Screen.width/2) - 60 * ((float)Screen.width/1280), (Screen.height) - 70 * ((float)Screen.height/720));
		info.text = "ESC - Menu";
		info.color = Color.black;

		isRunning = true;
	}
	
	void OnGameEnds(CharacterInfo winner, CharacterInfo loser){
		isRunning = false;
		Destroy(player1NameGO);
		Destroy(player2NameGO);
		Destroy(infoGO);
        showEndMenu = true;
	}
    
    void OnRoundBegins(int round){
		// Fires when a round begins
		if (player1AlertGO != null) Destroy(player1AlertGO);
		if (player2AlertGO != null) Destroy(player2AlertGO);
	}
	
	void OnRoundEnds(CharacterInfo winner, CharacterInfo loser){
		// Fires when a round ends
		// TODO add round counter to show how many rounds a player has won
	}

	void OnMove(MoveInfo move, CharacterInfo player){
		// Fires when a player successfully executes a move
	}

	void OnNewAlert(string alertMsg, CharacterInfo player){
		// You can use this to have your own custom events when a new text alert is fired from the engine
		if (player == player1){
			Vector3 startingLocation = player1AlertLocation;
			startingLocation.x -= .1f;
			player1AlertGO = CreateAlert(player1AlertGO, alertMsg, 2, startingLocation, player1Anchor);
		}else if (player == player2){
			Vector3 startingLocation = player2AlertLocation;
			startingLocation.x += .1f;
			player2AlertGO = CreateAlert(player2AlertGO, alertMsg, 2, startingLocation, player2Anchor);
		}else if (alertMsg.IndexOf("Round") != -1){
			Vector3 startingLocation = mainAlertLocation;
			startingLocation.z = 1;
			mainAlertGO = CreateAlert(mainAlertGO, alertMsg, 2, startingLocation, TextAnchor.MiddleCenter);
		}else if (alertMsg == UFE.config.selectedLanguage.fight){
			Vector3 startingLocation = mainAlertLocation;
			startingLocation.z = 1;
			mainAlertGO = CreateAlert(mainAlertGO, alertMsg, 1, startingLocation, TextAnchor.MiddleCenter);
		}else{
			Vector3 startingLocation = mainAlertLocation;
			startingLocation.z = 1;
			mainAlertGO = CreateAlert(mainAlertGO, alertMsg, 60, startingLocation, TextAnchor.MiddleCenter);
		}
	}
	
	void OnLifePointsChange(float newLife, CharacterInfo player){
		// You can use this to have your own custom events when a player's life points changes
	}

	void OnHit(HitBox strokeHitBox, MoveInfo move, CharacterInfo hitter){
		// You can use this to have your own custom events when a character gets hit
	}

	void Update () {
		if (!isRunning) return;
		// Animate the alert message if it exists
		if (player1AlertGO != null) GOTween(player1AlertGO, player1AlertLocation, 15);
		if (player2AlertGO != null) GOTween(player2AlertGO, player2AlertLocation, 15);

		// Animate life points when it goes down (P1)
		if (player1TargetLife > UFE.config.player1Character.currentLifePoints) player1TargetLife -= 5; // The speed the life moves down
		if (player1TargetLife < UFE.config.player1Character.currentLifePoints) player1TargetLife = UFE.config.player1Character.currentLifePoints;

		// Animate life points when it goes down (P2)
		if (player2TargetLife > UFE.config.player2Character.currentLifePoints) player2TargetLife -= 5; // The speed the life moves down
		if (player2TargetLife < UFE.config.player2Character.currentLifePoints) player2TargetLife = UFE.config.player2Character.currentLifePoints;
		
		if(Input.GetKeyDown(KeyCode.Escape) && !UFE.isPaused()){
			UFE.PauseGame(true);
		}else if(Input.GetKeyDown(KeyCode.Escape)){
			UFE.PauseGame(false);
		}
	}


	void OnGUI () {
		GUI.skin = customSkin;
		GUI.skin.label.fontSize = 20;
		

		if (showControls && UFE.isPaused()){
			GUI.BeginGroup(new Rect(Screen.width / 2 - 250, Screen.height / 2 - 100, 500, 400));{
				GUI.Box(new Rect(0, 0, 500, 200), "Controles"); 
				GUI.BeginGroup(new Rect(15, 0, 480, 200));{
					GUILayoutUtility.GetRect(1,25, GUILayout.Width(470));
                    { 
						GUILayout.BeginHorizontal();{
							GUILayout.Label("Jugador 1");
							GUILayout.FlexibleSpace();
							GUILayout.Label("Jugador 2");
						}GUILayout.EndHorizontal();

						GUILayout.BeginHorizontal();{
							GUILayout.Label("Mover - W A S D");
							GUILayout.FlexibleSpace();
							GUILayout.Label("Mover - Flechas de direccion");
						}GUILayout.EndHorizontal();
						GUILayout.BeginHorizontal();{
							GUILayout.Label("Patada Voladora - T");
							GUILayout.FlexibleSpace();
							GUILayout.Label("Patada Voladora - Insert");
						}GUILayout.EndHorizontal();
					}

                    GUILayoutUtility.GetRect(1, 30);
                    { 
					GUILayout.BeginHorizontal();{
						GUILayout.FlexibleSpace();
						if (GUILayout.Button("Volver")) showControls = false;
                        GUILayout.FlexibleSpace();
                        }
                        GUILayout.EndHorizontal();
					}
				}GUI.EndGroup();
			}GUI.EndGroup();

		}else if (!showEndMenu && UFE.isPaused()){
			GUI.BeginGroup(new Rect(Screen.width / 2 - 200, Screen.height / 2 - 130, 400, 260));{
				GUI.Box(new Rect(0, 0, 400, 260), "Opciones");
				GUI.BeginGroup(new Rect(15, 0, 380, 260));{
					GUILayoutUtility.GetRect(1,45);
					
					GUILayout.BeginHorizontal();{
						GUILayout.Label("Musica", GUILayout.Width(240));
						if (UFE.GetMusic()){
							if (GUILayout.Button("On", GUILayout.Width(120))) UFE.SetMusic(false);
						}else{
							if (GUILayout.Button("Off", GUILayout.Width(120))) UFE.SetMusic(true);
						}
					}GUILayout.EndHorizontal();
					
					if (UFE.GetMusic()){
						GUILayout.BeginHorizontal();{
							GUILayout.Label("Volumen de la musica", GUILayout.Width(240));
							UFE.SetVolume(GUILayout.HorizontalSlider(UFE.GetVolume(), 0, 1, GUILayout.Width(120)));
						}GUILayout.EndHorizontal();
					}else{
						GUILayoutUtility.GetRect(1,34);
					}
					
					GUILayout.BeginHorizontal();{
						GUILayout.Label("Efectos de Sonido", GUILayout.Width(240));
						if (UFE.GetSoundFX()){
							if (GUILayout.Button("On", GUILayout.Width(120))) UFE.SetSoundFX(false);
						}else{
							if (GUILayout.Button("Off", GUILayout.Width(120))) UFE.SetSoundFX(true);
						}
					}GUILayout.EndHorizontal();
					
					GUILayoutUtility.GetRect(1,30);

					GUILayout.BeginHorizontal();{
						GUILayout.FlexibleSpace();
						if (GUILayout.Button("Controles", GUILayout.Width(200))) showControls = true;
						GUILayout.FlexibleSpace();
					}GUILayout.EndHorizontal();

                    GUILayout.BeginHorizontal();
                    {
                        GUILayout.FlexibleSpace();
                        if (GUILayout.Button("Reiniciar")) SceneManager.LoadScene("TrainingRoom");
                        GUILayout.FlexibleSpace();
                    }
                    GUILayout.EndHorizontal();

                    GUILayout.BeginHorizontal();
                    {
                        GUILayout.FlexibleSpace();
                        if (GUILayout.Button("Salir")) Application.Quit();
                        GUILayout.FlexibleSpace();
                    }
                    GUILayout.EndHorizontal();

                    GUILayout.BeginHorizontal();{
						GUILayout.FlexibleSpace();
						if (GUILayout.Button("Cerrar")) UFE.PauseGame(false);
						GUILayout.FlexibleSpace();
					}GUILayout.EndHorizontal();

				}GUI.EndGroup();
			}GUI.EndGroup();
		}

		if (showEndMenu){
			GUI.BeginGroup(new Rect(Screen.width / 2 - 100, Screen.height / 2 + 20, 200, 130));{
				GUI.Box(new Rect(0, 0, 200, 100), "");
                {
                    GUILayoutUtility.GetRect(1, 20);
                    GUILayout.BeginHorizontal();{ 
						GUILayout.FlexibleSpace();
						if (GUILayout.Button("Revancha", GUILayout.Width(200))) {
                            SceneManager.LoadScene("TrainingRoom");
                        }
						GUILayout.FlexibleSpace();
					}GUILayout.EndHorizontal();

                    GUILayoutUtility.GetRect(1, 20);
                    GUILayout.BeginHorizontal();
                    {
                        GUILayout.FlexibleSpace();
                        if (GUILayout.Button("Salir", GUILayout.Width(200)))
                        {
                            Application.Quit();
                        }
                        GUILayout.FlexibleSpace();
                    }
                    GUILayout.EndHorizontal();
                }
			}GUI.EndGroup();
		}
		
		if (!isRunning) return;
		// Draw the lifebars and gauge bars using the data stored in UFE.config.guiOptions
		DrawBar(UFE.config.guiOptions.lifeBarOptions1, Side.Left, player1TargetLife, player1TotalLife, true);
		DrawBar(UFE.config.guiOptions.lifeBarOptions2, Side.Right, player2TargetLife, player2TotalLife, true);
		
		DrawBar(UFE.config.guiOptions.gaugeBarOptions1, Side.Left, 
		        UFE.config.player1Character.currentGaugePoints, UFE.config.player2Character.maxGaugePoints, false);
		DrawBar(UFE.config.guiOptions.gaugeBarOptions1, Side.Right, 
		        UFE.config.player2Character.currentGaugePoints, UFE.config.player2Character.maxGaugePoints, false);
	}
	
	void GOTween(GameObject gameObject, Vector3 destination, float speed){
		// Lerp effect to move the alert text
		gameObject.transform.position = Vector3.Lerp(gameObject.transform.position, destination, Time.deltaTime * speed);
	}

	void DrawBar(GUIBarOptions guiBar, Side side, float currentValue, float totalValue, bool topGUI){
		// A custom method to create the draining bar
		Rect backRect = SetResolution(guiBar.backgroundRect);
		Rect fillRect = SetResolution(guiBar.fillRect);

		Rect remainFill;
		float newWidth = (currentValue/totalValue) * fillRect.width;
		float leftAdjustment = fillRect.width - newWidth;
		if (side == Side.Right){
			backRect.x = Screen.width - backRect.width - backRect.x;
			remainFill = new Rect(fillRect.x, fillRect.y, newWidth, fillRect.height);
		}else{
			float newXPos = fillRect.x + (fillRect.width - newWidth);
			remainFill = new Rect(newXPos, fillRect.y, (currentValue/totalValue) * fillRect.width, fillRect.height);
		}

		if (!topGUI) backRect.y = Screen.height - backRect.height - backRect.y;

		GUI.DrawTexture(backRect, guiBar.backgroundImage);
		GUI.BeginGroup(backRect);{
			GUI.BeginGroup(remainFill);{
				if (side == Side.Right){
					GUI.DrawTexture(new Rect(0,0, fillRect.width, fillRect.height), guiBar.fillImage, ScaleMode.ScaleAndCrop);
				}else{
					GUI.DrawTexture(new Rect(-leftAdjustment,0, fillRect.width, fillRect.height), guiBar.fillImage, ScaleMode.ScaleAndCrop);
				}
			}GUI.EndGroup();
		}GUI.EndGroup();
	}
	
	Rect SetResolution(Rect rect){
		// Adjusts the texture's size and position according to the size of the window
		rect.x *= ((float)Screen.width/1280);
		rect.y *= ((float)Screen.height/720);
		rect.width *= ((float)Screen.width/1280);
		rect.height *= ((float)Screen.height/720);
		return rect;
	}

	private GameObject CreateAlert(GameObject alertGO, string msg, float killTime, Vector3 newPosition, TextAnchor anchor){
		// A custom method that creates and animates the alert message
		if (alertGO != null) Destroy(alertGO);
		alertGO = CreateText(msg, UFE.config.guiOptions.alertFont, 1, newPosition, anchor);
		if (killTime > 0) Destroy(alertGO, killTime);
		return alertGO;
	}
	
	private GameObject CreateText(string txt, FontId fontId, float size, Vector3 newPosition, TextAnchor newAchor){
		// A custom method that creates a gameobject based on the fontPrefab and write the string in it
		FontOptions fontOptions = UFE.GetFont(fontId);
		if (fontOptions.fontPrefab == null) Debug.LogError("Font prefab not found! Make sure you have all font prefabs set in the Global Editor");
		GameObject guiTextGO = (GameObject)Instantiate(fontOptions.fontPrefab, newPosition, Quaternion.identity);
		guiTextGO.GetComponent<GUIText>().anchor = newAchor;
		guiTextGO.GetComponent<GUIText>().text = txt;
		guiTextGO.transform.localScale *= size;
		
		return guiTextGO;
	}
}
