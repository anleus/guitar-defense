using System;

namespace Models
{
    [Serializable]
    public class NoteInfo
    {
        public string Name { get; }
        public float Frequency { get; }

        public NoteInfo(string name, float frequency)
        {
            Name = name;
            Frequency = frequency;
        }

        public override string ToString()
        {
            return $"{Name}-{Frequency}";
        }
    }
}