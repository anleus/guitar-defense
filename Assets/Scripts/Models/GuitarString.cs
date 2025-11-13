using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Models
{
    [CreateAssetMenu(fileName = "Guitar string", menuName = "ScriptableObjects/Guitar/String")]
    public class GuitarString : ScriptableObject
    {
        public int stringNum;
        public Color color;
        public List<GuitarNote> notes = new();
    }
}