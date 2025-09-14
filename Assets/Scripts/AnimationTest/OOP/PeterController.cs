using System;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;

namespace AnimationTest.OOP
{
    public class PeterController : MonoBehaviour
    {
        public Animator animator;

        public float walkDistance;
        public float walkSpeed;

        public string walkAnimation = "Walk";
        public string turnAnimation = "Turn";
        public string salute = "Salute";

        private string _currentAnimation;

        private PeterState _state;

        private float _remaining;
        private Quaternion _startRotation;
        private Quaternion _targetRotation;


        private void Start()
        {
            _state = PeterState.Saluting;
            
            PlayAnimation(salute);
        }

        private void PlayAnimation(string playMe, bool crossFade = true)
        {
            if (crossFade)
            {
                animator.CrossFade(playMe, 0.25f);    
            }
            else
            {
                animator.Play(playMe);    
            }
            
            _currentAnimation = playMe;
        }

        private void Update()
        {
            switch (_state)
            {
                case PeterState.Walking:
                    Walking();
                    break;
                case PeterState.Saluting:
                    Saluting();
                    break;
                case PeterState.Turning:
                    Turning();
                    break;
            }
        }

        private void Saluting()
        {
            if (_currentAnimation != salute)
            {
                PlayAnimation(salute);
                return;
            }
            
            var progress = animator.IsInTransition(0)
                ? animator.GetNextAnimatorStateInfo(0).normalizedTime
                : animator.GetCurrentAnimatorStateInfo(0).normalizedTime;
            
            if (progress >= 1)
            {
                _state = PeterState.Walking;
                _remaining = walkDistance;
            }
        }

        private void Turning()
        {
            if (_currentAnimation != turnAnimation)
            {
                PlayAnimation(turnAnimation);
                return;
            }

            var progress = animator.IsInTransition(0)
                ? animator.GetNextAnimatorStateInfo(0).normalizedTime
                : animator.GetCurrentAnimatorStateInfo(0).normalizedTime;

            if (progress >= 1f)
            {
                _state = PeterState.Saluting;
            }
            
            transform.rotation = Quaternion.Slerp(_startRotation, _targetRotation, progress);
        }

        private void Walking()
        {
            if (_remaining <= 0)
            {
                _state = PeterState.Turning;
                _startRotation = transform.rotation;
                _targetRotation = _startRotation * Quaternion.Euler(0, -90f, 0);
                return;
            }

            if (_currentAnimation != walkAnimation)
            {
                PlayAnimation(walkAnimation, false);
            }

            transform.Translate(Vector3.forward * (walkSpeed * Time.deltaTime));
            _remaining -= walkSpeed * Time.deltaTime;
        }
    }
}