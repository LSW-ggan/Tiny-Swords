using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PlayerDataManager : MonoBehaviour {
    public static PlayerDataManager Instance;

    public event Action OnGoldChanged;
    public event Action OnHpChanged;
    public event Action OnStatChanged;

    private int _attackPower = 25;
    private float _speed = 3.0f;
    private int _maxHp = 100;
    private int _currentHp;
    private int _gold = 0;

    public int AttackPower {
        get {
            return _attackPower;
        }
        set {
            _attackPower = value;
            OnStatChanged?.Invoke();
        }
    }

    public float Speed {
        get { 
            return _speed;
        }
        set {
            _speed = value;
            OnStatChanged?.Invoke();
        }
    }

    public int MaxHp { 
        get {
            return _maxHp;
        }
    }

    public int Hp {
        get {
            return _currentHp;
        }
        set {
            _currentHp = value;
            OnHpChanged?.Invoke();
        } 
    }

    public int Gold {
        get {
            return _gold;
        }
        set {
            _gold = value;
            OnGoldChanged?.Invoke();
        }
    }

    private void Awake() {
        _currentHp = _maxHp;
        if (Instance != null) {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }


}
