using System;
using Unity.VisualScripting;
using UnityEngine;

namespace Assets.Scripts.SceneScript.CharacterPick
{
    public class ReadyText : MonoBehaviour
    {
        // テキスト
        [SerializeField] Transform textTransform;
        [SerializeField] float moveSpeed = 2.0f;
        public Vector3 textStartPos;
        Vector3 myPos;

        // 背景
        [SerializeField] Transform backTransform;

        void Start()
        {
            myPos = transform.position;
            textStartPos = textTransform.position;
        }

        void Update()
        {
            Move();
            Scaling();
        }

        bool Move()
        {
            textTransform.position = Vector3.Lerp(textStartPos, myPos, Time.time * moveSpeed);
            return Vector3.Distance(textTransform.position, myPos) < 0.01;
        }

        bool Scaling()
        {
            backTransform.localScale = Vector3.Lerp(new(1.0f, 0.0f, 1.0f), new(1.0f, 1.0f, 1.0f), Time.time * moveSpeed);
            return Vector3.Distance(backTransform.localScale, new(1.0f, 1.0f, 1.0f)) < 0.01;
        }
    }
}