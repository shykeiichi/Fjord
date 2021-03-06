using static SDL2.SDL_mixer;
using System.Collections.Generic;
using System;

namespace Fjord.Modules.Sound {
    public static class Sound {
        public static Dictionary<string, IntPtr> sounds = new Dictionary<string, IntPtr>();

        public static void load(string path, string id) {
            if(!sounds.ContainsKey(id))
                sounds.Add(id, Mix_LoadWAV( game.get_resource_folder() + "/" + game.asset_pack + "/assets/sounds/" + path + ".wav" ));
            else
                Debug.Debug.send("Sounds already contains id '" + id + "'!");
        }

        public static void play(string id) {
            Mix_PlayChannel( -1, sounds[id], 0 );
        }

        public static IntPtr get(string id) {
            return sounds[id];
        }
    }
}