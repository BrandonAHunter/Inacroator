using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ListOfWords {

	private string letter;
	private List<string> words;

	public ListOfWords() {
		words = new List<string>();
	}

	public ListOfWords(List<string> list) {
		words = list;
	}

	public string getLetter() {
		return letter;
	}

	public string getWordAtIndex(int i) {
		if (words.Count > 0) {
			return words[i];
		}
		return "";
	}

	public int getNumberOfWords() {
		if (words.Count > 0) {
			letter = words[0].Substring(0);
			return words.Count;
		}

		return 0;
	}

	public bool addWord(string word) {
		if (!words.Contains(word)) {
			words.Add(word);
			return true;
		}
		return false;
	}

	public bool removeWord(string word) {
		if (words.Contains(word)) {
			words.Remove(word);
			return true;
		}
		return false;
	}

	public void replaceWords(int index, string target, string word) {
		words[index] = words[index].Replace(target, word);
	}
}
