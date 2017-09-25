using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Sentence {
	int iter;
	int TypeOFWord;
	int placeInSentence;
	string[] sentence;
	string moveSentence1;
	string moveSentence2;

	public Sentence()
	{
		iter = 0;
		TypeOFWord = 0;
		placeInSentence = 0;
		sentence = new string[7];
	}

	public List<string> getSentenceStructure(int sentenceSize)
	{
		iter = 2;
		TypeOFWord = 0;
		placeInSentence = -1;
		sentence[0] = "NOUN";
		sentence[1] = "VERB";
		int adjetiveCount = 0;
		int adverbCount = 0;
		int nounCount = 0;
		int verbCount = 0;

		for(int i = 2; i < sentence.Length; i++)
		{
			sentence[i] = "NOTHING";
		}

		while(iter < sentenceSize - 1)
		{
			TypeOFWord = Random.Range (0, 3);
			if(TypeOFWord == 0  && adjetiveCount < 2)//adjetive
			{
				moveSentence1 = sentence[0];
				sentence[0] = "ADJECTIVE";
				moveSentence2 = sentence[1];
				sentence[1] = moveSentence1;
				moveSentence1 = sentence[2];
				sentence[2] = moveSentence2;
				moveSentence2 = sentence[3];
				sentence[3] = moveSentence1;
				moveSentence1 = sentence[4];
				sentence[4] = moveSentence2;
				moveSentence2 = sentence[5];
				sentence[5] = moveSentence1;
				moveSentence1 = sentence[6];
				sentence[6] = moveSentence2;
				adjetiveCount++;
			}
			else if(TypeOFWord == 1 && nounCount < 2)//noun
			{
				for(int i = 0; i < sentence.Length; i++)
				{
					if(sentence[i] == "ADVERB" || sentence[i] == "VERB")
					{
						placeInSentence = i;
						break;
					}
				}
				moveSentence1 = sentence[placeInSentence];
				sentence[placeInSentence] = "NOUN";
				if(placeInSentence + 1 < 7)
				{
					moveSentence2 = sentence[placeInSentence + 1];
					sentence[placeInSentence + 1] = moveSentence1;
				}
				if(placeInSentence + 2 < 7)
				{
					moveSentence1 = sentence[placeInSentence + 2];
					sentence[placeInSentence + 2] = moveSentence2;
				}
				if(placeInSentence + 3 < 7)
				{
					moveSentence2 = sentence[placeInSentence + 3];
					sentence[placeInSentence + 3] = moveSentence1;
				}
				if(placeInSentence + 4 < 7)
				{
					moveSentence1 = sentence[placeInSentence + 4];
					sentence[placeInSentence + 4] = moveSentence2;
				}
				if(placeInSentence + 5 < 7)
				{
					moveSentence2 = sentence[placeInSentence + 5];
					sentence[placeInSentence + 5] = moveSentence1;
				}
				placeInSentence = -1;
				nounCount++;
			}
			else if(TypeOFWord == 2 && adverbCount < 2)//adverb
			{
				for(int i = 0; i < sentence.Length;i++)
				{
					if(sentence[i] == "VERB")
					{
						placeInSentence = i;
						break;
					}
				}
				moveSentence1 = sentence[placeInSentence];
				sentence[placeInSentence] = "ADVERB";
				if(placeInSentence + 1 < 7)
				{
					moveSentence2 = sentence[placeInSentence + 1];
					sentence[placeInSentence + 1] = moveSentence1;
				}
				if(placeInSentence + 2 < 7)
				{
					moveSentence1 = sentence[placeInSentence + 2];
					sentence[placeInSentence + 2] = moveSentence2;
				}
				if(placeInSentence + 3 < 7)
				{
					moveSentence2 = sentence[placeInSentence + 3];
					sentence[placeInSentence + 3] = moveSentence1;
				}
				if(placeInSentence + 4 < 7)
				{
					moveSentence1 = sentence[placeInSentence + 4];
					sentence[placeInSentence + 4] = moveSentence2;
				}
				placeInSentence = -1;
				adverbCount++;
			}
			else if(TypeOFWord == 3 && verbCount < 2)//verb
			{
				for(int i = 0; i < sentence.Length; i++)
				{
					if(sentence[i] == "NOTHING")
					{
						sentence[i] = "VERB";
						break;
					}
				}
				verbCount++;
			}
			else
				iter--;

			iter++;
		}

		if(sentenceSize > 2)
			sentence[sentenceSize - 1] = "ENDNoun";

		List<string> returnList = new List<string>();

		for(int i = 0; i < sentenceSize; i++)
		{
			returnList.Add(sentence[i]);
		}

		return returnList;
	}

	public List<string> addConjunctions(List<List<string>> aSentence)
	{
		string answer = "";
		List<string> fixedSentence = new List<string>();
		for(int i = 0; i < aSentence.Count; i++)
		{ 
			for(int j = 0; j < aSentence[i].Count; j++)
			{
				if (j != aSentence [i].Count - 1) {
					answer += (aSentence [i] [j] + " ");
				} 
				else {
					answer += (aSentence [i] [j]);
				}
			}	

			answer = answer.Replace("ADJECTIVE ADJECTIVE ADJECTIVE ADJECTIVE ADJECTIVE", "ADJECTIVE, ADJECTIVE, ADJECTIVE, ADJECTIVE, and ADJECTIVE");
			answer = answer.Replace("NOUN NOUN NOUN NOUN NOUN", "NOUN, NOUN, NOUN, NOUN, and NOUN");
			answer = answer.Replace("ADVERB ADVERB ADVERB ADVERB ADVERB", "ADVERB, ADVERB, ADVERB, ADVERB, and ADVERB");
			answer = answer.Replace(" VERB VERB VERB VERB VERB", " VERB, VERB, VERB, VERB, and VERB");

			answer = answer.Replace("ADJECTIVE ADJECTIVE ADJECTIVE ADJECTIVE", "ADJECTIVE, ADJECTIVE, ADJECTIVE, and ADJECTIVE");
			answer = answer.Replace("NOUN NOUN NOUN NOUN", "NOUN, NOUN, NOUN, and NOUN");
			answer = answer.Replace("ADVERB ADVERB ADVERB ADVERB", "ADVERB, ADVERB, ADVERB, and ADVERB");
			answer = answer.Replace(" VERB VERB VERB VERB", " VERB, VERB, VERB, and VERB");

			answer = answer.Replace("ADJECTIVE ADJECTIVE ADJECTIVE", "ADJECTIVE, ADJECTIVE, and ADJECTIVE");
			answer = answer.Replace("NOUN NOUN NOUN", "NOUN, NOUN, and NOUN");
			answer = answer.Replace("ADVERB ADVERB ADVERB", "ADVERB, ADVERB, and ADVERB");
			answer = answer.Replace(" VERB VERB VERB", " VERB, VERB, and VERB");

			answer = answer.Replace("ADJECTIVE ADJECTIVE", "ADJECTIVE and ADJECTIVE");
			answer = answer.Replace("NOUN NOUN", "NOUN and NOUN");
			answer = answer.Replace("ADVERB ADVERB", "ADVERB and ADVERB");
			answer = answer.Replace(" VERB VERB", " VERB and VERB");

			fixedSentence.Add(answer);
			answer = "";
		}

		return fixedSentence;
	}
}
