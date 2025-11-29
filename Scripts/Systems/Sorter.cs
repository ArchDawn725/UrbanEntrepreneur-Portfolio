using System.Collections.Generic;
using UnityEngine;
public class Sorter : MonoBehaviour
{
    public List<string> catagories = new List<string>();
    [HideInInspector] public int currentNumber;
    public void Sort(string sortMethod)
    {
        List<Transform> children = new List<Transform>();
        List<float> childrenNumbers = new List<float> ();

        char firstLetter = sortMethod[0];
        string afterLetter = "0";
        if (sortMethod.Length > 1) { afterLetter = sortMethod.Substring(1); }

        foreach (Transform child in transform) { children.Add(child); }

        switch (sortMethod)
        {
            //sorts the children by their name
            case "A":
                children.Sort((x, y) => string.Compare(x.name, y.name)); 
                for (int i = 0; i < children.Count; i++) { children[i].SetSiblingIndex(i); } 
                break;
            default: 
                switch(firstLetter)
                {
                    //sorts the children by the name of a child
                    case 'A':
                        children.Sort((x, y) => string.Compare(x.GetChild(int.Parse(afterLetter)).name, y.GetChild(int.Parse(afterLetter)).name));
                        for (int i = 0; i < children.Count; i++) { children[i].SetSiblingIndex(i); }
                        break;
                    //sorts the children by the number of a name of a child
                    case '#':
                        for (int x = 0; x < children.Count; x++)
                        { 
                            if (children[x].childCount > 0) 
                            {
                                childrenNumbers.Add(int.Parse(children[x].GetChild(int.Parse(afterLetter)).name)); 
                            } 
                        }
                        childrenNumbers.Sort();
                        for (int i = 0; i < children.Count; i++)
                        { children[i].SetSiblingIndex(childrenNumbers.IndexOf(int.Parse(children[i].GetChild(int.Parse(afterLetter)).name))); }
                        break;
                }
                break;
        }
    }
    public void catagoryButton(string value)
    {
        if (catagories.Contains(value))
        {
            catagories.Remove(value);

            foreach (Transform child in transform)
            {
                if (child.name == value) { child.gameObject.SetActive(false); }
            }
        }
        else
        {
            catagories.Add(value);

            foreach (Transform child in transform)
            {
                if (child.name == value) { child.gameObject.SetActive(true); }
            }
        }
    }
}
