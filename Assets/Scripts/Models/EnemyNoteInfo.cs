using UnityEngine;

namespace Models
{
    public class EnemyNoteInfo
    {
        public string Note;
        public int Fret;
        public int FretRelativeIndex;
        public Color Color;

        public EnemyNoteInfo(string note, int fret, int fretRelativeIndex, Color color)
        {
            Note = note;
            Fret = fret;
            FretRelativeIndex = fretRelativeIndex;
            Color = color;
        }
    }
}