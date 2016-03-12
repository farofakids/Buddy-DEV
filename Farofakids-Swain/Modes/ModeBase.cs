using EloBuddy;
using EloBuddy.SDK;

namespace Farofakids_Swain.Modes
{
    public abstract class ModeBase
    {
        protected Spell.Targeted Q
        {
            get { return SpellManager.Q; }
        }
        protected Spell.Skillshot W
        {
            get { return SpellManager.W; }
        }
        protected Spell.Targeted E
        {
            get { return SpellManager.E; }
        }
        protected Spell.Active R
        {
            get { return SpellManager.R; }
        }
        public abstract bool ShouldBeExecuted();

        public abstract void Execute();
    }
}
