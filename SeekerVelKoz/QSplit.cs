using System;
using System.Collections.Generic;
using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using SharpDX;

namespace SeekerVelKoz
{
    public static class QSplit
    {
        private const int MissileSpeed = 2100;
        private const int CastDelay = 250;
        private const int SpellWidth = 45;
        private const int SpellRange = 1100;

        static QSplit()
        {
            // Initialize properties
            Perpendiculars = new List<Vector2>();

            // Listen to required events
            Game.OnTick += OnTick;
            GameObject.OnCreate += OnCreate;
        }

        private static MissileClient Handle { get; set; }
        private static Vector2 Direction { get; set; }
        private static List<Vector2> Perpendiculars { get; set; }

        public static void Initialize()
        {

        }
        
        private static void OnCreate(GameObject sender, EventArgs args)
        {
            // Check if the sender is a MissleClient
            var missile = sender as MissileClient;
            if (missile != null && missile.SpellCaster.IsMe && missile.SData.Name == "VelkozQMissile")
            {
                // Apply the needed values
                Handle = missile;
                Direction = (missile.EndPosition.To2D() - missile.StartPosition.To2D()).Normalized();
                Perpendiculars.Add(Direction.Perpendicular());
                Perpendiculars.Add(Direction.Perpendicular2());
            }
        }

        private static void OnTick(EventArgs args)
        {
            // Check if the missile is active
            if (Handle != null && SpellManager.Q.IsReady() && SpellManager.Q.Name == "velkozqsplitactivate")
            {
                foreach (var perpendicular in Perpendiculars)
                {
                    if (Handle != null)
                    {
                        var startPos = Handle.Position.To2D();
                        var endPos = Handle.Position.To2D() + SpellRange*perpendicular;

                        var collisionObjects = ObjectManager.Get<Obj_AI_Base>()
                            .Where(o => o.IsEnemy && !o.IsDead && o.IsHPBarRendered &&
                                        !o.IsStructure() && !o.IsWard() && !o.IsInvulnerable &&
                                        o.Distance(Player.Instance, true) < (SpellRange + 200).Pow() &&
                                        o.ServerPosition.To2D().Distance(startPos, endPos, true, true) <=
                                        (SpellWidth*2 + o.BoundingRadius).Pow());

                        var colliding =
                            collisionObjects.Where(
                                o => o.Type == GameObjectType.AIHeroClient && o.IsValidTarget()
                                    && Prediction.Position.Collision.LinearMissileCollision(o, startPos, endPos,
                                        MissileSpeed, SpellWidth, CastDelay, (int)o.BoundingRadius))
                                .OrderBy(o => o.Distance(Player.Instance, true))
                                .FirstOrDefault();

                        if (colliding != null)
                        {
                            Player.CastSpell(SpellSlot.Q);
                            Handle = null;
                        }
                    }
                }
            }
            else
            {
                Handle = null;
            }
        }
    }
}