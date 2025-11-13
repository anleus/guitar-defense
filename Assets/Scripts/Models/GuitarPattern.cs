using System.Collections.Generic;
using UnityEngine;

namespace Models
{
    [CreateAssetMenu(fileName = "Guitar pattern", menuName = "ScriptableObjects/Guitar/Pattern")]
    public class GuitarPattern : ScriptableObject
    {
        public string patternName;
        
        [Header("Fret Range (inclusive)")]
        public int minFret;
        public int maxFret;
        
        public List<GuitarString> guitarStrings;

        public int GetIndexFromFret(int fret)
        {
            return fret - minFret;
        }

        public GuitarNote GetNoteAt(int stringNumb, int fret)
        {
            var guitarString = guitarStrings.Find(s => s.stringNum == stringNumb);
            if (guitarString == null) return null;
            
            return guitarString.notes.Find(n => n.fret == fret);
        }
        
        public int PatternSize => (maxFret - minFret) + 1;
    }
}