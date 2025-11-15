using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Models
{
    [CreateAssetMenu(fileName = "Guitar pattern", menuName = "ScriptableObjects/Guitar/Pattern")]
    public class GuitarPattern : ScriptableObject
    {
        public string patternName;
        public Sprite patternImage;

        [Header("Fret Range (inclusive)")] 
        public int minFret;
        public int maxFret;

        public List<GuitarString> guitarStrings;

        public int GetIndexFromFret(int fret)
        {
            return fret - minFret;
        }

        public GuitarNote GetNoteAt(int stringNum, int fret)
        {
            var guitarString = guitarStrings.Find(s => s.stringNum == stringNum);
            if (guitarString == null) return null;

            return guitarString.notes.Find(n => n.fret == fret);
        }

        public (GuitarNote, int) GetNote()
        {
            var randomString = Random.Range(0, guitarStrings.Count);
            var notes =  guitarStrings[randomString].notes;

            return GetRandomNoteFromString(randomString, notes);
        }

        public (GuitarNote, int) GetNonRepeatedNote(int stringNum, int fret)
        {
            var randomString = Random.Range(0, guitarStrings.Count);
            var validNotes = randomString == stringNum
                ? guitarStrings[randomString].notes.Where(n => n.fret != fret).ToList()
                : guitarStrings[randomString].notes;

            return GetRandomNoteFromString(randomString, validNotes);
        }

        private (GuitarNote, int) GetRandomNoteFromString(int stringNum, List<GuitarNote> validNotes)
        {
            var randomNote = Random.Range(0, validNotes.Count);
            return (validNotes[randomNote], stringNum);
        }

        public Color GetStringColor(int stringNum)
        {
            return guitarStrings[stringNum].color;
        }

        public int PatternSize => (maxFret - minFret) + 1;
    }
}