 //
 // Copyright (C) 2022 Stuart Heath. All rights reserved.
 //

 using DG.Tweening;
 using UnityEngine;
using UnityEngine.EventSystems;

 /// <summary>
    ///ButtonHoverEffectScale is a script that is attached to a button and scales the button when the mouse hovers over it.
    /// </summary>
    
    public class ButtonHoverEffectScale : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
	    [SerializeField] private float _scaleAmount = 1.1f;
	    [SerializeField] private float _scaleDuration = 0.2f;
   		
	    public void OnPointerEnter(PointerEventData eventData) => Hover();
   		
	    private void Hover()
	    {
		    transform.DOScale(Vector3.one * _scaleAmount, 0.2f);
	    }
   
	    public void OnPointerExit(PointerEventData eventData) => UnHover();
   
	    private void UnHover()
	    {
		    transform.DOScale(Vector3.one, 0.2f);
	    }
    }