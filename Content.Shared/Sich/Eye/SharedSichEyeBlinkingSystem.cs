using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Content.Shared.Sich.Eye;
public abstract partial class SharedSichEyeBlinkingSystem : EntitySystem
{
    [Dependency] private readonly SharedAppearanceSystem _appearance = default!;

    override public void Initialize()
    {
        base.Initialize();
        SubscribeLocalEvent<SichEyeBlinkingComponent, ComponentInit>(OnBlinkingInit);
    }

    private void OnBlinkingInit(Entity<SichEyeBlinkingComponent> ent, ref ComponentInit args)
    {
        throw new NotImplementedException();
    }

    public override void Update(float frameTime)
    {
        base.Update(frameTime);

        var query = EntityQueryEnumerator<SichEyeBlinkingComponent>();

        while (query.MoveNext(out var uid, out var comp))
        {
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

    private void EndBlink(EntityUid uid, SichEyeBlinkingComponent comp)
    {
        if (!TryComp<AppearanceComponent>(uid, out var appearance))
            return;

        UpdateAppearance(uid, appearance, false);
    }

    private void TriggerBlink(EntityUid uid, SichEyeBlinkingComponent comp)
    {
        if (!TryComp<AppearanceComponent>(uid, out var appearance))
            return;

        UpdateAppearance(uid, appearance, true);
    }

    private void UpdateAppearance(EntityUid uid, AppearanceComponent appearance, bool isBlinking)
    {
        _appearance.SetData(uid, SichEyeBlinkingVisuals.Blinking, isBlinking, appearance);
    }
}

public enum SichEyeBlinkingVisuals
{
    Blinking
}
