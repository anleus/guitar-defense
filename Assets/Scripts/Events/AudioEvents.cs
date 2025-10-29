using System;
using Audio.Models;
using UnityEngine;

namespace Events
{
    public static class AudioEvents
    {
        
        public static event Action<NoteInfo> OnNoteDetected;
        public static void NoteDetected(NoteInfo noteInfo)
        {
            OnNoteDetected?.Invoke(noteInfo);
        }
    }
}