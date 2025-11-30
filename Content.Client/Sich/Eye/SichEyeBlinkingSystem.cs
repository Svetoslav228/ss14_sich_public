using Content.Shared.Humanoid;
using Content.Shared.Sich.Eye;
using Microsoft.CodeAnalysis.Elfie.Serialization;
using Robust.Client.GameObjects;
using Robust.Shared.GameObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Content.Client.Sich.Eye;
public sealed partial class SichEyeBlinkingSystem : SharedSichEyeBlinkingSystem
{
    [Dependency] private readonly SharedAppearanceSystem _appearance = default!;
    [Dependency] private readonly SpriteSystem _sprite = default!;

    public override void Initialize()
    {
        base.Initialize();
        SubscribeLocalEvent<SichEyeBlinkingComponent, AppearanceChangeEvent>(OnAppearance);
    }

    private void OnAppearance(Entity<SichEyeBlinkingComponent> ent, ref AppearanceChangeEvent args)
    {
        if (!TryComp<SpriteComponent>(ent.Owner, out var spriteComponent))
            return;
        if (args.AppearanceData.TryGetValue(SichEyeBlinkingVisuals.Blinking, out var stateObj) && stateObj is bool state)
        {
            spriteComponent[_sprite.LayerMapReserve((ent.Owner, spriteComponent), HumanoidVisualLayers.Eyes)].Visible = !state;
        }

    }
}
