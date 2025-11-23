using UnityEngine;

namespace Tcg_cricket
{
    public class GameData : MonoBehaviour
    {
        #region Game States

        public enum GameState
        {
            None,
            Lobby,              // Waiting for 2 players to join
            MatchStart,         // Decks created, initial hand drawn

            TurnStart,          // Start of each turn: +1 draw, cost update
            PlayerAction,       // Players selecting cards (30s timer)
            WaitingForEnd,      // One player ended, waiting for the other

            Reveal,             // Both players' cards revealed
            Resolve,            // Base power + abilities processed

            TurnEnd,            // Cleanup, increment turn
            MatchEnd            // Winner decided after 6 turns
        }
        public enum PlayerState
        {
            None,
            Drawing,            // Start turn: drawing + setting cost
            Selecting,          // Player choosing cards
            EndedTurn,          // Player pressed End Turn
            Locked              // During reveal/resolve
        }
        
        public enum TurnPhase
        {
            None,
            Select,             // PlayerAction
            Reveal,             // Reveal phase
            Resolve,            // Server applies abilities
            Cleanup             // Prepare next turn
        }
        
        public enum NetAction
        {
            GameStart,
            TurnStart,
            PlayCards,
            EndTurn,
            RevealCards,
            GameEnd,
            FullState
        }
        public enum AbilityType
        {
            None,
            GainPoints,                 // +value to score
            StealPoints,                // steal value points
            DoublePower,                // multiply power * value
            DrawExtraCard,              // draw N extra cards
            DiscardOpponentRandomCard,  // remove N from opponent hand
            DestroyOpponentCardInPlay   // remove N from opponent reveal zone
        }
        
        public enum UIState
        {
            None,
            Waiting,
            SelectingCards,
            ShowingReveal,
            ShowingResult
        }
        #endregion
    }
}