using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Advertisements;

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;

public class RandomPhraseFrontEnd : MonoBehaviour {

	#region Fields

	//======================================================
	//Public Variables

	public TextAsset TextFile;

	public TextAsset NounText;
	public TextAsset AdverbText;
	public TextAsset VerbsText;
	public TextAsset AdjText;

	public List<GameObject> Panels = new List<GameObject>();

	/*
	 * 0: Menu
	 * 1: Generate
	 * 2: Solution
	 * 3: Options
	 * 4: Add Word
	 * 5: Delete Word
	 */

	public GameObject AcronymInput;
	public Transform SolutionContent;
	public GameObject SolutionText;

	public GameObject MainBackground;
	public GameObject BlackImage;

	public GameObject Setting1Dropdown;

	public GameObject AddWordInput;
	public GameObject AddWordDropdown;

	public GameObject DeleteWordInput;
	public Transform DeleteSearchListContent;
	public GameObject ButtonPrefab;
	public GameObject DeleteConfirmation;

	public GameObject ResetConfirmation;

	//======================================================
	//Private Variables

	private MyDictionary dictionary;

	private string SettingsFilePath;
	private string UserAddFilePath;
	private string UserDeleteFilePath;

	private int adCount = 20;
	// used to count ad pop up

	private ScreenOrientation currentOrientation;

	private bool showMain;
	private bool hideMain;
	private float alpha;

	private SceneOptions sceneOptions = new SceneOptions();

	private List<string> userAddList_Random = new List<string>();
	private List<string> userDeleteList_Random = new List<string>();

	private List<string> userAddList_Noun = new List<string>();
	private List<string> userDeleteList_Noun = new List<string>();

	private List<string> userAddList_Verb = new List<string>();
	private List<string> userDeleteList_Verb = new List<string>();

	private List<string> userAddList_Adjective = new List<string>();
	private List<string> userDeleteList_Adjective = new List<string>();

	private List<string> userAddList_Adverb = new List<string>();
	private List<string> userDeleteList_Adverb = new List<string>();

	private bool searchDone = true;
	private bool updateWaiting = false;
	private string wordToDelete;

	private float m_fScaleFactor;

	//======================================================

	#endregion

	#region Methods

	// Use this for initialization
	void Start() {

		Application.targetFrameRate = 60; //fps
		currentOrientation = Screen.orientation;

		showMain = false;
		hideMain = false;

		alpha = 255;
		MainBackground.GetComponent<SpriteRenderer>().color = new Color(255, 255, 255, alpha);
		BlackImage.SetActive(false);

		Panels[0].SetActive(true);
		Panels[1].SetActive(false);
		Panels[2].SetActive(false);
		Panels[3].SetActive(false);
		Panels[4].SetActive(false);
		Panels[5].SetActive(false);
		DeleteConfirmation.SetActive(false);
		ResetConfirmation.SetActive(false);

		Panels[0].GetComponent<MyPanel>().ForcePosition(getScreenCenter());
		Panels[1].GetComponent<MyPanel>().ForcePosition(getScreenCenter());
		Panels[2].GetComponent<MyPanel>().ForcePosition(getScreenCenter());
		Panels[3].GetComponent<MyPanel>().ForcePosition(new Vector3(Screen.width / 2.0f, -Screen.height * 0.5f, 0));
		Panels[4].GetComponent<MyPanel>().ForcePosition(getScreenCenter());
		Panels[5].GetComponent<MyPanel>().ForcePosition(getScreenCenter());

		SettingsFilePath = Application.persistentDataPath + "/UserSettings.txt";
		UserAddFilePath = Application.persistentDataPath + "/UserDictAdd.txt";
		UserDeleteFilePath = Application.persistentDataPath + "/UserDictDelete.txt";

		dictionary = new MyDictionary(TextFile, NounText, VerbsText, AdverbText, AdjText);

		//Open User Settings File
		OpenSettingsFile();

		//Open User Dictionary Add & Delete Files
		OpenUserDictionaryFiles();

		dictionary.UpdateDictionaryFromUserRandoms(userAddList_Random, userDeleteList_Random);
		dictionary.UpdateDictionaryFromUserNouns(userAddList_Noun, userDeleteList_Noun);
		dictionary.UpdateDictionaryFromUserVerbs(userAddList_Verb, userDeleteList_Verb);
		dictionary.UpdateDictionaryFromUserAdjectives(userAddList_Adjective, userDeleteList_Adjective);
		dictionary.UpdateDictionaryFromUserAdverbs(userAddList_Adverb, userDeleteList_Adverb);

		float defaultDPI = 326;
		float dpi = Screen.dpi == 0 ? defaultDPI : Screen.dpi;
		dpi = 326;

		float width = Screen.width;
		float height = Screen.height;

		float hypotenuse = Mathf.Sqrt((width * width) +  (height * height));
		float diagonalInches = hypotenuse / dpi; //convert from pixels to inches

		float diagonalPOW = Mathf.Pow(diagonalInches, (1.0f / 4.0f));
		m_fScaleFactor = (diagonalPOW * dpi) * 0.006251847f;

		//GameObject.Find("DPIReadOut").GetComponent<Text>().text = "dpi = " + dpi +
		//    "\nScreen Size = " + diagonalInches + "\nScale Factor = " + m_fScaleFactor;

		GameObject.Find("Root Canvas").GetComponent<CanvasScaler>().scaleFactor = m_fScaleFactor;
		GameObject.Find("Top Bar Canvas").GetComponent<CanvasScaler>().scaleFactor = m_fScaleFactor;
	}

	// Called once per frame
	void Update() {

		//Show background
		if (showMain == true) {
			alpha += 5f * Time.deltaTime;
			alpha = Mathf.Clamp01(alpha);
			MainBackground.GetComponent<SpriteRenderer>().color = new Color(255, 255, 255, alpha);
			if (alpha >= 1) {
				showMain = false;
			}
		}
		//Hide background
		else if (hideMain == true) {
			alpha -= 5f * Time.deltaTime;
			alpha = Mathf.Clamp01(alpha);
			MainBackground.GetComponent<SpriteRenderer>().color = new Color(255, 255, 255, alpha);
			if (alpha <= 0) {
				hideMain = false;
			}
		}


		if (Input.acceleration.magnitude > 3f) {
			OnShake();
		}

		if (Screen.orientation != currentOrientation) {
			currentOrientation = Screen.orientation;
			OnOrientationChange();
		}
	}

	#region FileIO

	#region SettingsFile

	private void OpenSettingsFile() {

		try {
			if (File.Exists(SettingsFilePath)) {
				StreamReader reader = new StreamReader(SettingsFilePath);

				string line = "";
				int numberOfSettings = 0;
				while ((line = reader.ReadLine()) != null) {
					string[] setting = line.Split(","[0]);
					if (setting[0] == "GenerationType") {
						GenerationType oType = GenerationType.CompleteSentence;

						switch (setting[1]) {
						case "CompleteSentence":
							oType = GenerationType.CompleteSentence;
							break;
						case "FullRandom":
							oType = GenerationType.FullRandom;
							break;
						default:
							oType = GenerationType.CompleteSentence;
							Debug.Log("Could not find GenType from settings file");
							break;
						}

						sceneOptions.GenType = oType;
						numberOfSettings++;
					}
					else if (setting[0] == "AdCount") {
						int count = 0;
						int.TryParse(setting[1], out count);

						sceneOptions.AdCount = count;
						numberOfSettings++;
					}
				}

				reader.Close();

				if (numberOfSettings != 2) {
					Debug.Log("Incorrect settings found");
					File.Delete(SettingsFilePath);
					File.Create(SettingsFilePath);
					OverWriteSettings();
				}
			}
			else {
				Debug.Log("Settings file does not exist. Creating new one");
				File.Create(SettingsFilePath);
				OverWriteSettings();
			}
		}
		catch {
			Debug.Log("Could not read settings text file");
		}
			

		//=================================================================
		//Update Settings in UI to Settings from settings file

		string newLabel = "";

		if (sceneOptions.GenType == GenerationType.FullRandom) {
			newLabel = "Random";
			Setting1Dropdown.GetComponent<Dropdown>().value = 0;
		}
		else if (sceneOptions.GenType == GenerationType.CompleteSentence) {
			newLabel = "Sentence";
			Setting1Dropdown.GetComponent<Dropdown>().value = 1;
		}

		adCount = sceneOptions.AdCount;

		Setting1Dropdown.GetComponentInChildren<Text>().text = newLabel;
		//=================================================================

		Debug.Log("Setting Load Complete " + adCount);
	}

	private void OverWriteSettings() {
		
		//============================================================
		//Get Settings From Options Menu

		string settingOne = Setting1Dropdown.GetComponentInChildren<Text>().text;

		GenerationType oType = GenerationType.CompleteSentence;

		switch (settingOne) {
		case "Sentence":
			oType = GenerationType.CompleteSentence;
			break;
		case "Random":
			oType = GenerationType.FullRandom;
			break;
		default:
			oType = GenerationType.CompleteSentence;
			Debug.Log("Could not find GenType from options menu");
			break;
		}
		//============================================================

		//if (oType != sceneOptions.GenType) {
			sceneOptions.GenType = oType;

			try {

				if (File.Exists(SettingsFilePath)) {
					File.Delete(SettingsFilePath);
				}

				StreamWriter writer = File.CreateText(SettingsFilePath);

				string settingsString = "GenerationType," + sceneOptions.GenType;
				int count = sceneOptions.AdCount;

				writer.WriteLine(settingsString);
				writer.WriteLine("AdCount," + count);
				writer.Close();

				Debug.Log("Setting Overwrite Complete " + count);
			}
			catch {
				Debug.Log("Could not overwrite settings file");
			}
		//}
	}

	#endregion

	#region UserDictionaryFiles

	private void OpenUserDictionaryFiles() {

		#region OpenAddFile
		if (File.Exists(UserAddFilePath)) {
			StreamReader reader = new StreamReader(UserAddFilePath);

			string line = "";
			while ((line = reader.ReadLine()) != null) {

				string[] wordWithType = line.Split(","[0]);

				if (wordWithType != null && wordWithType.Length == 2) {

					Debug.Log("add " + line);

					string type = wordWithType[1];

					if (type == "noun") {
						userAddList_Noun.Add(wordWithType[0]);
					}
					else if (type == "verb") {
						userAddList_Verb.Add(wordWithType[0]);
					}
					else if (type == "adjective") {
						userAddList_Adjective.Add(wordWithType[0]);
					}
					else if (type == "adverb") {
						userAddList_Adverb.Add(wordWithType[0]);
					}
					else if (type == "other") {
						userAddList_Random.Add(wordWithType[0]);
					}
				}
				else {
					userAddList_Random.Add(line);
				}
			}

			reader.Close();
		}
		else {
			Debug.Log("No user dictionary for adding found");
			Debug.Log("Creating new user dictionary for adding words");
			StreamWriter writer = File.CreateText(UserAddFilePath);
			writer.Close();
		}
		#endregion

		#region OpenDeleteFile
		if (File.Exists(UserDeleteFilePath)) {
			StreamReader reader = new StreamReader(UserDeleteFilePath);
			bool overkillDeleteWord = false;

			string line = "";
			while ((line = reader.ReadLine()) != null) {
				string[] wordWithType = line.Split(","[0]);

				if (wordWithType != null && wordWithType.Length == 2) {

					Debug.Log("delete " + line);

					string type = wordWithType[1];

					//CHeck if the word from the delete file exists in the dictionary
					List<string> possibleWords = dictionary.getWordsContainingString(wordWithType[0]);
					if (!possibleWords.Contains(wordWithType[0])) {
						overkillDeleteWord = true;
						continue; //if it does not, do not add it to delete list
					}

					if (type == "noun") {
						userDeleteList_Noun.Add(wordWithType[0]);
					}
					else if (type == "verb") {
						userDeleteList_Verb.Add(wordWithType[0]);
					}
					else if (type == "adjective") {
						userDeleteList_Adjective.Add(wordWithType[0]);
					}
					else if (type == "adverb") {
						userDeleteList_Adverb.Add(wordWithType[0]);
					}
					else if (type == "other") {
						userDeleteList_Random.Add(wordWithType[0]);
					}
				}
				else {
					userDeleteList_Random.Add(line);
				}
			}

			reader.Close();

			if (overkillDeleteWord) {
				updateDeleteFile();
			}
		}
		else {
			Debug.Log("No user dictionary for removing found");
			Debug.Log("Creating new user dictionary for removing words");
			StreamWriter writer = File.CreateText(UserDeleteFilePath);
			writer.Close();
		}
		#endregion
	}

	private void updateAddFile() {
		if (File.Exists(UserAddFilePath)) {
			File.Delete(UserAddFilePath);
		}

		StreamWriter writer = File.CreateText(UserAddFilePath);

		for (int i = 0; i < userAddList_Noun.Count; i++) {
			writer.WriteLine(userAddList_Noun[i] + "," + "noun");
		}

		for (int i = 0; i < userAddList_Verb.Count; i++) {
			writer.WriteLine(userAddList_Verb[i] + "," + "verb");
		}

		for (int i = 0; i < userAddList_Adjective.Count; i++) {
			writer.WriteLine(userAddList_Adjective[i] + "," + "adjective");
		}

		for (int i = 0; i < userAddList_Adverb.Count; i++) {
			writer.WriteLine(userAddList_Adverb[i] + "," + "adverb");
		}

		for (int i = 0; i < userAddList_Random.Count; i++) {
			writer.WriteLine(userAddList_Random[i] + "," + "other");
		}

		writer.Close();
	}

	private void updateDeleteFile() {
		if (File.Exists(UserDeleteFilePath)) {
			File.Delete(UserDeleteFilePath);
		}

		StreamWriter writer = File.CreateText(UserDeleteFilePath);

		for (int i = 0; i < userDeleteList_Noun.Count; i++) {
			writer.WriteLine(userDeleteList_Noun[i] + "," + "noun");
		}

		for (int i = 0; i < userDeleteList_Verb.Count; i++) {
			writer.WriteLine(userDeleteList_Verb[i] + "," + "verb");
		}

		for (int i = 0; i < userDeleteList_Adjective.Count; i++) {
			writer.WriteLine(userDeleteList_Adjective[i] + "," + "adjective");
		}

		for (int i = 0; i < userDeleteList_Adverb.Count; i++) {
			writer.WriteLine(userDeleteList_Adverb[i] + "," + "adverb");
		}

		for (int i = 0; i < userDeleteList_Random.Count; i++) {
			writer.WriteLine(userDeleteList_Random[i] + "," + "other");
		}

		writer.Close();
	}

	public void RequestResetDictionary() {
		ResetConfirmation.SetActive(true);
	}

	public void ConfirmResetDictionary() {
		
		userAddList_Random.Clear();
		userDeleteList_Random.Clear();

		userAddList_Noun.Clear();
		userDeleteList_Noun.Clear();

		userAddList_Verb.Clear();
		userDeleteList_Verb.Clear();

		userAddList_Adjective.Clear();
		userDeleteList_Adjective.Clear();

		userAddList_Random.Clear();
		userDeleteList_Adverb.Clear();

		updateAddFile();
		updateDeleteFile();

		dictionary = new MyDictionary(TextFile, NounText, VerbsText, AdverbText, AdjText);
		ResetConfirmation.SetActive(false);
	}

	public void HideResetConfirmation() {
		ResetConfirmation.SetActive(false);
	}

	#endregion

	#endregion

	#region Navigation

	private GameObject getCurrentPanel() {

		foreach (GameObject panel in Panels) {
			if (panel.activeSelf) {
				return panel;
			}
		}

		return Panels[0];
	}

	public void goToMainMenu() {
		showMain = true;
		performSideNavAnimation(Panels[0], true);
	}

	public void goToGenerate() {

		AcronymInput.GetComponent<InputField>().text = "";

		//If currently on main menu
		if (getCurrentPanel() == Panels[0]) {
			hideMain = true;
			performSideNavAnimation(Panels[1], false);
		} 
		//If currently on solution panel
		else {
			performSideNavAnimation(Panels[1], true);
		}
	}

	public void goToSolution() {
		performSideNavAnimation(Panels[2], false);
	}

	public void openOptions() {
		navOptions(false);
	}

	public void closeOptions() {
		OverWriteSettings();
		navOptions(true);
	}

	public void goToAddWord() {
		performSideNavAnimation(Panels[4], false);
		StartCoroutine(disablePanel(Panels[3]));
	}

	public void goToDeleteWord() {
		performSideNavAnimation(Panels[5], false);
		StartCoroutine(disablePanel(Panels[3]));
	}

	public void goBackToOptions() {

		DeleteWordInput.GetComponent<InputField>().text = "";
		foreach (Transform child in DeleteSearchListContent.transform) {
			GameObject.Destroy(child.gameObject);
		}
		DeleteConfirmation.SetActive(false);

		performSideNavAnimation(Panels[3], true);
		StartCoroutine(disablePanel(Panels[4]));
		StartCoroutine(disablePanel(Panels[5]));
	}

	private void performSideNavAnimation(GameObject newPanel, bool moveBack) {

		GameObject currentPanel = getCurrentPanel();

		if (currentPanel == newPanel || currentPanel.GetComponent<MyPanel>().IsMoving) {
			return;
		}

		newPanel.SetActive(true);
		float xPosPositive = Screen.width * 1.5f;
		float xPosNegative = -Screen.width * 0.5f;
		float yPos = Screen.height / 2.0f;

		if (moveBack) {
			newPanel.GetComponent<MyPanel>().ForcePosition(new Vector3(xPosNegative, yPos, 0));

			StartCoroutine(delayPanelMove(newPanel));
			currentPanel.GetComponent<MyPanel>().Position = new Vector3(xPosPositive, yPos, 0);
		}
		else {
			newPanel.GetComponent<MyPanel>().ForcePosition(new Vector3(xPosPositive, yPos, 0));

			StartCoroutine(delayPanelMove(newPanel));
			currentPanel.GetComponent<MyPanel>().Position = new Vector3(xPosNegative, yPos, 0);
		}
	}

	private void navOptions(bool close) {
		
		float yPosNegative = -Screen.height * 0.5f;
		float xPos = Screen.width / 2.0f;

		if (close) {
			Panels[0].SetActive(true);
			Panels[3].GetComponent<MyPanel>().Position = new Vector3(xPos, yPosNegative, 0);
			StartCoroutine(disablePanel(Panels[3]));
			BlackImage.SetActive(false);
		}
		else {
			Panels[3].SetActive(true);
			Panels[3].GetComponent<MyPanel>().ForcePosition(new Vector3(xPos, yPosNegative, 0));
			StartCoroutine(delayPanelMove(Panels[3]));
			StartCoroutine(disablePanel(Panels[0]));
			StartCoroutine(enableBlackImage());
		}
	}

	private IEnumerator disablePanel(GameObject panel) {
		yield return new WaitForSeconds(0.3f);
		panel.gameObject.SetActive(false);
	}

	private IEnumerator delayPanelMove(GameObject panel) {
		yield return new WaitForSeconds(0.05f);
		panel.GetComponent<MyPanel>().Position = getScreenCenter();
	}

	private IEnumerator enableBlackImage() {
		yield return new WaitForSeconds(0.3f);
		BlackImage.SetActive(true);
	}

	#endregion

	#region Motions Methods

	private void OnShake() {
		RefreshSolution();
	}

	private void OnOrientationChange() {
		//TODO: Resize Solution Content
	}

	#endregion

	#region Text Methods

	#region Copy Text

	public void CopyText() {

		string textToCopy = SolutionText.GetComponent<Text>().text;
		GUIUtility.systemCopyBuffer = textToCopy;

		try {
			ExportString(textToCopy);
		}
		catch { 
			Debug.Log("Text was not copied");
		}

		StartCoroutine(revertSolution(SolutionText.GetComponent<Text>().text));
		SolutionText.GetComponent<Text>().text = "Text Copied!";
	}

	[DllImport("__Internal")]
	private static extern void _exportString(string exportData);

	//passes string to xcode and xcode copies to clipboard
	public static void ExportString(string exportData) {
		// Debug.Log ("export: "+exportData);
		if (Application.platform == RuntimePlatform.IPhonePlayer) {
			_exportString(exportData);
		}
	}

	#region Goes In XCode

	//This goes in the UnityAppController.mm file after building in Unity

	/*
	#define MakeStringCopy( _x_ ) ( _x_ != NULL  [_x_ isKindOfClass:[NSString class]] ) ? strdup( [_x_ UTF8String] ) : NULL
	#define GetStringParam( _x_ ) ( _x_ != NULL ) ? [NSString stringWithUTF8String:_x_] : [NSString stringWithUTF8String:""]

	//get clipboard from unity
	extern "C" void _exportString(const char * eString)
	{
		[UIPasteboard generalPasteboard].string = GetStringParam(eString);//@"the text to copy";
	}
	*/

	#endregion

	private IEnumerator revertSolution(string text) {
		yield return new WaitForSeconds(0.5f);
		SolutionText.GetComponent<Text>().text = text;
	}

	#endregion

	private void findContentHeight(string phrase) {
		Vector2 vec = SolutionText.GetComponent<RectTransform>().sizeDelta;

		//float screenHeight = Screen.currentResolution.height;
		//float screenWidth = Screen.currentResolution.width;
		float newHeight = 1.1f * phrase.Length;
		if (newHeight < 100) {
			newHeight = 100;
		}

		//At phrase length 100, content height must be ~480 (for iPhone 6 tall)

		SolutionContent.GetComponent<RectTransform>().sizeDelta = new Vector2(vec.x, newHeight);

		//Move scroll position to the top
		//Vector2 pos = SolutionContent.GetComponent<RectTransform> ().position;
		//SolutionContent.GetComponent<RectTransform>().position = new Vector2(pos.x, 0);
	}

	public void GoPressed() {
		if (Panels[1].GetComponent<MyPanel>().IsMoving) {
			return;
		}

		GeneratePhrase();

		goToSolution();
	}

	public void RefreshSolution() {
		GeneratePhrase();
	}

	private void GeneratePhrase() {
		string input = AcronymInput.GetComponent<InputField>().text;
		if (input.Length == 0) {
			return;
		}

		string phrase = "";
		if (sceneOptions.GenType == GenerationType.FullRandom) {
			phrase = generateRandomPhrase(input);
		}
		else if (sceneOptions.GenType == GenerationType.CompleteSentence) {
			phrase = generateRandomCompleteSentence(input);
		}
		else {
			phrase = generateRandomCompleteSentence(input);
		}

		SolutionText.GetComponent<Text>().text = phrase;

		adCount++;
		sceneOptions.AdCount += 1;
		if (adCount >= 30) {
			adCount = 0;
			sceneOptions.AdCount = 0;
			ShowAd();
		}
		OverWriteSettings();
		findContentHeight(phrase);
	}

	public void AddWordButtonPressed() {
		
		string input = AddWordInput.GetComponent<InputField>().text.ToLower();
		string type = AddWordDropdown.GetComponentInChildren<Text>().text.ToLower();
		bool AddFileUpdate = false;
		bool DeleteFileUpdate = false;

		if (type == "noun") {
			if (dictionary.AddWordToDictionary(input, type)) {
				userAddList_Noun.Add(input);
				AddFileUpdate = true;
				if (userDeleteList_Noun.Remove(input))
					DeleteFileUpdate = true;
			}
		}
		else if (type == "verb") {
			if (dictionary.AddWordToDictionary(input, type)) {
				userAddList_Verb.Add(input);
				AddFileUpdate = true;
				if (userDeleteList_Verb.Remove(input))
					DeleteFileUpdate = true;
			}
		}
		else if (type == "adjective") {
			if (dictionary.AddWordToDictionary(input, type)) {
				userAddList_Adjective.Add(input);
				AddFileUpdate = true;
				if (userDeleteList_Adjective.Remove(input))
					DeleteFileUpdate = true;
			}
		}
		else if (type == "adverb") {
			if (dictionary.AddWordToDictionary(input, type)) {
				userAddList_Adverb.Add(input);
				AddFileUpdate = true;
				if (userDeleteList_Adverb.Remove(input))
					DeleteFileUpdate = true;
			}
		}
		else if (type == "other") {
			if (dictionary.AddWordToDictionary(input, type)) {
				userAddList_Random.Add(input);
				AddFileUpdate = true;
				if (userDeleteList_Random.Remove(input))
					DeleteFileUpdate = true;
			}
		}

		if (AddFileUpdate)
			updateAddFile(); 
		
		if (DeleteFileUpdate) {
			updateDeleteFile();
		}

		AddWordInput.GetComponent<InputField>().text = "";
	}

	public void DeleteWordButtonPressed(string word) {

		DeleteConfirmation.GetComponentInChildren<Text>().text = 
			"Are you sure you want to delete " + "\"" + word + "\"" + "?";

		DeleteConfirmation.SetActive(true);
		wordToDelete = word;
	}

	public void ConfirmDelete() {

		string[] successfullRemoves = dictionary.RemoveWordFromDictionary(wordToDelete);
		bool AddFileUpdate = false;
		bool DeleteFileUpdate = false;

		if (successfullRemoves[0] == "yes") {
			userDeleteList_Noun.Add(wordToDelete);
			DeleteFileUpdate = true;
			if (userAddList_Noun.Remove(wordToDelete))
				AddFileUpdate = true;
		}
		if (successfullRemoves[1] == "yes") {
			userDeleteList_Verb.Add(wordToDelete);
			DeleteFileUpdate = true;
			if (userAddList_Verb.Remove(wordToDelete))
				AddFileUpdate = true;
		}
		if (successfullRemoves[2] == "yes") {
			userDeleteList_Adjective.Add(wordToDelete);
			DeleteFileUpdate = true;
			if (userAddList_Adjective.Remove(wordToDelete))
				AddFileUpdate = true;
		}
		if (successfullRemoves[3] == "yes") {
			userDeleteList_Adverb.Add(wordToDelete);
			DeleteFileUpdate = true;
			if (userAddList_Adverb.Remove(wordToDelete))
				AddFileUpdate = true;
		}
		if (successfullRemoves[4] == "yes") {
			userDeleteList_Random.Add(wordToDelete);
			DeleteFileUpdate = true;
			if (userAddList_Random.Remove(wordToDelete))
				AddFileUpdate = true;
		}

		if (AddFileUpdate)
			updateAddFile(); 

		if (DeleteFileUpdate) {
			updateDeleteFile();
		}

		if (DeleteFileUpdate) {
			updateDeleteSearchList();
		}

		wordToDelete = "";
		DeleteConfirmation.SetActive(false);
	}

	public void HideDeleteConfirmation() {
		DeleteConfirmation.SetActive(false);
	}

	public void updateDeleteSearchList() {
		
		if (!searchDone) {
			updateWaiting = true;
		}

		StartCoroutine(delayAddition());
	}

	private IEnumerator delayAddition() {

		string input = DeleteWordInput.GetComponent<InputField>().text.ToLower();

		foreach (Transform child in DeleteSearchListContent.transform) {
			GameObject.Destroy(child.gameObject);
		}

		if (input.Length > 0) {
			List<string> list = dictionary.getWordsContainingString(input);

			if (list != null && list.Count > 0) {
				float yOffset = 0;

				Vector2 pos = SolutionContent.GetComponent<RectTransform>().position;
				DeleteSearchListContent.GetComponent<RectTransform>().position = new Vector2(pos.x, 0);

				Vector2 vec = DeleteSearchListContent.GetComponent<RectTransform>().sizeDelta;
				//float ySize = (20.25f * list.Count);
				float ySize = (20.25f * list.Count) * (3.0f / m_fScaleFactor);
				DeleteSearchListContent.GetComponent<RectTransform>().sizeDelta = new Vector2(vec.x, ySize);

				int count = 0;
				for (int i = 0; i < list.Count; i++) {

					if (updateWaiting) {
						foreach (Transform child in DeleteSearchListContent.transform) {
							GameObject.Destroy(child.gameObject);
						}
						break;
					}

					if (count > 50) {
						yield return new WaitForSeconds(0.05f);
						count = 0;
					}

					string name = list[i];

					float yPosition = DeleteSearchListContent.position.y - 40;

					GameObject button = (GameObject)Instantiate(ButtonPrefab, new Vector3(0, 0, 0), ButtonPrefab.transform.rotation);
					button.transform.parent = DeleteSearchListContent;
					button.transform.localScale = new Vector3(1, 1, 1);
					button.GetComponent<RectTransform>().sizeDelta = new Vector2(vec.x, 20);
					button.GetComponent<RectTransform>().position = new Vector3(Screen.width / 2, yPosition + (-yOffset), 0);

					button.name = name;
					button.GetComponentInChildren<Text>().text = name;

					SetButtonOnClick(button, name);

					yOffset += 60.0f;

					count++;
				}
			}
		}
		else {
			Vector2 vec = DeleteSearchListContent.GetComponent<RectTransform>().sizeDelta;
			DeleteSearchListContent.GetComponent<RectTransform>().sizeDelta = new Vector2(vec.x, 20.25f * 5);
		}

		searchDone = true;
		updateWaiting = false;
	}

	void SetButtonOnClick(GameObject btn, string value) {
		btn.GetComponent<Button>().onClick.AddListener(() => DeleteWordButtonPressed(value));
	}

	#region Generation

	private string generateRandomPhrase(string acronym) {

		string output = "";

		char[] letters = acronym.ToCharArray();

		for (int i = 0; i < letters.Length; i++) {

			if (letters[i].ToString() == " ") {
				continue;
			}

			string randomWord = dictionary.getRandomWord(letters[i].ToString());

			output += randomWord + " ";
		}

		return output;
	}

	private string generateRandomCompleteSentence(string acronym) {
		
		return dictionary.CreateRandomSentence(acronym);
	}

	#endregion

	#endregion

	#region Unity Ads

	private void ShowAd() {
		if (Advertisement.IsReady()) {
			Advertisement.Show();
		}
	}

	#endregion

	private Vector3 getScreenCenter() {
		return new Vector3(Screen.width / 2.0f, Screen.height / 2.0f, 0);
	}

	#endregion
}
