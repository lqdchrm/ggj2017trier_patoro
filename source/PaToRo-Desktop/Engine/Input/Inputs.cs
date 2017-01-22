using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaToRo_Desktop.Engine.Input
{
    public class Inputs : List<InputState>
    {
        private readonly Dictionary<int, int> playerMap;

        public int NumPlayers { get { return playerMap.Count; } }

        public Inputs()
        {
            playerMap = new Dictionary<int, int>();
        }

        public InputState Player(int playerNo)
        {
            if (playerMap.ContainsKey(playerNo))
                return this[playerMap[playerNo]];

            return null;
        }

        public void ReAssignToPlayers()
        {
            playerMap.Clear();
        }

        public bool AssignToPlayer(int playerNo)
        {
            for(int i=0; i<this.Count; ++i)
            {
                var inputState = this[i];
                if (!playerMap.ContainsValue(i) && inputState.AnyButtonDown)
                {
                    playerMap[playerNo] = i;
                    //Console.WriteLine(string.Format("Assigned Controller {0} to Player {1}", i, playerNo));
                    return true;
                }
            }
            return false;
        }

        public void Update(GameTime gameTime)
        {
            foreach(var input in this)
                input.Update(gameTime);
        }
    }
}
