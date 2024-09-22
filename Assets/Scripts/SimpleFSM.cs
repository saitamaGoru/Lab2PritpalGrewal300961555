using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public enum EnemySimpleFSMStates
{
    Patrol,
    Chase,
    Attack,
    FleeToHQ
}

public class SimpleFSM : MonoBehaviour
{
    [SerializeField] private EnemySimpleFSMStates _currentState;
    [SerializeField] private GameObject _player;
    [SerializeField] private NavMeshAgent _agent;
    [SerializeField] private GameObject _patrolArea;
    [SerializeField] private List<Transform> _patrollingLocations = new List<Transform>();
    [SerializeField] private int _index = 0;
    [SerializeField] private int _lastLocation;

    [Header("Guard HQ")]
    [SerializeField] private Transform HQ;
    
    [Header("Guard Stats")]
    [SerializeField, Range(8f, 15f)] 
    private float _distanceToChase = 10f;

    [SerializeField, Range(0.12f, 0.80f)]
    private float _fieldOfView = 0.28f;

    [SerializeField, Range(2f, 7f)]
    private float _distanceToAttack = 3f;

    [SerializeField] private bool _isInFront;
    [SerializeField] private float _restTime = 0f ;    
    void Start()
    {
        _restTime = 0f;
        _index = 0;
        HQ = GameObject.FindWithTag("HQ").GetComponent<Transform>();
        _agent = GetComponent<NavMeshAgent>();
        _currentState = EnemySimpleFSMStates.Patrol;
        _player = GameObject.FindWithTag("Player");
        _patrollingLocations = _patrolArea.GetComponentsInChildren<Transform>().ToList();
        _lastLocation = _patrollingLocations.Count - 9;
    }

    void Update()
    {
        FiniteStateMachineRunner();
    }
    private void FiniteStateMachineRunner()
    {
        switch (_currentState)
        {
            case EnemySimpleFSMStates.Patrol:
                Patrol();
                break;
            case EnemySimpleFSMStates.Chase:
                Chase();
                break;
            case EnemySimpleFSMStates.Attack:
                Attack();
                break;
            case EnemySimpleFSMStates.FleeToHQ:
                FleeToHQ();
                break;
            default: 
                Debug.LogError("State in FSM not implemented");
                break;
        }
    }

    private void Patrol()
    {
        Debug.Log("Patrolling...");
        Vector3 playerPosInRelationToGuard = _player.transform.position - transform.position;
        float distanceToPlayer = playerPosInRelationToGuard.magnitude;
        Vector3 directionToPlayer = playerPosInRelationToGuard / distanceToPlayer;

        Vector3 destination = _patrollingLocations[_index].position;
        _agent.SetDestination(destination);

        
        if(Vector3.Distance(transform.position, destination) < 2.0f)
        {
            _index = (_index + 1) % _patrollingLocations.Count;

            if(_lastLocation == _index)
            {
                _currentState = EnemySimpleFSMStates.FleeToHQ;
            }
            //Debug.Log($"Index: {_index}");
        }
        _isInFront = Vector3.Dot(transform.forward, directionToPlayer) > _fieldOfView;
        if (_isInFront && distanceToPlayer < _distanceToChase)
        {
            _currentState = EnemySimpleFSMStates.Chase;
        }
    }
    private void Chase()
    {
        _agent.SetDestination(_player.transform.position);
        Debug.Log("CHAAAASEEEE....");
        if(Vector3.Distance(transform.position, _player.transform.position) > 4.0f)
        {
            _currentState = EnemySimpleFSMStates.Patrol;
        }
    }
    private void Attack() { }

    //LAB3 PritpalGrewal 300961555
    private void FleeToHQ()
    {
        Debug.Log("Flee To HQ.......");
        Vector3 hqdestintion = HQ.position;
        _agent.SetDestination(hqdestintion);
        Debug.Log("Flee To HQ.......");
        _restTime += (float)1/1000;
        if(_restTime > 3f)
        {
           _restTime = 0f; 
            Debug.Log("Continuing Patrol.......");
            _currentState = EnemySimpleFSMStates.Patrol;
        }
    }
}
