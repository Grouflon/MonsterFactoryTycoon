using UnityEngine;
using System.Collections;

[System.Serializable]
public class Syllable 
{
    // A syllable is defined by several letters
    public string[] letters; 
}


[System.Serializable]
public class Word 
{
    // A word is defined by several syllables
    public Syllable[] syllables;
}


[System.Serializable]
public class NameGenerator
{
    // A name is defined by several words
    public Word[] words;

    // Generates a name
    public string Generate()
    {
        string name = "";

        for (int i = 0; i < words.Length; i++)
        {
            Word word = words[i];

            foreach (Syllable syllable in word.syllables)
            {
                name += syllable.letters[Random.Range(0, syllable.letters.Length)];
            }

            // Add a space between words if it's not the last word
            if (i != words.Length)
            {
                name += " ";
            }
        }
        return name;
    }
}
