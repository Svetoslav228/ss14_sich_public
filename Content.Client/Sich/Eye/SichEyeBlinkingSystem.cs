using Content.Shared.Humanoid;
using Content.Shared.Sich.Eye;
using Robust.Client.GameObjects;

namespace Content.Client.Sich.Eye;
public sealed partial class SichEyeBlinkingSystem : SharedSichEyeBlinkingSystem
{
    [Dependency] private readonly SharedAppearanceSystem _appearance = default!;
    [Dependency] private readonly SpriteSystem _sprite = default!;

    public override void Initialize()
    {
        base.Initialize();
        SubscribeLocalEvent<SichEyeBlinkingComponent, AppearanceChangeEvent>(OnAppearance);
        SubscribeLocalEvent<SichEyeBlinkingComponent, ComponentStartup>(OnStartup);
    }

    private void OnStartup(Entity<SichEyeBlinkingComponent> ent, ref ComponentStartup args)
    {
        if (!TryComp<SpriteComponent>(ent.Owner, out var spriteComponent))
            return;

        if (_appearance.TryGetData(ent.Owner, SichEyeBlinkingVisuals.Blinking, out var stateObj) && stateObj is bool state)
        {
            ChangeEyeState(ent, spriteComponent, state);
        }
    }

    private void OnAppearance(Entity<SichEyeBlinkingComponent> ent, ref AppearanceChangeEvent args)
    {
        if (!TryComp<SpriteComponent>(ent.Owner, out var spriteComponent))
            return;

        if (args.AppearanceData.TryGetValue(SichEyeBlinkingVisuals.Blinking, out var stateObj) && stateObj is bool state)
        {
            ChangeEyeState(ent, spriteComponent, state);
        }
    }

    private void ChangeEyeState(Entity<SichEyeBlinkingComponent> ent, SpriteComponent sprite, bool isBlinking)
    {
        if (!TryComp<HumanoidAppearanceComponent>(ent.Owner, out var humanoid)) return;
        var blinkFade = ent.Comp.BlinkSkinColorMultiplier;
        var blinkColor = new Color(
            humanoid.SkinColor.R * blinkFade,
            humanoid.SkinColor.G * blinkFade,
            humanoid.SkinColor.B * blinkFade);
        sprite[_sprite.LayerMapReserve((ent.Owner, sprite), HumanoidVisualLayers.Eyes)].Color = isBlinking ? blinkColor : humanoid.EyeColor;
    }
}
