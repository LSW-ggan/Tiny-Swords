using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
public class Sanctuary : Skill {
    public Sanctuary(SkillData data) : base(data) {
        LastUsedTime = -data.CoolTime;
    }

    public override void SkillUse(GameObject player) {
        LastUsedTime = Time.time;

        ManaDecrease();

        player.GetComponent<PlayerController>().StartCoroutine(CoSanctuary(player));
        player.GetComponent<Animator>().SetBool("isRunning", false);
    }

    private IEnumerator CoSanctuary(GameObject player) {
        PlayerController playerController = player.GetComponent<PlayerController>();
        playerController._isAttacking = true;

        Vector3 dir;
        if (player.transform.localScale.x > 0) {
            dir = Vector3.right;
            Data.EffectPrefab.transform.rotation = Quaternion.Euler(0, 0, 0);
        }
        else {
            dir = Vector3.left;
            Data.EffectPrefab.transform.rotation = Quaternion.Euler(0, 180f, 0);
        }

        GameObject hammer = GameObject.Instantiate(Data.EffectPrefab, player.transform.position, Data.EffectPrefab.transform.rotation);
        SanctuaryHammer hammerLogic = hammer.GetComponent<SanctuaryHammer>();
        if (hammerLogic != null) {
            int attackBase = (int)(PlayerDataManager.Instance.Attack * Data.BuffAmount[SkillLevel]);
            hammerLogic.Init(player, attackBase);
        }
        yield return null;
    }
}
