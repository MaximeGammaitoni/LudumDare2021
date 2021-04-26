using System;
using System.Collections;
using UnityEngine;

namespace UnityStandardAssets._2D
{
    public class Camera2DFollow : MonoBehaviour
    {
        public Transform target;
        public float damping = 1;
        public float lookAheadFactor = 3;
        public float lookAheadReturnSpeed = 0.5f;
        public float lookAheadMoveThreshold = 0.1f;

        public float offsetY = 0;
        public float offsetZ = 0;

        private Vector3 m_LastTargetPosition;
        private Vector3 m_CurrentVelocity;
        private Vector3 m_LookAheadPos;
        private bool canFollow = true;
        private GameObject player;
        // Use this for initialization
        private void Start()
        {
            var offset = new Vector3(0, 0, 0);
            m_LastTargetPosition = target.position + offset;
            transform.parent = null;
            EventsManager.StartListening(nameof(StatesEvents.OnLandingIn),ChangeTarget);
            EventsManager.StartListening(nameof(StatesEvents.OnFallingIn), StartAnim);
            UpdateTarget();
            player = GameObject.Find("Player");
        }

        void UpdateTarget()
        {
            Transform newTarget = GameManager.singleton.LevelsManager.CurrentLevel.transform.Find("CameraTarget");
            if (newTarget != null)
            {
                target = newTarget;
            }
        }

        private void ChangeTarget(Args args)
        {
            //target =  GameManager.singleton.LevelsManager.CurrentLevel.transform.Find("CameraTarget");
            canFollow = true;
        }
        private void StartAnim(Args args)
        {
            canFollow = false;
            StartCoroutine(Anim());
        }
        
        IEnumerator Anim()
        {
            target = null;
            float interpolation = 0;
            var initialRotation = transform.rotation;
            while ( new Vector3(gameObject.transform.position.x,0, gameObject.transform.position.z) != new Vector3(player.transform.position.x,0, player.transform.position.z))
            {

                interpolation += Time.deltaTime;
                gameObject.transform.rotation = Quaternion.Lerp(gameObject.transform.rotation, Quaternion.Euler(90, 0, 0),interpolation);
                gameObject.transform.position = Vector3.Lerp(gameObject.transform.position, new Vector3(player.transform.position.x, gameObject.transform.position.y, player.transform.position.z), interpolation);
                
                yield return null;
                
            }
            interpolation = 0f;
            UpdateTarget();
            while (gameObject.transform.position.y >= target.transform.position.y)
            {

                interpolation += Time.deltaTime*1.2f;
                
                gameObject.transform.position = Vector3.Lerp(gameObject.transform.position, player.transform.position, interpolation);
                yield return null;
            }
            while (initialRotation!= transform.rotation)
            {

                interpolation += Time.deltaTime * 0.5f;
                gameObject.transform.rotation = Quaternion.Lerp(gameObject.transform.rotation, initialRotation, interpolation);
                gameObject.transform.position = Vector3.Lerp(gameObject.transform.position, target.transform.position, interpolation/5);
                yield return null;
            }
            yield return 0;
        }
        // Update is called once per frame
        private void FixedUpdate()
        {
            if (!canFollow)
            {
                return;
            }
            var offset = new Vector3(0, offsetY, offsetZ);
            // only update lookahead pos if accelerating or changed direction
            float xMoveDelta = (target.position - m_LastTargetPosition).x;

            bool updateLookAheadTarget = Mathf.Abs(xMoveDelta) > lookAheadMoveThreshold;

            if (updateLookAheadTarget)
            {
                m_LookAheadPos = lookAheadFactor*Vector3.right*Mathf.Sign(xMoveDelta);
            }
            else
            {
                m_LookAheadPos = Vector3.MoveTowards(m_LookAheadPos, Vector3.zero, Time.deltaTime*lookAheadReturnSpeed);
            }

            Vector3 aheadTargetPos = target.position+ offset + m_LookAheadPos + Vector3.forward;
            Vector3 newPos = Vector3.SmoothDamp(transform.position, aheadTargetPos, ref m_CurrentVelocity, damping);

            transform.position = newPos;

            m_LastTargetPosition = target.position;
        }
    }
}
