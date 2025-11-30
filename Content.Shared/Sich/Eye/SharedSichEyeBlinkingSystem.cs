using Content.Shared.Bed.Sleep;

namespace Content.Shared.Sich.Eye;
public abstract partial class SharedSichEyeBlinkingSystem : EntitySystem
{
    [Dependency] private readonly SharedAppearanceSystem _appearance = default!;

    override public void Initialize()
    {
        base.Initialize();
        EntityManager.ComponentAdded += OnComponentAdded;
        EntityManager.ComponentRemoved += OnComponentRemoved;
        SubscribeLocalEvent<SichEyeBlinkingComponent, ComponentRemove>(OnBlinkingRemoved);
        SubscribeLocalEvent<SichEyeBlinkingComponent, ComponentShutdown>(OnBlinkingShutdown);
    }

    private void OnBlinkingRemoved(Entity<SichEyeBlinkingComponent> ent, ref ComponentRemove args)
    {
        EndBlink(ent.Owner, ent.Comp);
    }

    private void OnBlinkingShutdown(Entity<SichEyeBlinkingComponent> ent, ref ComponentShutdown args)
    {
        EndBlink(ent.Owner, ent.Comp);
    }

    private void OnComponentRemoved(RemovedComponentEventArgs args)
    {
        var comp = args.BaseArgs.Component;
        var entUID = args.BaseArgs.Owner;
        if (!(comp is SleepingComponent)) return;
        if (TryComp<SichEyeBlinkingComponent>(entUID, out var eyeComp))
        {
            eyeComp.IsSleeping = false;

            eyeComp.NextBlinkTimer = eyeComp.BlinkInterval;
            eyeComp.BlinkDurationTimer = 0f;
            EndBlink(entUID, eyeComp); //Make sure eyes are open when waking up
            Dirty(entUID, eyeComp);
        }
    }

    private void OnComponentAdded(AddedComponentEventArgs args)
    {
        var comp = args.BaseArgs.Component;
        var entUID = args.BaseArgs.Owner;
        if (!(comp is SleepingComponent)) return;
        if (TryComp<SichEyeBlinkingComponent>(entUID, out var eyeComp))
        {
            eyeComp.IsSleeping = true;

            eyeComp.NextBlinkTimer = eyeComp.BlinkInterval;
            eyeComp.BlinkDurationTimer = 0f;
            TriggerBlink(entUID, eyeComp); //Make sure eyes are open when waking up
            Dirty(entUID, eyeComp);
        }
    }

    public override void Update(float frameTime)
    {
        base.Update(frameTime);

        var query = EntityQueryEnumerator<SichEyeBlinkingComponent>();

        while (query.MoveNext(out var uid, out var comp))
        {
            if (comp.IsSleeping)
                continue;

            if (comp.NextBlinkTimer <= 0f)
            {
                TriggerBlink(uid, comp);
            }

            if (comp.BlinkDurationTimer > 0f)
            {
                if (comp.BlinkDurationTimer <= 0f)
                {
                    EndBlink(uid, comp);
                }
            }
        }
    }

    protected virtual void EndBlink(EntityUid uid, SichEyeBlinkingComponent comp)
    {
        if (!TryComp<AppearanceComponent>(uid, out var appearance))
            return;

        UpdateAppearance(uid, appearance, false);
    }

    protected virtual void TriggerBlink(EntityUid uid, SichEyeBlinkingComponent comp)
    {
        if (!TryComp<AppearanceComponent>(uid, out var appearance))
            return;

        UpdateAppearance(uid, appearance, true);
    }

    protected virtual void UpdateAppearance(EntityUid uid, AppearanceComponent appearance, bool isBlinking)
    {
        _appearance.SetData(uid, SichEyeBlinkingVisuals.Blinking, isBlinking, appearance);
    }
}

public enum SichEyeBlinkingVisuals
{
    Blinking
}
