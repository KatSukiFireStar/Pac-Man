using System.ComponentModel;
using UnityEngine;
using EventSystem.SO;
using UnityEngine.Tilemaps;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMovement : MonoBehaviour
{
    private Rigidbody2D rb;
    private Vector2 direction;
    private Vector2 nextDirection = Vector2.zero;
    private bool endGame = false;

    [SerializeField]
    private int speed = 1;
    [SerializeField]
    private LayerMask obstacleLayer;
    [SerializeField] 
    private Vector2EventSO directionEvent;
    [SerializeField]
    private GameStateEventSO gameStateEvent;
    [SerializeField]
    private Vector2EventSO positionEvent;
    [SerializeField]
    private GameObjectEventSO ghostEvent;
    
    private void Awake()
    {
        positionEvent.Value = transform.position;
        rb = GetComponent<Rigidbody2D>();
        directionEvent.PropertyChanged += DirectionEvent_PropertyChanged;
        gameStateEvent.PropertyChanged += GameStateEventOnPropertyChanged;
        ghostEvent.PropertyChanged += GhostEventOnPropertyChanged;
    }

    private void GhostEventOnPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
        Vector2 position = new(Mathf.Round(transform.position.x), Mathf.Round(transform.position.y));
        positionEvent.Value = position;
    }

    private void GameStateEventOnPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
        GenericEventSO<GameState> s = (GenericEventSO<GameState>)sender;
        if (s.Value == GameState.EndGame || s.Value == GameState.Death)
        {
            endGame = true;
        }else if (s.Value == GameState.Starting)
        {
            endGame = false;
            positionEvent.Value = transform.position;
        }
    }

    private void DirectionEvent_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        GenericEventSO<Vector2> s = (GenericEventSO<Vector2>)sender;
        SetDirection(s.Value);
    }

    void Update()
    {
        if (nextDirection != Vector2.zero)
        {
            SetDirection(nextDirection);
        }
    }

    private void FixedUpdate()
    {
        Vector2 position = rb.position;
        Vector2 translation = speed * Time.fixedDeltaTime * direction;
        
        float angle = Mathf.Atan2(direction.y, direction.x);
        transform.rotation = Quaternion.AngleAxis(angle * Mathf.Rad2Deg, Vector3.forward);

        if (!endGame)
        {
            Vector2 nextPosition = position + translation;
            rb.MovePosition(nextPosition);
            Vector2 nP = new(Mathf.Round(nextPosition.x), Mathf.Round(nextPosition.y));
            positionEvent.Value = nP;
        }
            
    }

    private void SetDirection(Vector2 dir)
    {
        if (!Occupied(dir))
        {
            direction = dir;
            nextDirection = Vector2.zero;
        }
        else
        {
            nextDirection = dir;
        }
    }

    private bool Occupied(Vector2 dir)
    {
        RaycastHit2D hit = Physics2D.BoxCast(transform.position, Vector2.one * 0.75f, 0f, dir, 1.5f, obstacleLayer);
        //Debug.Log(hit.collider != null);
        return hit.collider != null;
    }
    
}
