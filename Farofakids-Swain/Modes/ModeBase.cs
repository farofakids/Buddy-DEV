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
        protected Spell.Targeted _ignite
        {
            get { return SpellManager._ignite; }
        }
        public abstract bool ShouldBeExecuted();

        public abstract void Execute();
    }
}
