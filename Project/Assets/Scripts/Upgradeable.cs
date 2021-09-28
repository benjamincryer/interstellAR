using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface Upgradeable
{
    void ApplyUpgrade(Upgrade upgrade);
    void ApplyUpgrades();
}
