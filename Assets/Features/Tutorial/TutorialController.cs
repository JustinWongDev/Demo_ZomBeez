using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialController : MonoBehaviour
{
    [SerializeField] private GameObject _mainMenu = null;
    [SerializeField] private CinemachineController _cmController = null;
    [SerializeField] private TutorialTextController _textController = null;
    
    private tutorialSlides _currentSlide = tutorialSlides.hive;


    private enum tutorialSlides
    {
        hive,
        depot,
        caches,
        book
    };

    private void Start()
    {
        this.gameObject.SetActive(false);
    }

    public void ChangeSlide(bool isForward)
    {
        int newSlide = 0;
        
        if (isForward)
        {
            newSlide = (int) _currentSlide + 1;

            if (newSlide > (int) tutorialSlides.book)
                newSlide = 0;
            
        }
        else
        {
            newSlide = (int) _currentSlide - 1;

            if (newSlide < 0)
            {
                ReturnToMainMenu();
                return;
            }
        }
        
        _currentSlide = (tutorialSlides)newSlide;
        _cmController.SwitchTutorialCams((int)_currentSlide);
        _textController.SwitchTutorialText((int)_currentSlide);
        
    }

    public void ReturnToMainMenu()
    {
        _cmController.SwitchTutorialCams(0);
        _mainMenu.gameObject.SetActive(true);
        this.gameObject.SetActive(false);
    }
}
