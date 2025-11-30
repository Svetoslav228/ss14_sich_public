using Content.Shared.Sich.Eye;

namespace Content.Server.Sich.Eye;
public sealed partial class SichEyeBlinkingSystem : SharedSichEyeBlinkingSystem
{
    public override void Update(float frameTime)
    {
        base.Update(frameTime);

        var query = EntityQueryEnumerator<SichEyeBlinkingComponent>();

        while (query.MoveNext(out var uid, out var comp))
        {
            if (comp.IsSleeping)
                continue;
            comp.NextBlinkTimer -= frameTime;
            if (comp.NextBlinkTimer <= 0f)
            {
                comp.NextBlinkTimer = comp.BlinkInterval;
                comp.BlinkDurationTimer = comp.BlinkDuration;

                TriggerBlink(uid, comp);
            }

            if (comp.BlinkDurationTimer > 0f)
            {
                comp.BlinkDurationTimer -= frameTime;
                if (comp.BlinkDurationTimer <= 0f)
                {
                    EndBlink(uid, comp);
                }
            }
        }
    }

    protected override void EndBlink(EntityUid uid, SichEyeBlinkingComponent comp)
    {
    }

    protected override void TriggerBlink(EntityUid uid, SichEyeBlinkingComponent comp)
    {
    }
}
