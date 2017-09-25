using UnityEngine;
using System.Collections;
using System.Text;
using System.IO;
using System.Collections.Generic;

public class MyDictionary {
	
	private ListOfWords[] randomWords;

	private ListOfWords[] Nouns; 
	private ListOfWords[] Verbs; 
	private ListOfWords[] Adjectives; 
	private ListOfWords[] Adverbs;

	private string alphabet = "abcdefghijklmnopqrstuvwxyz";

	public MyDictionary(TextAsset textAsset, TextAsset nounFile, TextAsset verbFile, TextAsset adverbFile, TextAsset adjFile) {
		randomWords = new ListOfWords[26];
		Nouns = new ListOfWords[26];
		Verbs = new ListOfWords[26];
		Adjectives = new ListOfWords[26];
		Adverbs = new ListOfWords[26];

		for (int i = 0; i < 26; i++) {
			randomWords[i] = new ListOfWords();
			Nouns[i] = new ListOfWords();
			Verbs[i] = new ListOfWords();
			Adjectives[i] = new ListOfWords();
			Adverbs[i] = new ListOfWords();
		}

		ReadRandomFile(textAsset);

		Debug.Log("Generating words: Nouns");
		Nouns = ReadWordsFromFile (nounFile);

		Debug.Log("Generating words: Verbs");
		Verbs = ReadWordsFromFile (verbFile);

		Debug.Log("Generating words: Adverbs");
		Adverbs = ReadWordsFromFile (adverbFile);

		Debug.Log("Generating words: Adjectives");
		Adjectives = ReadWordsFromFile (adjFile);
	}

	private void ReadRandomFile(TextAsset textAsset) {

		Debug.Log("Generating random words");

		try {
			string[] textContent = textAsset.text.Split("\n"[0]);

			for (int i = 0; i < textContent.Length; i++) {
				string word = textContent[i];
				string firstLetter = word.ToCharArray()[0].ToString();
				int index = DeterminePlaceInAlphabet(firstLetter);
				if (index < 26 && index > -1) {
					try
					{
						randomWords[index].addWord(word);
					}
					catch {
						Debug.Log("Could not add " + word);
					}
				}
			}
		}
		catch {
			Debug.Log("Could not read text file");
		}

		int numberOfWords = 0;
		for (int i = 0; i < randomWords.Length; i++) {
			numberOfWords += randomWords[i].getNumberOfWords();
		}

		Debug.Log (numberOfWords + " words found");
	}

	public ListOfWords[] ReadWordsFromFile(TextAsset textAsset)
	{
		try
		{
			string[] fileLine = textAsset.text.Split("\n"[0]);

			ListOfWords[] words = new ListOfWords[26];

			for(int i = 0; i < words.Length; i++)
			{
				words[i] = new ListOfWords();
			}

			for(int i = 0; i < fileLine.Length; i++) {
				string word = fileLine[i];
				string firstLetter = word.ToCharArray()[0].ToString();
				int check = DeterminePlaceInAlphabet(firstLetter);

				if (check == -1)
					continue;

				words[check].addWord(word);
			}

			int numberOfWords = 0;
			for (int i = 0; i < words.Length; i++) {
				numberOfWords += words[i].getNumberOfWords();
			}

			Debug.Log (numberOfWords + " words found");

			return words;

		}
		catch
		{
			Debug.Log("Could not read text file: " + textAsset);
			return new ListOfWords[26];
		}
	}

	#region UpdateDictionaryAtStart

	public void UpdateDictionaryFromUserRandoms(List<string> addList, List<string> deleteList) {

		if (addList.Count > 0) {
			//Add words
			for (int i = 0; i < addList.Count; i++) {
				int index = DeterminePlaceInAlphabet (addList[i].ToCharArray()[0].ToString());
				if (index < 0 || index > 25) 
					continue;

				randomWords [index].addWord(addList[i]);
			}
		}

		if (deleteList.Count > 0) {
			//Delete words
			for (int i = 0; i < deleteList.Count; i++) {
				int index = DeterminePlaceInAlphabet (deleteList[i].ToCharArray()[0].ToString());
				if (index < 0 || index > 25) 
					continue;

				randomWords [index].removeWord(deleteList[i]);
			}
		}
	}

	public void UpdateDictionaryFromUserNouns(List<string> addList, List<string> deleteList) {

		if (addList.Count > 0) {
			//Add words
			for (int i = 0; i < addList.Count; i++) {
				int index = DeterminePlaceInAlphabet (addList[i].ToCharArray()[0].ToString());
				if (index < 0 || index > 25) 
					continue;

				Nouns [index].addWord(addList[i]);
			}
		}

		if (deleteList.Count > 0) {
			//Delete words
			for (int i = 0; i < deleteList.Count; i++) {
				int index = DeterminePlaceInAlphabet (deleteList[i].ToCharArray()[0].ToString());
				if (index < 0 || index > 25) 
					continue;

				Nouns [index].removeWord(deleteList[i]);
			}
		}
	}

	public void UpdateDictionaryFromUserVerbs(List<string> addList, List<string> deleteList) {

		if (addList.Count > 0) {
			//Add words
			for (int i = 0; i < addList.Count; i++) {
				int index = DeterminePlaceInAlphabet (addList[i].ToCharArray()[0].ToString());
				if (index < 0 || index > 25) 
					continue;

				Verbs [index].addWord(addList[i]);
			}
		}

		if (deleteList.Count > 0) {
			//Delete words
			for (int i = 0; i < deleteList.Count; i++) {
				int index = DeterminePlaceInAlphabet (deleteList[i].ToCharArray()[0].ToString());
				if (index < 0 || index > 25) 
					continue;

				Verbs [index].removeWord(deleteList[i]);
			}
		}
	}

	public void UpdateDictionaryFromUserAdjectives(List<string> addList, List<string> deleteList) {

		if (addList.Count > 0) {
			//Add words
			for (int i = 0; i < addList.Count; i++) {
				int index = DeterminePlaceInAlphabet (addList[i].ToCharArray()[0].ToString());
				if (index < 0 || index > 25) 
					continue;

				Adjectives [index].addWord(addList[i]);
			}
		}

		if (deleteList.Count > 0) {
			//Delete words
			for (int i = 0; i < deleteList.Count; i++) {
				int index = DeterminePlaceInAlphabet (deleteList[i].ToCharArray()[0].ToString());
				if (index < 0 || index > 25) 
					continue;

				Adjectives [index].removeWord(deleteList[i]);
			}
		}
	}

	public void UpdateDictionaryFromUserAdverbs(List<string> addList, List<string> deleteList) {

		if (addList.Count > 0) {
			//Add words
			for (int i = 0; i < addList.Count; i++) {
				int index = DeterminePlaceInAlphabet (addList[i].ToCharArray()[0].ToString());
				if (index < 0 || index > 25) 
					continue;

				Adverbs [index].addWord(addList[i]);
			}
		}

		if (deleteList.Count > 0) {
			//Delete words
			for (int i = 0; i < deleteList.Count; i++) {
				int index = DeterminePlaceInAlphabet (deleteList[i].ToCharArray()[0].ToString());
				if (index < 0 || index > 25) 
					continue;

				Adverbs [index].removeWord(deleteList[i]);
			}
		}
	}

	#endregion

	public List<string> getWordsContainingString(string phrase) {

		int index = DeterminePlaceInAlphabet (phrase.ToCharArray()[0].ToString());
		if (index < 0 || index > 25) 
			return null;

		List<string> list = new List<string>();

		//Start with Random Words
		for (int i = 0; i < randomWords[index].getNumberOfWords(); i++) {
			string word = randomWords[index].getWordAtIndex(i);

			if (word.Contains(phrase)) {
				list.Add(word);
			}
		}

		//Search through the other lists

		for (int i = 0; i < Nouns[index].getNumberOfWords(); i++) {
			string word = Nouns[index].getWordAtIndex(i);

			if (word.Contains(phrase) && !list.Contains(word)) {
				list.Add(word);
			}
		}

		for (int i = 0; i < Verbs[index].getNumberOfWords(); i++) {
			string word = Verbs[index].getWordAtIndex(i);

			if (word.Contains(phrase) && !list.Contains(word)) {
				list.Add(word);
			}
		}

		for (int i = 0; i < Adjectives[index].getNumberOfWords(); i++) {
			string word = Adjectives[index].getWordAtIndex(i);

			if (word.Contains(phrase) && !list.Contains(word)) {
				list.Add(word);
			}
		}

		for (int i = 0; i < Adverbs[index].getNumberOfWords(); i++) {
			string word = Adverbs[index].getWordAtIndex(i);

			if (word.Contains(phrase) && !list.Contains(word)) {
				list.Add(word);
			}
		}

		list.Sort();
		return list;
	}

	#region Get Random Word

	public string getRandomWord(string letter) {

		int index = DeterminePlaceInAlphabet(letter); 
		if (index < 0 || index > 25) 
			return "";

		string returnValue = "";
		int type = Random.Range(0, 4);

		if (type == 0) { //Noun
			returnValue = getRandomNoun(letter);
		}
		else if (type == 1) { //Verb
			returnValue = getRandomVerb(letter);
		}
		else if (type == 2) { //Adjective
			returnValue = getRandomAdjective(letter);
		}
		else if (type == 3) { //Adverb
			returnValue = getRandomAdverb(letter);
		}
		else if (type == 4) { //Random
			int randomNumber = Random.Range(0, randomWords[index].getNumberOfWords());

			returnValue = randomWords[index].getWordAtIndex(randomNumber);
		}

		if (returnValue == "") {
			int randomNumber = Random.Range(0, randomWords[index].getNumberOfWords());

			returnValue = randomWords[index].getWordAtIndex(randomNumber);
		}

		return returnValue;
	}

	public string getRandomNoun(string letter) {

		int index = DeterminePlaceInAlphabet (letter);
		if (index < 0 || index > 25)
			return "";

		int randomNumber = Random.Range (0, Nouns[index].getNumberOfWords());

		return Nouns[index].getWordAtIndex(randomNumber);
	}

	public string getRandomVerb(string letter) {
		int index = DeterminePlaceInAlphabet (letter);
		if (index < 0 || index > 25)
			return "";

		int randomNumber = Random.Range (0, Verbs[index].getNumberOfWords());

		return Verbs[index].getWordAtIndex(randomNumber);
	}

	public string getRandomAdjective(string letter) {
		int index = DeterminePlaceInAlphabet (letter);
		if (index < 0 || index > 25)
			return "";

		int randomNumber = Random.Range (0, Adjectives[index].getNumberOfWords());

		return Adjectives[index].getWordAtIndex(randomNumber);
	}
		
	public string getRandomAdverb(string letter) {
		int index = DeterminePlaceInAlphabet (letter);
		if (index < 0 || index > 25)
			return "";

		int randomNumber = Random.Range (0, Adverbs[index].getNumberOfWords());

		return Adverbs[index].getWordAtIndex(randomNumber);
	}

	#endregion

	#region AddWord

	public bool AddWordToDictionary(string word, string type) {
		bool wordAdded = false;

		if (type == "noun") {
			wordAdded = AddNoun(word);
		}
		else if (type == "verb") {
			wordAdded = AddVerb(word);
		}
		else if (type == "adjective") {
			wordAdded = AddAdjective(word);
		}
		else if (type == "adverb") {
			wordAdded = AddAdverb(word);
		}
		else if (type == "other") {
			wordAdded = AddOther(word);
		}

		return wordAdded;
	}

	private bool AddNoun(string word) {
		int index = DeterminePlaceInAlphabet (word.ToCharArray()[0].ToString());
		if (index < 0 || index > 25) 
			return false;

		if (Nouns[index].addWord(word))
			return true;

		return false;
	}

	private bool AddVerb(string word) {
		int index = DeterminePlaceInAlphabet (word.ToCharArray()[0].ToString());
		if (index < 0 || index > 25) 
			return false;

		if (Verbs[index].addWord(word))
			return true;

		return false;
	}

	private bool AddAdjective(string word) {
		int index = DeterminePlaceInAlphabet (word.ToCharArray()[0].ToString());
		if (index < 0 || index > 25) 
			return false;

		if (Adjectives[index].addWord(word))
			return true;

		return false;
	}

	private bool AddAdverb(string word) {
		int index = DeterminePlaceInAlphabet (word.ToCharArray()[0].ToString());
		if (index < 0 || index > 25) 
			return false;

		if (Adverbs[index].addWord(word))
			return true;

		return false;
	}

	private bool AddOther(string word) {
		int index = DeterminePlaceInAlphabet (word.ToCharArray()[0].ToString());
		if (index < 0 || index > 25) 
			return false;

		if (randomWords[index].addWord(word))
			return true;

		return false;
	}

	#endregion

	#region RemoveWord

	public string[] RemoveWordFromDictionary(string word) {

		//Noun, Verb, Adjective, Adverb, Other
		string[] successFiles = { "no", "no", "no", "no", "no" };

		int index = DeterminePlaceInAlphabet (word.ToCharArray()[0].ToString());
		if (index < 0 || index > 25) 
			return successFiles;

		if (Nouns [index].removeWord (word)) {
			successFiles [0] = "yes";
		}
		if (Verbs [index].removeWord (word)) {
			successFiles [1] = "yes";
		}
		if (Adjectives [index].removeWord (word)) {
			successFiles [2] = "yes";
		}
		if (Adverbs [index].removeWord (word)) {
			successFiles [3] = "yes";
		}
		if (randomWords [index].removeWord (word)) {
			successFiles [4] = "yes";
		}

		return successFiles;
	}

	#endregion

	#region SentenceStructure

	public string CreateRandomSentence(string input) {
		Sentence setUp = new Sentence ();

		//holds the list of all the sentences to be used
		List<List<string>> sentence = new List<List<string>>();

		string temp = "";

		for (int i = 0; i < input.Length; i++) {
			if (DeterminePlaceInAlphabet (input.ToCharArray()[i] + "") != -1) {
				temp += input.ToCharArray() [i];
			}
		}

		input = temp;

		//gets the amount and size of sentences needed
		int size = input.Length;
		while(size != 0)
		{
			if(size <= 7)
			{
				sentence.Add(setUp.getSentenceStructure(size));
				size = 0;
			}
			else
			{

				int length = Random.Range (3, 7);
				sentence.Add(setUp.getSentenceStructure(length));
				size -= length;
			}
		}

		//adds the conjunctions needed in the sentence
		List<string> conjunctions = setUp.addConjunctions(sentence);

		string acronym = input;

		//gets the final result
		List<string> finalResult = SubstituteStructureWithWords(conjunctions, acronym);

		string returnValue = "";
		for (int i = 0; i < finalResult.Count; i++) {
			returnValue += finalResult[i] + ". ";
		}

		return returnValue;
	}

	public List<string> SubstituteStructureWithWords(List<string> aStructure, string acronym)
	{
		int iter = 0;
		for (int i = 0; i < aStructure.Count; i++)
		{
			string sentence = aStructure[i];
			bool sentenceIsFilled = false;

			while(!sentenceIsFilled)
			{
				string character = acronym.ToCharArray () [iter].ToString ();

				if(sentence.Contains("ADJECTIVE"))
				{
					ReplaceFirst(ref sentence, "ADJECTIVE", getRandomAdjective(character));
				}
				else if(sentence.Contains("NOUN"))
				{
					ReplaceFirst(ref sentence, "NOUN", getRandomNoun(character));
				}
				else if(sentence.Contains("ADVERB"))
				{
					ReplaceFirst(ref sentence, "ADVERB", getRandomAdverb(character));
				}
				else if(sentence.Contains("VERB"))
				{
					ReplaceFirst(ref sentence, "VERB", getRandomVerb(character));
				}
				else if(sentence.Contains("ENDNoun"))
				{
					ReplaceFirst(ref sentence, "ENDNoun", getRandomNoun(character));
				}
				else
				{
					sentenceIsFilled = true;
					iter--;
				}

				iter++;
				if (iter >= acronym.Length) {
					sentenceIsFilled = true;
				}
			}

			sentence = char.ToUpper(sentence[0]) + sentence.Substring(1);
			aStructure[i] = sentence;
		}

		return aStructure;
	}

	public void ReplaceFirst(ref string text, string search, string replace)
	{
		int pos = text.IndexOf(search);
		if (pos >= 0) {
			text = text.Substring (0, pos) + replace + text.Substring (pos + search.Length);
		}
	}

	#endregion

	private int DeterminePlaceInAlphabet(string letter) {
		return alphabet.IndexOf(letter.ToLower());
	}
}
