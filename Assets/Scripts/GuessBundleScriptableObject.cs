using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
[CreateAssetMenu(fileName = "GuessBundle", menuName = "Guess Bundle", order = 1)]
public class GuessBundleScriptableObject : ScriptableObject
{
    public List<string> cellNames;
    public StringSpriteDict cellSprites;

    // Returns N random values from Cell Names
    public List<string> getSample(int count)
    {
        System.Random rnd = new System.Random();
        List<string> res = new List<string>(cellNames.OrderBy(x => rnd.Next()).Take(count)); 
        return res;
    }
}
