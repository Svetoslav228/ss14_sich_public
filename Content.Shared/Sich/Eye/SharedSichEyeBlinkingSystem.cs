using Content.Shared.Bed.Sleep;
using Robust.Shared.Serialization;
using Robust.Shared.Timing;

namespace Content.Shared.Sich.Eye;
public abstract partial class SharedSichEyeBlinkingSystem : EntitySystem
{
    [Dependency] private readonly SharedAppearanceSystem _appearance = default!;
    [Dependency] private readonly IGameTiming _timing = default!;

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
        OpenEyes(ent.Owner, ent.Comp);
    }

    private void OnBlinkingShutdown(Entity<SichEyeBlinkingComponent> ent, ref ComponentShutdown args)
    {
        OpenEyes(ent.Owner, ent.Comp);
    }

    private void OnComponentRemoved(RemovedComponentEventArgs args)
    {
        var comp = args.BaseArgs.Component;
        var entUID = args.BaseArgs.Owner;
        if (!(comp is SleepingComponent)) return;
        if (TryComp<SichEyeBlinkingComponent>(entUID, out var eyeComp))
        {
            eyeComp.IsSleeping = false;

            OpenEyes(entUID, eyeComp);
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
            Blink(entUID, eyeComp, _timing.CurTime);
            Dirty(entUID, eyeComp);
        }
    }

    public override void Update(float frameTime)
    {
        base.Update(frameTime);

        var curTime = _timing.CurTime;

        var query = EntityQueryEnumerator<SichEyeBlinkingComponent>();

        while (query.MoveNext(out var uid, out var comp))
        {
            if (comp.IsSleeping) continue;
            if (comp.NextBlinkingTime > curTime)
                continue;

            if (comp.IsBlinking)
            {
                if (curTime >= comp.LastBlinkTime + comp.BlinkDuration)
                {
                    OpenEyes(uid, comp);
                }
            }
            else
            {
                if (comp.NextBlinkingTime <= curTime)
                {
                    Blink(uid, comp, curTime);
                }
            }
        }
    }

    private void Blink(EntityUid uid, SichEyeBlinkingComponent comp, TimeSpan curTime)
    {
        comp.IsBlinking = true;
        comp.LastBlinkTime = curTime;

        comp.NextBlinkingTime = curTime + comp.BlinkInterval;

        Dirty(uid, comp);

        if (!TryComp<AppearanceComponent>(uid, out var appearance))
            return;
        UpdateAppearance(uid, appearance, true);
    }

    private void OpenEyes(EntityUid uid, SichEyeBlinkingComponent comp)
    {
        comp.IsBlinking = false;
        Dirty(uid, comp);
        if (!TryComp<AppearanceComponent>(uid, out var appearance))
            return;
        UpdateAppearance(uid, appearance, false);
    }

    protected virtual void UpdateAppearance(EntityUid uid, AppearanceComponent appearance, bool isBlinking)
    {
        _appearance.SetData(uid, SichEyeBlinkingVisuals.Blinking, isBlinking, appearance);
    }
}

[Serializable, NetSerializable]
public enum SichEyeBlinkingVisuals : byte
{
    Blinking
}
