using System.Diagnostics.CodeAnalysis;
using System.Numerics;
using Raylib_cs;

namespace BerryGame
{
    public class Minion : GameObject, IDrawable
    {
        public int Layer => 1;
        public int Depth => 2;

        public Bush? TargetBush;
        public Core TargetCore => Shared.Core;

        public Stack<Berry> HeldBerries = [];
        public int BerryLimit = 10;

        public MinionState State;

        public float Timer;
        public float HarvestTime = 0.1f;
        public float DepositTime = 0.3f;

        public bool anim_Started;
        public Vector2 anim_Position;

        public override void Update()
        {
            switch (State)
            {
                case MinionState.Idle:
                    if (!FindBush() && HeldBerries.Count > 0)
                        State = MinionState.ToCore;
                    break;
                case MinionState.ToBush:
                    WalkToBush(); break;
                case MinionState.Collect:
                    CollectBerry(); break;
                case MinionState.ToCore:
                    WalkToCore(); break;
                case MinionState.Deposit:
                    DepositBerry(); break;
                default:
                    break;
            }
            UpdateBerries();
        }

        public bool FindBush()
        {
            if (IsBushValid() && !Shared.SmartMinions)
            { State = MinionState.ToBush; return true; }

            if (!Manager.TryFindAll(out Bush[] bushes))
            { State = MinionState.Idle; return false; }

            Bush[] active = [.. bushes.Where(static b => b.State is BushState.Idle)];
            if (active.Length < 1)
            { State = MinionState.Idle; return false; }

            if (Shared.SmartMinions)
            {
                TargetBush = active
                    .OrderBy(b => Vector2.DistanceSquared(b.Position, Position))
                    .First();

                float DistanceToCore = Vector2.DistanceSquared(Position, TargetCore.Position);
                float DistanceToBush = Vector2.DistanceSquared(Position, TargetBush.Position);

                if (HeldBerries.Count > 0 &&
                    DistanceToCore < DistanceToBush)
                { State = MinionState.ToCore; return true; }
            }
            else
            {
                int index = Shared.RNG.Next(active.Length);
                TargetBush = active[index];
            }

            State = MinionState.ToBush;
            return true;
        }

        [MemberNotNullWhen(true, nameof(TargetBush))]
        public bool IsBushValid()
            => TargetBush != null && TargetBush.State == BushState.Idle;

        public void WalkToBush()
        {
            if (!IsBushValid())
            { State = MinionState.Idle; return; }
            Vector2 direction = Vector2.Normalize(TargetBush.Position - Position);
            Position += direction * TimeManager.Delta * 300f;
            if (Vector2.Distance(Position, TargetBush.Position) < 20)
                State = MinionState.Collect;
        }

        public void CollectBerry()
        {
            if (Timer < HarvestTime)
            {
                Timer += TimeManager.Delta;
                if (!anim_Started)
                {
                    if (!IsBushValid() || !TargetBush.TryGetBerry(out Berry? berry))
                    { State = MinionState.Idle; return; }
                    anim_Started = true;
                    anim_Position = berry.Position;
                    HeldBerries.Push(berry);
                    berry.Carry();
                }
                return;
            }
            Timer -= HarvestTime;
            anim_Started = false;

            if (HeldBerries.Count >= BerryLimit)
                State = MinionState.ToCore;
        }

        public void WalkToCore()
        {
            if (HeldBerries.Count == 0)
            { State = MinionState.Idle; return; }

            Vector2 direction = Vector2.Normalize(TargetCore.Position - Position);
            Position += direction * TimeManager.Delta * 300f;
            if (Vector2.Distance(Position, TargetCore.Position) < 90)
            { State = MinionState.Deposit; }
        }

        public void DepositBerry()
        {
            if (Timer < DepositTime)
            { Timer += TimeManager.Delta; return; }
            Timer -= DepositTime;

            if (!HeldBerries.TryPop(out Berry? berry))
            { State = MinionState.Idle; return; }
            TargetCore.Collect(berry);
            if (HeldBerries.Count == 0)
            { State = MinionState.Idle; return; }
        }

        public void UpdateBerries()
        {
            if (HeldBerries.Count != 0)
            {
                float height = Size.Y / 2.0f;
                foreach (Berry berry in HeldBerries.Reverse())
                {
                    height += berry.Size.Y / 2.0f;
                    berry.Position = Position - (Vector2.UnitY * height);
                    height += berry.Size.Y / 2.0f;
                }
            }

            if (State == MinionState.Deposit)
            {
                float p = Timer / DepositTime;
                if (HeldBerries.TryPeek(out Berry? berry))
                    berry.Position = Vector2.Lerp(berry.Position, TargetCore.Position, p);
            }

            if (State == MinionState.Collect && anim_Started)
            {
                float p = Timer / HarvestTime;
                if (HeldBerries.TryPeek(out Berry? berry))
                    berry.Position = Vector2.Lerp(anim_Position, berry.Position, p);
            }
        }

        public void Draw()
        {
            Raylib.BeginMode2D(Shared.Camera);
            Raylib.DrawRectangleRec(Rect, Color.Blue);
            Raylib.DrawRectangleLinesEx(Rect, 2, Color.DarkBlue);

            if (State == MinionState.Deposit && HeldBerries.TryPeek(out Berry? berry))
                Raylib.DrawLineV(berry.Position, TargetCore.Position, Color.Orange);

            if (IsBushValid())
                Raylib.DrawLineV(TargetBush.Position, Position, Color.Yellow);


            if (Shared.SmartMinions && Manager.TryFindAll(out Bush[] bushes))
            {
                Bush[] active = [.. bushes.Where(static b => b.State is BushState.Idle)];
                if (active.Length > 0)
                {
                    Bush bush = active.OrderBy(b => Vector2.Distance(b.Position, Position))
                     .First();

                    float DistanceToCore = Vector2.DistanceSquared(Position, TargetCore.Position);
                    float DistanceToBush = Vector2.DistanceSquared(Position, bush.Position);

                    bool held = HeldBerries.Count > 0;
                    bool full = HeldBerries.Count == BerryLimit;

                    bool tocore = (DistanceToCore < DistanceToBush && held) || full;

                    Raylib.DrawLineV(Position, bush.Position, !tocore ? Color.Green : Color.Red);
                    Raylib.DrawLineV(Position, TargetCore.Position, tocore ? Color.Green : Color.Red);
                }
            }

            Raylib.EndMode2D();
        }
    }

    public enum MinionState
    {
        Idle = 0,
        ToBush,
        Collect,
        ToCore,
        Deposit
    }
}
