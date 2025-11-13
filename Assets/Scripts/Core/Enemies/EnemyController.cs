using System.Collections.Generic;
using Models;
using TMPro;
using UnityEngine;

namespace Core.Enemies
{
    public class EnemyController : MonoBehaviour
    {
        private int health;
        private float speed;
        private bool linked;
        private int linkedNum;
        
        [SerializeField] private SpriteRenderer spriteRenderer;
        [SerializeField] private TMP_Text note;

        private void Update()
        {
            Move();
        }

        private void Move()
        {
            transform.position += Vector3.down * (speed * Time.deltaTime);
        }
        
        public void Setup(EnemyType configuration, EnemyNoteInfo noteInfo)
        {
            health = configuration.health;
            speed = configuration.speed;
            linked = configuration.linked;
            linkedNum = configuration.linkedNum;
            
            if (configuration.sprite !=  null)
            {
                spriteRenderer.sprite = configuration.sprite; 
            }
            spriteRenderer.color = noteInfo.Color;
            note.text = noteInfo.Note;
        }
    }
}
