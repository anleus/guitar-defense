using System;
using System.Collections.Generic;
using Events;
using Models;
using TMPro;
using UnityEngine;

namespace Core.Enemy
{
    public class EnemyController : MonoBehaviour
    {
        private string note;
        private int health;
        private float speed;
        private bool linked;
        private int linkedNum;

        private EnemyController Parent { get; set; }
        private EnemyController Child { get; set; }
        private int OrderInChain { get; set; }
        private bool CanBeKilled { get; set; }
        private bool IsDead { get; set; }
        public bool IsInChain => Parent != null || Child != null;
        
        [Header("References")]
        [SerializeField] private SpriteRenderer spriteRenderer;
        [SerializeField] private TMP_Text noteText;
        [SerializeField] private LineRenderer lineRenderer;

        private void OnEnable()
        {
            AudioEvents.OnNoteDetected += CheckDamage;
        }

        private void OnDisable()
        {
            AudioEvents.OnNoteDetected -= CheckDamage;
        }

        private void Update()
        {
            Move();
            DrawLink();
        }

        private void Move()
        {
            transform.Translate(Vector3.down * (speed * Time.deltaTime));
            //transform.position += Vector3.down * (speed * Time.deltaTime);
        }

        private void DrawLink()
        {
            if (Parent != null) return;
            
            var positions = new List<Vector3>();
            var current = this;

            while (current != null)
            {
                positions.Add(current.transform.position);
                current = current.Child;
            }
            lineRenderer.enabled = positions.Count > 1;
            lineRenderer.positionCount = positions.Count;
            lineRenderer.SetPositions(positions.ToArray());
        }
        
        public void Setup(EnemyType config, EnemyNoteInfo noteInfo)
        {
            ResetValues();
            
            health = config.health;
            speed = config.speed;
            
            linked = config.linked;
            linkedNum = config.linkedNum;
            
            //spriteRenderer.sprite = config.sprite; 
            spriteRenderer.color = noteInfo.Color;
            
            note = noteInfo.Note;
            noteText.text = note;

            OrderInChain = 0;
            CanBeKilled = true;

            if (linked)
            {
                var request = new LinkedEnemyRequest(this, noteInfo.StringNum, noteInfo.StringNum, linkedNum);
                GameEvents.SpawnLinkedEnemyRequest(request);
            }
        }

        public void SetupLinkedChild(EnemyType config, EnemyNoteInfo noteInfo, EnemyController parent)
        {
            ResetValues();

            health = config.health;
            speed = config.speed;
            linked = true;

            spriteRenderer.color = noteInfo.Color;
            note = noteInfo.Note;
            noteText.text = note;

            Parent = parent;
            OrderInChain = parent.OrderInChain + 1;
            CanBeKilled = false;
            
            parent.Child = this;
        }
        
        private void CheckDamage(string detectedNote)
        {
            if (IsDead) return;

            var correctNote = detectedNote == note;

            if (IsInChain)
            {
                if (!correctNote || !CanBeKilled) { ResetChain(); return; }

                HandleChainKill();
                return;
            }

            if (correctNote)
            {
                health--;
                if (health <= 0) Kill();
            }
        }
        
        private void ResetValues()
        {
            note = string.Empty;
            health = 0;
            speed = 0f;
            linked = false;
            linkedNum = -1;
            
            Parent = null;
            Child = null;
            OrderInChain = -1;
            CanBeKilled = false;
            IsDead = false;
            
            spriteRenderer.enabled = true;
            spriteRenderer.color = Color.white;
            noteText.enabled = true;
            noteText.text = string.Empty;
            
            lineRenderer.positionCount = 0;
            lineRenderer.enabled = false;
        }
        
        private void Kill()
        {
            GameEvents.EnemyKilled();
            Despawn();
        }
        
        private void HandleChainKill()
        {
            IsDead = true;

            spriteRenderer.enabled = false;
            noteText.enabled = false;

            if (Child != null)
            {
                Child.CanBeKilled = true;
                return;
            }

            ChainComplete();
        }
        
        private void Despawn()
        {
            ResetValues();
            ObjectPoolManager.ReturnObjectToPool(this.gameObject);
        }

        private void ResetChain()
        {
            var head = GetChainRoot();
            var current = head;

            while (current != null)
            {
                current.IsDead = false;
                current.CanBeKilled = current.OrderInChain == 0;
                current.health =  1;
                
                current.ResetVisuals();

                current = current.Child;
            }
        }
        
        private void ChainComplete()
        {
            var head = GetChainRoot();
            var current = head;

            while (current != null)
            {
                var next = current.Child;
                current.Despawn();
                current = next;
                GameEvents.EnemyKilled();
            }
        }

        private EnemyController GetChainRoot()
        {
            var root = this;
            while (root.Parent != null)
            {
                root = root.Parent;
            }

            return root;
        }

        private void ResetVisuals()
        {
            
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Limit"))
            {
                GameEvents.EnemyDamage();
                Despawn();
            }
        }
    }
}
