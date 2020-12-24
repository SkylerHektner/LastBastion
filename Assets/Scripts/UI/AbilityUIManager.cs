using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AbilityUIManager : MonoBehaviour
{
    [SerializeField] float TimeScaleLerpDuration = 0.3f;
    private float TimeScaleReturnToNormalLerpDuratin = 0.1f; // this should always be extremely fast
    [SerializeField] List<GameObject> ShowOnAbilityMenuActive;
    [SerializeField] Animator GameplayFieldScrim;

    private bool showing = false;
    private AbilityEnum? cur_ability_candidate = null;

    private void Start()
    {
        HideIcons();
    }

    private void Update()
    {
        if( showing )
        {
#if PC || UNITY_EDITOR
            if( !Input.GetMouseButton( 0 ) )
#endif
#if MOBILE && !UNITY_EDITOR
            if( Input.touchCount == 0 )
#endif
            {
                if( cur_ability_candidate != null )
                {
                    AbilityManager.Instance.UseAbility( (AbilityEnum)cur_ability_candidate );
                }
                HideIcons();
            }
        }
    }

    public void ShowIcons()
    {
        foreach( var g in ShowOnAbilityMenuActive )
            g.SetActive( true );
        showing = true;
        cur_ability_candidate = null;
        GameplayManager.Instance.SetTimeScale( 0.1f, TimeScaleLerpDuration );
        GameplayFieldScrim.gameObject.SetActive( true );
        GameplayFieldScrim.SetBool( "Fade", true );
    }

    private void HideIcons()
    {
        foreach( var g in ShowOnAbilityMenuActive )
            g.SetActive( false );
        showing = false;
        cur_ability_candidate = null;
        GameplayManager.Instance.SetTimeScale( 1.0f, TimeScaleReturnToNormalLerpDuratin );
        GameplayFieldScrim.SetBool( "Fade", false );
        GameplayFieldScrim.gameObject.SetActive( false );
    }

    public void SetAbilityCandidate( string ability )
    {
        if( !AbilityManager.AbilityStringMap.ContainsKey( ability ) )
        {
            Debug.LogError( "ERROR: No corresponding ability for ability string " + ability );
            return;
        }
        cur_ability_candidate = AbilityManager.AbilityStringMap[ability];
    }

}
