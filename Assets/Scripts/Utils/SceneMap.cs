using System.Collections.Generic;
using Models.Enums;

namespace Utils
{
    public static class SceneMap
    {
        public static readonly Dictionary<SceneId, string> Names = new()
        {
            { SceneId.Base, "Base" },
            { SceneId.MainMenu, "MainMenu" },
            { SceneId.Gameplay, "Gameplay" },
            { SceneId.InitialSettings, "InitialSettings" },
        };
    }
}