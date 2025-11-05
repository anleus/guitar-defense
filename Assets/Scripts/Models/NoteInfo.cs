using System;

namespace Models
{
    [Serializable]
    public class NoteInfo
    {
        public string Note { get; }
        public float Frequency { get; }

        public NoteInfo(string note, float frequency)
        {
            Note = note;
            Frequency = frequency;
        }

        public override string ToString()
        {
            return $"{Note}-{Frequency}";
        }
    }
}