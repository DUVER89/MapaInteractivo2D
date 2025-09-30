using System;
using System.Collections.Generic;
using UnityEngine;

public class WorldButton : MonoBehaviour
{
    [Serializable]
    public class SpriteTotem
    {
        public GameObject normal;
        public GameObject selected;
    }

    public List<SpriteTotem> Sprites = new List<SpriteTotem>();

    public void SetSelectedColor(int index)
    {
        if (Sprites == null || Sprites.Count == 0) return;

        for (int i = 0; i < Sprites.Count; i++)
        {
            if (Sprites[i] == null) continue;

            bool state = (i == index) ? true : false;

            Sprites[i].selected.SetActive(state);
            Sprites[i].normal.SetActive(!state);
        }
    }
}