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
        }
        
        private void DrawLink()
        {
            if (!IsInChain || Parent != null)
            {
                lineRenderer.enabled = false;
                return;
            }

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

            spriteRenderer.sprite = config.sprite;
            spriteRenderer.color = noteInfo.Color;

            note = noteInfo.Note;
            noteText.text = note;
            
            if (linked)
            {
                var request = new LinkedEnemyRequest(
                    this,
                    noteInfo.StringNum,
                    noteInfo.Fret,
                    linkedNum
                );

                GameEvents.SpawnLinkedEnemyRequest(request);
            }
        }
        
        public void SetupLinkedChild(EnemyType config, EnemyNoteInfo noteInfo, EnemyController parent)
        {
            ResetValues();

            health = config.health;
            speed = config.speed;
            linked = true;

            spriteRenderer.sprite = config.sprite;
            spriteRenderer.color = noteInfo.Color;

            note = noteInfo.Note;
            noteText.text = note;

            Parent = parent;
            parent.Child = this;
        }
        
        private void CheckDamage(string detectedNote)
        {
            if (IsDead) return;
            if (detectedNote != note) return;
            
            if (IsInChain)
            {
                KillInChain();
                return;
            }

            // Enemigo normal
            health--;
            if (health <= 0)
            {
                Kill();
            }
        }
        
        private void Kill()
        {
            IsDead = true;
            GameEvents.EnemyKilled();
            Despawn();
        }
        
        private void KillInChain()
        {
            IsDead = true;
            
            spriteRenderer.enabled = false;
            noteText.enabled = false;

            GameEvents.EnemyKilled();

            if (IsChainComplete())
            {
                KillEntireChain();
            }
        }
        
        private bool IsChainComplete()
        {
            var head = GetChainHead();
            var current = head;

            while (current != null)
            {
                if (!current.IsDead)
                    return false;

                current = current.Child;
            }

            return true;
        }

        private EnemyController GetChainHead()
        {
            var node = this;
            while (node.Parent != null)
            {
                node = node.Parent;
            }

            return node;
        }
        
        private void KillEntireChain()
        {
            var head = GetChainHead();
            var current = head;

            while (current != null)
            {
                var next = current.Child;
                current.Despawn();
                current = next;
            }
        }
        
        private void Despawn()
        {
            ResetValues();
            ObjectPoolManager.ReturnObjectToPool(gameObject);
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
            IsDead = false;

            spriteRenderer.enabled = true;
            spriteRenderer.color = Color.white;

            noteText.enabled = true;
            noteText.text = string.Empty;

            lineRenderer.positionCount = 0;
            lineRenderer.enabled = false;
        }
        
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!other.CompareTag("Limit")) return;
            
            if (IsInChain)
            {
                if (Parent == null)
                {
                    GameEvents.EnemyDamage();
                    KillEntireChain();
                }
            }
            else
            {
                // Enemigo normal: daño y despawn
                GameEvents.EnemyDamage();
                Despawn();
            }
        }
    }
}
