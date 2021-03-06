using System.Collections.Generic;
using System.Linq;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.SandBox.GameComponents;
using TaleWorlds.Core;

namespace CommunityPatch.Patches {

  public class DailyInfluenceGainPatch : PatchBase<DailyInfluenceGainPatch> {

    private static List<IDailyInfluenceGain> _subPatches;
    
    public static readonly byte[][] Hashes = {
      new byte[] {
        // e1.0.4
        0x9D, 0x7D, 0x06, 0x79, 0x02, 0x3A, 0xDE, 0x53,
        0x88, 0xA4, 0x61, 0xCD, 0x09, 0x6C, 0xB5, 0x0E,
        0x02, 0x69, 0x74, 0xEE, 0x1B, 0xCD, 0xB8, 0xDB,
        0x61, 0x9F, 0xC2, 0x45, 0xF0, 0x4D, 0x06, 0x4F
      },
      new byte[] {
        // e1.0.5
        0x0D, 0x0F, 0x98, 0xD5, 0xA8, 0xD7, 0x33, 0x9F,
        0xCB, 0xCC, 0x40, 0xFE, 0x6A, 0x83, 0xBF, 0xA8,
        0xED, 0xB5, 0x15, 0xB2, 0xDA, 0xC0, 0xC9, 0xD2,
        0x61, 0xA3, 0x53, 0x81, 0xF7, 0xC2, 0x83, 0xA1
      },
      new byte[] {
        // e1.1.0.224785
        0xDB, 0x69, 0x3E, 0x84, 0xBE, 0x6B, 0x4C, 0xA6,
        0x13, 0x32, 0xFA, 0xA4, 0x06, 0xF5, 0xB5, 0xA3,
        0xF1, 0x2E, 0x47, 0xB5, 0xE8, 0x8F, 0x19, 0x84,
        0xCD, 0x21, 0xF8, 0x42, 0x24, 0xD7, 0x30, 0x31
      },
      new byte[] {
        // e1.4.0.228616
        0x70, 0x4A, 0xD6, 0xDA, 0x6E, 0xDC, 0x5C, 0x7E,
        0xF5, 0xDE, 0xDB, 0x68, 0x17, 0xD5, 0x67, 0x22,
        0xAC, 0x56, 0x86, 0x44, 0x07, 0x03, 0xBA, 0xCF,
        0x41, 0x1D, 0x43, 0xE6, 0x76, 0x39, 0x44, 0x32
      },
      new byte[] {
        // e1.4.1.231071
        0xD8, 0x16, 0x5F, 0x4F, 0xA7, 0x11, 0x1C, 0x32,
        0x8D, 0x13, 0xD8, 0x02, 0x21, 0x7E, 0x16, 0x69,
        0x85, 0x93, 0xE8, 0x93, 0x16, 0x86, 0x6D, 0xA2,
        0x93, 0x15, 0xDA, 0x54, 0x59, 0xE7, 0xBF, 0x0C
      },
      new byte[] {
        // e1.4.3.237794
        0xD4, 0x4C, 0xFF, 0xA7, 0x79, 0xBC, 0x6C, 0x34,
        0xF6, 0xC3, 0xC6, 0x9F, 0x21, 0x9A, 0x10, 0x93,
        0xD3, 0x26, 0xC1, 0x09, 0xDB, 0x2D, 0x64, 0x03,
        0xD8, 0x4A, 0xBE, 0x8C, 0x1A, 0xAC, 0xB8, 0x0A
      }
    };

    public override void Apply(Game game) {
      if (Applied) return;

      _subPatches = CommunityPatchSubModule.Patches
        .Where(p => p is IDailyInfluenceGain && p.IsApplicable(game) == true)
        .Cast<IDailyInfluenceGain>()
        .ToList();

      base.Apply(game);
    }

    [PatchClass(typeof(DefaultClanPoliticsModel))]
    private static void CalculateInfluenceChangeInternalPostfix(Clan clan, ref ExplainedNumber influenceChange) {
      foreach (var subPatch in _subPatches) {
        subPatch.ModifyDailyInfluenceGain(clan, ref influenceChange);
      }
    }

  }

}