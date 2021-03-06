using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using HarmonyLib;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.SandBox.GameComponents;
using TaleWorlds.Core;
using static System.Reflection.BindingFlags;
using static CommunityPatch.HarmonyHelpers;

namespace CommunityPatch.Patches.Perks.Intelligence.Steward {

  public sealed class ProsperousReignPatch : PerkPatchBase<ProsperousReignPatch> {

    public override bool Applied { get; protected set; }

    private static readonly MethodInfo TargetMethodInfo = typeof(DefaultSettlementProsperityModel).GetMethod("CalculateHearthChangeInternal", NonPublic | Instance | DeclaredOnly);

    private static readonly MethodInfo PatchMethodInfo1 = typeof(ProsperousReignPatch).GetMethod(nameof(Prefix), NonPublic | Static | DeclaredOnly);

    private static readonly MethodInfo PatchMethodInfo2 = typeof(ProsperousReignPatch).GetMethod(nameof(Postfix), NonPublic | Static | DeclaredOnly);

    public override IEnumerable<MethodBase> GetMethodsChecked() {
      yield return TargetMethodInfo;
    }

    public static readonly byte[][] Hashes = {
      new byte[] {
        // e1.1.0.224785
        0x93, 0x92, 0xD6, 0xB0, 0x05, 0x07, 0xEF, 0xFA,
        0xFA, 0x86, 0x1D, 0xC4, 0x3A, 0xA3, 0x78, 0x07,
        0x27, 0x33, 0x4A, 0x6A, 0xCC, 0xB7, 0x9F, 0xFF,
        0x49, 0x3A, 0xE8, 0x50, 0x71, 0x54, 0x89, 0x0A
      },
      new byte[] {
        // e1.4.0.228531
        0xC6, 0xA0, 0xD4, 0xC3, 0x67, 0xE7, 0x6E, 0x43,
        0x61, 0x47, 0x3D, 0xC7, 0x79, 0x6F, 0xFC, 0x83,
        0x81, 0x61, 0x0A, 0x22, 0x8B, 0x88, 0xF7, 0x87,
        0x61, 0x1D, 0x64, 0x8C, 0x31, 0xB1, 0x1B, 0x45
      },
      new byte[] {
        // e1.4.1.229326
        0x3D, 0x36, 0x0D, 0x6F, 0x84, 0x47, 0x87, 0x16,
        0xD2, 0xFD, 0xCF, 0x51, 0x18, 0x48, 0x30, 0x2B,
        0xE5, 0x74, 0x65, 0x34, 0x35, 0x83, 0x1B, 0x28,
        0x2C, 0xF7, 0x17, 0xCF, 0x77, 0x26, 0x5F, 0xD3
      },
      new byte[] {
        // e1.4.1.230546
        0x3F, 0x88, 0xAA, 0xB6, 0x97, 0x2B, 0x9B, 0x92,
        0x91, 0xE3, 0x8F, 0xC9, 0x18, 0x02, 0x82, 0xDF,
        0x7C, 0x6F, 0x18, 0xFA, 0x71, 0x4E, 0x85, 0x25,
        0xF9, 0x20, 0x74, 0x9D, 0x35, 0x7C, 0xB0, 0x82
      },
      new byte[] {
        // e1.4.3.237794
        0x87, 0xC8, 0x6F, 0x46, 0x2B, 0x47, 0xC3, 0x17,
        0xAE, 0x18, 0xC3, 0xF7, 0x4F, 0xFB, 0xBF, 0x72,
        0x34, 0xC8, 0xBE, 0xEB, 0x6F, 0x27, 0xCC, 0x6C,
        0xF9, 0xEB, 0xBA, 0x3D, 0xB0, 0x82, 0x27, 0xB5
      }
    };

    public ProsperousReignPatch() : base("5MjmCaUx") {
    }

    public override void Apply(Game game) {
      if (Applied) return;

      CommunityPatchSubModule.Harmony.Patch(TargetMethodInfo,
        new HarmonyMethod(PatchMethodInfo1),
        new HarmonyMethod(PatchMethodInfo2));
      Applied = true;
    }

    public override bool? IsApplicable(Game game) {
      var patchInfo = Harmony.GetPatchInfo(TargetMethodInfo);
      if (AlreadyPatchedByOthers(patchInfo))
        return false;

      var hash = TargetMethodInfo.MakeCilSignatureSha256();
      if (!hash.MatchesAnySha256(Hashes))
        return false;

      return base.IsApplicable(game);
    }

    // ReSharper disable once UnusedParameter.Local

    private static void Prefix(Village village, ref StatExplainer explanation)
      => explanation ??= new StatExplainer();

    // ReSharper disable once InconsistentNaming

    private static void Postfix(ref float __result, Village village, StatExplainer explanation) {
      var perk = ActivePatch.Perk;
      if (!(village.Bound?.OwnerClan?.Leader?.GetPerkValue(perk) ?? false))
        return;

      var explainedNumber = new ExplainedNumber(__result, explanation);

      if (!(__result > -0.0001f) || !(__result < 0.0001f) && explanation.Lines.Count > 0)
        explanation.Lines.RemoveAt(explanation.Lines.Count - 1);

      var extra = explanation.Lines.Where(line => line.Number > 0).Sum(line => line.Number);

      if (extra < float.Epsilon)
        return;

      explainedNumber.Add(extra * perk.PrimaryBonus - extra, perk.Name);
      __result = explainedNumber.ResultNumber;
    }

  }

}