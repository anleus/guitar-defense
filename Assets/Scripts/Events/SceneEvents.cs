using System;
using Models.Enums;

namespace Events
{
    public static class SceneEvents
    {
        public static event Action<SceneId> OnSceneChangeRequested;
        public static void SceneChange(SceneId id)
        {
            OnSceneChangeRequested?.Invoke(id);
        }
    }
}