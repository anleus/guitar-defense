using UnityEngine;

namespace Models
{
    public class EnemyNoteInfo
    {
        public string Note;
        public int StringNum;
        public int Fret;
        public int FretRelativeIndex;
        public Color Color;

        public EnemyNoteInfo(string note, int stringNum, int fret, int fretRelativeIndex, Color color)
        {
            Note = note;
            StringNum = stringNum;
            Fret = fret;
            FretRelativeIndex = fretRelativeIndex;
            Color = color;
        }
    }
}