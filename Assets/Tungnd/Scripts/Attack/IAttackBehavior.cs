using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public interface IAttackBehavior
{
    void ExecuteAttack(BotController bot);
}
